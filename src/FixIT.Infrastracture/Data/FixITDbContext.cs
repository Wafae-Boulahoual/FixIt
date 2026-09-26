using FixIT.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FixIT.Infrastracture.Data
{
    public class FixITDbContext(DbContextOptions<FixITDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<ServiceRequests> ServiceRequests => Set<ServiceRequests>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ServiceRequests>(entity =>
            {
                entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).HasMaxLength(2000);
                entity.Property(r => r.Adress).HasMaxLength(300);
            });
        }
    }
}
