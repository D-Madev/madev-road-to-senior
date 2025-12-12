namespace NotesAPI.Tests.Integration;

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
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Sobreescribimos la configuracion de la app.
        builder.ConfigureServices(services =>
        {
            // Eliminar la config de DbContext de producción.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<NotesDbContext>));

            if (descriptor != null) services.Remove(descriptor);

            // Añadir la config de DbContext para la BD en memoria.
            services.AddDbContext<NotesDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryNotesTest");
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<NotesDbContext>();

                // Asegurar que la DB se crea (la primera vez que se accede)
                db.Database.EnsureCreated();
            }
        });
    }
}