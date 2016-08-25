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
    class PieChart_MakeDistribution : PieChartModel
    {
        //Properties
        protected List<MakeExtended> Makes { get; set; }

        //Constructor
        public PieChart_MakeDistribution() : base()
        {
            int? numberOfClaims =null;
            decimal? claimedValue = null;
            this.Makes = MainWindow.GetActiveMakes();

            foreach (MakeExtended make in Makes)
            {
                this.Make = make.Make_ID;
                GetNoClaimsClaimedValue(this.StartDate, this.EndDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numberOfClaims, ref claimedValue);
                this.Values.Add((double)numberOfClaims);
                this.Tags.Add(make.Name);
            }// End of foreach loop

            this.Model = new PlotModel() { Title = "Make Distribution" };
            this.SeriesPie.InsideLabelFormat = "{1}";
            this.SeriesPie = this.PopulatePieSeries(this.Tags,this.Values);
            this.Model.Series.Add(this.SeriesPie);

        }//End of constructor
    }// End of class
}//End of namespace
