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
    class LinearChart_SpeedKpi :LinearChartModel
    {
        static double speedLimit = 14;
        //Constructor
        public LinearChart_SpeedKpi() : base() {
            //Initialize list to store weekly values
            this.Values = new List<WeeklyValues>();

            //Declare variable to define start date and final date of a specific week
            DateTime initialDate;
            DateTime finalDate;
            double? KPI;

            //For loop to get weekly values for each week between StartDate and EndDate
            for (int i = 0; i < this.Weeks; i++)
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
                if (i == (this.Weeks - 1))
                {
                    finalDate = (DateTime)this.EndDate;
                }
                else
                {
                    finalDate = CalculateFinalDateOfWeek(initialDate);
                }

                //Get speed kpi for current week
                KPI = CalculateSpeedKPI(initialDate,finalDate,this.Make);
                if (KPI == null) {
                    KPI = 0;
                }
                //Creat a new WeeklyValues object to store the data in current week and add the object to the values list
                this.Values.Add(new WeeklyValues(initialDate, finalDate, KPI));
            }//End of for loop

            //Define plot axes
            var weekAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Week",
                Maximum = this.Weeks,
                MinimumMinorStep = 1,
                MinimumMajorStep = 1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };

            var KPIAxis = new LinearAxis
            {
                Title = "Claiming speed KPI",
                Position = AxisPosition.Left,
                IntervalLength = 15,
                Maximum = 20,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };

            //Set up model features
            this.Model = new PlotModel() { Title = "Claiming Speed KPI" };
            this.Model.Axes.Add(weekAxis);
            this.Model.Axes.Add(KPIAxis);

            //List to limit the KPI series
            LineSeries limit = new LineSeries();
            limit.Color = OxyColors.Blue;
            List<DataPoint> limitPoints = new List<DataPoint>();

            //Populate the number of claims serie and claimed value serie
            LineSeries KPISerie = new LineSeries();
            List<DataPoint> KPIPoints = new List<DataPoint>();

            int j = 1;
            foreach (WeeklyValues value in this.Values)
            {

                limitPoints.Add(new DataPoint((double)j, speedLimit));
                KPIPoints.Add(new DataPoint((double)j, (double)value.speedKPI));
                j++;
            }
            //Add the series to the PlotModel
            KPISerie.ItemsSource = KPIPoints;
            limit.ItemsSource = limitPoints;
            this.Model.Series.Add(KPISerie);
            this.Model.Series.Add(limit);

        }//End of constructor
    }//End of class
}//End of namespace
