using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GanttChartFramework.Views
{
    public partial class MultiLevelTimeScaleTick : UserControl
    {
        #region 依赖属性 - 时间文本
        
        public static readonly DependencyProperty YearTextProperty =
            DependencyProperty.Register("YearText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty QuarterTextProperty =
            DependencyProperty.Register("QuarterText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty MonthTextProperty =
            DependencyProperty.Register("MonthText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty WeekTextProperty =
            DependencyProperty.Register("WeekText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty DayTextProperty =
            DependencyProperty.Register("DayText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty TimeTextProperty =
            DependencyProperty.Register("TimeText", typeof(string), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(""));
        
        #endregion
        
        #region 依赖属性 - 显示控制
        
        public static readonly DependencyProperty ShowYearProperty =
            DependencyProperty.Register("ShowYear", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(true));
        
        public static readonly DependencyProperty ShowQuarterProperty =
            DependencyProperty.Register("ShowQuarter", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(false));
        
        public static readonly DependencyProperty ShowMonthProperty =
            DependencyProperty.Register("ShowMonth", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(true));
        
        public static readonly DependencyProperty ShowWeekProperty =
            DependencyProperty.Register("ShowWeek", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(false));
        
        public static readonly DependencyProperty ShowDayProperty =
            DependencyProperty.Register("ShowDay", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(true));
        
        public static readonly DependencyProperty ShowTimeProperty =
            DependencyProperty.Register("ShowTime", typeof(bool), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(false));
        
        #endregion
        
        #region 依赖属性 - 布局控制
        
        public static readonly DependencyProperty LayoutOrientationProperty =
            DependencyProperty.Register("LayoutOrientation", typeof(Orientation), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Orientation.Vertical, OnLayoutOrientationChanged));
        
        public static readonly DependencyProperty MaxRowsProperty =
            DependencyProperty.Register("MaxRows", typeof(int), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(3));
        
        public static readonly DependencyProperty LevelMarginProperty =
            DependencyProperty.Register("LevelMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(2)));
        
        #endregion
        
        #region 依赖属性 - 样式控制
        
        public static readonly DependencyProperty YearFontSizeProperty =
            DependencyProperty.Register("YearFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(14.0));
        
        public static readonly DependencyProperty YearForegroundProperty =
            DependencyProperty.Register("YearForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.DarkBlue));
        
        public static readonly DependencyProperty YearHorizontalAlignmentProperty =
            DependencyProperty.Register("YearHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty YearMarginProperty =
            DependencyProperty.Register("YearMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty QuarterFontSizeProperty =
            DependencyProperty.Register("QuarterFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(12.0));
        
        public static readonly DependencyProperty QuarterForegroundProperty =
            DependencyProperty.Register("QuarterForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.DarkGreen));
        
        public static readonly DependencyProperty QuarterHorizontalAlignmentProperty =
            DependencyProperty.Register("QuarterHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty QuarterMarginProperty =
            DependencyProperty.Register("QuarterMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty MonthFontSizeProperty =
            DependencyProperty.Register("MonthFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(12.0));
        
        public static readonly DependencyProperty MonthForegroundProperty =
            DependencyProperty.Register("MonthForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.DarkRed));
        
        public static readonly DependencyProperty MonthHorizontalAlignmentProperty =
            DependencyProperty.Register("MonthHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty MonthMarginProperty =
            DependencyProperty.Register("MonthMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty WeekFontSizeProperty =
            DependencyProperty.Register("WeekFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(11.0));
        
        public static readonly DependencyProperty WeekForegroundProperty =
            DependencyProperty.Register("WeekForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.Purple));
        
        public static readonly DependencyProperty WeekHorizontalAlignmentProperty =
            DependencyProperty.Register("WeekHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty WeekMarginProperty =
            DependencyProperty.Register("WeekMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty DayFontSizeProperty =
            DependencyProperty.Register("DayFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(12.0));
        
        public static readonly DependencyProperty DayForegroundProperty =
            DependencyProperty.Register("DayForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.Black));
        
        public static readonly DependencyProperty DayHorizontalAlignmentProperty =
            DependencyProperty.Register("DayHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty DayMarginProperty =
            DependencyProperty.Register("DayMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty TimeFontSizeProperty =
            DependencyProperty.Register("TimeFontSize", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(11.0));
        
        public static readonly DependencyProperty TimeForegroundProperty =
            DependencyProperty.Register("TimeForeground", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.Gray));
        
        public static readonly DependencyProperty TimeHorizontalAlignmentProperty =
            DependencyProperty.Register("TimeHorizontalAlignment", typeof(HorizontalAlignment), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(HorizontalAlignment.Center));
        
        public static readonly DependencyProperty TimeMarginProperty =
            DependencyProperty.Register("TimeMargin", typeof(Thickness), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(new Thickness(0)));
        
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(Brushes.Black));
        
        public static readonly DependencyProperty TickThicknessProperty =
            DependencyProperty.Register("TickThickness", typeof(double), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty TickDashArrayProperty =
            DependencyProperty.Register("TickDashArray", typeof(DoubleCollection), typeof(MultiLevelTimeScaleTick),
                new PropertyMetadata(null));
        
        #endregion
        
        #region 属性包装器 - 时间文本
        
        public string YearText
        {
            get => (string)GetValue(YearTextProperty);
            set => SetValue(YearTextProperty, value);
        }
        
        public string QuarterText
        {
            get => (string)GetValue(QuarterTextProperty);
            set => SetValue(QuarterTextProperty, value);
        }
        
        public string MonthText
        {
            get => (string)GetValue(MonthTextProperty);
            set => SetValue(MonthTextProperty, value);
        }
        
        public string WeekText
        {
            get => (string)GetValue(WeekTextProperty);
            set => SetValue(WeekTextProperty, value);
        }
        
        public string DayText
        {
            get => (string)GetValue(DayTextProperty);
            set => SetValue(DayTextProperty, value);
        }
        
        public string TimeText
        {
            get => (string)GetValue(TimeTextProperty);
            set => SetValue(TimeTextProperty, value);
        }
        
        #endregion
        
        #region 属性包装器 - 显示控制
        
        public bool ShowYear
        {
            get => (bool)GetValue(ShowYearProperty);
            set => SetValue(ShowYearProperty, value);
        }
        
        public bool ShowQuarter
        {
            get => (bool)GetValue(ShowQuarterProperty);
            set => SetValue(ShowQuarterProperty, value);
        }
        
        public bool ShowMonth
        {
            get => (bool)GetValue(ShowMonthProperty);
            set => SetValue(ShowMonthProperty, value);
        }
        
        public bool ShowWeek
        {
            get => (bool)GetValue(ShowWeekProperty);
            set => SetValue(ShowWeekProperty, value);
        }
        
        public bool ShowDay
        {
            get => (bool)GetValue(ShowDayProperty);
            set => SetValue(ShowDayProperty, value);
        }
        
        public bool ShowTime
        {
            get => (bool)GetValue(ShowTimeProperty);
            set => SetValue(ShowTimeProperty, value);
        }
        
        #endregion
        
        #region 属性包装器 - 布局控制
        
        public Orientation LayoutOrientation
        {
            get => (Orientation)GetValue(LayoutOrientationProperty);
            set => SetValue(LayoutOrientationProperty, value);
        }
        
        public int MaxRows
        {
            get => (int)GetValue(MaxRowsProperty);
            set => SetValue(MaxRowsProperty, value);
        }
        
        public Thickness LevelMargin
        {
            get => (Thickness)GetValue(LevelMarginProperty);
            set => SetValue(LevelMarginProperty, value);
        }
        
        #endregion
        
        #region 属性包装器 - 样式控制
        
        public double YearFontSize
        {
            get => (double)GetValue(YearFontSizeProperty);
            set => SetValue(YearFontSizeProperty, value);
        }

        public Brush YearForeground
        {
            get => (Brush)GetValue(YearForegroundProperty);
            set => SetValue(YearForegroundProperty, value);
        }

        public HorizontalAlignment YearHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(YearHorizontalAlignmentProperty);
            set => SetValue(YearHorizontalAlignmentProperty, value);
        }

        public Thickness YearMargin
        {
            get => (Thickness)GetValue(YearMarginProperty);
            set => SetValue(YearMarginProperty, value);
        }

        public double QuarterFontSize
        {
            get => (double)GetValue(QuarterFontSizeProperty);
            set => SetValue(QuarterFontSizeProperty, value);
        }

        public Brush QuarterForeground
        {
            get => (Brush)GetValue(QuarterForegroundProperty);
            set => SetValue(QuarterForegroundProperty, value);
        }

        public HorizontalAlignment QuarterHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(QuarterHorizontalAlignmentProperty);
            set => SetValue(QuarterHorizontalAlignmentProperty, value);
        }

        public Thickness QuarterMargin
        {
            get => (Thickness)GetValue(QuarterMarginProperty);
            set => SetValue(QuarterMarginProperty, value);
        }

        public double MonthFontSize
        {
            get => (double)GetValue(MonthFontSizeProperty);
            set => SetValue(MonthFontSizeProperty, value);
        }

        public Brush MonthForeground
        {
            get => (Brush)GetValue(MonthForegroundProperty);
            set => SetValue(MonthForegroundProperty, value);
        }

        public HorizontalAlignment MonthHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(MonthHorizontalAlignmentProperty);
            set => SetValue(MonthHorizontalAlignmentProperty, value);
        }

        public Thickness MonthMargin
        {
            get => (Thickness)GetValue(MonthMarginProperty);
            set => SetValue(MonthMarginProperty, value);
        }

        public double WeekFontSize
        {
            get => (double)GetValue(WeekFontSizeProperty);
            set => SetValue(WeekFontSizeProperty, value);
        }

        public Brush WeekForeground
        {
            get => (Brush)GetValue(WeekForegroundProperty);
            set => SetValue(WeekForegroundProperty, value);
        }

        public HorizontalAlignment WeekHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(WeekHorizontalAlignmentProperty);
            set => SetValue(WeekHorizontalAlignmentProperty, value);
        }

        public Thickness WeekMargin
        {
            get => (Thickness)GetValue(WeekMarginProperty);
            set => SetValue(WeekMarginProperty, value);
        }

        public double DayFontSize
        {
            get => (double)GetValue(DayFontSizeProperty);
            set => SetValue(DayFontSizeProperty, value);
        }

        public Brush DayForeground
        {
            get => (Brush)GetValue(DayForegroundProperty);
            set => SetValue(DayForegroundProperty, value);
        }

        public HorizontalAlignment DayHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(DayHorizontalAlignmentProperty);
            set => SetValue(DayHorizontalAlignmentProperty, value);
        }

        public Thickness DayMargin
        {
            get => (Thickness)GetValue(DayMarginProperty);
            set => SetValue(DayMarginProperty, value);
        }

        public double TimeFontSize
        {
            get => (double)GetValue(TimeFontSizeProperty);
            set => SetValue(TimeFontSizeProperty, value);
        }

        public Brush TimeForeground
        {
            get => (Brush)GetValue(TimeForegroundProperty);
            set => SetValue(TimeForegroundProperty, value);
        }

        public HorizontalAlignment TimeHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(TimeHorizontalAlignmentProperty);
            set => SetValue(TimeHorizontalAlignmentProperty, value);
        }

        public Thickness TimeMargin
        {
            get => (Thickness)GetValue(TimeMarginProperty);
            set => SetValue(TimeMarginProperty, value);
        }

        public Brush TickStroke
        {
            get => (Brush)GetValue(TickStrokeProperty);
            set => SetValue(TickStrokeProperty, value);
        }

        public double TickThickness
        {
            get => (double)GetValue(TickThicknessProperty);
            set => SetValue(TickThicknessProperty, value);
        }

        public DoubleCollection TickDashArray
        {
            get => (DoubleCollection)GetValue(TickDashArrayProperty);
            set => SetValue(TickDashArrayProperty, value);
        }

        #endregion
        
        public MultiLevelTimeScaleTick()
        {
            InitializeComponent();
            this.SizeChanged += MultiLevelTimeScaleTick_SizeChanged;
            UpdateRightTickLinePosition();
        }
        
        private void MultiLevelTimeScaleTick_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdateRightTickLinePosition();
        }
        
        private void UpdateRightTickLinePosition()
        {
            if (RightTickLine != null)
            {
                double rightEdge = this.ActualWidth;
                RightTickLine.X1 = rightEdge;
                RightTickLine.X2 = rightEdge;
            }
        }
        
        private static void OnLayoutOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultiLevelTimeScaleTick;
            control?.UpdateLayoutOrientation();
        }
        
        private void UpdateLayoutOrientation()
        {
            if (TimeLevelsPanel != null)
            {
                TimeLevelsPanel.Orientation = LayoutOrientation;
            }
        }
        
        // 方法：设置所有时间文本
        public void SetTimeTexts(string year, string quarter, string month, string week, string day, string time)
        {
            YearText = year;
            QuarterText = quarter;
            MonthText = month;
            WeekText = week;
            DayText = day;
            TimeText = time;
        }
        
        // 方法：设置显示级别
        public void SetDisplayLevels(bool showYear, bool showQuarter, bool showMonth, bool showWeek, bool showDay, bool showTime)
        {
            ShowYear = showYear;
            ShowQuarter = showQuarter;
            ShowMonth = showMonth;
            ShowWeek = showWeek;
            ShowDay = showDay;
            ShowTime = showTime;
        }
        
        // 方法：设置样式
        public void SetLevelStyles(
            double yearSize, Brush yearColor,
            double quarterSize, Brush quarterColor,
            double monthSize, Brush monthColor,
            double weekSize, Brush weekColor,
            double daySize, Brush dayColor,
            double timeSize, Brush timeColor)
        {
            YearFontSize = yearSize;
            YearForeground = yearColor;
            QuarterFontSize = quarterSize;
            QuarterForeground = quarterColor;
            MonthFontSize = monthSize;
            MonthForeground = monthColor;
            WeekFontSize = weekSize;
            WeekForeground = weekColor;
            DayFontSize = daySize;
            DayForeground = dayColor;
            TimeFontSize = timeSize;
            TimeForeground = timeColor;
        }
    }
}