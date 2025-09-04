using GPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public interface IProjectService
    {
        List<Project> GetProjects();
        Project GetProjectById(int id);
        void AddProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(int id);
    }
}