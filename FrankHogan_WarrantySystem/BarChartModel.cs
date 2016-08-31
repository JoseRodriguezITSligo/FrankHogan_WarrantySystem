using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace FrankHogan_WarrantySystem
{
    class BarChartModel : ChartModel
    {
        //Constructor
        public BarChartModel() : base() {

        }//End of consructor


        protected void SetupCategories(string[] categoryList) {
            CategoryAxis categories = new CategoryAxis();
            categories.Position = AxisPosition.Left;
            categories.Key = "Axis";
            categories.ItemsSource = categoryList;
            this.Model.Axes.Add(categories);
        }// End of SetupCategories

        protected BarSeries UpdateBarSeries(double[] values)
        {
            BarSeries bars = new BarSeries();
            List<BarItem> totalClaims = new List<BarItem>();
            foreach (double value in values) {
                totalClaims.Add(new BarItem { Value = value});
            }
            bars.LabelPlacement = LabelPlacement.Inside;
            bars.LabelFormatString = "{0:F0}";
            bars.ItemsSource = totalClaims;
            return bars;
        }

    }//End of class
}//End of namespace
