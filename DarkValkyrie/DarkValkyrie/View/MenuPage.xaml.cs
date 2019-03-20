using DarkValkyrie.ViewModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 *  Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * The Menu Page is where the user starts or continues a game
 * and navigates to the options menu
 * 
 * ==========================================================*/

namespace DarkValkyrie.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        MenuPageViewModel vms;

        OptionsPage _options;

        internal GamePage currentGame;

        //==================================================

        /*--------------------------------------
         * 
         *  Constructor
         * 
         * -----------------------------------*/

        public MenuPage()
        {
            InitializeComponent();

            vms = new MenuPageViewModel();
            BindingContext = vms;
            BackgroundImage = vms.GetImageSource();
        }

        //==================================================

        /*---------------------------------
         * 
         * Event to update the background image
         * if the device orientation changes
         * 
         * -------------------------------*/

        protected override void OnSizeAllocated(double width, double height)
        {
            vms.DeviceScreen.GetScreenDetails();

            BackgroundImage = vms.GetImageSource();

            vms.ButtonHeight = (int)vms.DeviceScreen.Height / 4;

            base.OnSizeAllocated(width, height);
        }

        //========================================================

        /*--------------------------
         * 
         * New Game
         * 
         * -----------------------*/

        private void Newgame_Clicked(object sender, EventArgs e)
        {
            currentGame = new GamePage();
            Navigation.PushAsync(currentGame);

            //-- enable the other buttons

            //Save_Btn.IsEnabled = true;
            Continue_Btn.IsEnabled = true;
            Options_Btn.IsEnabled = true;

            _options = new OptionsPage(currentGame.gpvm, currentGame);
        }

        //========================================================

        /*-------------------------------------------
         * 
         * Continue Game
         * 
         * resumes the paused game that's already
         * running.
         * 
         * ---------------------------------------*/

        private void Continue_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(currentGame);
        }

        //=================================================

        /*--------------------------
         * 
         * Options Menu
         * 
         * -----------------------*/

        private void Options_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(_options);
        }

        //=========================================================

        /*--------------------------------
         * 
         * Save Game
         * 
         * currently too glitchy to use
         * 
         * -----------------------------*/

        private void Save_Clicked(object sender, EventArgs e)
        {
            //currentGame.gpvm.level.SaveLevel(currentGame.gpvm.player1);
        }
    }
}