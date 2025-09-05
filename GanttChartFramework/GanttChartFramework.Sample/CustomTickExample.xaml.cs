using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GanttChartFramework.Views;

namespace GanttChartFramework.Sample
{
    public partial class CustomTickExample : Window
    {
        public CustomTickExample()
        {
            InitializeComponent();
        }
        
        private void TriangleTicks_Click(object sender, RoutedEventArgs e)
        {
            // 创建三角形刻度形状
            if (TimeScaleCanvas != null)
            {
                for (int i = 0; i < TimeScaleCanvas.Children.Count; i++)
                {
                    if (TimeScaleCanvas.Children[i] is TimeScaleTickControl tickControl)
                    {
                        var triangle = new Polygon
                        {
                            Points = new PointCollection { new Point(15, 0), new Point(30, 30), new Point(0, 30) },
                            Fill = Brushes.SteelBlue,
                            Stroke = Brushes.DarkBlue,
                            StrokeThickness = 1
                        };
                        
                        tickControl.SetCustomShape(triangle);
                    }
                }
            }
        }
        
        private void CircleTicks_Click(object sender, RoutedEventArgs e)
        {
            // 创建圆形刻度形状
            if (TimeScaleCanvas != null)
            {
                for (int i = 0; i < TimeScaleCanvas.Children.Count; i++)
                {
                    if (TimeScaleCanvas.Children[i] is TimeScaleTickControl tickControl)
                    {
                        var ellipse = new Ellipse
                        {
                            Width = 20,
                            Height = 20,
                            Fill = Brushes.Tomato,
                            Stroke = Brushes.DarkRed,
                            StrokeThickness = 1
                        };
                        
                        tickControl.SetCustomShape(ellipse);
                    }
                }
            }
        }
        
        private void DiamondTicks_Click(object sender, RoutedEventArgs e)
        {
            // 创建菱形刻度形状
            if (TimeScaleCanvas != null)
            {
                for (int i = 0; i < TimeScaleCanvas.Children.Count; i++)
                {
                    if (TimeScaleCanvas.Children[i] is TimeScaleTickControl tickControl)
                    {
                        var diamond = new Polygon
                        {
                            Points = new PointCollection { new Point(15, 0), new Point(30, 15), new Point(15, 30), new Point(0, 15) },
                            Fill = Brushes.Gold,
                            Stroke = Brushes.DarkGoldenrod,
                            StrokeThickness = 1
                        };
                        
                        tickControl.SetCustomShape(diamond);
                    }
                }
            }
        }
        
        private void ResetTicks_Click(object sender, RoutedEventArgs e)
        {
            // 恢复默认刻度形状
            if (TimeScaleCanvas != null)
            {
                for (int i = 0; i < TimeScaleCanvas.Children.Count; i++)
                {
                    if (TimeScaleCanvas.Children[i] is TimeScaleTickControl tickControl)
                    {
                        tickControl.ResetToDefault();
                    }
                }
            }
        }
        
        // 辅助方法：获取时间轴画布
        private Canvas? TimeScaleCanvas
        {
            get
            {
                var grid = TimeScale.Content as Grid;
                return grid?.Children[0] as Canvas;
            }
        }
    }
}