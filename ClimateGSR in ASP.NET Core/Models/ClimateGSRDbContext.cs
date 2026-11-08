using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Entity Framework database context for the ClimateGSR application, managing ASP.NET Identity
    /// user accounts as well as Country and Continent datasets.
    /// </summary>
    public class ClimateGSRDbContext : IdentityDbContext<ApplicationUser>
    {
        public ClimateGSRDbContext(DbContextOptions<ClimateGSRDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries => Set<Country>();

        public DbSet<Continent> Continents => Set<Continent>();

        public DbSet<Member> Members => Set<Member>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // required for Identity tables

            // Replaces the EF6 [Index] attribute: one Member row per user.
            builder.Entity<Member>()
                .HasIndex(m => m.ApplicationUserId)
                .IsUnique()
                .HasDatabaseName("IX_Member_ApplicationUserId");
        }
    }
}
