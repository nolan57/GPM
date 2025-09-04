using GPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public interface ITaskService
    {
        List<TaskModel> GetTasksByProjectId(int projectId);
        TaskModel GetTaskById(int id);
        void AddTask(TaskModel task);
        void UpdateTask(TaskModel task);
        void DeleteTask(int id);
    }
}