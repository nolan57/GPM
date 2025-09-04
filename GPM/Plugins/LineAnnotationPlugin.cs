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
    /// 线条类型枚举
    /// </summary>
    public enum LineType
    {
        Solid,
        Dashed,
        Dotted,
        DashDot,
        DashDotDot
    }

    /// <summary>
    /// 线条注释配置类
    /// </summary>
    public class LineAnnotationConfig : IAnnotationConfig
    {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Colors.Red;
        public double Opacity { get; set; } = 1.0;
        public double Size { get; set; } = 2.0; // 线条粗细
        // 线条特有的配置
        public LineType LineType { get; set; } = LineType.Solid;
        public Point StartPoint { get; set; } = new Point(0, 0);
        public Point EndPoint { get; set; } = new Point(100, 0);
    }

    /// <summary>
    /// 线条注释插件
    /// </summary>
    public class LineAnnotationPlugin : IAnnotationPlugin
    {
        public string Name => "Line Annotation";
        public string Description => "Create line annotations on the Gantt chart";
        public AnnotationType Type => AnnotationType.Line;

        public UserControl GetAnnotationControl()
        {
            var control = new UserControl();
            control.Width = 300;
            control.Height = 250;
            
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            
            var header = new TextBlock
            {
                Text = "Line Annotation Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            // 线条类型选择
            var lineTypeBlock = new TextBlock { Text = "Line Type:" };
            var lineTypeComboBox = new ComboBox
            {
                ItemsSource = Enum.GetValues(typeof(LineType)),
                SelectedItem = LineType.Solid,
                Margin = new Thickness(0, 5, 0, 10)
            };
            
            // 线条粗细设置
            var thicknessBlock = new TextBlock { Text = "Line Thickness:" };
            var thicknessSlider = new Slider
            {
                Minimum = 1,
                Maximum = 10,
                Value = 2,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 5, 0, 10)
            };
            
            stackPanel.Children.Add(header);
            stackPanel.Children.Add(lineTypeBlock);
            stackPanel.Children.Add(lineTypeComboBox);
            stackPanel.Children.Add(thicknessBlock);
            stackPanel.Children.Add(thicknessSlider);
            
            control.Content = stackPanel;
            return control;
        }

        public UIElement CreateAnnotationElement(IAnnotationConfig config)
        {
            var lineConfig = config as LineAnnotationConfig ?? new LineAnnotationConfig();
            var line = new Line
            {
                X1 = lineConfig.StartPoint.X,
                Y1 = lineConfig.StartPoint.Y,
                X2 = lineConfig.EndPoint.X,
                Y2 = lineConfig.EndPoint.Y,
                Stroke = new SolidColorBrush(lineConfig.Color),
                StrokeThickness = lineConfig.Size,
                Opacity = lineConfig.Opacity,
                StrokeDashArray = GetDashArray(lineConfig.LineType)
            };
            
            return line;
        }

        public void ConfigureAnnotation(UIElement element, IAnnotationConfig config)
        {
            if (element is Line line && config is LineAnnotationConfig lineConfig)
            {
                line.Stroke = new SolidColorBrush(lineConfig.Color);
                line.StrokeThickness = lineConfig.Size;
                line.Opacity = lineConfig.Opacity;
                line.StrokeDashArray = GetDashArray(lineConfig.LineType);
                line.X1 = lineConfig.StartPoint.X;
                line.Y1 = lineConfig.StartPoint.Y;
                line.X2 = lineConfig.EndPoint.X;
                line.Y2 = lineConfig.EndPoint.Y;
            }
        }

        /// <summary>
        /// 根据线条类型获取对应的虚线样式
        /// </summary>
        private DoubleCollection GetDashArray(LineType lineType)
        {
            switch (lineType)
            {
                case LineType.Dashed:
                    return new DoubleCollection { 5, 5 };
                case LineType.Dotted:
                    return new DoubleCollection { 2, 2 };
                case LineType.DashDot:
                    return new DoubleCollection { 5, 2, 2, 2 };
                case LineType.DashDotDot:
                    return new DoubleCollection { 5, 2, 2, 2, 2, 2 };
                case LineType.Solid:
                default:
                    return null;
            }
        }
    }
}