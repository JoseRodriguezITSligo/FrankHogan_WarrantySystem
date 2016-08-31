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
    class PieChart_TypeDistribution : PieChartModel
    {
        List<ClaimTypeExtended> Types { get; set; }

        //Constructor
        public PieChart_TypeDistribution() : base()
        {
            int? numberOfClaims = null;
            decimal? claimedValue = null;
            this.Types = MainWindow.GetTypes();

            foreach (ClaimTypeExtended type in Types)
            {
                this.Type = type.ClaimType_ID;
                GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numberOfClaims, ref claimedValue);
                this.Values.Add((double)numberOfClaims);
                this.Tags.Add(type.TypeDescription);
            }// End of foreach loop

            this.Model = new PlotModel() { Title = "Claim Type Distribution" };
            //this.SeriesPie.InsideLabelFormat = "{2}%";
            //this.SeriesPie.OutsideLabelFormat = "";
            this.Model.IsLegendVisible = true;
            this.Model.LegendPosition = LegendPosition.RightMiddle;
            this.SeriesPie = this.PopulatePieSeries(this.Tags, this.Values);
            this.SeriesPie.InsideLabelFormat = "";
            this.Model.Series.Add(this.SeriesPie);

        }//End of constructor
    }// End of class
}//End of namespace
