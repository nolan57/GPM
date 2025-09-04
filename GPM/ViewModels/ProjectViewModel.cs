using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPM.Models;
using GPM.Services;

namespace GPM.ViewModels
{
    public class ProjectViewModel : BindableBase
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;

        private string _projectName = "Sample Project";
        public string ProjectName
        {
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
        }

        private List<Project> _projects = null!;
        public List<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        private TaskViewModel _taskViewModel = null!;
        public TaskViewModel TaskViewModel
        {
            get { return _taskViewModel; }
            set { SetProperty(ref _taskViewModel, value); }
        }

        public ProjectViewModel(IProjectService projectService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
            Projects = _projectService.GetProjects();
            TaskViewModel = new TaskViewModel(_taskService);
        }
    }
}