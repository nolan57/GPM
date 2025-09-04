using GPM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public class ProjectService : IProjectService
    {
        public List<Project> GetProjects()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Projects.Include(p => p.Tasks).ThenInclude(t => t.SubTasks).ToList();
            }
        }

        public Project GetProjectById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Projects.Include(p => p.Tasks).ThenInclude(t => t.SubTasks).FirstOrDefault(p => p.Id == id);
            }
        }

        public void AddProject(Project project)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Projects.Add(project);
                context.SaveChanges();
            }
        }

        public void UpdateProject(Project project)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Entry(project).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void DeleteProject(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var project = context.Projects.Find(id);
                if (project != null)
                {
                    context.Projects.Remove(project);
                    context.SaveChanges();
                }
            }
        }
    }
}