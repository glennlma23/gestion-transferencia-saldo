using Api.GestionTransferenciaSaldo.Controllers;
using Application.DTOs.Movement;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Api.GestionTransferenciaSaldo.Tests.Controllers;

public class MovementControllerTests
{
    private readonly Mock<IMovementService> _movementServiceMock;
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly MovementController _controller;

    public MovementControllerTests()
    {
        _movementServiceMock = new Mock<IMovementService>();
        _walletServiceMock = new Mock<IWalletService>();

        _controller = new MovementController(_movementServiceMock.Object, _walletServiceMock.Object);
    }

    private ClaimsPrincipal GetFakeUser(string documentId)
    {
        var claims = new List<Claim>
        {
            new Claim("documentId", documentId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task GetByWalletId_ReturnsOkWithMovements()
    {
        // Arrange
        var walletId = 1;
        var movements = new List<Movement>
        {
            new Movement { Id = 1, WalletId = walletId, Amount = 100, Type = MovementType.Credit, CreatedAt = DateTime.UtcNow }
        };

        _movementServiceMock.Setup(s => s.GetByWalletIdAsync(walletId))
                            .ReturnsAsync(movements);

        // Act
        var result = await _controller.GetByWalletId(walletId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as IEnumerable<MovementResponse>;
        response.Should().NotBeNull();
        response!.Count().Should().Be(1);
    }

    [Fact]
    public async Task Create_ReturnsCreatedMovement_WhenWalletBelongsToUser()
    {
        // Arrange
        var request = new CreateMovementRequest
        {
            WalletId = 1,
            Amount = 50,
            Type = (int)MovementType.Debit
        };

        var userDocumentId = "12345678";

        var wallet = new Wallet
        {
            Id = 1,
            DocumentId = userDocumentId,
            Name = "Test User",
            Balance = 100
        };

        _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(wallet);

        var movement = request.ToEntity();
        movement.Id = 10;

        _movementServiceMock.Setup(s => s.CreateAsync(It.IsAny<Movement>()))
                            .ReturnsAsync(movement);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = GetFakeUser(userDocumentId)
            }
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var response = createdResult.Value as MovementResponse;
        response.Should().NotBeNull();
        response!.WalletId.Should().Be(request.WalletId);
        response.Amount.Should().Be(request.Amount);
    }

    [Fact]
    public async Task Create_ReturnsUnauthorized_WhenWalletDoesNotBelongToUser()
    {
        // Arrange
        var request = new CreateMovementRequest
        {
            WalletId = 1,
            Amount = 50,
            Type = (int)MovementType.Debit
        };

        var userDocumentId = "12345678";

        var wallet = new Wallet
        {
            Id = 1,
            DocumentId = "99999999",
            Name = "Otro Usuario"
        };

        _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(wallet);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = GetFakeUser(userDocumentId)
            }
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var unauthorized = result.Result as ForbidResult;
        unauthorized.Should().NotBeNull();
    }
}
