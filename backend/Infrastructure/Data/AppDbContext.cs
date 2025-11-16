using Microsoft.EntityFrameworkCore;
using ContactApp.Core.Entities;

namespace ContactApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().ToTable("companies");
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<ContactInfo>().ToTable("contact_infos");
            modelBuilder.Entity<User>().ToTable("users");
        }

    }
}
