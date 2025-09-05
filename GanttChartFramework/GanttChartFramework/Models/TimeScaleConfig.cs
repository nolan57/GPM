using System;
using System.Collections.Generic;

namespace GanttChartFramework.Models
{
    /// <summary>
    /// 时间轴刻度配置类
    /// </summary>
    public class TimeScaleConfig
    {
        /// <summary>
        /// 时间单位信息列表
        /// </summary>
        public List<TimeUnitInfo> TimeUnits { get; set; } = new List<TimeUnitInfo>();
        
        /// <summary>
        /// 显示选项配置
        /// </summary>
        public DisplayOptions DisplayOptions { get; set; } = new DisplayOptions();
        
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// 时间间隔（以天为单位）
        /// </summary>
        public int IntervalDays { get; set; } = 1;
        
        /// <summary>
        /// 生成时间单位信息
        /// </summary>
        public void GenerateTimeUnits()
        {
            TimeUnits.Clear();
            
            var currentTime = StartTime;
            while (currentTime <= EndTime)
            {
                TimeUnits.Add(new TimeUnitInfo { DateTime = currentTime });
                currentTime = currentTime.AddDays(IntervalDays);
            }
        }
    }
    
    /// <summary>
    /// 时间单位信息类
    /// </summary>
    public class TimeUnitInfo
    {
        public DateTime DateTime { get; set; }
        
        public string Year => DateTime.Year.ToString();
        
        public string Quarter => $"Q{(DateTime.Month - 1) / 3 + 1}";
        
        public string Month => DateTime.ToString("MMM");
        
        public string Week => $"第{GetWeekNumber(DateTime)}周";
        
        public string Day => DateTime.Day.ToString();
        
        public string Time => DateTime.ToString("HH:mm");
        
        private int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
    }
    
    /// <summary>
    /// 显示选项配置类
    /// </summary>
    public class DisplayOptions
    {
        /// <summary>
        /// 是否显示年份级别
        /// </summary>
        public bool ShowYearLevel { get; set; } = true;
        
        /// <summary>\        /// 是否显示季度级别
        /// </summary>
        public bool ShowQuarterLevel { get; set; } = false;
        
        /// <summary>
        /// 是否显示月份级别
        /// </summary>
        public bool ShowMonthLevel { get; set; } = true;
        
        /// <summary>
        /// 是否显示周级别
        /// </summary>
        public bool ShowWeekLevel { get; set; } = false;
        
        /// <summary>
        /// 是否显示日期级别
        /// </summary>
        public bool ShowDayLevel { get; set; } = true;
        
        /// <summary>
        /// 是否显示时间级别
        /// </summary>
        public bool ShowTimeLevel { get; set; } = false;
        
        /// <summary>
        /// 时间单位显示方向
        /// </summary>
        public System.Windows.Controls.Orientation TimeUnitsOrientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;
        
        /// <summary>
        /// 最大显示行数
        /// </summary>
        public int MaxDisplayRows { get; set; } = 2;
        
        /// <summary>
        /// 级别间距
        /// </summary>
        public System.Windows.Thickness LevelMargin { get; set; } = new System.Windows.Thickness(2);
        
        /// <summary>
        /// 年份文本前景色
        /// </summary>
        public System.Windows.Media.Brush YearForeground { get; set; } = System.Windows.Media.Brushes.DarkBlue;
        
        /// <summary>
        /// 年份字体大小
        /// </summary>
        public double YearFontSize { get; set; } = 14;
        
        /// <summary>
        /// 季度文本前景色
        /// </summary>
        public System.Windows.Media.Brush QuarterForeground { get; set; } = System.Windows.Media.Brushes.DarkGreen;
        
        /// <summary>
        /// 季度字体大小
        /// </summary>
        public double QuarterFontSize { get; set; } = 12;
        
        /// <summary>
        /// 月份文本前景色
        /// </summary>
        public System.Windows.Media.Brush MonthForeground { get; set; } = System.Windows.Media.Brushes.DarkRed;
        
        /// <summary>
        /// 月份字体大小
        /// </summary>
        public double MonthFontSize { get; set; } = 12;
        
        /// <summary>
        /// 周文本前景色
        /// </summary>
        public System.Windows.Media.Brush WeekForeground { get; set; } = System.Windows.Media.Brushes.Purple;
        
        /// <summary>
        /// 周字体大小
        /// </summary>
        public double WeekFontSize { get; set; } = 11;
        
        /// <summary>
        /// 日期文本前景色
        /// </summary>
        public System.Windows.Media.Brush DayForeground { get; set; } = System.Windows.Media.Brushes.Black;
        
        /// <summary>
        /// 日期字体大小
        /// </summary>
        public double DayFontSize { get; set; } = 12;
        
        /// <summary>
        /// 时间文本前景色
        /// </summary>
        public System.Windows.Media.Brush TimeForeground { get; set; } = System.Windows.Media.Brushes.Gray;
        
        /// <summary>
        /// 时间字体大小
        /// </summary>
        public double TimeFontSize { get; set; } = 11;
        
        /// <summary>
        /// 年份文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment YearHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 年份文本边距
        /// </summary>
        public System.Windows.Thickness YearMargin { get; set; } = new System.Windows.Thickness(0);
        
        /// <summary>
        /// 季度文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment QuarterHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 季度文本边距
        /// </summary>
        public System.Windows.Thickness QuarterMargin { get; set; } = new System.Windows.Thickness(0);
        
        /// <summary>
        /// 月份文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment MonthHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 月份文本边距
        /// </summary>
        public System.Windows.Thickness MonthMargin { get; set; } = new System.Windows.Thickness(0);
        
        /// <summary>
        /// 周文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment WeekHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 周文本边距
        /// </summary>
        public System.Windows.Thickness WeekMargin { get; set; } = new System.Windows.Thickness(0);
        
        /// <summary>
        /// 日期文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment DayHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 日期文本边距
        /// </summary>
        public System.Windows.Thickness DayMargin { get; set; } = new System.Windows.Thickness(0);
        
        /// <summary>
        /// 时间文本水平对齐方式
        /// </summary>
        public System.Windows.HorizontalAlignment TimeHorizontalAlignment { get; set; } = System.Windows.HorizontalAlignment.Center;
        
        /// <summary>
        /// 时间文本边距
        /// </summary>
        public System.Windows.Thickness TimeMargin { get; set; } = new System.Windows.Thickness(0);
    }
}