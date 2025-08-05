using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class MovementServiceTests
    {
        private readonly Mock<IMovementRepository> _movementRepositoryMock;
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly MovementService _movementService;

        public MovementServiceTests()
        {
            _movementRepositoryMock = new Mock<IMovementRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _movementService = new MovementService(_movementRepositoryMock.Object, _walletRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByWalletIdAsync_ShouldReturnMovements()
        {
            // Arrange
            var walletId = 1;
            var movements = new List<Movement>
            {
                new() { Id = 1, WalletId = walletId, Amount = 100, Type = MovementType.Credit }
            };

            _movementRepositoryMock
                .Setup(repo => repo.GetByWalletIdAsync(walletId))
                .ReturnsAsync(movements);

            // Act
            var result = await _movementService.GetByWalletIdAsync(walletId);

            // Assert
            result.Should().BeEquivalentTo(movements);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenWalletDoesNotExist()
        {
            // Arrange
            var movement = new Movement { WalletId = 1, Amount = 100, Type = MovementType.Credit };

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(movement.WalletId))
                .ReturnsAsync((Wallet?)null);

            // Act
            var act = async () => await _movementService.CreateAsync(movement);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Billetera no encontrada, valide el WalletId ingresado.");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenInsufficientBalanceForDebit()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, Balance = 50 };
            var movement = new Movement { WalletId = 1, Amount = 100, Type = MovementType.Debit };

            _walletRepositoryMock.Setup(r => r.GetByIdAsync(wallet.Id)).ReturnsAsync(wallet);

            // Act
            var act = async () => await _movementService.CreateAsync(movement);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Saldo insuficiente. Modifique el monto de la transferencia.");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddMovementAndUpdateWalletBalance_WhenCredit()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, Balance = 200 };
            var movement = new Movement { WalletId = wallet.Id, Amount = 100, Type = MovementType.Credit };

            _walletRepositoryMock.Setup(r => r.GetByIdAsync(wallet.Id)).ReturnsAsync(wallet);

            // Act
            var result = await _movementService.CreateAsync(movement);

            // Assert
            result.Should().BeEquivalentTo(movement);
            wallet.Balance.Should().Be(300);

            _movementRepositoryMock.Verify(r => r.AddAsync(movement), Times.Once);
            _walletRepositoryMock.Verify(r => r.UpdateAsync(wallet), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddMovementAndUpdateWalletBalance_WhenDebit()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, Balance = 200 };
            var movement = new Movement { WalletId = wallet.Id, Amount = 50, Type = MovementType.Debit };

            _walletRepositoryMock.Setup(r => r.GetByIdAsync(wallet.Id)).ReturnsAsync(wallet);

            // Act
            var result = await _movementService.CreateAsync(movement);

            // Assert
            result.Should().BeEquivalentTo(movement);
            wallet.Balance.Should().Be(150);

            _movementRepositoryMock.Verify(r => r.AddAsync(movement), Times.Once);
            _walletRepositoryMock.Verify(r => r.UpdateAsync(wallet), Times.Once);
        }
    }
}
