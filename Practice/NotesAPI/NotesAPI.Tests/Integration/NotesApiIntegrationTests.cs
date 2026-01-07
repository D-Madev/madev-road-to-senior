namespace NotesAPI.Tests.Integration;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http;
using NotesAPI.Models;
using System.Net;
using System.Text;
using NotesAPI;
using Xunit;

// IClassFixture asegura que la factoría se inicializa una vez por clase.
public class NotesApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    // Opciones para deserializar JSON (ignorando mayúsculas/minúsculas)
    private readonly JsonSerializerOptions _jsonOptions;

    public NotesApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        // Crea un cliente HTTP para interactuar con la aplicación en memoria.
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (_client.DefaultRequestHeaders.Authorization == null)
        {
            var loginRequest = new { username = "testuser", password = "password" };
            
            // Usamos PostAsJsonAsync que es más seguro que StringContent manual
            var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);
            
            // ESTO ES CLAVE: Si el login falla, el test debe morir aquí con el detalle
            if (!response.IsSuccessStatusCode)
            {
                var detail = await response.Content.ReadAsStringAsync();
                throw new Exception($"EL LOGIN FALLÓ: Status {response.StatusCode}. Detalle: {detail}");
            }

            var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>(_jsonOptions);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData!.Token);
        }
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
        await EnsureAuthenticatedAsync();

        var note = new Note { Title = title, Content = string.IsNullOrEmpty(content)? "" : content };
        
        var response = await _client.PostAsJsonAsync("/notes", note);
        response.EnsureSuccessStatusCode();

        // Deserializa la respuesta para obtener ID generado por la DB.
        var createdNote = await response.Content.ReadFromJsonAsync<Note>(_jsonOptions);
        return createdNote!;
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
        await EnsureAuthenticatedAsync();
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
        await EnsureAuthenticatedAsync();
        // Act
        var response = await _client.GetAsync($"/notes/99999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostNote_Returns201CreatedAndLocationHeader()
    {
        await EnsureAuthenticatedAsync();
        // ARRANGE
        var newNote = new Note { Title = "E2E Test Note", Content = "Full pipeline check" };
        
        // ACT - Usando el método de extensión de System.Net.Http.Json
        var response = await _client.PostAsJsonAsync("/notes", newNote);

        // ASSERT
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // Aquí es donde necesitabas el ReadFromJsonAsync
        var createdNote = await response.Content.ReadFromJsonAsync<Note>(_jsonOptions);
        
        
        // Verificamos que se pueda recuperar
        var getResponse = await _client.GetAsync($"/notes/{createdNote!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenNoteIsUpdated()
    {
        await EnsureAuthenticatedAsync();
        // Arrange
        var createdNote = await CreateNoteAsync("Note to Update", "Initial Content");

        // Preparar datos para actualizar
        var updateData = new Note 
        { 
            Id = createdNote.Id, 
            Title = "Updated Title", 
            Content = "Updated Content" 
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/notes/{createdNote.Id}", updateData);

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
        await EnsureAuthenticatedAsync();
        // ARRANGE
        var note = new Note { Title = "Test", Content = "Test" };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(note), 
            Encoding.UTF8, 
            "application/json"
        );

        // ACT: ID en URL (x+10) != ID en Body (x)
        var response = await _client.PutAsync($"/notes/{note.Id+10}", jsonContent);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenNoteIsDeleted()
    {
        await EnsureAuthenticatedAsync();
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
