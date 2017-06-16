using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HSEInformer.Server.Models
{
    public class HSEInformerServerContext : DbContext
    {
        public HSEInformerServerContext(DbContextOptions<HSEInformerServerContext> options)
            : base(options)
        {

        }


        public DbSet<HSEMember> HseMembers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Confirmation> Confirmations { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostPermission> PostPermissions { get; set; }

        public DbSet<PostPermissionRequest> PostPermissionRequests { get; set; }

        public DbSet<InviteToGroup> Invites { get; set; }

        public DbSet<Deadline> Deadlines { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(t => new { t.GroupId, t.UserId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
               .HasOne(ug => ug.Group)
               .WithMany(g => g.UserGroups)
               .HasForeignKey(ug => ug.GroupId);
        }

    }
}
