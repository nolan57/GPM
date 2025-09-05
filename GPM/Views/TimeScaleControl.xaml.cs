using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GPM.Views
{
    public partial class TimeScaleControl : UserControl
    {
        #region 依赖属性
        
        public static readonly DependencyProperty BackgroundProperty = 
            DependencyProperty.Register("Background", typeof(Brush), typeof(TimeScaleControl),
                new PropertyMetadata(new LinearGradientBrush(Color.FromRgb(245, 247, 250), Color.FromRgb(235, 238, 242), 90)));
        
        public static readonly DependencyProperty ForegroundProperty = 
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(TimeScaleControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(45, 62, 80))));
        
        public static readonly DependencyProperty FontSizeProperty = 
            DependencyProperty.Register("FontSize", typeof(double), typeof(TimeScaleControl),
                new PropertyMetadata(13.0));
        
        public static readonly DependencyProperty TimeScaleTypeProperty = 
            DependencyProperty.Register("TimeScaleType", typeof(string), typeof(TimeScaleControl),
                new PropertyMetadata("Day", OnTimeScaleTypeChanged));
        
        public static readonly DependencyProperty WidthProperty = 
            DependencyProperty.Register("Width", typeof(double), typeof(TimeScaleControl),
                new PropertyMetadata(2400.0, OnWidthChanged));
        
        #endregion
        
        #region 属性包装器
        
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        
        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        
        public string TimeScaleType
        {
            get => (string)GetValue(TimeScaleTypeProperty);
            set => SetValue(TimeScaleTypeProperty, value);
        }
        
        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }
        
        #endregion
        
        public TimeScaleControl()
        {
            InitializeComponent();
            InitializeTimeScale();
        }
        
        private static void OnTimeScaleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TimeScaleControl;
            control?.InitializeTimeScale();
        }
        
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TimeScaleControl;
            if (control != null)
            {
                control.TimeScaleCanvas.Width = control.Width;
                control.InitializeTimeScale();
            }
        }
        
        public void InitializeTimeScale()
        {
            TimeScaleCanvas.Children.Clear();
            TimeScaleCanvas.Width = Width;
            
            // 根据时间粒度绘制时间轴
            switch (TimeScaleType)
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
                    Stroke = Foreground,
                    StrokeThickness = 1
                };
                
                var dayLabel = new TextBlock
                {
                    Text = (i + 1).ToString(),
                    FontSize = FontSize,
                    Foreground = Foreground,
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
                    Stroke = Foreground,
                    StrokeThickness = 2
                };
                
                var weekLabel = new TextBlock
                {
                    Text = $"第{i + 1}周",
                    FontSize = FontSize,
                    Foreground = Foreground,
                    Margin = new Thickness(i * 150 + 45, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(weekLine);
                TimeScaleCanvas.Children.Add(weekLabel);
            }
        }
        
        private void DrawMonthlyTimeScale()
        {
            string[] months = { "一月", "二月", "三月", "四月" };
            for (int i = 0; i < 4; i++)
            {
                var monthLine = new Line
                {
                    X1 = i * 300 + 50,
                    Y1 = 0,
                    X2 = i * 300 + 50,
                    Y2 = 30,
                    Stroke = Foreground,
                    StrokeThickness = 2
                };
                
                var monthLabel = new TextBlock
                {
                    Text = months[i],
                    FontSize = FontSize,
                    Foreground = Foreground,
                    Margin = new Thickness(i * 300 + 45, 15, 0, 0)
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
                    X1 = i * 600 + 50,
                    Y1 = 0,
                    X2 = i * 600 + 50,
                    Y2 = 30,
                    Stroke = Foreground,
                    StrokeThickness = 3
                };
                
                var quarterLabel = new TextBlock
                {
                    Text = $"第{i + 1}季度",
                    FontSize = FontSize,
                    Foreground = Foreground,
                    Margin = new Thickness(i * 600 + 45, 15, 0, 0)
                };
                
                TimeScaleCanvas.Children.Add(quarterLine);
                TimeScaleCanvas.Children.Add(quarterLabel);
            }
        }
    }
}