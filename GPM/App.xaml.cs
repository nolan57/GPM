using Prism.Ioc;
using Prism.Unity;
using Prism.Navigation.Regions;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using GPM.Views;
using GPM.ViewModels;
using GPM.Services;

namespace GPM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            // 初始化数据库
            DatabaseInitializer.Initialize();
            
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册视图和视图模型以支持导航
            containerRegistry.RegisterForNavigation<ProjectView, ProjectViewModel>();
            containerRegistry.RegisterForNavigation<TaskView, TaskViewModel>();
            containerRegistry.RegisterForNavigation<GanttChartView, GanttChartViewModel>();
            containerRegistry.RegisterForNavigation<PluginView, PluginViewModel>();
            
            // 注册服务
            containerRegistry.Register<IProjectService, ProjectService>();
            containerRegistry.Register<ITaskService, TaskService>();
            containerRegistry.Register<IPluginService, PluginService>();
        }
    }

}
