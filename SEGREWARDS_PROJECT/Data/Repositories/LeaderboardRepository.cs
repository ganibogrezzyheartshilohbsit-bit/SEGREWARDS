using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IDbConnectionFactory _connections;

        public LeaderboardRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public IReadOnlyList<LeaderboardRow> GetTopByEcoPoints(int take)
        {
            var list = new List<LeaderboardRow>();
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT
  u.id AS user_id,
  u.student_number,
  u.full_name,
  u.eco_points_balance,
  COUNT(ws.id) AS submission_count,
  SUM(CASE WHEN ws.status = 'approved' THEN 1 ELSE 0 END) AS approved_submissions
FROM users u
LEFT JOIN waste_submissions ws ON ws.user_id = u.id
WHERE u.is_active = 1
GROUP BY u.id, u.student_number, u.full_name, u.eco_points_balance
ORDER BY u.eco_points_balance DESC, approved_submissions DESC
LIMIT @take;";
                cmd.Parameters.Add("@take", MySqlDbType.Int32).Value = take;

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new LeaderboardRow
                        {
                            UserId = r.GetInt64("user_id"),
                            StudentNumber = r.GetString("student_number"),
                            FullName = r.GetString("full_name"),
                            EcoPointsBalance = r.GetInt32("eco_points_balance"),
                            SubmissionCount = r.IsDBNull(r.GetOrdinal("submission_count")) ? 0 : r.GetInt32("submission_count"),
                            ApprovedSubmissions = r.IsDBNull(r.GetOrdinal("approved_submissions")) ? 0 : ConvertToInt32(r["approved_submissions"])
                        });
                    }
                }
            }

            return list;
        }

        private static int ConvertToInt32(object value)
        {
            if (value == null || value == System.DBNull.Value) return 0;
            if (value is int i) return i;
            if (value is long l) return (int)l;
            if (value is decimal d) return (int)d;
            return System.Convert.ToInt32(value);
        }
    }
}
