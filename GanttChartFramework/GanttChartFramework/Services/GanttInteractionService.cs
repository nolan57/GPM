using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GanttChartFramework.Models;

namespace GanttChartFramework.Services
{
    /// <summary>
    /// Advanced interaction service for Gantt chart user interactions
    /// </summary>
    public class GanttInteractionService
    {
        #region Events

        public event EventHandler<TaskInteractionEventArgs> TaskClicked;
        public event EventHandler<TaskInteractionEventArgs> TaskDoubleClicked;
        public event EventHandler<TaskDragEventArgs> TaskDragStarted;
        public event EventHandler<TaskDragEventArgs> TaskDragCompleted;
        public event EventHandler<TaskResizeEventArgs> TaskResizeStarted;
        public event EventHandler<TaskResizeEventArgs> TaskResizeCompleted;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<ContextMenuRequestedEventArgs> ContextMenuRequested;
        public event EventHandler<TooltipRequestedEventArgs> TooltipRequested;

        #endregion

        #region Private Fields

        private Canvas _canvas;
        private readonly Dictionary<UIElement, EnhancedTaskItem> _elementTaskMap = new();
        private readonly HashSet<EnhancedTaskItem> _selectedTasks = new();
        private readonly Dictionary<int, UIElement> _taskElements = new();
        
        private bool _isDragging;
        private bool _isResizing;
        private Point _dragStartPoint;
        private EnhancedTaskItem _draggedTask;
        private UIElement _draggedElement;
        private ResizeHandle _activeResizeHandle;
        private Rectangle _selectionRectangle;
        private Point _selectionStartPoint;
        
        private readonly ToolTipService _tooltipService;
        private readonly SelectionService _selectionService;
        private readonly DragDropService _dragDropService;

        #endregion

        #region Constructor

        public GanttInteractionService()
        {
            _tooltipService = new ToolTipService();
            _selectionService = new SelectionService();
            _dragDropService = new DragDropService();
        }

        #endregion

        #region Initialization

        public void Initialize(Canvas canvas, InteractionConfiguration config)
        {
            _canvas = canvas;
            
            if (config.EnableSelection)
                EnableSelection();
            
            if (config.EnableDragDrop)
                EnableDragDrop();
            
            if (config.EnableResize)
                EnableResize();
            
            if (config.EnableTooltips)
                EnableTooltips();

            SetupCanvasEvents();
        }

        private void SetupCanvasEvents()
        {
            _canvas.MouseDown += OnCanvasMouseDown;
            _canvas.MouseMove += OnCanvasMouseMove;
            _canvas.MouseUp += OnCanvasMouseUp;
            _canvas.MouseLeave += OnCanvasMouseLeave;
            _canvas.KeyDown += OnCanvasKeyDown;
            _canvas.Focusable = true; // Enable keyboard events
        }

        #endregion

        #region Task Registration

        public void RegisterTaskElement(UIElement element, EnhancedTaskItem task)
        {
            _elementTaskMap[element] = task;
            _taskElements[task.Id] = element;
            
            SetupTaskElementEvents(element, task);
        }

        private void SetupTaskElementEvents(UIElement element, EnhancedTaskItem task)
        {
            element.MouseDown += (s, e) => OnTaskMouseDown(task, element, e);
            element.MouseUp += (s, e) => OnTaskMouseUp(task, element, e);
            element.MouseMove += (s, e) => OnTaskMouseMove(task, element, e);
            element.MouseEnter += (s, e) => OnTaskMouseEnter(task, element, e);
            element.MouseLeave += (s, e) => OnTaskMouseLeave(task, element, e);
            element.MouseRightButtonUp += (s, e) => OnTaskRightClick(task, element, e);
            
            // Enable double-click detection
            var clickTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            
            var clickCount = 0;
            element.MouseLeftButtonDown += (s, e) =>
            {
                clickCount++;
                if (clickCount == 1)
                {
                    clickTimer.Start();
                    clickTimer.Tick += (_, __) =>
                    {
                        clickTimer.Stop();
                        if (clickCount == 1)
                            TaskClicked?.Invoke(this, new TaskInteractionEventArgs { Task = task, Element = element });
                        else if (clickCount == 2)
                            TaskDoubleClicked?.Invoke(this, new TaskInteractionEventArgs { Task = task, Element = element });
                        clickCount = 0;
                    };
                }
            };
        }

        #endregion

        #region Selection Management

        private void EnableSelection()
        {
            _selectionService.MultiSelectEnabled = true;
            _selectionService.RectangleSelectEnabled = true;
        }

        public void SelectTask(EnhancedTaskItem task, bool addToSelection = false)
        {
            if (!addToSelection)
                ClearSelection();

            if (_selectedTasks.Add(task) && _taskElements.TryGetValue(task.Id, out var element))
            {
                ApplySelectionVisuals(element, true);
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs 
                { 
                    SelectedTasks = _selectedTasks.ToList(),
                    AddedTask = task
                });
            }
        }

        public void DeselectTask(EnhancedTaskItem task)
        {
            if (_selectedTasks.Remove(task) && _taskElements.TryGetValue(task.Id, out var element))
            {
                ApplySelectionVisuals(element, false);
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs 
                { 
                    SelectedTasks = _selectedTasks.ToList(),
                    RemovedTask = task
                });
            }
        }

        public void ClearSelection()
        {
            foreach (var task in _selectedTasks.ToList())
            {
                DeselectTask(task);
            }
        }

        private void ApplySelectionVisuals(UIElement element, bool selected)
        {
            if (element is Rectangle rect)
            {
                if (selected)
                {
                    rect.Stroke = Brushes.Blue;
                    rect.StrokeThickness = 2;
                    rect.StrokeDashArray = new DoubleCollection { 5, 3 };
                }
                else
                {
                    rect.Stroke = null;
                    rect.StrokeThickness = 0;
                    rect.StrokeDashArray = null;
                }
            }
        }

        #endregion

        #region Drag and Drop

        private void EnableDragDrop()
        {
            _dragDropService.DragThreshold = 5; // pixels
            _dragDropService.ShowDragPreview = true;
        }

        private void StartTaskDrag(EnhancedTaskItem task, UIElement element, Point startPoint)
        {
            _isDragging = true;
            _draggedTask = task;
            _draggedElement = element;
            _dragStartPoint = startPoint;
            
            _canvas.CaptureMouse();
            _canvas.Cursor = Cursors.Hand;

            TaskDragStarted?.Invoke(this, new TaskDragEventArgs 
            { 
                Task = task, 
                Element = element, 
                StartPosition = startPoint 
            });
        }

        private void UpdateTaskDrag(Point currentPoint)
        {
            if (!_isDragging || _draggedTask == null) return;

            var deltaX = currentPoint.X - _dragStartPoint.X;
            var deltaY = currentPoint.Y - _dragStartPoint.Y;

            // Update visual position
            Canvas.SetLeft(_draggedElement, Canvas.GetLeft(_draggedElement) + deltaX);
            Canvas.SetTop(_draggedElement, Canvas.GetTop(_draggedElement) + deltaY);

            _dragStartPoint = currentPoint;
        }

        private void EndTaskDrag(Point endPoint)
        {
            if (!_isDragging || _draggedTask == null) return;

            _isDragging = false;
            _canvas.ReleaseMouseCapture();
            _canvas.Cursor = Cursors.Arrow;

            // Calculate new task dates based on position
            var newStartDate = CalculateDateFromPosition(endPoint);
            var duration = _draggedTask.Duration;

            TaskDragCompleted?.Invoke(this, new TaskDragEventArgs 
            { 
                Task = _draggedTask, 
                Element = _draggedElement,
                StartPosition = _dragStartPoint,
                EndPosition = endPoint,
                NewStartDate = newStartDate,
                NewEndDate = newStartDate.Add(duration)
            });

            _draggedTask = null;
            _draggedElement = null;
        }

        #endregion

        #region Resize Functionality

        private void EnableResize()
        {
            // Add resize handles to task elements
            foreach (var kvp in _taskElements)
            {
                AddResizeHandles(kvp.Value, kvp.Key);
            }
        }

        private void AddResizeHandles(UIElement element, int taskId)
        {
            if (element is Rectangle rect)
            {
                // Left handle
                var leftHandle = CreateResizeHandle(ResizeHandle.Left);
                Canvas.SetLeft(leftHandle, Canvas.GetLeft(rect) - 3);
                Canvas.SetTop(leftHandle, Canvas.GetTop(rect) + rect.Height / 2 - 3);
                _canvas.Children.Add(leftHandle);

                // Right handle
                var rightHandle = CreateResizeHandle(ResizeHandle.Right);
                Canvas.SetLeft(rightHandle, Canvas.GetLeft(rect) + rect.Width - 3);
                Canvas.SetTop(rightHandle, Canvas.GetTop(rect) + rect.Height / 2 - 3);
                _canvas.Children.Add(rightHandle);
            }
        }

        private Rectangle CreateResizeHandle(ResizeHandle handleType)
        {
            var handle = new Rectangle
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Blue,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Cursor = Cursors.SizeWE
            };

            handle.MouseDown += (s, e) => StartResize(handleType, e.GetPosition(_canvas));
            return handle;
        }

        private void StartResize(ResizeHandle handleType, Point startPoint)
        {
            _isResizing = true;
            _activeResizeHandle = handleType;
            _dragStartPoint = startPoint;
            _canvas.CaptureMouse();
        }

        #endregion

        #region Context Menu

        private void ShowContextMenu(EnhancedTaskItem task, UIElement element, Point position)
        {
            var contextMenu = CreateContextMenu(task);
            contextMenu.PlacementTarget = element;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            contextMenu.IsOpen = true;

            ContextMenuRequested?.Invoke(this, new ContextMenuRequestedEventArgs 
            { 
                Task = task, 
                Element = element, 
                Position = position,
                ContextMenu = contextMenu
            });
        }

        private ContextMenu CreateContextMenu(EnhancedTaskItem task)
        {
            var contextMenu = new ContextMenu();

            // Edit task
            var editItem = new MenuItem { Header = "Edit Task" };
            editItem.Click += (s, e) => EditTask(task);
            contextMenu.Items.Add(editItem);

            // Delete task
            var deleteItem = new MenuItem { Header = "Delete Task" };
            deleteItem.Click += (s, e) => DeleteTask(task);
            contextMenu.Items.Add(deleteItem);

            contextMenu.Items.Add(new Separator());

            // Mark as complete
            if (task.Status != Models.TaskStatus.Completed)
            {
                var completeItem = new MenuItem { Header = "Mark as Complete" };
                completeItem.Click += (s, e) => MarkTaskComplete(task);
                contextMenu.Items.Add(completeItem);
            }

            // Add dependency
            var dependencyItem = new MenuItem { Header = "Add Dependency" };
            contextMenu.Items.Add(dependencyItem);

            return contextMenu;
        }

        #endregion

        #region Tooltip Management

        private void EnableTooltips()
        {
            _tooltipService.ShowDelay = TimeSpan.FromMilliseconds(500);
            _tooltipService.HideDelay = TimeSpan.FromMilliseconds(5000);
        }

        private void ShowTooltip(EnhancedTaskItem task, UIElement element, Point position)
        {
            var tooltipContent = CreateTooltipContent(task);
            _tooltipService.ShowTooltip(element, tooltipContent, position);

            TooltipRequested?.Invoke(this, new TooltipRequestedEventArgs 
            { 
                Task = task, 
                Element = element, 
                Position = position,
                Content = tooltipContent
            });
        }

        private FrameworkElement CreateTooltipContent(EnhancedTaskItem task)
        {
            var panel = new StackPanel { Orientation = Orientation.Vertical };

            panel.Children.Add(new TextBlock 
            { 
                Text = task.Name, 
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            panel.Children.Add(new TextBlock 
            { 
                Text = $"Start: {task.StartDate:d}",
                FontSize = 11
            });

            panel.Children.Add(new TextBlock 
            { 
                Text = $"End: {task.EndDate:d}",
                FontSize = 11
            });

            panel.Children.Add(new TextBlock 
            { 
                Text = $"Progress: {task.Progress:F1}%",
                FontSize = 11
            });

            if (!string.IsNullOrEmpty(task.Assignee))
            {
                panel.Children.Add(new TextBlock 
                { 
                    Text = $"Assigned to: {task.Assignee}",
                    FontSize = 11
                });
            }

            return panel;
        }

        #endregion

        #region Event Handlers

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StartRectangleSelection(e.GetPosition(_canvas));
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(_canvas);

            if (_isDragging)
                UpdateTaskDrag(currentPoint);
            else if (_isResizing)
                UpdateResize(currentPoint);
            else if (_selectionRectangle != null)
                UpdateRectangleSelection(currentPoint);
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
                EndTaskDrag(e.GetPosition(_canvas));
            else if (_isResizing)
                EndResize(e.GetPosition(_canvas));
            else if (_selectionRectangle != null)
                EndRectangleSelection();
        }

        private void OnTaskMouseDown(EnhancedTaskItem task, UIElement element, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(_canvas);
                if (ShouldStartDrag(position))
                {
                    StartTaskDrag(task, element, position);
                }
                else
                {
                    SelectTask(task, Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
                }
            }

            e.Handled = true;
        }

        private void OnTaskRightClick(EnhancedTaskItem task, UIElement element, MouseButtonEventArgs e)
        {
            SelectTask(task);
            ShowContextMenu(task, element, e.GetPosition(_canvas));
            e.Handled = true;
        }

        private void OnTaskMouseEnter(EnhancedTaskItem task, UIElement element, MouseEventArgs e)
        {
            ShowTooltip(task, element, e.GetPosition(_canvas));
            if (element is FrameworkElement fe)
                fe.Cursor = Cursors.Hand;
        }

        private void OnTaskMouseLeave(EnhancedTaskItem task, UIElement element, MouseEventArgs e)
        {
            _tooltipService.HideTooltip();
            if (element is FrameworkElement fe)
                fe.Cursor = Cursors.Arrow;
        }

        // Missing event handler methods
        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            // Handle canvas mouse leave - clear any hover states
            if (_isDragging)
            {
                // Continue dragging outside the canvas if needed
                return;
            }
            
            // Clear any temporary visual states
        }

        private void OnCanvasKeyDown(object sender, KeyEventArgs e)
        {
            // Handle keyboard shortcuts
            switch (e.Key)
            {
                case Key.Delete:
                    if (_selectedTasks.Any())
                    {
                        foreach (var task in _selectedTasks.ToList())
                        {
                            DeleteTask(task);
                        }
                    }
                    e.Handled = true;
                    break;
                    
                case Key.Escape:
                    ClearSelection();
                    e.Handled = true;
                    break;
                    
                case Key.A when (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control:
                    // Select all tasks
                    foreach (var kvp in _elementTaskMap)
                    {
                        SelectTask(kvp.Value, true);
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void OnTaskMouseUp(EnhancedTaskItem task, UIElement element, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                EndTaskDrag(e.GetPosition(_canvas));
            }
            
            if (_isResizing)
            {
                EndResize(e.GetPosition(_canvas));
            }
            
            e.Handled = true;
        }

        private void OnTaskMouseMove(EnhancedTaskItem task, UIElement element, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(_canvas);
            
            if (_isDragging && _draggedTask == task)
            {
                UpdateTaskDrag(currentPoint);
            }
            
            if (_isResizing)
            {
                UpdateResize(currentPoint);
            }
        }

        private void UpdateResize(Point currentPoint)
        {
            if (!_isResizing || _draggedTask == null || _draggedElement == null) return;
            
            var deltaX = currentPoint.X - _dragStartPoint.X;
            
            if (_draggedElement is Rectangle rect)
            {
                if (_activeResizeHandle == ResizeHandle.Left)
                {
                    var newLeft = Canvas.GetLeft(rect) + deltaX;
                    var newWidth = rect.Width - deltaX;
                    
                    if (newWidth > 10) // Minimum width
                    {
                        Canvas.SetLeft(rect, newLeft);
                        rect.Width = newWidth;
                    }
                }
                else if (_activeResizeHandle == ResizeHandle.Right)
                {
                    var newWidth = rect.Width + deltaX;
                    
                    if (newWidth > 10) // Minimum width
                    {
                        rect.Width = newWidth;
                    }
                }
            }
            
            _dragStartPoint = currentPoint;
        }

        private void EndResize(Point endPoint)
        {
            if (!_isResizing || _draggedTask == null) return;
            
            _isResizing = false;
            _canvas.ReleaseMouseCapture();
            
            // Calculate new task dates based on resize
            var newStartDate = CalculateDateFromPosition(endPoint);
            var newEndDate = _draggedTask.EndDate; // This would be calculated based on resize direction
            
            TaskResizeCompleted?.Invoke(this, new TaskResizeEventArgs
            {
                Task = _draggedTask,
                Element = _draggedElement,
                Handle = _activeResizeHandle,
                NewStartDate = newStartDate,
                NewEndDate = newEndDate
            });
            
            _draggedTask = null;
            _draggedElement = null;
            _activeResizeHandle = ResizeHandle.Left;
        }

        #endregion

        #region Utility Methods

        private bool ShouldStartDrag(Point position)
        {
            // Implement logic to determine if drag should start
            return true; // Simplified for example
        }

        private DateTime CalculateDateFromPosition(Point position)
        {
            // Convert pixel position to date - simplified calculation
            var pixelsPerDay = 60; // This should come from viewport configuration
            var dayOffset = (position.X - 200) / pixelsPerDay; // 200 is task list width
            return DateTime.Today.AddDays(dayOffset);
        }

        private void StartRectangleSelection(Point startPoint)
        {
            _selectionStartPoint = startPoint;
            _selectionRectangle = new Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Fill = new SolidColorBrush(Color.FromArgb(50, 0, 123, 255))
            };

            Canvas.SetLeft(_selectionRectangle, startPoint.X);
            Canvas.SetTop(_selectionRectangle, startPoint.Y);
            _canvas.Children.Add(_selectionRectangle);
        }

        private void UpdateRectangleSelection(Point currentPoint)
        {
            if (_selectionRectangle == null) return;

            var left = Math.Min(_selectionStartPoint.X, currentPoint.X);
            var top = Math.Min(_selectionStartPoint.Y, currentPoint.Y);
            var width = Math.Abs(currentPoint.X - _selectionStartPoint.X);
            var height = Math.Abs(currentPoint.Y - _selectionStartPoint.Y);

            Canvas.SetLeft(_selectionRectangle, left);
            Canvas.SetTop(_selectionRectangle, top);
            _selectionRectangle.Width = width;
            _selectionRectangle.Height = height;
        }

        private void EndRectangleSelection()
        {
            if (_selectionRectangle != null)
            {
                _canvas.Children.Remove(_selectionRectangle);
                
                // Find intersecting tasks
                var selectionBounds = new Rect(
                    Canvas.GetLeft(_selectionRectangle),
                    Canvas.GetTop(_selectionRectangle),
                    _selectionRectangle.Width,
                    _selectionRectangle.Height
                );

                SelectTasksInRectangle(selectionBounds);
                _selectionRectangle = null;
            }
        }

        private void SelectTasksInRectangle(Rect selectionRect)
        {
            ClearSelection();
            
            foreach (var kvp in _taskElements)
            {
                var element = kvp.Value;
                var elementBounds = new Rect(
                    Canvas.GetLeft(element),
                    Canvas.GetTop(element),
                    element.RenderSize.Width,
                    element.RenderSize.Height
                );

                if (selectionRect.IntersectsWith(elementBounds))
                {
                    var task = _elementTaskMap[element];
                    SelectTask(task, true);
                }
            }
        }

        #endregion

        #region Task Operations

        private void EditTask(EnhancedTaskItem task)
        {
            // Implement task editing logic
        }

        private void DeleteTask(EnhancedTaskItem task)
        {
            // Implement task deletion logic
        }

        private void MarkTaskComplete(EnhancedTaskItem task)
        {
            task.Status = Models.TaskStatus.Completed;
            task.Progress = 100;
        }

        #endregion
    }

    #region Event Args Classes

    public class TaskInteractionEventArgs : EventArgs
    {
        public EnhancedTaskItem Task { get; set; }
        public UIElement Element { get; set; }
    }

    public class TaskDragEventArgs : EventArgs
    {
        public EnhancedTaskItem Task { get; set; }
        public UIElement Element { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
    }

    public class TaskResizeEventArgs : EventArgs
    {
        public EnhancedTaskItem Task { get; set; }
        public UIElement Element { get; set; }
        public ResizeHandle Handle { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public List<EnhancedTaskItem> SelectedTasks { get; set; }
        public EnhancedTaskItem AddedTask { get; set; }
        public EnhancedTaskItem RemovedTask { get; set; }
    }

    public class ContextMenuRequestedEventArgs : EventArgs
    {
        public EnhancedTaskItem Task { get; set; }
        public UIElement Element { get; set; }
        public Point Position { get; set; }
        public ContextMenu ContextMenu { get; set; }
    }

    public class TooltipRequestedEventArgs : EventArgs
    {
        public EnhancedTaskItem Task { get; set; }
        public UIElement Element { get; set; }
        public Point Position { get; set; }
        public FrameworkElement Content { get; set; }
    }

    #endregion

    #region Enums

    public enum ResizeHandle
    {
        Left,
        Right
    }

    #endregion

    #region Helper Services

    public class ToolTipService
    {
        public TimeSpan ShowDelay { get; set; }
        public TimeSpan HideDelay { get; set; }

        public void ShowTooltip(UIElement target, FrameworkElement content, Point position)
        {
            // Implement tooltip display logic
        }

        public void HideTooltip()
        {
            // Implement tooltip hiding logic
        }
    }

    public class SelectionService
    {
        public bool MultiSelectEnabled { get; set; }
        public bool RectangleSelectEnabled { get; set; }
    }

    public class DragDropService
    {
        public double DragThreshold { get; set; }
        public bool ShowDragPreview { get; set; }
    }

    #endregion
}
