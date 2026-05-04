namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface IAuditLogRepository
    {
        void Insert(long? userId, string action, string entityType, string entityId, string details);
    }
}
