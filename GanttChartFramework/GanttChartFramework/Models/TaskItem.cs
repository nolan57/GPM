using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GanttChartFramework.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDay { get; set; }
        public int Duration { get; set; }
        public int? ParentId { get; set; }
        public Brush Color { get; set; }
        public double Progress { get; set; }
    }
}