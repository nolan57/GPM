# Time Scaling Feature Detailed Documentation

## Feature Overview

The time scaling feature allows users to select specific time periods in the Gantt chart and expand them into finer time units while keeping other areas at their original time units. This provides detailed analysis capability for critical project phases without losing the overall project context.

## Technical Architecture

### 1. Data Model (TimeScalingRegion.cs)

```csharp
public class TimeScalingRegion
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan OriginalDuration => EndDate - StartDate;
    public TimeSpan ScaledDuration { get; set; }
    public double ScaleFactor => ScaledDuration.TotalMilliseconds / OriginalDuration.TotalMilliseconds;
    public TimeUnit OriginalUnit { get; set; }
    public TimeUnit ScaledUnit { get; set; }
    public int ProjectId { get; set; }
    public bool IsExpanded { get; set; }
}

public enum TimeUnit
{
    Days,
    Weeks,
    Months,
    Years
}
```

### 2. Service Layer (TimeScalingService.cs)

Core service interface and implementation:
- `DateTime ScaleTime(DateTime originalTime, List<TimeScalingRegion> scalingRegions)` - Time forward transformation
- `DateTime ReverseScaleTime(DateTime scaledTime, List<TimeScalingRegion> scalingRegions)` - Time reverse transformation
- `double GetTimePosition(...)` - Get time position on timeline
- `void ExpandRegion(TimeScalingRegion region, TimeUnit targetUnit)` - Expand region
- `void CollapseRegion(TimeScalingRegion region)` - Collapse region

### 3. ViewModel Enhancements (GanttChartViewModel.cs)

New features:
- `ObservableCollection<TimeScalingRegion> ScalingRegions` - Scaling regions collection
- `ICommand ExpandRegionCommand` - Expand region command
- `ICommand CollapseRegionCommand` - Collapse region command
- `ICommand SelectTimeRegionCommand` - Select time region command

### 4. User Interface (GanttChartView.xaml)

New controls:
- Time scaling toolbar buttons (expand to days/weeks/months/years, collapse region)
- Scaling region selection dropdown
- Visual indicators on timeline (color-coded rectangles)
- Status bar information

## Implementation Details

### Time Transformation Algorithm

Time scaling uses a piecewise linear transformation algorithm:
1. Calculate time offset for each scaling region
2. Adjust time span within regions based on scaling factor
3. Maintain time relationships in non-scaled regions unchanged
4. Support multiple concurrent scaling regions

### Visual Indicators

- **Color Coding**: Use different colors to distinguish scaling states (expanded/collapsed)
- **Transparency Effects**: Semi-transparent overlay showing scaling region boundaries
- **Tooltips**: Display scaling details (original unit → target unit)
- **Interaction Feedback**: Highlight effects for mouse hover and selection states

### User Interaction Flow

1. **Select Time Region**: User selects area on Gantt chart timeline via mouse drag
2. **Choose Scaling Level**: Click toolbar buttons to select target time unit (days/weeks/months/years)
3. **Apply Scaling**: System calculates scaling parameters and updates Gantt chart display
4. **Manage Scaling Regions**: Select and manage multiple scaling regions via dropdown
5. **Restore Original Scale**: Use collapse button to restore original time unit for selected region

## Database Design

### TimeScalingRegions Table

| Field Name | Type | Description |
|------------|------|-------------|
| Id | int | Primary key |
| StartDate | datetime | Region start time |
| EndDate | datetime | Region end time |
| ScaledDuration | bigint | Scaled duration (milliseconds) |
| OriginalUnit | int | Original time unit |
| ScaledUnit | int | Scaled time unit |
| ProjectId | int | Associated project ID |
| IsExpanded | bit | Whether in expanded state |

## Dependency Injection Configuration

Register services in App.xaml.cs:

```csharp
containerRegistry.RegisterSingleton<ITimeScalingService, TimeScalingService>();
containerRegistry.RegisterForNavigation<GanttChartView, GanttChartViewModel>();
```

## Test Cases

### Functional Testing
1. Create time scaling regions and verify time conversion accuracy
2. Test interaction effects of multiple concurrent scaling regions
3. Verify scaling state persistence and recovery functionality
4. Test boundary conditions (minimum/maximum time ranges)

### Performance Testing
1. Processing performance with large-scale time scaling regions
2. Response time for real-time interactions
3. Memory usage monitoring

## Compatibility Considerations

- Integration with existing plugin system
- Synchronization mechanism with collaboration features
- Backward compatibility guarantee

## Future Extensions

1. **Nested Scaling**: Support further scaling within already scaled regions
2. **Automatic Scaling**: Automatically suggest scaling regions based on project critical path
3. **Preset Templates**: Save and load commonly used scaling configurations
4. **Advanced Visualization**: 3D timeline and dynamic scaling effects

## Version History

- v1.0: Basic time scaling functionality implementation
- v1.1: Multi-region concurrent support
- v1.2: Visualization enhancements and performance optimization

---

*This document last updated: 2024*