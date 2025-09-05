using System.Windows;
using System.Windows.Controls;
using GanttChartFramework.Views;

namespace GanttChartFramework.Sample.Examples
{
    public partial class TimeTextPositionExample : Window
    {
        public TimeTextPositionExample()
        {
            InitializeComponent();
            
            // 绑定交互控制事件
            HorizontalAlignmentComboBox.SelectionChanged += (s, e) =>
            {
                if (InteractiveTick != null)
                {
                    var selectedAlignment = (HorizontalAlignmentComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var alignment = HorizontalAlignment.Center;
                    
                    switch (selectedAlignment)
                    {
                        case "Left": alignment = HorizontalAlignment.Left; break;
                        case "Right": alignment = HorizontalAlignment.Right; break;
                        case "Stretch": alignment = HorizontalAlignment.Stretch; break;
                        default: alignment = HorizontalAlignment.Center; break;
                    }
                    
                    InteractiveTick.YearHorizontalAlignment = alignment;
                    InteractiveTick.QuarterHorizontalAlignment = alignment;
                    InteractiveTick.MonthHorizontalAlignment = alignment;
                    InteractiveTick.DayHorizontalAlignment = alignment;
                }
            };
            
            LeftMarginSlider.ValueChanged += (s, e) =>
            {
                if (InteractiveTick != null)
                {
                    var leftMargin = LeftMarginSlider.Value;
                    var rightMargin = RightMarginSlider.Value;
                    
                    InteractiveTick.YearMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.QuarterMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.MonthMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.DayMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                }
            };
            
            RightMarginSlider.ValueChanged += (s, e) =>
            {
                if (InteractiveTick != null)
                {
                    var leftMargin = LeftMarginSlider.Value;
                    var rightMargin = RightMarginSlider.Value;
                    
                    InteractiveTick.YearMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.QuarterMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.MonthMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                    InteractiveTick.DayMargin = new Thickness(leftMargin, 0, rightMargin, 0);
                }
            };
        }
    }
}