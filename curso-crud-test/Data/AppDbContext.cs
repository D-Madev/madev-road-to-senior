namespace CRUD_TEST.Data;

using Microsoft.EntityFrameworkCore;
using CRUD_TEST.Models;

public class AppDbContext : DbContext
{
  // Tablas de la BD
  public DbSet<Item> Items { get; set; }
  public DbSet<SerialNumber> SerialNumbers { get; set; }
  public DbSet<Category> Categories { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Relation a lot * entity Items have 1 Category  
    modelBuilder.Entity<Category>()
      .HasMany(i => i.Items)
      .WithOne(c => c.Category)
      .HasForeignKey(c => c.CategoryId); 

    // Initialize Categories table.
    modelBuilder.Entity<Category>().HasData(
      new Category() { Id = 1, Name = "Lacteos" },
      new Category() { Id = 2, Name = "Carniceria" },
      new Category() { Id = 3, Name = "Verduleria" }
    );
    // Initialize SerialNumbers table.
    modelBuilder.Entity<SerialNumber>().HasData(
      new SerialNumber() { Id = 1, Name = "SN00001", Description = "No sabe lo que esta esta carne." },
      new SerialNumber() { Id = 2, Name = "SN00001", Description = "Lecha del mejor tambo de bs." },
      new SerialNumber() { Id = 3, Name = "SN00001", Description = "Las naranjas mas sabrosas de todas." }
    );
    modelBuilder.Entity<Item>().HasData(
      new Item() { Id = 1, Name = "Naranja", Price = 18.4, SerialNumberId = 3, CategoryId = 3 },
      new Item() { Id = 2, Name = "Leche", Price = 12.2, SerialNumberId = 2, CategoryId = 1 },
      new Item() { Id = 3, Name = "Asado", Price = 3.55, SerialNumberId = 1, CategoryId = 2 }
    );
    // Initialize Items table.
    base.OnModelCreating(modelBuilder);
  }
}