using DemoLib.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<ServiceMessage> Messages { get; set; }
        public DbSet<PageView> Analytics { get; set; }
        public DbSet<Email> Emails { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    } 
}
