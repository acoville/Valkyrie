using DarkValkyrie.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DarkValkyrie.ViewModel
{
    public class MenuPageViewModel
    {
        internal Screen deviceScreen;

        public Screen DeviceScreen
        {
            get
            {
                return deviceScreen;
            }
        }

        public Screen.Orientation Orientation
        {
            get
            {
                return DeviceScreen.ScreenOrientation;
            }
        }

        //=======================================================

        /*-------------------------------
         * 
         * Constructor
         * 
         * -----------------------------*/

        public MenuPageViewModel()
        {
            deviceScreen = new Screen();
        }

        //=========================================================

        /*---------------------------------
         * 
         * Helper function to determine which 
         * image is appropriate based on the 
         * device's native display and orientation
         * 
         * 
         * -------------------------------*/

        public string GetImageSource()
        {
            //-- landscape orientation

            if (deviceScreen.ScreenOrientation == Screen.Orientation.landscape)
                return "menu_landscape.png";

            //--- portrait orientation

            else if (deviceScreen.ScreenOrientation == Screen.Orientation.portrait)
                return "menu_portrait.png";

            //-- square orientation

            else
            {
                return "menu_square.png";
            }
        }
    }
}
