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
    class LineChart_WeeklyClaiming : LinearChartModel
    {
        
        //Constructor
        public LineChart_WeeklyClaiming(): base()
        {          
            //Declare variable to store number of claims and claimed value in a specific week
            int? numOfClaimsWeek = null;
            decimal? claimedValueWeek = null;

            //Initialize list to store weekly values
            this.Values = new List<WeeklyValues>();

            //Declare variable to define start date and final date of a specific week
            DateTime initialDate;
            DateTime finalDate;

            //For loop to get weekly values for each week between StartDate and EndDate
            for (int i = 0; i < Weeks; i++)
            {
                //Define initial date of current week
                if (i == 0)
                {
                    initialDate = (DateTime)this.StartDate;
                }
                else
                {
                    initialDate = this.Values.ElementAt(i - 1).FinalDate.AddDays(1);
                }
                //Define final date of current week
                if (i == (Weeks - 1))
                {
                    finalDate = (DateTime)this.EndDate;
                }
                else
                {
                    finalDate = CalculateFinalDateOfWeek(initialDate);
                }

                //Get number of claims and claimed value for current week
                GetNoClaimsClaimedValue(initialDate, finalDate, this.Status, this.Make, this.Type, this.Group, this.MinValue, this.MaxValue, this.Fault, ref numOfClaimsWeek, ref claimedValueWeek);
                if (claimedValueWeek == null)
                {
                    claimedValueWeek = 0;
                }
                //Creat a new WeeklyValues object to store the data in current week and add the object to the values list
                this.Values.Add(new WeeklyValues(initialDate, finalDate, numOfClaimsWeek, claimedValueWeek));
            }//End of for loop

            //Define plot axes
            var weekAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Week",
                Maximum = this.Weeks,
                MinimumMinorStep =1,
                MinimumMajorStep =1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            var claimsAxis = new LinearAxis
            {
                Title = "Number of claims",
                Position = AxisPosition.Left,
                IntervalLength = 15,
                Maximum = 60,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            var valueAxis = new LinearAxis() {
                Title = "Claimed value",
                Maximum = 15000,
                IntervalLength = 10,
                Key = "Value scale",
                Position = AxisPosition.Right
            };

            this.Model = new PlotModel() { Title = "Weekly claims and value" };
            this.Model.Axes.Add(weekAxis);
            this.Model.Axes.Add(claimsAxis);
            this.Model.Axes.Add(valueAxis);

            //Populate the number of claim serie and claimed value serie
            LineSeries claimSerie = new LineSeries();
            claimSerie.MarkerSize = 7;
            claimSerie.MarkerType = MarkerType.Star;
            LineSeries valueSerie = new LineSeries();
            valueSerie.MarkerSize = 7;
            valueSerie.MarkerType = MarkerType.Cross;
            valueSerie.YAxisKey = "Value scale";

            List<DataPoint> claimPoints = new List<DataPoint>();
            List<DataPoint> valuePoints = new List<DataPoint>();
            int j = 1;
            foreach (WeeklyValues value in this.Values) {
                claimPoints.Add(new DataPoint((double)j,(double)value.NumOfClaimsWeek));
                valuePoints.Add(new DataPoint((double)j,(double)value.ClaimedValueWeek));
                j++;
            }
            //Add the series to the PlotModel
            claimSerie.ItemsSource = claimPoints;
            valueSerie.ItemsSource = valuePoints;
            this.Model.Series.Add(claimSerie);
            this.Model.Series.Add(valueSerie);
        }//End of constructor
    }// End of class
}// End of namespace
