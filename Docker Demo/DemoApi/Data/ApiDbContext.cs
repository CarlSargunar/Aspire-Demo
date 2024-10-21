using DemoLib.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Analytics> Analytics { get; set; }
        public DbSet<Email> Emails { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    } 
}
