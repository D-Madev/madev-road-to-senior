namespace NotesAPI.Tests.Integration;

using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NotesAPI.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using NotesAPI;

// IClassFixture asegura que la factoría se inicializa una vez por clase.
public class NotesApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    // Opciones para deserializar JSON (ignorando mayúsculas/minúsculas)
    private readonly JsonSerializerOptions _jsonOptions;
    // Campo para guardar el token jwt
    private readonly string _jwtToken;

    public NotesApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        // Crea un cliente HTTP para interactuar con la aplicación en memoria.
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jwtToken = GetJwtToken().GetAwaiter().GetResult();
        // 
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
    }

    // Clase interna para deserializar la respuesta del login
    private class TokenResponse { public string Token { get; set; } = string.Empty; }

    // Método Asíncrono para obtener el token
    private async Task<string> GetJwtToken()
    {
        var loginRequest = new
        {
            username = "testuser",
            password = "password"
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/auth/login", jsonContent);

        // Aseguramos que el login fue exitoso (200 OK)
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(content, _jsonOptions);

        return tokenData!.Token;
    }

    // Healper para crear notas
    public async Task<Note> CreateNoteAsync(string title, string? content)
    {
        var note = new Note { Title = title, Content = string.IsNullOrEmpty(content)? "" : content };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(note), 
            Encoding.UTF8, 
            "application/json"
        );
        var response = await _client.PostAsync("/notes", jsonContent);
        response.EnsureSuccessStatusCode();

        // Deserializa la respuesta para obtener ID generado por la DB.
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Note>(responseString, _jsonOptions)!;
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndListOfNotes()
    {
        // Arrange: creamos una nota para asegurar que hay algo.
        await CreateNoteAsync("Test Note", "This is a test note.");

        // Act
        var response = await _client.GetAsync("/notes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var notes = JsonSerializer.Deserialize<List<Note>>(content, _jsonOptions);

        Assert.NotNull(notes);
        Assert.NotNull(_jsonOptions);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenNotesExists()
    {
        // Arrange
        var createdNote = await CreateNoteAsync("Specific Note", "Content for specific note.");

        // Act
        var response = await _client.GetAsync($"/notes/{createdNote.Id}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var fetchedNote = JsonSerializer.Deserialize<Note>(content, _jsonOptions);

        Assert.NotNull(fetchedNote);
        Assert.Equal(createdNote.Id, fetchedNote!.Id);
        Assert.Equal("Specific Note", fetchedNote.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"/notes/99999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostNote_Returns201CreatedAndLocationHeader()
    {
        // ARRANGE
        var newNote = new Note { Title = "E2E Test Note", Content = "Full pipeline check" };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(newNote),
            Encoding.UTF8,
            "application/json"
        );

        // ACT
        var response = await _client.PostAsync("/notes", jsonContent);

        // ASSERT
        // Verifica el código HTTP (201 Created)
        // Lanza excepción si el código es 4xx o 5xx
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verifica el encabezado Location 
        Assert.NotNull(response.Headers.Location);

        // Ver si la nota realmente se guardó.
        var locationUri = response.Headers.Location;
        var getResponse = await _client.GetAsync(locationUri);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenNoteIsUpdated()
    {
        // Arrange
        var createdNote = await CreateNoteAsync("Note to Update", "Initial Content");

        // Preparar datos para actualizar
        var updateData = new Note 
        { 
            Id = createdNote.Id, 
            Title = "Updated Title", 
            Content = "Updated Content" 
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PutAsync($"/notes/{createdNote.Id}", jsonContent);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/notes/{createdNote.Id}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var updatedNoteInDb = JsonSerializer.Deserialize<Note>(content, _jsonOptions);

        Assert.Equal("Updated Title", updatedNoteInDb!.Title);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenIdMismatch()
    {
        // ARRANGE
        var note = new Note { Id = 1, Title = "Test", Content = "Test" };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(note), 
            Encoding.UTF8, 
            "application/json"
        );

        // ACT: ID en URL (5) != ID en Body (1)
        var response = await _client.PutAsync("/notes/5", jsonContent);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenNoteIsDeleted()
    {
        // Arrange
        var createdNote = await CreateNoteAsync("ToDelete", "Bye bye");

        // Act
        var response = await _client.DeleteAsync($"/notes/{createdNote.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


        var getResponse = await _client.GetAsync($"/notes/{createdNote.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
