namespace SEGREWARDS_PROJECT.Models
{
    public sealed class LeaderboardRow
    {
        public long UserId { get; set; }
        public string StudentNumber { get; set; }
        public string FullName { get; set; }
        public int EcoPointsBalance { get; set; }
        public int SubmissionCount { get; set; }
        public int ApprovedSubmissions { get; set; }
    }
}
