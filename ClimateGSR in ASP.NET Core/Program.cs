using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClimateGSR_in_ASP.NET_Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ClimateGSRDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ClimateGSRDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddScoped<LeaderboardService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations and seed climate data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClimateGSRDbContext>();
    db.Database.Migrate();
    DatabaseInitialiser.Seed(db, app.Environment.ContentRootPath);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
