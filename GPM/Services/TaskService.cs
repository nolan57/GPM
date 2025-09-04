using GPM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = GPM.Models.Task;

namespace GPM.Services
{
    public class TaskService : ITaskService
    {
        public List<Task> GetTasksByProjectId(int projectId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tasks
                    .Where(t => t.ProjectId == projectId)
                    .ToList();
            }
        }

        public Task GetTaskById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tasks.Find(id);
            }
        }

        public void AddTask(Task task)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }

        public void UpdateTask(Task task)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void DeleteTask(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var task = context.Tasks.Find(id);
                if (task != null)
                {
                    context.Tasks.Remove(task);
                    context.SaveChanges();
                }
            }
        }
    }
}