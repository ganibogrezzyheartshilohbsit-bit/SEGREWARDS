using System;

namespace SEGREWARDS_PROJECT.Models
{
    public sealed class UserRecord
    {
        public long Id { get; set; }
        public string StudentNumber { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string YearLevel { get; set; }
        public string Course { get; set; }
        public byte RoleId { get; set; }
        public int EcoPointsBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
