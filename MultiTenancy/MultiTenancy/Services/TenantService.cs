using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiTenancy.Data;
using MultiTenancy.Settings;
using System.Net.Http.Headers;
using System.Text;

namespace MultiTenancy.Services
{
    public class TenantService : ITenantService
    {
        private HttpContext? _httpContext;
        private TenantModel? _currentTenant;
        private TenantSettings? _tenantSettings;
        private TenancyDbContext _tenancyDbContext;

        public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSettings> tenantSettings, TenancyDbContext tenancyDbContext)
        {
            _tenancyDbContext = tenancyDbContext;
            _httpContext = contextAccessor.HttpContext;
            var tenants = GetTenantList();
            foreach (var tenant in tenants)
            {
                tenantSettings.Value.Tenants.Add(new TenantModel
                {
                    ConnectionString = tenant.ConnectionString,
                    Name = tenant.Name,
                    TID = tenant.TID
                });
            }
            _tenantSettings = tenantSettings.Value;

            if (contextAccessor.HttpContext is not null)
            {
                var authHeader = AuthenticationHeaderValue.Parse(_httpContext.Request.Headers["Authorization"]);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                string username = credentials[0];
                string password = credentials[1];

                string tenantId = GetTenant(username, password);
                if(tenantId == null)
                {
                    throw new Exception("No Tenant Provided");
                }

                SetCurrentTenant(tenantId!);

            }
            
        }
        public string? GetConnectionString()
        {
            var currentConnectionString = _currentTenant is null
                ? _tenantSettings.Defaults.ConnectionString
                : _currentTenant.ConnectionString;
            return currentConnectionString;
        }

        public TenantModel GetCurrentTenant()
        {
            return _currentTenant;
        }

        public string GetDatabaseProvider()
        {
            return _tenantSettings.Defaults.DBProvider;
        }

        private void SetCurrentTenant(string tenantId)
        {
            _currentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TID == tenantId);

            if (_currentTenant is null)
            {
                throw new Exception();
            }

            if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
            }
        }

        public List<Tenant> GetTenantList()
        {
            return _tenancyDbContext.Tenants.ToList();
        }

        private string? GetTenant(string username, string password)
        {
            string tenantId = null;
            var credential = _tenancyDbContext.UserCredentials.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
            if (credential != null)
            {
                tenantId = credential.Tenant.TID;
            }

            return tenantId;

        }
    }
}
