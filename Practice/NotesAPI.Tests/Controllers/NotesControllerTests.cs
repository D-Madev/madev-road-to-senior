namespace NotesAPI.Tests.Controllers;

using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Controllers;
using NotesAPI.Services;
using NotesAPI.Models;

/* 
 * El objetivo aquí es probar que el Controller (la capa HTTP) 
 * se comporta correctamente, independientemente de si la 
 * base de datos funciona o no, que maneja correctamente las solicitudes 
 * y devuelve el código HTTP esperado, basándose en la respuesta 
 * que le da el Service mockeado.
 */
public class NotesControllerTests
{
    // Propiedades de la Clase (Accesibles para todos los tests)
    private readonly Mock<INotesService> _mockService;
    private readonly NotesController _controller;

    // Constructor: Se ejecuta ANTES de cada método [Fact].
    // Esto garantiza que CADA test obtiene un Mock de Servicio y un Controller NUEVOS y AISLADOS.
    public NotesControllerTests()
    {
        // ARRANGE Global: Se ejecuta antes de cada test.
        _mockService = new Mock<INotesService>();
        _controller = new NotesController(_mockService.Object);
    }

    // GET /notes
    [Fact]
    public async Task GetNotes_ReturnsOkResult_WithListOfNotes()
    {
        // Arrange
        var fakeNotes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Content = "Content 1" },
            new Note { Id = 2, Title = "Note 2", Content = "Content 2" }
        };

        // Setup the mock to return the fake notes
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeNotes);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert : return 200 OK with list of notes
        var okResult = Assert.IsType<OkObjectResult>(result);

        // verify the returned value is a list of notes
        var returnedNotes = Assert.IsType<List<Note>>(okResult.Value);

        // verify the count
        Assert.Equal(2, returnedNotes.Count);

        // Verify that the service method was called exactly once.
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    // GET /notes
    [Fact]
    public async Task GetNotes_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var fakeNotes = new List<Note>();

        // Setup the mock to return the fake notes
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeNotes);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert : return 200 OK with empty list 
        var okResult = Assert.IsType<OkObjectResult>(result);

        // Verfiy the returned value is a list of notes
        var returnedNotes = Assert.IsType<List<Note>>(okResult.Value);

        // Verify the count
        Assert.Empty(returnedNotes);

        // Verify that the service method was called exactly once.
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    // GET /notes/{id}
    [Fact]
    public async Task GetNote_ReturnsOkResult()
    {
        // Arrange
        var fakeNote = new Note { Id = 1, Title = "Note 1", Content = "Content 1" };

        // Setup the mock to return the fake note
        _mockService.Setup(s => s.GetByIdAsync(fakeNote.Id)).ReturnsAsync(fakeNote);

        // Act
        var result = await _controller.GetByIdAsync(fakeNote.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        // verify the returned value is a note
        var returnedNote = Assert.IsType<Note>(okResult.Value);

        // verify the note properties
        Assert.Equal(1, returnedNote.Id);

        // Verify that the service method was called exactly once.
        _mockService.Verify(s => s.GetByIdAsync(fakeNote.Id), Times.Once);
    }

    // GET /notes/{id}
    [Fact]
    public async Task GetNote_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange : Non-existing note
        var nonExistingNoteId = 1;

        // Setup the mock to return null for non-existing note
        _mockService.Setup(s => s.GetByIdAsync(nonExistingNoteId)).ReturnsAsync((Note?)null);

        // Act
        var result = await _controller.GetByIdAsync(nonExistingNoteId);

        // Assert 
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        // Verify that the service method was called exactly once.
        _mockService.Verify(s => s.GetByIdAsync(nonExistingNoteId), Times.Once);
    }

    // POST /notes
    [Fact]
    public async Task CreateNote_ReturnsCreatedResult_AndLocationHeader()
    {
        // ARRANGE 
        var newNote = new Note { Title = "New Note", Content = "New Content" };
        int fakeId = 10;

        _mockService.Setup(s => s.CreateAsync(newNote)).ReturnsAsync(
            (Note note) =>
            {
                note.Id = fakeId;
                return note;
            });

        // Mockear el ControllerContext (Necesario para construir la URL)
        _controller.ControllerContext = new ControllerContext
        {
            RouteData = new Microsoft.AspNetCore.Routing.RouteData()
        };
        _controller.ControllerContext.RouteData.Values.Add("controller", "Notes");

        // ACT
        var result = await _controller.CreateAsync(newNote);

        // ASSERT
        // Verificar que el resultado es 201 Created y que el tipo es CreatedResult.
        var createdResult = Assert.IsType<CreatedResult>(result);

        // Verificar que la URL de Location se construyó correctamente.
        Assert.Equal($"/Notes/{fakeId}", createdResult.Location);

        // Verificar que el Service fue llamado.
        _mockService.Verify(s => s.CreateAsync(newNote), Times.Once);
    }

    // POST /notes
    [Fact]
    public async Task CreateNote_ReturnsNotFound_WhenServiceReturnsNull()
    {
        // ARRANGE
        // El Service espera un objeto, pero devolverá null
        var nullNote = new Note { Title = "Title" };

        // Configurar el Mock: Le decimos al Service que falló (devuelve null).
        _mockService.Setup(s => s.CreateAsync(It.IsAny<Note>())).ReturnsAsync((Note?)null);

        // ACT
        var result = await _controller.CreateAsync(nullNote);

        // ASSERT
        // 1. Verificar que el resultado es 404 Not Found.
        Assert.IsType<NotFoundObjectResult>(result);

        // 2. Verificar que el Service fue llamado.
        _mockService.Verify(s => s.CreateAsync(It.IsAny<Note>()), Times.Once);
    }

    // PUT /notes/{id}
    [Fact]
    public async Task UpdateNote_ReturnsNoContent_WhenSuccessful()
    {
        // ARRANGE
        int idToUpdate = 1;
        var noteUpdate = new Note { Id = idToUpdate, Title = "Nuevo Título", Content = "Nuevo Contenido" };

        // Configurar el Mock: Le decimos al Service que UpdateAsync() fue exitoso (devuelve true).
        _mockService.Setup(s => s.UpdateAsync(idToUpdate, noteUpdate))
                    .ReturnsAsync(true);

        // ACT
        var result = await _controller.UpdateAsync(idToUpdate, noteUpdate);

        // ASSERT
        // Verifica que el resultado es 204 No Content.
        Assert.IsType<NoContentResult>(result);

        // Verificación (Comportamiento): Asegura que el Service fue llamado exactamente una vez.
        _mockService.Verify(s => s.UpdateAsync(idToUpdate, noteUpdate), Times.Once);
    }

    // PUT /notes/{id}
    [Fact]
    public async Task UpdateNote_ReturnsBadRequest_WhenIdsMismatch()
    {
        // Arrange
        int routeId = 1;
        var noteUpdate = new Note { Id = 2, Title = "Title", Content = "Content" };

        // Act
        var result = await _controller.UpdateAsync(routeId, noteUpdate);

        // Assert
        // Verifica que el resultado es 400 Bad Request.
        Assert.IsType<BadRequestObjectResult>(result);


        // Asegura que el Service NUNCA fue llamado.
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<Note>()), Times.Never);
    }

    // PUT /notes/{id}
    [Fact]
    public async Task UpdateNote_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        int nonExistingId = 99;
        var fakeNoteToUpdate = new Note { Id = nonExistingId, Title = "Title", Content = "Content" };

        // Configuramos el Mock : Le decimos al Service que UpdateAsync() falló (devuelve false).
        _mockService.Setup(s => s.UpdateAsync(nonExistingId, fakeNoteToUpdate)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateAsync(nonExistingId, fakeNoteToUpdate);

        // Assert
        // Verificamos que el resultado es 404 Not Found.
        Assert.IsType<NotFoundObjectResult>(result);

        // Aseguramos que el service no se llame ni una sola vez.
        _mockService.Verify(s => s.UpdateAsync(nonExistingId, fakeNoteToUpdate), Times.Once);
    }

    // DELETE /notes/{id}
    [Fact]
    public async Task DeleteNote_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        int idToDelete = 1;

        // Configuramos el Mock: Le decimos al Service que DeleteAsync() fue exitoso (devuelve true).
        _mockService.Setup(s => s.DeleteAsync(idToDelete)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAsync(idToDelete);

        // Assert
        // Verificamos que el resultado es 204 No Content.
        Assert.IsType<NoContentResult>(result);

        // Verificación (Comportamiento): Asegura que el Service fue llamado exactamente una vez.
        _mockService.Verify(s => s.DeleteAsync(idToDelete), Times.Once);
    }

    // DELETE /notes/{id}
    [Fact]
    public async Task DeleteNote_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        int fakeIdToDelete = 99;

        // Configuramos el Mock: Le decimos al Service que DeleteAsync() no fue exitoso (devuelve false).
        _mockService.Setup(s => s.DeleteAsync(fakeIdToDelete)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAsync(fakeIdToDelete);

        // Assert
        // Verificamos que el resultado es 404 not found.
        Assert.IsType<NotFoundObjectResult>(result);

        // Verificación (Comportamiento): Asegura que el Service fue llamado exactamente una vez.
        _mockService.Verify(s => s.DeleteAsync(fakeIdToDelete), Times.Once);
    }
}
