using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.EntityModels.Contracts;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetByUserId(string userId);
    Task<Asset?> GetByUserIdAndInstrumentId(string userId, string instrumentId);
    Task Remove(Asset asset);
    Task Add(Asset asset);
    Task Update(Asset asset);
}
