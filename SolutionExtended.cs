using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class SolutionExtended : Solution
    {
        #region CONSTRUCTOR
        //Full constructor
        public SolutionExtended(int solutionID, string solution) {
            this.Solution_ID = solutionID;
            this.Solution1 = solution;
        }// End of full cosntrucotr
        #endregion

        #region HELPER METHODS
        public string Print() {
            return string.Format("Solution: {0}",this.Solution1);
        }// End of Print method
        #endregion
    }// End of SolutionExtended class
}
