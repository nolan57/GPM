using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace GanttChartFramework.Models
{
    /// <summary>
    /// Enhanced task item with full property change notifications and validation
    /// </summary>
    public class EnhancedTaskItem : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Private Fields

        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(1);
        private double _progress;
        private int? _parentId;
        private TaskPriority _priority = TaskPriority.Normal;
        private TaskStatus _status = TaskStatus.NotStarted;
        private Color _color = Colors.Blue;
        private double _completedWork;
        private double _totalWork = 8.0; // Default 8 hours
        private string _assignee = string.Empty;
        private ObservableCollection<string> _tags = new();
        private ObservableCollection<TaskDependency> _dependencies = new();
        private ObservableCollection<EnhancedTaskItem> _children = new();
        private bool _isMilestone;
        private bool _isCritical;

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value ?? string.Empty);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value ?? string.Empty);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    ValidateDateRange();
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    ValidateDateRange();
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public TimeSpan Duration => EndDate - StartDate;

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, Math.Max(0, Math.Min(100, value)));
        }

        public int? ParentId
        {
            get => _parentId;
            set => SetProperty(ref _parentId, value);
        }

        public TaskPriority Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        public TaskStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                {
                    UpdateProgressBasedOnStatus();
                }
            }
        }

        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public Brush ColorBrush => new SolidColorBrush(Color);

        public double CompletedWork
        {
            get => _completedWork;
            set
            {
                if (SetProperty(ref _completedWork, Math.Max(0, value)))
                {
                    UpdateProgressFromWork();
                }
            }
        }

        public double TotalWork
        {
            get => _totalWork;
            set
            {
                if (SetProperty(ref _totalWork, Math.Max(0.1, value)))
                {
                    UpdateProgressFromWork();
                }
            }
        }

        public string Assignee
        {
            get => _assignee;
            set => SetProperty(ref _assignee, value ?? string.Empty);
        }

        public ObservableCollection<string> Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value ?? new ObservableCollection<string>());
        }

        public ObservableCollection<TaskDependency> Dependencies
        {
            get => _dependencies;
            set => SetProperty(ref _dependencies, value ?? new ObservableCollection<TaskDependency>());
        }

        public ObservableCollection<EnhancedTaskItem> Children
        {
            get => _children;
            set => SetProperty(ref _children, value ?? new ObservableCollection<EnhancedTaskItem>());
        }

        public bool IsMilestone
        {
            get => _isMilestone;
            set => SetProperty(ref _isMilestone, value);
        }

        public bool IsCritical
        {
            get => _isCritical;
            set => SetProperty(ref _isCritical, value);
        }

        // Computed properties
        public bool IsParent => Children.Any();
        public bool HasDependencies => Dependencies.Any();
        public bool IsOverdue => EndDate < DateTime.Today && Status != TaskStatus.Completed;
        public double WorkRemaining => Math.Max(0, TotalWork - CompletedWork);
        public string TagsString => string.Join(", ", Tags);

        #endregion

        #region Methods

        private void ValidateDateRange()
        {
            if (EndDate < StartDate)
            {
                EndDate = StartDate.AddDays(1);
            }
        }

        private void UpdateProgressBasedOnStatus()
        {
            switch (Status)
            {
                case TaskStatus.NotStarted:
                    Progress = 0;
                    break;
                case TaskStatus.Completed:
                    Progress = 100;
                    CompletedWork = TotalWork;
                    break;
            }
        }

        private void UpdateProgressFromWork()
        {
            if (TotalWork > 0)
            {
                Progress = (CompletedWork / TotalWork) * 100;
                
                // Update status based on progress
                if (Progress == 0)
                    Status = TaskStatus.NotStarted;
                else if (Progress >= 100)
                    Status = TaskStatus.Completed;
                else
                    Status = TaskStatus.InProgress;
            }
        }

        /// <summary>
        /// Creates a deep copy of the task
        /// </summary>
        public EnhancedTaskItem Clone()
        {
            return new EnhancedTaskItem
            {
                Id = Id,
                Name = Name,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                Progress = Progress,
                ParentId = ParentId,
                Priority = Priority,
                Status = Status,
                Color = Color,
                CompletedWork = CompletedWork,
                TotalWork = TotalWork,
                Assignee = Assignee,
                IsMilestone = IsMilestone,
                IsCritical = IsCritical,
                Tags = new ObservableCollection<string>(Tags),
                Dependencies = new ObservableCollection<TaskDependency>(Dependencies.Select(d => d.Clone()))
            };
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region IDataErrorInfo

        public string Error => null;

        public string this[string propertyName]
        {
            get
            {
                return propertyName switch
                {
                    nameof(Name) => string.IsNullOrWhiteSpace(Name) ? "Name is required" : null,
                    nameof(StartDate) => StartDate < DateTime.Today.AddYears(-10) ? "Start date is too far in the past" : null,
                    nameof(EndDate) => EndDate < StartDate ? "End date must be after start date" : null,
                    nameof(TotalWork) => TotalWork <= 0 ? "Total work must be greater than 0" : null,
                    _ => null
                };
            }
        }

        #endregion
    }

    /// <summary>
    /// Task priority enumeration
    /// </summary>
    public enum TaskPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Critical = 4
    }

    /// <summary>
    /// Task status enumeration
    /// </summary>
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Task dependency relationship
    /// </summary>
    public class TaskDependency : INotifyPropertyChanged
    {
        private int _predecessorId;
        private int _successorId;
        private DependencyType _type = DependencyType.FinishToStart;
        private TimeSpan _lag = TimeSpan.Zero;

        public int PredecessorId
        {
            get => _predecessorId;
            set => SetProperty(ref _predecessorId, value);
        }

        public int SuccessorId
        {
            get => _successorId;
            set => SetProperty(ref _successorId, value);
        }

        public DependencyType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public TimeSpan Lag
        {
            get => _lag;
            set => SetProperty(ref _lag, value);
        }

        public TaskDependency Clone()
        {
            return new TaskDependency
            {
                PredecessorId = PredecessorId,
                SuccessorId = SuccessorId,
                Type = Type,
                Lag = Lag
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Dependency type enumeration
    /// </summary>
    public enum DependencyType
    {
        FinishToStart,
        StartToStart,
        FinishToFinish,
        StartToFinish
    }

    /// <summary>
    /// Task hierarchy and management service
    /// </summary>
    public class TaskHierarchyService
    {
        /// <summary>
        /// Builds a hierarchical tree from flat task list
        /// </summary>
        public ObservableCollection<EnhancedTaskItem> BuildHierarchy(IEnumerable<EnhancedTaskItem> tasks)
        {
            var taskDict = tasks.ToDictionary(t => t.Id);
            var rootTasks = new ObservableCollection<EnhancedTaskItem>();

            foreach (var task in tasks)
            {
                if (task.ParentId == null)
                {
                    rootTasks.Add(task);
                }
                else if (taskDict.TryGetValue(task.ParentId.Value, out var parent))
                {
                    parent.Children.Add(task);
                }
            }

            return rootTasks;
        }

        /// <summary>
        /// Flattens hierarchical task tree to a list
        /// </summary>
        public List<EnhancedTaskItem> FlattenHierarchy(IEnumerable<EnhancedTaskItem> tasks)
        {
            var result = new List<EnhancedTaskItem>();
            
            foreach (var task in tasks)
            {
                result.Add(task);
                result.AddRange(FlattenHierarchy(task.Children));
            }

            return result;
        }

        /// <summary>
        /// Calculates parent task progress based on children
        /// </summary>
        public void UpdateParentProgress(EnhancedTaskItem parent)
        {
            if (!parent.Children.Any()) return;

            var totalWork = parent.Children.Sum(c => c.TotalWork);
            var completedWork = parent.Children.Sum(c => c.CompletedWork);

            if (totalWork > 0)
            {
                parent.Progress = (completedWork / totalWork) * 100;
                parent.CompletedWork = completedWork;
                parent.TotalWork = totalWork;
            }

            // Update dates based on children
            parent.StartDate = parent.Children.Min(c => c.StartDate);
            parent.EndDate = parent.Children.Max(c => c.EndDate);
        }

        /// <summary>
        /// Validates task dependencies for circular references
        /// </summary>
        public bool ValidateDependencies(IEnumerable<EnhancedTaskItem> tasks)
        {
            var visited = new HashSet<int>();
            var recursionStack = new HashSet<int>();

            foreach (var task in tasks)
            {
                if (!visited.Contains(task.Id))
                {
                    if (HasCircularDependency(task.Id, tasks, visited, recursionStack))
                        return false;
                }
            }

            return true;
        }

        private bool HasCircularDependency(int taskId, IEnumerable<EnhancedTaskItem> tasks, 
                                         HashSet<int> visited, HashSet<int> recursionStack)
        {
            visited.Add(taskId);
            recursionStack.Add(taskId);

            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                foreach (var dependency in task.Dependencies)
                {
                    var dependentId = dependency.PredecessorId;
                    
                    if (!visited.Contains(dependentId))
                    {
                        if (HasCircularDependency(dependentId, tasks, visited, recursionStack))
                            return true;
                    }
                    else if (recursionStack.Contains(dependentId))
                    {
                        return true;
                    }
                }
            }

            recursionStack.Remove(taskId);
            return false;
        }
    }
}
