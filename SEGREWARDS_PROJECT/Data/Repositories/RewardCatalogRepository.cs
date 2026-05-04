using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class RewardCatalogRepository : IRewardCatalogRepository
    {
        private readonly IDbConnectionFactory _connections;

        public RewardCatalogRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public IReadOnlyList<RewardCatalogRecord> ListActive(int take)
        {
            return SearchActive(null, take);
        }

        public IReadOnlyList<RewardCatalogRecord> SearchActive(string searchText, int take)
        {
            var list = new List<RewardCatalogRecord>();
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    cmd.CommandText = @"
SELECT id, name, description, points_cost, is_active, sort_order
FROM reward_catalog
WHERE is_active = 1
ORDER BY sort_order, name
LIMIT @take;";
                }
                else
                {
                    cmd.CommandText = @"
SELECT id, name, description, points_cost, is_active, sort_order
FROM reward_catalog
WHERE is_active = 1
  AND (name LIKE @q OR description LIKE @q)
ORDER BY sort_order, name
LIMIT @take;";
                    cmd.Parameters.Add("@q", MySqlDbType.VarChar, 255).Value = "%" + searchText.Trim() + "%";
                }

                cmd.Parameters.Add("@take", MySqlDbType.Int32).Value = take;

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(Map(r));
                    }
                }
            }

            return list;
        }

        private static RewardCatalogRecord Map(MySqlDataReader r)
        {
            return new RewardCatalogRecord
            {
                Id = r.GetInt16("id"),
                Name = r.GetString("name"),
                Description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description"),
                PointsCost = r.GetInt32("points_cost"),
                IsActive = r.GetBoolean("is_active"),
                SortOrder = r.GetInt32("sort_order")
            };
        }
    }
}
