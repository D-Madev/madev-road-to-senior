namespace NotesAPI.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Models;

// PASADO: DbContext (Q12)
// Heredamos de IdentityDbContext en lugar de DbContext
public class NotesDbContext : IdentityDbContext<IdentityUser>
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options) { }

    // public static void Initialize(IServiceProvider serviceProvider)
    // {
    //     using (var context = new NotesDbContext(
    //         serviceProvider.GetRequiredService<DbContextOptions<NotesDbContext>>()))
    //     {
    //         // Si ya hay notas, no hacemos nada.
    //         if (context.Notes.Any()) return;

    //         // Si la tabla está vacía, añadimos notas iniciales.
    //         context.Notes.AddRange(
    //             new Note { Title = "Task 1: Complete Project Setup", Content = "Review Routing and DI." },
    //             new Note { Title = "Task 2: Implement CRUD Operations", Content = "Focus on Controllers and DbContext." },
    //             new Note { Title = "Task 3: Implement Async", Content = "Apply ToListAsync and SaveChangesAsync." }
    //         );

    //         context.SaveChanges();
    //     }
    // }

    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura las claves primarias de IdentityPasskeyData y demás tablas de Identity
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Note>().HasData(
            new Note { Id = 1, Title = "Task 1", Content = "Review Routing" },
            new Note { Id = 2, Title = "Task 2", Content = "Focus on DbContext" }
        );
    }
}
