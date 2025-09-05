using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GanttChartFramework.Views
{
    public partial class TimeScaleTickControl : UserControl
    {
        #region 依赖属性
        
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(TimeScaleTickControl),
                new PropertyMetadata(Brushes.Black));
        
        public static readonly DependencyProperty TickThicknessProperty =
            DependencyProperty.Register("TickThickness", typeof(double), typeof(TimeScaleTickControl),
                new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty TickDashArrayProperty =
            DependencyProperty.Register("TickDashArray", typeof(DoubleCollection), typeof(TimeScaleTickControl),
                new PropertyMetadata(null));
        
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(TimeScaleTickControl),
                new PropertyMetadata(""));
        
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(TimeScaleTickControl),
                new PropertyMetadata(13.0));
        
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(TimeScaleTickControl),
                new PropertyMetadata(Brushes.Black));
        
        public static readonly DependencyProperty CustomTickTemplateProperty =
            DependencyProperty.Register("CustomTickTemplate", typeof(ControlTemplate), typeof(TimeScaleTickControl),
                new PropertyMetadata(null, OnCustomTickTemplateChanged));
        
        #endregion
        
        #region 属性包装器
        
        public Brush TickStroke
        {
            get => (Brush)GetValue(TickStrokeProperty);
            set => SetValue(TickStrokeProperty, value);
        }
        
        public double TickThickness
        {
            get => (double)GetValue(TickThicknessProperty);
            set => SetValue(TickThicknessProperty, value);
        }
        
        public DoubleCollection TickDashArray
        {
            get => (DoubleCollection)GetValue(TickDashArrayProperty);
            set => SetValue(TickDashArrayProperty, value);
        }
        
        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }
        
        public double LabelFontSize
        {
            get => (double)GetValue(LabelFontSizeProperty);
            set => SetValue(LabelFontSizeProperty, value);
        }
        
        public Brush LabelForeground
        {
            get => (Brush)GetValue(LabelForegroundProperty);
            set => SetValue(LabelForegroundProperty, value);
        }
        
        public ControlTemplate CustomTickTemplate
        {
            get => (ControlTemplate)GetValue(CustomTickTemplateProperty);
            set => SetValue(CustomTickTemplateProperty, value);
        }
        
        #endregion
        
        public TimeScaleTickControl()
        {
            InitializeComponent();
        }
        
        private static void OnCustomTickTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TimeScaleTickControl;
            control?.ApplyCustomTemplate();
        }
        
        private void ApplyCustomTemplate()
        {
            if (CustomTickTemplate != null)
            {
                // 清除默认内容
                TickContainer.Children.Clear();
                
                // 应用自定义模板
                var contentControl = new ContentControl
                {
                    Template = CustomTickTemplate,
                    Content = this
                };
                
                TickContainer.Children.Add(contentControl);
            }
            else
            {
                // 恢复默认内容
                TickContainer.Children.Clear();
                
                // 重新添加默认的线条和文本
                var line = new System.Windows.Shapes.Line
                {
                    X1 = 30, Y1 = 0, X2 = 30, Y2 = 30,
                    Stroke = TickStroke,
                    StrokeThickness = TickThickness,
                    StrokeDashArray = TickDashArray
                };
                
                var textBlock = new TextBlock
                {
                    Text = LabelText,
                    FontSize = LabelFontSize,
                    Foreground = LabelForeground,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                
                TickContainer.Children.Add(line);
                TickContainer.Children.Add(textBlock);
            }
        }
        
        // 方法：设置自定义形状
        public void SetCustomShape(FrameworkElement shape)
        {
            TickContainer.Children.Clear();
            TickContainer.Children.Add(shape);
            
            // 如果需要，也可以添加标签
            if (!string.IsNullOrEmpty(LabelText))
            {
                var textBlock = new TextBlock
                {
                    Text = LabelText,
                    FontSize = LabelFontSize,
                    Foreground = LabelForeground,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                TickContainer.Children.Add(textBlock);
            }
        }
        
        // 方法：清除自定义形状，恢复默认
        public void ResetToDefault()
        {
            ApplyCustomTemplate();
        }
    }
}