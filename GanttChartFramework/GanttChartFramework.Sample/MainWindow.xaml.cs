using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GanttChartFramework.Models;

namespace GanttChartFramework.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSampleData();
        }
        
        private void LoadSampleData()
        {
            var startDate = DateTime.Now;
            var tasks = new List<TaskItem>
            {
                // 父任务
                new TaskItem { Id = 1, Name = "项目规划", StartDay = startDate, Duration = 5 },
                new TaskItem { Id = 2, Name = "需求分析", StartDay = startDate.AddDays(5), Duration = 10 },
                new TaskItem { Id = 3, Name = "系统设计", StartDay = startDate.AddDays(15), Duration = 15 },
                new TaskItem { Id = 4, Name = "编码实现", StartDay = startDate.AddDays(30), Duration = 30 },
                new TaskItem { Id = 5, Name = "测试", StartDay = startDate.AddDays(60), Duration = 20 },
                new TaskItem { Id = 6, Name = "部署", StartDay = startDate.AddDays(80), Duration = 5 },
                
                // 子任务
                new TaskItem { Id = 7, Name = "前端开发", StartDay = startDate.AddDays(30), Duration = 15, ParentId = 4 },
                new TaskItem { Id = 8, Name = "后端开发", StartDay = startDate.AddDays(35), Duration = 20, ParentId = 4 },
                new TaskItem { Id = 9, Name = "单元测试", StartDay = startDate.AddDays(60), Duration = 10, ParentId = 5 },
                new TaskItem { Id = 10, Name = "集成测试", StartDay = startDate.AddDays(70), Duration = 10, ParentId = 5 }
            };
            
            GanttChart.Tasks = tasks;
        }
        
        private void CustomTicks_Click(object sender, RoutedEventArgs e)
        {
            var customTickWindow = new CustomTickExample();
            customTickWindow.ShowDialog();
        }
        
        private void MultiLevelTimeScale_Click(object sender, RoutedEventArgs e)
        {
            var multiLevelWindow = new MultiLevelTimeScaleExample();
            multiLevelWindow.ShowDialog();
        }
    }
}