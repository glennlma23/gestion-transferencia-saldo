using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<IEnumerable<Wallet>> GetAllAsync()
    {
        return await _walletRepository.GetAllAsync();
    }

    public async Task<Wallet?> GetByIdAsync(int id)
    {
        return await _walletRepository.GetByIdAsync(id);
    }

    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        await _walletRepository.AddAsync(wallet);
        return wallet;
    }

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        await _walletRepository.UpdateAsync(wallet);
        return wallet;
    }

    public async Task DeleteAsync(int id)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet != null)
        {
            await _walletRepository.DeleteAsync(wallet);
        }
    }

    public async Task<Wallet?> GetByIdAndDocumentIdAsync(int id, string documentId)
    {
        return await _walletRepository.GetByIdAndDocumentIdAsync(id, documentId);
    }

    public async Task<IEnumerable<Wallet>> GetByDocumentIdAsync(string documentId)
    {
        return await _walletRepository.GetByDocumentIdAsync(documentId);
    }
}