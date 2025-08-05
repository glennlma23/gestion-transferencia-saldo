using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Wallet>> GetAllAsync()
    {
        return await _context.Wallets.ToListAsync();
    }

    public async Task<Wallet?> GetByIdAsync(int id)
    {
        return await _context.Wallets.FindAsync(id);
    }

    public async Task AddAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Wallet wallet)
    {
        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Wallet>> GetByDocumentIdAsync(string documentId)
    {
        return await _context.Wallets
            .Where(w => w.DocumentId == documentId)
            .ToListAsync();
    }

    public async Task<Wallet?> GetByIdAndDocumentIdAsync(int id, string documentId)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.DocumentId == documentId);
    }
}