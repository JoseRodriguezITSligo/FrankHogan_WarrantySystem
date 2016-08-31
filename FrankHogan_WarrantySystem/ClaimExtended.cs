using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FrankHogan_WarrantySystem
{
    class ClaimExtended : Claim
    {
        //Max length for manufacturer code
        private static int ManuCodeLength = 3;
        private static int MaxClaimNoLength = 6;
        private static int MinClaimNoLength = 4;
        public static int MaxCommentsLength = 100;

        #region PROPERTIES
        private static string DefaultVWManuCode = "K21";
        public new ClaimTypeExtended ClaimType { get; set; }
        public new RepairExtended Repair { get; set; }
        public new StatusExtended Status { get; set; }
        public new DateTime? PaymentDate { get; set; }
        #endregion


        #region CONSTRUCTORS
        //Full Constructor
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, DateTime claimDate, decimal claimValue,
                             double timeAllowed, StatusExtended status, DateTime? paymentDate,
                             string manuCode)
        {
            this.Claim_ID = claimID;
            this.ClaimNumber = claimNumber;
            this.ClaimType = claimType;
            this.Repair = repair;
            this.ClaimDate = claimDate;
            this.ClaimValue = claimValue;
            this.TimeAllowed = timeAllowed;
            this.Status = status;
            this.PaymentDate = paymentDate;
            this.ManuCode = manuCode;
        }// End of full constructor

        //Constructor with no ManuCode
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, DateTime claimDate, decimal claimValue,
                             double timeAllowed, StatusExtended status,
                             DateTime? paymentDate) : this(claimID, claimNumber, claimType, repair,
                                                          claimDate, claimValue, timeAllowed, status,
                                                          paymentDate, DefaultVWManuCode){ }

        //Constructor with no ManuCode and payment day
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, DateTime claimDate, decimal claimValue,
                             double timeAllowed, StatusExtended status) : this(claimID, claimNumber, claimType, repair,
                                                                               claimDate, claimValue, timeAllowed, 
                                                                               status,null,DefaultVWManuCode){ }

        //Constructor with no ManuCode, payment day and claim date
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, decimal claimValue,
                             double timeAllowed, StatusExtended status) : this(claimID, claimNumber, claimType, repair,
                                                                               DateTime.Today, claimValue, timeAllowed,
                                                                               status){ }
        //Constructor with no payment day
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, DateTime claimDate, decimal claimValue,
                             double timeAllowed, StatusExtended status,string manuCode) : this(claimID, claimNumber, claimType, repair,
                                                                               claimDate, claimValue, timeAllowed,
                                                                               status,null,manuCode){ }

        //Constructor with no Claim date
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, decimal claimValue,
                             double timeAllowed, StatusExtended status,
                             DateTime? paymentDate,string manuCode) : this(claimID, claimNumber, claimType, repair,
                                                                           DateTime.Today,claimValue, timeAllowed, status,
                                                                           paymentDate,manuCode){ }

        //Constructor with no Claim date and ManuCode
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, decimal claimValue,
                             double timeAllowed, StatusExtended status,
                             DateTime? paymentDate) : this(claimID, claimNumber, claimType, repair,
                                                           claimValue, timeAllowed, status,
                                                           paymentDate,DefaultVWManuCode){ }

        //Constructor with no Claim date and payment day
        public ClaimExtended(int claimID, string claimNumber, ClaimTypeExtended claimType,
                             RepairExtended repair, decimal claimValue,
                             double timeAllowed, StatusExtended status,
                             string manuCode) : this(claimID, claimNumber, claimType, repair,
                                                     DateTime.Today, claimValue, timeAllowed, 
                                                     status,manuCode){ }
        #endregion

        #region HELPER METHODS
        // Print method
        public string Print() {
            return string.Format("");
        }// End of print method

        //Static method to create a claim number based on two strings
        public static string CreateClaimNo(string No1, string No2) {
            return string.Concat(No1, "-",No2);
        }// End of CreateClaimNo method

        //Method to check the claim number is correct
        public static bool CheckClaimNo(TextBox ClaimNo1, TextBox ClaimNo2)
        {
            int claimNo1, claimNo2;
            bool valid = false;
            if (ClaimNo1.Text.Length > MinClaimNoLength && ClaimNo1.Text.Length < MaxClaimNoLength && int.TryParse(ClaimNo2.Text, out claimNo2))
            {
                valid = true;
            }
            return valid;
        }// End of CheckClaimNo method

        // Method to check the manuCode length is equel to 3
        public static bool CheckManuCode(TextBox manuCode)
        {
            bool valid = false;
            if (manuCode.Text.Length == ManuCodeLength)
            {
                valid = true;
            }
            return valid;
        }// End of CheckManuCode methd
        #endregion

    }// End of ClaimExtended class
}//End of namespace
