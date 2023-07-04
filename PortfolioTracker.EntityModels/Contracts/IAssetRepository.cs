using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.EntityModels.Contracts;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetByUserId(string userId);
    Task<Asset?> GetByUserIdAndInstrumentId(string userId, string instrumentId);
    Task<int> Remove(Asset asset);
    Task<int> Add(Asset asset);
    Task<int> Update(Asset asset);
}
