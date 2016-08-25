using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class ClaimTypeExtended : ClaimType
    {

        #region CONSTRUCTORS
        //Full Constructor
        public ClaimTypeExtended(string claimTypeID,string typeDescription) {
            this.ClaimType_ID = claimTypeID;
            this.TypeDescription = typeDescription;
        }//End of full constructor
        #endregion

        #region HELPER METHODS
        // Print method
        public string Print() {
            return string.Format("Claim type: {0}/nDescription: {1}",this.ClaimType_ID,this.TypeDescription);
        }// End of print method

        //Method to insert claim types' data in combo boxes
        public string PrintComboBox() {
            return string.Format("{0}--{1}", this.ClaimType_ID, this.TypeDescription);
        }// End of PrintComboBox method
        #endregion
    }//End of ClaimTypeExtended  class
}
