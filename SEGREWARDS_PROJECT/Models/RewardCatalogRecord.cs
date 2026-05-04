namespace SEGREWARDS_PROJECT.Models
{
    public sealed class RewardCatalogRecord
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PointsCost { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}
