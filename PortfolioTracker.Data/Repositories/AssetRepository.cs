using Microsoft.EntityFrameworkCore;
using PortfolioTracker.EntityModels.Contracts;
using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.Data.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly PortfolioTrackerDbContext _db;

    public AssetRepository(PortfolioTrackerDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsset(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        await _db.Assets.AddAsync(asset);
        int affected = _db.SaveChanges();
        return affected;
    }

    public async Task<IEnumerable<Asset>> GetAssetByUserId(string userId)
    {
        return await _db.Assets.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Asset?> GetAssetByUserIdAndInstrumentId(string userId, string instrumentId)
    {
        return await _db.Assets.FirstOrDefaultAsync(x => x.UserId == userId && x.InstrumentId == instrumentId);
    }

    public async Task<int> RemoveAsset(Asset asset)
    {
        _db.Assets.Remove(asset);
        int affected = await _db.SaveChangesAsync();
        return affected;
    }

    public async Task<int> UpdateAsset(Asset asset)
    {
        _db.Entry(asset).State = EntityState.Modified;
        int affected = await _db.SaveChangesAsync();
        return affected;
    }
}
