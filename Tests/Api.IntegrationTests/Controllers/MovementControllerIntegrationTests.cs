using Api.GestionTransferenciaSaldo;
using Application.DTOs.Movement;
using Application.DTOs.Wallet;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
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

    [Fact]
    public async Task CreateMovement_ShouldReturnCreated()
    {
        // Arrange: crear primero una billetera para asociarla al movimiento
        var walletRequest = new
        {
            DocumentId = "99999999",
            Name = "Billetera de prueba"
        };

        var walletResponse = await _client.PostAsJsonAsync("/api/Wallet", walletRequest);
        walletResponse.EnsureSuccessStatusCode();
        var wallet = JsonSerializer.Deserialize<WalletResponse>(await walletResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var movementRequest = new CreateMovementRequest
        {
            WalletId = wallet!.Id,
            Amount = 100,
            Type = (int)MovementType.Credit
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        // Assert
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
        // Arrange: crear billetera y movimiento
        var walletRequest = new
        {
            DocumentId = "12345678",
            Name = "Wallet Movimientos"
        };

        var walletResponse = await _client.PostAsJsonAsync("/api/Wallet", walletRequest);
        walletResponse.EnsureSuccessStatusCode();
        var wallet = JsonSerializer.Deserialize<WalletResponse>(await walletResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var movementRequest = new CreateMovementRequest
        {
            WalletId = wallet!.Id,
            Amount = 50,
            Type = (int)MovementType.Credit
        };

        var movementResponse = await _client.PostAsJsonAsync("/api/Movement", movementRequest);
        movementResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/Movement/wallet/{wallet.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movements = await response.Content.ReadFromJsonAsync<List<MovementResponse>>();
        movements.Should().NotBeNullOrEmpty();
        movements!.Any(m => m.Amount == 50 && m.Type == "Credit").Should().BeTrue();
    }

    [Fact]
    public async Task CreateMovement_WithInvalidWalletId_ShouldReturnBadRequest()
    {
        // Arrange
        var movementRequest = new CreateMovementRequest
        {
            WalletId = 999999, // Wallet que no existe
            Amount = 100,
            Type = (int)MovementType.Credit
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("Billetera no encontrada");
    }

    [Fact]
    public async Task CreateDebitMovement_WithInsufficientBalance_ShouldReturnBadRequest()
    {
        // Arrange: primero creamos una billetera con saldo cero
        var walletRequest = new
        {
            DocumentId = "22222222",
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

        // Act
        var response = await _client.PostAsJsonAsync("/api/Movement", movementRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("Saldo insuficiente");
    }

}
