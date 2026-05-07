using System.Collections.Generic;
using System;
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
                cmd.Parameters.Add("@take", MySqlDbType.Int32).Value = take;

                // Backward-compatible query: if the DB hasn't been migrated to include image_path yet,
                // retry without it so rewards still load.
                var hasSearch = !string.IsNullOrWhiteSpace(searchText);
                if (hasSearch)
                {
                    cmd.Parameters.Add("@q", MySqlDbType.VarChar, 255).Value = "%" + searchText.Trim() + "%";
                }

                try
                {
                    cmd.CommandText = BuildQuery(includeImagePath: true, hasSearch: hasSearch);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(Map(r));
                        }
                    }
                }
                catch (MySqlException ex) when (ex.Number == 1054) // Unknown column
                {
                    list.Clear();
                    cmd.CommandText = BuildQuery(includeImagePath: false, hasSearch: hasSearch);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(Map(r));
                        }
                    }
                }
            }

            return list;
        }

        private static string BuildQuery(bool includeImagePath, bool hasSearch)
        {
            var select = includeImagePath
                ? "SELECT id, name, description, points_cost, image_path, is_active, sort_order"
                : "SELECT id, name, description, points_cost, is_active, sort_order";

            if (!hasSearch)
            {
                return @"
" + select + @"
FROM reward_catalog
WHERE is_active = 1
ORDER BY sort_order, name
LIMIT @take;";
            }

            return @"
" + select + @"
FROM reward_catalog
WHERE is_active = 1
  AND (name LIKE @q OR description LIKE @q)
ORDER BY sort_order, name
LIMIT @take;";
        }

        private static RewardCatalogRecord Map(MySqlDataReader r)
        {
            string imagePath = null;
            try
            {
                var ord = r.GetOrdinal("image_path");
                imagePath = r.IsDBNull(ord) ? null : r.GetString(ord);
            }
            catch (IndexOutOfRangeException)
            {
                // Column doesn't exist in older schemas.
            }

            return new RewardCatalogRecord
            {
                Id = r.GetInt16("id"),
                Name = r.GetString("name"),
                Description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description"),
                PointsCost = r.GetInt32("points_cost"),
                ImagePath = imagePath,
                IsActive = r.GetBoolean("is_active"),
                SortOrder = r.GetInt32("sort_order")
            };
        }
    }
}
