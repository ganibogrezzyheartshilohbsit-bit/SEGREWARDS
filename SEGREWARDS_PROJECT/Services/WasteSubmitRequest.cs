namespace SEGREWARDS_PROJECT.Services
{
    public sealed class WasteSubmitRequest
    {
        public string StudentNumber { get; set; }
        public string FullName { get; set; }
        public string YearLevel { get; set; }
        public string Course { get; set; }
        public string WasteTypeCode { get; set; }
        public byte[] ProofImageBytes { get; set; }
        public string ProofMimeType { get; set; }
        public string ProofFileName { get; set; }
    }
}
