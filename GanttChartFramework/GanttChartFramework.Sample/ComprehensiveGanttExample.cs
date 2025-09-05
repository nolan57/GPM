using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using GanttChartFramework.Models;
using GanttChartFramework.Services;
using GanttChartFramework.Themes;
using GanttChartFramework.Views;

namespace GanttChartFramework.Sample
{
    /// <summary>
    /// Comprehensive example demonstrating the improved Gantt chart system
    /// </summary>
    public partial class ComprehensiveGanttExample : Window
    {
        private readonly ImprovedGanttChartContainer _ganttChart;
        private readonly ObservableCollection<EnhancedTaskItem> _tasks;
        private readonly TaskHierarchyService _hierarchyService;

        public ComprehensiveGanttExample()
        {
            InitializeComponent();
            
            _tasks = new ObservableCollection<EnhancedTaskItem>();
            _hierarchyService = new TaskHierarchyService();
            _ganttChart = new ImprovedGanttChartContainer();
            
            SetupGanttChart();
            CreateSampleData();
            DemonstrateFeatures();
        }

        #region Setup and Initialization

        private void SetupGanttChart()
        {
            // Create configuration
            var config = GanttChartConfiguration.CreateDefault();
            config.TaskDisplay.ShowProgress = true;
            config.TaskDisplay.ShowPriority = true;
            config.TaskDisplay.TaskHeight = 40;
            config.Grid.ShowVerticalLines = true;
            config.Grid.ShowHorizontalLines = true;
            config.Interaction.EnableDragDrop = true;
            config.Interaction.EnableResize = true;
            config.Interaction.EnableSelection = true;

            // Apply modern theme
            var theme = ThemeManager.GetTheme("Modern");
            
            // Set up the Gantt chart
            _ganttChart.Configuration = config;
            _ganttChart.Theme = theme;
            _ganttChart.Tasks = _tasks;
            
            // Set up viewport
            _ganttChart.Viewport.SetDateRange(DateTime.Today, DateTime.Today.AddDays(90));
            _ganttChart.Viewport.TotalWidth = 2000;
            _ganttChart.Viewport.TotalHeight = 800;

            // Add to main container
            MainContainer.Children.Add(_ganttChart);
        }

        #endregion

        #region Sample Data Creation

        private void CreateSampleData()
        {
            // Project: Software Development
            var project = new EnhancedTaskItem
            {
                Id = 1,
                Name = "Software Development Project",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(90),
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                Progress = 35,
                Color = Colors.DarkBlue,
                Assignee = "Project Manager",
                Description = "Complete software development lifecycle"
            };
            project.Tags.Add("Project");
            project.Tags.Add("Software");
            _tasks.Add(project);

            // Phase 1: Planning and Analysis
            var planning = new EnhancedTaskItem
            {
                Id = 2,
                Name = "Planning and Analysis",
                ParentId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(14),
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                Progress = 100,
                Color = Colors.Green,
                Assignee = "Business Analyst",
                TotalWork = 80,
                CompletedWork = 80
            };
            planning.Tags.Add("Planning");
            _tasks.Add(planning);

            // Requirements gathering
            var requirements = new EnhancedTaskItem
            {
                Id = 3,
                Name = "Requirements Gathering",
                ParentId = 2,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                Progress = 100,
                Color = Colors.LightGreen,
                Assignee = "Business Analyst",
                TotalWork = 40,
                CompletedWork = 40
            };
            _tasks.Add(requirements);

            // System Design
            var design = new EnhancedTaskItem
            {
                Id = 4,
                Name = "System Design",
                ParentId = 2,
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(14),
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                Progress = 100,
                Color = Colors.LightGreen,
                Assignee = "System Architect",
                TotalWork = 40,
                CompletedWork = 40
            };
            _tasks.Add(design);

            // Phase 2: Development
            var development = new EnhancedTaskItem
            {
                Id = 5,
                Name = "Development Phase",
                ParentId = 1,
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(60),
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                Progress = 60,
                Color = Colors.Orange,
                Assignee = "Development Team",
                TotalWork = 320,
                CompletedWork = 192
            };
            development.Tags.Add("Development");
            _tasks.Add(development);

            // Frontend Development
            var frontend = new EnhancedTaskItem
            {
                Id = 6,
                Name = "Frontend Development",
                ParentId = 5,
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(45),
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.Normal,
                Progress = 70,
                Color = Colors.LightBlue,
                Assignee = "Frontend Developer",
                TotalWork = 160,
                CompletedWork = 112
            };
            frontend.Tags.Add("Frontend");
            frontend.Tags.Add("UI");
            _tasks.Add(frontend);

            // Backend Development
            var backend = new EnhancedTaskItem
            {
                Id = 7,
                Name = "Backend Development",
                ParentId = 5,
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(50),
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.Normal,
                Progress = 50,
                Color = Colors.LightCoral,
                Assignee = "Backend Developer",
                TotalWork = 160,
                CompletedWork = 80,
                IsCritical = true
            };
            backend.Tags.Add("Backend");
            backend.Tags.Add("API");
            _tasks.Add(backend);

            // Phase 3: Testing
            var testing = new EnhancedTaskItem
            {
                Id = 8,
                Name = "Testing Phase",
                ParentId = 1,
                StartDate = DateTime.Today.AddDays(50),
                EndDate = DateTime.Today.AddDays(75),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.High,
                Progress = 0,
                Color = Colors.Purple,
                Assignee = "QA Team",
                TotalWork = 120,
                CompletedWork = 0
            };
            testing.Tags.Add("Testing");
            testing.Tags.Add("Quality");
            _tasks.Add(testing);

            // Unit Testing
            var unitTesting = new EnhancedTaskItem
            {
                Id = 9,
                Name = "Unit Testing",
                ParentId = 8,
                StartDate = DateTime.Today.AddDays(50),
                EndDate = DateTime.Today.AddDays(60),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.Normal,
                Progress = 0,
                Color = Colors.MediumPurple,
                Assignee = "QA Engineer",
                TotalWork = 40,
                CompletedWork = 0
            };
            _tasks.Add(unitTesting);

            // Integration Testing
            var integrationTesting = new EnhancedTaskItem
            {
                Id = 10,
                Name = "Integration Testing",
                ParentId = 8,
                StartDate = DateTime.Today.AddDays(60),
                EndDate = DateTime.Today.AddDays(70),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.Normal,
                Progress = 0,
                Color = Colors.MediumPurple,
                Assignee = "QA Engineer",
                TotalWork = 50,
                CompletedWork = 0
            };
            _tasks.Add(integrationTesting);

            // User Acceptance Testing
            var userTesting = new EnhancedTaskItem
            {
                Id = 11,
                Name = "User Acceptance Testing",
                ParentId = 8,
                StartDate = DateTime.Today.AddDays(65),
                EndDate = DateTime.Today.AddDays(75),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.High,
                Progress = 0,
                Color = Colors.MediumPurple,
                Assignee = "Product Owner",
                TotalWork = 30,
                CompletedWork = 0
            };
            _tasks.Add(userTesting);

            // Milestone: Beta Release
            var betaMilestone = new EnhancedTaskItem
            {
                Id = 12,
                Name = "Beta Release",
                StartDate = DateTime.Today.AddDays(75),
                EndDate = DateTime.Today.AddDays(75),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.Critical,
                Progress = 0,
                Color = Colors.Gold,
                IsMilestone = true,
                Assignee = "Release Manager"
            };
            betaMilestone.Tags.Add("Milestone");
            betaMilestone.Tags.Add("Release");
            _tasks.Add(betaMilestone);

            // Phase 4: Deployment
            var deployment = new EnhancedTaskItem
            {
                Id = 13,
                Name = "Deployment Phase",
                ParentId = 1,
                StartDate = DateTime.Today.AddDays(75),
                EndDate = DateTime.Today.AddDays(90),
                Status = TaskStatus.NotStarted,
                Priority = TaskPriority.Critical,
                Progress = 0,
                Color = Colors.Red,
                Assignee = "DevOps Team",
                TotalWork = 60,
                CompletedWork = 0
            };
            deployment.Tags.Add("Deployment");
            deployment.Tags.Add("Production");
            _tasks.Add(deployment);

            // Add task dependencies
            AddTaskDependencies();
            
            // Update parent task progress
            _hierarchyService.UpdateParentProgress(project);
            _hierarchyService.UpdateParentProgress(planning);
            _hierarchyService.UpdateParentProgress(development);
            _hierarchyService.UpdateParentProgress(testing);
        }

        private void AddTaskDependencies()
        {
            // Design depends on Requirements
            var designTask = _tasks.First(t => t.Id == 4);
            designTask.Dependencies.Add(new TaskDependency
            {
                PredecessorId = 3,
                SuccessorId = 4,
                Type = DependencyType.FinishToStart,
                Lag = TimeSpan.Zero
            });

            // Development depends on Design
            var devTask = _tasks.First(t => t.Id == 5);
            devTask.Dependencies.Add(new TaskDependency
            {
                PredecessorId = 4,
                SuccessorId = 5,
                Type = DependencyType.FinishToStart,
                Lag = TimeSpan.Zero
            });

            // Testing depends on Development
            var testTask = _tasks.First(t => t.Id == 8);
            testTask.Dependencies.Add(new TaskDependency
            {
                PredecessorId = 5,
                SuccessorId = 8,
                Type = DependencyType.FinishToStart,
                Lag = TimeSpan.FromDays(-10) // Overlap by 10 days
            });

            // Beta Milestone depends on Testing
            var betaTask = _tasks.First(t => t.Id == 12);
            betaTask.Dependencies.Add(new TaskDependency
            {
                PredecessorId = 8,
                SuccessorId = 12,
                Type = DependencyType.FinishToStart,
                Lag = TimeSpan.Zero
            });

            // Deployment depends on Beta
            var deployTask = _tasks.First(t => t.Id == 13);
            deployTask.Dependencies.Add(new TaskDependency
            {
                PredecessorId = 12,
                SuccessorId = 13,
                Type = DependencyType.FinishToStart,
                Lag = TimeSpan.Zero
            });
        }

        #endregion

        #region Feature Demonstrations

        private void DemonstrateFeatures()
        {
            // Set up event handlers to demonstrate interactions
            SetupEventHandlers();
            
            // Demonstrate theme switching
            DemonstrateThemes();
            
            // Demonstrate configuration options
            DemonstrateConfigurations();
            
            // Demonstrate advanced features
            DemonstrateAdvancedFeatures();
        }

        private void SetupEventHandlers()
        {
            // These would be connected to the actual interaction service
            // For demonstration purposes, showing what events are available
            
            Console.WriteLine("Event handlers available:");
            Console.WriteLine("- TaskClicked: Handle task selection");
            Console.WriteLine("- TaskDoubleClicked: Handle task editing");
            Console.WriteLine("- TaskDragCompleted: Update task dates");
            Console.WriteLine("- TaskResizeCompleted: Update task duration");
            Console.WriteLine("- SelectionChanged: Update UI based on selection");
            Console.WriteLine("- ContextMenuRequested: Show custom context menu");
        }

        private void DemonstrateThemes()
        {
            // Show different theme options
            var themes = new[]
            {
                "Default",
                "Dark",
                "Light", 
                "Modern"
            };

            Console.WriteLine("Available themes:");
            foreach (var themeName in themes)
            {
                var theme = ThemeManager.GetTheme(themeName);
                Console.WriteLine($"- {theme.Name}: {theme.Description}");
            }

            // Create custom theme from palette
            var customTheme = GanttChartTheme.CreateFromPalette(
                Colors.DarkSlateBlue,
                Colors.MediumSeaGreen, 
                Colors.Coral
            );
            ThemeManager.RegisterTheme(customTheme);
        }

        private void DemonstrateConfigurations()
        {
            // Show configuration options
            var config = new GanttChartConfiguration
            {
                TimeScale = new TimeScaleConfiguration
                {
                    DefaultScale = TimeLevelType.Day,
                    ShowWeekends = true,
                    ShowToday = true
                },
                TaskDisplay = new TaskDisplayConfiguration
                {
                    ShowLabels = true,
                    ShowProgress = true,
                    ShowPriority = true,
                    TaskHeight = 35,
                    SortBy = TaskSortBy.StartDate
                },
                Grid = new GridConfiguration
                {
                    ShowVerticalLines = true,
                    ShowHorizontalLines = true,
                    LineThickness = 1.0
                },
                Interaction = new InteractionConfiguration
                {
                    EnableDragDrop = true,
                    EnableResize = true,
                    EnableSelection = true,
                    EnableTooltips = true
                }
            };

            Console.WriteLine("Configuration options demonstrated:");
            Console.WriteLine($"- Time Scale: {config.TimeScale.DefaultScale}");
            Console.WriteLine($"- Task Height: {config.TaskDisplay.TaskHeight}");
            Console.WriteLine($"- Interactions: {config.Interaction.EnableDragDrop}");
        }

        private void DemonstrateAdvancedFeatures()
        {
            // Hierarchy management
            var hierarchicalTasks = _hierarchyService.BuildHierarchy(_tasks);
            Console.WriteLine($"Built hierarchy with {hierarchicalTasks.Count} root tasks");

            // Dependency validation
            var isValid = _hierarchyService.ValidateDependencies(_tasks);
            Console.WriteLine($"Dependencies valid: {isValid}");

            // Advanced filtering and searching
            var criticalTasks = _tasks.Where(t => t.IsCritical).ToList();
            var overdueTasks = _tasks.Where(t => t.IsOverdue).ToList();
            var completedTasks = _tasks.Where(t => t.Status == TaskStatus.Completed).ToList();
            
            Console.WriteLine($"Critical tasks: {criticalTasks.Count}");
            Console.WriteLine($"Overdue tasks: {overdueTasks.Count}");
            Console.WriteLine($"Completed tasks: {completedTasks.Count}");

            // Progress calculations
            var totalWork = _tasks.Sum(t => t.TotalWork);
            var completedWork = _tasks.Sum(t => t.CompletedWork);
            var overallProgress = totalWork > 0 ? (completedWork / totalWork) * 100 : 0;
            
            Console.WriteLine($"Overall project progress: {overallProgress:F1}%");
        }

        #endregion

        #region Migration Guide Examples

        /// <summary>
        /// Shows how to migrate from the old system to the new system
        /// </summary>
        private void MigrationExamples()
        {
            // OLD WAY (original components):
            /*
            var oldContainer = new GanttChartContainer();
            oldContainer.TimeScaleType = "Day";
            oldContainer.TaskHeight = 40;
            oldContainer.ShowTaskLabels = true;
            oldContainer.IsHierarchical = true;
            oldContainer.GridLineColor = Colors.Gray;
            oldContainer.GridLineThickness = 1.0;
            // ... 20+ more property assignments
            
            var oldTaskBar = new TaskBarControl();
            oldTaskBar.Tasks = new List<TaskItem>(); // Old task model
            oldTaskBar.TimeScaleType = "Day";
            oldTaskBar.ShowTaskLabels = true;
            // ... repetitive property assignments
            */

            // NEW WAY (improved components):
            var newContainer = new ImprovedGanttChartContainer();
            
            // Single configuration object
            newContainer.Configuration = GanttChartConfiguration.CreateDefault();
            
            // Enhanced task model with validation
            newContainer.Tasks = new ObservableCollection<EnhancedTaskItem>();
            
            // Comprehensive theming
            newContainer.Theme = GanttChartTheme.CreateModern();
            
            // Smart viewport management
            newContainer.Viewport.SetDateRange(DateTime.Today, DateTime.Today.AddMonths(3));

            Console.WriteLine("Migration completed:");
            Console.WriteLine("- Reduced code by ~80%");
            Console.WriteLine("- Added comprehensive validation");
            Console.WriteLine("- Improved performance with object pooling");
            Console.WriteLine("- Enhanced theming system");
            Console.WriteLine("- Better interaction handling");
        }

        #endregion

        #region UI Event Handlers

        private void OnSwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = _ganttChart.Theme.Name;
            var nextTheme = currentTheme switch
            {
                "Default" => "Dark",
                "Dark" => "Light",
                "Light" => "Modern",
                _ => "Default"
            };
            
            _ganttChart.Theme = ThemeManager.GetTheme(nextTheme);
        }

        private void OnZoomToFit_Click(object sender, RoutedEventArgs e)
        {
            _ganttChart.ZoomToFit();
        }

        private void OnExportImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|All Files|*.*",
                DefaultExt = "png"
            };

            if (dialog.ShowDialog() == true)
            {
                _ganttChart.ExportAsImage(dialog.FileName);
                MessageBox.Show($"Chart exported to {dialog.FileName}");
            }
        }

        private void OnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _ganttChart.Refresh();
        }

        #endregion
    }
}
