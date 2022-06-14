using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppConfigFeatureFilteringDemo.Data;
using AppConfigFeatureFilteringDemo.TestFeatureFlags;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = "wwwroot",
    Args = args
});

builder.WebHost.ConfigureAppConfiguration(config =>
    {
        var settings = config.Build();
        var connection = settings.GetConnectionString("AppConfig");

        config.AddAzureAppConfiguration( (options) =>
            options 
                .Connect(connection)
                .UseFeatureFlags()
                .ConfigureRefresh(refresh =>
                {
                    refresh.SetCacheExpiration(new TimeSpan(0, 1, 0));
                })
        );
    }
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddFeatureManagement()
                .AddFeatureFilter<TargetingFilter>();

builder.Services.AddSingleton<ITargetingContextAccessor, BetaTargetingContextAccessor>();

var app = builder.Build();

app.UseMigrationsEndPoint();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
