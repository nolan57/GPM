# GanttChartFramework Usage Guide

## Quick Start Guide

### 1. Basic Setup

#### Step 1: Install the Package
```xml
<!-- Add to your .csproj file -->
<PackageReference Include="GanttChartFramework" Version="1.0.0" />
```

#### Step 2: Add Namespace
```xml
xmlns:views="clr-namespace:GanttChartFramework.Views;assembly=GanttChartFramework"
```

#### Step 3: Basic XAML Declaration
```xml
<views:GanttChartContainer 
    x:Name="MainGanttChart"
    Width="800"
    Height="600" />
```

### 2. Creating Your First Gantt Chart

#### Complete Example - Code Behind
```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                Name = "Project Kickoff",
                StartDay = DateTime.Today,
                Duration = 2,
                Progress = 100,
                Color = Brushes.ForestGreen
            },
            new TaskItem
            {
                Id = 2,
                Name = "Requirement Analysis",
                StartDay = DateTime.Today.AddDays(3),
                Duration = 5,
                Progress = 60,
                Color = Brushes.SteelBlue
            },
            new TaskItem
            {
                Id = 3,
                Name = "Design Phase",
                StartDay = DateTime.Today.AddDays(9),
                Duration = 7,
                Progress = 30,
                ParentId = 2,
                Color = Brushes.DarkOrange
            }
        };

        MainGanttChart.Tasks = tasks;
        MainGanttChart.TimeScaleType = "Week";
    }
}
```

#### Complete Example - MVVM
```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:views="clr-namespace:GanttChartFramework.Views;assembly=GanttChartFramework">
    
    <Grid>
        <views:ImprovedGanttChartContainer 
            Tasks="{Binding ProjectTasks}"
            Configuration="{Binding ChartConfiguration}"
            Theme="{Binding SelectedTheme}" />
    </Grid>
</Window>
```

```csharp
// ViewModel
public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<TaskItem> ProjectTasks { get; set; }
    public GanttChartConfiguration ChartConfiguration { get; set; }
    public GanttChartTheme SelectedTheme { get; set; }

    public MainViewModel()
    {
        LoadProjectData();
        SetupConfiguration();
    }

    private void LoadProjectData()
    {
        ProjectTasks = new ObservableCollection<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                Name = "Software Development Project",
                StartDay = DateTime.Today,
                Duration = 90,
                Progress = 25,
                Color = new SolidColorBrush(Color.FromRgb(52, 152, 219))
            }
        };
    }

    private void SetupConfiguration()
    {
        ChartConfiguration = new GanttChartConfiguration
        {
            TimeScale = new TimeScaleConfiguration
            {
                DefaultScale = TimeLevelType.Week,
                ShowWeekends = true,
                ShowToday = true
            },
            TaskDisplay = new TaskDisplayConfiguration
            {
                TaskHeight = 35,
                ShowProgress = true,
                ShowLabels = true
            }
        };

        SelectedTheme = GanttChartTheme.CreateModern();
    }
}
```

## Advanced Usage Examples

### 1. Custom Task Styling

```csharp
public class StyledTask : TaskItem
{
    public Brush ProgressBrush { get; set; }
    public Brush HoverBrush { get; set; }
    public double CornerRadius { get; set; }

    public StyledTask()
    {
        ProgressBrush = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        HoverBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
        CornerRadius = 3;
    }
}

// Usage
var styledTasks = new List<StyledTask>
{
    new StyledTask
    {
        Id = 1,
        Name = "Custom Styled Task",
        StartDay = DateTime.Today,
        Duration = 7,
        Progress = 45,
        Color = new SolidColorBrush(Color.FromRgb(155, 89, 182))
    }
};
```

### 2. Interactive Task Management

```csharp
public class InteractiveGanttManager
{
    private ImprovedGanttChartContainer _ganttChart;

    public InteractiveGanttManager(ImprovedGanttChartContainer ganttChart)
    {
        _ganttChart = ganttChart;
        SetupInteractions();
    }

    private void SetupInteractions()
    {
        // Handle task selection
        _ganttChart.SelectedTasks.CollectionChanged += (s, e) =>
        {
            if (e.NewItems?.Count > 0)
            {
                var selectedTask = e.NewItems[0] as EnhancedTaskItem;
                ShowTaskDetails(selectedTask);
            }
        };

        // Handle task updates
        _ganttChart.TaskUpdated += (s, task) =>
        {
            SaveTaskChanges(task);
            RefreshChart();
        };
    }

    public void AddTask(string name, DateTime start, int duration)
    {
        var newTask = new EnhancedTaskItem
        {
            Id = GenerateTaskId(),
            Name = name,
            StartDate = start,
            EndDate = start.AddDays(duration),
            Progress = 0,
            Status = TaskStatus.NotStarted
        };

        _ganttChart.Tasks.Add(newTask);
    }

    public void UpdateTaskProgress(int taskId, double progress)
    {
        var task = _ganttChart.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            task.Progress = progress;
            _ganttChart.Refresh();
        }
    }

    private void ShowTaskDetails(EnhancedTaskItem task)
    {
        // Show task details in a dialog or panel
        var dialog = new TaskDetailsDialog(task);
        dialog.ShowDialog();
    }

    private void SaveTaskChanges(EnhancedTaskItem task)
    {
        // Save to database or service
        _repository.UpdateTask(task);
    }
}
```

### 3. Dynamic Time Scale Switching

```csharp
public class TimeScaleManager
{
    private ImprovedGanttChartContainer _ganttChart;

    public TimeScaleManager(ImprovedGanttChartContainer ganttChart)
    {
        _ganttChart = ganttChart;
    }

    public void SwitchToDailyView()
    {
        _ganttChart.Configuration.TimeScale.DefaultScale = TimeLevelType.Day;
        _ganttChart.Viewport.SetDateRange(DateTime.Today, DateTime.Today.AddDays(30));
        _ganttChart.Refresh();
    }

    public void SwitchToWeeklyView()
    {
        _ganttChart.Configuration.TimeScale.DefaultScale = TimeLevelType.Week;
        _ganttChart.Viewport.SetDateRange(DateTime.Today, DateTime.Today.AddDays(90));
        _ganttChart.Refresh();
    }

    public void SwitchToMonthlyView()
    {
        _ganttChart.Configuration.TimeScale.DefaultScale = TimeLevelType.Month;
        _ganttChart.Viewport.SetDateRange(DateTime.Today, DateTime.Today.AddDays(365));
        _ganttChart.Refresh();
    }

    public void ZoomToTask(int taskId)
    {
        var task = _ganttChart.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            _ganttChart.Viewport.SetDateRange(
                task.StartDate.AddDays(-2),
                task.EndDate.AddDays(2)
            );
            _ganttChart.ScrollToTask(taskId);
        }
    }
}
```

### 4. Export Functionality

```csharp
public class GanttExporter
{
    private readonly GanttRenderingService _renderer;

    public GanttExporter()
    {
        _renderer = new GanttRenderingService();
    }

    public async Task ExportToPngAsync(ImprovedGanttChartContainer gantt, string filePath)
    {
        await Task.Run(() =>
        {
            _renderer.ExportAsImage(gantt.GetCanvas(), filePath);
        });
    }

    public async Task ExportToPdfAsync(ImprovedGanttChartContainer gantt, string filePath)
    {
        var bitmap = _renderer.RenderToBitmap(gantt.GetCanvas());
        
        using (var document = new PdfDocument())
        {
            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);
            
            using (var image = XImage.FromBitmapSource(bitmap))
            {
                graphics.DrawImage(image, 0, 0);
            }
            
            document.Save(filePath);
        }
    }

    public string GenerateReportHtml(ImprovedGanttChartContainer gantt)
    {
        var tasks = gantt.Tasks;
        var html = new StringBuilder();
        
        html.AppendLine("<html><body>");
        html.AppendLine("<h1>Project Gantt Report</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Task</th><th>Start</th><th>Duration</th><th>Progress</th></tr>");
        
        foreach (var task in tasks)
        {
            html.AppendLine($"<tr>" +
                $"<td>{task.Name}</td>" +
                $"<td>{task.StartDay:yyyy-MM-dd}</td>" +
                $"<td>{task.Duration} days</td>" +
                $"<td>{task.Progress}%</td>" +
                $"</tr>");
        }
        
        html.AppendLine("</table></body></html>");
        return html.ToString();
    }
}
```

### 5. Data Binding Examples

#### Binding with Entity Framework
```csharp
public class GanttDataService
{
    private readonly YourDbContext _context;

    public async Task<ObservableCollection<TaskItem>> LoadProjectTasksAsync(int projectId)
    {
        var dbTasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return new ObservableCollection<TaskItem>(
            dbTasks.Select(t => new TaskItem
            {
                Id = t.Id,
                Name = t.Name,
                StartDay = t.StartDate,
                Duration = (int)(t.EndDate - t.StartDate).TotalDays,
                Progress = t.ProgressPercentage,
                ParentId = t.ParentTaskId,
                Color = GetTaskColor(t.Status)
            })
        );
    }

    public async Task SaveTaskChangesAsync(TaskItem task)
    {
        var dbTask = await _context.Tasks.FindAsync(task.Id);
        if (dbTask != null)
        {
            dbTask.StartDate = task.StartDay;
            dbTask.EndDate = task.StartDay.AddDays(task.Duration);
            dbTask.ProgressPercentage = task.Progress;
            await _context.SaveChangesAsync();
        }
    }
}
```

#### Real-time Updates
```csharp
public class RealTimeGanttManager
{
    private readonly HubConnection _hubConnection;
    private readonly ImprovedGanttChartContainer _ganttChart;

    public RealTimeGanttManager(ImprovedGanttChartContainer ganttChart)
    {
        _ganttChart = ganttChart;
        SetupSignalR();
    }

    private void SetupSignalR()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/ganttHub")
            .Build();

        _hubConnection.On<TaskItem>("TaskUpdated", task =>
        {
            UpdateTaskInGantt(task);
        });

        _hubConnection.On<TaskItem>("TaskAdded", task =>
        {
            _ganttChart.Tasks.Add(task);
        });

        _hubConnection.StartAsync();
    }

    private void UpdateTaskInGantt(TaskItem updatedTask)
    {
        var existingTask = _ganttChart.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
        if (existingTask != null)
        {
            existingTask.Name = updatedTask.Name;
            existingTask.StartDay = updatedTask.StartDay;
            existingTask.Duration = updatedTask.Duration;
            existingTask.Progress = updatedTask.Progress;
            
            _ganttChart.Refresh();
        }
    }
}
```

## Common Patterns and Best Practices

### 1. MVVM Pattern Implementation

```csharp
public class GanttChartViewModel : ViewModelBase
{
    private ObservableCollection<TaskItem> _tasks;
    private GanttChartConfiguration _configuration;
    private GanttChartTheme _theme;

    public ObservableCollection<TaskItem> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    public GanttChartConfiguration Configuration
    {
        get => _configuration;
        set => SetProperty(ref _configuration, value);
    }

    public GanttChartTheme Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public ICommand AddTaskCommand { get; }
    public ICommand RemoveTaskCommand { get; }
    public ICommand ExportCommand { get; }

    public GanttChartViewModel()
    {
        AddTaskCommand = new RelayCommand(AddTask);
        RemoveTaskCommand = new RelayCommand<TaskItem>(RemoveTask);
        ExportCommand = new RelayCommand(ExportGantt);
        
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        Configuration = GanttChartConfiguration.CreateDefault();
        Theme = GanttChartTheme.CreateModern();
        Tasks = new ObservableCollection<TaskItem>();
    }
}
```

### 2. Performance Optimization

```csharp
public class OptimizedGanttManager
{
    private readonly ImprovedGanttChartContainer _ganttChart;
    private readonly DispatcherTimer _updateTimer;

    public OptimizedGanttManager(ImprovedGanttChartContainer ganttChart)
    {
        _ganttChart = ganttChart;
        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _updateTimer.Tick += (s, e) => _ganttChart.Refresh();
    }

    public void BatchUpdateTasks(IEnumerable<TaskItem> updates)
    {
        _updateTimer.Stop();
        
        using (_ganttChart.Tasks.DeferRefresh())
        {
            foreach (var update in updates)
            {
                var task = _ganttChart.Tasks.FirstOrDefault(t => t.Id == update.Id);
                if (task != null)
                {
                    task.Progress = update.Progress;
                    task.StartDay = update.StartDay;
                    task.Duration = update.Duration;
                }
            }
        }
        
        _updateTimer.Start();
    }
}
```

## Troubleshooting

### Common Issues and Solutions

#### 1. Tasks Not Displaying
```csharp
// Check if tasks have valid dates and durations
foreach (var task in tasks)
{
    if (task.StartDay == default(DateTime))
        task.StartDay = DateTime.Today;
    
    if (task.Duration <= 0)
        task.Duration = 1;
}

// Ensure tasks collection is not null
if (ganttChart.Tasks == null)
    ganttChart.Tasks = new List<TaskItem>();
```

#### 2. Performance Issues with Large Datasets
```csharp
// Enable virtualization
container.Configuration.EnableVirtualization = true;

// Set reasonable viewport limits
container.Viewport.SetDateRange(
    DateTime.Today.AddDays(-30),
    DateTime.Today.AddDays(90)
);

// Use async loading
await LoadTasksAsync();
```

#### 3. Theme Not Applying
```csharp
// Ensure theme is set after initialization
container.Loaded += (s, e) =>
{
    container.Theme = GanttChartTheme.CreateModern();
    container.ApplyTheme();
};
```

## Sample Applications

### 1. Project Management Dashboard
```xml
<Window ...>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        
        <!-- Toolbar -->
        <ToolBarTray Grid.Row="0">
            <ToolBar>
                <Button Content="Add Task" Command="{Binding AddTaskCommand}"/>
                <Button Content="Export" Command="{Binding ExportCommand}"/>
                <Separator/>
                <ComboBox ItemsSource="{Binding TimeScaleOptions}"
                         SelectedItem="{Binding SelectedTimeScale}"/>
            </ToolBar>
        </ToolBarTray>
        
        <!-- Gantt Chart -->
        <views:ImprovedGanttChartContainer 
            Grid.Row="1"
            Tasks="{Binding ProjectTasks}"
            Configuration="{Binding ChartConfiguration}"
            Theme="{Binding SelectedTheme}"/>
    </Grid>
</Window>
```

This comprehensive usage guide provides everything needed to effectively use the GanttChartFramework in your applications, from basic setup to advanced scenarios and performance optimization.