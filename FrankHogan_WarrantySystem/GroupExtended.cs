using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrankHogan_WarrantySystem
{
    class GroupExtended : Group
    {
        #region CONSTRUCTURS
        public GroupExtended(string groupID, string description)  {
            this.Group_ID = groupID;
            this.GroupDescription = description;
        }// End of full constructor       
        #endregion

        #region METHODS
        public string Print() {
            return string.Format("{0}",this.GroupDescription);
        }//End of Print method

        //Method to insert groups' data in combo boxes
        public string PrintComboBox()
        {
            return string.Format("{0}--{1}", this.Group_ID, this.GroupDescription);
        }// End of PrintComboBox method
        #endregion
    }// End of GroupExtended class
}
