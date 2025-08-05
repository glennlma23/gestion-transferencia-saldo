using Domain.Entities;

namespace Application.Interfaces;

public interface IWalletService
{
    Task<IEnumerable<Wallet>> GetAllAsync();
    Task<Wallet?> GetByIdAsync(int id);
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task DeleteAsync(int id);
    Task<Wallet?> GetByIdAndDocumentIdAsync(int id, string documentId);
    Task<IEnumerable<Wallet>> GetByDocumentIdAsync(string documentId);
}