namespace NotesAPI.Tests.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NotesAPI.Services;
using NotesAPI.Models;
using NotesAPI.Data;
using System.Linq;
using System;
using Xunit;
using Moq;

public class NotesServiceTests : IDisposable
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly NotesDbContext _context;
    private readonly NotesService _service;

    public NotesServiceTests()
    {
        // Creamos un contenedor de servicios aislado para este test
        var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

        // Configurar la BD
        // Usamos un GUID único para que cada test tenga una DB limpia y aislada.
        var options = new DbContextOptionsBuilder<NotesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        _cacheMock = new Mock<IDistributedCache>();
        // Instanciar el contexto y el servicio
        _context = new NotesDbContext(options);
        
        // Asegura que la DB está limpia antes de CADA test
        _context.Database.EnsureCreated();
        
        // Limpiar los datos del Seeding (Task 1, Task 2) 
        // para que la base de datos empiece REALMENTE vacía.
        if (_context.Notes.Any())
        {
            _context.Notes.RemoveRange(_context.Notes);
            _context.SaveChanges();
        }

        _service = new NotesService(_context, _cacheMock.Object);
    }

    // IDisposable: Se ejecuta luego de cada Test.
    // Cierra la conexión y limpia la BD en memoria.
    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoNotesExist()
    {
        // Arrange: La BD ya está vacía por la limpieza del constructor.

        // Act
        var notes = await _service.GetAllAsync();

        // Assert
        // Verifica que retorna una lista vacía, no null.
        Assert.Empty(notes);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllNotes()
    {
        // Arrange
        _context.Notes.Add(new Note { Title = "Note 1", Content = "Content 1" });
        _context.Notes.Add(new Note { Title = "Note 2", Content = "Content 2" });
        await _context.SaveChangesAsync();

        // Act
        var notes = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, notes.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNote_WhenIdIsValidAndExists()
    {
        // Arrange
        var noteToFind = new Note { Title = "Test Note", Content = "Content" };
        _context.Notes.Add(noteToFind);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(noteToFind.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(noteToFind.Id, result.Id);
        Assert.Equal("Test Note", result.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_ReturnsNull_WhenIdIsInvalid(int invalidId)
    {
        // Act
        var result = await _service.GetByIdAsync(invalidId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNoteDoesNotExist()
    {
        // Arrange: La BD está vacía.

        // Act
        // ID inexistente
        var result = await _service.GetByIdAsync(999); 

        // Assert
        // Esperamos null por FindAsync que devuelve null si no encuentra.
        Assert.Null(result); 
    }

    [Fact]
    public async Task CreateAsync_AddNoteToDatabase()
    {
        // Arrange
        var newNote = new Note { Title = "New Note", Content = "New Content" };

        // Act
        var createdNote = await _service.CreateAsync(newNote);

        // Assert 
        // Verificar que la nota realmente se insertó en la DB en memoria.
        var noteInDb = await _context.Notes.FirstOrDefaultAsync(n => n.Title == "New Note");

        Assert.NotNull(noteInDb);
        Assert.Equal(newNote.Title, noteInDb.Title);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenNewNoteIsNull()
    {
        // Arrange
        Note? newNote = null;

        // Act
        // Usamos ! para evitar advertencia de null.
        var createdNote = await _service.CreateAsync(newNote!);

        // Assert
        // Esperamos null por la validación: if (newNote == null)
        Assert.Null(createdNote);

        // Opcional: Asegurarse de que no se guardó nada en la DB
        Assert.Empty(await _context.Notes.ToListAsync());
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrueAndUpdatesNote_WhenSuccessful()
    {
        // Arrange
        var entry = _context.Notes.Add(new Note { Title = "Old Title", Content = "Old Content" });
        await _context.SaveChangesAsync();
        
        var validId = entry.Entity.Id;

        // Desconectar para simular un update.
        _context.Entry(entry.Entity).State = EntityState.Detached; 

        var noteUpdate = new Note { Id = validId, Title = "New Title", Content = "New Content" };

        // Act
        var result = await _service.UpdateAsync(validId, noteUpdate);

        // Assert
        // Verifica que la operación fue exitosa (devuelve true)
        Assert.True(result); 

        // Verifica que los cambios se reflejan en la DB
        var updatedNote = await _context.Notes.FindAsync(validId);
        Assert.NotNull(updatedNote);
        Assert.Equal("New Title", updatedNote.Title);
        Assert.Equal("New Content", updatedNote.Content);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task UpdateAsync_ReturnsFalse_WhenIdIsInvalid(int invalidId)
    {
        // Arrange
        var noteUpdate = new Note { Id = 1, Title = "Title" };

        // Act
        var result = await _service.UpdateAsync(invalidId, noteUpdate);

        // Assert
        // Esperamos false por la validación: if (id <= 0)
        Assert.False(result); 

        // Opcional: Asegurarse de que no se guardó nada (la DB sigue vacía)
        Assert.Empty(await _context.Notes.ToListAsync());
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenUpdateNoteIsNull()
    {
        // Arrange
        Note? noteUpdate = null;
        int validId = 1;

        // Act
        var result = await _service.UpdateAsync(validId, noteUpdate!);

        // Assert
        // Esperamos false por la validación: if (noteUpdate == null)
        Assert.False(result); 
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNoteDoesNotExist()
    {
        // Arrange: La DB está vacía.
        var noteUpdate = new Note { Id = 99, Title = "New Title" };

        // Act
        var result = await _service.UpdateAsync(99, noteUpdate);

        // Assert
        // Esperamos false porque FindAsync devuelve null.
        Assert.False(result);
        // Confirma que no se agregó nada.
        Assert.Empty(await _context.Notes.ToListAsync()); 
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrueAndRemovesNote_WhenSuccessful()
    {
        // Arrange
        var entry = _context.Notes.Add(new Note { Title = "To Delete" });
        await _context.SaveChangesAsync();
        var validId = entry.Entity.Id; // ID Dinámico

        // Act
        var result = await _service.DeleteAsync(validId);

        // Assert
        Assert.True(result); // Verifica éxito

        // Verificar que la nota fue eliminada de la DB
        var noteInDb = await _context.Notes.FindAsync(validId);
        Assert.Null(noteInDb); // Debe ser null
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task DeleteAsync_ReturnsFalse_WhenIdIsInvalid(int invalidId)
    {
        // Arrange

        // Act
        var result = await _service.DeleteAsync(invalidId);

        // Assert
        // Esperamos false por la validación: if (id <= 0)
        Assert.False(result); 
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNoteDoesNotExist()
    {
        // Arrange: La DB está vacía.

        // Act
        var result = await _service.DeleteAsync(999); // ID inexistente

        // Assert
        Assert.False(result); // Esperamos false porque FindAsync devuelve null.
    }
}