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
    class BarChart_NumberOfClaims: BarChartModel
    {
        //Plot model constructor
        public BarChart_NumberOfClaims() : base()
        {
            //Declare variables to store retrived data from database
            int? numberOfClaims = null, numOfClaimsPaid = null, numOfClaimsCancelled = null;
            decimal? claimedValue = null, claimedValuePaid = null, claimedValueCancelled = null;

            //Invoke the methods to call database stroed procedures
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numberOfClaims, ref claimedValue);
            this.Status = 9;
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numOfClaimsPaid, ref claimedValuePaid);
            this.Status = 10;
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numOfClaimsCancelled, ref claimedValueCancelled);
            this.NumOfClaimsProcess = numberOfClaims-numOfClaimsPaid-numOfClaimsCancelled;

            //Create new plot model and add the series of this bar chart
            this.Model = new PlotModel() { Title = "Number of Claims"};
            double[] values = { (double)numberOfClaims, (double)numOfClaimsPaid, (double)this.NumOfClaimsProcess, (double)numOfClaimsCancelled };
            BarSeries bars = this.UpdateBarSeries(values);
            this.Model.Series.Add(bars);

            //Set up the chart axis by calling the proper method
            this.SetupCategories(new[]{
                                        "Total",
                                        "Paid",
                                        "In process",
                                        "Cancelled"
                                     });

        }//End of constructor

    }// End of Class
}//End of namespace
