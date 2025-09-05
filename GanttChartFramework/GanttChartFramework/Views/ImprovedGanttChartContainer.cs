using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GanttChartFramework.Models;
using GanttChartFramework.Services;
using GanttChartFramework.Themes;

namespace GanttChartFramework.Views
{
    /// <summary>
    /// Improved Gantt chart container with modern architecture and performance optimizations
    /// </summary>
    public class ImprovedGanttChartContainer : UserControl, INotifyPropertyChanged
    {
        #region Core Dependency Properties

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(nameof(Configuration), typeof(GanttChartConfiguration), 
                typeof(ImprovedGanttChartContainer),
                new PropertyMetadata(null, OnConfigurationChanged));

        public static readonly DependencyProperty TasksProperty =
            DependencyProperty.Register(nameof(Tasks), typeof(ObservableCollection<TaskItem>), 
                typeof(ImprovedGanttChartContainer),
                new PropertyMetadata(new ObservableCollection<TaskItem>(), OnTasksChanged));

        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(nameof(Viewport), typeof(GanttViewport), 
                typeof(ImprovedGanttChartContainer),
                new PropertyMetadata(new GanttViewport(), OnViewportChanged));

        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register(nameof(Theme), typeof(GanttChartTheme), 
                typeof(ImprovedGanttChartContainer),
                new PropertyMetadata(GanttChartTheme.CreateDefault(), OnThemeChanged));

        #endregion

        #region Properties

        public GanttChartConfiguration Configuration
        {
            get => (GanttChartConfiguration)GetValue(ConfigurationProperty);
            set => SetValue(ConfigurationProperty, value);
        }

        public ObservableCollection<TaskItem> Tasks
        {
            get => (ObservableCollection<TaskItem>)GetValue(TasksProperty);
            set => SetValue(TasksProperty, value);
        }

        public GanttViewport Viewport
        {
            get => (GanttViewport)GetValue(ViewportProperty);
            set => SetValue(ViewportProperty, value);
        }

        public GanttChartTheme Theme
        {
            get => (GanttChartTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        #endregion

        #region Private Fields

        private readonly GanttRenderingService _renderingService;
        private readonly GanttInteractionService _interactionService;
        private Grid _mainGrid;
        private ScrollViewer _scrollViewer;
        private Canvas _drawingCanvas;
        private TimeScalePanel _timeScalePanel;
        private TaskListPanel _taskListPanel;

        #endregion

        #region Constructor

        public ImprovedGanttChartContainer()
        {
            _renderingService = new GanttRenderingService();
            _interactionService = new GanttInteractionService();
            
            Configuration = GanttChartConfiguration.CreateDefault();
            Tasks = new ObservableCollection<TaskItem>();
            Viewport = new GanttViewport();
            Theme = GanttChartTheme.CreateDefault();

            InitializeComponent();
            SetupEventHandlers();
        }

        #endregion

        #region Initialization

        private void InitializeComponent()
        {
            _mainGrid = new Grid();
            _mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Time scale header
            _timeScalePanel = new TimeScalePanel();
            Grid.SetRow(_timeScalePanel, 0);
            Grid.SetColumn(_timeScalePanel, 1);
            _mainGrid.Children.Add(_timeScalePanel);

            // Task list panel
            _taskListPanel = new TaskListPanel();
            Grid.SetRow(_taskListPanel, 1);
            Grid.SetColumn(_taskListPanel, 0);
            _mainGrid.Children.Add(_taskListPanel);

            // Main drawing area with scroll viewer
            _scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                CanContentScroll = true
            };

            _drawingCanvas = new Canvas();
            _scrollViewer.Content = _drawingCanvas;
            Grid.SetRow(_scrollViewer, 1);
            Grid.SetColumn(_scrollViewer, 1);
            _mainGrid.Children.Add(_scrollViewer);

            Content = _mainGrid;
        }

        private void SetupEventHandlers()
        {
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            
            if (Tasks != null)
            {
                Tasks.CollectionChanged += OnTasksCollectionChanged;
            }
        }

        #endregion

        #region Event Handlers

        private static void OnConfigurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedGanttChartContainer container)
            {
                container.ApplyConfiguration();
            }
        }

        private static void OnTasksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedGanttChartContainer container)
            {
                container.HandleTasksChanged(e.OldValue as ObservableCollection<TaskItem>, 
                                          e.NewValue as ObservableCollection<TaskItem>);
            }
        }

        private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedGanttChartContainer container)
            {
                container.UpdateViewport();
            }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImprovedGanttChartContainer container)
            {
                container.ApplyTheme();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitialRender();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateViewport();
            RequestRender();
        }

        private void OnTasksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RequestRender();
        }

        #endregion

        #region Core Methods

        private void HandleTasksChanged(ObservableCollection<TaskItem> oldTasks, ObservableCollection<TaskItem> newTasks)
        {
            if (oldTasks != null)
            {
                oldTasks.CollectionChanged -= OnTasksCollectionChanged;
            }

            if (newTasks != null)
            {
                newTasks.CollectionChanged += OnTasksCollectionChanged;
            }

            RequestRender();
        }

        private void ApplyConfiguration()
        {
            if (Configuration == null) return;

            // Update time scale
            _timeScalePanel?.ApplyConfiguration(Configuration.TimeScale);
            
            // Update task display
            _taskListPanel?.ApplyConfiguration(Configuration.TaskDisplay);
            
            RequestRender();
        }

        private void UpdateViewport()
        {
            if (Viewport == null) return;

            // Update canvas size based on viewport
            _drawingCanvas.Width = Viewport.TotalWidth;
            _drawingCanvas.Height = Viewport.TotalHeight;

            // Update scroll position if needed
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHorizontalOffset(Viewport.HorizontalOffset);
                _scrollViewer.ScrollToVerticalOffset(Viewport.VerticalOffset);
            }
        }

        private void ApplyTheme()
        {
            if (Theme == null) return;

            Background = Theme.BackgroundBrush;
            _timeScalePanel?.ApplyTheme(ConvertToTimeScaleTheme(Theme.TimeScale));
            _taskListPanel?.ApplyTheme(Theme.TaskList);

            RequestRender();
        }

        private TimeScaleTheme ConvertToTimeScaleTheme(GanttTimeScaleTheme ganttTheme)
        {
            return new TimeScaleTheme
            {
                PrimaryColor = new SolidColorBrush(ganttTheme.ForegroundColor),
                SecondaryColor = new SolidColorBrush(ganttTheme.AccentColor),
                AccentColor = new SolidColorBrush(ganttTheme.BackgroundColor),
                BaseFontSize = ganttTheme.FontSizes?.Values.FirstOrDefault() ?? 12.0,
                FontFamily = ganttTheme.FontFamily
            };
        }

        private void InitialRender()
        {
            ApplyConfiguration();
            ApplyTheme();
            RequestRender();
        }

        private void RequestRender()
        {
            // Use dispatcher to batch render requests
            Dispatcher.BeginInvoke(new Action(PerformRender), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void PerformRender()
        {
            _renderingService.RenderGanttChart(
                _drawingCanvas,
                ConvertTasksToEnhancedTasks(Tasks?.ToList() ?? new System.Collections.Generic.List<TaskItem>()),
                Configuration,
                Viewport,
                Theme
            );
        }

        private List<EnhancedTaskItem> ConvertTasksToEnhancedTasks(List<TaskItem> tasks)
        {
            return tasks.Select(t => new EnhancedTaskItem
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDay,
                EndDate = t.StartDay.AddDays(t.Duration),
                Progress = t.Progress,
                ParentId = t.ParentId,
                Priority = TaskPriority.Normal,
                Status = Models.TaskStatus.NotStarted,
                Color = ConvertBrushToColor(t.Color) ?? Color.FromRgb(52, 152, 219) // Default blue
            }).ToList();
        }

        private Color? ConvertBrushToColor(Brush brush)
        {
            if (brush is SolidColorBrush solidBrush)
            {
                return solidBrush.Color;
            }
            return null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Zooms the chart to fit all tasks
        /// </summary>
        public void ZoomToFit()
        {
            if (Tasks == null || !Tasks.Any()) return;

            var startDate = Tasks.Min(t => t.StartDay);
            var endDate = Tasks.Max(t => t.StartDay.AddDays(t.Duration));
            
            Viewport.SetDateRange(startDate, endDate);
            RequestRender();
        }

        /// <summary>
        /// Scrolls to a specific task
        /// </summary>
        public void ScrollToTask(int taskId)
        {
            var task = Tasks?.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return;

            var taskIndex = Tasks.IndexOf(task);
            var y = taskIndex * Configuration.TaskDisplay.TaskHeight;
            
            _scrollViewer?.ScrollToVerticalOffset(y);
        }

        /// <summary>
        /// Exports the chart as an image
        /// </summary>
        public void ExportAsImage(string filePath)
        {
            _renderingService.ExportAsImage(_drawingCanvas, filePath);
        }

        /// <summary>
        /// Refreshes the entire chart
        /// </summary>
        public void Refresh()
        {
            RequestRender();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Configuration for the Gantt chart
    /// </summary>
    public class GanttChartConfiguration
    {
        public TimeScaleConfiguration TimeScale { get; set; } = new();
        public TaskDisplayConfiguration TaskDisplay { get; set; } = new();
        public GridConfiguration Grid { get; set; } = new();
        public InteractionConfiguration Interaction { get; set; } = new();

        public static GanttChartConfiguration CreateDefault() => new()
        {
            TimeScale = TimeScaleConfiguration.CreateDefault(),
            TaskDisplay = TaskDisplayConfiguration.CreateDefault(),
            Grid = GridConfiguration.CreateDefault(),
            Interaction = InteractionConfiguration.CreateDefault()
        };
    }

    /// <summary>
    /// Viewport information for the Gantt chart
    /// </summary>
    public class GanttViewport : INotifyPropertyChanged
    {
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(30);
        private double _totalWidth = 2000;
        private double _totalHeight = 1000;

        public DateTime StartDate 
        { 
            get => _startDate; 
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); } 
        }

        public DateTime EndDate 
        { 
            get => _endDate; 
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); } 
        }

        public double TotalWidth 
        { 
            get => _totalWidth; 
            set { _totalWidth = value; OnPropertyChanged(nameof(TotalWidth)); } 
        }

        public double TotalHeight 
        { 
            get => _totalHeight; 
            set { _totalHeight = value; OnPropertyChanged(nameof(TotalHeight)); } 
        }

        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }
        public TimeSpan VisibleTimeSpan => EndDate - StartDate;

        public void SetDateRange(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
