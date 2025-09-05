using GPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = GPM.Models.Task;

namespace GPM.Services
{
    public interface ITaskService
    {
        List<Task> GetTasksByProjectId(int projectId);
        Task GetTaskById(int id);
        void AddTask(Task task);
        void UpdateTask(Task task);
        void DeleteTask(int id);
    }
}