# GanttChartFramework Quick Reference

## 🚀 Quick Setup (30 seconds)

```xml
<!-- XAML -->
<views:GanttChartContainer 
    x:Name="GanttChart"
    Tasks="{Binding Tasks}"
    TimeScaleType="Week"
    Width="800" Height="600" />
```

```csharp
// Code
var tasks = new List<TaskItem>
{
    new TaskItem { Name = "Task 1", StartDay = DateTime.Today, Duration = 5 }
};
GanttChart.Tasks = tasks;
```

## 📋 Common Properties

| Task Property | Type | Example |
|---------------|------|---------|
| `Name` | string | `"Development"` |
| `StartDay` | DateTime | `DateTime.Today` |
| `Duration` | int | `7` (days) |
| `Progress` | double | `75` (%) |
| `Color` | Brush | `Brushes.Blue` |
| `ParentId` | int? | `1` (hierarchy) |

## 🎨 Theme Options

```csharp
// Pre-built themes
GanttChartTheme.CreateDefault()
GanttChartTheme.CreateDarkTheme()
GanttChartTheme.CreateLightTheme()
GanttChartTheme.CreateModernTheme()

// Custom colors
new GanttChartTheme 
{ 
    BackgroundBrush = Brushes.White,
    TaskBarBrush = Brushes.SteelBlue 
}
```

## ⏰ Time Scale Types

- `"Day"` - Daily view
- `"Week"` - Weekly view
- `"Month"` - Monthly view
- `"Quarter"` - Quarterly view

## 🔧 Essential Methods

```csharp
// Add task
GanttChart.Tasks.Add(new TaskItem { ... });

// Remove task
GanttChart.Tasks.RemoveAt(index);

// Refresh display
GanttChart.RefreshGanttChart();

// Export image
new GanttRenderingService().ExportAsImage(canvas, "chart.png");
```

## 📊 Data Binding Patterns

### Simple Binding
```csharp
// ViewModel
public ObservableCollection<TaskItem> Tasks { get; set; }

// XAML
Tasks="{Binding Tasks}"
```

### Async Loading
```csharp
public async Task LoadTasksAsync()
{
    var tasks = await service.GetTasksAsync();
    Tasks = new ObservableCollection<TaskItem>(tasks);
}
```

## 🎯 Performance Tips

- Use `ObservableCollection` for automatic updates
- Enable virtualization for 1000+ tasks
- Batch updates with `DeferRefresh()`
- Set reasonable date ranges

## 🔄 Interactive Features

```csharp
// Handle selection
GanttChart.SelectedTasks.CollectionChanged += (s, e) => { ... };

// Handle drag/drop
GanttChart.TaskUpdated += (s, task) => { ... };

// Zoom to fit
container.ZoomToFit();
```

## 📱 Responsive Design

```xml
<!-- Auto-size -->
<views:GanttChartContainer 
    HorizontalAlignment="Stretch"
    VerticalAlignment="Stretch" />

<!-- Fixed size -->
<views:GanttChartContainer 
    Width="800" Height="600" />
```

## 🎨 Color Schemes

### Professional Colors
- Primary: `#3498DB` (Blue)
- Success: `#2ECC71` (Green)
- Warning: `#F39C12` (Orange)
- Danger: `#E74C3C` (Red)

### Quick Apply
```csharp
var theme = GanttChartTheme.CreateModern();
theme.TaskBarBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
```

## 📈 Common Scenarios

### 1. Project Timeline
```csharp
var projectTasks = new List<TaskItem>
{
    new TaskItem { Name = "Planning", StartDay = DateTime.Today, Duration = 5, Progress = 100 },
    new TaskItem { Name = "Development", StartDay = DateTime.Today.AddDays(5), Duration = 14, Progress = 60 },
    new TaskItem { Name = "Testing", StartDay = DateTime.Today.AddDays(19), Duration = 7, Progress = 0 }
};
```

### 2. Team Tasks
```csharp
var teamTasks = new List<TaskItem>
{
    new TaskItem { Name = "UI Design", StartDay = DateTime.Today, Duration = 3, Color = Brushes.Purple },
    new TaskItem { Name = "Backend", StartDay = DateTime.Today, Duration = 5, Color = Brushes.Green },
    new TaskItem { Name = "Frontend", StartDay = DateTime.Today.AddDays(2), Duration = 4, Color = Brushes.Blue }
};
```

## 🔍 Debugging Checklist

- [ ] Tasks collection is not null
- [ ] StartDay has valid date
- [ ] Duration > 0
- [ ] Tasks bound to control
- [ ] Theme applied after initialization

## 📋 Common Issues & Fixes

| Issue | Solution |
|-------|----------|
| Tasks not showing | Check `StartDay` and `Duration` |
| Performance slow | Enable virtualization |
| Colors not applying | Set theme after `Loaded` event |
| Data not updating | Use `ObservableCollection` |

## 🚀 Copy-Paste Templates

### Template 1: Basic Project Gantt
```xml
<views:GanttChartContainer 
    Tasks="{Binding ProjectTasks}"
    TimeScaleType="Week"
    TaskHeight="35"
    ShowTaskLabels="True"
    Width="1000" Height="500" />
```

### Template 2: Team Dashboard
```xml
<views:ImprovedGanttChartContainer 
    Tasks="{Binding TeamTasks}"
    Configuration="{Binding ChartConfig}"
    Theme="{Binding ModernTheme}"
    HorizontalAlignment="Stretch" />
```

### Template 3: Report Export
```csharp
var exporter = new GanttRenderingService();
exporter.ExportAsImage(ganttChart.GetCanvas(), "project-report.png");
```

## 📞 Support Resources

- **Documentation**: [API Documentation](API_DOCUMENTATION.md)
- **Usage Guide**: [Usage Guide](USAGE_GUIDE.md)
- **Examples**: See sample projects in repository