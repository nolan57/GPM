using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPM.Plugins;
using GPM.Services;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Controls;
using System.Windows;

namespace GPM.ViewModels
{
    public class PluginViewModel : BindableBase
    {
        private readonly IPluginService _pluginService;

        private List<IAnnotationPlugin> _annotationPlugins = null!;
        public List<IAnnotationPlugin> AnnotationPlugins
        {
            get { return _annotationPlugins; }
            set { SetProperty(ref _annotationPlugins, value); }
        }

        private IAnnotationPlugin _selectedPlugin;
        public IAnnotationPlugin SelectedPlugin
        {
            get { return _selectedPlugin; }
            set 
            {
                SetProperty(ref _selectedPlugin, value);
                // 当选择插件时，更新插件控件
                if (value != null)
                {
                    SelectedPluginControl = value.GetAnnotationControl();
                }
            }
        }

        private UserControl _selectedPluginControl;
        public UserControl SelectedPluginControl
        {
            get { return _selectedPluginControl; }
            set { SetProperty(ref _selectedPluginControl, value); }
        }

        // 创建标识命令
        public DelegateCommand<IAnnotationPlugin> CreateAnnotationCommand { get; private set; }

        public PluginViewModel(IPluginService pluginService)
        {
            _pluginService = pluginService;
            AnnotationPlugins = _pluginService.GetAnnotationPlugins();
            
            // 初始化命令
            CreateAnnotationCommand = new DelegateCommand<IAnnotationPlugin>(CreateAnnotation);
        }

        private void CreateAnnotation(IAnnotationPlugin plugin)
        {
            if (plugin == null)
            {
                MessageBox.Show("请先选择一个插件");
                return;
            }

            // 创建插件的配置对象
            IAnnotationConfig config = CreateDefaultConfig(plugin.Type);
            
            // 创建注释元素
            var annotationElement = plugin.CreateAnnotationElement(config);
            
            // 这里应该将注释元素添加到甘特图上
            // 由于我们还没有实现甘特图，所以暂时显示一个消息
            MessageBox.Show($"已创建{plugin.Name}类型的标识");
        }

        /// <summary>
        /// 根据注释类型创建默认配置
        /// </summary>
        private IAnnotationConfig CreateDefaultConfig(AnnotationType type)
        {
            switch (type)
            {
                case AnnotationType.Text:
                    return new TextAnnotationConfig { Text = "新文本注释" };
                case AnnotationType.Shape:
                    return new ShapeAnnotationConfig();
                case AnnotationType.Line:
                    return new LineAnnotationConfig();
                default:
                    return null;
            }
        }
    }
}