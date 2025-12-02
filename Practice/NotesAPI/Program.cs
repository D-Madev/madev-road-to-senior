using Microsoft.EntityFrameworkCore;
using NotesAPI.Data;
using NotesAPI.IRepository;
using NotesAPI.Repository;

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
builder.Services.AddDbContext<NotesDbContext>(options => options.UseInMemoryDatabase("NotesDb"));

var app = builder.Build();
/* Sembramos dadtos */
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NotesDbContext>();

    // Llama al método estático que creamos
    NotesDbContext.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();