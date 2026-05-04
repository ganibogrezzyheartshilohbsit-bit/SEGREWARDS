using System.Collections.Generic;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface ILeaderboardRepository
    {
        IReadOnlyList<LeaderboardRow> GetTopByEcoPoints(int take);
    }
}
