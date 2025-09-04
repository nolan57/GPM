# Gantt Chart Component Customization Guide

## Overview

The GanttChartView component now provides extensive customizable properties, allowing developers to customize the appearance and behavior of the Gantt chart according to project requirements.

## Available Properties List

### Time Scale Style Properties
- `TimeScaleBackground` (Brush): Time scale background color
- `TimeScaleForeground` (Brush): Time scale text and line color  
- `TimeScaleFontSize` (double): Time scale text size
- `DefaultTimeScale` (string): Default time granularity (Day/Week/Month/Quarter)

### Grid Line Style Properties
- `GridLineColor` (Color): Grid line color
- `GridLineThickness` (double): Grid line thickness
- `GridLineDashArray` (DoubleCollection): Grid line dash pattern
- `HorizontalGridLineSpacing` (double): Horizontal grid line spacing

### Task Bar Style Properties
- `TaskBarCornerRadius` (double): Task bar corner radius
- `TaskBarBorderThickness` (double): Task bar border thickness
- `TaskBarBorderColor` (Color): Task bar border color
- `TaskLabelFontSize` (double): Task label text size
- `TaskLabelForeground` (Brush): Task label text color
- `TaskLabelFontWeight` (FontWeight): Task label text weight

### Canvas Dimension Properties
- `CanvasWidth` (double): Canvas width
- `CanvasHeight` (double): Canvas height
- `CanvasBackground` (Brush): Canvas background color

### Hierarchy Display Properties
- `ShowTaskHierarchy` (bool): Whether to display tasks in hierarchical structure
- `HierarchyIndentation` (double): Indentation width for child tasks
- `ParentTaskBackground` (Color): Background color for parent tasks
- `ChildTaskBackground` (Color): Background color for child tasks
- `ParentTaskHeight` (double): Height of parent task bars
- `ChildTaskHeight` (double): Height of child task bars

## Usage Examples

### Using in XAML
```xml
<views:GanttChartView
    TimeScaleBackground="LightBlue"
    TimeScaleForeground="DarkBlue"
    TimeScaleFontSize="12"
    DefaultTimeScale="Week"
    GridLineColor="Gray"
    GridLineThickness="1"
    GridLineDashArray="3,3"
    HorizontalGridLineSpacing="40"
    TaskBarCornerRadius="8"
    TaskBarBorderThickness="2"
    TaskBarBorderColor="DarkGray"
    TaskLabelFontSize="12"
    TaskLabelForeground="Black"
    TaskLabelFontWeight="Normal"
    CanvasWidth="2500"
    CanvasHeight="800"
    CanvasBackground="WhiteSmoke"/>
```

### Using in C# Code
```csharp
var ganttChart = new GanttChartView
{
    TimeScaleBackground = Brushes.LightBlue,
    TimeScaleForeground = Brushes.DarkBlue,
    TimeScaleFontSize = 12,
    DefaultTimeScale = "Week",
    GridLineColor = Colors.Gray,
    GridLineThickness = 1,
    GridLineDashArray = new DoubleCollection { 3, 3 },
    HorizontalGridLineSpacing = 40,
    TaskBarCornerRadius = 8,
    TaskBarBorderThickness = 2,
    TaskBarBorderColor = Colors.DarkGray,
    TaskLabelFontSize = 12,
    TaskLabelForeground = Brushes.Black,
    TaskLabelFontWeight = FontWeights.Normal,
    CanvasWidth = 2500,
    CanvasHeight = 800,
    CanvasBackground = Brushes.WhiteSmoke
};
```

### Dynamic Property Modification
```csharp
// Modify time scale style
ganttChart.TimeScaleBackground = Brushes.LightGreen;
ganttChart.TimeScaleForeground = Brushes.DarkGreen;

// Modify grid line style
ganttChart.GridLineColor = Colors.LightGray;
ganttChart.GridLineDashArray = new DoubleCollection { 4, 2 };

// Modify task bar style
ganttChart.TaskBarCornerRadius = 10;
ganttChart.TaskLabelFontSize = 14;
```csharp
// Re-render to apply changes
ganttChart.InitializeTimeScale();

// Enable hierarchical display
ganttChart.ShowTaskHierarchy = true;
ganttChart.HierarchyIndentation = 25;
ganttChart.ParentTaskBackground = Colors.DarkBlue;
ganttChart.ChildTaskBackground = Colors.LightBlue;
ganttChart.ParentTaskHeight = 35;
ganttChart.ChildTaskHeight = 25;

// Refresh task display
ganttChart.RedrawTasks();
```

## Best Practices

1. **Performance Optimization**: For large Gantt charts, it's recommended to call the `InitializeTimeScale()` method once after modifying multiple properties, rather than re-rendering after each property change.
2. **Style Consistency**: Maintain consistency in the styles of time scales, grid lines, and task bars to ensure a coordinated visual effect.
3. **Responsive Design**: Consider adjusting canvas dimensions and font sizes for different screen sizes to ensure good user experience.
4. **Default Value Settings**: It's recommended to set default property values when the application starts to ensure all Gantt chart components have a unified appearance.
5. **Testing Validation**: Before deployment, test the effects of different property combinations to ensure proper display under various configurations.

## Compatibility Notes

- All properties support data binding and style settings
- Property changes automatically trigger UI updates (requires calling corresponding rendering methods)
- Fully compatible with WPF standard controls
- Supports MVVM pattern development

## Version History

- v1.0 (2024): Initial version, providing basic custom property support

## Technical Support

For issues or suggestions, please contact the development team or refer to relevant documentation.