using DarkValkyrie.Graphics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/*=======================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * OptionsPageViewModel
 * 
 * ====================================================*/

namespace DarkValkyrie.ViewModel
{
    public class OptionsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal Screen deviceScreen;

        public string ImageSource { get; set; }

        //---------------------------------------------------

        internal string positionsEnabled;
        public string PositionsEnabled
        {
            get
            {
                return positionsEnabled;
            }
            set
            {
                positionsEnabled = value;
                RaisePropertyChanged();
            }
        }
        //--------------------------------------------------

        internal string linesEnabled;
        public string LinesEnabled
        {
            get
            {
                return linesEnabled;
            }
            set
            {
                linesEnabled = value;
                RaisePropertyChanged();
            }
        }

        //====================================================================

        /*-----------------------------------
         * 
         * Constructor 
         * 
         * ---------------------------------*/

        public OptionsPageViewModel()
        {
            deviceScreen = new Screen();

            GetImageSource();

            LinesEnabled = "OFF";
            PositionsEnabled = "OFF";
        }

        //====================================================================

        /*-----------------------------------
         * 
         * Get Image Source
         * 
         * Helper function to determine which 
         * image is appropriate based on the 
         * device's native display and 
         * orientation
         * 
         * ---------------------------------*/

        public string GetImageSource()
        {
            //-- landscape orientation

            if (deviceScreen.ScreenOrientation == Screen.Orientation.landscape)
            {
                return "menu_landscape.png";

            }

            //------------------------------------------------
            //--- portrait orientation

            else if (deviceScreen.ScreenOrientation == Screen.Orientation.portrait)
            {
                return "menu_portrait.png";

            }

            //------------------------------------------------
            //-- square orientation

            else
            {
                return "menu_square.png";
            }
        }
        
        //=================================================================

        /*------------------------------------------
         * 
         * Event Handler to raise propertyChanged
         * 
         * ---------------------------------------*/

        protected void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
