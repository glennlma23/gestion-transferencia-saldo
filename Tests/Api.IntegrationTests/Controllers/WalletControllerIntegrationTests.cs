using Api.GestionTransferenciaSaldo;
using Application.DTOs.User;
using Application.DTOs.Wallet;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
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

    private async Task<string> AuthenticateAsync(string username, string password, string documentId)
    {
        await _client.PostAsJsonAsync("/api/user/register", new RegisterRequest
        {
            Username = username,
            Password = password,
            DocumentId = documentId
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.Token;
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
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);
        var token = await AuthenticateAsync("user1", "pass1", documentId);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateWalletRequest
        {
            Name = "Test User"
        };

        var response = await _client.PostAsJsonAsync("/api/wallet", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<WalletResponse>();
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.DocumentId.Should().Be(documentId);
    }

    [Fact]
    public async Task GetByDocumentId_ShouldReturnOk()
    {
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);
        var token = await AuthenticateAsync("user2", "pass2", documentId);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
            Name = "User To Search"
        });

        var response = await _client.GetAsync($"/api/wallet/document/{documentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        var documentId = Guid.NewGuid().ToString().Substring(0, 8);
        var token = await AuthenticateAsync("user3", "pass3", documentId);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
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
        var token = await AuthenticateAsync("user4", "pass4", documentId);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/wallet", new CreateWalletRequest
        {
            Name = "User To Delete"
        });

        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletResponse>();

        var response = await _client.DeleteAsync($"/api/wallet/{wallet!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
