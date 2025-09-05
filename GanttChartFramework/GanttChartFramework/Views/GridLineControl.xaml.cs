using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GanttChartFramework.Views
{
    public partial class GridLineControl : UserControl
    {
        #region 依赖属性
        
        public static readonly DependencyProperty GridLineColorProperty = 
            DependencyProperty.Register("GridLineColor", typeof(Color), typeof(GridLineControl),
                new PropertyMetadata(Color.FromRgb(200, 200, 200), OnGridLinePropertyChanged));
        
        public static readonly DependencyProperty GridLineThicknessProperty = 
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(GridLineControl),
                new PropertyMetadata(1.0, OnGridLinePropertyChanged));
        
        public static readonly DependencyProperty GridLineDashArrayProperty = 
            DependencyProperty.Register("GridLineDashArray", typeof(DoubleCollection), typeof(GridLineControl),
                new PropertyMetadata(new DoubleCollection() { 1, 1 }, OnGridLinePropertyChanged));
        
        public static readonly DependencyProperty VerticalGridLinesProperty = 
            DependencyProperty.Register("VerticalGridLines", typeof(bool), typeof(GridLineControl),
                new PropertyMetadata(true, OnGridLinePropertyChanged));
        
        public static readonly DependencyProperty HorizontalGridLinesProperty = 
            DependencyProperty.Register("HorizontalGridLines", typeof(bool), typeof(GridLineControl),
                new PropertyMetadata(true, OnGridLinePropertyChanged));
        
        public static readonly DependencyProperty CanvasWidthProperty = 
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(GridLineControl),
                new PropertyMetadata(2400.0, OnCanvasSizeChanged));
        
        public static readonly DependencyProperty CanvasHeightProperty = 
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(GridLineControl),
                new PropertyMetadata(1800.0, OnCanvasSizeChanged));
        
        public static readonly DependencyProperty TimeScaleTypeProperty = 
            DependencyProperty.Register("TimeScaleType", typeof(string), typeof(GridLineControl),
                new PropertyMetadata("Day", OnTimeScaleTypeChanged));
        
        public static readonly DependencyProperty TasksCountProperty = 
            DependencyProperty.Register("TasksCount", typeof(int), typeof(GridLineControl),
                new PropertyMetadata(10, OnTasksCountChanged));
        
        public static readonly DependencyProperty TaskHeightProperty = 
            DependencyProperty.Register("TaskHeight", typeof(double), typeof(GridLineControl),
                new PropertyMetadata(40.0, OnTaskHeightChanged));
        
        #endregion
        
        #region 属性包装器
        
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
        
        public bool VerticalGridLines
        {
            get => (bool)GetValue(VerticalGridLinesProperty);
            set => SetValue(VerticalGridLinesProperty, value);
        }
        
        public bool HorizontalGridLines
        {
            get => (bool)GetValue(HorizontalGridLinesProperty);
            set => SetValue(HorizontalGridLinesProperty, value);
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
        
        public string TimeScaleType
        {
            get => (string)GetValue(TimeScaleTypeProperty);
            set => SetValue(TimeScaleTypeProperty, value);
        }
        
        public int TasksCount
        {
            get => (int)GetValue(TasksCountProperty);
            set => SetValue(TasksCountProperty, value);
        }
        
        public double TaskHeight
        {
            get => (double)GetValue(TaskHeightProperty);
            set => SetValue(TaskHeightProperty, value);
        }
        
        #endregion
        
        public GridLineControl()
        {
            InitializeComponent();
            InitializeGridLines();
        }
        
        private static void OnGridLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLineControl;
            control?.DrawGridLines();
        }
        
        private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLineControl;
            control?.UpdateCanvasSize();
            control?.DrawGridLines();
        }
        
        private static void OnTimeScaleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLineControl;
            control?.DrawGridLines();
        }
        
        private static void OnTasksCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLineControl;
            control?.DrawGridLines();
        }
        
        private static void OnTaskHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLineControl;
            control?.DrawGridLines();
        }
        
        private void UpdateCanvasSize()
        {
            GridLinesCanvas.Width = CanvasWidth;
            GridLinesCanvas.Height = CanvasHeight;
        }
        
        public void InitializeGridLines()
        {
            UpdateCanvasSize();
            DrawGridLines();
        }
        
        public void DrawGridLines()
        {
            // 清除现有网格线
            GridLinesCanvas.Children.Clear();
            
            // 创建笔刷
            var gridLineBrush = new SolidColorBrush(GridLineColor);
            
            // 绘制垂直线
            if (VerticalGridLines)
            {
                DrawVerticalGridLines(gridLineBrush);
            }
            
            // 绘制水平线
            if (HorizontalGridLines)
            {
                DrawHorizontalGridLines(gridLineBrush);
            }
        }
        
        private void DrawVerticalGridLines(SolidColorBrush gridLineBrush)
        {
            // 根据时间粒度确定垂直线间隔
            double interval = 60; // 默认日粒度
            int count = 30; // 默认30天
            
            switch (TimeScaleType)
            {
                case "Week":
                    interval = 150;
                    count = 4;
                    break;
                case "Month":
                    interval = 300;
                    count = 4;
                    break;
                case "Quarter":
                    interval = 600;
                    count = 4;
                    break;
            }
            
            for (int i = 0; i < count; i++)
            {
                double x = i * interval + 50;
                var verticalLine = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = CanvasHeight,
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    StrokeDashArray = GridLineDashArray
                };
                
                GridLinesCanvas.Children.Add(verticalLine);
            }
        }
        
        private void DrawHorizontalGridLines(SolidColorBrush gridLineBrush)
        {
            // 根据任务数和任务高度绘制水平线
            for (int i = 0; i <= TasksCount; i++)
            {
                double y = i * TaskHeight + 10;
                var horizontalLine = new Line
                {
                    X1 = 50,
                    Y1 = y,
                    X2 = CanvasWidth,
                    Y2 = y,
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    StrokeDashArray = GridLineDashArray
                };
                
                GridLinesCanvas.Children.Add(horizontalLine);
            }
        }
    }
}