using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GPM.Models;

namespace GPM.ViewModels
{
    public class GanttChartViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Project> _projects;
        private ObservableCollection<Models.Task> _tasks;
        private string _selectedTimeScale;
        
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<Models.Task> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }
        
        public string SelectedTimeScale
        {
            get => _selectedTimeScale;
            set
            {
                _selectedTimeScale = value;
                OnPropertyChanged();
            }
        }
        
        public GanttChartViewModel()
        {
            InitializeData();
            SelectedTimeScale = "Day";
        }
        
        private void InitializeData()
        {
            // 初始化示例项目数据
            Projects = new ObservableCollection<Project>
            {
                new Project { Id = 1, Name = "项目A", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) },
                new Project { Id = 2, Name = "项目B", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(40) }
            };
            
            // 初始化示例任务数据
            Tasks = new ObservableCollection<Models.Task>
            {
                new Models.Task 
                { 
                    Id = 1, 
                    Name = "需求分析", 
                    StartDate = DateTime.Now, 
                    EndDate = DateTime.Now.AddDays(5),
                    Progress = 80,
                    ProjectId = 1
                },
                new Models.Task 
                { 
                    Id = 2, 
                    Name = "UI设计", 
                    StartDate = DateTime.Now.AddDays(3), 
                    EndDate = DateTime.Now.AddDays(10),
                    Progress = 50,
                    ProjectId = 1
                },
                new Models.Task 
                { 
                    Id = 3, 
                    Name = "开发实现", 
                    StartDate = DateTime.Now.AddDays(8), 
                    EndDate = DateTime.Now.AddDays(20),
                    Progress = 30,
                    ProjectId = 1
                },
                new Models.Task 
                { 
                    Id = 4, 
                    Name = "项目规划", 
                    StartDate = DateTime.Now.AddDays(5), 
                    EndDate = DateTime.Now.AddDays(12),
                    Progress = 100,
                    ProjectId = 2
                }
            };
        }
        
        public void ChangeTimeScale(string timeScale)
        {
            SelectedTimeScale = timeScale;
            // 这里可以添加时间粒度变化时的逻辑，比如重新计算任务位置等
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        // 计算任务在时间轴上的位置（第一阶段使用固定算法）
        public double CalculateTaskPosition(DateTime startDate, DateTime endDate)
        {
            var baseDate = DateTime.Now.Date;
            var daysFromBase = (startDate - baseDate).TotalDays;
            
            // 根据选择的时间粒度调整位置计算
            double positionMultiplier = SelectedTimeScale switch
            {
                "Day" => 60.0,      // 每天60像素
                "Week" => 150.0,    // 每周150像素
                "Month" => 200.0,   // 每月200像素
                "Quarter" => 150.0, // 每季度150像素
                _ => 60.0
            };
            
            return 50 + (daysFromBase * positionMultiplier);
        }
        
        // 计算任务宽度（第一阶段使用固定算法）
        public double CalculateTaskWidth(DateTime startDate, DateTime endDate)
        {
            var duration = (endDate - startDate).TotalDays;
            
            // 根据选择的时间粒度调整宽度计算
            double widthMultiplier = SelectedTimeScale switch
            {
                "Day" => 60.0,      // 每天60像素
                "Week" => 150.0,    // 每周150像素
                "Month" => 200.0,   // 每月200像素
                "Quarter" => 150.0, // 每季度150像素
                _ => 60.0
            };
            
            return duration * widthMultiplier;
        }
        
        // 获取任务颜色（根据进度）
        public string GetTaskColor(int progress)
        {
            return progress switch
            {
                100 => "#4CAF50", // 完成 - 绿色
                >= 80 => "#2196F3", // 接近完成 - 蓝色
                >= 50 => "#FF9800", // 进行中 - 橙色
                _ => "#F44336"     // 未开始/滞后 - 红色
            };
        }
    }
}