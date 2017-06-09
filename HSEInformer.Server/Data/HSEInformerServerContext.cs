using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HSEInformer.Server.Models
{
    public class HSEInformerServerContext : DbContext
    {
        public HSEInformerServerContext (DbContextOptions<HSEInformerServerContext> options)
            : base(options)
        {
        }

        public DbSet<HSEInformer.Server.Models.User> Users { get; set; }

        public DbSet<HSEInformer.Server.Models.Confirmation> Confirmations { get; set; }
    }
}
