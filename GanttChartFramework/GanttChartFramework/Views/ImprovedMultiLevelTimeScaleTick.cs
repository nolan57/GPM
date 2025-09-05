using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GanttChartFramework.Models;

namespace GanttChartFramework.Views
{
    /// <summary>
    /// Improved multi-level time scale tick control with reduced complexity and enhanced features
    /// </summary>
    public class ImprovedMultiLevelTimeScaleTick : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(nameof(Configuration), typeof(MultiLevelTimeScaleConfiguration), 
                typeof(ImprovedMultiLevelTimeScaleTick), 
                new PropertyMetadata(null, OnConfigurationChanged));

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(nameof(DateTime), typeof(DateTime), 
                typeof(ImprovedMultiLevelTimeScaleTick),
                new PropertyMetadata(System.DateTime.Now, OnDateTimeChanged));

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register(nameof(Context), typeof(TimeScaleContext), 
                typeof(ImprovedMultiLevelTimeScaleTick),
                new PropertyMetadata(null, OnContextChanged));

        public static readonly DependencyProperty EnableAnimationsProperty =
            DependencyProperty.Register(nameof(EnableAnimations), typeof(bool), 
                typeof(ImprovedMultiLevelTimeScaleTick),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register(nameof(IsHighlighted), typeof(bool), 
                typeof(ImprovedMultiLevelTimeScaleTick),
                new PropertyMetadata(false, OnHighlightChanged));

        #endregion

        #region Properties

        public MultiLevelTimeScaleConfiguration Configuration
        {
            get => (MultiLevelTimeScaleConfiguration)GetValue(ConfigurationProperty);
            set => SetValue(ConfigurationProperty, value);
        }

        public DateTime DateTime
        {
            get => (DateTime)GetValue(DateTimeProperty);
            set => SetValue(DateTimeProperty, value);
        }

        public TimeScaleContext Context
        {
            get => (TimeScaleContext)GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        public bool EnableAnimations
        {
            get => (bool)GetValue(EnableAnimationsProperty);
            set => SetValue(EnableAnimationsProperty, value);
        }

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        #endregion

        #region Private Fields

        private StackPanel _timeLevelsPanel;
        private Line _leftTickLine;
        private Line _rightTickLine;
        private Border _highlightBorder;

        #endregion

        #region Constructor

        public ImprovedMultiLevelTimeScaleTick()
        {
            InitializeComponent();
            Configuration = MultiLevelTimeScaleConfiguration.CreateDefault();
        }

        #endregion

        #region Initialization

        private void InitializeComponent()
        {
            var mainGrid = new Grid();
            
            // Highlight border (invisible by default)
            _highlightBorder = new Border
            {
                Background = new SolidColorBrush(Colors.Yellow) { Opacity = 0.3 },
                Visibility = Visibility.Collapsed
            };
            mainGrid.Children.Add(_highlightBorder);

            // Time levels container
            _timeLevelsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            mainGrid.Children.Add(_timeLevelsPanel);

            // Tick lines
            _leftTickLine = new Line
            {
                X1 = 0, Y1 = 0, X2 = 0, Y2 = 60,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            mainGrid.Children.Add(_leftTickLine);

            _rightTickLine = new Line
            {
                X1 = 0, Y1 = 0, X2 = 0, Y2 = 60,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            mainGrid.Children.Add(_rightTickLine);

            Content = mainGrid;
            
            SizeChanged += OnSizeChanged;
        }

        #endregion

        #region Event Handlers

        private static void OnConfigurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedMultiLevelTimeScaleTick control)
            {
                control.UpdateDisplay();
            }
        }

        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedMultiLevelTimeScaleTick control)
            {
                control.UpdateDisplay();
            }
        }

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedMultiLevelTimeScaleTick control)
            {
                control.OptimizeConfiguration();
                control.UpdateDisplay();
            }
        }

        private static void OnHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedMultiLevelTimeScaleTick control)
            {
                control.UpdateHighlight();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTickLines();
        }

        #endregion

        #region Display Update Methods

        private void OptimizeConfiguration()
        {
            if (Configuration != null && Context != null)
            {
                Configuration.OptimizeForContext(Context);
            }
        }

        private void UpdateDisplay()
        {
            if (Configuration == null) return;

            _timeLevelsPanel.Children.Clear();
            _timeLevelsPanel.Orientation = Configuration.Orientation;

            // Get visible time levels ordered by priority
            var visibleLevels = Configuration.TimeLevels
                .Where(level => level.IsVisible && ShouldShowLevel(level))
                .OrderByDescending(level => level.DisplayPriority)
                .Take(Configuration.MaxRows);

            foreach (var level in visibleLevels)
            {
                var textBlock = CreateTextBlockForLevel(level);
                _timeLevelsPanel.Children.Add(textBlock);
            }

            UpdateTickLines();
        }

        private bool ShouldShowLevel(TimeLevelConfiguration level)
        {
            // Additional context-based visibility logic
            if (level.VisibilityCondition != null && Context != null)
            {
                return level.VisibilityCondition(DateTime, Context);
            }
            
            return true;
        }

        private TextBlock CreateTextBlockForLevel(TimeLevelConfiguration level)
        {
            var textBlock = new TextBlock
            {
                Text = GetFormattedText(level),
                FontSize = level.FontSize,
                Foreground = level.Foreground,
                HorizontalAlignment = level.HorizontalAlignment,
                Margin = level.Margin,
                FontWeight = level.FontWeight,
                FontStyle = level.FontStyle
            };

            // Add tooltip for additional information
            if (!string.IsNullOrEmpty(level.Text))
            {
                textBlock.ToolTip = $"{level.Type}: {level.Text}";
            }

            // Add animations if enabled
            if (EnableAnimations)
            {
                AddFadeInAnimation(textBlock);
            }

            return textBlock;
        }

        private string GetFormattedText(TimeLevelConfiguration level)
        {
            if (!string.IsNullOrEmpty(level.Text))
            {
                return level.Text;
            }

            if (level.TextFormatter != null)
            {
                return level.TextFormatter(DateTime);
            }

            // Fallback formatting
            return level.Type switch
            {
                TimeLevelType.Year => DateTime.Year.ToString(),
                TimeLevelType.Quarter => $"Q{(DateTime.Month - 1) / 3 + 1}",
                TimeLevelType.Month => DateTime.ToString("MMM"),
                TimeLevelType.Week => $"W{GetWeekNumber(DateTime)}",
                TimeLevelType.Day => DateTime.Day.ToString(),
                TimeLevelType.Time => DateTime.ToString("HH:mm"),
                _ => string.Empty
            };
        }

        private void UpdateTickLines()
        {
            if (Configuration == null) return;

            // Update tick line properties
            _leftTickLine.Stroke = Configuration.TickStroke;
            _leftTickLine.StrokeThickness = Configuration.TickThickness;
            _leftTickLine.StrokeDashArray = Configuration.TickDashArray;

            _rightTickLine.Stroke = Configuration.TickStroke;
            _rightTickLine.StrokeThickness = Configuration.TickThickness;
            _rightTickLine.StrokeDashArray = Configuration.TickDashArray;

            // Update tick line positions
            _rightTickLine.X1 = ActualWidth;
            _rightTickLine.X2 = ActualWidth;
        }

        private void UpdateHighlight()
        {
            _highlightBorder.Visibility = IsHighlighted ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Animation Methods

        private void AddFadeInAnimation(FrameworkElement element)
        {
            if (!EnableAnimations) return;

            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            element.BeginAnimation(OpacityProperty, fadeIn);
        }

        #endregion

        #region Utility Methods

        private static int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        #endregion

        #region Public API Methods

        /// <summary>
        /// Updates the configuration for a specific time level
        /// </summary>
        public void UpdateTimeLevel(TimeLevelType levelType, Action<TimeLevelConfiguration> configAction)
        {
            var level = Configuration?.TimeLevels.FirstOrDefault(l => l.Type == levelType);
            if (level != null)
            {
                configAction(level);
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Sets visibility for multiple time levels at once
        /// </summary>
        public void SetLevelVisibility(params (TimeLevelType type, bool visible)[] visibilitySettings)
        {
            if (Configuration == null) return;

            foreach (var (type, visible) in visibilitySettings)
            {
                var level = Configuration.TimeLevels.FirstOrDefault(l => l.Type == type);
                if (level != null)
                {
                    level.IsVisible = visible;
                }
            }
            UpdateDisplay();
        }

        /// <summary>
        /// Applies a theme to all time levels
        /// </summary>
        public void ApplyTheme(TimeScaleTheme theme)
        {
            if (Configuration == null) return;

            foreach (var level in Configuration.TimeLevels)
            {
                theme.Apply(level);
            }
            UpdateDisplay();
        }

        #endregion
    }

    /// <summary>
    /// Theme system for consistent styling
    /// </summary>
    public class TimeScaleTheme
    {
        public Brush PrimaryColor { get; set; } = Brushes.DarkBlue;
        public Brush SecondaryColor { get; set; } = Brushes.DarkGray;
        public Brush AccentColor { get; set; } = Brushes.DarkRed;
        public double BaseFontSize { get; set; } = 12.0;
        public FontFamily FontFamily { get; set; } = new FontFamily("Segoe UI");

        public virtual void Apply(TimeLevelConfiguration level)
        {
            level.Foreground = level.DisplayPriority switch
            {
                >= 5 => PrimaryColor,
                >= 3 => SecondaryColor,
                _ => AccentColor
            };
            
            level.FontSize = BaseFontSize + (level.DisplayPriority - 3) * 0.5;
        }

        public static TimeScaleTheme CreateModern() => new TimeScaleTheme
        {
            PrimaryColor = new SolidColorBrush(Color.FromRgb(45, 62, 80)),
            SecondaryColor = new SolidColorBrush(Color.FromRgb(108, 117, 125)),
            AccentColor = new SolidColorBrush(Color.FromRgb(220, 53, 69)),
            BaseFontSize = 11.0,
            FontFamily = new FontFamily("Segoe UI")
        };
    }
}
