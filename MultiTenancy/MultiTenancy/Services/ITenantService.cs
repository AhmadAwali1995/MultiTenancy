namespace MultiTenancy.Services
{
    public interface ITenantService
    {
        string GetDatabaseProvider();
        string GetConnectionString();
        TenantModel GetCurrentTenant();
        List<Tenant> GetTenantList();
    }
}
