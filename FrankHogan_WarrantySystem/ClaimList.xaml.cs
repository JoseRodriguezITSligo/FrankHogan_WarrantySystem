using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FrankHogan_WarrantySystem
{
    /// <summary>
    /// Interaction logic for ClaimList.xaml
    /// </summary>
    public partial class ClaimList : Window
    {
        public ClaimList()
        {
            InitializeComponent();
        }

        //New constructor that receives only one parameter: a list of claims
        public ClaimList(List<string> source):this() {
            this.ClearHeader();
            this.listBoxClaims.ItemsSource = source;
        }

        //New constructor that takes in VIN and regnumber to be displayed in window
        public ClaimList(List<string> source, string VIN,string regNumber) : this() {
            this.lblVINValue.Content = VIN;
            this.lblRegNumberValue.Content = regNumber;
            this.listBoxClaims.ItemsSource = source;
        }// End of constructor with 3 parameters

        //Method to clear VIN and reg number headings
        private void ClearHeader() {
            this.lblVIN.Content = "";
            this.lblVINValue.Content ="";
            this.lblRegNumber.Content = "";
            this.lblRegNumberValue.Content = "";
        }// End of ClearHeader method
     }// End of class
}// End of namespace
