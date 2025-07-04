using DemoLib.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<ServiceMessage> ServiceMessages { get; set; }
        public DbSet<PageView> PageViews { get; set; }
        public DbSet<Email> Emails { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>()
                .HasOne(e => e.ServiceMessage)
                .WithMany()
                .HasForeignKey(e => e.MessageId);
        }
    } 
}
