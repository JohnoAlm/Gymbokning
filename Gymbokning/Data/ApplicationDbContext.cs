using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gymbokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gymbokning.Models.GymClass> GymClass { get; set; }

        public DbSet <Gymbokning.Models.ApplicationUserGymClass> ApplicationUserGymClass { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FluentAPI
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(t => new { t.ApplicationUserId, t.GymClassId });
                
        }
    }
}