using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FrankHogan_WarrantySystem
{
    class RepairExtended : Repair
    {
        //Min Repair order allowed
        public static int MinRepairOrder = 10000;
        //Max repair order allowed
        public static int MaxRepairOrder = 99999;
        //Max invoice number allowed
        public static int MaxInvoice = 99999999;
        //Min invoice number allowed
        public static int MinInvoice = 30000000;

        #region PROPERTIES
        private static int DefaultInvNumber = 30000000;
        public new CarExtended Car { get; set; }
        public new FaultExtended Fault { get; set; }
        public new SolutionExtended Solution { get; set; }
        #endregion

        #region CONSTRUCTORS
        //Full constructor
        public RepairExtended(int repairID, int repairOrder, CarExtended car, 
                              FaultExtended fault,SolutionExtended solution,
                              DateTime receptionDate, DateTime repairDate, 
                              double timeTaken, int invoiceNumber) {
            this.Repair_ID = repairID;
            this.RepairOrder = repairOrder;
            this.Car = car;
            this.Fault = fault;
            this.Solution = solution;
            this.ReceptionDate = receptionDate;
            this.RepairDate = repairDate;
            this.InvoiceNumber = invoiceNumber;
        }// End of full constructor

        //Constructor that does not take in the repair order
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, SolutionExtended solution,
                              DateTime receptionDate, DateTime repairDate,
                              double timeTaken, int invoiceNumber) : this(repairID,0,car,fault,
                                                                          solution,receptionDate,
                                                                          repairDate,timeTaken,
                                                                          invoiceNumber) { }
        //Constructor that does not take in the reception date
        public RepairExtended(int repairID,int repairOrder ,CarExtended car,
                              FaultExtended fault, SolutionExtended solution,
                              DateTime repairDate,double timeTaken, 
                              int invoiceNumber) : this(repairID, repairOrder, car, fault,
                                                solution,DateTime.Today,repairDate, timeTaken,
                                                invoiceNumber){ }
        //Constructor that does not take in the reception date or repair date
        public RepairExtended(int repairID, int repairOrder, CarExtended car,
                              FaultExtended fault, SolutionExtended solution, double timeTaken,
                              int invoiceNumber) : this(repairID, repairOrder, car, fault,
                                                        solution,DateTime.Today, timeTaken,
                                                        invoiceNumber){ }
        
        //Constructor that does not take in the reception date, repair date nor repair order
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, SolutionExtended solution, double timeTaken,
                              int invoiceNumber) : this(repairID,0, car, fault,
                                                        solution, DateTime.Today, timeTaken,
                                                        invoiceNumber){ }

        //Constructor that does not take in a solution
        public RepairExtended(int repairID, int repairOrder, CarExtended car,
                              FaultExtended fault,DateTime receptionDate,DateTime repairDate,  
                              double timeTaken,int invoiceNumber) : this(repairID, 
                                                                         repairOrder, car, fault,null,
                                                                         receptionDate,repairDate, 
                                                                         timeTaken,invoiceNumber) { }

        //Constructor that does not take in the reception date, repair date, repair order nor solution
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, double timeTaken,
                              int invoiceNumber) : this(repairID, car, fault,null,
                                                        timeTaken,invoiceNumber) { }

        //Constructor that does not take in the invoice number
        public RepairExtended(int repairID, int repairOrder,CarExtended car,
                              FaultExtended fault, SolutionExtended solution,
                              DateTime receptionDate, DateTime repairDate,
                              double timeTaken) : this(repairID, repairOrder, car, fault,
                                                       solution, receptionDate,repairDate, 
                                                       timeTaken,DefaultInvNumber){ }
        
        //Constructor that does not take in the invoice number nor repair order
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, SolutionExtended solution,
                              DateTime receptionDate, DateTime repairDate,
                              double timeTaken) : this(repairID,0,car, fault,
                                                       solution, receptionDate, repairDate,
                                                       timeTaken){ }
        //Constructor that does not take in the invoice number, reception date, repair date nor repair order
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, SolutionExtended solution,
                              double timeTaken) : this(repairID,car, fault,
                                                       solution,timeTaken,DefaultInvNumber){ }

        //Constructor that does not take in the invoice number, reception date, nor repair order
        public RepairExtended(int repairID, CarExtended car,
                              FaultExtended fault, SolutionExtended solution, DateTime receptionDate,
                              double timeTaken) : this(repairID,0,car, fault,
                                                       solution, receptionDate,
                                                       timeTaken,DefaultInvNumber){ }
        #endregion

        #region HELPER METHODS
        //Print method
        public string Print() {
            return string.Format("Order: {0}/nCar details: /n{1}/n{2}/n{3}/nReception date: {4}/nRepair Date{5}/nInvoice: {6}",
                                 this.RepairOrder,this.Car.Print(),this.Fault.Print(),this.Solution.Print(),this.ReceptionDate,
                                 this.RepairDate,this.InvoiceNumber);
        }// End of Print method

        //Method to check the repair order is valid
        public static bool CheckRepairOrder(TextBox repairOrder)
        {
            bool valid = false;
            int order;
            //Check the number passed in is a number
            if (int.TryParse(repairOrder.Text, out order))
            {
                //Check the number is within the admisible range
                if (order >= MinRepairOrder && order <= MaxRepairOrder)
                {
                    valid = true;
                }//End of if to check the number is in range
            }//End of if statement to check it is a number
            return valid;
        }//End of CheckRepairOrder method

        //Method to check the invoice number is valid
        public static bool CheckInvoice(TextBox inv,out int invoice)
        {
            bool valid = false;
            if (int.TryParse(inv.Text.Trim(), out invoice))
            {
                if (invoice >= MinInvoice && invoice <= MaxInvoice)
                {
                    valid = true;
                }
            }
            return valid;
        }// End of CheckiInvoice mehtod

        #endregion
    }// End of RepairExtended class
}
