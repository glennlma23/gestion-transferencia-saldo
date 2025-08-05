using Application.DTOs.Wallet;
using Domain.Entities;

namespace Application.Mappings;

public static class WalletMappingExtensions
{
    public static Wallet ToEntity(this CreateWalletRequest request)
    {
        return new Wallet
        {
            DocumentId = request.DocumentId,
            Name = request.Name,
            Balance = 0,
            CreatedAt = DateTime.UtcNow.AddHours(-5),
            UpdatedAt = DateTime.UtcNow.AddHours(-5)
        };
    }

    public static void UpdateFromRequest(this Wallet wallet, UpdateWalletRequest request)
    {
        wallet.Name = request.Name;
        wallet.UpdatedAt = DateTime.UtcNow.AddHours(-5);
    }

    public static WalletResponse ToResponse(this Wallet wallet)
    {
        return new WalletResponse
        {
            Id = wallet.Id,
            DocumentId = wallet.DocumentId,
            Name = wallet.Name,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }

    public static IEnumerable<WalletResponse> ToResponseList(this IEnumerable<Wallet> wallets)
    {
        return wallets.Select(w => w.ToResponse());
    }
}