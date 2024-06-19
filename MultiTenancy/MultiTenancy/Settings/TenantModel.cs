namespace MultiTenancy.Settings
{
    public class TenantModel
    {
        public string Name { get; set; } = null!;
        public string TID { get; set; } = null!;
        public string? ConnectionString { get; set; }
    }
}
