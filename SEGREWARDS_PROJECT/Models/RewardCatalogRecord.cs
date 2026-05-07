namespace SEGREWARDS_PROJECT.Models
{
    public sealed class RewardCatalogRecord
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PointsCost { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        public string DisplayText
        {
            get
            {
                var name = string.IsNullOrWhiteSpace(Name) ? "Reward" : Name.Trim();
                return name + " — " + PointsCost.ToString("N0") + " EcoPoints";
            }
        }

        public override string ToString()
        {
            // WinForms ListBox fallback if DisplayMember isn't applied for any reason.
            return DisplayText;
        }
    }
}
