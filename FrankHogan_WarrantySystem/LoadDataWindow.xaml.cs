using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for LoadDataWindow.xaml
    /// </summary>
    public partial class LoadDataWindow : Window
    {
        public LoadDataWindow()
        {
            InitializeComponent();
        }

        //Click event handler for the cancel button
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }//End of Click event handler for the cancel button


        //Click event handler for Load button
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            int MaxWIP = 99999;
            int MinWIP = 10000;
            int wipNumber;
            bool error = false;
            //Try to parse the input and validate it
            if (int.TryParse(this.tbxWIP.Text,out wipNumber)) {
                if (wipNumber >= MinWIP && wipNumber <= MaxWIP)
                {
                    List<WIPData> claimData = new List<WIPData>();
                    string[] data = new string[10];
                    string line;
                    try
                    {
                        string[] lines = File.ReadAllLines(string.Format("C:\\temp\\{0}.csv", this.tbxWIP.Text.Trim()));
                        for (int i = 0; i < lines.Length; i++)
                        {
                            line = lines[i];
                            data = line.Split(',');
                            claimData.Add(new WIPData(data));
                        }// End for loop
                        if (claimData.Count > 1)
                        {
                            double tTaken = 0, tAllowed= 0;
                            foreach (WIPData item in claimData) {
                                tTaken = tTaken + Convert.ToDouble(item.TimeTaken);
                                tAllowed = tAllowed + Convert.ToDouble(item.TimeAllowed);
                            }//End of foreach loop
                            claimData.First().TimeTaken = tTaken.ToString();
                            claimData.First().TimeAllowed = tAllowed.ToString();
                        }
                        this.TransferWIPData(claimData.First());
                        this.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to find file for WIP number provided. Try a different number.");
                    }//End of try catch block               
                    
                }//if inout value is not between min and max wip number 
                else {
                    error = true;
                }
            }//in case the input value is not an integer number
            else {
                error = true;
            }//End of if else statements to validate input data
            
            //Show an error message in case of any error
            if (error) {
                MessageBox.Show(string.Format("WIP number invalid. Only integer numbers between {0} and {1} are valid!",MinWIP.ToString(),MaxWIP.ToString()));
            }
        }//End of Click event handler for OK button

        //Internal class to deal with data coming from files
        private class WIPData
        {
            public string VIN { get; set; }
            public string RegNumber { get; set; }
            public string MakeCode { get; set; }
            public string Mileage { get; set; }
            public string TimeTaken { get; set; }
            public string TimeAllowed { get; set; }
            public string ReceptionDate { get; set; }
            public string RepairDate { get; set; }
            public string Invoice { get; set; }
            public string WIP { get; set; }
            //Constructor
            public WIPData(string[] data)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            this.VIN = data[i];
                            break;
                        case 1:
                            this.RegNumber = data[i];
                            break;
                        case 2:
                            this.MakeCode = data[i];
                            break;
                        case 3:
                            this.Mileage = data[i];
                            break;
                        case 4:
                            this.TimeTaken = data[i];
                            break;
                        case 5:
                            this.TimeAllowed = data[i];
                            break;
                        case 6:
                            this.ReceptionDate = data[i];
                            break;
                        case 7:
                            this.RepairDate = data[i];
                            break;
                        case 8:
                            this.Invoice = data[i];
                            break;
                        case 9:
                            this.WIP = data[i];
                            break;
                    }//end of switch statement
                }//End of for loop
            }//End of constructor
        }//End of internal class

        //Method to call owner's method to populate automatically the retrieved fields
        private  void TransferWIPData(WIPData data)
        {
            MainWindow  main = ((MainWindow)System.Windows.Application.Current.MainWindow);
            main.TransferWIPData(data.VIN,data.RegNumber,data.MakeCode,data.Mileage,
                                 data.TimeTaken,data.TimeAllowed,data.ReceptionDate,
                                 data.RepairDate,data.Invoice,data.WIP);
        }//End of TransferWIPData method

    }//End of class
}//End of namespace
