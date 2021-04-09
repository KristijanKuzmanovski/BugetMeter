using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetMeter.Models
{

        public class PDFrecord
        {
            public string type { get; set; }
            public string description { get; set; }
            public double amount { get; set; }
            public string category { get; set; }
            public DateTime date { get; set; }
        }
    
}
