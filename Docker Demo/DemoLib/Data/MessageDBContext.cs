using DemoLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DemoLib.Data
{
    public class MessageDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options)
        {
        }
    }
}
