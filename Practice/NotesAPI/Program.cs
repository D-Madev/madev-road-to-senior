using Microsoft.EntityFrameworkCore;
using Serilog;
using NotesAPI.Data;
using NotesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

/*
 * Explicación del Lifetime (Q19): El método AddDbContext automáticamente registra 
 * el NotesDbContext con el Lifetime Scoped. Esto garantiza que cada nueva petición 
 * HTTP obtenga una nueva conexión a la DB, evitando conflictos entre usuarios.
*/
builder.Services.AddDbContext<NotesDbContext>(options 
    => options.UseInMemoryDatabase("NotesDb"));

// Revisar salud de la API
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotesDbContext>(name: "NotesDb");

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

    /* Sembramos datos y contexto */
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<NotesDbContext>();

        // Llama al método estático que creamos
        NotesDbContext.Initialize(services);
    }

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

                // 4. Devuelve un mensaje genérico al cliente
                await context.Response.WriteAsJsonAsync(new
                {
                    // En producción solo se devuelve un mensaje genérico.
                    Message = "Ocurrió un error inesperado en el servidor."
                });
            }
        });
    });

    if (app.Environment.IsDevelopment()) app.MapOpenApi();

    app.UseHttpsRedirection();
    app.MapControllers();
    app.UseCors("Origins");

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