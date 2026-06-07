using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Desk.Models;
using Desk.Models.TestCategoryA;
using Desk.Models.Device.MultiFunctionPrinter;
using Desk.Models.Authorization;

namespace Desk.Data
{
    public class DeskContext : DbContext
    {

        // CATEGORIES (only single form of the noun!) - Special Properties
        public DbSet<KonicaMinolta> KonicaMinolta { get; set; } = default!;
        public DbSet<Xerox> Xerox { get; set; } = default!;
        public DbSet<NewPassword> NewPassword { get; set; } = default!;


        // TECHNICAL TABLES (not special properties)
        public DbSet<ADUser> ADUsers { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<Supplier> Suppliers { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<UserSetting> UserSettings { get; set; } = default!;


        // MANY TO MANY RELATIONSHIPS
        public DbSet<UserGroup> UserGroups { get; set; } = default!;


        public DeskContext(DbContextOptions<DeskContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            // OnDelete behaviors
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToOperator)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToOperatorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ReportedBy)
                .WithMany(u => u.ReportedTickets)
                .HasForeignKey(t => t.ReportedById)
                .OnDelete(DeleteBehavior.NoAction);

            //Many to many relationships
            modelBuilder.Entity<UserGroup>().HasKey(ug => new
            {
                ug.UserId, 
                ug.GroupId
            });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);
            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
