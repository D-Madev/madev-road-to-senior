using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using NotesAPI.Data;
using NotesAPI.IRepository;
using NotesAPI.Repository;
using NotesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Usar la implementación con datos estaticos.
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