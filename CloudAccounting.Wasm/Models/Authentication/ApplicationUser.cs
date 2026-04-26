namespace CloudAccounting.Wasm.Models.Authentication
{
    public class ApplicationUser
    {
        public string UserId { get; set; } = string.Empty;
        public int CompanyCode { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public Int16 CompanyYear { get; set; }
        public byte CompanyMonthId { get; set; }
        public string CompanyMonthName { get; set; } = string.Empty;
        public Int16 GroupId { get; set; }
        public string? Admin { get; set; } = string.Empty;
        public string GroupTitle { get; set; } = string.Empty;
    }
}
