using DarkValkyrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 *  Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * ==========================================================*/

namespace DarkValkyrie.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        MenuPageViewModel vms;

        internal GamePage currentGame;

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

        //=================================================

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

            Save_Btn.IsEnabled = true;
            Continue_Btn.IsEnabled = true;
            Options_Btn.IsEnabled = true;
        }

        //=================================================

        /*--------------------------
         * 
         * Continue Game
         * 
         * -----------------------*/

        private void Continue_Clicked(object sender, EventArgs e)
        {
            //currentGame = new GamePage(true);

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
            Navigation.PushAsync(new OptionsPage(currentGame.gpvm, currentGame));
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