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
    /// 形状类型枚举
    /// </summary>
    public enum ShapeType
    {
        Rectangle,
        Circle,
        Triangle,
        Star,
        Diamond
    }

    /// <summary>
    /// 形状注释配置类
    /// </summary>
    public class ShapeAnnotationConfig : IAnnotationConfig
    {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Colors.Yellow;
        public double Opacity { get; set; } = 0.7;
        public double Size { get; set; } = 40;
        // 形状特有的配置
        public ShapeType ShapeType { get; set; } = ShapeType.Rectangle;
        public Color BorderColor { get; set; } = Colors.Black;
        public double BorderThickness { get; set; } = 1.0;
        public bool IsFilled { get; set; } = true;
    }

    /// <summary>
    /// 形状注释插件
    /// </summary>
    public class ShapeAnnotationPlugin : IAnnotationPlugin
    {
        public string Name => "Shape Annotation";
        public string Description => "Create shape annotations on the Gantt chart";
        public AnnotationType Type => AnnotationType.Shape;

        public UserControl GetAnnotationControl()
        {
            var control = new UserControl();
            control.Width = 300;
            control.Height = 300;
            
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            
            var header = new TextBlock
            {
                Text = "Shape Annotation Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            // 形状选择
            var shapeTypeBlock = new TextBlock { Text = "Shape Type:" };
            var shapeTypeComboBox = new ComboBox
            {
                ItemsSource = Enum.GetValues(typeof(ShapeType)),
                SelectedItem = ShapeType.Rectangle,
                Margin = new Thickness(0, 5, 0, 10)
            };
            
            // 大小设置
            var sizeBlock = new TextBlock { Text = "Size:" };
            var sizeSlider = new Slider
            {
                Minimum = 10,
                Maximum = 100,
                Value = 40,
                TickFrequency = 10,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 5, 0, 10)
            };
            
            stackPanel.Children.Add(header);
            stackPanel.Children.Add(shapeTypeBlock);
            stackPanel.Children.Add(shapeTypeComboBox);
            stackPanel.Children.Add(sizeBlock);
            stackPanel.Children.Add(sizeSlider);
            
            control.Content = stackPanel;
            return control;
        }

        public UIElement CreateAnnotationElement(IAnnotationConfig config)
        {
            var shapeConfig = config as ShapeAnnotationConfig ?? new ShapeAnnotationConfig();
            Shape shape = null;
            
            switch (shapeConfig.ShapeType)
            {
                case ShapeType.Rectangle:
                    shape = new Rectangle
                    {
                        Width = shapeConfig.Size,
                        Height = shapeConfig.Size
                    };
                    break;
                case ShapeType.Circle:
                    shape = new Ellipse
                    {
                        Width = shapeConfig.Size,
                        Height = shapeConfig.Size
                    };
                    break;
                case ShapeType.Triangle:
                    // 创建三角形
                    var triangle = new Path();
                    var triangleGeometry = new PathGeometry();
                    var figure = new PathFigure
                    {
                        StartPoint = new Point(shapeConfig.Size / 2, 0)
                    };
                    figure.Segments.Add(new LineSegment(new Point(shapeConfig.Size, shapeConfig.Size), true));
                    figure.Segments.Add(new LineSegment(new Point(0, shapeConfig.Size), true));
                    figure.IsClosed = true;
                    triangleGeometry.Figures.Add(figure);
                    triangle.Data = triangleGeometry;
                    shape = triangle;
                    break;
                case ShapeType.Star:
                    // 创建五角星（简化版）
                    var star = new Path();
                    var starGeometry = new PathGeometry();
                    var starFigure = new PathFigure();
                    // 这里可以添加更复杂的星形绘制逻辑
                    starFigure.StartPoint = new Point(shapeConfig.Size / 2, 0);
                    starFigure.Segments.Add(new LineSegment(new Point(shapeConfig.Size, shapeConfig.Size), true));
                    starFigure.Segments.Add(new LineSegment(new Point(0, shapeConfig.Size / 2), true));
                    starFigure.Segments.Add(new LineSegment(new Point(shapeConfig.Size, shapeConfig.Size / 2), true));
                    starFigure.Segments.Add(new LineSegment(new Point(0, shapeConfig.Size), true));
                    starFigure.IsClosed = true;
                    starGeometry.Figures.Add(starFigure);
                    star.Data = starGeometry;
                    shape = star;
                    break;
                case ShapeType.Diamond:
                    var diamond = new Path();
                    var diamondGeometry = new PathGeometry();
                    var diamondFigure = new PathFigure
                    {
                        StartPoint = new Point(shapeConfig.Size / 2, 0)
                    };
                    diamondFigure.Segments.Add(new LineSegment(new Point(shapeConfig.Size, shapeConfig.Size / 2), true));
                    diamondFigure.Segments.Add(new LineSegment(new Point(shapeConfig.Size / 2, shapeConfig.Size), true));
                    diamondFigure.Segments.Add(new LineSegment(new Point(0, shapeConfig.Size / 2), true));
                    diamondFigure.IsClosed = true;
                    diamondGeometry.Figures.Add(diamondFigure);
                    diamond.Data = diamondGeometry;
                    shape = diamond;
                    break;
            }
            
            if (shape != null)
            {
                shape.Fill = shapeConfig.IsFilled ? new SolidColorBrush(shapeConfig.Color) : Brushes.Transparent;
                shape.Stroke = new SolidColorBrush(shapeConfig.BorderColor);
                shape.StrokeThickness = shapeConfig.BorderThickness;
                shape.Opacity = shapeConfig.Opacity;
            }
            
            return shape;
        }

        public void ConfigureAnnotation(UIElement element, IAnnotationConfig config)
        {
            if (element is Shape shape && config is ShapeAnnotationConfig shapeConfig)
            {
                shape.Fill = shapeConfig.IsFilled ? new SolidColorBrush(shapeConfig.Color) : Brushes.Transparent;
                shape.Stroke = new SolidColorBrush(shapeConfig.BorderColor);
                shape.StrokeThickness = shapeConfig.BorderThickness;
                shape.Opacity = shapeConfig.Opacity;
                
                // 根据形状类型设置大小
                if (shape is Rectangle rectangle)
                {
                    rectangle.Width = shapeConfig.Size;
                    rectangle.Height = shapeConfig.Size;
                }
                else if (shape is Ellipse ellipse)
                {
                    ellipse.Width = shapeConfig.Size;
                    ellipse.Height = shapeConfig.Size;
                }
            }
        }
    }
}