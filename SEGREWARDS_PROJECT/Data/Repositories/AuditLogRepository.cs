using System;
using MySql.Data.MySqlClient;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDbConnectionFactory _connections;

        public AuditLogRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public void Insert(long? userId, string action, string entityType, string entityId, string details)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO audit_log (user_id, action, entity_type, entity_id, details)
VALUES (@uid, @act, @et, @eid, @det);";
                cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = (object)userId ?? DBNull.Value;
                cmd.Parameters.Add("@act", MySqlDbType.VarChar, 64).Value = action ?? string.Empty;
                cmd.Parameters.Add("@et", MySqlDbType.VarChar, 64).Value = (object)entityType ?? DBNull.Value;
                cmd.Parameters.Add("@eid", MySqlDbType.VarChar, 64).Value = (object)entityId ?? DBNull.Value;
                cmd.Parameters.Add("@det", MySqlDbType.Text).Value = (object)details ?? DBNull.Value;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
