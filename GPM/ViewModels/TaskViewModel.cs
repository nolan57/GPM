using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPM.Models;
using GPM.Services;
using Task = GPM.Models.Task;

namespace GPM.ViewModels
{
    public class TaskViewModel : BindableBase
    {
        private readonly ITaskService _taskService;

        private List<Task> _tasks = null!;
        public List<Task> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        public TaskViewModel(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public void LoadTasks(int projectId)
        {
            Tasks = _taskService.GetTasksByProjectId(projectId);
        }
    }
}