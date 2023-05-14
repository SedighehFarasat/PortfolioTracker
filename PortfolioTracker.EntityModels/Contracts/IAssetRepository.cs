using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.EntityModels.Contracts;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAssetByUserId(string userId);
    Task<Asset?> GetAssetByUserIdAndInstrumentId(string userId, string instrumentId);
    Task<int> RemoveAsset(Asset asset);
    Task<int> AddAsset(Asset asset);
    Task<int> UpdateAsset(Asset asset);
}
