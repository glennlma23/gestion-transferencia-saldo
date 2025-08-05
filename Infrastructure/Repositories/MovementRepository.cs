using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MovementRepository : IMovementRepository
{
    private readonly AppDbContext _context;

    public MovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId)
    {
        return await _context.Movements
                             .Where(m => m.WalletId == walletId)
                             .ToListAsync();
    }

    public async Task AddAsync(Movement movement)
    {
        await _context.Movements.AddAsync(movement);
        await _context.SaveChangesAsync();
    }
}