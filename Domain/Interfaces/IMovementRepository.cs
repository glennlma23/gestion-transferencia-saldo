using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMovementRepository
    {
        Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId);
        Task AddAsync(Movement movement);
    }
}