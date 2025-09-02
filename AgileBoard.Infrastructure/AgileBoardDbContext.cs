using AgileBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure
{
    public class AgileBoardDbContext : DbContext
    {
        public AgileBoardDbContext(DbContextOptions<AgileBoardDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // many Project to many User
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Participants)
                .WithMany(u => u.ParticipatingProjects)
                .UsingEntity(j => j.ToTable("ProjectParticipants"));

            // many WorkItem to many User
            modelBuilder.Entity<WorkItem>()
                .HasMany(t => t.AssignedUsers)
                .WithMany(u => u.AssignedWorkItems)
                .UsingEntity(j => j.ToTable("WorkItemAssignedUsers"));

            // many WorkItem to many Tag
            modelBuilder.Entity<WorkItem>()
                .HasMany(t => t.Tags)
                .WithMany(u => u.WorkItems)
                .UsingEntity(j => j.ToTable("WorkItemTags"));

            // one User to many Project (Owner)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // one Project to many WorkItem
            modelBuilder.Entity<WorkItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.WorkItems)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // one Project to many Sprint
            modelBuilder.Entity<Sprint>()
                .HasOne(s => s.Project)
                .WithMany(p => p.Sprints)
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // one Sprint to many WorkItem
            modelBuilder.Entity<WorkItem>()
                .HasOne(t => t.Sprint)
                .WithMany(s => s.WorkItems)
                .HasForeignKey(t => t.SprintId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
