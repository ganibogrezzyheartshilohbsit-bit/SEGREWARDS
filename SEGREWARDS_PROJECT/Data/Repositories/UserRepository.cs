using System;
using MySql.Data.MySqlClient;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connections;

        public UserRepository(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public UserRecord GetByStudentNumber(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber)) return null;

            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT id, student_number, email, password_hash, password_salt, full_name, year_level, course,
       role_id, eco_points_balance, is_active, created_at, updated_at
FROM users
WHERE student_number = @sn
LIMIT 1;";
                cmd.Parameters.Add("@sn", MySqlDbType.VarChar, 64).Value = studentNumber.Trim();

                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public UserRecord GetById(long userId)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT id, student_number, email, password_hash, password_salt, full_name, year_level, course,
       role_id, eco_points_balance, is_active, created_at, updated_at
FROM users
WHERE id = @id
LIMIT 1;";
                cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public bool StudentNumberExists(string studentNumber)
        {
            return GetByStudentNumber(studentNumber) != null;
        }

        public bool EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM users WHERE email = @e LIMIT 1;";
                cmd.Parameters.Add("@e", MySqlDbType.VarChar, 255).Value = email.Trim();
                var o = cmd.ExecuteScalar();
                return o != null && o != DBNull.Value;
            }
        }

        public long Insert(UserRecord user)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO users (student_number, email, password_hash, password_salt, full_name, year_level, course, role_id, eco_points_balance, is_active)
VALUES (@sn, @em, @ph, @ps, @fn, @yl, @cr, @rid, @pts, 1);";
                cmd.Parameters.Add("@sn", MySqlDbType.VarChar, 64).Value = user.StudentNumber.Trim();
                cmd.Parameters.Add("@em", MySqlDbType.VarChar, 255).Value = (object)user.Email ?? DBNull.Value;
                cmd.Parameters.Add("@ph", MySqlDbType.Binary).Value = user.PasswordHash;
                cmd.Parameters.Add("@ps", MySqlDbType.Binary).Value = user.PasswordSalt;
                cmd.Parameters.Add("@fn", MySqlDbType.VarChar, 255).Value = user.FullName ?? string.Empty;
                cmd.Parameters.Add("@yl", MySqlDbType.VarChar, 64).Value = (object)user.YearLevel ?? DBNull.Value;
                cmd.Parameters.Add("@cr", MySqlDbType.VarChar, 512).Value = (object)user.Course ?? DBNull.Value;
                cmd.Parameters.Add("@rid", MySqlDbType.Byte).Value = user.RoleId;
                cmd.Parameters.Add("@pts", MySqlDbType.Int32).Value = user.EcoPointsBalance;

                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
        }

        public void UpdateEcoPoints(long userId, int newBalance)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET eco_points_balance = @b WHERE id = @id;";
                cmd.Parameters.Add("@b", MySqlDbType.Int32).Value = newBalance;
                cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProfile(long userId, string fullName, string yearLevel, string course)
        {
            using (var conn = _connections.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE users
SET full_name = @fn, year_level = @yl, course = @cr
WHERE id = @id;";
                cmd.Parameters.Add("@fn", MySqlDbType.VarChar, 255).Value = fullName ?? string.Empty;
                cmd.Parameters.Add("@yl", MySqlDbType.VarChar, 64).Value = (object)yearLevel ?? DBNull.Value;
                cmd.Parameters.Add("@cr", MySqlDbType.VarChar, 512).Value = (object)course ?? DBNull.Value;
                cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;
                cmd.ExecuteNonQuery();
            }
        }

        private static UserRecord Map(MySqlDataReader r)
        {
            return new UserRecord
            {
                Id = r.GetInt64("id"),
                StudentNumber = r.GetString("student_number"),
                Email = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
                PasswordHash = (byte[])r["password_hash"],
                PasswordSalt = (byte[])r["password_salt"],
                FullName = r.GetString("full_name"),
                YearLevel = r.IsDBNull(r.GetOrdinal("year_level")) ? null : r.GetString("year_level"),
                Course = r.IsDBNull(r.GetOrdinal("course")) ? null : r.GetString("course"),
                RoleId = r.GetByte("role_id"),
                EcoPointsBalance = r.GetInt32("eco_points_balance"),
                IsActive = r.GetBoolean("is_active"),
                CreatedAt = r.GetDateTime("created_at"),
                UpdatedAt = r.GetDateTime("updated_at")
            };
        }
    }
}
