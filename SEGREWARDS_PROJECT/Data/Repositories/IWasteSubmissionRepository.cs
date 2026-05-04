namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface IWasteSubmissionRepository
    {
        long InsertPending(
            long userId,
            short wasteTypeId,
            string studentNumberSnapshot,
            string fullNameSnapshot,
            string yearLevelSnapshot,
            string courseSnapshot,
            byte[] proofImage,
            string proofMimeType,
            string proofOriginalName);
    }
}
