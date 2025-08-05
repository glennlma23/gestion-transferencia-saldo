using Api.GestionTransferenciaSaldo;
using Application.DTOs.Wallet;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Api.IntegrationTests.Controllers;

public class WalletControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WalletControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/wallet");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var request = new CreateWalletRequest
        {
            DocumentId = Guid.NewGuid().ToString().Substring(0, 8),
            Name = "Test User"
        };

        var response = await _client.PostAsJsonAsync("/api/wallet", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<WalletResponse>();
        result.Should().NotBeNull();
        result!.DocumentId.Should().Be(request.DocumentId);
        result.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetByDocumentId_ShouldReturnOk()
    {
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);

        // Crear primero
        await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
            DocumentId = documentId,
            Name = "User To Search"
        });

        var response = await _client.GetAsync($"/api/wallet/document/{documentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);
        var createResponse = await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
            DocumentId = documentId,
            Name = "User Before"
        });

        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletResponse>();

        var updateRequest = new UpdateWalletRequest
        {
            Name = "User After"
        };

        var response = await _client.PutAsJsonAsync($"/api/wallet/{wallet!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);
        var createResponse = await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
            DocumentId = documentId,
            Name = "User To Delete"
        });

        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletResponse>();

        var response = await _client.DeleteAsync($"/api/wallet/{wallet!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
