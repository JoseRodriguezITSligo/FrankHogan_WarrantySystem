using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FrankHogan_WarrantySystem
{
    class MakeExtended : Make 
    {

        // A new enum has to be included in the MakeExtended object 
        public enum MakeStatus { Active, Inactive }
        // Static Constant for default service labour rate
        private static double DefaultRate = 70;
        #region PROPERTIES
        public double WAR_LRate {get; set;}
        public BitmapImage Image { get; set; }
        public MakeStatus Status { get; set; }
        #endregion

        #region CONSTRUCTORS
        //Full constructor
        public MakeExtended(int makeID, string name,double WAR_LRate, double SER_LRate, MakeExtended.MakeStatus status) {
            this.Make_ID = makeID;
            this.Name = name;
            this.WAR_LRate = WAR_LRate;
            this.SER_LRate = SER_LRate;
            this.Image = ImageSelector(name);
            this.Status = status;
        }// End of Full Constructor

        //Constructor with no service labour rate supplied
        public MakeExtended(int makeID, string name, double WAR_LRate, MakeExtended.MakeStatus status) : this(makeID, name, WAR_LRate, DefaultRate, status) { }

        //Constructor with no warranty nor service labour rate supplied
        public MakeExtended(int makeID, string name, MakeExtended.MakeStatus status) : this(makeID, name, DefaultRate, status) { }

        //Constructor with no Status
        public MakeExtended(int makeID, string name, double WAR_LRate, double SER_LRate) : this(makeID, name, WAR_LRate, SER_LRate, DefineMakeStatus(name)) { }

        //Constructor with no service labour rate and no Status supplied
        public MakeExtended(int makeID, string name, double WAR_LRate) : this(makeID, name, WAR_LRate, DefineMakeStatus(name)){ }

        //Constructor with no service, warranrty labour rates and no Status supplied
        public MakeExtended(int makeID, string name) : this(makeID, name, DefineMakeStatus(name)) { }
        #endregion

        #region HELPER METHODS
        public string Print()
        {
            return string.Format("Make: {0}\n",this.Name);
        }// End of Print method

        //Method to select the correct image for a make
        private static  BitmapImage ImageSelector(string name) {
            string path;
            switch (name.ToLower()) { 
                case "audi": path = "Audo.jpg";   
                    break;
                case "porsche": path = "Porsche.png";
                    break;
                case "seat":  path = "Seat.png";
                    break;
                case "skoda": path = "Skoda.png";
                    break;
                case "volkswagen": path = "Volkswagen.png";
                    break;
                case "vw commercial": path = "Volkswagen-commercial.png";
                    break;
                default: path = "Volkswagen.png";
                    break;
            }// End of switch statement to select correct image path
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }// End of ImageSelector method

        //Method to determine the make status (Active or Inactive) as the dealership may change the makes it deals with
        private static MakeExtended.MakeStatus DefineMakeStatus(string name)
        {
            MakeExtended.MakeStatus status;
            switch (name.ToLower())
            {
                case "volkswagen":
                case "vw commercial":
                case "skoda":
                    status = MakeExtended.MakeStatus.Active;
                    break;
                default:
                    status = MakeExtended.MakeStatus.Inactive;
                    break;
            }//End of switch estatement
            return status;
        }// End of DefineMakeStatus method
        #endregion
    }// End of MakeExtended class
}
