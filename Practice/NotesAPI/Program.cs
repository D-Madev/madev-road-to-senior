using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotesAPI.Data;
using NotesAPI.Services;
using Prometheus;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Obtener el IConfiguration (ya disponible a través del builder)
var configuration = builder.Configuration;

// CONFIGURACIÓN JWT
//  Leemos la clave desde la sección "Jwt:Key" de appsettings, Secret Manager o Variables de Entorno.
var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // Usamos la clave leída
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // Para este demo, no validamos el emisor
        ValidateIssuer = false,
        // Para este demo, no validamos la audiencia
        ValidateAudience = false
    };
});

var frontend = builder.Configuration.GetValue<string>("AllowedOrigins:Frontend");

if (!string.IsNullOrEmpty(frontend))
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "Origins",
            builder =>
            {
                builder.WithOrigins(frontend)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });
}

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

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

/*
 * Explicación del Lifetime (Q19): El método AddDbContext automáticamente registra 
 * el NotesDbContext con el Lifetime Scoped. Esto garantiza que cada nueva petición 
 * HTTP obtenga una nueva conexión a la DB, evitando conflictos entre usuarios.
*/
// builder.Services.AddDbContext<NotesDbContext>(options 
//     => options.UseInMemoryDatabase("NotesDb"));
// Si estamos en Docker, usaremos una variable de entorno. Si es local, appsettings.
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseNpgsql(connectionString));

var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") 
                            ?? builder.Configuration.GetConnectionString("RedisConnection") 
                            ?? "localhost:6379"; // Fallback por si te olvidás de configurarlo

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "NotesAPI_";
});

// Revisar salud de la API
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotesDbContext>(name: "Postgres")
    .AddRedis(redisConnectionString, name: "Redis");

// 11. Registrar el service layer
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

    for (int i = 0; i < 5; i++)
    {
        try
        {
            /* Sembramos datos y contexto */
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<NotesDbContext>();

            // Llama al método estático que creamos
            // NotesDbContext.Initialize(services);

            // Aplica las migraciones pendientes o crea la DB si no existe
            context.Database.Migrate();
            break; // Si tiene éxito, salimos del bucle
        }
        catch (Exception ex)
        {
            if (i == 4) throw; // Si falló 5 veces, apagamos la app
            Console.WriteLine("Postgres no está listo, reintentando en 2 segundos...");
            Thread.Sleep(2000);
        }
    }

    // Exponer el endpoint /metrics para Prometheus
    app.MapMetrics();

    // TAREA 9: Configurar el endpoint de Health Checks (Q20)
    app.MapHealthChecks("/health");

    // TAREA 8: GLOBAL EXCEPTION HANDLING (Q23)
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

    if (app.Environment.IsDevelopment())
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