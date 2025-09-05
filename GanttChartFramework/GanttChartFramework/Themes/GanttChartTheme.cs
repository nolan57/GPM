using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GanttChartFramework.Models;

namespace GanttChartFramework.Themes
{
    /// <summary>
    /// Comprehensive theming system for Gantt chart visual appearance
    /// </summary>
    public class GanttChartTheme
    {
        public string Name { get; set; } = "Default";
        public string Description { get; set; } = string.Empty;
        
        public BackgroundTheme Background { get; set; } = new();
        public GridTheme Grid { get; set; } = new();
        public TaskTheme Task { get; set; } = new();
        public GanttTimeScaleTheme TimeScale { get; set; } = new();
        public DependencyTheme Dependency { get; set; } = new();
        public ProgressTheme Progress { get; set; } = new();
        public MilestoneTheme Milestone { get; set; } = new();
        public CriticalPathTheme CriticalPath { get; set; } = new();
        public SelectionTheme Selection { get; set; } = new();
        public TooltipTheme Tooltip { get; set; } = new();

        // Computed brushes for performance
        public Brush BackgroundBrush => new SolidColorBrush(Background.PrimaryColor);
        public TaskListTheme TaskList { get; set; } = new();

        public static GanttChartTheme CreateDefault() => new GanttChartTheme
        {
            Name = "Default",
            Description = "Default Gantt chart theme with professional appearance"
        };

        public static GanttChartTheme CreateDark() => new GanttChartTheme
        {
            Name = "Dark",
            Description = "Dark theme optimized for low-light environments",
            Background = new BackgroundTheme
            {
                PrimaryColor = Color.FromRgb(30, 30, 30),
                SecondaryColor = Color.FromRgb(45, 45, 45),
                AlternateColor = Color.FromRgb(38, 38, 38)
            },
            Grid = new GridTheme
            {
                LineColor = Color.FromRgb(60, 60, 60),
                MajorLineColor = Color.FromRgb(80, 80, 80)
            },
            Task = new TaskTheme
            {
                ForegroundColor = Color.FromRgb(240, 240, 240),
                BorderColor = Color.FromRgb(100, 100, 100)
            },
            TimeScale = new GanttTimeScaleTheme
            {
                BackgroundColor = Color.FromRgb(40, 40, 40),
                ForegroundColor = Color.FromRgb(220, 220, 220)
            }
        };

        public static GanttChartTheme CreateLight() => new GanttChartTheme
        {
            Name = "Light",
            Description = "Light theme with high contrast for readability",
            Background = new BackgroundTheme
            {
                PrimaryColor = Color.FromRgb(250, 250, 250),
                SecondaryColor = Colors.White,
                AlternateColor = Color.FromRgb(245, 247, 250)
            },
            Grid = new GridTheme
            {
                LineColor = Color.FromRgb(220, 220, 220),
                MajorLineColor = Color.FromRgb(180, 180, 180)
            }
        };

        public static GanttChartTheme CreateModern() => new GanttChartTheme
        {
            Name = "Modern",
            Description = "Modern flat design with vibrant colors",
            Task = new TaskTheme
            {
                CornerRadius = 6,
                ShadowEnabled = true,
                GradientEnabled = false
            },
            Progress = new ProgressTheme
            {
                UseGradient = true,
                AnimationEnabled = true
            }
        };

        /// <summary>
        /// Creates a custom theme based on a primary color palette
        /// </summary>
        public static GanttChartTheme CreateFromPalette(Color primaryColor, Color secondaryColor, Color accentColor)
        {
            return new GanttChartTheme
            {
                Name = "Custom",
                Background = new BackgroundTheme { PrimaryColor = LightenColor(primaryColor, 0.95f) },
                Task = new TaskTheme
                {
                    NormalColor = primaryColor,
                    CompletedColor = secondaryColor,
                    OverdueColor = accentColor
                },
                TimeScale = new GanttTimeScaleTheme
                {
                    BackgroundColor = LightenColor(primaryColor, 0.9f),
                    AccentColor = accentColor
                }
            };
        }

        private static Color LightenColor(Color color, float factor)
        {
            return Color.FromRgb(
                (byte)(color.R + (255 - color.R) * factor),
                (byte)(color.G + (255 - color.G) * factor),
                (byte)(color.B + (255 - color.B) * factor)
            );
        }
    }

    /// <summary>
    /// Background theme configuration
    /// </summary>
    public class BackgroundTheme
    {
        public Color PrimaryColor { get; set; } = Colors.White;
        public Color SecondaryColor { get; set; } = Color.FromRgb(248, 249, 250);
        public Color AlternateColor { get; set; } = Color.FromRgb(245, 247, 250);
        public bool UseGradient { get; set; } = false;
        public bool UsePattern { get; set; } = false;
        public string PatternName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Grid lines theme configuration
    /// </summary>
    public class GridTheme
    {
        public Color LineColor { get; set; } = Color.FromRgb(220, 220, 220);
        public Color MajorLineColor { get; set; } = Color.FromRgb(180, 180, 180);
        public double LineThickness { get; set; } = 1.0;
        public double MajorLineThickness { get; set; } = 1.5;
        public DoubleCollection DashPattern { get; set; }
        public bool ShowWeekendHighlight { get; set; } = true;
        public Color WeekendColor { get; set; } = Color.FromRgb(248, 248, 248);
    }

    /// <summary>
    /// Task bars theme configuration
    /// </summary>
    public class TaskTheme
    {
        public Color NormalColor { get; set; } = Color.FromRgb(52, 152, 219);
        public Color CompletedColor { get; set; } = Color.FromRgb(46, 204, 113);
        public Color OverdueColor { get; set; } = Color.FromRgb(231, 76, 60);
        public Color ParentColor { get; set; } = Color.FromRgb(155, 89, 182);
        public Color MilestoneColor { get; set; } = Color.FromRgb(241, 196, 15);
        
        public Color ForegroundColor { get; set; } = Colors.White;
        public Color BorderColor { get; set; } = Colors.Transparent;
        public double BorderThickness { get; set; } = 1.0;
        public double CornerRadius { get; set; } = 3.0;
        
        public FontFamily FontFamily { get; set; } = new("Segoe UI");
        public double FontSize { get; set; } = 11.0;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        
        public bool ShadowEnabled { get; set; } = false;
        public bool GradientEnabled { get; set; } = true;
        public double GradientStrength { get; set; } = 0.3;
        
        // Priority-based colors
        public Dictionary<TaskPriority, Color> PriorityColors { get; set; } = new()
        {
            { TaskPriority.Low, Color.FromRgb(149, 165, 166) },
            { TaskPriority.Normal, Color.FromRgb(52, 152, 219) },
            { TaskPriority.High, Color.FromRgb(230, 126, 34) },
            { TaskPriority.Critical, Color.FromRgb(231, 76, 60) }
        };

        // Status-based colors
        public Dictionary<Models.TaskStatus, Color> StatusColors { get; set; } = new()
        {
            { Models.TaskStatus.NotStarted, Color.FromRgb(149, 165, 166) },
            { Models.TaskStatus.InProgress, Color.FromRgb(52, 152, 219) },
            { Models.TaskStatus.OnHold, Color.FromRgb(230, 126, 34) },
            { Models.TaskStatus.Completed, Color.FromRgb(46, 204, 113) },
            { Models.TaskStatus.Cancelled, Color.FromRgb(127, 140, 141) }
        };

        public Brush ForegroundBrush => new SolidColorBrush(ForegroundColor);
    }

    /// <summary>
    /// Time scale theme configuration
    /// </summary>
    public class GanttTimeScaleTheme
    {
        public Color BackgroundColor { get; set; } = Color.FromRgb(248, 249, 250);
        public Color ForegroundColor { get; set; } = Color.FromRgb(52, 58, 64);
        public Color AccentColor { get; set; } = Color.FromRgb(52, 152, 219);
        public Color BorderColor { get; set; } = Color.FromRgb(220, 220, 220);
        
        public FontFamily FontFamily { get; set; } = new("Segoe UI");
        public Dictionary<TimeLevelType, double> FontSizes { get; set; } = new()
        {
            { TimeLevelType.Year, 14.0 },
            { TimeLevelType.Quarter, 12.0 },
            { TimeLevelType.Month, 12.0 },
            { TimeLevelType.Week, 11.0 },
            { TimeLevelType.Day, 12.0 },
            { TimeLevelType.Time, 10.0 }
        };
        
        public bool ShowTodayMarker { get; set; } = true;
        public Color TodayMarkerColor { get; set; } = Color.FromRgb(220, 53, 69);
        public double TodayMarkerThickness { get; set; } = 2.0;
    }

    /// <summary>
    /// Task dependencies theme configuration
    /// </summary>
    public class DependencyTheme
    {
        public Color LineColor { get; set; } = Color.FromRgb(108, 117, 125);
        public double LineThickness { get; set; } = 1.5;
        public DoubleCollection DashPattern { get; set; }
        public Color ArrowColor { get; set; } = Color.FromRgb(108, 117, 125);
        public double ArrowSize { get; set; } = 8.0;
        public bool ShowLabels { get; set; } = false;
        
        public Brush LineBrush => new SolidColorBrush(LineColor);
    }

    /// <summary>
    /// Progress indicators theme configuration
    /// </summary>
    public class ProgressTheme
    {
        public Color FillColor { get; set; } = Color.FromRgb(40, 167, 69);
        public Color BackgroundColor { get; set; } = Color.FromRgb(233, 236, 239);
        public bool UseGradient { get; set; } = false;
        public bool ShowPercentage { get; set; } = true;
        public bool AnimationEnabled { get; set; } = false;
        public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(300);
    }

    /// <summary>
    /// Milestone markers theme configuration
    /// </summary>
    public class MilestoneTheme
    {
        public Color Color { get; set; } = Color.FromRgb(241, 196, 15);
        public Color BorderColor { get; set; } = Color.FromRgb(243, 156, 18);
        public double Size { get; set; } = 16.0;
        public MilestoneShape Shape { get; set; } = MilestoneShape.Diamond;
        public bool ShowLabel { get; set; } = true;
    }

    /// <summary>
    /// Critical path highlighting theme
    /// </summary>
    public class CriticalPathTheme
    {
        public Color HighlightColor { get; set; } = Color.FromRgb(220, 53, 69);
        public double HighlightThickness { get; set; } = 3.0;
        public bool UseGlow { get; set; } = true;
        public double GlowRadius { get; set; } = 5.0;
    }

    /// <summary>
    /// Selection indicators theme
    /// </summary>
    public class SelectionTheme
    {
        public Color BorderColor { get; set; } = Color.FromRgb(0, 123, 255);
        public Color FillColor { get; set; } = Color.FromArgb(50, 0, 123, 255);
        public double BorderThickness { get; set; } = 2.0;
        public DoubleCollection DashPattern { get; set; } = new() { 5, 3 };
        public bool AnimateSelection { get; set; } = true;
    }

    /// <summary>
    /// Tooltip appearance theme
    /// </summary>
    public class TooltipTheme
    {
        public Color BackgroundColor { get; set; } = Color.FromRgb(45, 45, 45);
        public Color ForegroundColor { get; set; } = Colors.White;
        public Color BorderColor { get; set; } = Color.FromRgb(60, 60, 60);
        public double CornerRadius { get; set; } = 4.0;
        public FontFamily FontFamily { get; set; } = new("Segoe UI");
        public double FontSize { get; set; } = 11.0;
        public Thickness Padding { get; set; } = new(8, 4, 8, 4);
        public bool ShowShadow { get; set; } = true;
    }

    /// <summary>
    /// Task list panel theme
    /// </summary>
    public class TaskListTheme
    {
        public Color BackgroundColor { get; set; } = Color.FromRgb(248, 249, 250);
        public Color ForegroundColor { get; set; } = Color.FromRgb(52, 58, 64);
        public Color BorderColor { get; set; } = Color.FromRgb(220, 220, 220);
        public FontFamily FontFamily { get; set; } = new("Segoe UI");
        public double FontSize { get; set; } = 12.0;
        public double RowHeight { get; set; } = 40.0;
        public bool ShowHierarchyLines { get; set; } = true;
        public Color HierarchyLineColor { get; set; } = Color.FromRgb(180, 180, 180);
    }

    /// <summary>
    /// Milestone shape enumeration
    /// </summary>
    public enum MilestoneShape
    {
        Diamond,
        Circle,
        Square,
        Triangle,
        Star
    }

    /// <summary>
    /// Theme manager for handling theme operations
    /// </summary>
    public class ThemeManager
    {
        private static readonly Dictionary<string, GanttChartTheme> _themes = new();
        
        static ThemeManager()
        {
            RegisterDefaultThemes();
        }

        private static void RegisterDefaultThemes()
        {
            RegisterTheme(GanttChartTheme.CreateDefault());
            RegisterTheme(GanttChartTheme.CreateDark());
            RegisterTheme(GanttChartTheme.CreateLight());
            RegisterTheme(GanttChartTheme.CreateModern());
        }

        public static void RegisterTheme(GanttChartTheme theme)
        {
            _themes[theme.Name] = theme;
        }

        public static GanttChartTheme GetTheme(string name)
        {
            return _themes.TryGetValue(name, out var theme) ? theme : GanttChartTheme.CreateDefault();
        }

        public static IEnumerable<string> GetAvailableThemes()
        {
            return _themes.Keys;
        }

        public static GanttChartTheme CreateCustomTheme(string name, Action<GanttChartTheme> configure)
        {
            var theme = GanttChartTheme.CreateDefault();
            theme.Name = name;
            configure(theme);
            RegisterTheme(theme);
            return theme;
        }
    }
}
