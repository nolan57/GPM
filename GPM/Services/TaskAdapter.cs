using System.Collections.ObjectModel;
using System.Windows.Media;
using GPM.Models;
using GanttChartFramework.Models;

namespace GPM.Services
{
    /// <summary>
    /// Adapter service to convert between GPM models and GanttChartFramework models
    /// </summary>
    public static class TaskAdapter
    {
        /// <summary>
        /// Convert GPM Task to GanttChartFramework TaskItem
        /// </summary>
        public static TaskItem ConvertToTaskItem(Models.Task gpmTask)
        {
            return new TaskItem
            {
                Id = gpmTask.Id,
                Name = gpmTask.Name,
                StartDay = gpmTask.StartDate,
                Duration = (int)(gpmTask.EndDate - gpmTask.StartDate).TotalDays,
                Progress = gpmTask.Progress,
                Color = GetTaskBrush(gpmTask.Progress)
            };
        }

        /// <summary>
        /// Convert GPM Task to GanttChartFramework EnhancedTaskItem
        /// </summary>
        public static EnhancedTaskItem ConvertToEnhancedTaskItem(Models.Task gpmTask)
        {
            var enhancedTask = new EnhancedTaskItem
            {
                Id = gpmTask.Id,
                Name = gpmTask.Name,
                Description = gpmTask.Description,
                StartDate = gpmTask.StartDate,
                EndDate = gpmTask.EndDate,
                Progress = gpmTask.Progress,
                Priority = DetermineTaskPriority(gpmTask),
                Status = DetermineTaskStatus(gpmTask),
                Color = GetTaskColor(gpmTask.Progress)
            };

            return enhancedTask;
        }

        /// <summary>
        /// Convert collection of GPM Tasks to TaskItems
        /// </summary>
        public static ObservableCollection<TaskItem> ConvertToTaskItems(IEnumerable<Models.Task> gpmTasks)
        {
            var taskItems = new ObservableCollection<TaskItem>();
            foreach (var task in gpmTasks)
            {
                taskItems.Add(ConvertToTaskItem(task));
            }
            return taskItems;
        }

        /// <summary>
        /// Convert collection of GPM Tasks to EnhancedTaskItems
        /// </summary>
        public static ObservableCollection<EnhancedTaskItem> ConvertToEnhancedTaskItems(IEnumerable<Models.Task> gpmTasks)
        {
            var taskItems = new ObservableCollection<EnhancedTaskItem>();
            foreach (var task in gpmTasks)
            {
                taskItems.Add(ConvertToEnhancedTaskItem(task));
            }
            return taskItems;
        }

        /// <summary>
        /// Convert EnhancedTaskItem back to GPM Task
        /// </summary>
        public static Models.Task ConvertFromEnhancedTaskItem(EnhancedTaskItem enhancedTask)
        {
            return new Models.Task
            {
                Id = enhancedTask.Id,
                Name = enhancedTask.Name,
                Description = enhancedTask.Description,
                StartDate = enhancedTask.StartDate,
                EndDate = enhancedTask.EndDate,
                Progress = enhancedTask.Progress
            };
        }

        /// <summary>
        /// Get task color based on progress
        /// </summary>
        private static Color GetTaskColor(double progress)
        {
            return progress switch
            {
                100 => Color.FromRgb(76, 175, 80),   // Green - Completed
                >= 80 => Color.FromRgb(33, 150, 243), // Blue - Nearly completed
                >= 50 => Color.FromRgb(255, 152, 0),  // Orange - In progress
                >= 25 => Color.FromRgb(255, 193, 7),  // Amber - Started
                _ => Color.FromRgb(244, 67, 54)       // Red - Not started/Behind
            };
        }

        /// <summary>
        /// Get task brush based on progress
        /// </summary>
        private static SolidColorBrush GetTaskBrush(double progress)
        {
            return new SolidColorBrush(GetTaskColor(progress));
        }

        /// <summary>
        /// Determine task priority based on various factors
        /// </summary>
        private static TaskPriority DetermineTaskPriority(Models.Task gpmTask)
        {
            // Simple logic based on timeline and progress
            var timeRemaining = (gpmTask.EndDate - DateTime.Now).TotalDays;
            var progressRatio = gpmTask.Progress / 100.0;
            var expectedProgress = Math.Max(0, 100 - (timeRemaining / (gpmTask.EndDate - gpmTask.StartDate).TotalDays * 100));

            if (gpmTask.Progress < expectedProgress * 0.5)
                return TaskPriority.Critical;
            else if (gpmTask.Progress < expectedProgress * 0.8)
                return TaskPriority.High;
            else if (timeRemaining < 3)
                return TaskPriority.High;
            else
                return TaskPriority.Normal;
        }

        /// <summary>
        /// Determine task status based on progress and dates
        /// </summary>
        private static GanttChartFramework.Models.TaskStatus DetermineTaskStatus(Models.Task gpmTask)
        {
            if (gpmTask.Progress >= 100)
                return GanttChartFramework.Models.TaskStatus.Completed;
            else if (gpmTask.Progress > 0)
                return GanttChartFramework.Models.TaskStatus.InProgress;
            else if (DateTime.Now > gpmTask.StartDate)
                return GanttChartFramework.Models.TaskStatus.InProgress;
            else
                return GanttChartFramework.Models.TaskStatus.NotStarted;
        }

        /// <summary>
        /// Create sample tasks for demonstration
        /// </summary>
        public static ObservableCollection<Models.Task> CreateSampleTasks()
        {
            var now = DateTime.Now;
            return new ObservableCollection<Models.Task>
            {
                new Models.Task
                {
                    Id = 1,
                    Name = "项目启动",
                    Description = "项目启动和需求收集阶段",
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(2),
                    Progress = 85,
                    ProjectId = 1
                },
                new Models.Task
                {
                    Id = 2,
                    Name = "系统设计",
                    Description = "系统架构和详细设计",
                    StartDate = now.AddDays(1),
                    EndDate = now.AddDays(8),
                    Progress = 30,
                    ProjectId = 1
                },
                new Models.Task
                {
                    Id = 3,
                    Name = "前端开发",
                    Description = "用户界面开发",
                    StartDate = now.AddDays(6),
                    EndDate = now.AddDays(18),
                    Progress = 0,
                    ProjectId = 1
                },
                new Models.Task
                {
                    Id = 4,
                    Name = "后端开发",
                    Description = "服务端接口开发",
                    StartDate = now.AddDays(8),
                    EndDate = now.AddDays(20),
                    Progress = 0,
                    ProjectId = 1
                },
                new Models.Task
                {
                    Id = 5,
                    Name = "系统测试",
                    Description = "功能测试和集成测试",
                    StartDate = now.AddDays(18),
                    EndDate = now.AddDays(25),
                    Progress = 0,
                    ProjectId = 1
                },
                new Models.Task
                {
                    Id = 6,
                    Name = "项目部署",
                    Description = "生产环境部署",
                    StartDate = now.AddDays(24),
                    EndDate = now.AddDays(27),
                    Progress = 0,
                    ProjectId = 1
                }
            };
        }
    }
}
