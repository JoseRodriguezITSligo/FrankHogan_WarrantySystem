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
    class ChartModel
    {
        //Reference to main window
        MainWindow main = ((MainWindow)System.Windows.Application.Current.MainWindow);
        #region PROPERTIES
        //Model property to be linked with plot view
        public PlotModel Model { get;protected set; }

        protected int? NumberOfClaims { get; set; }
        protected int? NumOfClaimsPaid {get;set;}
        protected int? NumOfClaimsCancelled {get;set;}
        protected int? NumOfClaimsProcess {get; set;}
        protected decimal? ClaimedValue { get; set; }
        protected decimal?  ClaimedValuePaid {get;set;}
        protected decimal? ClaimedValueCancelled {get;set;}
        protected decimal? ClaimedValueProcess { get; set; }
        protected DateTime? StartDate {get; set;}
        protected DateTime?  EndDate {get;set;}
        protected int? Status { get; set; }
        protected int? Make { get; set; }
        protected string Type { get; set; }
        protected string Group { get; set; }
        protected decimal? MinValue { get; set; }
        protected decimal? MaxValue { get; set; }
        protected string Fault {get; set;}
        #endregion

        //Constructor
        public ChartModel()
        {
            this.NumberOfClaims = null;
            this.NumOfClaimsPaid = null;
            this.NumOfClaimsCancelled = null;
            this.NumOfClaimsProcess = null;
            this.ClaimedValue = null;
            this.ClaimedValuePaid = null;
            this.ClaimedValueCancelled = null;
            this.ClaimedValueProcess = null;
            this.StartDate = main.GetDateDPStartDateT1();
            this.EndDate = main.GetDateDPEndDateT1();
            this.Status = null;
            this.Make = null;
            this.Type = "%";
            this.Group = "%";
            this.MinValue = null;
            this.MaxValue = null;
            this.Fault = "%";
        }// End of constructor

        //Overloaded Constructor
        public ChartModel(int? status,int? make,string type) :this()
        {      
            this.Status = status;
            this.Make = make;
            this.Type = type;
            this.Group = "%";
        }// End of constructor

        #region HELPER METHODS
        //Method to call the mainwindow data context and get data off it (claimed value and number of claims)
        protected static void GetNoClaimsClaimedValue(DateTime? startDate, DateTime? endDate, int? status, 
                                                      int? make, string type, string group, decimal? minValue, 
                                                      decimal? maxValue, string fault, ref int? numberOfClaims, 
                                                      ref decimal? claimedValue)
        {
            //Call the mainwindow method to get access to data context
            MainWindow.GetNumOfClaimsClaimedValue(startDate, endDate, status, make, type,group, minValue, maxValue, fault, ref numberOfClaims, ref claimedValue);
        }// End of GetNoClaimsClaimedValue method

        //Method to call the mainwindow data context and get off it (labour claimed)
        protected static decimal? GetLabourClaimed(DateTime? startDate, DateTime? endDate, int? status,
                                                      int? make, string type, string group, decimal? minValue,
                                                      decimal? maxValue, string fault)
        {
            decimal? labour;
            labour = MainWindow.GetLabourClaimed(startDate, endDate, status, make, type, group, minValue, maxValue, fault);
            return labour;
        }// End of GetLabourClaimed method
        //Method to calculate the number of claims in process
        protected int? CalculateClaimsInProcess()
        {
            return NumberOfClaims - NumOfClaimsPaid - NumOfClaimsCancelled;
        }//End of CalculateClaimsInProcess method

        //Method to calculate the claimed value in process
        protected decimal? CalculateValueInProcess()
        {
            return ClaimedValue - ClaimedValuePaid - ClaimedValueCancelled;
        }// End of calculateValueInProcess method

        //Method to refresh the PlotView Model
        protected virtual void RefreshPlotView() {
        }//End of RefreshPlotView method
        #endregion
    }// End of class
}// End of namespace
