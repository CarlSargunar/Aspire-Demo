using DemoLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor.Data;

public class WorkerContext : DbContext
{
    public DbSet<ServiceMessage> ServiceMessages { get; set; }
    public DbSet<PageView> PageViews { get; set; }
    public DbSet<Email> Emails { get; set; }

    public WorkerContext(DbContextOptions<WorkerContext> options) : base(options)
    {
    }
}