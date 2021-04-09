using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BudgetMeter.Models
{
   public class CategoryList : INotifyPropertyChanged
    {
        public List<Category> category { get; set; }

        public CategoryList(List<Category> x)
        {
           category = x;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
        }
    }
}
