using Application.DTOs.Movement;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.Tests.Mappings;

public class MovementMappingExtensionsTests
{
    [Fact]
    public void ToEntity_Should_Map_Correctly()
    {
        // Arrange
        var request = new CreateMovementRequest
        {
            WalletId = 10,
            Amount = 100,
            Type = (int)MovementType.Credit
        };

        // Act
        var result = request.ToEntity();

        // Assert
        result.WalletId.Should().Be(10);
        result.Amount.Should().Be(100);
        result.Type.Should().Be(MovementType.Credit);

        var expectedTime = DateTime.UtcNow.AddHours(-5);
        result.CreatedAt.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ToResponse_Should_Map_Correctly()
    {
        // Arrange
        var movement = new Movement
        {
            Id = 1,
            WalletId = 2,
            Amount = 50,
            Type = MovementType.Debit,
            CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0)
        };

        // Act
        var response = movement.ToResponse();

        // Assert
        response.Id.Should().Be(1);
        response.WalletId.Should().Be(2);
        response.Amount.Should().Be(50);
        response.Type.Should().Be("Debit");
        response.CreatedAt.Should().Be(new DateTime(2024, 1, 1, 12, 0, 0));
    }

    [Fact]
    public void ToResponseList_Should_Map_List_Correctly()
    {
        // Arrange
        var movements = new List<Movement>
        {
            new Movement
            {
                Id = 1,
                WalletId = 2,
                Amount = 30,
                Type = MovementType.Credit,
                CreatedAt = DateTime.UtcNow
            },
            new Movement
            {
                Id = 2,
                WalletId = 3,
                Amount = 20,
                Type = MovementType.Debit,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var responses = movements.ToResponseList().ToList();

        // Assert
        responses.Count.Should().Be(2);

        responses[0].Id.Should().Be(1);
        responses[0].Type.Should().Be("Credit");

        responses[1].Id.Should().Be(2);
        responses[1].Type.Should().Be("Debit");
    }
}
