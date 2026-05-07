using System;
using MySql.Data.MySqlClient;
using SEGREWARDS_PROJECT.Data;

namespace SEGREWARDS_PROJECT.Services
{
    public sealed class RewardRedemptionService : IRewardRedemptionService
    {
        private readonly IDbConnectionFactory _connections;

        public RewardRedemptionService(IDbConnectionFactory connections)
        {
            _connections = connections;
        }

        public RedeemResult Redeem(long userId, short rewardCatalogId)
        {
            if (userId <= 0) return RedeemResult.Fail("Invalid user.");
            if (rewardCatalogId <= 0) return RedeemResult.Fail("Invalid reward.");

            using (var conn = _connections.CreateOpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    int cost;
                    string rewardName;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
SELECT points_cost, name
FROM reward_catalog
WHERE id = @rid AND is_active = 1
LIMIT 1;";
                        cmd.Parameters.Add("@rid", MySqlDbType.Int16).Value = rewardCatalogId;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                            {
                                return RedeemResult.Fail("That reward is not available.");
                            }

                            cost = r.GetInt32("points_cost");
                            rewardName = r.GetString("name");
                        }
                    }

                    int currentBalance;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
SELECT eco_points_balance
FROM users
WHERE id = @uid
LIMIT 1
FOR UPDATE;";
                        cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = userId;
                        var o = cmd.ExecuteScalar();
                        if (o == null || o == DBNull.Value)
                        {
                            return RedeemResult.Fail("User not found.");
                        }

                        currentBalance = Convert.ToInt32(o);
                    }

                    if (currentBalance < cost)
                    {
                        return RedeemResult.Fail("Not enough EcoPoints to redeem this reward.");
                    }

                    long redemptionId;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
INSERT INTO redemptions (user_id, reward_catalog_id, points_spent, status)
VALUES (@uid, @rid, @pts, 'completed');";
                        cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = userId;
                        cmd.Parameters.Add("@rid", MySqlDbType.Int16).Value = rewardCatalogId;
                        cmd.Parameters.Add("@pts", MySqlDbType.Int32).Value = cost;
                        cmd.ExecuteNonQuery();
                        redemptionId = cmd.LastInsertedId;
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
INSERT INTO eco_point_transactions (user_id, redemption_id, type, amount, description)
VALUES (@uid, @redId, 'redeem', @amt, @desc);";
                        cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = userId;
                        cmd.Parameters.Add("@redId", MySqlDbType.Int64).Value = redemptionId;
                        cmd.Parameters.Add("@amt", MySqlDbType.Int32).Value = -cost;
                        cmd.Parameters.Add("@desc", MySqlDbType.VarChar, 512).Value = "Redeemed reward: " + rewardName;
                        cmd.ExecuteNonQuery();
                    }

                    var newBalance = currentBalance - cost;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE users SET eco_points_balance = @b WHERE id = @uid;";
                        cmd.Parameters.Add("@b", MySqlDbType.Int32).Value = newBalance;
                        cmd.Parameters.Add("@uid", MySqlDbType.Int64).Value = userId;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return RedeemResult.Ok(newBalance);
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return RedeemResult.Fail(ex.Message);
                }
            }
        }
    }
}

