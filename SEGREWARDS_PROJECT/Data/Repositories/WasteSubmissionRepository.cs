using System;
using MySql.Data.MySqlClient;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class WasteSubmissionRepository : IWasteSubmissionRepository
    {
        private readonly IDbConnectionFactory _connections;

        public WasteSubmissionRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public long InsertPending(
            long userId,
            short wasteTypeId,
            string studentNumberSnapshot,
            string fullNameSnapshot,
            string yearLevelSnapshot,
            string courseSnapshot,
            byte[] proofImage,
            string proofMimeType,
            string proofOriginalName)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO waste_submissions (
  user_id, waste_type_id, student_number_snapshot, full_name_snapshot,
  year_level_snapshot, course_snapshot, proof_image, proof_mime_type, proof_original_name,
  status, points_awarded)
VALUES (
  @uid, @wtid, @sns, @fns, @yls, @cs, @img, @mime, @oname,
  'pending', 0);";
                cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = userId;
                cmd.Parameters.Add("@wtid", MySqlDbType.Int16).Value = wasteTypeId;
                cmd.Parameters.Add("@sns", MySqlDbType.VarChar, 64).Value = studentNumberSnapshot ?? string.Empty;
                cmd.Parameters.Add("@fns", MySqlDbType.VarChar, 255).Value = fullNameSnapshot ?? string.Empty;
                cmd.Parameters.Add("@yls", MySqlDbType.VarChar, 64).Value = (object)yearLevelSnapshot ?? DBNull.Value;
                cmd.Parameters.Add("@cs", MySqlDbType.VarChar, 512).Value = (object)courseSnapshot ?? DBNull.Value;
                cmd.Parameters.Add("@img", MySqlDbType.MediumBlob).Value = (object)proofImage ?? DBNull.Value;
                cmd.Parameters.Add("@mime", MySqlDbType.VarChar, 128).Value = (object)proofMimeType ?? DBNull.Value;
                cmd.Parameters.Add("@oname", MySqlDbType.VarChar, 255).Value = (object)proofOriginalName ?? DBNull.Value;

                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
        }
    }
}
