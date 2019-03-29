using DarkValkyrie.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

/*===========================================================
 * 
 *  Adam Coville    
 *  adam.coville@gmail.com  
 *  
 *  MenuPage View Model
 *  
 * =========================================================*/

namespace DarkValkyrie.ViewModel
{
    public class MenuPageViewModel
    {
        //------------------------------------------------

        internal bool savedStateExists = false;
        public bool SavedStateExists
        {
            get
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "save.xml");

                return File.Exists(fileName);
            }
            set
            {
                savedStateExists = value;
            }
        }

        //------------------------------------------------

        internal Screen deviceScreen;

        //------------------------------------------------

        public Screen DeviceScreen
        {
            get
            {
                return deviceScreen;
            }
        }

        //------------------------------------------------

        public Screen.Orientation Orientation
        {
            get
            {
                return DeviceScreen.ScreenOrientation;
            }
        }

        //------------------------------------------------

        internal int buttonHeight;
        public int ButtonHeight
        {
            get
            {
                return buttonHeight;
            }
            set
            {
                buttonHeight = value;
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
            ButtonHeight = (int)deviceScreen.Height / 4;
        }

        //=========================================================

        /*---------------------------------
         * 
         * Helper function to determine which 
         * image is appropriate based on the 
         * device's native display and orientation
         * 
         * -------------------------------*/

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
    }
}
