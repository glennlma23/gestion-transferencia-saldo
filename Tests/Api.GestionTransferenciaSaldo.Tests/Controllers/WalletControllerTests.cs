using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.GestionTransferenciaSaldo.Controllers;
using Xunit;

namespace Api.GestionTransferenciaSaldo.Tests.Controllers
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _walletServiceMock;
        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _walletServiceMock = new Mock<IWalletService>();
            _controller = new WalletController(_walletServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithWallets()
        {
            var wallets = new List<Wallet>
            {
                new Wallet { Id = 1, DocumentId = "123", Name = "Glenn", Balance = 10 },
                new Wallet { Id = 2, DocumentId = "456", Name = "Maura", Balance = 20 }
            };
            _walletServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(wallets);

            var result = await _controller.GetAll();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedWallets = okResult!.Value as IEnumerable<WalletResponse>;
            returnedWallets.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByDocumentId_ShouldReturnOk()
        {
            var wallets = new List<Wallet> { new Wallet { Id = 1, DocumentId = "123", Name = "Test" } };
            _walletServiceMock.Setup(s => s.GetByDocumentIdAsync("123")).ReturnsAsync(wallets);

            var result = await _controller.GetByDocumentId("123");

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as IEnumerable<WalletResponse>;
            response.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByIdAndDocumentId_ShouldReturnOk_WhenWalletExists()
        {
            var wallet = new Wallet { Id = 1, DocumentId = "123", Name = "Test" };
            _walletServiceMock.Setup(s => s.GetByIdAndDocumentIdAsync(1, "123")).ReturnsAsync(wallet);

            var result = await _controller.GetByIdAndDocumentId(1, "123");

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as WalletResponse;
            response!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAndDocumentId_ShouldReturnNotFound_WhenWalletDoesNotExist()
        {
            _walletServiceMock.Setup(s => s.GetByIdAndDocumentIdAsync(1, "999")).ReturnsAsync((Wallet?)null);

            var result = await _controller.GetByIdAndDocumentId(1, "999");

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var request = new CreateWalletRequest { DocumentId = "123", Name = "New User" };
            var wallet = request.ToEntity();
            wallet.Id = 1;

            _walletServiceMock.Setup(s => s.CreateAsync(It.IsAny<Wallet>())).ReturnsAsync(wallet);

            var result = await _controller.Create(request);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            var response = createdResult!.Value as WalletResponse;
            response!.DocumentId.Should().Be("123");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenWalletExists()
        {
            var wallet = new Wallet { Id = 1, Name = "Old Name", DocumentId = "123" };
            var request = new UpdateWalletRequest { Name = "New Name" };

            _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(wallet);

            var result = await _controller.Update(1, request);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenWalletDoesNotExist()
        {
            var request = new UpdateWalletRequest { Name = "New Name" };
            _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            var result = await _controller.Update(1, request);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenWalletExists()
        {
            var wallet = new Wallet { Id = 1, DocumentId = "123" };
            _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(wallet);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenWalletDoesNotExist()
        {
            _walletServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
