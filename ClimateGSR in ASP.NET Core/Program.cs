using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClimateGSR_in_ASP.NET_Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ClimateGSRDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Matches the MVC 5 template: users sign in with their email, so it must be unique
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ClimateGSRDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

// Your controllers use /Account/Login and /Account/LogOff (Core's defaults are Login/Logout)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOff";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddScoped<LeaderboardService>();

// Session stores the game in progress (GameController)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

// Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
