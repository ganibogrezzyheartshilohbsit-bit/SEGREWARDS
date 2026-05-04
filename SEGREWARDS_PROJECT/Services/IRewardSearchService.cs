using System.Collections.Generic;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Services
{
    public interface IRewardSearchService
    {
        IReadOnlyList<RewardCatalogRecord> Search(string query, int take = 50);
    }
}
