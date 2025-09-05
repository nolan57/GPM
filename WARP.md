# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

GPM (Gantt Project Management) is a WPF-based Gantt chart project management application built with C# targeting .NET 8. The application features a modular plugin architecture that allows for extensible annotation functionality on Gantt charts. Key features include project/task management, time scaling, and a sophisticated plugin system for graphical annotations.

## Development Commands

### Building the Project
```powershell
# Build the solution
dotnet build GPM/GPM.sln

# Build in release mode
dotnet build GPM/GPM.sln --configuration Release

# Clean and rebuild
dotnet clean GPM/GPM.sln
dotnet build GPM/GPM.sln
```

### Running the Application
```powershell
# Run from the GPM project directory
cd GPM
dotnet run

# Or run the built executable
dotnet run --project GPM.csproj
```

### Database Operations
```powershell
# The application uses SQLite with Entity Framework Core
# Database is automatically initialized on first run via DatabaseInitializer.Initialize()
# Database file location: GPM/GPM.db
```

### Project Management
```powershell
# Restore NuGet packages
dotnet restore GPM/GPM.sln

# Update packages (be careful with major version changes)
dotnet list GPM/GPM.csproj package --outdated
```

## Architecture Overview

### High-Level Architecture
The application follows **MVVM pattern with Prism framework** for dependency injection and region navigation. Key architectural decisions:

- **Pattern**: MVVM (Model-View-ViewModel) with Prism framework
- **DI Container**: Unity (via Prism.Unity)  
- **Database**: SQLite with Entity Framework Core
- **UI Framework**: WPF with Material Design themes
- **Plugin Architecture**: Interface-based with MEF-style loading

### Core Components

#### 1. Data Layer (`Models/`)
- **ApplicationDbContext**: EF Core DbContext managing Projects, Tasks, and SubTasks
- **Project**: Root entity with Tasks collection
- **Task**: Main work unit with SubTasks and progress tracking  
- **SubTask**: Granular work breakdown
- **NavigationItem**: UI navigation model for menu items

#### 2. Service Layer (`Services/`)
- **IProjectService/ProjectService**: Project CRUD operations
- **ITaskService/TaskService**: Task management and progress tracking
- **IPluginService/PluginService**: Plugin discovery and loading
- **DatabaseInitializer**: Handles database creation and seeding

#### 3. View Layer (`Views/`)
- **MainWindow**: Shell with navigation sidebar and content region
- **ProjectView**: Project list and management interface
- **TaskView**: Task creation and editing interface
- **GanttChartView**: Main Gantt chart visualization (custom implementation)
- **PluginView**: Plugin selection and annotation interface

#### 4. ViewModel Layer (`ViewModels/`)
- **MainViewModel**: Navigation and shell coordination
- **ProjectViewModel**: Project management logic
- **TaskViewModel**: Task operations
- **GanttChartViewModel**: Gantt chart data binding and interactions
- **PluginViewModel**: Plugin system coordination

#### 5. Plugin System (`Plugins/`)
- **IAnnotationPlugin**: Core plugin interface
- **TextAnnotationPlugin**: Text-based annotations
- **ShapeAnnotationPlugin**: Geometric shape annotations  
- **LineAnnotationPlugin**: Line and connector annotations
- Supports 6 annotation types: Text, Shape, Image, Line, Connector, Highlight

#### 6. Supporting Components
- **Converters/**: XAML value converters (BoolToBrushConverter, SubtractConverter)
- **Regions.cs**: Prism region definitions for navigation
- **App.xaml.cs**: Application bootstrapping and DI registration

### Navigation Architecture
The application uses **Prism Region Navigation** with a shell-based layout:
- Left sidebar for navigation menu
- Main content region for views (ProjectView, TaskView, GanttChartView, PluginView)
- Navigation handled through MainViewModel with NavigateCommand

### Plugin Architecture Details
The plugin system is **interface-based and extensible**:
- Plugins implement `IAnnotationPlugin` interface
- Support for multiple annotation types through `AnnotationType` enum
- Configuration through `IAnnotationConfig` interface
- Runtime discovery and loading through `PluginService`
- UI integration through dedicated `PluginView` and `PluginViewModel`

### Database Schema
- **Projects**: Core project entity with name, description, and date range
- **Tasks**: Work items linked to projects with progress tracking
- **SubTasks**: Granular breakdown of tasks
- **Relationships**: Project -> Tasks (1:many), Task -> SubTasks (1:many)
- **Storage**: Local SQLite database (GPM.db)

## Key Dependencies

### Core Framework
- **.NET 8 Windows**: `<TargetFramework>net8.0-windows</TargetFramework>`
- **WPF**: `<UseWPF>true</UseWPF>`

### NuGet Packages
- **Prism.Wpf** (9.0.537): MVVM framework and navigation
- **Prism.Unity** (9.0.537): Dependency injection container
- **MaterialDesignThemes** (5.2.1): Material Design UI components
- **Microsoft.EntityFrameworkCore** (9.0.8): Data access layer
- **Microsoft.EntityFrameworkCore.Sqlite** (9.0.8): SQLite provider
- **AutoMapper** (15.0.1): Object-to-object mapping
- **Newtonsoft.Json** (13.0.3): JSON serialization

## Development Notes

### Current Development Phase
According to the project documentation, the application is in **Phase 1: Basic Architecture and Core Visualization** with several completed features:
- ✅ Core visualization components implemented
- ✅ Basic time granularity switching
- ✅ Task bar rendering and layout
- ✅ Hierarchical task display support
- 🔄 Progress indicators and MVVM refinements ongoing

### Time Scaling Feature
The application includes sophisticated **time scaling functionality**:
- Support for multiple time granularities (day/week/month/quarter/year)
- Local time period scaling for expanded detail in specific regions  
- Time transformation between scaled and actual coordinates
- Visual indicators for scaled time periods

### Plugin Development Guidelines
When extending the plugin system:
1. Implement `IAnnotationPlugin` interface
2. Create corresponding configuration class inheriting `IAnnotationConfig`
3. Register plugin in `PluginService.GetAnnotationPlugins()`
4. Follow the annotation type enum for categorization
5. Ensure UI controls are provided via `GetAnnotationControl()` method

### Code Organization Principles
- **Separation of Concerns**: Clear separation between Models, Views, ViewModels, and Services
- **Dependency Injection**: All services registered in App.xaml.cs container registration
- **Interface-Based Design**: Services and plugins use interfaces for extensibility
- **Region-Based Navigation**: Prism regions enable modular view composition
- **Entity Framework Conventions**: Follow EF Core patterns for data access

## Project Structure Context

The root directory contains both the source code (`GPM/` folder) and comprehensive documentation:
- **ProjectDevelopmentGuide.md**: Detailed development methodology and phase breakdown
- **PluginSystemDesignDocument.md**: Complete plugin architecture specification
- **GanttChartCustomizationGuide.md**: UI customization guidelines  
- **TimeScalingFeatureDocumentation.md**: Time scaling implementation details

This dual structure (code + documentation) reflects active development with thorough documentation practices.
