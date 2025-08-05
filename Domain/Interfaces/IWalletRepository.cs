using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByIdAsync(int id);
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(Wallet wallet);
        Task<Wallet?> GetByIdAndDocumentIdAsync(int id, string documentId);
        Task<IEnumerable<Wallet>> GetByDocumentIdAsync(string documentId);
    }
}