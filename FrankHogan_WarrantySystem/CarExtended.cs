using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FrankHogan_WarrantySystem
{
    class CarExtended : Car
    {
        //Max length for VIN code
        private static int VINLength = 17;
        //Max length for reg number strings
        public static int MaxRegLength = 10;
        //Min length for reg number strings
        public static int MinRegLength = 4;

        #region PROPERTIES
        public new MakeExtended Make { get; set; }
        #endregion

        #region CONSTRUCTORS
        //Full constructor
        public CarExtended(string VIN, MakeExtended make, string regNumber, int mileage) {
            this.VIN = VIN;
            this.Make = make;
            this.RegNumber = regNumber;
            this.Mileage = mileage;
        }//End of full constructor

        //Constructor with no Registration number
        public CarExtended(string VIN, MakeExtended make, int mileage) : this(VIN,make,null,mileage) {}

        //Constructor with no Registration number nor milege
        public CarExtended(string VIN, MakeExtended make) : this(VIN,make,null,0) { }
        #endregion

        public string Print() {
            return string.Format("Make: {0}/nVIN: {1}/nReginstration No.: {2}n/Mileage: {3}",this.Make.Print(),this.VIN,this.RegNumber,this.Mileage);
        }// End of Print method

        //Method to check the VIN is 17 character long
        public static bool CheckVIN(TextBox VIN)
        {
            bool valid = false;
            if (VIN.Text.Length == VINLength)
            {
                valid = true;
            }
            return valid;
        }// End of CheckVIN method

        //Method to check the reg number is not longer than 10 character long
        public static bool CheckRegNumber(TextBox regNumber)
        {
            bool valid = false;
            if (regNumber.Text.Length >= MinRegLength && regNumber.Text.Length <= MaxRegLength)
            {
                valid = true;
            }
            return valid;
        }// End of CheckRegNumber method
    }// End of CarExtended class
}// End of namespace
