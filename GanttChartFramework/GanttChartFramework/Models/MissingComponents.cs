using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GanttChartFramework.Models;
using GanttChartFramework.Themes;
using GanttChartFramework.Services;

namespace GanttChartFramework.Models
{
    /// <summary>
    /// Time scale configuration for the Gantt chart
    /// </summary>
    public class TimeScaleConfiguration
    {
        public TimeLevelType DefaultScale { get; set; } = TimeLevelType.Day;
        public bool ShowWeekends { get; set; } = true;
        public bool ShowToday { get; set; } = true;
        public bool ShowHolidays { get; set; } = false;
        public double MinimumColumnWidth { get; set; } = 30;
        public double MaximumColumnWidth { get; set; } = 200;
        public bool EnableAutoScale { get; set; } = true;
        public TimeScaleHeader HeaderConfiguration { get; set; } = new();

        public static TimeScaleConfiguration CreateDefault() => new();
    }

    public class TimeScaleHeader
    {
        public double Height { get; set; } = 60;
        public bool ShowMultipleLevels { get; set; } = true;
        public int MaxLevels { get; set; } = 3;
        public Brush BackgroundBrush { get; set; } = new SolidColorBrush(Color.FromRgb(248, 249, 250));
    }
}

namespace GanttChartFramework.Views
{
    /// <summary>
    /// Time scale panel for displaying time headers
    /// </summary>
    public class TimeScalePanel : UserControl
    {
        public TimeScaleConfiguration Configuration { get; set; }
        public GanttViewport Viewport { get; set; }

        public void ApplyConfiguration(TimeScaleConfiguration config)
        {
            Configuration = config;
            RefreshDisplay();
        }

        public void ApplyTheme(TimeScaleTheme theme)
        {
            Background = theme.PrimaryColor;
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            // Implementation would create time scale headers based on configuration
        }
    }

    /// <summary>
    /// Task list panel for displaying task hierarchy
    /// </summary>
    public class TaskListPanel : UserControl
    {
        public TaskDisplayConfiguration Configuration { get; set; }

        public void ApplyConfiguration(TaskDisplayConfiguration config)
        {
            Configuration = config;
            RefreshDisplay();
        }

        public void ApplyTheme(TaskListTheme theme)
        {
            Background = new SolidColorBrush(theme.BackgroundColor);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            // Implementation would create task list based on configuration
        }
    }
}
