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
    private readonly CustomWebApplicationFactory<Program> _factory;

    public NotesApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        // Crea un cliente HTTP para interactuar con la aplicación en memoria.
        _client = factory.CreateClient();
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

}
