using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPerformance.Models
{
    public class TPContext:DbContext
    {
        public TPContext(DbContextOptions options):base(options)
        {
            // Configuration    
        }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Topic> topics { get; set; }
    }
}
