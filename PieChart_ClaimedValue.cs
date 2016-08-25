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
    class PieChart_ClaimedValue : PieChartModel
    {   //Constructor
        public PieChart_ClaimedValue() : base()
        {
            int? numberOfClaims = null, numOfClaimsPaid = null, numOfClaimsCancelled=null;
            decimal? claimedValue = null, claimedValuePaid=null, claimedValueCancelled=null;

            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numberOfClaims, ref claimedValue);
            this.Status = 9;
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numOfClaimsPaid, ref claimedValuePaid);
            this.Status = 10;
            GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numOfClaimsCancelled, ref claimedValueCancelled);
            this.NumOfClaimsProcess = CalculateClaimsInProcess();
            if (claimedValueCancelled == null)
            {
                claimedValueCancelled = 0;
            }
            if (claimedValuePaid == null)
            {
                claimedValuePaid = 0;
            }
            if (claimedValue == null)
            {
                claimedValue = 0;
            }
            if (claimedValueCancelled == null) {
                claimedValueCancelled = 0;
            }
            this.ClaimedValueProcess = claimedValue-claimedValuePaid-claimedValueCancelled;

            this.Values.Add((double)claimedValuePaid);
            this.Values.Add((double)this.ClaimedValueProcess);
            this.Values.Add((double)claimedValueCancelled);

            this.Tags.Add("Paid");
            this.Tags.Add("Process");
            this.Tags.Add("Cancelled");

            this.Model = new PlotModel() { Title = "Claimed Value Breakdown" };
            this.SeriesPie.InsideLabelFormat = "{1}";
            this.SeriesPie = this.PopulatePieSeries(this.Tags,this.Values);
            this.Model.Series.Add(this.SeriesPie);

        }//End of constructor
    }//End of class
}//End of namespace
