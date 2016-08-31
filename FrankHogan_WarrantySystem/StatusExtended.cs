using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class StatusExtended : Status
    {
        #region CONSTRUCTOR
        // Full constructor
        public StatusExtended(int statusID, string description) {
            this.Status_ID = statusID;
            this.Description = description;
        }// End of full constructor
        #endregion

        #region HELPER METHODS
        public string Print() {
            return string.Format("{0}",this.Description);
        }// End of Print method
        #endregion
    }// End of StatusExtended class
}
