using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GanttChartFramework.Models;

namespace GanttChartFramework.Views
{
    public partial class GanttChartContainer : UserControl
    {
        #region 依赖属性
        
        // 时间粒度控制
        public static readonly DependencyProperty TimeScaleTypeProperty = 
            DependencyProperty.Register("TimeScaleType", typeof(string), typeof(GanttChartContainer),
                new PropertyMetadata("Day", OnTimeScaleTypeChanged));
        
        // 时间轴宽度
        public static readonly DependencyProperty TimeScaleWidthProperty = 
            DependencyProperty.Register("TimeScaleWidth", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(2400.0));
        
        // 画布宽度
        public static readonly DependencyProperty CanvasWidthProperty = 
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(2400.0, OnCanvasWidthChanged));
        
        // 画布高度
        public static readonly DependencyProperty CanvasHeightProperty = 
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(1800.0, OnCanvasHeightChanged));
        
        // 任务高度
        public static readonly DependencyProperty TaskHeightProperty = 
            DependencyProperty.Register("TaskHeight", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(40.0, OnTaskHeightChanged));
        
        // 任务数量
        public static readonly DependencyProperty TasksCountProperty = 
            DependencyProperty.Register("TasksCount", typeof(int), typeof(GanttChartContainer),
                new PropertyMetadata(10, OnTasksCountChanged));
        
        // 是否显示任务标签
        public static readonly DependencyProperty ShowTaskLabelsProperty = 
            DependencyProperty.Register("ShowTaskLabels", typeof(bool), typeof(GanttChartContainer),
                new PropertyMetadata(true, OnShowTaskLabelsChanged));
        
        // 是否层级显示
        public static readonly DependencyProperty IsHierarchicalProperty = 
            DependencyProperty.Register("IsHierarchical", typeof(bool), typeof(GanttChartContainer),
                new PropertyMetadata(true, OnIsHierarchicalChanged));
        
        // 网格线颜色
        public static readonly DependencyProperty GridLineColorProperty = 
            DependencyProperty.Register("GridLineColor", typeof(Color), typeof(GanttChartContainer),
                new PropertyMetadata(Color.FromRgb(200, 200, 200), OnGridLineColorChanged));
        
        // 网格线粗细
        public static readonly DependencyProperty GridLineThicknessProperty = 
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(1.0, OnGridLineThicknessChanged));
        
        // 网格线样式
        public static readonly DependencyProperty GridLineDashArrayProperty = 
            DependencyProperty.Register("GridLineDashArray", typeof(DoubleCollection), typeof(GanttChartContainer),
                new PropertyMetadata(new DoubleCollection() { 1, 1 }, OnGridLineDashArrayChanged));
        
        // 是否显示垂直线
        public static readonly DependencyProperty ShowVerticalGridLinesProperty = 
            DependencyProperty.Register("ShowVerticalGridLines", typeof(bool), typeof(GanttChartContainer),
                new PropertyMetadata(true, OnShowVerticalGridLinesChanged));
        
        // 是否显示水平线
        public static readonly DependencyProperty ShowHorizontalGridLinesProperty = 
            DependencyProperty.Register("ShowHorizontalGridLinesProperty", typeof(bool), typeof(GanttChartContainer),
                new PropertyMetadata(true, OnShowHorizontalGridLinesChanged));
        
        // 画布背景色
        public static readonly DependencyProperty CanvasBackgroundProperty = 
            DependencyProperty.Register("CanvasBackground", typeof(Brush), typeof(GanttChartContainer),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));
        
        // 默认时间粒度
        public static readonly DependencyProperty DefaultTimeScaleProperty = 
            DependencyProperty.Register("DefaultTimeScale", typeof(string), typeof(GanttChartContainer),
                new PropertyMetadata("Day"));
        
        // 是否显示任务层级
        public static readonly DependencyProperty ShowTaskHierarchyProperty = 
            DependencyProperty.Register("ShowTaskHierarchy", typeof(bool), typeof(GanttChartContainer),
                new PropertyMetadata(true));
        
        // 层级缩进量
        public static readonly DependencyProperty HierarchyIndentationProperty = 
            DependencyProperty.Register("HierarchyIndentation", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(20.0));
        
        // 父任务背景色
        public static readonly DependencyProperty ParentTaskBackgroundProperty = 
            DependencyProperty.Register("ParentTaskBackground", typeof(Color), typeof(GanttChartContainer),
                new PropertyMetadata(Color.FromRgb(52, 152, 219)));
        
        // 子任务背景色
        public static readonly DependencyProperty ChildTaskBackgroundProperty = 
            DependencyProperty.Register("ChildTaskBackground", typeof(Color), typeof(GanttChartContainer),
                new PropertyMetadata(Color.FromRgb(85, 172, 238)));
        
        // 父任务高度
        public static readonly DependencyProperty ParentTaskHeightProperty = 
            DependencyProperty.Register("ParentTaskHeight", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(60.0));
        
        // 子任务高度
        public static readonly DependencyProperty ChildTaskHeightProperty = 
            DependencyProperty.Register("ChildTaskHeight", typeof(double), typeof(GanttChartContainer),
                new PropertyMetadata(40.0));
        
        // 任务列表
        public static readonly DependencyProperty TasksProperty = 
            DependencyProperty.Register("Tasks", typeof(List<TaskItem>), typeof(GanttChartContainer),
                new PropertyMetadata(new List<TaskItem>()));
        
        #endregion
        
        #region 属性包装器
        
        public string TimeScaleType
        {
            get => (string)GetValue(TimeScaleTypeProperty);
            set => SetValue(TimeScaleTypeProperty, value);
        }
        
        public double TimeScaleWidth
        {
            get => (double)GetValue(TimeScaleWidthProperty);
            set => SetValue(TimeScaleWidthProperty, value);
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
        
        public double TaskHeight
        {
            get => (double)GetValue(TaskHeightProperty);
            set => SetValue(TaskHeightProperty, value);
        }
        
        public int TasksCount
        {
            get => (int)GetValue(TasksCountProperty);
            set => SetValue(TasksCountProperty, value);
        }
        
        public bool ShowTaskLabels
        {
            get => (bool)GetValue(ShowTaskLabelsProperty);
            set => SetValue(ShowTaskLabelsProperty, value);
        }
        
        public bool IsHierarchical
        {
            get => (bool)GetValue(IsHierarchicalProperty);
            set => SetValue(IsHierarchicalProperty, value);
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
        
        public bool ShowVerticalGridLines
        {
            get => (bool)GetValue(ShowVerticalGridLinesProperty);
            set => SetValue(ShowVerticalGridLinesProperty, value);
        }
        
        public bool ShowHorizontalGridLines
        {
            get => (bool)GetValue(ShowHorizontalGridLinesProperty);
            set => SetValue(ShowHorizontalGridLinesProperty, value);
        }
        
        public Brush CanvasBackground
        {
            get => (Brush)GetValue(CanvasBackgroundProperty);
            set => SetValue(CanvasBackgroundProperty, value);
        }
        
        public List<TaskItem> Tasks
        {
            get => (List<TaskItem>)GetValue(TasksProperty);
            set => SetValue(TasksProperty, value);
        }
        
        public string DefaultTimeScale
        {
            get => (string)GetValue(DefaultTimeScaleProperty);
            set => SetValue(DefaultTimeScaleProperty, value);
        }
        
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
        
        public GanttChartContainer()
        {
            InitializeComponent();
            InitializeGanttChart();
        }
        
        private void InitializeGanttChart()
        {
            // 设置初始状态
            UpdateButtonStates();
        }
        
        #region 事件处理程序
        
        private void TimeScaleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                TimeScaleType = button.Content.ToString();
                UpdateButtonStates();
            }
        }
        
        #endregion
        
        #region 属性变更处理程序
        
        private static void OnTimeScaleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = d as GanttChartContainer;
            container?.UpdateButtonStates();
        }
        
        private static void OnCanvasHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 画布高度变化时，通知相关组件
        }
        
        private static void OnTaskHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 任务高度变化时，通知相关组件
        }
        
        private static void OnTasksCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 任务数量变化时，通知相关组件
        }
        
        private static void OnShowTaskLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 显示任务标签状态变化时，通知相关组件
        }
        
        private static void OnIsHierarchicalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 层级显示状态变化时，通知相关组件
        }
        
        private static void OnGridLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 网格线颜色变化时，通知相关组件
        }
        
        private static void OnGridLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 网格线粗细变化时，通知相关组件
        }
        
        private static void OnGridLineDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 网格线样式变化时，通知相关组件
        }
        
        private static void OnShowVerticalGridLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 垂直线显示状态变化时，通知相关组件
        }
        
        private static void OnShowHorizontalGridLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 水平线显示状态变化时，通知相关组件
        }
        
        private static void OnCanvasWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 实现画布宽度变化时的逻辑
        }
        
        #endregion
        
        #region 辅助方法
        
        private void UpdateButtonStates()
        {
            // 更新时间粒度按钮的状态
            if (TimeScaleType == "日")
            {
                BtnDay.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                BtnDay.Foreground = Brushes.White;
                BtnWeek.Background = Brushes.Transparent;
                BtnWeek.Foreground = Brushes.Black;
                BtnMonth.Background = Brushes.Transparent;
                BtnMonth.Foreground = Brushes.Black;
                BtnQuarter.Background = Brushes.Transparent;
                BtnQuarter.Foreground = Brushes.Black;
            }
            else if (TimeScaleType == "周")
            {
                BtnDay.Background = Brushes.Transparent;
                BtnDay.Foreground = Brushes.Black;
                BtnWeek.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                BtnWeek.Foreground = Brushes.White;
                BtnMonth.Background = Brushes.Transparent;
                BtnMonth.Foreground = Brushes.Black;
                BtnQuarter.Background = Brushes.Transparent;
                BtnQuarter.Foreground = Brushes.Black;
            }
            else if (TimeScaleType == "月")
            {
                BtnDay.Background = Brushes.Transparent;
                BtnDay.Foreground = Brushes.Black;
                BtnWeek.Background = Brushes.Transparent;
                BtnWeek.Foreground = Brushes.Black;
                BtnMonth.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                BtnMonth.Foreground = Brushes.White;
                BtnQuarter.Background = Brushes.Transparent;
                BtnQuarter.Foreground = Brushes.Black;
            }
            else if (TimeScaleType == "季")
            {
                BtnDay.Background = Brushes.Transparent;
                BtnDay.Foreground = Brushes.Black;
                BtnWeek.Background = Brushes.Transparent;
                BtnWeek.Foreground = Brushes.Black;
                BtnMonth.Background = Brushes.Transparent;
                BtnMonth.Foreground = Brushes.Black;
                BtnQuarter.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                BtnQuarter.Foreground = Brushes.White;
            }
        }
        
        public void RefreshGanttChart()
        {
            // 刷新整个甘特图
            TimeScale.InitializeTimeScale();
            GridLines.InitializeGridLines();
            TaskBars.InitializeTasks();
        }
        
        #endregion
    }
}