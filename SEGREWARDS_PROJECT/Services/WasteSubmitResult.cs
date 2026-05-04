namespace SEGREWARDS_PROJECT.Services
{
    public sealed class WasteSubmitResult
    {
        private WasteSubmitResult(bool success, string message, long? submissionId)
        {
            Success = success;
            Message = message;
            SubmissionId = submissionId;
        }

        public bool Success { get; }
        public string Message { get; }
        public long? SubmissionId { get; }

        public static WasteSubmitResult Ok(long submissionId, string message)
        {
            return new WasteSubmitResult(true, message, submissionId);
        }

        public static WasteSubmitResult Fail(string message)
        {
            return new WasteSubmitResult(false, message, null);
        }
    }
}
