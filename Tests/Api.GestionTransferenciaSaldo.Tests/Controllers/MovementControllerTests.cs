using Api.GestionTransferenciaSaldo.Controllers;
using Application.DTOs.Movement;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.GestionTransferenciaSaldo.Tests.Controllers;

public class MovementControllerTests
{
    private readonly Mock<IMovementService> _movementServiceMock;
    private readonly MovementController _controller;

    public MovementControllerTests()
    {
        _movementServiceMock = new Mock<IMovementService>();
        _controller = new MovementController(_movementServiceMock.Object);
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
    public async Task Create_ReturnsCreatedMovement()
    {
        // Arrange
        var request = new CreateMovementRequest
        {
            WalletId = 1,
            Amount = 50,
            Type = (int)MovementType.Debit
        };

        var movement = request.ToEntity();
        movement.Id = 10;

        _movementServiceMock.Setup(s => s.CreateAsync(It.IsAny<Movement>()))
                            .ReturnsAsync(movement);

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
}
