using Application.DTOs.Wallet;
using Application.Mappings;
using Domain.Entities;
using FluentAssertions;
using System.Reflection.Metadata;

namespace Application.Tests.Mappings;

public class WalletMappingExtensionsTests
{
    [Fact]
    public void ToEntity_Should_Map_CreateWalletRequest_To_Wallet()
    {
        // Arrange
        var request = new CreateWalletRequest
        {
            Name = "Glenn Maura"
        };

        string documentId = "12345678";
        // Act
        var wallet = request.ToEntity(documentId);

        // Assert
        wallet.DocumentId.Should().Be("12345678");
        wallet.Name.Should().Be("Glenn Maura");
        wallet.Balance.Should().Be(0);

        var expectedTime = DateTime.UtcNow.AddHours(-5);
        wallet.CreatedAt.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(5));
        wallet.UpdatedAt.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateFromRequest_Should_Update_Wallet_Name_And_UpdatedAt()
    {
        // Arrange
        var wallet = new Wallet
        {
            Id = 1,
            Name = "Anterior",
            UpdatedAt = new DateTime(2000, 1, 1)
        };

        var request = new UpdateWalletRequest
        {
            Name = "Nuevo Nombre"
        };

        // Act
        wallet.UpdateFromRequest(request);

        // Assert
        wallet.Name.Should().Be("Nuevo Nombre");
        wallet.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(-5), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ToResponse_Should_Map_Wallet_To_WalletResponse()
    {
        // Arrange
        var wallet = new Wallet
        {
            Id = 10,
            DocumentId = "98765432",
            Name = "Test User",
            Balance = 150.75m,
            CreatedAt = new DateTime(2023, 1, 1),
            UpdatedAt = new DateTime(2023, 1, 2)
        };

        // Act
        var response = wallet.ToResponse();

        // Assert
        response.Id.Should().Be(10);
        response.DocumentId.Should().Be("98765432");
        response.Name.Should().Be("Test User");
        response.Balance.Should().Be(150.75m);
        response.CreatedAt.Should().Be(new DateTime(2023, 1, 1));
        response.UpdatedAt.Should().Be(new DateTime(2023, 1, 2));
    }

    [Fact]
    public void ToResponseList_Should_Map_List_Correctly()
    {
        // Arrange
        var wallets = new List<Wallet>
        {
            new Wallet { Id = 1, Name = "User 1", DocumentId = "0001", Balance = 10, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Wallet { Id = 2, Name = "User 2", DocumentId = "0002", Balance = 20, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        // Act
        var responses = wallets.ToResponseList().ToList();

        // Assert
        responses.Should().HaveCount(2);
        responses[0].Id.Should().Be(1);
        responses[1].Id.Should().Be(2);
    }
}
