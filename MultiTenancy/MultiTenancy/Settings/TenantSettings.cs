namespace MultiTenancy.Settings
{
    public class TenantSettings
    {
        public Configuration Defaults { get; set; } = default!;
        public List<TenantModel> Tenants { get; set; } = new();
    }
}
