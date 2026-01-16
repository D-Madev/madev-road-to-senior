using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HealthChecks.Redis;
using NotesAPI.Services;
using NotesAPI.Data;
using System.Text;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Obtener el IConfiguration (ya disponible a través del builder)
var configuration = builder.Configuration;

// Helper para validar variables requeridas
string GetRequiredConfig(string key) => 
    Environment.GetEnvironmentVariable(key) ?? configuration[key] 
    ?? throw new InvalidOperationException($"Variable de entorno faltante: {key}");

// CONFIGURACIÓN JWT
// Buscamos la clave. Si no existe (como en el servidor de integración), 
// usamos una clave de emergencia de 32 caracteres para que la API no explote al iniciar.
var jwtKeyString = GetRequiredConfig("JWT_KEY");
var connectionString = GetRequiredConfig("DB_CONNECTION_STRING");
var redisConnectionString = GetRequiredConfig("REDIS_CONNECTION_STRING");
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "*";

var key = Encoding.UTF8.GetBytes(jwtKeyString);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Esto obliga a usar el validador compatible con JwtSecurityToken
    options.UseSecurityTokenValidators = true;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    var signingKey = new SymmetricSecurityKey(key);
    signingKey.KeyId = "NotesApiKeyId";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options => 
{
    options.AddPolicy("Origins", policy =>
    {
        policy.WithOrigins(allowedOrigins.Split(","))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

/* Usar la implementación con datos estaticos.
 * 
 * Abstracción (Desacoplamiento): El Controller solo sabía que dependía de una interfaz (IRepository), 
 * no sabía si el dato venía de una lista de la memoria, un archivo XML o una base de datos.
 * 
 * El Patrón Repository se fusionó de manera eficiente con el Patrón de Service Layer.
 */
// builder.Services.AddSingleton<INotesRepository, StaticNotesRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Definir el esquema de seguridad (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer" // El valor debe ser "bearer" (minúsculas)
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Notes API - Madev Road to Senior",
        Description = "API para la gestión de notas personales con seguridad JWT",
        Contact = new OpenApiContact
        {
            Name = "Madev",
            Email = "3matias.sm@gmail.com"
        }
    });

    // 2. Definir qué operaciones requieren este esquema
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Toma los comentarios de arriba de los métodos y los "inyecta" dentro de Swagger
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename);
    
    // Solo incluir si el archivo existe físicamente en el contenedor
    if (System.IO.File.Exists(xmlPath)) 
    {
        options.IncludeXmlComments(xmlPath);
    }
});


// Seccion de Datos e Identity
builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseNpgsql(connectionString));

// Añadimos Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<NotesDbContext>();

// REDIS
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "NotesAPI_";
});

// Revisar salud de la API
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotesDbContext>(name: "Postgres")
    .AddRedis(redisConnectionString, name: "Redis");

// Registrar el service layer
builder.Services.AddScoped<INotesService, NotesService>();

// Configurar el logger de serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Nivel minimo a registrar
    .WriteTo.Console()          //Salida a consola
    .WriteTo.File("logs/NotesAPI_.log", rollingInterval: RollingInterval.Day) // Salida a un archivo diario
    .CreateLogger();

try
{
    // Reemplaza el logger predeterminado por serilog
    builder.Host.UseSerilog();
    var app = builder.Build();

    // --- BLOQUE DE MIGRACIONES ---
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var env = services.GetRequiredService<IWebHostEnvironment>();

        for (int i = 1; i <= 5; i++)
        {
            try
            {
                var context = services.GetRequiredService<NotesDbContext>();
                
                // Si usamos In-Memory (Tests), no migramos
                if (context.Database.ProviderName?.Contains("InMemory") == false) context.Database.Migrate();
                
                logger.LogInformation("✅ Base de datos migrada con éxito.");
                break; 
            }
            catch (Exception ex)
            {
                if (i == 5) 
                {
                    logger.LogCritical(ex, "❌ Falló la migración tras 5 intentos. Apagando...");
                    throw; 
                }

                if (env.IsEnvironment("Testing")) break;

                logger.LogWarning("⚠️ Intento {Attempt}/5: Postgres no está listo. Reintentando en 2s...", i);
                Thread.Sleep(2000);
            }
        }
    }

    // Exponer el endpoint /metrics para Prometheus
    app.MapMetrics();

    // Configurar el endpoint de Health Checks (Q20)
    app.MapHealthChecks("/health");

    // GLOBAL EXCEPTION HANDLING (Q23)
    // Configura un manejador global de excepciones (Middleware)
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            // 1. Establecemos la respueta HTTP
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            // 2. Busca los detalles del error para el LOGGING (Q18)
            var exceptionHandlerPathFeature =
                context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error != null)
            {
                // 3. Registra el error (Logging)
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(exceptionHandlerPathFeature.Error, "Error global no controlado en el servidor.");

                var traceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;

                // 4. Devuelve un mensaje genérico al cliente
                await context.Response.WriteAsJsonAsync(new
                {
                    // En producción solo se devuelve un mensaje genérico.
                    Message = "Ocurrió un error inesperado en el servidor.",
                    TraceId = traceId
                });
            }
        });
    });

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI(); 
    }

    app.UseHttpsRedirection();
    app.UseCors("Origins");

    // Recolectar métricas de las peticiones HTTP automáticamente
    app.UseHttpMetrics();

    // Deben ir después de UseRouting y UseCors (si lo usas), y antes de MapControllers.
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoints automáticos de Identity (Registro, Login, etc)
    app.MapGroup("/identity").MapIdentityApi<IdentityUser>();
    app.MapControllers();

    Log.Information("✅ NotesAPI ha iniciado correctamente.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ El host de la aplicación terminó inesperadamente.");
}
finally
{
    await Log.CloseAndFlushAsync();
}