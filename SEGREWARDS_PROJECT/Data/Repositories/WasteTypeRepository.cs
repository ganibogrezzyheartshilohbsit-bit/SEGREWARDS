using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class WasteTypeRepository : IWasteTypeRepository
    {
        private readonly IDbConnectionFactory _connections;

        public WasteTypeRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public IReadOnlyList<WasteTypeRecord> GetAll()
        {
            var list = new List<WasteTypeRecord>();
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, code, display_name, default_points FROM waste_types ORDER BY id;";
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

        public WasteTypeRecord GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT id, code, display_name, default_points
FROM waste_types
WHERE code = @c
LIMIT 1;";
                cmd.Parameters.Add("@c", MySqlDbType.VarChar, 32).Value = code.Trim().ToUpperInvariant();

                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        private static WasteTypeRecord Map(MySqlDataReader r)
        {
            return new WasteTypeRecord
            {
                Id = r.GetInt16("id"),
                Code = r.GetString("code"),
                DisplayName = r.GetString("display_name"),
                DefaultPoints = r.GetInt32("default_points")
            };
        }
    }
}
