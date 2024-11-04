using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Security
{
    public class SecurityContext: IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public SecurityContext(DbContextOptions<SecurityContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUserRole>().HasKey(x=> new 
            {
                x.ApplicationRoleID
                ,x.ApplicationUserID
            });
            builder.Entity<ApplicationRole>()
                .Property(x => x.Description).HasMaxLength(50);
            base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>().HasMany(x=>x.ApplicationUserRoles)
                .WithOne(x => x.ApplicationRole).HasForeignKey(x=>x.ApplicationRoleID);

            builder.Entity<ApplicationUser>()
                .HasMany(x=>x.ApplicationUserRoles)
                .WithOne(x=>x.ApplicationUser)
                .HasForeignKey(x=>x.ApplicationUserID);

        }
    }
}
