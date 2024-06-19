using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Authentication;
using MultiTenancy.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<TenancyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TenancyDatabase")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(nameof(TenantSettings)));
TenantSettings options = new();
builder.Configuration.GetSection(nameof(TenantSettings)).Bind(options);

var defaultDbProvicer = options.Defaults.DBProvider;
if(defaultDbProvicer.ToLower() == "mysql")
{
    builder.Services.AddDbContext<ApplicationDbContext>(m => m.UseNpgsql());
}

var tenantScope = builder.Services.BuildServiceProvider().CreateScope();
var tenantService = tenantScope.ServiceProvider.GetRequiredService<ITenantService>();
foreach(var tenant in tenantService.GetTenantList())
{
    options.Tenants.Add(new TenantModel
    {
        ConnectionString = tenant.ConnectionString,
        Name = tenant.Name,
        TID = tenant.TID
    });
}

foreach (var tenant in options.Tenants)
{
    var connectionString = tenant.ConnectionString ?? options.Defaults.ConnectionString;
    using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.SetConnectionString(connectionString);
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
