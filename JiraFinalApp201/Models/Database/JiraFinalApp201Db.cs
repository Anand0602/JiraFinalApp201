using Microsoft.EntityFrameworkCore;
using JiraFinalApp201.Models.Authentication;
using JiraFinalApp201.Models.Projects;
using JiraFinalApp201.Models.Tasks;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.Models.Database
{
    public class JiraFinalApp201Db : DbContext
    {
        public JiraFinalApp201Db(DbContextOptions<JiraFinalApp201Db> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prevent deletion of users who reported tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Reporter)
                .WithMany( u => u.ReportedTasks)
                .HasForeignKey(t => t.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent deletion of users assigned to tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Delete tasks when their project is deleted
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task ID format constraints
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CONId)
                .IsRequired()
                .HasMaxLength(50);  
        }
    }
}
