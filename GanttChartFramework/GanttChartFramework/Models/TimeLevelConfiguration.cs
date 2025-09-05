using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GanttChartFramework.Models
{
    /// <summary>
    /// Enum defining time level types
    /// </summary>
    public enum TimeLevelType
    {
        Year,
        Quarter, 
        Month,
        Week,
        Day,
        Time,
        Custom
    }

    /// <summary>
    /// Configuration for a single time level
    /// </summary>
    public class TimeLevelConfiguration
    {
        public TimeLevelType Type { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
        public double FontSize { get; set; } = 12.0;
        public Brush Foreground { get; set; } = Brushes.Black;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        public Thickness Margin { get; set; } = new Thickness(0);
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public int ZIndex { get; set; } = 0;
        
        /// <summary>
        /// Priority for display ordering (higher = more prominent)
        /// </summary>
        public int DisplayPriority { get; set; } = 0;

        /// <summary>
        /// Custom formatter function for text display
        /// </summary>
        public Func<DateTime, string> TextFormatter { get; set; }

        /// <summary>
        /// Conditional visibility based on context
        /// </summary>
        public Func<DateTime, TimeScaleContext, bool> VisibilityCondition { get; set; }

        public static TimeLevelConfiguration CreateDefault(TimeLevelType type)
        {
            return type switch
            {
                TimeLevelType.Year => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 14.0,
                    Foreground = Brushes.DarkBlue,
                    DisplayPriority = 6,
                    TextFormatter = dt => dt.Year.ToString()
                },
                TimeLevelType.Quarter => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 12.0,
                    Foreground = Brushes.DarkGreen,
                    DisplayPriority = 5,
                    TextFormatter = dt => $"Q{(dt.Month - 1) / 3 + 1}"
                },
                TimeLevelType.Month => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 12.0,
                    Foreground = Brushes.DarkRed,
                    DisplayPriority = 4,
                    TextFormatter = dt => dt.ToString("MMM")
                },
                TimeLevelType.Week => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 11.0,
                    Foreground = Brushes.Purple,
                    DisplayPriority = 3,
                    TextFormatter = dt => $"W{GetWeekNumber(dt)}"
                },
                TimeLevelType.Day => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 12.0,
                    Foreground = Brushes.Black,
                    DisplayPriority = 2,
                    TextFormatter = dt => dt.Day.ToString()
                },
                TimeLevelType.Time => new TimeLevelConfiguration
                {
                    Type = type,
                    FontSize = 11.0,
                    Foreground = Brushes.Gray,
                    DisplayPriority = 1,
                    TextFormatter = dt => dt.ToString("HH:mm")
                },
                _ => new TimeLevelConfiguration { Type = type }
            };
        }

        private static int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
    }

    /// <summary>
    /// Context information for time scale rendering
    /// </summary>
    public class TimeScaleContext
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double AvailableWidth { get; set; }
        public double AvailableHeight { get; set; }
        public int MaxVisibleLevels { get; set; } = 3;
        public double ZoomLevel { get; set; } = 1.0;
        
        public TimeSpan TotalSpan => EndDate - StartDate;
        public double DaysSpan => TotalSpan.TotalDays;
    }

    /// <summary>
    /// Multi-level time scale configuration
    /// </summary>
    public class MultiLevelTimeScaleConfiguration
    {
        public List<TimeLevelConfiguration> TimeLevels { get; set; } = new();
        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public int MaxRows { get; set; } = 3;
        public Thickness LevelMargin { get; set; } = new Thickness(2);
        public Brush TickStroke { get; set; } = Brushes.Black;
        public double TickThickness { get; set; } = 1.0;
        public DoubleCollection TickDashArray { get; set; }
        
        /// <summary>
        /// Auto-configuration based on time span and available space
        /// </summary>
        public bool EnableSmartVisibility { get; set; } = true;
        
        /// <summary>
        /// Adaptive text formatting based on available space
        /// </summary>
        public bool EnableAdaptiveFormatting { get; set; } = true;

        public static MultiLevelTimeScaleConfiguration CreateDefault()
        {
            var config = new MultiLevelTimeScaleConfiguration();
            
            foreach (TimeLevelType type in Enum.GetValues<TimeLevelType>())
            {
                if (type != TimeLevelType.Custom)
                {
                    config.TimeLevels.Add(TimeLevelConfiguration.CreateDefault(type));
                }
            }
            
            return config;
        }

        /// <summary>
        /// Auto-configure visibility based on time span and context
        /// </summary>
        public void OptimizeForContext(TimeScaleContext context)
        {
            if (!EnableSmartVisibility) return;

            // Smart visibility logic based on time span
            var daysSpan = context.DaysSpan;
            
            foreach (var level in TimeLevels)
            {
                level.IsVisible = level.Type switch
                {
                    TimeLevelType.Year => daysSpan > 365,
                    TimeLevelType.Quarter => daysSpan > 90,
                    TimeLevelType.Month => daysSpan > 30,
                    TimeLevelType.Week => daysSpan > 7 && daysSpan < 365,
                    TimeLevelType.Day => daysSpan <= 90,
                    TimeLevelType.Time => daysSpan <= 3,
                    _ => level.IsVisible
                };
            }

            // Limit to MaxRows most relevant levels
            var visibleLevels = TimeLevels
                .Where(l => l.IsVisible)
                .OrderByDescending(l => l.DisplayPriority)
                .Take(MaxRows)
                .ToList();

            foreach (var level in TimeLevels)
            {
                level.IsVisible = visibleLevels.Contains(level);
            }
        }
    }
}
