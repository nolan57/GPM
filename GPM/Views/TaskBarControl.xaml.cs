using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GPM.Views
{
    public partial class TaskBarControl : UserControl
    {
        #region 依赖属性
        
        public static readonly DependencyProperty TasksProperty = 
            DependencyProperty.Register("Tasks", typeof(List<TaskItem>), typeof(TaskBarControl),
                new PropertyMetadata(new List<TaskItem>(), OnTasksChanged));
        
        public static readonly DependencyProperty TimeScaleTypeProperty = 
            DependencyProperty.Register("TimeScaleType", typeof(string), typeof(TaskBarControl),
                new PropertyMetadata("Day", OnTimeScaleTypeChanged));
        
        public static readonly DependencyProperty TaskHeightProperty = 
            DependencyProperty.Register("TaskHeight", typeof(double), typeof(TaskBarControl),
                new PropertyMetadata(40.0, OnTaskHeightChanged));
        
        public static readonly DependencyProperty ShowTaskLabelsProperty = 
            DependencyProperty.Register("ShowTaskLabels", typeof(bool), typeof(TaskBarControl),
                new PropertyMetadata(true, OnShowTaskLabelsChanged));
        
        public static readonly DependencyProperty CanvasWidthProperty = 
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(TaskBarControl),
                new PropertyMetadata(2400.0, OnCanvasSizeChanged));
        
        public static readonly DependencyProperty CanvasHeightProperty = 
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(TaskBarControl),
                new PropertyMetadata(1800.0, OnCanvasSizeChanged));
        
        public static readonly DependencyProperty IsHierarchicalProperty = 
            DependencyProperty.Register("IsHierarchical", typeof(bool), typeof(TaskBarControl),
                new PropertyMetadata(true, OnIsHierarchicalChanged));
        
        #endregion
        
        #region 属性包装器
        
        public List<TaskItem> Tasks
        {
            get => (List<TaskItem>)GetValue(TasksProperty);
            set => SetValue(TasksProperty, value);
        }
        
        public string TimeScaleType
        {
            get => (string)GetValue(TimeScaleTypeProperty);
            set => SetValue(TimeScaleTypeProperty, value);
        }
        
        public double TaskHeight
        {
            get => (double)GetValue(TaskHeightProperty);
            set => SetValue(TaskHeightProperty, value);
        }
        
        public bool ShowTaskLabels
        {
            get => (bool)GetValue(ShowTaskLabelsProperty);
            set => SetValue(ShowTaskLabelsProperty, value);
        }
        
        public double CanvasWidth
        {
            get => (double)GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }
        
        public double CanvasHeight
        {
            get => (double)GetValue(CanvasHeightProperty);
            set => SetValue(CanvasHeightProperty, value);
        }
        
        public bool IsHierarchical
        {
            get => (bool)GetValue(IsHierarchicalProperty);
            set => SetValue(IsHierarchicalProperty, value);
        }
        
        #endregion
        
        public TaskBarControl()
        {
            InitializeComponent();
            InitializeTasks();
        }
        
        private static void OnTasksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.RedrawTasks();
        }
        
        private static void OnTimeScaleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.RedrawTasks();
        }
        
        private static void OnTaskHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.RedrawTasks();
        }
        
        private static void OnShowTaskLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.RedrawTasks();
        }
        
        private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.UpdateCanvasSize();
            control?.RedrawTasks();
        }
        
        private static void OnIsHierarchicalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskBarControl;
            control?.RedrawTasks();
        }
        
        private void UpdateCanvasSize()
        {
            TaskCanvas.Width = CanvasWidth;
            TaskCanvas.Height = CanvasHeight;
        }
        
        public void InitializeTasks()
        {
            UpdateCanvasSize();
            // 如果没有任务，绘制示例任务
            if (Tasks == null || Tasks.Count == 0)
            {
                DrawSampleTasks();
            }
            else
            {
                RedrawTasks();
            }
        }
        
        public void RedrawTasks()
        {
            TaskCanvas.Children.Clear();
            
            if (Tasks == null || Tasks.Count == 0)
            {
                DrawSampleTasks();
                return;
            }
            
            if (IsHierarchical)
            {
                // 层级显示模式
                DrawHierarchicalTasks();
            }
            else
            {
                // 平铺显示模式
                DrawFlatTasks();
            }
        }
        
        private void DrawSampleTasks()
        {
            // 创建示例任务数据
            var sampleTasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Name = "需求分析", StartDay = 1, Duration = 5, ParentId = null, Color = Colors.Blue, Progress = 100 },
                new TaskItem { Id = 2, Name = "设计开发", StartDay = 6, Duration = 10, ParentId = null, Color = Colors.Green, Progress = 75 },
                new TaskItem { Id = 3, Name = "前端开发", StartDay = 6, Duration = 8, ParentId = 2, Color = Colors.LightGreen, Progress = 90 },
                new TaskItem { Id = 4, Name = "后端开发", StartDay = 8, Duration = 9, ParentId = 2, Color = Colors.DarkGreen, Progress = 60 },
                new TaskItem { Id = 5, Name = "测试验证", StartDay = 16, Duration = 5, ParentId = null, Color = Colors.Orange, Progress = 20 },
                new TaskItem { Id = 6, Name = "部署上线", StartDay = 21, Duration = 3, ParentId = null, Color = Colors.Purple, Progress = 0 }
            };
            
            // 绘制示例任务
            if (IsHierarchical)
            {
                DrawHierarchicalTasks(sampleTasks);
            }
            else
            {
                DrawFlatTasks(sampleTasks);
            }
        }
        
        private void DrawHierarchicalTasks(List<TaskItem> tasks = null)
        {
            var targetTasks = tasks ?? Tasks;
            
            // 找出根任务
            var rootTasks = targetTasks.FindAll(t => t.ParentId == null);
            
            double currentY = 10;
            
            // 绘制根任务和子任务
            foreach (var rootTask in rootTasks)
            {
                // 绘制根任务
                DrawTaskBar(rootTask, currentY, 0);
                currentY += TaskHeight;
                
                // 查找并绘制子任务
                DrawChildTasks(rootTask.Id, targetTasks, ref currentY, 20);
            }
        }
        
        private void DrawChildTasks(int parentId, List<TaskItem> tasks, ref double currentY, double indentation)
        {
            var childTasks = tasks.FindAll(t => t.ParentId == parentId);
            
            foreach (var childTask in childTasks)
            {
                // 绘制子任务
                DrawTaskBar(childTask, currentY, indentation);
                currentY += TaskHeight;
                
                // 递归绘制子任务的子任务
                DrawChildTasks(childTask.Id, tasks, ref currentY, indentation + 20);
            }
        }
        
        private void DrawFlatTasks(List<TaskItem> tasks = null)
        {
            var targetTasks = tasks ?? Tasks;
            
            double currentY = 10;
            
            // 平铺绘制所有任务
            foreach (var task in targetTasks)
            {
                DrawTaskBar(task, currentY, 0);
                currentY += TaskHeight;
            }
        }
        
        private void DrawTaskBar(TaskItem task, double y, double indentation)
        {
            // 根据时间粒度计算任务条宽度
            double widthPerDay = 60; // 默认日粒度
            
            switch (TimeScaleType)
            {
                case "Week":
                    widthPerDay = 30;
                    break;
                case "Month":
                    widthPerDay = 10;
                    break;
                case "Quarter":
                    widthPerDay = 2.5;
                    break;
            }
            
            double taskWidth = task.Duration * widthPerDay;
            double taskX = 50 + (task.StartDay - 1) * widthPerDay + indentation;
            
            // 创建任务条背景
            var taskBarBackground = new Rectangle
            {
                Width = taskWidth,
                Height = TaskHeight - 10,
                Fill = new SolidColorBrush(Color.FromArgb(200, task.Color.R, task.Color.G, task.Color.B)),
                Stroke = new SolidColorBrush(task.Color),
                StrokeThickness = 1,
                RadiusX = 3,
                RadiusY = 3,
                Margin = new Thickness(taskX, y + 5, 0, 0)
            };
            
            // 创建进度条
            if (task.Progress > 0)
            {
                var progressBar = new Rectangle
                {
                    Width = taskWidth * task.Progress / 100,
                    Height = TaskHeight - 10,
                    Fill = new SolidColorBrush(Color.FromArgb(150, task.Color.R, task.Color.G, task.Color.B)),
                    RadiusX = 3,
                    RadiusY = 3,
                    Margin = new Thickness(taskX, y + 5, 0, 0)
                };
                
                TaskCanvas.Children.Add(progressBar);
            }
            
            TaskCanvas.Children.Add(taskBarBackground);
            
            // 创建任务标签
            if (ShowTaskLabels && !string.IsNullOrEmpty(task.Name))
            {
                var taskLabel = new TextBlock
                {
                    Text = task.Name,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(taskX + 5, y + 8, 0, 0),
                    MaxWidth = taskWidth - 10
                };
                
                TaskCanvas.Children.Add(taskLabel);
            }
        }
    }
    
    // 任务项数据结构
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartDay { get; set; }
        public int Duration { get; set; }
        public int? ParentId { get; set; }
        public Color Color { get; set; }
        public int Progress { get; set; }
    }
}