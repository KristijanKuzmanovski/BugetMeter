using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetMeter.SlideMenu
{
    public class SlideMenuFlyoutMenuItem
    {
        public SlideMenuFlyoutMenuItem()
        {
            TargetType = typeof(SlideMenuFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }

        public string Icon { get; set; }
    }
}