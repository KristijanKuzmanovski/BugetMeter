using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetMeter.Models
{
    public class Record
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public double amount { get; set; }
        public Category category { get; set; }
        public DateTime date { get; set; }
    }
}
