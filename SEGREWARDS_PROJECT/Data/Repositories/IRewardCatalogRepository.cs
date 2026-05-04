using System.Collections.Generic;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface IRewardCatalogRepository
    {
        IReadOnlyList<RewardCatalogRecord> SearchActive(string searchText, int take);
        IReadOnlyList<RewardCatalogRecord> ListActive(int take);
    }
}
