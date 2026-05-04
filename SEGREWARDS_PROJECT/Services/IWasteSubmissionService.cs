namespace SEGREWARDS_PROJECT.Services
{
    public interface IWasteSubmissionService
    {
        WasteSubmitResult Submit(WasteSubmitRequest request);
    }
}
