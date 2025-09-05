# GanttChartFramework API Documentation

## Overview

The GanttChartFramework is a comprehensive WPF library for creating interactive Gantt charts with modern UI, high performance, and extensive customization capabilities.

## Core Components

### 1. GanttChartContainer (Main Control)

The primary control that hosts the entire Gantt chart visualization.

#### XAML Usage
```xml
<views:GanttChartContainer 
    x:Name="GanttChart"
    Tasks="{Binding ProjectTasks}"
    TimeScaleType="Week"
    TaskHeight="40"
    ShowTaskLabels="True"
    IsHierarchical="True" />
```

#### Key Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `Tasks` | `List<TaskItem>` | Collection of tasks to display | `new List<TaskItem>()` |
| `TimeScaleType` | `string` | Time scale granularity ("Day", "Week", "Month", "Quarter") | `"Day"` |
| `TaskHeight` | `double` | Height of individual task bars | `40.0` |
| `ShowTaskLabels` | `bool` | Display task names on bars | `true` |
| `IsHierarchical` | `bool` | Enable parent/child task relationships | `true` |
| `CanvasWidth` | `double` | Total canvas width in pixels | `2400.0` |
| `CanvasHeight` | `double` | Total canvas height in pixels | `1800.0` |

#### Methods

```csharp
// Refresh the entire chart
GanttChart.RefreshGanttChart();

// Change time scale programmatically
GanttChart.TimeScaleType = "Month";

// Update tasks
GanttChart.Tasks = new List<TaskItem> { new TaskItem { ... } };
```

### 2. TaskItem (Data Model)

Represents a single task in the Gantt chart.

```csharp
public class TaskItem
{
    public int Id { get; set; }           // Unique identifier
    public string Name { get; set; }      // Display name
    public DateTime StartDay { get; set; } // Start date
    public int Duration { get; set; }     // Duration in days
    public double Progress { get; set; }  // Progress percentage (0-100)
    public int? ParentId { get; set; }    // Parent task ID for hierarchy
    public Brush Color { get; set; }      // Task bar color
}
```

#### Example Task Creation
```csharp
var tasks = new List<TaskItem>
{
    new TaskItem
    {
        Id = 1,
        Name = "Project Planning",
        StartDay = DateTime.Today,
        Duration = 5,
        Progress = 75,
        Color = Brushes.SteelBlue
    },
    new TaskItem
    {
        Id = 2,
        Name = "Development",
        StartDay = DateTime.Today.AddDays(6),
        Duration = 10,
        Progress = 30,
        ParentId = 1,
        Color = Brushes.ForestGreen
    }
};
```

### 3. EnhancedTaskItem (Extended Model)

Advanced task model with additional properties for the improved container.

```csharp
public class EnhancedTaskItem : TaskItem
{
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public List<int> Dependencies { get; set; }
    public string AssignedTo { get; set; }
    public string Description { get; set; }
}
```

### 4. ImprovedGanttChartContainer (Advanced Control)

Modern MVVM-compatible control with enhanced features.

#### Properties

```csharp
// Configuration
public GanttChartConfiguration Configuration { get; set; }
public GanttChartTheme Theme { get; set; }
public GanttViewport Viewport { get; set; }

// Data
public ObservableCollection<TaskItem> Tasks { get; set; }
```

#### Configuration Example
```csharp
var container = new ImprovedGanttChartContainer
{
    Configuration = GanttChartConfiguration.CreateDefault(),
    Theme = GanttChartTheme.CreateModern(),
    Tasks = new ObservableCollection<TaskItem>(tasks)
};

// Customize configuration
container.Configuration.TimeScale.DefaultScale = TimeLevelType.Week;
container.Configuration.TaskDisplay.TaskHeight = 45;
container.Configuration.Grid.ShowVerticalLines = true;
```

### 5. GanttChartTheme (Styling System)

Comprehensive theming system with predefined themes.

#### Available Themes
```csharp
// Predefined themes
var defaultTheme = GanttChartTheme.CreateDefault();
var darkTheme = GanttChartTheme.CreateDarkTheme();
var lightTheme = GanttChartTheme.CreateLightTheme();
var modernTheme = GanttChartTheme.CreateModernTheme();

// Custom theme
var customTheme = new GanttChartTheme
{
    BackgroundBrush = Brushes.White,
    GridLineBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
    TaskBarBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219))
};
```

## Services

### 1. GanttRenderingService

Handles all rendering operations for optimal performance.

```csharp
var renderer = new GanttRenderingService();

// Export as image
renderer.ExportAsImage(canvas, "gantt-chart.png");

// Custom rendering
renderer.RenderGanttChart(canvas, enhancedTasks, config, viewport, theme);
```

### 2. GanttInteractionService

Manages user interactions and mouse events.

#### Features
- Task selection and multi-selection
- Drag and drop operations
- Resize handles
- Context menus
- Tooltips

### 3. TimeScaleRenderingService

Specialized service for time axis rendering.

```csharp
var timeScaleService = new TimeScaleRenderingService();

// Configure time scale
var config = new TimeScaleConfiguration
{
    DefaultScale = TimeLevelType.Week,
    ShowWeekends = true,
    MinimumColumnWidth = 40
};
```

## Configuration System

### TimeScaleConfiguration

```csharp
var timeConfig = new TimeScaleConfiguration
{
    DefaultScale = TimeLevelType.Day,
    ShowWeekends = true,
    ShowToday = true,
    MinimumColumnWidth = 30,
    MaximumColumnWidth = 200,
    EnableAutoScale = true
};
```

### TaskDisplayConfiguration

```csharp
var taskConfig = new TaskDisplayConfiguration
{
    TaskHeight = 40,
    ShowProgress = true,
    ShowLabels = true,
    LabelFontSize = 12,
    EnableAnimations = true
};
```

### GridConfiguration

```csharp
var gridConfig = new GridConfiguration
{
    ShowVerticalLines = true,
    ShowHorizontalLines = true,
    LineColor = Color.FromRgb(200, 200, 200),
    LineThickness = 1.0,
    LineDashArray = new DoubleCollection { 2, 2 }
};
```

## Events and Callbacks

### Task Events
```csharp
// Task selection changed
ganttChart.TaskSelected += (sender, task) => 
{
    Console.WriteLine($"Selected: {task.Name}");
};

// Task progress updated
ganttChart.TaskProgressChanged += (sender, task) =>
{
    Console.WriteLine($"Progress: {task.Name} - {task.Progress}%");
};
```

### Interaction Events
```csharp
// Drag operations
container.DragStarted += (sender, args) => { ... };
container.DragCompleted += (sender, args) => { ... };

// Resize operations
container.TaskResized += (sender, args) => { ... };
```

## Advanced Features

### 1. Zoom and Pan

```csharp
// Zoom to fit all tasks
container.ZoomToFit();

// Scroll to specific task
container.ScrollToTask(taskId);

// Set viewport range
container.Viewport.SetDateRange(startDate, endDate);
```

### 2. Custom Task Rendering

```csharp
// Override task rendering
public class CustomTaskRenderer : GanttRenderingService
{
    protected override void DrawTaskBar(DrawingContext dc, EnhancedTaskItem task, Rect bounds)
    {
        // Custom drawing logic
        var brush = GetCustomBrush(task);
        dc.DrawRectangle(brush, null, bounds);
    }
}
```

### 3. Theme Customization

```csharp
// Create custom theme
var myTheme = new GanttChartTheme
{
    BackgroundBrush = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
    GridLineBrush = new SolidColorBrush(Color.FromRgb(233, 236, 239)),
    TaskBarBrush = new SolidColorBrush(Color.FromRgb(0, 123, 255)),
    CompletedTaskBrush = new SolidColorBrush(Color.FromRgb(40, 167, 69))
};

// Apply theme
container.Theme = myTheme;
```

### 4. Localization

```csharp
// Localized date formats
container.Configuration.TimeScale.DateFormat = "yyyy-MM-dd";
container.Configuration.TimeScale.Culture = new CultureInfo("en-US");
```

## Performance Optimization

### Best Practices

1. **Batch Updates**: Use `ObservableCollection` for efficient UI updates
2. **Virtualization**: Enable virtualization for large datasets
3. **Caching**: Leverage built-in caching mechanisms
4. **Async Rendering**: Use async methods for complex operations

```csharp
// Efficient batch updates
var tasks = new ObservableCollection<TaskItem>();
container.Tasks = tasks;

// Add multiple tasks efficiently
using (tasks.DeferRefresh())
{
    foreach (var task in newTasks)
    {
        tasks.Add(task);
    }
}
```

## Error Handling

```csharp
try
{
    container.RefreshGanttChart();
}
catch (GanttRenderingException ex)
{
    // Handle rendering errors
    Console.WriteLine($"Rendering failed: {ex.Message}");
}
catch (TaskValidationException ex)
{
    // Handle task validation errors
    Console.WriteLine($"Task validation failed: {ex.Message}");
}
```

## Migration Guide

### From Basic to Improved Container

```csharp
// Old way
var basic = new GanttChartContainer();
basic.Tasks = tasks;
basic.TimeScaleType = "Week";

// New way
var improved = new ImprovedGanttChartContainer();
improved.Tasks = new ObservableCollection<TaskItem>(tasks);
improved.Configuration.TimeScale.DefaultScale = TimeLevelType.Week;
improved.Theme = GanttChartTheme.CreateModern();
```