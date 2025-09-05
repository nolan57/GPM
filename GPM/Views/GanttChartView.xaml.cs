using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace GPM.Views
{
    public partial class GanttChartView : UserControl
    {
        #region 依赖属性
        
        // 时间轴样式属性
        public static readonly DependencyProperty TimeScaleBackgroundProperty =
            DependencyProperty.Register("TimeScaleBackground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(240, 240, 240))));
        
        public static readonly DependencyProperty TimeScaleForegroundProperty =
            DependencyProperty.Register("TimeScaleForeground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(64, 64, 64))));
        
        public static readonly DependencyProperty TimeScaleFontSizeProperty =
            DependencyProperty.Register("TimeScaleFontSize", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(12.0));
        
        // 网格线样式属性
        public static readonly DependencyProperty GridLineColorProperty =
            DependencyProperty.Register("GridLineColor", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(220, 220, 220)));
        
        public static readonly DependencyProperty GridLineThicknessProperty =
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty GridLineDashArrayProperty =
            DependencyProperty.Register("GridLineDashArray", typeof(DoubleCollection), typeof(GanttChartView), 
                new PropertyMetadata(new DoubleCollection { 4, 2 }));
        
        public static readonly DependencyProperty HorizontalGridLineSpacingProperty =
            DependencyProperty.Register("HorizontalGridLineSpacing", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(40.0));
        
        // 任务条样式属性
        public static readonly DependencyProperty TaskBarCornerRadiusProperty =
            DependencyProperty.Register("TaskBarCornerRadius", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(6.0));
        
        public static readonly DependencyProperty TaskBarBorderThicknessProperty =
            DependencyProperty.Register("TaskBarBorderThickness", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(1.5));
        
        public static readonly DependencyProperty TaskBarBorderColorProperty =
            DependencyProperty.Register("TaskBarBorderColor", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(200, 200, 200)));
        
        public static readonly DependencyProperty TaskLabelFontSizeProperty =
            DependencyProperty.Register("TaskLabelFontSize", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(12.0));
        
        public static readonly DependencyProperty TaskLabelForegroundProperty =
            DependencyProperty.Register("TaskLabelForeground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(Brushes.White));
        
        public static readonly DependencyProperty TaskLabelFontWeightProperty =
            DependencyProperty.Register("TaskLabelFontWeight", typeof(FontWeight), typeof(GanttChartView), 
                new PropertyMetadata(FontWeights.SemiBold));
        
        // 画布尺寸属性
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(2400.0));
        
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(800.0));
        
        public static readonly DependencyProperty CanvasBackgroundProperty =
            DependencyProperty.Register("CanvasBackground", typeof(Brush), typeof(GanttChartView), 
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));
        
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
                new PropertyMetadata(25.0, OnHierarchySettingsChanged));
        
        public static readonly DependencyProperty ParentTaskBackgroundProperty =
            DependencyProperty.Register("ParentTaskBackground", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(65, 105, 225))); // RoyalBlue
        
        public static readonly DependencyProperty ChildTaskBackgroundProperty =
            DependencyProperty.Register("ChildTaskBackground", typeof(Color), typeof(GanttChartView), 
                new PropertyMetadata(Color.FromRgb(135, 206, 250))); // LightSkyBlue
        
        public static readonly DependencyProperty ParentTaskHeightProperty =
            DependencyProperty.Register("ParentTaskHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(45.0));
        
        public static readonly DependencyProperty ChildTaskHeightProperty =
            DependencyProperty.Register("ChildTaskHeight", typeof(double), typeof(GanttChartView), 
                new PropertyMetadata(35.0));
        
        private static void OnHierarchySettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GanttChartView;
            control?.RedrawTasks();
        }
        
        private static void OnDefaultTimeScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GanttChartView;
            control?.InitializeTimeScale();
        }
        
        #endregion
        
        #region 属性包装器
        
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
            InitializeCanvas();
            InitializeTimeScale();
            DrawSampleTasks();
        }
        
        private void InitializeCanvas()
        {
            // 使用依赖属性设置画布尺寸和背景
            GanttCanvas.Width = CanvasWidth;
            GanttCanvas.Height = CanvasHeight;
            GanttCanvas.Background = CanvasBackground;
            
            TimeScaleCanvas.Width = CanvasWidth;
            TimeScaleCanvas.Background = TimeScaleBackground;
            
            TaskCanvas.Width = CanvasWidth;
            TaskCanvas.Height = CanvasHeight - 30; // 减去时间轴高度
        }

        private void InitializeTimeScale()
        {
            TimeScaleCanvas.Children.Clear();
            
            // 根据默认时间粒度绘制时间轴
            switch (DefaultTimeScale)
            {
                case "Day":
                    DrawDailyTimeScale();
                    break;
                case "Week":
                    DrawWeeklyTimeScale();
                    break;
                case "Month":
                    DrawMonthlyTimeScale();
                    break;
                case "Quarter":
                    DrawQuarterlyTimeScale();
                    break;
            }
            
            // 绘制网格线
            DrawGridLines();
        }

        private void RedrawTasks()
        {
            // 清除现有任务
            TaskCanvas.Children.Clear();
            
            // 重新绘制示例任务
            DrawSampleTasks();
        }
        
        private void DrawSampleTasks()
        {
            if (ShowTaskHierarchy)
            {
                // 层次化显示：父任务和子任务（垂直方向上下覆盖）
                DrawTaskBar("父任务1", 50, 50, 300, ParentTaskHeight, ParentTaskBackground, true);
                DrawTaskBar("子任务1.1", 50 + HierarchyIndentation, 50 + ParentTaskHeight + 5, 200, ChildTaskHeight, ChildTaskBackground, false);
                DrawTaskBar("子任务1.2", 260 + HierarchyIndentation, 50 + ParentTaskHeight + 5, 90, ChildTaskHeight, ChildTaskBackground, false);
                
                // 第二个父任务组，垂直方向与第一个组重叠显示
                DrawTaskBar("父任务2", 150, 80, 250, ParentTaskHeight, ParentTaskBackground, true);
                DrawTaskBar("子任务2.1", 150 + HierarchyIndentation, 80 + ParentTaskHeight + 5, 180, ChildTaskHeight, ChildTaskBackground, false);
                
                // 第三个父任务组，展示更复杂的垂直覆盖
                DrawTaskBar("父任务3", 400, 40, 350, ParentTaskHeight, ParentTaskBackground, true);
                DrawTaskBar("子任务3.1", 400 + HierarchyIndentation, 40 + ParentTaskHeight + 5, 120, ChildTaskHeight, ChildTaskBackground, false);
                DrawTaskBar("子任务3.2", 530 + HierarchyIndentation, 40 + ParentTaskHeight + 5, 100, ChildTaskHeight, ChildTaskBackground, false);
                DrawTaskBar("子任务3.3", 640 + HierarchyIndentation, 40 + ParentTaskHeight + 5, 110, ChildTaskHeight, ChildTaskBackground, false);
            }
            else
            {
                // 普通显示：所有任务在同一层级
                DrawTaskBar("任务1", 50, 50, 200, 35, Color.FromRgb(70, 130, 180), false); // SteelBlue
                DrawTaskBar("任务2", 150, 100, 300, 35, Color.FromRgb(60, 179, 113), false); // MediumSeaGreen
                DrawTaskBar("任务3", 300, 150, 180, 35, Color.FromRgb(255, 165, 0), false);  // Orange
            }
        }

        private void DrawTaskBar(string taskName, double startX, double startY, double width, double height, Color color, bool isParentTask)
        {
            // 绘制任务矩形
            var taskRect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(TaskBarBorderColor),
                StrokeThickness = TaskBarBorderThickness,
                RadiusX = TaskBarCornerRadius,
                RadiusY = TaskBarCornerRadius
            };
            
            Canvas.SetLeft(taskRect, startX);
            Canvas.SetTop(taskRect, startY);
            
            // 添加任务名称标签
            var taskLabel = new TextBlock
            {
                Text = taskName,
                FontSize = isParentTask ? TaskLabelFontSize + 2 : TaskLabelFontSize,
                Foreground = TaskLabelForeground,
                FontWeight = isParentTask ? FontWeights.Bold : TaskLabelFontWeight
            };
            
            Canvas.SetLeft(taskLabel, startX + 5);
            Canvas.SetTop(taskLabel, startY + (height - taskLabel.FontSize) / 2);
            
            TaskCanvas.Children.Add(taskRect);
            TaskCanvas.Children.Add(taskLabel);
        }

        private void TimeScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string timeScale)
            {
                // 清除现有时间轴
                TimeScaleCanvas.Children.Clear();
                
                // 根据选择的时间粒度重新绘制时间轴
                switch (timeScale)
                {
                    case "Day":
                        DrawDailyTimeScale();
                        break;
                    case "Week":
                        DrawWeeklyTimeScale();
                        break;
                    case "Month":
                        DrawMonthlyTimeScale();
                        break;
                    case "Quarter":
                        DrawQuarterlyTimeScale();
                        break;
                }
                
                // 重新绘制网格线
                DrawGridLines();
            }
        }

        private void DrawDailyTimeScale()
        {
            for (int i = 0; i < 30; i++)
            {
                var dayLine = new Line
                {
                    X1 = i * 60 + 50,
                    Y1 = 0,
                    X2 = i * 60 + 50,
                    Y2 = 30,
                    Stroke = TimeScaleForeground,
                    StrokeThickness = 1
                };
                
                var dayLabel = new TextBlock
                {
                    Text = (i + 1).ToString(),
                    FontSize = TimeScaleFontSize,
                    Foreground = TimeScaleForeground,
                    Margin = new Thickness(i * 60 + 45, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(dayLine);
                TimeScaleCanvas.Children.Add(dayLabel);
            }
        }

        private void DrawWeeklyTimeScale()
        {
            for (int i = 0; i < 4; i++)
            {
                var weekLine = new Line
                {
                    X1 = i * 150 + 50,
                    Y1 = 0,
                    X2 = i * 150 + 50,
                    Y2 = 30,
                    Stroke = TimeScaleForeground,
                    StrokeThickness = 2
                };
                
                var weekLabel = new TextBlock
                {
                    Text = $"第{i + 1}周",
                    FontSize = TimeScaleFontSize,
                    Foreground = TimeScaleForeground,
                    Margin = new Thickness(i * 150 + 30, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(weekLine);
                TimeScaleCanvas.Children.Add(weekLabel);
            }
        }

        private void DrawMonthlyTimeScale()
        {
            for (int i = 0; i < 3; i++)
            {
                var monthLine = new Line
                {
                    X1 = i * 200 + 50,
                    Y1 = 0,
                    X2 = i * 200 + 50,
                    Y2 = 30,
                    Stroke = TimeScaleForeground,
                    StrokeThickness = 2
                };
                
                var monthLabel = new TextBlock
                {
                    Text = $"{i + 1}月",
                    FontSize = TimeScaleFontSize,
                    Foreground = TimeScaleForeground,
                    Margin = new Thickness(i * 200 + 80, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(monthLine);
                TimeScaleCanvas.Children.Add(monthLabel);
            }
        }

        private void DrawQuarterlyTimeScale()
        {
            for (int i = 0; i < 4; i++)
            {
                var quarterLine = new Line
                {
                    X1 = i * 150 + 50,
                    Y1 = 0,
                    X2 = i * 150 + 50,
                    Y2 = 30,
                    Stroke = TimeScaleForeground,
                    StrokeThickness = 3
                };
                
                var quarterLabel = new TextBlock
                {
                    Text = $"Q{i + 1}",
                    FontSize = TimeScaleFontSize,
                    Foreground = TimeScaleForeground,
                    Margin = new Thickness(i * 150 + 65, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(quarterLine);
                TimeScaleCanvas.Children.Add(quarterLabel);
            }
        }
        
        /// <summary>
        /// 绘制网格线组件
        /// </summary>
        private void DrawGridLines()
        {
            // 清除现有网格线
            var gridLinesToRemove = new List<UIElement>();
            foreach (UIElement element in TaskCanvas.Children)
            {
                if (element is Line line && line.Tag?.ToString() == "GridLine")
                {
                    gridLinesToRemove.Add(element);
                }
            }
            
            foreach (var element in gridLinesToRemove)
            {
                TaskCanvas.Children.Remove(element);
            }
            
            // 绘制垂直网格线（与时间轴刻度对齐）
            for (int i = 0; i < 30; i++)
            {
                var gridLine = new Line
                {
                    X1 = i * 60 + 50,
                    Y1 = 0,
                    X2 = i * 60 + 50,
                    Y2 = CanvasHeight - 30,
                    Stroke = new SolidColorBrush(GridLineColor),
                    StrokeThickness = GridLineThickness,
                    StrokeDashArray = GridLineDashArray,
                    Tag = "GridLine"
                };
                
                TaskCanvas.Children.Insert(0, gridLine); // 插入到最底层
            }
            
            // 绘制水平网格线
            int horizontalLineCount = (int)((CanvasHeight - 30) / HorizontalGridLineSpacing);
            for (int i = 1; i <= horizontalLineCount; i++)
            {
                var horizontalGridLine = new Line
                {
                    X1 = 0,
                    Y1 = i * HorizontalGridLineSpacing,
                    X2 = CanvasWidth,
                    Y2 = i * HorizontalGridLineSpacing,
                    Stroke = new SolidColorBrush(GridLineColor),
                    StrokeThickness = GridLineThickness,
                    StrokeDashArray = GridLineDashArray,
                    Tag = "GridLine"
                };
                
                TaskCanvas.Children.Insert(0, horizontalGridLine); // 插入到最底层
            }
        }
    }
}