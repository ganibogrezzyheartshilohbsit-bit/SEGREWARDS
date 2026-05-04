using SEGREWARDS_PROJECT.Data;
using SEGREWARDS_PROJECT.Data.Repositories;
using SEGREWARDS_PROJECT.Infrastructure;
using SEGREWARDS_PROJECT.Services;

namespace SEGREWARDS_PROJECT.Composition
{
    /// <summary>
    /// Manual composition root for WinForms (.NET Framework) without external DI containers.
    /// </summary>
    public sealed class AppCompositionRoot
    {
        public static readonly AppCompositionRoot Instance = new AppCompositionRoot();

        private AppCompositionRoot()
        {
            var connectionString = DatabaseConfig.GetConnectionString();
            ConnectionFactory = new MySqlConnectionFactory(connectionString);

            Users = new UserRepository(ConnectionFactory);
            WasteTypes = new WasteTypeRepository(ConnectionFactory);
            WasteSubmissions = new WasteSubmissionRepository(ConnectionFactory);
            RewardCatalog = new RewardCatalogRepository(ConnectionFactory);
            AuditLog = new AuditLogRepository(ConnectionFactory);
            Leaderboard = new LeaderboardRepository(ConnectionFactory);

            Auth = new AuthService(Users, AuditLog);
            WasteSubmission = new WasteSubmissionService(Users, WasteTypes, WasteSubmissions, AuditLog);
            RewardSearch = new RewardSearchService(RewardCatalog);
        }

        public IDbConnectionFactory ConnectionFactory { get; }

        public IUserRepository Users { get; }
        public IWasteTypeRepository WasteTypes { get; }
        public IWasteSubmissionRepository WasteSubmissions { get; }
        public IRewardCatalogRepository RewardCatalog { get; }
        public IAuditLogRepository AuditLog { get; }
        public ILeaderboardRepository Leaderboard { get; }

        public IAuthService Auth { get; }
        public IWasteSubmissionService WasteSubmission { get; }
        public IRewardSearchService RewardSearch { get; }
    }
}
