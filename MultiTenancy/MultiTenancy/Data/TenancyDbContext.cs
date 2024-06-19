using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Data
{
    public class TenancyDbContext : DbContext
    {
        public TenancyDbContext(DbContextOptions<TenancyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
    }
}
