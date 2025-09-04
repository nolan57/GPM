using GPM.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GPM.Services
{
    public class PluginService : IPluginService
    {
        public List<IAnnotationPlugin> GetAnnotationPlugins()
        {
            var plugins = new List<IAnnotationPlugin>();
            
            // 在实际应用中，这里会动态加载插件
            // 目前我们手动添加已知的插件类型
            plugins.Add(new TextAnnotationPlugin());
            plugins.Add(new ShapeAnnotationPlugin());
            plugins.Add(new LineAnnotationPlugin());
            
            return plugins;
        }
    }
}