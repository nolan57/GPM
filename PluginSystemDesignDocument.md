# Gantt Project Management (GPM) Plugin System Design Document

## 1. System Overview

This design document describes the plugin system architecture for the Gantt Project Management software (GPM). The system allows users to extend the software's graphical annotation functionality through plugins. The plugin system uses interface-based design, supports multiple types of graphical annotations (text, shapes, lines, etc.), and allows users to customize the properties of these annotations.

## 2. Plugin System Architecture

### 2.1 Core Interface

The core of the plugin system is the `IAnnotationPlugin` interface, which defines the methods and properties that all graphical annotation plugins must implement:

```csharp
public interface IAnnotationPlugin
{
    string Name { get; }             // Plugin name
    string Description { get; }      // Plugin description
    AnnotationType Type { get; }     // Plugin type
    UserControl GetAnnotationControl();  // Get plugin configuration control
    UIElement CreateAnnotationElement(IAnnotationConfig config);  // Create annotation element
    void ConfigureAnnotation(UIElement element, IAnnotationConfig config);  // Configure annotation element
}
```

### 2.2 Plugin Types

The system supports the following types of graphical annotation plugins:

```csharp
public enum AnnotationType
{
    Text,       // Text annotation
    Shape,      // Shape annotation
    Image,      // Image annotation
    Line,       // Line annotation
    Connector,  // Connector annotation
    Highlight   // Highlight annotation
}
```

### 2.3 Configuration Interface

Each plugin requires a configuration object to define the annotation properties:

```csharp
public interface IAnnotationConfig
{
    string Text { get; set; }    // Text content
    Color Color { get; set; }    // Color
    double Opacity { get; set; } // Opacity
    double Size { get; set; }    // Size
}
```

## 3. Plugin Implementation

### 3.1 Text Annotation Plugin

`TextAnnotationPlugin` allows users to add text annotations to the Gantt chart. It includes the following features:

- Customizable text content
- Configurable font size, color, and style (bold, italic)
- Adjustable transparency

```csharp
public class TextAnnotationPlugin : IAnnotationPlugin
{
    public string Name => "Text Annotation";
    public string Description => "Create text annotations on the Gantt chart";
    public AnnotationType Type => AnnotationType.Text;
    
    // Implement interface methods...
}
```

### 3.2 Shape Annotation Plugin

`ShapeAnnotationPlugin` allows users to add various shapes as annotations on the Gantt chart. It supports the following shapes:

- Rectangle
- Circle
- Triangle
- Star
- Diamond

Each shape can be customized with color, border, size, and transparency.

### 3.3 Line Annotation Plugin

`LineAnnotationPlugin` allows users to draw various types of lines on the Gantt chart. It supports the following line types:

- Solid line
- Dashed line
- Dotted line
- Dash-dot line
- Dash-dot-dot line

Each line can be customized with color, thickness, and transparency.

## 4. Plugin Service

`PluginService` is the core service of the plugin system, responsible for managing and loading all available plugins:

```csharp
public class PluginService : IPluginService
{
    public List<IAnnotationPlugin> GetAnnotationPlugins()
    {
        var plugins = new List<IAnnotationPlugin>();
        
        // Load all available plugins
        plugins.Add(new TextAnnotationPlugin());
        plugins.Add(new ShapeAnnotationPlugin());
        plugins.Add(new LineAnnotationPlugin());
        
        return plugins;
    }
}
```

## 5. Plugin View Model

`PluginViewModel` serves as the bridge between the plugin service and the user interface, responsible for:

- Retrieving all available plugins from the plugin service
- Managing user-selected plugins
- Providing commands for creating annotations

```csharp
public class PluginViewModel : BindableBase
{
    // Property and command definitions
    
    private void CreateAnnotation(IAnnotationPlugin plugin)
    {
        // Create and configure annotation element
        // Add annotation element to Gantt chart
    }
}
```

## 6. Plugin User Interface

PluginView.xaml provides a user-friendly interface that allows users to:

- Browse and select available plugins
- View plugin description information
- Configure plugin properties
- Create graphical annotations

## 7. Usage Instructions

1. Select a plugin type from the plugin list
2. Set the plugin properties in the right configuration panel
3. Click the "Create Annotation" button
4. Select the position on the Gantt chart to place the annotation

## 8. Extension Guide

### 8.1 Creating New Plugins

To create a new plugin, you need to:

1. Create a class that implements the `IAnnotationPlugin` interface
2. Create a configuration class that inherits from `IAnnotationConfig` for the plugin
3. Register the new plugin in `PluginService`

### 8.2 Example: Creating a New Highlight Plugin

```csharp
public class HighlightConfig : IAnnotationConfig
{
    // Configuration properties
}

public class HighlightPlugin : IAnnotationPlugin
{
    // Implement interface methods
}
```

## 9. Future Plans

1. Implement dynamic plugin loading (from external DLLs)
2. Add more types of graphical annotation plugins
3. Enhance plugin configuration options
4. Implement plugin persistence (save and load plugin configurations)
5. Add plugin sorting and categorization functionality