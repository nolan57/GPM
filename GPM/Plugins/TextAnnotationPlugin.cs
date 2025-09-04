using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace GPM.Plugins
{
    /// <summary>
    /// 文本注释配置类
    /// </summary>
    public class TextAnnotationConfig : IAnnotationConfig
    {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Colors.Black;
        public double Opacity { get; set; } = 1.0;
        public double Size { get; set; } = 12;
        // 文本注释特有的配置
        public FontFamily FontFamily { get; set; } = new FontFamily("Arial");
        public bool IsBold { get; set; } = false;
        public bool IsItalic { get; set; } = false;
    }

    /// <summary>
    /// 文本注释插件
    /// </summary>
    public class TextAnnotationPlugin : IAnnotationPlugin
    {
        public string Name => "Text Annotation";
        public string Description => "Create text annotations on the Gantt chart";
        public AnnotationType Type => AnnotationType.Text;

        public UserControl GetAnnotationControl()
        {
            var control = new UserControl();
            control.Width = 300;
            control.Height = 200;
            
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            
            var header = new TextBlock
            {
                Text = "Text Annotation Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            var textBlock = new TextBlock { Text = "Annotation Text:" };
            var textBox = new TextBox
            {
                Text = "This is a text annotation",
                Margin = new Thickness(0, 5, 0, 10)
            };
            
            stackPanel.Children.Add(header);
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            
            control.Content = stackPanel;
            return control;
        }

        public UIElement CreateAnnotationElement(IAnnotationConfig config)
        {
            var textConfig = config as TextAnnotationConfig ?? new TextAnnotationConfig();
            var textBlock = new TextBlock
            {
                Text = textConfig.Text,
                FontSize = textConfig.Size,
                Foreground = new SolidColorBrush(textConfig.Color),
                Opacity = textConfig.Opacity,
                FontFamily = textConfig.FontFamily,
                FontWeight = textConfig.IsBold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = textConfig.IsItalic ? FontStyles.Italic : FontStyles.Normal,
                Padding = new Thickness(5),
                Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
                TextWrapping = TextWrapping.Wrap
            };
            
            return textBlock;
        }

        public void ConfigureAnnotation(UIElement element, IAnnotationConfig config)
        {
            if (element is TextBlock textBlock && config is TextAnnotationConfig textConfig)
            {
                textBlock.Text = textConfig.Text;
                textBlock.FontSize = textConfig.Size;
                textBlock.Foreground = new SolidColorBrush(textConfig.Color);
                textBlock.Opacity = textConfig.Opacity;
                textBlock.FontFamily = textConfig.FontFamily;
                textBlock.FontWeight = textConfig.IsBold ? FontWeights.Bold : FontWeights.Normal;
                textBlock.FontStyle = textConfig.IsItalic ? FontStyles.Italic : FontStyles.Normal;
            }
        }
    }
}