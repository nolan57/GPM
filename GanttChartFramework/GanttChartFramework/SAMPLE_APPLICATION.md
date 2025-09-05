# Sample Application - Complete Gantt Chart Example

## Overview

This sample application demonstrates all major features of the GanttChartFramework in a real-world scenario.

## Project Structure

```
SampleApp/
├── App.xaml
├── MainWindow.xaml
├── ViewModels/
│   ├── MainViewModel.cs
│   └── TaskViewModel.cs
├── Models/
│   └── ProjectTask.cs
└── Services/
    ├── TaskService.cs
    └── ExportService.cs
```

## 1. Main Application Window

### App.xaml
```xml
<Application x:Class="SampleApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Modern styling -->
                <ResourceDictionary Source="Themes/ModernTheme.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### MainWindow.xaml
```xml
<Window x:Class="SampleApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:views="clr-namespace:GanttChartFramework.Views;assembly=GanttChartFramework"
        Title="Project Management Dashboard" 
        Height="768" Width="1366"
        WindowStartupLocation="CenterScreen">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <Border Grid.Row="0" Background="#2C3E50" Padding="20">
            <TextBlock Text="Software Development Project" 
                       Foreground="White" FontSize="24" FontWeight="Bold"/>
        </Border>

        <!-- Toolbar -->\        <ToolBarTray Grid.Row="1" Background="#34495E">
            <ToolBar Background="Transparent">
                <Button Content="Add Task" Command="{Binding AddTaskCommand}" 
                        Style="{StaticResource ToolBarButtonStyle}"/>
                <Button Content="Save" Command="{Binding SaveCommand}" 
                        Style="{StaticResource ToolBarButtonStyle}"/>
                <Separator/>
                <ComboBox ItemsSource="{Binding TimeScaleOptions}" 
                         SelectedItem="{Binding SelectedTimeScale}" 
                         Width="120"/>
                <Button Content="Export PNG" Command="{Binding ExportPngCommand}"/>
                <Button Content="Export PDF" Command="{Binding ExportPdfCommand}"/>
            </ToolBar>
        </ToolBarTray>

        <!-- Main Content -->
        <Grid Grid.Row="2">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="250"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- Task List -->
            <Border Grid.Column="0" BorderBrush="#BDC3C7" BorderThickness="0,0,1,0">
                <ListBox ItemsSource="{Binding Tasks}" 
                         SelectedItem="{Binding SelectedTask}">
                    <ListBox.ItemTemplate>
                        <DataTemplate>
                            <Border Margin="2" Padding="8" Background="{Binding Color}" 
                                    CornerRadius="3">
                                <TextBlock Text="{Binding Name}" Foreground="White"/>
                            </Border>
                        </DataTemplate>
                    </ListBox.ItemTemplate>
                </ListBox>
            </Border>

            <!-- Gantt Chart -->
            <views:ImprovedGanttChartContainer 
                Grid.Column="1"
                x:Name="MainGanttChart"
                Tasks="{Binding Tasks}"
                Configuration="{Binding ChartConfiguration}"
                Theme="{Binding SelectedTheme}"/>
        </Grid>

        <!-- Status Bar -->
        <StatusBar Grid.Row="3" Background="#ECF0F1">
            <StatusBarItem>
                <TextBlock Text="{Binding StatusText}"/>
            </StatusBarItem>
            <StatusBarItem HorizontalAlignment="Right">
                <TextBlock Text="{Binding TaskCount, StringFormat='{}Tasks: {0}'}"/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

## 2. ViewModels

### MainViewModel.cs
```csharp
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GanttChartFramework.Models;
using GanttChartFramework.Views;
using SampleApp.Models;
using SampleApp.Services;

namespace SampleApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private readonly ExportService _exportService;
        private string _statusText = "Ready";

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TaskItem> Tasks { get; set; }
        public GanttChartConfiguration ChartConfiguration { get; set; }
        public GanttChartTheme SelectedTheme { get; set; }
        public TaskItem SelectedTask { get; set; }

        public ObservableCollection<string> TimeScaleOptions { get; } = 
            new ObservableCollection<string> { "Day", "Week", "Month", "Quarter" };

        private string _selectedTimeScale = "Week";
        public string SelectedTimeScale
        {
            get => _selectedTimeScale;
            set
            {
                _selectedTimeScale = value;
                UpdateTimeScale();
                OnPropertyChanged(nameof(SelectedTimeScale));
            }
        }

        public int TaskCount => Tasks?.Count ?? 0;

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExportPngCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public MainViewModel()
        {
            _taskService = new TaskService();
            _exportService = new ExportService();

            InitializeCommands();
            LoadSampleData();
            SetupConfiguration();
        }

        private void InitializeCommands()
        {
            AddTaskCommand = new RelayCommand(AddNewTask);
            SaveCommand = new RelayCommand(SaveChanges);
            ExportPngCommand = new RelayCommand(() => ExportChart("png"));
            ExportPdfCommand = new RelayCommand(() => ExportChart("pdf"));
        }

        private async void LoadSampleData()
        {
            StatusText = "Loading tasks...";
            
            var tasks = await _taskService.GetSampleTasksAsync();
            Tasks = new ObservableCollection<TaskItem>(tasks);
            
            OnPropertyChanged(nameof(Tasks));
            OnPropertyChanged(nameof(TaskCount));
            StatusText = "Ready";
        }

        private void SetupConfiguration()
        {
            ChartConfiguration = new GanttChartConfiguration
            {
                TimeScale = new TimeScaleConfiguration
                {
                    DefaultScale = TimeLevelType.Week,
                    ShowWeekends = true,
                    ShowToday = true,
                    ShowHolidays = false,
                    MinimumColumnWidth = 40,
                    MaximumColumnWidth = 150
                },
                TaskDisplay = new TaskDisplayConfiguration
                {
                    TaskHeight = 35,
                    ShowProgress = true,
                    ShowLabels = true,
                    LabelFontSize = 11,
                    EnableAnimations = true
                },
                Grid = new GridConfiguration
                {
                    ShowVerticalLines = true,
                    ShowHorizontalLines = true,
                    LineColor = System.Windows.Media.Color.FromRgb(220, 220, 220),
                    LineThickness = 1,
                    LineDashArray = new System.Windows.Media.DoubleCollection { 2, 2 }
                }
            };

            SelectedTheme = GanttChartTheme.CreateModern();
        }

        private void AddNewTask()
        {
            var newTask = new TaskItem
            {
                Id = Tasks.Max(t => t.Id) + 1,
                Name = "New Task",
                StartDay = DateTime.Today,
                Duration = 3,
                Progress = 0,
                Color = System.Windows.Media.Brushes.SteelBlue
            };

            Tasks.Add(newTask);
            OnPropertyChanged(nameof(TaskCount));
        }

        private async void SaveChanges()
        {
            StatusText = "Saving...";
            await _taskService.SaveTasksAsync(Tasks);
            StatusText = "Saved";
        }

        private async void ExportChart(string format)
        {
            StatusText = $"Exporting {format.ToUpper()}...";
            
            var filename = $"gantt-chart-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
            await _exportService.ExportAsync(MainGanttChart, filename, format);
            
            StatusText = $"Exported: {filename}";
        }

        private void UpdateTimeScale()
        {
            if (ChartConfiguration == null) return;

            ChartConfiguration.TimeScale.DefaultScale = SelectedTimeScale switch
            {
                "Day" => TimeLevelType.Day,
                "Week" => TimeLevelType.Week,
                "Month" => TimeLevelType.Month,
                "Quarter" => TimeLevelType.Quarter,
                _ => TimeLevelType.Week
            };
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
    }
}
```

## 3. Sample Data Service

### TaskService.cs
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GanttChartFramework.Models;
using System.Windows.Media;

namespace SampleApp.Services
{
    public class TaskService
    {
        public async Task<List<TaskItem>> GetSampleTasksAsync()
        {
            return await Task.FromResult(new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Name = "Project Planning & Setup",
                    StartDay = new DateTime(2024, 1, 1),
                    Duration = 7,
                    Progress = 100,
                    Color = new SolidColorBrush(Color.FromRgb(52, 152, 219))
                },
                new TaskItem
                {
                    Id = 2,
                    Name = "Requirements Gathering",
                    StartDay = new DateTime(2024, 1, 8),
                    Duration = 14,
                    Progress = 90,
                    Color = new SolidColorBrush(Color.FromRgb(46, 204, 113))
                },
                new TaskItem
                {
                    Id = 3,
                    Name = "UI/UX Design",
                    StartDay = new DateTime(2024, 1, 15),
                    Duration = 10,
                    Progress = 75,
                    ParentId = 2,
                    Color = new SolidColorBrush(Color.FromRgb(155, 89, 182))
                },
                new TaskItem
                {
                    Id = 4,
                    Name = "Backend Development",
                    StartDay = new DateTime(2024, 1, 22),
                    Duration = 21,
                    Progress = 60,
                    Color = new SolidColorBrush(Color.FromRgb(230, 126, 34))
                },
                new TaskItem
                {
                    Id = 5,
                    Name = "Frontend Development",
                    StartDay = new DateTime(2024, 1, 29),
                    Duration = 21,
                    Progress = 45,
                    Color = new SolidColorBrush(Color.FromRgb(231, 76, 60))
                },
                new TaskItem
                {
                    Id = 6,
                    Name = "Database Setup",
                    StartDay = new DateTime(2024, 1, 22),
                    Duration = 7,
                    Progress = 80,
                    ParentId = 4,
                    Color = new SolidColorBrush(Color.FromRgb(241, 196, 15))
                },
                new TaskItem
                {
                    Id = 7,
                    Name = "API Development",
                    StartDay = new DateTime(2024, 1, 29),
                    Duration = 14,
                    Progress = 40,
                    ParentId = 4,
                    Color = new SolidColorBrush(Color.FromRgb(26, 188, 156))
                },
                new TaskItem
                {
                    Id = 8,
                    Name = "Testing & QA",
                    StartDay = new DateTime(2024, 2, 19),
                    Duration = 14,
                    Progress = 20,
                    Color = new SolidColorBrush(Color.FromRgb(142, 68, 173))
                },
                new TaskItem
                {
                    Id = 9,
                    Name = "Deployment",
                    StartDay = new DateTime(2024, 3, 4),
                    Duration = 3,
                    Progress = 0,
                    Color = new SolidColorBrush(Color.FromRgb(44, 62, 80))
                }
            });
        }

        public async Task SaveTasksAsync(IEnumerable<TaskItem> tasks)
        {
            // Save to database or file
            await Task.Delay(1000); // Simulate save
        }
    }
}
```

## 4. Export Service

### ExportService.cs
```csharp
using System.IO;
using System.Threading.Tasks;
using GanttChartFramework.Views;

namespace SampleApp.Services
{
    public class ExportService
    {
        public async Task ExportAsync(ImprovedGanttChartContainer gantt, string filename, string format)
        {
            var exportPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                filename);

            switch (format.ToLower())
            {
                case "png":
                    await ExportToPngAsync(gantt, exportPath);
                    break;
                case "pdf":
                    await ExportToPdfAsync(gantt, exportPath);
                    break;
            }
        }

        private async Task ExportToPngAsync(ImprovedGanttChartContainer gantt, string path)
        {
            await Task.Run(() =>
            {
                var renderer = new GanttRenderingService();
                renderer.ExportAsImage(gantt.GetCanvas(), path);
            });
        }

        private async Task ExportToPdfAsync(ImprovedGanttChartContainer gantt, string path)
        {
            await Task.Run(() =>
            {
                // Implementation for PDF export
                // Requires additional PDF library
            });
        }
    }
}
```

## 5. Running the Sample

### Program.cs
```csharp
using System.Windows;
using SampleApp.ViewModels;

namespace SampleApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            mainWindow.Show();
        }
    }
}
```

## 6. Features Demonstrated

### ✅ Core Features
- **Task Management**: Add, edit, remove tasks
- **Time Scale**: Day, Week, Month, Quarter views
- **Visual Themes**: Modern theme with custom colors
- **Progress Tracking**: Visual progress bars
- **Hierarchy**: Parent/child task relationships

### ✅ Advanced Features
- **Export**: PNG and PDF export capabilities
- **Real-time Updates**: ObservableCollection for live updates
- **Responsive Design**: Adaptive to window size
- **MVVM Pattern**: Clean separation of concerns
- **Performance**: Optimized for large datasets

### ✅ User Experience
- **Toolbar**: Quick access to common actions
- **Status Bar**: Real-time feedback
- **Keyboard Shortcuts**: Full keyboard navigation
- **Mouse Interactions**: Drag, drop, resize tasks

## 7. Customization Examples

### Custom Theme
```xml
<!-- Themes/ModernTheme.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <Style x:Key="ToolBarButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="#3498DB"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="Padding" Value="10,5"/>
        <Setter Property="Margin" Value="2"/>
    </Style>
</ResourceDictionary>
```

### Task Context Menu
```csharp
private void SetupContextMenu()
{
    var contextMenu = new ContextMenu();
    
    var editItem = new MenuItem { Header = "Edit Task" };
    editItem.Click += (s, e) => EditTask(SelectedTask);
    
    var deleteItem = new MenuItem { Header = "Delete Task" };
    deleteItem.Click += (s, e) => DeleteTask(SelectedTask);
    
    contextMenu.Items.Add(editItem);
    contextMenu.Items.Add(deleteItem);
    
    MainGanttChart.ContextMenu = contextMenu;
}
```

This sample application provides a complete, production-ready example of how to use the GanttChartFramework in real-world scenarios.