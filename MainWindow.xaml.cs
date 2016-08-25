using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrankHogan_WarrantySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Establish data context
        private static DataClasses1DataContext DB = new DataClasses1DataContext();

        // Enum for mileage unit
        public enum distanceUnits {Km,mi};

        //Creates a list object to store makes the dealership is currently dealing with
        private static List<MakeExtended> makeList;

        //Creates a list object to store claim types valid for warranty claiming
        private static List<ClaimTypeExtended> claimTypeList;

        // Creates a list object to store claim status currently in use
        private List<StatusExtended> statusList;
      
        // Creates a list object to store the possible vehcile subgroups
        private List<GroupExtended> groupList;

        //Double variable to hold the result of trying to parse the time allowed input by user
        private double timeAllowed;
        //Decimal variable to hold the result of trying to parse the claim value input by user
        private decimal claimValue;
        //Int variable to hold the result of trying parse the mileage input by user
        private int mileage;
        //Double variable to hold the result of trying parse the time taken input by user
        private double timeTaken;
        //int variable to hold the result of trying parse the invoice number input by user
        private int invoice;
        
      
        public MainWindow()
        {
            //Get the makes available in the database and list them in a list object
            makeList = GetMakes();
            //Get the claim types available in the database and list them in a list object
            claimTypeList = GetClaimTypes();

            //These lists have to be populate before the PieChartModel object is instantiated to avoid null value exception

            //Continue with normal initialization
            InitializeComponent();
        }// End of MainWindoe() method


        #region EVENT HANDLERS
        //Load Event handler for main window. Queries the database and populates combo boxes
        private void FrankHogan_WarrantySystem_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the claim status available in the database and list them in a list object
            statusList = GetClaimStatus();

            //Get vehicle subsystems where a fault can be developed
            groupList = GetGroups();

            //Populaie the comboxes that will store the name of the different makes
            this.PopulateMakes(this.cbxMakeT1);
            this.PopulateMakes(this.cbxMake);
            this.PopulateMakes(this.cbxMakeT3);
            this.PopulateMakes(this.cbxMakeOthersT3);

            //Populate the combo boxes that will store claim types
            this.PopulateClaimTypes(this.cbxClaimTypeT1);
            this.PopulateClaimTypes(this.cbxClaimType);
            this.PopulateClaimTypes(this.cbxClaimTypeT3);
            this.PopulateClaimTypes(this.cbxClaimTypeOthersT3);

            //Populate the combo boxes that will store claim status
            this.PopulateClaimStatus(this.cbxStatusT1);
            this.PopulateClaimStatus(this.cbxStatus);
            this.PopulateClaimStatus(this.cbxStatusT3);

            //Populate the combo boxes that will store groups affected
            this.PopulateGroups(this.cbxGroup);
            this.PopulateGroups(this.cbxGroupT3);

            //Populate the combo boxes that will store the milage units
            this.PopulateMileageUnits(this.cbxMileage);

            //Populate the combo boxes that will store faults
            this.PopulateFaults(this.cbxFault);

            //Populate the combo boxes that will store solutions
            this.PopulateSolutions(this.cbxSolution);
        }// End of event loaded event handler for the main window

        //Click Event handler for the New Claim button
        private void btnNewClaim_Click(object sender, RoutedEventArgs e)
        {
            //Declare error message  and boolean flag variables 
            bool error = false;
            string errorMessage="";
            //Invoke the input validation method for the claim form section
            error = this.ValidateClaimFormInput(ref errorMessage);
            //Check if any error occured and present error message if so
            if (error)
            {
                MessageBox.Show(errorMessage);
            }// In case all inputs are valid
            else {
                //Declare variables to store values passed out from helper methods
                int? faultID = null;
                int? solutionID = null;

                //Check the fault and solution selection (new solution and faoult? or already in system?)
                this.CheckFaultAndSolutionSelections(this.chbxNewFault,this.cbxFault,this.tbxNewFault,this.chbxNewSolution,this.cbxSolution,this.tbxNewSolution,out faultID,out solutionID);             

                //Call stored procedure to begin rollback transaction for inserting a new claim
                try
                {
                    DB.sp19InsertClaimTransaction(this.tbxVIN.Text.Trim(),this.mileage,this.FindMakeByName(this.cbxMake.SelectedItem.ToString()).Make_ID,
                                                  this.tbxRegNumber.Text,faultID,this.tbxNewFault.Text,this.cbxGroup.SelectedItem.ToString().Substring(0,2),
                                                  solutionID,this.tbxNewSolution.Text,int.Parse(this.tbxRepairOrder.Text),this.dpReceptionDate.SelectedDate,
                                                  this.dpRepairDate.SelectedDate,this.timeTaken,this.invoice,ClaimExtended.CreateClaimNo(this.tbxClaimNo1.Text.Trim(),this.tbxClaimNo2.Text.Trim()),
                                                  this.cbxClaimType.SelectedItem.ToString().Substring(0,4),this.dpClaimDate.SelectedDate,this.claimValue,this.timeAllowed,
                                                  this.FindStatusByDescription(this.cbxStatus.SelectedItem.ToString()).Status_ID,this.dpPaymentDate.SelectedDate,this.tbxManuCode.Text.Trim(),
                                                  this.tbxComments.Text.Trim());
                    //Update the fault and solution combo boxes in case a new fault or solution has been inserted
                    this.PopulateFaults(this.cbxFault);
                    this.PopulateSolutions(this.cbxSolution);
                    //Refresh charts
                    this.RefreshCharts();
                    //Show message to confirm the transaction was successful
                    MessageBox.Show(string.Format("Claim {0}-{1} has been successfully recorded!", this.tbxClaimNo1.Text, this.tbxClaimNo2.Text));
                    //Call method to clear screen
                    this.ClearClaimForm();
                }
                catch
                {
                    MessageBox.Show("Error when registering new claim. Try again and make sure all detials are valid.");
                }//End of try catch block for inserting a new claim in the database
            }// End of if else statement for input validation and claim registering
        }// End of Click Event handler for the New Claim button

        //Click event handler for update claim button
        private void btnUpdateClaim_Click(object sender, RoutedEventArgs e)
        {
            //Declare error message  and boolean flag variables 
            bool error = false;
            string errorMessage = "";
            //Invoke the input validation method for the claim form section
            error = this.ValidateClaimFormInput(ref errorMessage);
            if (error)
            {
                MessageBox.Show(errorMessage);
            }// In case all inputs are valid
            else
            {
                //Declare variables to store values passed out from helper methods
                int? faultID = null;
                int? solutionID = null;

                //Check the fault and solution selection (new solution and faoult? or already in system?)
                this.CheckFaultAndSolutionSelections(this.chbxNewFault, this.cbxFault, this.tbxNewFault, this.chbxNewSolution, this.cbxSolution, this.tbxNewSolution, out faultID, out solutionID);

                //Try to get data off database
                try
                {
                    //Call the stored procedure used to update a specific claim
                    DB.sp26UpdateClaimTransaction(this.tbxVIN.Text.Trim(), this.mileage, this.FindMakeByName(this.cbxMake.SelectedItem.ToString()).Make_ID,
                                                      this.tbxRegNumber.Text, faultID, this.tbxNewFault.Text, this.cbxGroup.SelectedItem.ToString().Substring(0, 2),
                                                      solutionID, this.tbxNewSolution.Text, int.Parse(this.tbxRepairOrder.Text), this.dpReceptionDate.SelectedDate,
                                                      this.dpRepairDate.SelectedDate, this.timeTaken, this.invoice, ClaimExtended.CreateClaimNo(this.tbxClaimNo1.Text.Trim(), this.tbxClaimNo2.Text.Trim()),
                                                      this.cbxClaimType.SelectedItem.ToString().Substring(0, 4), this.dpClaimDate.SelectedDate, this.claimValue, this.timeAllowed,
                                                      this.FindStatusByDescription(this.cbxStatus.SelectedItem.ToString()).Status_ID, this.dpPaymentDate.SelectedDate, this.tbxManuCode.Text.Trim(),
                                                      this.tbxComments.Text.Trim());                  
                    //Update the fault and solution combo boxes in case a new fault or solution has been inserted
                    this.PopulateFaults(this.cbxFault);
                    this.PopulateSolutions(this.cbxSolution);
                    //Refresh charts
                    this.RefreshCharts();
                    //Show message to confirm the transaction was successful
                    MessageBox.Show(string.Format("Claim {0}-{1} has been successfully updated!", this.tbxClaimNo1.Text, this.tbxClaimNo2.Text));
                    //Call method to clear screen
                    this.ClearClaimForm();
                }//End of try block
                 //In case the update process go wrong 
                catch
                {
                    MessageBox.Show("Error when updating claim. Try again and make sure all detials are valid.");
                }//End of cath blok
            }//End of if else statement that checks if there is any error during input validation            
        }//End of Click event handler for the Update claim button

        //Click event handler for search claim button
        private void btnSearchClaim_Click(object sender, RoutedEventArgs e)
        {

            //Define error message,error boolean flag and variable to be passed in as arguments
            bool error = false;
            string errorMessage, claimNo, VIN;
            //Input validation: check the search citeria has been input
            error = this.CheckClaimSearchCriteria(this.tbxClaimNo1, this.tbxClaimNo2, this.tbxVIN, out claimNo, out VIN, out errorMessage);
            if (!error)
            {
                //Call stored procedure that returns claim data by passing in claim number and VIN
                try
                {
                    var claimData = (from Claim in DB.sp9ClaimDetails(claimNo, VIN)
                                     select new
                                     {
                                         Claim.ClaimDate,
                                         Claim.ClaimType_ID,
                                         Claim.TypeDescription,
                                         Claim.ClaimValue,
                                         Claim.Comments,
                                         Claim.Description,
                                         Claim.Fault,
                                         Claim.Group_ID,
                                         Claim.GroupDescription,
                                         Claim.InvoiceNumber,
                                         Claim.ManuCode,
                                         Claim.Mileage,
                                         Claim.Name,
                                         Claim.PaymentDate,
                                         Claim.ReceptionDate,
                                         Claim.RegNumber,
                                         Claim.RepairDate,
                                         Claim.RepairOrder,
                                         Claim.Solution,
                                         Claim.TimeAllowed,
                                         Claim.TimeTaken
                                     }).Single();
                    if (claimData != null)
                    {
                        //Update input fields with data retrieved
                        this.dpClaimDate.SelectedDate = claimData.ClaimDate;
                        this.SelectCBoxText(this.cbxClaimType, string.Concat(claimData.ClaimType_ID, "--", claimData.TypeDescription));
                        this.tbxClaimValue.Text = string.Format("{0:F2}", claimData.ClaimValue);
                        this.tbxComments.Text = claimData.Comments;
                        this.SelectCBoxText(this.cbxStatus, claimData.Description);
                        this.SelectCBoxText(this.cbxFault, claimData.Fault);
                        this.SelectCBoxText(this.cbxGroup, string.Concat(claimData.Group_ID, "--", claimData.GroupDescription));
                        this.tbxInvoice.Text = claimData.InvoiceNumber.ToString();
                        this.tbxManuCode.Text = claimData.ManuCode.ToString();
                        this.tbxMileage.Text = claimData.Mileage.ToString();
                        this.cbxMileage.SelectedIndex = 0;
                        this.tbxRegNumber.Text = claimData.RegNumber;
                        this.SelectCBoxText(this.cbxMake, claimData.Name);
                        this.dpPaymentDate.SelectedDate = claimData.PaymentDate;
                        this.dpReceptionDate.SelectedDate = claimData.ReceptionDate;
                        this.dpRepairDate.SelectedDate = claimData.RepairDate;
                        this.tbxRepairOrder.Text = claimData.RepairOrder.ToString();
                        this.SelectCBoxText(this.cbxSolution, claimData.Solution);
                        this.tbxTimeAllowed.Text = claimData.TimeAllowed.ToString();
                        this.tbxTimeTaken.Text = claimData.TimeTaken.ToString();

                        //If a claim was found the new claim button is disabled and the update and delete buttons are enabled
                        //Disable the new claim button 
                        this.btnNewClaim.IsEnabled = false;
                        //Enable other two buttons
                        this.btnUpdateClaim.IsEnabled = true;
                        this.btnDeleteClaim.IsEnabled = true;
                        this.btnClearForm.IsEnabled = true;
                        this.btnSearchClaim.IsEnabled = false;
                        //The claim number fields are set to disabled so the number cannot be changed
                        this.tbxClaimNo1.IsEnabled = false;
                        this.tbxClaimNo2.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Claim not found for the search cirteria provided!");
                    }//End of if else statement
                }//End of try block
                catch
                {
                    MessageBox.Show("Error while communicating with database!");
                }//End of catch block
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }// End of click event handler for search claim button

        //Click event handler for the clear claim form button
        private void btnClearForm_Click(object sender, RoutedEventArgs e)
        {
            this.ClearClaimForm();
        }//End of Click event handler for the clear claim form button

        //Click event handler for load data button
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //Create the new child window to load some claim data from a file
            LoadDataWindow loadWindow = new LoadDataWindow();
            loadWindow.Owner = this;
            loadWindow.ShowDialog();
        }//End of Click event handler for load data button
        
        //Click event handler for Delete claim button
        private void btnDeleteClaim_Click(object sender, RoutedEventArgs e)
        {
            //Check that a claim with claim number and VIN input by user does exist in the database
             var query = (from Claim in DB.Claims
                         join Repair in DB.Repairs on Claim.Repair_ID equals Repair.Repair_ID
                         where (Claim.ClaimNumber == ClaimExtended.CreateClaimNo(this.tbxClaimNo1.Text.Trim(),this.tbxClaimNo2.Text.Trim()) && Repair.VIN == this.tbxVIN.Text.Trim())
                         select Claim.Claim_ID);
            if (query != null) {
                try
                {
                    //Call the store procedure that deletes an specific claim number
                    DB.sp27DeleteClaim(ClaimExtended.CreateClaimNo(this.tbxClaimNo1.Text.Trim(), this.tbxClaimNo2.Text.Trim()), this.tbxVIN.Text.Trim());
                    //Refresh charts
                    this.RefreshCharts();
                    //Show message to confirm the transaction was successful
                    MessageBox.Show(string.Format("Claim {0}-{1} has been successfully deleted!", this.tbxClaimNo1.Text, this.tbxClaimNo2.Text));
                    //Call method to clear screen
                    this.ClearClaimForm();
                }//End of try block

                catch
                {
                    MessageBox.Show("Error while trying to delete the claim");
                }// End of catch block
            }
            else {
                MessageBox.Show("There is no claim for the specified VIN and claim number!");
            }//End of if else statement to check the claim_ID result
        }// End of click event handler for Delete claim button

        //Event handler for checking the New Solution check box
        private void chbxNewSolution_Checked(object sender, RoutedEventArgs e)
        {
            this.tbxNewSolution.IsEnabled = true;
            this.cbxSolution.SelectedItem = null;
            this.cbxSolution.IsEnabled = false;
        }// End of event handler for checking the New Solution check box

        //Event handler for unchecking the New Solution check box
        private void chbxNewSolution_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tbxNewSolution.Text = "";
            this.cbxSolution.IsEnabled = true;
        }// End of event handler for unchecking the New Solution check box

        //Event handler for checking the New Fault check box
        private void chbxNewFault_Checked(object sender, RoutedEventArgs e)
        {
            this.tbxNewFault.IsEnabled = true;
            this.cbxFault.SelectedItem = null;
            this.cbxFault.IsEnabled = false;
        }// End of event handler for checking the New Fault check box

        //Event handler for unchecking the New Fault check box
        private void chbxNewFault_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tbxNewFault.Text = "";
            this.cbxFault.IsEnabled = true;
        }// End of event handler for unchecking the New Fault check box

        //Event handler for ticking off the make filter to calculate KPIs (Tab 3)
        private void chbxMakeOthersT3_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxMakeOthersT3.IsEnabled = true;
        }// End of Event handler for ticking off the make filter to calculate KPIs (Tab 3)

        //Event handler for unchecking the make filter to calculate KPIs (Tab 3)
        private void chbxMakeOthersT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxMakeOthersT3.SelectedItem = null;
            this.cbxMakeOthersT3.IsEnabled = false;
        }//End of Event handler for unchecking the make filter to calculate KPIs (Tab 3)

        //Event handler for ticking off the claim type filter to calculate KPIs (Tab 3)
        private void chbxClaimTypeOthersT3_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxClaimTypeOthersT3.IsEnabled = true;
        }// End of Event handler for ticking off the claim filter to calculate KPIs (Tab 3)

        //Event handler for unchecking the claim type filter to calculate KPIs (Tab 3)
        private void chbxClaimTypeOthersT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxClaimTypeOthersT3.SelectedItem = null;
            this.cbxClaimTypeOthersT3.IsEnabled = false;
        }//End of Event handler for ticking off the claim type filter to calculate KPIs (Tab 3)

        //Event handler for selection changed in the Make combo box (on Tab 1)
        private void cbxMake_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Call method to check the make and change the image
            this.ChangeImage(cbxMake, imgLogoTab2);
        }// End of Make combo box selection changed event handler

        //Event handler for selection changed in the Make combo box (on Tab 3, Claims filter section)
        private void cbxMakeT3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Call method to check the make and change the image
            this.ChangeImage(cbxMakeT3, imgLogoTab3);
        }// End of Make combo box selection changed event handler

        //Event handler for selection changed in the Make combo box (on Tab 3, KPI calculation section)
        private void cbxMakeOthersT3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Call method to check the make and change the image
            this.ChangeImage(cbxMakeOthersT3, imgLogoTab3);
        }// End of Make combo box selection changed event handler

        //Click Event handler for calculate KPIs button on tab 3
        private void btnCalculateKPIS_Click(object sender, RoutedEventArgs e)
        {
            //Declare variables to work within the method.
            bool error = false;
            string errorMessage = "The following error(s) has(have) been found:\n";
            //Variables to call the stored procedure that calculates the speed KPI
            DateTime? startDate = null, endDate = null;
            double? speed = null;
            double? performace = null;
            int? makeID= null;
            MakeExtended make=null;
            //Varaibles to call the stored procedure that calculates the workshop performace KPI
            string claimTypeID="%";
            //Clear the previous data displayed in the labels
            this.lblPerformanceKPIValueT3.Content = "";
            this.lblSpeedKPIValueT3.Content = "";

            //1.- Check if dates are valid
            if (this.ValidateFilterDates(dpStartDateOthersT3, dpEndDateOthersT3, ref errorMessage))
            {
                startDate = this.dpStartDateOthersT3.SelectedDate;
                endDate = this.dpEndDateOthersT3.SelectedDate;
            }
            else
            { 
               this.SetErrorMessage(ref error,ref errorMessage,"");
            }

            //2.- Check if make filtering is to be applied
            if (this.CheckFiltringOption(this.chbxMakeOthersT3, this.cbxMakeOthersT3)) {
                //Check a make has been selected from drop down list
                if (this.chbxMakeOthersT3.IsChecked == true)
                {
                    //Get the make by passing in the name from the drop down list
                    make = this.FindMakeByName(this.cbxMakeOthersT3.SelectedItem.ToString());
                    //Make the makeID equals the Make_ID property of the new make object
                    makeID = make.Make_ID;
                }
            }
            else
            {
                //Register an error message
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Make invalid. A make must be selected from the make list.\n");
            }// End of if statement that checks the make filtering option
           
            //3.- Check if any error has come up so far
            if (!error) {
                // Call the store procedure to calculate the speed KPI
                DB.sp2CalculateClaimingSpeed(startDate, endDate, makeID, ref speed);
            }
            
            //4.- Check the result got fromt he sp is not null
            if (speed != null)
            {
                //Update the user interface with the result obtained
                this.lblSpeedKPIValueT3.Content = speed;
            }
            //If no result was retrieved an error messaged is prompted
            else
            {
                this.SetErrorMessage(ref error,ref errorMessage, "\t*Unable to calculate speed KPI for period of time specified.\n");
            }// End of if else statement that check the claiming speed sp result

            //5.- Check the claim type filtering option has been selected
            if (this.CheckFiltringOption(this.chbxClaimTypeOthersT3, this.cbxClaimTypeOthersT3))
            {
                if (this.chbxClaimTypeOthersT3.IsChecked == true)
                {
                    //If the option has been ticked off, the claim type is to be gotten by taking the first 4 characters from the selected item in the dd list
                    claimTypeID = this.cbxClaimTypeOthersT3.SelectedItem.ToString().Substring(0, 4);
                }
            }
            else
            {
                //Register an error message
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim type invalid. An option must be selected from the \n\tclaim type list.\n");
            }// End of if else statement that checks if a claim type has been selected correctly from the drop down list
        
            //6.- Check if any error has come up so far
            if (!error) {
                //Call the stored procedure to calculate the workshop performace index
                DB.sp3CalculateWSPerformance(startDate, endDate, makeID, claimTypeID, ref performace);
            }
            //7.- Check the result retrieved is not null
            if (performace != null)
            {
                //Update the user interface with the value returned by the stored procedure
                this.lblPerformanceKPIValueT3.Content = performace;
            } // If the value is null, an error message is prompted
            else
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Unable to calculate the workshop performance for the filter criteria.\n");
            }// End of if else statement that checks the workshop performance stored procedure result
            //8.- Final check for errors and prompt message if any
            if (error)
            {
                MessageBox.Show(errorMessage);
            }// End of if statement to prompt an error message when required

        }// End of click event handler for calcualte KPIs button

        //Click event handler for the search invoice button (Tab 3)
        private void btnSearchInvoice_Click(object sender, RoutedEventArgs e)
        {
            string claimNo = null, VIN =null;
            int? invoice = null;
            bool error = false;
            string errorMessage;
            //Clear any previous invoice number shown in the UI
            this.lblInvoiceValueOthersT3.Content = "";
            //Input validation for Claim number and VIN
            //Check the claim number is valid and is not empty
            error = this.CheckClaimSearchCriteria(this.tbxClaimNo1OthersT3,this.tbxClaimNo2OthersT3,this.tbxVINOthersT3,out claimNo,out VIN,out errorMessage);
            if (!error)
            {
                //Call stored procedure that returns the invoice number by passing in the claim number and VIN
                try {
                    DB.sp6GetInvoiceNumber(claimNo,VIN,ref invoice);
                    if (invoice != null)
                    {
                        this.lblInvoiceValueOthersT3.Content = invoice.ToString();
                    }
                    else {
                        MessageBox.Show("Invoice number not found for search cirteria provided!");
                    }//End of if else statement
                
                }//End of try block
                
                catch {
                    MessageBox.Show("Error while communicating with database!");
                }// End of catch block
            }
            else {
                MessageBox.Show(errorMessage);
            }// End of if else to check any input error
        }// End of click event handler for search invoice button (Tab 3)

        //Click event handler for claim listing by car
        private void btnClaimListByCarT3_Click(object sender, RoutedEventArgs e)
        {
            string VIN="";
            string regNumber="";
            List<string> claimList = new List<string>();
            bool error = false;
            string errorMessage = "The following error(s) has(have) been detected: \n";
            //Input validation
            //Check one radiobutton is checked at least
            if (this.rdbtnRegNumberT3.IsChecked == false && this.rdbtnVINT3.IsChecked == false)
            {
                this.SetErrorMessage(ref error,ref errorMessage, "\t*No search criteria has been selected.\n\tPlease select the VIN or Reg number filter.\n");

            }// End of if statement that validates one radio button was checked
            if (!error)
            {
                //Query the database and populate the list box with the result set
                //Check what the filter criteria is: VIN or Reg?
                if (this.rdbtnVINT3.IsChecked == true)
                {
                    //Check the VIN is valid
                    if (!CarExtended.CheckVIN(this.tbxVINFilterT3))
                    {
                        this.SetErrorMessage(ref error, ref errorMessage, "\t*VIN invalid. Only a 17 digit aplphanumeric number is allowed.\n");
                    }
                    else {
                        //Before calling the stored procedure, check if VIN actually does exist in the database, othewise prompt a message
                        if (this.IsCarInDB(this.tbxVINFilterT3.Text.Trim()))
                        {
                            var query = DB.sp4ListOfClaimsByVIN(this.tbxVINFilterT3.Text.Trim()).ToList();
                            //Add headings for the claim list
                            claimList.Add(string.Format("{0,-15}\t{1,-15}\t{2,-15}\t{3,-15}\t{4,-60}\t{5}",
                                                        "Claim", "Type", "Claim Value", "Claim Date", "Status", "Fault"));
                            foreach (var claim in query)
                            {
                                VIN = claim.VIN;
                                regNumber = claim.Registration_number;
                                
                                claimList.Add(string.Format("{0,-15}\t{1,-15}\t{2,-15:F2}\t{3,-15}\t{4,-60}\t{5}",
                                                            claim.ClaimNumber,claim.ClaimType_ID,claim.Claim_Value,
                                                            Convert.ToDateTime(claim.Claim_Date).ToShortDateString(),
                                                            claim.Repair,claim.Status));
                            }//End of foreach loop
                        }
                        else
                        {
                            this.SetErrorMessage(ref error, ref errorMessage, "\t*VIN not registered in the database. Try a different VIN.\n");
                        }// End of if else statement that checks the VIN does exist in the database
                    }//End of input validation of VIN
                    
                }//End of if statement that checks the VIN radio button was selected
                else if (this.rdbtnRegNumberT3.IsChecked == true)
                {
                    //Check the reg number is valid
                    if (!CarExtended.CheckRegNumber(this.tbxRegNumberFilterT3))
                    {
                        this.SetErrorMessage(ref error, ref errorMessage, string.Format("\t*Registration number invalid. Alphanumeric number between {0} and {1}\n\t digist is allowed.\n", CarExtended.MinRegLength, CarExtended.MaxRegLength));
                    }
                    else {
                        //Before calling the stored procedure, check if VIN actually does exist in the database, othewise prompt a message
                        if (this.FindReg(this.tbxRegNumberFilterT3.Text.Trim()))
                        {
                            var query = DB.sp5ListOfClaimsByReg(this.tbxRegNumberFilterT3.Text.Trim()).ToList();
                            //Add headings for the claim list
                            claimList.Add(string.Format("{0,-15}\t{1,-15}\t{2,-15}\t{3,-15}\t{4,-60}\t{5}",
                                                        "Claim", "Type", "Claim Value", "Claim Date", "Status", "Fault"));
                            foreach (var claim in query)
                            {
                                VIN = claim.VIN;
                                regNumber = claim.Registration_number;

                                claimList.Add(string.Format("{0,-15}\t{1,-15}\t{2,-15:F2}\t{3,-15}\t{4,-60}\t{5}",
                                                            claim.ClaimNumber, claim.ClaimType_ID, claim.Claim_Value,
                                                            Convert.ToDateTime(claim.Claim_Date).ToShortDateString(),
                                                            claim.Repair, claim.Status));
                            }// End of foreach loop  
                        }
                        else
                        {
                            this.SetErrorMessage(ref error, ref errorMessage, "\t*Registration number not registered in the database.\n\tTry a different Registration.\n");
                        }// End of if else statement that checks the Reg does exist in the database
                    }//End of input validation of Reg number
                }//End of if statement that checks the Reg radio button was selected

            }//End of if statement that check there was no error before querying the DB
            if (!error)
            {
                //Clear data in UI
                this.ClearClaimFilterByCarSection();
                //Create the new child window
                ClaimList listing = new ClaimList(claimList,VIN,regNumber);
                listing.Owner = this;
                listing.ShowDialog();
            }
            else
            {
                MessageBox.Show(errorMessage);
            }//End of if statement to check if no error exists after checking one radio button was checked at least
            

        }// End of click event handler for search claims by car button

        //Event handler for checking the VIN radio button
        private void rdbtnVINT3_Checked(object sender, RoutedEventArgs e)
        {
            //Clear the text boxes
            this.tbxVINFilterT3.Text = "";
            this.tbxRegNumberFilterT3.Text = "";
            //Enable thej VIN text box and disable the reg text box
            this.tbxVINFilterT3.IsEnabled = true;
            this.tbxRegNumberFilterT3.IsEnabled = false;
        }// End of event handler for checking the VIN radio button

        //Event handler for checking the Reg radio button
        private void rdbtnRegNumberT3_Checked(object sender, RoutedEventArgs e)
        {
            //Clear the text boxes
            this.tbxVINFilterT3.Text = "";
            this.tbxRegNumberFilterT3.Text = "";
            //Enable thej VIN text box and disable the reg text box
            this.tbxVINFilterT3.IsEnabled = false;
            this.tbxRegNumberFilterT3.IsEnabled = true;
        }// End of event handler for checking the Reg radio button

        //Event handler for checking the make option to filter claims (Tab 3)
        private void chbxMakeT3_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxMakeT3.IsEnabled = true;
        }//End of event handler for checking the make filter to filter claims (Tab 3)

        //Event handler for unchecking the make option to filter claims (Tab 3)
        private void chbxMakeT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxMakeT3.SelectedItem = null;
            this.cbxMakeT3.IsEnabled = false;
        }//End of event handler for unchecking the make option to filter claims (Tab 3)

        //Event handler for checking the claim type option to filter claims (Tab 3)
        private void chbxClaimTypeT3_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxClaimTypeT3.IsEnabled = true;
        }//End of Event handler for checking the claim type option to filter claims (Tab 3)

        //Event handler for unchecking the claim type option to filter claims (Tab 3)
        private void chbxClaimTypeT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxClaimTypeT3.SelectedItem = null;
            this.cbxClaimTypeT3.IsEnabled = false;
        }//End of event handler for unchecking the claim type option to filter claims (Tab 3)

        //Event handler for checking the group option to filter claims (Tab 3)
        private void chbxGroupT3_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxGroupT3.IsEnabled = true;
        }//End of event handler for checking the group option to filter claims (Tab 3)

        //Event handler for unchecking the group option to filter claims (Tab 3)
        private void chbxGroupT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxGroupT3.SelectedItem = null;
            this.cbxGroupT3.IsEnabled = false;
        }//End of event handler for unchecking the group option to filter claims (Tab 3)

        //Event handler for checking the status option to filter claims (Tab 3)
        private void chbxStatus_Checked(object sender, RoutedEventArgs e)
        {
            this.cbxStatusT3.IsEnabled = true;
        }//End of event handler for checking the status option to filter claims (Tab 3)

        //Event handler for unchecking the status option to filter claims (Tab 3)
        private void chbxStatus_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbxStatusT3.SelectedItem = null;
            this.cbxStatusT3.IsEnabled = false;
        }//End of event handler for unchecking the status option to filter claims (Tab 3)

        //Event handler for checking the fault option to filter claims (Tab 3)
        private void chbxFault_Checked(object sender, RoutedEventArgs e)
        {
            this.tbxFaultT3.IsEnabled = true;
        }//End of event handler for checking the fault option to filter claims (Tab 3)

        //Event handler for unchecking the fault option to filter claims (Tab 3)
        private void chbxFault_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tbxFaultT3.Text = "";
            this.tbxFaultT3.IsEnabled = false;
        }//End of event handler for unchecking the fault option to filter claims (Tab 3)

        //Event handler for checking the value option to filter claims (Tab 3)
        private void chbxValueT3_Checked(object sender, RoutedEventArgs e)
        {
            this.tbxMaxValueT3.IsEnabled = true;
            this.tbxMinValueT3.IsEnabled = true;
        }//End of event handler for checking the value option to filter claims (Tab 3)

        //Event handler for unchecking the value option to filter claims (Tab 3)
        private void chbxValueT3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tbxMaxValueT3.Text = "";
            this.tbxMinValueT3.Text = "";
            this.tbxMaxValueT3.IsEnabled = false;
            this.tbxMinValueT3.IsEnabled = false;
        }//End of veent handler for unchecking the value option to filter claims (Tab 3)

        //Event handler for cliking the filter claims button (on Tab 3)
        private void btnClaimsFilterT3_Click(object sender, RoutedEventArgs e)
        {
            //Declare variables to work within the method.
            bool error = false;
            string errorMessage = "The following error(s) has(have) been found:\n";
            List<string> claimsFiltered = new List<string>();
            //Define variables to call the store procedure that return the result set of filtered claims
            DateTime? startDate = null, endDate = null;
            MakeExtended make = null;
            int? makeID = null;
            decimal? minValue = null, maxValue = null;
            decimal parseResult;
            string claimTypeID = "%";
            StatusExtended status = null;
            int? statusID=null;
            string groupID = "%";
            string fault = null;
            //Check if start date is null
            if (this.dpStartDateT3.SelectedDate != null)
            {
                startDate = this.dpStartDateT3.SelectedDate;
            }
            else {
                var minDate = (from Claims in DB.Claims
                               select Claims.ClaimDate).Min();
                startDate = minDate;
            }
            // Check if endDate is null
            if (this.dpEndDateT3.SelectedDate != null)
            {
                endDate = this.dpEndDateT3.SelectedDate;
            }
            else {
                var maxDate = (from Claims in DB.Claims
                              select Claims.ClaimDate).Max();
                endDate = maxDate;
            }
            //1.- Check the dates are valid (if they are not null)
            if (this.ValidateFilterDates(dpStartDateT3, dpEndDateT3, ref errorMessage))
            {
                startDate = this.dpStartDateOthersT3.SelectedDate;
                endDate = this.dpEndDateOthersT3.SelectedDate;
            }
            else
            {
                this.SetErrorMessage(ref error, ref errorMessage, "");
            }//End of date validation
           

            //2.- Check if make filtering is to be applied
            if (this.CheckFiltringOption(this.chbxMakeT3, this.cbxMakeT3))
            {   //Check a make has been selected from drop down list
                if (this.chbxMakeT3.IsChecked == true)
                {
                    //Get the make by passing in the name from the drop down list
                    make = this.FindMakeByName(this.cbxMakeT3.SelectedItem.ToString());
                    //Make the makeID equals the Make_ID property of the new make object
                    makeID = make.Make_ID;
                }
            }
            else {
                //Register an error message
                this.SetErrorMessage(ref error,ref errorMessage, "\t*Make invalid. A make must be selected from the make list.\n");
            }// End of if statement that checks the make filtering option

            //3.- Check the claim type filtering option has been selected
            if (this.CheckFiltringOption(this.chbxClaimTypeT3, this.cbxClaimTypeT3)) {
                if (this.chbxClaimTypeT3.IsChecked == true) {
                    //If the option has been ticked off, the claim type is to be gotten by taking the first 4 characters from the selected item in the dd list
                    claimTypeID = this.cbxClaimTypeT3.SelectedItem.ToString().Substring(0, 4);
                }
            }
            else
            {
                //Register an error message
                this.SetErrorMessage(ref error,ref errorMessage, "\t*Claim type invalid. An option must be selected from the \n\tclaim type list.\n");               
            }// End of if else statement that checks if a claim type has been selected correctly from the drop down list
            

            //4.- Check the group filtering option has been selected
            if (this.CheckFiltringOption(this.chbxGroupT3, this.cbxGroupT3))
            {
                if (this.chbxGroupT3.IsChecked == true) {
                    //Get the groupID number by triming the first two digits of the text selected from the dd list
                    groupID = this.cbxGroupT3.SelectedItem.ToString().Substring(0, 2);
                }
            }
            else {
                //Register an error message
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Group invalid.An option must be selected from the \n\tgroup list.\n");
            }//End of if  statement that checks group filtering option

            //5.- Check the Status filtering option has been selected
            if (this.CheckFiltringOption(this.chbxStatusT3,this.cbxStatusT3)) {
                if (this.chbxStatusT3.IsChecked == true) {
                    //Get the Status ID by calling a method that recieves the status description and returns the Status object
                    status = this.FindStatusByDescription(this.cbxStatusT3.Text);
                    statusID = status.Status_ID;
                }
            }
            else {
                //Register an error message
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Status invalid. An option must be selected from the \n\tstatus list.\n ");

            }//End of if  statement that checks status filtering option

            //6.- Check the Fault filtering option has been selected 
            if (this.chbxFaultT3.IsChecked == true) {
                if (this.tbxFaultT3.Text.Trim() != "") {
                    fault = this.tbxFaultT3.Text.Trim();
                }
                else
                {
                    //Register an error message
                    this.SetErrorMessage(ref error, ref errorMessage, "\t*Fault invalid. Pleae input a fault description to be searched.\n");
                }
            }// End of if  statement that checks fault filtering option


            //7.- Check the Value filtering option has been selected 
            if (this.chbxValueT3.IsChecked == true) {
                //Check both max and min values are not empty
                if (this.tbxMinValueT3.Text.Trim() != "" && this.tbxMaxValueT3.Text.Trim()!="") {
                    //Check the value entered in min value is a real number
                    if (decimal.TryParse(tbxMinValueT3.Text, out parseResult))
                    {
                        //If it is, set the nullabe decimal varaible minValue to be equal to the number just parsed
                        minValue = parseResult;
                        //Check the value entered in max value is a real number
                        if (decimal.TryParse(tbxMaxValueT3.Text, out parseResult))
                        {
                            //If it is a real number, set the nullabe decimal varaible maxValue to be equal to the number just parsed
                            maxValue = parseResult;
                        }// If maxValue is not a real number prompt an error message
                        else
                        {
                            this.SetErrorMessage(ref error, ref errorMessage, "\t*Invalid max value. Only real numbers can be entered in max value field.\n");
                        }//End of if else statement that checks maxValue field
                    }//If minValue is not a real number prompt an error message
                    else
                    {
                        this.SetErrorMessage(ref error, ref errorMessage, "\t*Invalid min value. Only real numbers can be entered in min value field.\n");
                    }// End of if that checks the numbers are valid
                }//If both minValue and maxValue fields are empty prompt the proper message
                else {
                    this.SetErrorMessage(ref error, ref errorMessage,"\t*Invalid values. Enter numbers for min and max claim value.\t");
                }// End of if else statement that checks the text input in max and min values are not empty
            }// End of if  statement that checks value filtering option

            //8.- Check if any error has come up so far and show message is any
            if (!error) {
                //Call the store procedure to retrive the list of claims as per filter criteria
                var query = DB.sp20FilterClaims(startDate,endDate,statusID,makeID,claimTypeID,groupID,minValue,maxValue,fault).ToList();

                claimsFiltered.Add(string.Format("{0,-15}\t{1,-15}\t{2,-17}\t{3,-30}\t{4,-15}\t{5,-15}\t{6,-15}\t{7,-15}\t{8,-15:F2}\t{9,-5:F2}\t{10,-5:F2}\t{11,-15}\t{12,-60}\t{13,-60}",
                                       "Claim","Type","VIN Number","Reg Number","Make","Mileage",
                                       "Repair Date","Claim Date","Claim Value","Time Taken",
                                       "Time Allowed","Invoice","Fault","Status"));
                foreach (var claim in query)
                {
                    claimsFiltered.Add(string.Format("{0,8}\t{1,4}\t{2,17}\t{3,-30}\t{4,-15}\t{5,-15}\t{6}\t{7}\t{8,-15:F2}\t{9,-20:F2}\t{10,-20:F2}\t{11,-5}\t{12,-60}\t{13,-60}",
                                       claim.ClaimNumber, claim.ClaimType_ID, claim.VIN, claim.RegNumber, claim.Make, claim.Mileage,
                                       claim.RepairDate.ToShortDateString(), Convert.ToDateTime(claim.ClaimDate).ToShortDateString(), claim.ClaimValue, claim.TimeTaken,
                                       claim.TimeAllowed, claim.InvoiceNumber, claim.Fault, claim.Status));
                }//End of foreach loop 

                //Clear data in UI

                //Create the new child window
                ClaimList listing = new ClaimList(claimsFiltered);
                listing.Owner = this;
                listing.ShowDialog();
            }
            else {
                MessageBox.Show(errorMessage);              
            }
        }// End of event handler for cliking the filter claims button (on Tab 3)

        //Click event handler for Update charts button
        private void btnUpdateCharts_Click(object sender, RoutedEventArgs e)
        {
            //Validate user input
            bool valid=false;
            string errorMessage = "The following error(s) has(have) been found:\n";
            //Invoke method that check dates input are valid
            valid = this.ValidateFilterDates(this.dpStartDateT1,this.dpEndDateT1,ref errorMessage);
            if (valid)
            {
                //Call method to update charts
                this.RefreshCharts();
            }
            else {
                MessageBox.Show(errorMessage);
            }//End of if else statement to check input was valid
            
        }//End of click event handler for update charts button
        #endregion

        #region HELPER METHODS
        //Method to populate the Make combo boxes
        private void PopulateMakes(ComboBox ddList)
        {
            //Declare and instantiate a new string list to store the claim type descriptions
            List<string> makes = new List<string>();
            //foreach loop to go through the MakeExtended list that stores all the Make objects
            foreach (MakeExtended make in makeList)
            {
                //Only the active makes will be included in the drop down lists
                if (make.Status.Equals(MakeExtended.MakeStatus.Active))
                {
                    makes.Add(make.Name);
                }// End of if statement to check the make status
            }// End of foreach loop
            ddList.ItemsSource = makes;
            /* //Defining the LINQ query to get a list of makes
             var makes = from Makes in DB.Makes
                         select Makes.Name;
             ddList.ItemsSource = makes.ToList();*/
        }// End of PopulateMakes method

        //Method to populate the Claim type combo boxes
        private void PopulateClaimTypes(ComboBox ddList)
        {
            //Declare and instantiate a new string list to store the claim type descriptions
            List<string> claimTypes = new List<string>();
            //foreach loop to go through the claimTypeExtended list that stores all the claim type objects
            foreach (ClaimTypeExtended type in claimTypeList)
            {
                claimTypes.Add(type.PrintComboBox());
            }// End of foreach loop
            ddList.ItemsSource = claimTypes;
            /*//Defining the LINQ query to get a claim types listing off the database
            var types = from ClaimTypes in DB.ClaimTypes
                        select new {ClaimTypes.ClaimType_ID, ClaimTypes.TypeDescription };
            List<string> claimTypeList = new List<string>();
            foreach (var type in types) {
                claimTypeList.Add(string.Concat(type.ClaimType_ID,"//",type.TypeDescription));
            }//End of foreach loop
            ddList.ItemsSource = claimTypeList;*/
        }// End of PopulateClaimTypes method

        //Method to populate the claim status combo boxes
        private void PopulateClaimStatus(ComboBox ddList)
        {
            //Declare and instantiate a new string list to stores the claim status
            List<string> claimStatus = new List<string>();
            //foreach loop to go through the StatusExtended list that store all the status objects
            foreach (StatusExtended status in this.statusList)
            {
                claimStatus.Add(status.Print());
            }// End of foreach loop
            ddList.ItemsSource = claimStatus;
            /* //Defining the LINQ query to get a status list off the database
             var statusList = from Status in DB.Status
                              select Status.Description;
             ddList.ItemsSource = statusList.ToList();*/
        }//End of PopulateCloimStatus method

        //Method to populate the group affected combo boxes
        private void PopulateGroups(ComboBox ddList)
        {
            //Declare and instantiate a new string list to stores the group descriptions
            List<string> groups = new List<string>();
            //foreach loop to go through the GroupExtended list that store all the group objects
            foreach (GroupExtended group in this.groupList)
            {
                groups.Add(group.PrintComboBox());
            }// End of foreach loop
            ddList.ItemsSource = groups;
            /*
            var groups = from GroupsAffected in DB.Groups
                         select new { GroupsAffected.Group_ID, GroupsAffected.GroupDescription };
            List<string> groupList = new List<string>();
            foreach (var group in groups) {
                groupList.Add(string.Concat(group.Group_ID,"-",group.GroupDescription));
            }// End of foreach loop
            ddList.ItemsSource = groupList;*/
        }// End of PopulateGroups method

        //Method to populate the mileage unit combo boxes
        private void PopulateMileageUnits(ComboBox ddList)
        {
            //Declare and instantiate a new string list
            List<string> units = new List<string>();
            //Add the two distance units
            units.Add(distanceUnits.Km.ToString());
            units.Add(distanceUnits.mi.ToString());
            ddList.ItemsSource = units;
        }//End of PopulateMileageUnits method

        //Method to populate the Fault combo boxes
        private void PopulateFaults(ComboBox ddList)
        {
            //Query that gets all the fault descriptions stored in the Solutions table
            var faultList = (from Faults in DB.Faults
                             orderby Faults.Fault1
                             select Faults.Fault1);
            //Send the result set to a list and make ItemsSource porperty of combobox equal to the list
            ddList.ItemsSource = faultList.ToList();
        }// End of PopulateFaults method

        //Method to populate the Solution combo boxes
        private void PopulateSolutions(ComboBox ddList)
        {
            //Query that gets all the solution descriptions stored in the Solutions table
            var solutionList = from Solutions in DB.Solutions
                               orderby Solutions.Solution1
                               select Solutions.Solution1;
            //Send the result set to a list and make ItemsSource porperty of combobox equal to the list
            ddList.ItemsSource = solutionList.ToList();
        }// End of PopulateSolutions method        

        //Method to check dates are correct
        private bool CheckDates(DateTime? claimDate, DateTime? repairDate, DateTime? receptionDate)
        {
            bool validDates = false;
            //Check that claim date is greater or equal to repair date, repair date is greater than or equal to 
            //reception date and claim date is not before present day
            if (claimDate >= repairDate && repairDate >= receptionDate && claimDate <= DateTime.Today)
            {
                validDates = true;
            }
            return validDates;
        }//End of CheckDates method

        //Method to check fault or solution input by user is not null or left in blank
        private bool CheckFaultSolution(ComboBox ddList, TextBox text)
        {
            bool valid = false;
            if (ddList.SelectedItem != null || (text.Text != "" && text.Text.Length<= FaultExtended.MaxFaultLength))
            {
                valid = true;
            }
            return valid;
        }// End of CheckFault method

        //Method to check the filtering datas are valid (end date > start date)
        private bool CheckFilterPeriod(DateTime? startDate, DateTime? endDate) {
            bool valids = false;
            if (startDate <= endDate && endDate <= DateTime.Today) {
                valids = true;
            }
            return valids;
        }// End of CheckFilterPeriod method

        //Method to find a make object in the make list by passing in the make name
        private MakeExtended FindMakeByName(string name)
        {
            //Declare make object, boolean flag and counter variables
            MakeExtended make = null;
            bool found = false;
            int i = 0;
            //Repeat while a match is not found and the postion in the list is valid
            while (!found && i < makeList.Count())
            {
                //If a match is found instantiate the MakeExtended object and set boolean flag to true
                if (makeList.ElementAt(i).Name.Equals(name))
                {
                    make = makeList.ElementAt(i);
                    found = true;
                }
                i++;
            };
            return make;
        }// End of FindMakeByName method

        //Method to check is a Status with the description passed in as argument
        private StatusExtended FindStatusByDescription(string description)
        {
            //Declare boolean flag 
            bool found = false;
            StatusExtended status = null;
            int i = 0;
            //Repeat while a match is not found and the postion in the list is valid
            while (!found && i < this.statusList.Count())
            {
                //If a match is found instantiate the MakeExtended object and set boolean flag to true
                if (this.statusList.ElementAt(i).Description.Equals(description))
                {
                    status = statusList.ElementAt(i);
                    found = true;
                }
                i++;
            };
            return status;

        }// End of FindStatusByDescription method

        //Method to clear the claim filtering by car section
        private void ClearClaimFilterByCarSection()
        {
            this.rdbtnVINT3.IsChecked = false;
            this.rdbtnRegNumberT3.IsChecked = false;
            this.tbxVINFilterT3.Text = "";
            this.tbxRegNumberFilterT3.Text = "";
            this.tbxVINFilterT3.IsEnabled = false;
            this.tbxRegNumberFilterT3.IsEnabled = false;
        }// End of ClearClaimFilterByCarSection method

        //Method to check a filtering option has been correctly selected (ticked off and option from drop down list selected)
        private bool CheckFiltringOption(CheckBox checkbox, ComboBox combobox) {
            bool valid = false;
            //Check the filter selection is valid an option ticked off and an item selected from the drop down list or no option ticked off
            if ((checkbox.IsChecked == true && combobox.SelectedItem != null) || (checkbox.IsChecked == false))
            {
                valid = true; 
            }// End of if that checks the check box is ticked off
            return valid;
        }// End of CheckFilteringOption method

        //Method to set up a new error message
        private void SetErrorMessage(ref bool error,ref string errorMessage, string msgToAdd) {
            error = true;
            errorMessage = errorMessage + msgToAdd;
        }//End of SetErrorMessage method

        //Method to change image as per make selected
        private void ChangeImage(ComboBox makeList,Image img) {
            //Get the Make object corresponding to the name selected in the combo box
            //Check the make item list of the make combo box is not null to avoid problems when clearing the claim form
            if (makeList.SelectedItem != null)
            {
                //Find the make by passing in the make name
                MakeExtended make = FindMakeByName(makeList.SelectedItem.ToString());
                //assign the corresponding image
                if (make != null)
                {
                    img.Source = make.Image;
                }
                else
                {
                    img.Source = new BitmapImage(new Uri("Volkswagen.png", UriKind.Relative));
                }
            }
            else {
                img.Source = new BitmapImage(new Uri("Volkswagen.png", UriKind.Relative));
            }//End of if else statement to check the make list is not null
            
            
        }//End of ChangeImage method

        //Method to go through the list of makes and return only those which are active
        internal static List<MakeExtended> GetActiveMakes() {
           List<MakeExtended> makes = new List<MakeExtended>();
            foreach (MakeExtended make in makeList) {
                if (make.Status.Equals(MakeExtended.MakeStatus.Active)) {
                    makes.Add(make);
                }// End of if statement to check make status
            }//End of foreach loop
            return makes;
        }//End of GetActiveMakes method
        
        //Method to go through the list of makes and return only those which are active
        internal static List<ClaimTypeExtended> GetTypes() {
            return claimTypeList;
        }//End of GetTypes method

        //Method to return the selected date in dpStartDate date picker element
        public DateTime? GetDateDPStartDateT1() {
            return this.dpStartDateT1.SelectedDate;
        }//End of GetDateDPStartDate

        //Method to return the selected date in dpEndDAte date picker element
        public DateTime? GetDateDPEndDateT1() {
            return this.dpEndDateT1.SelectedDate;
        }//End of GetDateDPEndDateT1

        //Method to validate search criteria for searchin a claim
        private bool CheckClaimSearchCriteria(TextBox claimNo1, TextBox claimNo2, TextBox VIN,out string claimNumber,out string VINText ,out string errorMessage) {
            //Define error message and error boolean flag
            bool error = false;
            errorMessage = "The following error(s) has(have) been detected: \n";
            claimNumber = "";
            VINText = "";
            
            //Check the claim number is valid and is not empty
            if (!ClaimExtended.CheckClaimNo(claimNo1, claimNo2))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim number invalid. Only alphanumeric characters are valid for the \n\tfirst section (up to 5 digits) and only numbers for the second one.\n");
            }
            else
            {
                claimNumber = ClaimExtended.CreateClaimNo(claimNo1.Text, claimNo2.Text);
            }
            //Check VIN is valid
            if (!CarExtended.CheckVIN(VIN))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*VIN invalid. Only a 17 digit aplphanumeric number is allowed.\n ");
            }
            else
            {
                VINText = VIN.Text.Trim();
            }
            return error;
        }//End of CheckClaimSearchCriteria method

        //Method to select the correct item in a combo box as per text passed in as argument of fuction
        private void SelectCBoxText(ComboBox ddList,string text) {
            bool found = false;
            int i = 0;
            int index;
            List<string> list = (List<string>)ddList.ItemsSource;
            while (!found && i<= ddList.Items.Count) {
                if (list.ElementAt(i).Equals(text))
                {
                    found = true;
                    index = i;
                }
                else
                {
                    i++;
                }//End of if else statement
            }//End of while loop
            //Check the reason to exit the while loop
            if (found == false)
            {
                MessageBox.Show("No match found for drop down list");
            }
            else {
                ddList.SelectedIndex = i;
            }
        }//End of SelectCBoxText method

        //Method to clear the claim form 
        private void ClearClaimForm() {
            this.tbxClaimNo1.Text = "";
            this.tbxClaimNo2.Text = "";
            this.dpClaimDate.SelectedDate = null;
            this.cbxClaimType.SelectedItem = null;
            this.tbxClaimValue.Text = "";
            this.tbxComments.Text = "";
            this.cbxStatus.SelectedItem = null;
            this.cbxFault.SelectedItem = null;
            this.chbxNewFault.IsChecked = false;
            this.cbxGroup.SelectedItem = null;
            this.tbxInvoice.Text = "";
            this.tbxManuCode.Text = "";
            this.tbxVIN.Text = "";
            this.tbxMileage.Text = "";
            this.cbxMileage.SelectedItem = null;
            this.tbxRegNumber.Text = "";
            this.cbxMake.SelectedItem = null;
            this.dpPaymentDate.SelectedDate = null;
            this.dpReceptionDate.SelectedDate = null;
            this.dpRepairDate.SelectedDate = null;
            this.tbxRepairOrder.Text = "";
            this.cbxSolution.SelectedItem = null;
            this.chbxNewSolution.IsChecked = false;
            this.tbxTimeAllowed.Text = "";
            this.tbxTimeTaken.Text = "";
            this.tbxClaimNo1.Text = "";
            this.tbxClaimNo2.Text = "";
            this.tbxVIN.Text = "";
            //Enable the search and new claim button
            this.btnSearchClaim.IsEnabled = true;
            this.btnNewClaim.IsEnabled = true;
            //Disable the clear, update and delete buttons
            this.btnClearForm.IsEnabled = false;
            this.btnUpdateClaim.IsEnabled = false;
            this.btnDeleteClaim.IsEnabled = false;
            //Enable the claim number fields
            this.tbxClaimNo1.IsEnabled = true;
            this.tbxClaimNo2.IsEnabled = true;
        }//End of ClearClaimForm method

        //Method to validate input before registering a new claim or updating an old one
        private bool ValidateClaimFormInput(ref string errorMessage) {
            bool error = false;
            errorMessage = "The following error(s) has(have) been detected: \n";
            if (!ClaimExtended.CheckClaimNo(this.tbxClaimNo1, this.tbxClaimNo2))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim number invalid. Only alphanumeric characters are valid for the \n\tfirst section (up to 5 digits) and only numbers for the second one.\n");
            }
            //Check Claim type is not null
            if (this.cbxClaimType.SelectedItem == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim type invalid. A claim type must be selected.\n");
            }
            //Check claim date is not null
            if (this.dpClaimDate.SelectedDate == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim date invlaid. A date for the claim submission must be selected.\n");
            }
            //Check the claim status is not null
            if (this.cbxStatus.SelectedItem == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim Status invalid. A claim status must be selected.\n");
            }
            //Check the time allowed is valid
            if (!double.TryParse(this.tbxTimeAllowed.Text.Trim(), out timeAllowed))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Time allowed invalid. Only real numbers are allowed.\n");
            }
            //Check the claim value is valid
            if (!(decimal.TryParse(this.tbxClaimValue.Text, out claimValue) && claimValue > 0))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Claim value invalid. Only real numbers are allowed.\n");
            }
            //Check the Manucode is valid
            if (!ClaimExtended.CheckManuCode(this.tbxManuCode))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Manufacturer code invalid. Only A 3 digit alphanumeric code is \n\tallowed.\n");
            }
            //Check VIN is valid
            if (!CarExtended.CheckVIN(this.tbxVIN))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*VIN invalid. Only a 17 digit aplphanumeric number is allowed.\n ");
            }
            //Check the make is not null 
            if (this.cbxMake.SelectedItem == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Make invalid. A make must be selected.\n");
            }
            //Check mileage is valid
            if (!(int.TryParse(this.tbxMileage.Text, out mileage) && mileage > 0))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Mileage invalid. Only integer numbers are allowed.\n");
            }
            //Check mileage unit is valid
            if (this.cbxMileage.SelectedItem == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Distance unit invalid. A distance unit (Km or mi) must be selected.\n");
            }//In case an option has been selected, check if mileage is selected and then transform to km by multiplying by 1.6
            else {
                if (this.cbxMileage.SelectedIndex == 1) {
                    this.mileage = (int) (this.mileage * 1.6);
                }
            }
            //Check the group is not null
            if (this.cbxGroup.SelectedItem == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Vehicle group invalid. A vehicle subsystem must be selected.\n");
            }
            //Check Reception date is not null
            if (this.dpReceptionDate.SelectedDate == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Reception date invlaid. A date for the complaint reception must be \n\tselected.\n");
            }
            //Check repair order is valid
            if (!RepairExtended.CheckRepairOrder(this.tbxRepairOrder))
            {
                this.SetErrorMessage(ref error, ref errorMessage, string.Format("\t*Repair order invalid. Only integer numbers between {0} and {1}\n\tare allowed.\n", RepairExtended.MinRepairOrder, RepairExtended.MaxRepairOrder));
            }
            //Check Repair date is not null 
            if (this.dpRepairDate == null)
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Repair date invlaid. A date for the repair completion must be \n\tselected.\n");
            }
            //Check all dates
            if (!CheckDates(this.dpClaimDate.SelectedDate, this.dpRepairDate.SelectedDate, this.dpReceptionDate.SelectedDate))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Invalid dates. Claim date cannot be before Repair date,\n\tRepair date cannot be before Reception date and \n\tReception date cannot be later than present day.\n");
            }
            //Check Time taken is valid
            if (!double.TryParse(this.tbxTimeTaken.Text.Trim(), out timeTaken))
            {
                this.SetErrorMessage(ref error, ref errorMessage, "\t*Time taken invalid. Only real numbers are allowed.\n ");
            }
            //Check invoice is valid
            if (!RepairExtended.CheckInvoice(this.tbxInvoice, out invoice))
            {
                this.SetErrorMessage(ref error, ref errorMessage, string.Format("\t*Invoice number invalid. Only integer numbers are allowed and they \n\tmust be between {0} and {1}\n", RepairExtended.MinInvoice, RepairExtended.MaxInvoice));
            }
            //Check Fault or New fault are not null 
            if (!CheckFaultSolution(this.cbxFault, this.tbxNewFault))
            {
                this.SetErrorMessage(ref error, ref errorMessage, string.Format("\t*Fault invalid. A fault option must be selected or a new fault must be \n\tregistered (max {0} characters).\n", FaultExtended.MaxFaultLength));
            }
            //Check Solution or new solution are valid
            if (!CheckFaultSolution(this.cbxSolution, this.tbxNewSolution))
            {
                this.SetErrorMessage(ref error, ref errorMessage, string.Format("\t*Solution invalid. A solution option must be selected or a new solution \n\tmust be registered.\n", FaultExtended.MaxFaultLength));
            }
            //Check the comment is not longer than max length
            if (this.tbxComments.Text.Length > ClaimExtended.MaxCommentsLength) {
                this.SetErrorMessage(ref error, ref errorMessage,string.Format("\t*Comments cannot be longer than {0} characters.",ClaimExtended.MaxCommentsLength));
            }
            return error;
        }// End of ValidateClaimFormInput method

        //Method to check logical selection to register a new claim or update a preious one
        private void CheckFaultAndSolutionSelections(CheckBox newFault, ComboBox faultOptions, TextBox newFaultText,CheckBox newSolution, ComboBox solutionOptions,TextBox newSolutionText,out int? faultID,out int? solutionID) {

            string fault, solution;
            if (newFault.IsChecked == true)
            {
                fault = newFaultText.Text.Trim();
                //Call the helper method that checks if a fault is in the database and returns the faultID if so
                if (IsFaultInDB(fault, out faultID))
                {
                    MessageBox.Show("The fault for this claim already exists in the system. Next time you can selected for the drop down list.");
                }
            }
            else
            {
                fault = faultOptions.SelectedItem.ToString();
                //Call the helper method that checks if a fault is in the database and returns the faultID if so
                IsFaultInDB(fault, out faultID);
            }//End of if else statement to check the fault selecttion 

            //Check the text coming from either the combo box or the new fault text box
            if (newSolution.IsChecked == true)
            {
                solution = newSolutionText.Text.Trim();
                //Call the helper method that checks if a solution is in the database and returns the solutionID if so
                if (IsSolutionInDB(solution, out solutionID))
                {
                    MessageBox.Show("The solution for this claim already exists in the system. Next time you can selected for the drop down list.");
                }
            }
            else
            {
                solution = solutionOptions.SelectedItem.ToString();
                //Call the helper method that checks if a solution is in the database and returns the solutionID if so
                IsSolutionInDB(solution, out solutionID);
            }
        }//End of CheckFaultAndSolutionSelection mehtod

        public string CreateMakeName(string makeCode)
        {
            string make="";
            if (makeCode.Equals("V"))
            {
                make = "Volkswagen";
            }
            else if (makeCode.Equals("W"))
            {
                make = "VW Commercial";
            }
            else if (makeCode.Equals("K"))
            {
                make = "Skoda";
            }
            else if (makeCode.Equals("S"))
            {
                make = "Seat";
            }
            else if (makeCode.Equals("A"))
            {
                make = "Audi";
            }//End of if else statement
            return make;
        }// End of CreateMakeName mehtod

        //Method to trnasfer calim data from child window into main window
        public void TransferWIPData(string VIN, string regNumber, string makeCode, string mileage, string timeTaken, string timeAllowed, string receptionDate, string repairDate, string invoice, string WIP)
        {
            this.tbxVIN.Text = VIN;
            this.tbxRegNumber.Text = regNumber;
            MakeExtended selectedMake;
            selectedMake = this.FindMakeByName(this.CreateMakeName(makeCode));
            this.SelectCBoxText(this.cbxMake, selectedMake.Name);
            this.cbxMake.SelectedIndex = selectedMake.Make_ID;
            this.tbxMileage.Text = mileage;
            this.cbxMileage.SelectedIndex = 0;
            this.tbxTimeTaken.Text = timeTaken;
            this.tbxTimeAllowed.Text = timeAllowed;
            this.dpReceptionDate.SelectedDate = Convert.ToDateTime(receptionDate);
            this.dpRepairDate.SelectedDate = Convert.ToDateTime(repairDate);
            this.tbxClaimNo1.Text = WIP;
            this.tbxClaimNo2.Text = "01";
            if (invoice!="0") {
                this.tbxInvoice.Text = invoice;
            }
            this.tbxRepairOrder.Text = WIP;
            //Make the clear button enabled
            this.btnClearForm.IsEnabled = true;
        }//End of TranserWIPData method

        //Method to validate filtering dates are valid
        private bool ValidateFilterDates(DatePicker dpStartDate, DatePicker dpEndDate,ref string errorMessage) {
            bool valid = false;

            //Check the dates are valid (if they are not null)
            if (dpStartDate.SelectedDate == null && dpEndDate.SelectedDate == null) {
                valid = true;
            }
            else if (dpStartDate.SelectedDate == null && dpEndDate.SelectedDate != null || dpStartDate.SelectedDate != null && dpEndDate.SelectedDate == null) {
                errorMessage = errorMessage + "\t*Filtering period invalid. Select both dates or leave them unselected.\n";
            }
            else if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null)
            {
                if (!this.CheckFilterPeriod(dpStartDate.SelectedDate, dpEndDate.SelectedDate))
                {
                    errorMessage = errorMessage + "\t*Filtering period invalid. Start date cannot be after end date,\n\tand end date cannot be later then present date.\n";
                }//End of if else that checks if start and end dates are valids
                else {
                    valid = true;
                }
            }// End of if statement that checks if start date and end dates are not null
            return valid;
        }//End of ValidateFilerDates method

        //Database querying methods
        #region DATABASE QUERYING METHODS
        //Method to get the list of Makes in the database and save them in a list object
        private List<MakeExtended> GetMakes()
        {
            //Declare and instantiate a list to store the MakeExtended objects
            List<MakeExtended> makeList = new List<MakeExtended>();
            //Define query that gets all the makes off the database
            var makes = from Makes in DB.Makes
                        select new { Makes.Make_ID, Makes.Name, Makes.WAR_LRate_ID, Makes.SER_LRate };
            //foreach loop to go through the result set, create MakeExtended objects and store them in the MakeExtended list
            foreach (var make in makes)
            {
                makeList.Add(new MakeExtended(make.Make_ID, make.Name, this.DefineWAR_LRate(make.WAR_LRate_ID), make.SER_LRate));
            }// End of foreach loop
            return makeList;
        }// End of GetMakes method

        // Method to set the correct warranty labour rate for a MakeExtended Object
        private double DefineWAR_LRate(int WAR_LRate_ID)
        {
            double rate = 0;
            //Define query that gets the labour rate corresponding to the labour rate ID passed in as argument
            var warRates = (from Rates in DB.LabourRates
                            where (Rates.LabourRate_ID == WAR_LRate_ID)
                            select Rates.Rate).Single();
            //Invoke and convert the value retrieved form database
            rate = Convert.ToDouble(warRates);
            return rate;
        }// End of DefineWAR_LRate method

        // Method to get the claim types off the database
        private List<ClaimTypeExtended> GetClaimTypes()
        {
            //Declare and instantiate a list to store the ClaimTypeExtended objects
            List<ClaimTypeExtended> claimTypeList = new List<ClaimTypeExtended>();
            //Define query that gets all the claim types off the database
            var claimTypes = from Types in DB.ClaimTypes
                             select new { Types.ClaimType_ID, Types.TypeDescription };
            //foreach loop to go through result set, create ClaimTypeExtended objects and store them in the ClaimTypeExtended list
            foreach (var type in claimTypes)
            {
                claimTypeList.Add(new ClaimTypeExtended(type.ClaimType_ID, type.TypeDescription));
            }// End of foreach loop
            return claimTypeList;
        }// End of GetClaimTypes method

        //Method to get the claim status off the database
        private List<StatusExtended> GetClaimStatus()
        {
            //Declare and instantiate a list to store the StatusExtended objects
            List<StatusExtended> statusList = new List<StatusExtended>();
            //Define query that gets all the status off the database
            var claimStatus = from Status in DB.Status
                              select new { Status.Status_ID, Status.Description };
            //foreach loop to go through result set, create StatusExtended objects and store them in the StatusExtended list
            foreach (var status in claimStatus)
            {
                statusList.Add(new StatusExtended(status.Status_ID, status.Description));
            }// End of foreach loop
            return statusList;
        }// End of GetClaimStatus method

        //Method to get the vehicle subsystems off the database
        private List<GroupExtended> GetGroups()
        {
            //Declare and instantiate a list to store the GroupExtended objects
            List<GroupExtended> groupList = new List<GroupExtended>();
            //Define query that gets all the groups off the database
            var groups = from Groups in DB.Groups
                         select new { Groups.Group_ID, Groups.GroupDescription };
            //foreach loop to go through result set, create GroupExtended objects and store them in the GroupExtended list
            foreach (var group in groups)
            {
                groupList.Add(new GroupExtended(group.Group_ID, group.GroupDescription));
            }// End of foreach loop
            return groupList;
        }// End of GetGroups method

        //Method to check is a car already recorded in the database
        private bool IsCarInDB(string VIN)
        {
            //Declare boolean flag 
            bool exists = false;
            //Query the database to find a car with the VIN passed in
            var query = (from Cars in DB.Cars
                         where (Cars.VIN == VIN)
                         select Cars.VIN);
            //Check result of the query an make a decision
            if (query.Count() != 0)
            {
                exists = true;
            }
            return exists;
        }// End of IsCarInDB method

        //Method to check is a car with the reg number passed in as argument
        private bool FindReg(string regNumber) {
            bool exists = false;
            var query = (from Cars in DB.Cars
                         where (Cars.RegNumber.Equals(regNumber))
                         select Cars.VIN);
            //Check result of the query an make a decision
            if (query.Count() != 0)
            {
                exists = true;
            }
            return exists;
        }// End of FindReg method

        //Method to check is a Fault already recorded in the database
        private bool IsFaultInDB(string faultDescription, out int? faultID)
        {
            //Declare boolean flag and value to store fault ID
            bool exists = false;
            faultID = null;
            //Query the database to find a fault with the text passed in
            var query = from Faults in DB.Faults
                         where (Faults.Fault1.Equals(faultDescription))
                         select Faults.Fault_ID;
            //Check result of the query
            if (query.Count() != 0)
            {
                exists = true;
                faultID = query.First();
            }
            return exists;
        }//End of IsFaultInDB method

        //Method to check is a Solution already recorded in the database
        private bool IsSolutionInDB(string solutionDescription, out int? solutionID)
        {
            //Declare boolean flag and value to store solution ID
            bool exists = false;
            solutionID = null;
            //Query the database to find a solution with the text passed in
            var query = (from Solutions in DB.Solutions
                         where (Solutions.Solution1.Equals(solutionDescription))
                         select Solutions.Solution_ID);
            //Check result of the query
            if (query.Count() != 0)
            {
                exists = true;
                solutionID = query.First();
            }
            return exists;
        }// End of IsSolutionInDB method
        
        //Method to get data to generate and update number of claims bar chart 
        public static void GetNumOfClaimsClaimedValue(DateTime? startDate, DateTime? endDate,int? status,int? make,string type,string group,decimal? minValue,decimal? maxValue,string fault, ref int? numberOfClaims, ref decimal? claimedValue)
        {
            numberOfClaims = null;
            claimedValue = null;
            //Get the total number of claims based on date filtering and status
            DB.sp1CountClaims(startDate, endDate, status, make, type, group, minValue, maxValue, fault, ref numberOfClaims, ref claimedValue);        
        }// End of GetNumOfClaimsBarChart method

        //Method to get the labour claimed according to the arguments passed in
        public static decimal? GetLabourClaimed(DateTime? startDate, DateTime? endDate, int? status, int? make, string type, string group, decimal? minValue, decimal? maxValue, string fault) {
            decimal? labour=null;
            DB.sp21LabourClaimed(startDate,endDate,status,make,type,group,minValue,maxValue,fault,ref labour);
            return labour;
        }//End of GetLabourClaimed

        //Method to get the maximun date in the database
        public DateTime GetMaxDateInDB() {
            var query = (from Claims in DB.Claims
                         select Claims.ClaimDate).Max();
            return (DateTime)query;
        }//End of GetMaxDateInDB method

        //Method to get speed KPI from database
        public static double? CalculateSpeedKPI(DateTime? startDate, DateTime? endDate, int? make) {
            double? KPI=null;
            DB.sp2CalculateClaimingSpeed(startDate,endDate,make,ref KPI);
            return KPI;
        }//End of CalculateSpeedKPI

        #endregion

        #endregion
        //Method to update the plot model for each plot view controller in the xmal file
        private void RefreshCharts() {

            //Update data series for Number of claims Bar Chart by creating a new model and updting the datacontext of PlotView control in xmal
            BarChart_NumberOfClaims totalClaims = new BarChart_NumberOfClaims();
            this.Num_Claims.DataContext = totalClaims;

            //Update data series for Claimed value Pie chart
            PieChart_ClaimedValue totalClaimed = new PieChart_ClaimedValue();
            this.Claimed_Value.DataContext = totalClaimed;

            //Update the data series for Labour + Parts claimed Bar chart by creating a new model and updting the datacontext of PlotView control in xmal
            BarChart_LabourClaimed labourPartsBreakdown = new BarChart_LabourClaimed();
            this.LabourClaimed.DataContext = labourPartsBreakdown;

            //Update the dara series for Weekly claims linear chart by creating a new model and updting the datacontext of PlotView control in xmal
            LineChart_WeeklyClaiming weeklyClaiming = new LineChart_WeeklyClaiming();
            this.ClaimsValuesWeek.DataContext = weeklyClaiming;

            // Update the data series for weekly claiming speed KPI linear chart by creating a new model and updting the datacontext of PlotView control in xmal
            LinearChart_SpeedKpi weeklySpeed = new LinearChart_SpeedKpi();
            this.SpeedKPIChart.DataContext = weeklySpeed;

            //Update the data series for make distribution Pie chart by creating a new model and updting the datacontext of PlotView control in xmal
            PieChart_MakeDistribution makeBreakDown = new PieChart_MakeDistribution();
            this.MakeDistribution.DataContext = makeBreakDown;

            //Update the data series for type of claim distribution Pie chart by creating a new model and updting the datacontext of PlotView control in xmal
            PieChart_TypeDistribution typeBreakDown = new PieChart_TypeDistribution();
            this.TypeDistribution.DataContext = typeBreakDown;
        }//End of RefreshCharts method

        //Method to update the plot model for each plot view controller in the xmal file
        private void RefreshCharts(int? status,int? make,string type)
        {

            //Update data series for Number of claims Bar Chart by creating a new model and updting the datacontext of PlotView control in xmal
            BarChart_NumberOfClaims totalClaims = new BarChart_NumberOfClaims(status,make,type);
            this.Num_Claims.DataContext = totalClaims;

            //Update data series for Claimed value Pie chart
            PieChart_ClaimedValue totalClaimed = new PieChart_ClaimedValue();
            this.Claimed_Value.DataContext = totalClaimed;

            //Update the data series for Labour + Parts claimed Bar chart by creating a new model and updting the datacontext of PlotView control in xmal
            BarChart_LabourClaimed labourPartsBreakdown = new BarChart_LabourClaimed();
            this.LabourClaimed.DataContext = labourPartsBreakdown;

            //Update the dara series for Weekly claims linear chart by creating a new model and updting the datacontext of PlotView control in xmal
            LineChart_WeeklyClaiming weeklyClaiming = new LineChart_WeeklyClaiming();
            this.ClaimsValuesWeek.DataContext = weeklyClaiming;

            // Update the data series for weekly claiming speed KPI linear chart by creating a new model and updting the datacontext of PlotView control in xmal
            LinearChart_SpeedKpi weeklySpeed = new LinearChart_SpeedKpi();
            this.SpeedKPIChart.DataContext = weeklySpeed;

            //Update the data series for make distribution Pie chart by creating a new model and updting the datacontext of PlotView control in xmal
            PieChart_MakeDistribution makeBreakDown = new PieChart_MakeDistribution();
            this.MakeDistribution.DataContext = makeBreakDown;

            //Update the data series for type of claim distribution Pie chart by creating a new model and updting the datacontext of PlotView control in xmal
            PieChart_TypeDistribution typeBreakDown = new PieChart_TypeDistribution();
            this.TypeDistribution.DataContext = typeBreakDown;
        }//End of RefreshCharts method

    }// End of MainWindow Class
}// End of namespace
