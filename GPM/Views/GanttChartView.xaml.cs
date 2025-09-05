using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using GanttChartFramework.Models;
using GPM.Models;
using GPM.ViewModels;
using System.Windows.Data;

namespace GPM.Views
{
    public partial class GanttChartView : UserControl
    {
        #region 依赖属性
        
        // 数据上下文属性，用于绑定GanttChartViewModel
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(GanttChartViewModel), typeof(GanttChartView),
                new PropertyMetadata(null, OnViewModelChanged));
        
        
        // 时间轴样式属性
        public static readonly DependencyProperty TimeScaleBackgroundProperty =
            DependencyProperty.Register("TimeScaleBackground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new LinearGradientBrush(Color.FromRgb(245, 247, 250), Color.FromRgb(235, 238, 242), 90)));
        
        public static readonly DependencyProperty TimeScaleForegroundProperty =
            DependencyProperty.Register("TimeScaleForeground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(45, 62, 80))));
        
        public static readonly DependencyProperty TimeScaleFontSizeProperty =
            DependencyProperty.Register("TimeScaleFontSize", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(13.0));
        
        // 网格线样式属性
        public static readonly DependencyProperty GridLineColorProperty =
            DependencyProperty.Register("GridLineColor", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(230, 236, 241)));
        
        public static readonly DependencyProperty GridLineThicknessProperty =
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(0.8));
        
        public static readonly DependencyProperty GridLineDashArrayProperty =
            DependencyProperty.Register("GridLineDashArray", typeof(DoubleCollection), typeof(GanttChartView), 
                new PropertyMetadata(new DoubleCollection { 3, 3 }));
        
        public static readonly DependencyProperty HorizontalGridLineSpacingProperty =
            DependencyProperty.Register("HorizontalGridLineSpacing", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(45.0));
        
        // 任务条样式属性
        public static readonly DependencyProperty TaskBarCornerRadiusProperty =
            DependencyProperty.Register("TaskBarCornerRadius", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(8.0));
        
        public static readonly DependencyProperty TaskBarBorderThicknessProperty =
            DependencyProperty.Register("TaskBarBorderThickness", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty TaskBarBorderColorProperty =
            DependencyProperty.Register("TaskBarBorderColor", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(180, 198, 214)));
        
        public static readonly DependencyProperty TaskLabelFontSizeProperty =
            DependencyProperty.Register("TaskLabelFontSize", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(12.5));
        
        public static readonly DependencyProperty TaskLabelForegroundProperty =
            DependencyProperty.Register("TaskLabelForeground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(Brushes.White));
        
        public static readonly DependencyProperty TaskLabelFontWeightProperty =
            DependencyProperty.Register("TaskLabelFontWeight", typeof(FontWeight), typeof(GanttChartView), 
                new PropertyMetadata(FontWeights.Medium));
        
        // 画布尺寸属性
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(2800.0));
        
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(900.0));
        
        public static readonly DependencyProperty CanvasBackgroundProperty =
            DependencyProperty.Register("CanvasBackground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new LinearGradientBrush(Color.FromRgb(252, 253, 254), Color.FromRgb(248, 250, 252), 135)));
        
        // 时间粒度属性
        public static readonly DependencyProperty DefaultTimeScaleProperty =
            DependencyProperty.Register("DefaultTimeScale", typeof(string), typeof(GanttChartView), 
                new PropertyMetadata("Day", OnDefaultTimeScaleChanged));
        
        // 层次设定属性
        public static readonly DependencyProperty ShowTaskHierarchyProperty =
            DependencyProperty.Register("ShowTaskHierarchy", typeof(bool), typeof(GanttChartView), 
                new PropertyMetadata(true, OnHierarchySettingsChanged));
        
        public static readonly DependencyProperty HierarchyIndentationProperty =
            DependencyProperty.Register("HierarchyIndentation", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(30.0, OnHierarchySettingsChanged));
        
        public static readonly DependencyProperty ParentTaskBackgroundProperty =
            DependencyProperty.Register("ParentTaskBackground", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(52, 152, 219))); // SteelBlue
        
        public static readonly DependencyProperty ChildTaskBackgroundProperty =
            DependencyProperty.Register("ChildTaskBackground", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(85, 172, 238))); // SkyBlue
        
        public static readonly DependencyProperty ParentTaskHeightProperty =
            DependencyProperty.Register("ParentTaskHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(48.0));
        
        public static readonly DependencyProperty ChildTaskHeightProperty =
            DependencyProperty.Register("ChildTaskHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(36.0));
        
        private static void OnHierarchySettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GanttChartView;
            control?.RedrawTasks();
        }
        
        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GanttChartView;
            if (control != null && e.NewValue is GanttChartViewModel viewModel)
            {
                // 监听Tasks集合变化
                viewModel.Tasks.CollectionChanged += (sender, args) =>
                {
                    control.UpdateGanttChartData();
                };
                
                // 初始加载数据
                control.UpdateGanttChartData();
            }
        }
        
        private static void OnDefaultTimeScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GanttChartView;
            control?.InitializeTimeScale();
        }
        
        #endregion
        #region 属性包装器
        
        public GanttChartViewModel ViewModel
        {
            get => (GanttChartViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        
        public Brush TimeScaleBackground
        {
            get => (Brush)GetValue(TimeScaleBackgroundProperty);
            set => SetValue(TimeScaleBackgroundProperty, value);
        }
        
        public Brush TimeScaleForeground
        {
            get => (Brush)GetValue(TimeScaleForegroundProperty);
            set => SetValue(TimeScaleForegroundProperty, value);
        }
        
        public double TimeScaleFontSize
        {
            get => (double)GetValue(TimeScaleFontSizeProperty);
            set => SetValue(TimeScaleFontSizeProperty, value);
        }
        
        public Color GridLineColor
        {
            get => (Color)GetValue(GridLineColorProperty);
            set => SetValue(GridLineColorProperty, value);
        }
        
        public double GridLineThickness
        {
            get => (double)GetValue(GridLineThicknessProperty);
            set => SetValue(GridLineThicknessProperty, value);
        }
        
        public DoubleCollection GridLineDashArray
        {
            get => (DoubleCollection)GetValue(GridLineDashArrayProperty);
            set => SetValue(GridLineDashArrayProperty, value);
        }
        
        public double HorizontalGridLineSpacing
        {
            get => (double)GetValue(HorizontalGridLineSpacingProperty);
            set => SetValue(HorizontalGridLineSpacingProperty, value);
        }
        
        public double TaskBarCornerRadius
        {
            get => (double)GetValue(TaskBarCornerRadiusProperty);
            set => SetValue(TaskBarCornerRadiusProperty, value);
        }
        
        public double TaskBarBorderThickness
        {
            get => (double)GetValue(TaskBarBorderThicknessProperty);
            set => SetValue(TaskBarBorderThicknessProperty, value);
        }
        
        public Color TaskBarBorderColor
        {
            get => (Color)GetValue(TaskBarBorderColorProperty);
            set => SetValue(TaskBarBorderColorProperty, value);
        }
        
        public double TaskLabelFontSize
        {
            get => (double)GetValue(TaskLabelFontSizeProperty);
            set => SetValue(TaskLabelFontSizeProperty, value);
        }
        
        public Brush TaskLabelForeground
        {
            get => (Brush)GetValue(TaskLabelForegroundProperty);
            set => SetValue(TaskLabelForegroundProperty, value);
        }
        
        public FontWeight TaskLabelFontWeight
        {
            get => (FontWeight)GetValue(TaskLabelFontWeightProperty);
            set => SetValue(TaskLabelFontWeightProperty, value);
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
        
        public Brush CanvasBackground
        {
            get => (Brush)GetValue(CanvasBackgroundProperty);
            set => SetValue(CanvasBackgroundProperty, value);
        }
        
        public string DefaultTimeScale
        {
            get => (string)GetValue(DefaultTimeScaleProperty);
            set => SetValue(DefaultTimeScaleProperty, value);
        }
        
        // 层次设定属性包装器
        public bool ShowTaskHierarchy
        {
            get => (bool)GetValue(ShowTaskHierarchyProperty);
            set => SetValue(ShowTaskHierarchyProperty, value);
        }
        
        public double HierarchyIndentation
        {
            get => (double)GetValue(HierarchyIndentationProperty);
            set => SetValue(HierarchyIndentationProperty, value);
        }
        
        public Color ParentTaskBackground
        {
            get => (Color)GetValue(ParentTaskBackgroundProperty);
            set => SetValue(ParentTaskBackgroundProperty, value);
        }
        
        public Color ChildTaskBackground
        {
            get => (Color)GetValue(ChildTaskBackgroundProperty);
            set => SetValue(ChildTaskBackgroundProperty, value);
        }
        
        public double ParentTaskHeight
        {
            get => (double)GetValue(ParentTaskHeightProperty);
            set => SetValue(ParentTaskHeightProperty, value);
        }
        
        public double ChildTaskHeight
        {
            get => (double)GetValue(ChildTaskHeightProperty);
            set => SetValue(ChildTaskHeightProperty, value);
        }
        
        #endregion
        public GanttChartView()
        {
            InitializeComponent();
            
            // 设置数据上下文绑定
            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue is GanttChartViewModel viewModel)
                {
                    ViewModel = viewModel;
                }
            };
        }
        
        private void UpdateGanttChartData()
        {
            if (ViewModel == null || ViewModel.Tasks == null || GanttContainer == null || GanttContainer.TaskBars == null)
                return;
            
            // 将GPM的Task模型转换为GPM.Views.TaskItem模型
            var taskItems = new List<TaskItem>();
            int itemId = 1;
            
            foreach (var task in ViewModel.Tasks)
            {
                // 添加主任务
                var mainTaskItem = new TaskItem
                {
                    Id = itemId++, 
                    Name = task.Name,
                    StartDay = (int)((task.StartDate - DateTime.Now.Date).TotalDays) + 1, // 转换为相对天数
                    Duration = (task.EndDate - task.StartDate).Days,
                    Progress = task.Progress * 100, // 转换为百分比
                    ParentId = null,
                    Color = ParentTaskBackground // 直接使用Color类型
                };
                taskItems.Add(mainTaskItem);
                
                // 添加子任务
                foreach (var subtask in task.SubTasks)
                {
                    var subTaskItem = new TaskItem
                    {
                        Id = itemId++, 
                        Name = subtask.Name,
                        StartDay = (int)((subtask.StartDate - DateTime.Now.Date).TotalDays) + 1, // 转换为相对天数
                        Duration = (subtask.EndDate - subtask.StartDate).Days,
                        Progress = subtask.Progress * 100, // 转换为百分比
                        ParentId = mainTaskItem.Id,
                        Color = ChildTaskBackground // 直接使用Color类型
                    };
                    taskItems.Add(subTaskItem);
                }
            }
            
            // 将转换后的数据设置到TaskBarControl
            GanttContainer.TaskBars.Tasks = taskItems;
            GanttContainer.TaskBars.IsHierarchical = true;
            
            // 设置时间刻度类型
            string timeScaleType = "日";
            switch (ViewModel.SelectedTimeScale)
            {
                case "Week":
                    timeScaleType = "周";
                    break;
                case "Month":
                    timeScaleType = "月";
                    break;
                case "Quarter":
                    timeScaleType = "季";
                    break;
                default:
                    timeScaleType = "日";
                    break;
            }
            
            GanttContainer.TimeScaleType = timeScaleType;
            GanttContainer.TaskBars.TimeScaleType = timeScaleType;
        }
        
        // 这些方法保留为空以保持向后兼容性
        private void InitializeCanvas() { }
        private void InitializeTimeScale() { }
        private void RedrawTasks() { }
        private void DrawSampleTasks() { }
        private void DrawTaskBar(string taskName, double startX, double startY, double width, double height, Color color, bool isParentTask) { }
        private void DrawDailyTimeScale() { }
        private void DrawWeeklyTimeScale() { }
        private void DrawMonthlyTimeScale() { }
        private void DrawQuarterlyTimeScale() { }
        private void DrawGridLines() { }
        
        private void TimeScaleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 时间轴选择逻辑现在由 GanttChartContainer 处理
        }
    }
}