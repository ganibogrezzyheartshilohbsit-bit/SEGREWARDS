using System;

namespace SEGREWARDS_PROJECT.Models
{
    public sealed class WasteSubmissionRecord
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public short WasteTypeId { get; set; }
        public string StudentNumberSnapshot { get; set; }
        public string FullNameSnapshot { get; set; }
        public string YearLevelSnapshot { get; set; }
        public string CourseSnapshot { get; set; }
        public string Status { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
