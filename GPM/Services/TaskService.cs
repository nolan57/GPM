using GPM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public class TaskService : ITaskService
    {
        public List<TaskModel> GetTasksByProjectId(int projectId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TaskModels.ToList();
            }
        }

        public TaskModel GetTaskById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TaskModels.Find(id);
            }
        }

        public void AddTask(TaskModel task)
        {
            using (var context = new ApplicationDbContext())
            {
                context.TaskModels.Add(task);
                context.SaveChanges();
            }
        }

        public void UpdateTask(TaskModel task)
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
                var task = context.TaskModels.Find(id);
                if (task != null)
                {
                    context.TaskModels.Remove(task);
                    context.SaveChanges();
                }
            }
        }
    }
}