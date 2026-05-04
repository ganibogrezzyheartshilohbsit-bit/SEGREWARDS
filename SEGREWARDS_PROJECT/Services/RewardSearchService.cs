using System.Collections.Generic;
using SEGREWARDS_PROJECT.Data.Repositories;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Services
{
    public sealed class RewardSearchService : IRewardSearchService
    {
        private readonly IRewardCatalogRepository _rewards;

        public RewardSearchService(IRewardCatalogRepository rewards)
        {
            _rewards = rewards;
        }

        public IReadOnlyList<RewardCatalogRecord> Search(string query, int take = 50)
        {
            return _rewards.SearchActive(query, take);
        }
    }
}
