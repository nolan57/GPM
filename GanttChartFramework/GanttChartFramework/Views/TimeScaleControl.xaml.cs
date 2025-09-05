using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GanttChartFramework.Views
{
    public partial class TimeScaleControl : UserControl
    {
        #region 依赖属性
        
        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(TimeScaleControl),
                new PropertyMetadata(new LinearGradientBrush(Color.FromRgb(245, 247, 250), Color.FromRgb(235, 238, 242), 90)));
        
        public static new readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(TimeScaleControl),
                new PropertyMetadata(Brushes.Black));
        
        public static new readonly DependencyProperty FontSizeProperty = 
            DependencyProperty.Register("FontSize", typeof(double), typeof(TimeScaleControl),
                new PropertyMetadata(13.0));
        
        // 时间粒度依赖属性
        public static readonly DependencyProperty TimeScaleTypeProperty =
            DependencyProperty.Register("TimeScaleType", typeof(string), typeof(TimeScaleControl), 
                new PropertyMetadata("Day", OnTimeScaleTypeChanged));
        
        public static new readonly DependencyProperty WidthProperty = 
            DependencyProperty.Register("Width", typeof(double), typeof(TimeScaleControl),
                new PropertyMetadata(2400.0, OnWidthChanged));
        
        #endregion
        
        #region 属性包装器
        
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        
        public new Brush Foreground
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
                case "MultiLevel":
                    DrawMultiLevelTimeScale();
                    break;
            }
        }
        
        private void DrawDailyTimeScale()
        {
            for (int i = 0; i < 30; i++)
            {
                var tickControl = new TimeScaleTickControl
                {
                    TickStroke = Foreground,
                    TickThickness = 1,
                    LabelText = (i + 1).ToString(),
                    LabelFontSize = FontSize,
                    LabelForeground = Foreground,
                    Width = 60,
                    Height = 30
                };
                
                Canvas.SetLeft(tickControl, i * 60 + 20);
                Canvas.SetTop(tickControl, 0);
                
                TimeScaleCanvas.Children.Add(tickControl);
            }
        }
        
        private void DrawWeeklyTimeScale()
        {
            for (int i = 0; i < 4; i++)
            {
                var tickControl = new TimeScaleTickControl
                {
                    TickStroke = Foreground,
                    TickThickness = 2,
                    LabelText = $"第{i + 1}周",
                    LabelFontSize = FontSize,
                    LabelForeground = Foreground,
                    Width = 150,
                    Height = 30
                };
                
                Canvas.SetLeft(tickControl, i * 150 + 20);
                Canvas.SetTop(tickControl, 0);
                
                TimeScaleCanvas.Children.Add(tickControl);
            }
        }
        
        private void DrawMonthlyTimeScale()
        {
            string[] months = { "一月", "二月", "三月", "四月" };
            for (int i = 0; i < 4; i++)
            {
                var tickControl = new TimeScaleTickControl
                {
                    TickStroke = Foreground,
                    TickThickness = 2,
                    LabelText = months[i],
                    LabelFontSize = FontSize,
                    LabelForeground = Foreground,
                    Width = 300,
                    Height = 30
                };
                
                Canvas.SetLeft(tickControl, i * 300 + 20);
                Canvas.SetTop(tickControl, 0);
                
                TimeScaleCanvas.Children.Add(tickControl);
            }
        }
        
        private void DrawQuarterlyTimeScale()
        {
            for (int i = 0; i < 4; i++)
            {
                var tickControl = new TimeScaleTickControl
                {
                    TickStroke = Foreground,
                    TickThickness = 3,
                    LabelText = $"第{i + 1}季度",
                    LabelFontSize = FontSize,
                    LabelForeground = Foreground,
                    Width = 600,
                    Height = 30
                };
                
                Canvas.SetLeft(tickControl, i * 600 + 20);
                Canvas.SetTop(tickControl, 0);
                
                TimeScaleCanvas.Children.Add(tickControl);
            }
        }
        
        // 绘制多级时间刻度
        private void DrawMultiLevelTimeScale()
        {
            // 示例：显示30天的多级时间刻度
            for (int i = 0; i < 30; i++)
            {
                var date = DateTime.Now.AddDays(i);
                
                var multiTick = new MultiLevelTimeScaleTick
                {
                    YearText = date.Year.ToString(),
                    QuarterText = $"Q{(date.Month - 1) / 3 + 1}",
                    MonthText = date.ToString("MMM"),
                    WeekText = $"第{GetWeekNumber(date)}周",
                    DayText = date.Day.ToString(),
                    TimeText = date.ToString("HH:mm"),
                    
                    ShowYear = true,
                    ShowQuarter = i % 7 == 0, // 每周显示季度
                    ShowMonth = true,
                    ShowWeek = i % 7 == 0,    // 每周显示周
                    ShowDay = true,
                    ShowTime = false,
                    
                    YearFontSize = 12,
                    YearForeground = Brushes.DarkBlue,
                    QuarterFontSize = 11,
                    QuarterForeground = Brushes.DarkGreen,
                    MonthFontSize = 11,
                    MonthForeground = Brushes.DarkRed,
                    WeekFontSize = 10,
                    WeekForeground = Brushes.Purple,
                    DayFontSize = 12,
                    DayForeground = Foreground,
                    
                    TickStroke = Foreground,
                    TickThickness = 1,
                    Width = 80,
                    Height = 60
                };
                
                Canvas.SetLeft(multiTick, i * 80 + 20);
                Canvas.SetTop(multiTick, 0);
                
                TimeScaleCanvas.Children.Add(multiTick);
            }
        }
        
        private int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
        
        // 方法：初始化多级时间刻度（使用配置）
        public void InitializeMultiLevelTimeScale(Models.TimeScaleConfig config)
        {
            TimeScaleCanvas.Children.Clear();
            TimeScaleCanvas.Width = Width;
            
            config.GenerateTimeUnits();
            
            double tickWidth = 80; // 默认刻度宽度
            double spacing = 20;   // 间距
            
            for (int i = 0; i < config.TimeUnits.Count; i++)
            {
                var timeUnit = config.TimeUnits[i];
                
                var multiTick = new MultiLevelTimeScaleTick
                {
                    YearText = timeUnit.Year,
                    QuarterText = timeUnit.Quarter,
                    MonthText = timeUnit.Month,
                    WeekText = timeUnit.Week,
                    DayText = timeUnit.Day,
                    TimeText = timeUnit.Time,
                    
                    ShowYear = config.DisplayOptions.ShowYearLevel,
                    ShowQuarter = config.DisplayOptions.ShowQuarterLevel,
                    ShowMonth = config.DisplayOptions.ShowMonthLevel,
                    ShowWeek = config.DisplayOptions.ShowWeekLevel,
                    ShowDay = config.DisplayOptions.ShowDayLevel,
                    ShowTime = config.DisplayOptions.ShowTimeLevel,
                    
                    LayoutOrientation = config.DisplayOptions.TimeUnitsOrientation,
                    MaxRows = config.DisplayOptions.MaxDisplayRows,
                    LevelMargin = config.DisplayOptions.LevelMargin,
                    
                    YearFontSize = config.DisplayOptions.YearFontSize,
                    YearForeground = config.DisplayOptions.YearForeground,
                    YearHorizontalAlignment = config.DisplayOptions.YearHorizontalAlignment,
                    YearMargin = config.DisplayOptions.YearMargin,
                    QuarterFontSize = config.DisplayOptions.QuarterFontSize,
                    QuarterForeground = config.DisplayOptions.QuarterForeground,
                    QuarterHorizontalAlignment = config.DisplayOptions.QuarterHorizontalAlignment,
                    QuarterMargin = config.DisplayOptions.QuarterMargin,
                    MonthFontSize = config.DisplayOptions.MonthFontSize,
                    MonthForeground = config.DisplayOptions.MonthForeground,
                    MonthHorizontalAlignment = config.DisplayOptions.MonthHorizontalAlignment,
                    MonthMargin = config.DisplayOptions.MonthMargin,
                    WeekFontSize = config.DisplayOptions.WeekFontSize,
                    WeekForeground = config.DisplayOptions.WeekForeground,
                    WeekHorizontalAlignment = config.DisplayOptions.WeekHorizontalAlignment,
                    WeekMargin = config.DisplayOptions.WeekMargin,
                    DayFontSize = config.DisplayOptions.DayFontSize,
                    DayForeground = config.DisplayOptions.DayForeground,
                    DayHorizontalAlignment = config.DisplayOptions.DayHorizontalAlignment,
                    DayMargin = config.DisplayOptions.DayMargin,
                    TimeFontSize = config.DisplayOptions.TimeFontSize,
                    TimeForeground = config.DisplayOptions.TimeForeground,
                    TimeHorizontalAlignment = config.DisplayOptions.TimeHorizontalAlignment,
                    TimeMargin = config.DisplayOptions.TimeMargin,
                    
                    TickStroke = Foreground,
                    TickThickness = 1,
                    Width = tickWidth,
                    Height = 60
                };
                
                Canvas.SetLeft(multiTick, i * (tickWidth + spacing) + spacing);
                Canvas.SetTop(multiTick, 0);
                
                TimeScaleCanvas.Children.Add(multiTick);
            }
        }
    }
}