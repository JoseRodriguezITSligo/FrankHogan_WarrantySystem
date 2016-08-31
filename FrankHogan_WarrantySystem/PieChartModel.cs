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
    class PieChartModel : ChartModel
    {
        #region PRPERTIES
        protected PieSeries SeriesPie {get;set;}
        protected List<double> Values { get; set; }
        protected List<string> Tags { get; set; }
        #endregion

        public PieChartModel() : base(){
            //Set up visual preferences
		    SeriesPie = new PieSeries { StrokeThickness = 2, InsideLabelPosition = 0.8, InsideLabelFormat ="{0:F2}" };
            //Instanciate lists
            Values = new List<double>();
		    Tags = new List<string> ();
	    }// End of constructor
        #region HELPER METHODS
        //Method to populate the Pie series
        public PieSeries PopulatePieSeries(List<string> slices, List<double> values)
        {
            PieSeries series = new PieSeries();
            series.InsideLabelFormat = "{1}";
            series.OutsideLabelFormat = "{2:0}%";
            //Declare a counter
            int i = 0;
            foreach (string slice in slices) { 
                series.Slices.Add(new PieSlice(slice, (double)values.ElementAt(i)));
                i++;
            }//End of foreach loop
            return series;
        }//End of PopulatePieSeries method
        #endregion
    }// End of class

    
}// End of namespace
