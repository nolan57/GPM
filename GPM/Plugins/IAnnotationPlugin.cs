using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GPM.Plugins
{
    /// <summary>
    /// 图形标识类型枚举
    /// </summary>
    public enum AnnotationType
    {
        Text,
        Shape,
        Image,
        Line,
        Connector,
        Highlight
    }

    /// <summary>
    /// 注释配置接口
    /// </summary>
    public interface IAnnotationConfig
    {
        string Text { get; set; }
        Color Color { get; set; }
        double Opacity { get; set; }
        double Size { get; set; }
        // 可以根据需要添加更多配置项
    }

    /// <summary>
    /// 图形标识插件接口
    /// </summary>
    public interface IAnnotationPlugin
    {
        string Name { get; }
        string Description { get; }
        AnnotationType Type { get; }
        UserControl GetAnnotationControl();
        UIElement CreateAnnotationElement(IAnnotationConfig config);
        void ConfigureAnnotation(UIElement element, IAnnotationConfig config);
    }
}