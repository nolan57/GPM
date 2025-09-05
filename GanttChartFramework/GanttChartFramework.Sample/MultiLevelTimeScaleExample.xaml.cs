using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GanttChartFramework.Models;

namespace GanttChartFramework.Sample
{
    public partial class MultiLevelTimeScaleExample : Window
    {
        private bool _isHorizontalLayout = true;
        private DisplayOptions _displayOptions;
        
        public MultiLevelTimeScaleExample()
        {
            InitializeComponent();
            
            // 初始化显示选项
            _displayOptions = new DisplayOptions();
            
            // 绑定文本对齐控制事件
            TextAlignmentComboBox.SelectionChanged += (s, e) =>
            {
                if (_displayOptions != null)
                {
                    var selectedAlignment = (TextAlignmentComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var alignment = HorizontalAlignment.Center;
                    
                    switch (selectedAlignment)
                    {
                        case "Left": alignment = HorizontalAlignment.Left; break;
                        case "Right": alignment = HorizontalAlignment.Right; break;
                        default: alignment = HorizontalAlignment.Center; break;
                    }
                    
                    // 更新显示选项
                    _displayOptions.YearHorizontalAlignment = alignment;
                    _displayOptions.QuarterHorizontalAlignment = alignment;
                    _displayOptions.MonthHorizontalAlignment = alignment;
                    _displayOptions.WeekHorizontalAlignment = alignment;
                    _displayOptions.DayHorizontalAlignment = alignment;
                    _displayOptions.TimeHorizontalAlignment = alignment;
                    
                    // 重新应用配置
                    ShowCustomConfig_Click(null, null);
                }
            };
            
            // 绑定边距控制事件
            LeftMarginSlider.ValueChanged += (s, e) => UpdateMargins();
            RightMarginSlider.ValueChanged += (s, e) => UpdateMargins();
        }
        
        private void UpdateMargins()
        {
            if (_displayOptions != null)
            {
                var leftMargin = LeftMarginSlider.Value;
                var rightMargin = RightMarginSlider.Value;
                var margin = new Thickness(leftMargin, 0, rightMargin, 0);
                
                // 更新显示选项
                _displayOptions.YearMargin = margin;
                _displayOptions.QuarterMargin = margin;
                _displayOptions.MonthMargin = margin;
                _displayOptions.WeekMargin = margin;
                _displayOptions.DayMargin = margin;
                _displayOptions.TimeMargin = margin;
                
                // 重新应用配置
                ShowCustomConfig_Click(null, null);
            }
        }
        
        private void ShowDefaultMultiLevel_Click(object sender, RoutedEventArgs e)
        {
            TimeScaleControl.TimeScaleType = "MultiLevel";
            TimeScaleControl.InitializeTimeScale();
        }
        
        private void ShowCustomConfig_Click(object sender, RoutedEventArgs e)
        {
            // 更新显示选项
            _displayOptions.ShowYearLevel = ShowYearCheck.IsChecked ?? true;
            _displayOptions.ShowQuarterLevel = ShowQuarterCheck.IsChecked ?? true;
            _displayOptions.ShowMonthLevel = ShowMonthCheck.IsChecked ?? true;
            _displayOptions.ShowWeekLevel = ShowWeekCheck.IsChecked ?? true;
            _displayOptions.ShowDayLevel = ShowDayCheck.IsChecked ?? true;
            _displayOptions.ShowTimeLevel = ShowTimeCheck.IsChecked ?? false;
            
            _displayOptions.TimeUnitsOrientation = _isHorizontalLayout ? System.Windows.Controls.Orientation.Horizontal : System.Windows.Controls.Orientation.Vertical;
            _displayOptions.MaxDisplayRows = 3;
            _displayOptions.LevelMargin = new Thickness(2);
            
            _displayOptions.YearFontSize = 12;
            _displayOptions.YearForeground = Brushes.DarkBlue;
            _displayOptions.QuarterFontSize = 11;
            _displayOptions.QuarterForeground = Brushes.DarkGreen;
            _displayOptions.MonthFontSize = 11;
            _displayOptions.MonthForeground = Brushes.DarkRed;
            _displayOptions.WeekFontSize = 10;
            _displayOptions.WeekForeground = Brushes.Purple;
            _displayOptions.DayFontSize = 12;
            _displayOptions.DayForeground = Brushes.Black;
            _displayOptions.TimeFontSize = 10;
            _displayOptions.TimeForeground = Brushes.Gray;

            // 创建自定义配置
            var config = new TimeScaleConfig
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(30),
                DisplayOptions = _displayOptions
            };
            
            TimeScaleControl.InitializeMultiLevelTimeScale(config);
        }
        
        private void ToggleOrientation_Click(object sender, RoutedEventArgs e)
        {
            _isHorizontalLayout = !_isHorizontalLayout;
            ToggleOrientation.Content = _isHorizontalLayout ? "切换为垂直布局" : "切换为水平布局";
        }
        
        private void ClearTimeScale_Click(object sender, RoutedEventArgs e)
        {
            // 清除时间轴内容
            TimeScaleControl.InitializeTimeScale();
        }
    }
}