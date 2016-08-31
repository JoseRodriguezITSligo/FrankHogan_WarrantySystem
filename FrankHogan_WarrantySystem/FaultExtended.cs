using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class FaultExtended : Fault
    {
        public static int MaxFaultLength = 60;
        #region PROPERTIES
        public new GroupExtended Group { get; set; }
        #endregion

        #region CONSTRUCTORS
        //Full constructor
        public FaultExtended(int faultID, string fault, GroupExtended group) {
            this.Fault_ID = faultID;
            this.Fault1 = fault;
            this.Group = group;
        }// End of full constructor
        #endregion

        #region HELPER METHODS
        public string Print() {
            return string.Format("Fault: {0}/nGroup: {1}",this.Fault1, this.Group.Print());
        }//End of Print method
        #endregion
    }// End of FaultExtended class
}
