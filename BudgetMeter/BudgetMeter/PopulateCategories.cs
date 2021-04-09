using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BudgetMeter
{
    class PopulateCategories
    {
        public static List<Models.Category> defaultExpenceCategories = new List<Models.Category>()
        {
            new Models.Category (){  color = HexConverter(Color.Red), name = "Food & Drink"},
            new Models.Category (){  color = HexConverter(Color.Blue), name = "Bills"},
            new Models.Category (){  color = HexConverter(Color.Green), name = "Fitness"},
            new Models.Category (){  color = HexConverter(Color.Orange), name = "Entertainment"},
            new Models.Category (){  color = HexConverter(Color.Yellow), name = "Maintenance"},
        };
        public static List<Models.Category> defaultIncomeCategories = new List<Models.Category>()
        {
            new Models.Category (){  color = HexConverter(Color.LightCyan), name = "Gift"},
            new Models.Category (){  color = HexConverter(Color.LightGreen), name = "Payment"},
            new Models.Category (){  color = HexConverter(Color.Gold), name = "Bonus"},
            new Models.Category (){  color = HexConverter(Color.LightSeaGreen), name = "Winnings"},
        };

        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

    }
}
