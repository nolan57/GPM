using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GPM.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置实体关系
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne()
                .HasForeignKey("ProjectId");

            modelBuilder.Entity<Task>()
                .HasMany(t => t.SubTasks)
                .WithOne()
                .HasForeignKey("TaskId");


        }
    }
}