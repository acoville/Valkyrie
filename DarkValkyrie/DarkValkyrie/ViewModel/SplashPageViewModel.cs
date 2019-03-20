using DarkValkyrie.Graphics;

/*===============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * UpSkilled ICT_CIV_PROG_201810
 * Mobile Project
 * 
 * SplashPage View Model class
 * 
 * =============================================================*/

namespace DarkValkyrie.ViewModel
{
    public class SplashPageViewModel
    {
        internal Screen deviceScreen;
        internal Screen.Orientation orientation;

        //=================================================

        /*--------------------------------
         * 
         * Constructor
         * 
         * ------------------------------*/

        public SplashPageViewModel()
        {
            deviceScreen = new Screen();
            orientation = deviceScreen.orientation;
        }
    }
}
