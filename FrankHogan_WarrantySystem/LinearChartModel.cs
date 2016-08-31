using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class LinearChartModel : ChartModel
    {
        //Reference to main window
        MainWindow main = ((MainWindow)System.Windows.Application.Current.MainWindow);
        //Delcare a list of the internal class.This list will be used to populate the series
        protected List<WeeklyValues> Values { get; set; }
        protected int Weeks { get; set; }
        //Declare variable to define start date and final date of a specific week
       
        //Constructor
        public LinearChartModel()
        {
            
            //Get the dates from the UI
            this.StartDate = main.GetDateDPStartDateT1();
            this.EndDate = main.GetDateDPEndDateT1();

            //If dates are null (when the systems just boots up, select current year to date by default)
            if (this.StartDate == null)
            {
                if (this.EndDate == null)
                {
                    DateTime today = DateTime.Today;
                    this.StartDate = CreateMinDate(today.Year);
                    this.EndDate = GetMaxDateinDB();
                }//Show an error message if only one date has been selected
            }
            //Calculate the number of weeks between the two dates
            Weeks = this.CalculateWeeks((DateTime)this.StartDate, (DateTime)this.EndDate);
        }//End of constructor

        #region HELPER METHODS
        //Create new minimun date
        protected DateTime CreateMinDate(int year)
        {
            DateTime min;
            string firstDay = string.Format("01/01/{0}", year);
            min = Convert.ToDateTime(firstDay);
            return min;
        }//End of CreateMinDate

        //Method to create max data for linear chart
        protected DateTime CreateMaxDate(int year)
        {
            DateTime max;
            string lastDay = string.Format("31/12/{0}", year);
            max = Convert.ToDateTime(lastDay);
            return max;
        }// End of CreateMaxDate

        //Method to get the max date in the database
        protected DateTime GetMaxDateinDB()
        {
            DateTime maxDate;
            maxDate = this.main.GetMaxDateInDB();
            return maxDate;
        }//End of GetMaxDate

        //Method to calculate the number of weeks between two dates
        protected int CalculateWeeks(DateTime starDate, DateTime endDate)
        {
            int weeks;
            weeks = (int)(endDate - starDate).TotalDays / 7;
            if (starDate.DayOfWeek != DayOfWeek.Sunday)
            {
                weeks++;
            }
            if (weeks == 0)
            {
                weeks++;
            }
            return weeks;
        }//End of CalculateWeeks method

        //Method to define the number of days to be added based on the day of the week of start date
        protected DateTime CalculateFinalDateOfWeek(DateTime initialDate)
        {
            DateTime finalDate;
            DayOfWeek day = initialDate.DayOfWeek;
            switch (day)
            {
                case DayOfWeek.Sunday:
                    finalDate = initialDate.AddDays(6);
                    break;
                case DayOfWeek.Monday:
                    finalDate = initialDate.AddDays(5);
                    break;
                case DayOfWeek.Tuesday:
                    finalDate = initialDate.AddDays(4);
                    break;
                case DayOfWeek.Wednesday:
                    finalDate = initialDate.AddDays(3);
                    break;
                case DayOfWeek.Thursday:
                    finalDate = initialDate.AddDays(2);
                    break;
                case DayOfWeek.Friday:
                    finalDate = initialDate.AddDays(1);
                    break;
                case DayOfWeek.Saturday:
                    finalDate = initialDate.AddDays(7);
                    break;
                default:
                    finalDate = initialDate;
                    break;
            }//End of switch statement
            return finalDate;
        }//End of CalculateFinalDateOfWeek method

        //Method to define initial date of current week


        //Method to calculate weekly speed KPI
        protected static double? CalculateSpeedKPI(DateTime? startDate, DateTime? endDate, int? make) {
            double? KPI = null;
            KPI = MainWindow.CalculateSpeedKPI(startDate, endDate, make);
            return KPI;
        }//End of CalculateSpeedKPI method

        #endregion

        //Internal class to store weekly values of number of claims and claimed value
        protected class WeeklyValues
        {
            #region INNER CLASS PROPERTIES
            public DateTime InitialDate { get; set; }
            public DateTime FinalDate { get; set; }
            public int? NumOfClaimsWeek { get; set; }
            public decimal? ClaimedValueWeek { get; set; }
            public double? speedKPI { get; set; }
            #endregion

            //Constructor for number of claims and claimed value chart
            public WeeklyValues(DateTime initialDate, DateTime finalDate, int? numOfClaimsWeek, decimal? claimedValueWeek)
            {
                this.InitialDate = initialDate;
                this.FinalDate = finalDate;
                this.NumOfClaimsWeek = numOfClaimsWeek;
                this.ClaimedValueWeek = claimedValueWeek;
            }//End of constructor

            //Constructor for speed KPI chart
            public WeeklyValues(DateTime initialDate, DateTime finalDate, double? speedKPI) {
                this.InitialDate = initialDate;
                this.FinalDate = finalDate;
                this.speedKPI = speedKPI;
            }// End of constructor


        }//End of internal class

    }// End of class
}//End of namespace
