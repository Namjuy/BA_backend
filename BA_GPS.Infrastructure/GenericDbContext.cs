using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   05/04/2024 Created
/// </Modified>
namespace BA_GPS.Infrastructure
{
    public class GenericDbContext : DbContext
    {
        public GenericDbContext(DbContextOptions<GenericDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(up => new { up.UserId, up.PermissionId });
            });

        }


    }
}
    