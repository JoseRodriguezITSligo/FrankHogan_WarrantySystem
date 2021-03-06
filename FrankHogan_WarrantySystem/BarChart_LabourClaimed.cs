﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace FrankHogan_WarrantySystem
{
    class BarChart_LabourClaimed : BarChartModel 
    {
        //Constructor
        public BarChart_LabourClaimed() : base() {

            //Declare variables to store retrived data from database
            int? numberOfClaims = null;
            decimal? claimedValue = null;
            decimal? labourClaimed = null;

            //Invoke the methods to call database stroed procedures
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numberOfClaims, ref claimedValue);
            labourClaimed = GetLabourClaimed(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault);

            //Create new plot model and add the series of this bar chart
            this.Model = new PlotModel() { Title = "Labour + Parts Claimed" };
            double[] values = { (double)claimedValue, (double)labourClaimed,(double)(claimedValue-labourClaimed)};
            BarSeries bars = this.UpdateBarSeries(values);
            bars.FillColor = OxyColors.Aquamarine;
            this.Model.Series.Add(bars);

            //Set up the chart axis by calling the proper method
            this.SetupCategories(new[]{
                                        "Total Value",
                                        "Labour",
                                        "Parts",
                                     });

        }//End of constructor
    }//End of class
}//End of namespace
