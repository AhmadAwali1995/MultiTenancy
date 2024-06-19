namespace MultiTenancy.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string TID { get; set; } = null!;
        public string? ConnectionString { get; set; }
    }
}
