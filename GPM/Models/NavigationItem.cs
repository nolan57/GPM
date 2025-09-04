using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Models
{
    public class NavigationItem : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _viewName;
        public string ViewName
        {
            get { return _viewName; }
            set { SetProperty(ref _viewName, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public NavigationItem(string title, string viewName)
        {
            Title = title;
            ViewName = viewName;
        }
    }
}