using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly WalletService _walletService;

        public WalletServiceTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _walletService = new WalletService(_walletRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllWallets()
        {
            // Arrange
            var wallets = new List<Wallet> { new() { Id = 1 }, new() { Id = 2 } };
            _walletRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(wallets);

            // Act
            var result = await _walletService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnWallet_WhenExists()
        {
            // Arrange
            var wallet = new Wallet { Id = 1 };
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddWallet()
        {
            // Arrange
            var wallet = new Wallet { Id = 1 };

            // Act
            var result = await _walletService.CreateAsync(wallet);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.AddAsync(wallet), Times.Once);
            Assert.Equal(wallet, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateWallet()
        {
            // Arrange
            var wallet = new Wallet { Id = 1 };

            // Act
            var result = await _walletService.UpdateAsync(wallet);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(wallet), Times.Once);
            Assert.Equal(wallet, result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteWallet_WhenExists()
        {
            // Arrange
            var wallet = new Wallet { Id = 1 };
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(wallet);

            // Act
            await _walletService.DeleteAsync(1);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.DeleteAsync(wallet), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete_WhenWalletIsNull()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            // Act
            await _walletService.DeleteAsync(1);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Wallet>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAndDocumentIdAsync_ShouldReturnWallet_WhenExists()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, DocumentId = "12345678" };
            _walletRepositoryMock.Setup(repo => repo.GetByIdAndDocumentIdAsync(1, "12345678"))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetByIdAndDocumentIdAsync(1, "12345678");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wallet.Id, result!.Id);
            Assert.Equal(wallet.DocumentId, result.DocumentId);
        }

        [Fact]
        public async Task GetByDocumentIdAsync_ShouldReturnWallets()
        {
            // Arrange
            var wallets = new List<Wallet> { new() { Id = 1, DocumentId = "123" } };
            _walletRepositoryMock.Setup(repo => repo.GetByDocumentIdAsync("123")).ReturnsAsync(wallets);

            // Act
            var result = await _walletService.GetByDocumentIdAsync("123");

            // Assert
            Assert.Single(result);
        }
    }
}