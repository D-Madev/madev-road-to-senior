namespace NotesAPI.Tests.Integration;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using NotesAPI.Data;
using System.Text;
using System;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    // Generamos el ID una sola vez por instancia de la factoría
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Asegúrate de que la ruta "Jwt:Key" coincida con la que usas en Program.cs
                ["Jwt:Key"] = "clave_secreta_de_prueba_para_tests_12345",
                ["Jwt:Issuer"] = "NotesAPI",
                ["Jwt:Audience"] = "NotesAPIUsers"
            });
        });
        // Sobreescribimos la configuracion de la app.
        builder.ConfigureServices(services =>
        {
            // Eliminar la config de DbContext de producción.
            var descriptors = services.Where(
            d => d.ServiceType.Name.Contains("NotesDbContext") || 
                 d.ServiceType.Name.Contains("DbContextOptions")).ToList();

            foreach (var d in descriptors) services.Remove(d);

            services.AddEntityFrameworkInMemoryDatabase();
            // Añadir la config de DbContext para la BD en memoria.
            services.AddDbContext<NotesDbContext>((sp, options) =>
            {
                // Generamos un nombre único para la DB en memoria.
                // Para evitar conflictos entre tests y el pipeline de integracion.
                options.UseInMemoryDatabase(_dbName)
                       .UseInternalServiceProvider(sp);
            });
            
            // Evita que la API intente conectar a Redis en el pipeline.
            var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
            if (redisDescriptor != null) services.Remove(redisDescriptor);
            services.AddDistributedMemoryCache();

            var healthCheckDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(HealthCheckService));
            if (healthCheckDescriptor != null) services.Remove(healthCheckDescriptor);
            services.AddHealthChecks();

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<NotesDbContext>();
                
                // Limpiamos CUALQUIER dato que haya venido del Program.cs
                db.Database.EnsureDeleted(); 
                db.Database.EnsureCreated();
            }
        });
    }
}