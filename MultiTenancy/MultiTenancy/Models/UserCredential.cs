namespace MultiTenancy.Models
{
    public class UserCredential
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
