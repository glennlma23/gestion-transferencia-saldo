using Api.GestionTransferenciaSaldo;
using Application.DTOs.Movement;
using Application.DTOs.User;
using Application.DTOs.Wallet;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.IntegrationTests.Controllers;

public class MovementControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MovementControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync(string documentId)
    {
        var username = $"user{documentId}";
        var password = "Test@123";

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Password = password,
            DocumentId = documentId
        };

        await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return loginResult!.Token;
    }

    private void AddJwtHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateMovement_ShouldReturnCreated()
    {
        var documentId = "99999999";
        var token = await RegisterAndLoginAsync(documentId);
        AddJwtHeader(token);

        var walletRequest = new
        {
            DocumentId = documentId,
            Name = "Billetera de prueba"
        };

        var walletResponse = await _client.PostAsJsonAsync("/api/Wallet", walletRequest);
        walletResponse.EnsureSuccessStatusCode();
        var wallet = JsonSerializer.Deserialize<WalletResponse>(
            await walletResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var movementRequest = new CreateMovementRequest
        {
            WalletId = wallet!.Id,
            Amount = 100,
            Type = (int)MovementType.Credit
        };

        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<MovementResponse>();
        content.Should().NotBeNull();
        content!.WalletId.Should().Be(wallet.Id);
        content.Amount.Should().Be(100);
        content.Type.Should().Be("Credit");
    }

    [Fact]
    public async Task GetByWalletId_ShouldReturnMovements()
    {
        var documentId = "12345678";
        var token = await RegisterAndLoginAsync(documentId);
        AddJwtHeader(token);

        var walletRequest = new
        {
            DocumentId = documentId,
            Name = "Wallet Movimientos"
        };

        var walletResponse = await _client.PostAsJsonAsync("/api/Wallet", walletRequest);
        walletResponse.EnsureSuccessStatusCode();
        var wallet = JsonSerializer.Deserialize<WalletResponse>(
            await walletResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var movementRequest = new CreateMovementRequest
        {
            WalletId = wallet!.Id,
            Amount = 50,
            Type = (int)MovementType.Credit
        };

        var movementResponse = await _client.PostAsJsonAsync("/api/Movement", movementRequest);
        movementResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/api/Movement/wallet/{wallet.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movements = await response.Content.ReadFromJsonAsync<List<MovementResponse>>();
        movements.Should().NotBeNullOrEmpty();
        movements!.Any(m => m.Amount == 50 && m.Type == "Credit").Should().BeTrue();
    }

    [Fact]
    public async Task CreateMovement_WithInvalidWalletId_ShouldReturnBadRequest()
    {
        var documentId = "11112222";
        var token = await RegisterAndLoginAsync(documentId);
        AddJwtHeader(token);

        var movementRequest = new CreateMovementRequest
        {
            WalletId = 999999,
            Amount = 100,
            Type = (int)MovementType.Credit
        };

        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateDebitMovement_WithInsufficientBalance_ShouldReturnBadRequest()
    {
        var documentId = "22222222";
        var token = await RegisterAndLoginAsync(documentId);
        AddJwtHeader(token);

        var walletRequest = new
        {
            DocumentId = documentId,
            Name = "Wallet sin saldo"
        };

        var walletResponse = await _client.PostAsJsonAsync("/api/Wallet", walletRequest);
        walletResponse.EnsureSuccessStatusCode();

        var wallet = JsonSerializer.Deserialize<WalletResponse>(
            await walletResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var movementRequest = new CreateMovementRequest
        {
            WalletId = wallet!.Id,
            Amount = 100,
            Type = (int)MovementType.Debit
        };

        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("Saldo insuficiente");
    }
}
