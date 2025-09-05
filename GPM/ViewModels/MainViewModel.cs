using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPM.Services;
using Prism.Commands;
using GPM.Models;
using GPM.Views;
using GPM;

namespace GPM.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IProjectService _projectService;
        private readonly IPluginService _pluginService;
        private readonly IRegionManager _regionManager;

        private string _title = "GPM -甘特图项目管理软件";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private List<NavigationItem> _navigationItems = new();
        public List<NavigationItem> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref _navigationItems, value); }
        }

        private NavigationItem _selectedNavigationItem;
        public NavigationItem SelectedNavigationItem
        {
            get { return _selectedNavigationItem; }
            set { SetProperty(ref _selectedNavigationItem, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public MainViewModel(IProjectService projectService, ITaskService taskService, IPluginService pluginService, 
                            IRegionManager regionManager)
        {
            _projectService = projectService;
            _pluginService = pluginService;
            _regionManager = regionManager;

            // 初始化导航命令
            NavigateCommand = new DelegateCommand<string>(Navigate);

            // 初始化导航菜单项
            InitializeNavigationItems();

            // 默认导航到项目视图
            Navigate("ProjectView");
        }

        private void InitializeNavigationItems()
        {
            NavigationItems = new List<NavigationItem>
            {
                new NavigationItem("项目管理", "ProjectView"),
                new NavigationItem("任务管理", "TaskView"),
                new NavigationItem("甘特图", "GanttChartView"),
                new NavigationItem("插件管理", "PluginView")
            };
        }

        private void Navigate(string viewName)
        {
            // 更新选中状态
            foreach (var item in NavigationItems)
            {
                item.IsSelected = item.ViewName == viewName;
            }

            // 导航到指定视图
            _regionManager.RequestNavigate(Regions.MainContentRegion, viewName);
        }
    }
}