using DemoLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor.Data
{
    internal class MessageProcessorDBContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Analytics> Analytics { get; set; }
        public DbSet<Email> Emails { get; set; }

        public MessageProcessorDBContext(DbContextOptions<MessageProcessorDBContext> options) : base(options)
        {
        }
    }
}
