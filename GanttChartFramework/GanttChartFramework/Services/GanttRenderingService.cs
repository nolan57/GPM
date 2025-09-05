using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GanttChartFramework.Models;
using GanttChartFramework.Themes;
using GanttChartFramework.Views;

namespace GanttChartFramework.Services
{
    /// <summary>
    /// High-performance rendering service for Gantt chart visualization
    /// </summary>
    public class GanttRenderingService
    {
        private readonly Dictionary<string, DrawingBrush> _patternCache = new();
        private readonly Dictionary<Color, SolidColorBrush> _brushCache = new();
        private readonly Queue<UIElement> _elementPool = new();
        private const int MaxPoolSize = 500;

        #region Core Rendering Methods

        public void RenderGanttChart(Canvas canvas, List<EnhancedTaskItem> tasks, 
                                   GanttChartConfiguration config, GanttViewport viewport, 
                                   GanttChartTheme theme)
        {
            if (canvas == null || config == null || viewport == null) return;

            // Clear existing content
            canvas.Children.Clear();

            // Calculate rendering bounds
            var bounds = CalculateRenderingBounds(tasks, viewport);
            
            // Render in layers for optimal performance
            RenderBackground(canvas, bounds, theme);
            RenderGridLines(canvas, bounds, config.Grid, theme);
            RenderTimeScale(canvas, bounds, config.TimeScale, theme);
            RenderTaskBars(canvas, tasks, bounds, config.TaskDisplay, theme);
            RenderDependencyLines(canvas, tasks, bounds, theme);
            RenderMilestones(canvas, tasks.Where(t => t.IsMilestone), bounds, theme);
            RenderCriticalPath(canvas, tasks.Where(t => t.IsCritical), bounds, theme);
        }

        #endregion

        #region Layer Rendering Methods

        private void RenderBackground(Canvas canvas, RenderingBounds bounds, GanttChartTheme theme)
        {
            var background = new Rectangle
            {
                Width = bounds.TotalWidth,
                Height = bounds.TotalHeight,
                Fill = theme.BackgroundBrush
            };

            Canvas.SetLeft(background, 0);
            Canvas.SetTop(background, 0);
            Canvas.SetZIndex(background, -100);
            
            canvas.Children.Add(background);
        }

        private void RenderGridLines(Canvas canvas, RenderingBounds bounds, 
                                   GridConfiguration config, GanttChartTheme theme)
        {
            if (!config.ShowVerticalLines && !config.ShowHorizontalLines) return;

            var gridGroup = new Canvas();
            Canvas.SetZIndex(gridGroup, -50);

            // Vertical lines (time divisions)
            if (config.ShowVerticalLines)
            {
                RenderVerticalGridLines(gridGroup, bounds, config, theme);
            }

            // Horizontal lines (task divisions)
            if (config.ShowHorizontalLines)
            {
                RenderHorizontalGridLines(gridGroup, bounds, config, theme);
            }

            canvas.Children.Add(gridGroup);
        }

        private void RenderVerticalGridLines(Canvas canvas, RenderingBounds bounds, 
                                           GridConfiguration config, GanttChartTheme theme)
        {
            var interval = CalculateTimeInterval(bounds.TimeSpan, bounds.TotalWidth);
            var currentDate = bounds.StartDate;
            var x = bounds.TimeScaleStartX;

            while (currentDate <= bounds.EndDate && x <= bounds.TotalWidth)
            {
                var line = GetPooledLine();
                line.X1 = x;
                line.Y1 = bounds.TimeScaleHeight;
                line.X2 = x;
                line.Y2 = bounds.TotalHeight;
                line.Stroke = GetCachedBrush(theme.Grid.LineColor);
                line.StrokeThickness = config.LineThickness;
                line.StrokeDashArray = config.DashPattern;

                canvas.Children.Add(line);

                currentDate = currentDate.Add(interval);
                x += interval.TotalDays * bounds.PixelsPerDay;
            }
        }

        private void RenderHorizontalGridLines(Canvas canvas, RenderingBounds bounds, 
                                             GridConfiguration config, GanttChartTheme theme)
        {
            var taskHeight = config.TaskHeight;
            var y = bounds.TimeScaleHeight;

            while (y <= bounds.TotalHeight)
            {
                var line = GetPooledLine();
                line.X1 = bounds.TimeScaleStartX;
                line.Y1 = y;
                line.X2 = bounds.TotalWidth;
                line.Y2 = y;
                line.Stroke = GetCachedBrush(theme.Grid.LineColor);
                line.StrokeThickness = config.LineThickness;
                line.StrokeDashArray = config.DashPattern;

                canvas.Children.Add(line);
                y += taskHeight;
            }
        }

        private void RenderTaskBars(Canvas canvas, List<EnhancedTaskItem> tasks, 
                                  RenderingBounds bounds, TaskDisplayConfiguration config, 
                                  GanttChartTheme theme)
        {
            var taskGroup = new Canvas();
            Canvas.SetZIndex(taskGroup, 10);

            var sortedTasks = SortTasksForDisplay(tasks, config.SortBy);
            var y = bounds.TimeScaleHeight + config.TaskMargin;

            foreach (var task in sortedTasks)
            {
                if (IsTaskVisible(task, bounds))
                {
                    RenderSingleTaskBar(taskGroup, task, bounds, config, theme, y);
                }
                y += config.TaskHeight + config.TaskSpacing;
            }

            canvas.Children.Add(taskGroup);
        }

        private void RenderSingleTaskBar(Canvas canvas, EnhancedTaskItem task, 
                                       RenderingBounds bounds, TaskDisplayConfiguration config, 
                                       GanttChartTheme theme, double y)
        {
            var startX = CalculateTimePosition(task.StartDate, bounds);
            var endX = CalculateTimePosition(task.EndDate, bounds);
            var width = Math.Max(1, endX - startX);

            // Main task bar
            var taskBar = CreateTaskBar(task, startX, y, width, config.TaskHeight, theme);
            canvas.Children.Add(taskBar);

            // Progress overlay
            if (task.Progress > 0 && config.ShowProgress)
            {
                var progressBar = CreateProgressBar(task, startX, y, width, config.TaskHeight, theme);
                canvas.Children.Add(progressBar);
            }

            // Task label
            if (config.ShowLabels && !string.IsNullOrEmpty(task.Name))
            {
                var label = CreateTaskLabel(task, startX, y, width, config.TaskHeight, theme);
                canvas.Children.Add(label);
            }

            // Priority indicator
            if (config.ShowPriority && task.Priority != TaskPriority.Normal)
            {
                var indicator = CreatePriorityIndicator(task, startX, y, theme);
                canvas.Children.Add(indicator);
            }
        }

        private void RenderDependencyLines(Canvas canvas, List<EnhancedTaskItem> tasks, 
                                         RenderingBounds bounds, GanttChartTheme theme)
        {
            var dependencyGroup = new Canvas();
            Canvas.SetZIndex(dependencyGroup, 5);

            var taskPositions = BuildTaskPositionMap(tasks, bounds);

            foreach (var task in tasks)
            {
                foreach (var dependency in task.Dependencies)
                {
                    if (taskPositions.TryGetValue(dependency.PredecessorId, out var predPos) &&
                        taskPositions.TryGetValue(dependency.SuccessorId, out var succPos))
                    {
                        var line = CreateDependencyLine(predPos, succPos, dependency.Type, theme);
                        dependencyGroup.Children.Add(line);
                    }
                }
            }

            canvas.Children.Add(dependencyGroup);
        }

        #endregion

        #region UI Element Creation Methods

        private Rectangle CreateTaskBar(EnhancedTaskItem task, double x, double y, double width, 
                                      double height, GanttChartTheme theme)
        {
            var brush = GetTaskBrush(task, theme);
            
            var rect = new Rectangle
            {
                Width = width,
                Height = height - 2, // Leave margin
                Fill = brush,
                Stroke = GetCachedBrush(theme.Task.BorderColor),
                StrokeThickness = 1,
                RadiusX = theme.Task.CornerRadius,
                RadiusY = theme.Task.CornerRadius
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y + 1);

            // Add effects based on task status
            ApplyTaskEffects(rect, task, theme);

            return rect;
        }

        private Rectangle CreateProgressBar(EnhancedTaskItem task, double x, double y, double width, 
                                          double height, GanttChartTheme theme)
        {
            var progressWidth = width * (task.Progress / 100.0);
            
            var rect = new Rectangle
            {
                Width = progressWidth,
                Height = height - 2,
                Fill = GetProgressBrush(task, theme),
                RadiusX = theme.Task.CornerRadius,
                RadiusY = theme.Task.CornerRadius
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y + 1);

            return rect;
        }

        private TextBlock CreateTaskLabel(EnhancedTaskItem task, double x, double y, double width, 
                                        double height, GanttChartTheme theme)
        {
            var label = new TextBlock
            {
                Text = TruncateText(task.Name, width - 10, theme.Task.FontSize),
                FontSize = theme.Task.FontSize,
                FontFamily = theme.Task.FontFamily,
                Foreground = theme.Task.ForegroundBrush,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            Canvas.SetLeft(label, x + 5);
            Canvas.SetTop(label, y + (height - theme.Task.FontSize) / 2);

            return label;
        }

        private System.Windows.Shapes.Path CreateDependencyLine(TaskPosition from, TaskPosition to, 
                                        DependencyType type, GanttChartTheme theme)
        {
            var geometry = CreateDependencyGeometry(from, to, type);
            
            return new System.Windows.Shapes.Path
            {
                Data = geometry,
                Stroke = theme.Dependency.LineBrush,
                StrokeThickness = theme.Dependency.LineThickness,
                StrokeDashArray = theme.Dependency.DashPattern,
                Fill = null
            };
        }

        #endregion

        #region Performance Optimizations

        private SolidColorBrush GetCachedBrush(Color color)
        {
            if (!_brushCache.TryGetValue(color, out var brush))
            {
                brush = new SolidColorBrush(color);
                brush.Freeze(); // Important for performance
                _brushCache[color] = brush;
            }
            return brush;
        }

        private Line GetPooledLine()
        {
            if (_elementPool.Count > 0 && _elementPool.Dequeue() is Line line)
            {
                // Reset line properties
                line.StrokeDashArray = null;
                return line;
            }
            return new Line();
        }

        private void ReturnToPool(UIElement element)
        {
            if (_elementPool.Count < MaxPoolSize)
            {
                _elementPool.Enqueue(element);
            }
        }

        #endregion

        #region Utility Methods

        private RenderingBounds CalculateRenderingBounds(List<EnhancedTaskItem> tasks, GanttViewport viewport)
        {
            var startDate = tasks.Any() ? tasks.Min(t => t.StartDate) : viewport.StartDate;
            var endDate = tasks.Any() ? tasks.Max(t => t.EndDate) : viewport.EndDate;
            var timeSpan = endDate - startDate;
            var pixelsPerDay = viewport.TotalWidth / Math.Max(1, timeSpan.TotalDays);

            return new RenderingBounds
            {
                StartDate = startDate,
                EndDate = endDate,
                TimeSpan = timeSpan,
                TotalWidth = viewport.TotalWidth,
                TotalHeight = viewport.TotalHeight,
                PixelsPerDay = pixelsPerDay,
                TimeScaleHeight = 60,
                TimeScaleStartX = 200 // Space for task names
            };
        }

        private double CalculateTimePosition(DateTime date, RenderingBounds bounds)
        {
            var dayOffset = (date - bounds.StartDate).TotalDays;
            return bounds.TimeScaleStartX + (dayOffset * bounds.PixelsPerDay);
        }

        private bool IsTaskVisible(EnhancedTaskItem task, RenderingBounds bounds)
        {
            return task.EndDate >= bounds.StartDate && task.StartDate <= bounds.EndDate;
        }

        private Dictionary<int, TaskPosition> BuildTaskPositionMap(List<EnhancedTaskItem> tasks, RenderingBounds bounds)
        {
            var positions = new Dictionary<int, TaskPosition>();
            var y = bounds.TimeScaleHeight;

            foreach (var task in tasks)
            {
                var startX = CalculateTimePosition(task.StartDate, bounds);
                var endX = CalculateTimePosition(task.EndDate, bounds);
                
                positions[task.Id] = new TaskPosition
                {
                    StartX = startX,
                    EndX = endX,
                    CenterY = y + 20, // Half of typical task height
                    TaskId = task.Id
                };

                y += 40; // Task height + spacing
            }

            return positions;
        }

        public void ExportAsImage(Canvas canvas, string filePath)
        {
            var renderBitmap = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            renderBitmap.Render(canvas);

            var encoder = System.IO.Path.GetExtension(filePath).ToLower() switch
            {
                ".jpg" or ".jpeg" => (BitmapEncoder)new JpegBitmapEncoder(),
                ".bmp" => new BmpBitmapEncoder(),
                ".gif" => new GifBitmapEncoder(),
                ".tiff" => new TiffBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        // Missing method implementations
        private void RenderTimeScale(Canvas canvas, RenderingBounds bounds, 
                                  TimeScaleConfiguration config, GanttChartTheme theme)
        {
            // Simple time scale implementation
            var timeScaleGroup = new Canvas();
            Canvas.SetZIndex(timeScaleGroup, 100);
            
            var background = new Rectangle
            {
                Width = bounds.TotalWidth,
                Height = bounds.TimeScaleHeight,
                Fill = new SolidColorBrush(theme.TimeScale.BackgroundColor)
            };
            Canvas.SetLeft(background, 0);
            Canvas.SetTop(background, 0);
            timeScaleGroup.Children.Add(background);
            
            canvas.Children.Add(timeScaleGroup);
        }

        private void RenderMilestones(Canvas canvas, IEnumerable<EnhancedTaskItem> milestones, 
                                   RenderingBounds bounds, GanttChartTheme theme)
        {
            // Render milestone markers
            foreach (var milestone in milestones)
            {
                if (IsTaskVisible(milestone, bounds))
                {
                    var x = CalculateTimePosition(milestone.StartDate, bounds);
                    var milestoneShape = CreateMilestoneShape(x, bounds.TimeScaleHeight, theme);
                    canvas.Children.Add(milestoneShape);
                }
            }
        }

        private void RenderCriticalPath(Canvas canvas, IEnumerable<EnhancedTaskItem> criticalTasks, 
                                     RenderingBounds bounds, GanttChartTheme theme)
        {
            // Render critical path highlighting
            foreach (var task in criticalTasks)
            {
                if (IsTaskVisible(task, bounds))
                {
                    var startX = CalculateTimePosition(task.StartDate, bounds);
                    var endX = CalculateTimePosition(task.EndDate, bounds);
                    var width = Math.Max(1, endX - startX);
                    
                    var highlight = new Rectangle
                    {
                        Width = width,
                        Height = 40,
                        Stroke = new SolidColorBrush(theme.CriticalPath.HighlightColor),
                        StrokeThickness = theme.CriticalPath.HighlightThickness,
                        Fill = Brushes.Transparent
                    };
                    
                    Canvas.SetLeft(highlight, startX);
                    Canvas.SetTop(highlight, bounds.TimeScaleHeight);
                    canvas.Children.Add(highlight);
                }
            }
        }

        private TimeSpan CalculateTimeInterval(TimeSpan totalTimeSpan, double totalWidth)
        {
            var days = totalTimeSpan.TotalDays;
            
            if (days <= 7) return TimeSpan.FromDays(1);
            if (days <= 30) return TimeSpan.FromDays(7);
            if (days <= 365) return TimeSpan.FromDays(30);
            
            return TimeSpan.FromDays(365);
        }

        private List<EnhancedTaskItem> SortTasksForDisplay(List<EnhancedTaskItem> tasks, TaskSortBy sortBy)
        {
            return sortBy switch
            {
                TaskSortBy.Id => tasks.OrderBy(t => t.Id).ToList(),
                TaskSortBy.Name => tasks.OrderBy(t => t.Name).ToList(),
                TaskSortBy.StartDate => tasks.OrderBy(t => t.StartDate).ToList(),
                TaskSortBy.EndDate => tasks.OrderBy(t => t.EndDate).ToList(),
                TaskSortBy.Priority => tasks.OrderByDescending(t => t.Priority).ToList(),
                TaskSortBy.Progress => tasks.OrderByDescending(t => t.Progress).ToList(),
                _ => tasks
            };
        }

        private UIElement CreatePriorityIndicator(EnhancedTaskItem task, double x, double y, GanttChartTheme theme)
        {
            var indicator = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = new SolidColorBrush(theme.Task.PriorityColors.GetValueOrDefault(task.Priority, Colors.Gray))
            };
            
            Canvas.SetLeft(indicator, x - 12);
            Canvas.SetTop(indicator, y + 16);
            
            return indicator;
        }

        private Brush GetTaskBrush(EnhancedTaskItem task, GanttChartTheme theme)
        {
            if (theme.Task.StatusColors.TryGetValue(task.Status, out var statusColor))
            {
                return new SolidColorBrush(statusColor);
            }
            
            if (theme.Task.PriorityColors.TryGetValue(task.Priority, out var priorityColor))
            {
                return new SolidColorBrush(priorityColor);
            }
            
            return new SolidColorBrush(theme.Task.NormalColor);
        }

        private void ApplyTaskEffects(Rectangle taskBar, EnhancedTaskItem task, GanttChartTheme theme)
        {
            if (theme.Task.ShadowEnabled)
            {
                // Add drop shadow effect
                taskBar.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 3,
                    ShadowDepth = 2,
                    Opacity = 0.3
                };
            }
        }

        private Brush GetProgressBrush(EnhancedTaskItem task, GanttChartTheme theme)
        {
            return new SolidColorBrush(theme.Progress.FillColor);
        }

        private string TruncateText(string text, double availableWidth, double fontSize)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var characterWidth = fontSize * 0.6; // Approximate character width
            var maxCharacters = (int)(availableWidth / characterWidth);
            
            if (text.Length <= maxCharacters) return text;
            
            return text.Substring(0, Math.Max(0, maxCharacters - 3)) + "...";
        }

        private Geometry CreateDependencyGeometry(TaskPosition from, TaskPosition to, DependencyType type)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = new Point(from.EndX, from.CenterY) };
            
            // Simple line from end of predecessor to start of successor
            figure.Segments.Add(new LineSegment(new Point(to.StartX, to.CenterY), true));
            
            geometry.Figures.Add(figure);
            return geometry;
        }

        private UIElement CreateMilestoneShape(double x, double y, GanttChartTheme theme)
        {
            var diamond = new Polygon
            {
                Fill = new SolidColorBrush(theme.Milestone.Color),
                Stroke = new SolidColorBrush(theme.Milestone.BorderColor),
                StrokeThickness = 2
            };
            
            var size = theme.Milestone.Size;
            diamond.Points = new PointCollection
            {
                new Point(0, size / 2),      // Left
                new Point(size / 2, 0),      // Top
                new Point(size, size / 2),   // Right
                new Point(size / 2, size)    // Bottom
            };
            
            Canvas.SetLeft(diamond, x - size / 2);
            Canvas.SetTop(diamond, y + 10);
            
            return diamond;
        }

        #endregion

        #region Helper Classes

        private class RenderingBounds
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public double TotalWidth { get; set; }
            public double TotalHeight { get; set; }
            public double PixelsPerDay { get; set; }
            public double TimeScaleHeight { get; set; }
            public double TimeScaleStartX { get; set; }
        }

        private class TaskPosition
        {
            public double StartX { get; set; }
            public double EndX { get; set; }
            public double CenterY { get; set; }
            public int TaskId { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// Configuration classes for improved architecture
    /// </summary>
    public class GridConfiguration
    {
        public bool ShowVerticalLines { get; set; } = true;
        public bool ShowHorizontalLines { get; set; } = true;
        public double LineThickness { get; set; } = 1.0;
        public DoubleCollection DashPattern { get; set; }
        public double TaskHeight { get; set; } = 40;

        public static GridConfiguration CreateDefault() => new();
    }

    public class TaskDisplayConfiguration
    {
        public bool ShowLabels { get; set; } = true;
        public bool ShowProgress { get; set; } = true;
        public bool ShowPriority { get; set; } = true;
        public double TaskHeight { get; set; } = 40;
        public double TaskSpacing { get; set; } = 5;
        public double TaskMargin { get; set; } = 5;
        public TaskSortBy SortBy { get; set; } = TaskSortBy.StartDate;

        public static TaskDisplayConfiguration CreateDefault() => new();
    }

    public class InteractionConfiguration
    {
        public bool EnableDragDrop { get; set; } = true;
        public bool EnableResize { get; set; } = true;
        public bool EnableSelection { get; set; } = true;
        public bool EnableTooltips { get; set; } = true;

        public static InteractionConfiguration CreateDefault() => new();
    }

    public enum TaskSortBy
    {
        Id,
        Name,
        StartDate,
        EndDate,
        Priority,
        Progress
    }
}
