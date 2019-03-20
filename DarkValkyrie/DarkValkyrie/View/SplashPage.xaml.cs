
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DarkValkyrie.ViewModel;
using DarkValkyrie.Graphics;

/*====================================================================
 * 
 *  Adam Coville
 *  adam.coville@gmail.com
 *  
 *  UpSkilled ICT401515 / Core Infrastructure Mobile Project
 * 
 *  this splash screen briefly displays the developer credit image
 *  then transitions to the main menu
 * 
 * ==================================================================*/

namespace DarkValkyrie.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        internal SplashPageViewModel spvm;

        //======================================================================

        /*-----------------------------------------
         * 
         * Constructor will load the appropriate
         * splash image
         * 
         * ---------------------------------------*/

        public SplashPage()
        {
            InitializeComponent();

            spvm = new SplashPageViewModel();

            if (spvm.orientation == Screen.Orientation.landscape)
            {
                ImageTag.Source = "splash_landscape.png";
            }
            else if(spvm.orientation == Screen.Orientation.portrait)
            {
                ImageTag.Source = "splash_portrait.png";
            }
        }

        //======================================================================

        /*----------------------------------------
         * 
         * OnSizeAllocated override will 
         * switch if orientation changes
         * 
         * this splash screen is only up for a few 
         * seconds so it shouldn't, but just incase
         * 
         * -------------------------------------*/

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    spvm.deviceScreen.GetScreenDetails();

        //    if (spvm.orientation == Screen.Orientation.landscape)
        //    {
        //        ImageTag.Source = "splash_landscape.png";
        //    }
        //    else if (spvm.orientation == Screen.Orientation.portrait)
        //    {
        //        ImageTag.Source = "splash_portrait.png";
        //    }

        //    base.OnSizeAllocated(width, height);
        //}
    }
}