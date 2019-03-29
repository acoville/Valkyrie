using DarkValkyrie.Graphics;
using DarkValkyrie.ViewModel;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * View for the Options Page
 * 
 * Player will be able to control colors, appearance, difficulty
 * and other options here
 * 
 * ==========================================================*/

namespace DarkValkyrie.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OptionsPage : ContentPage
	{
        internal OptionsPageViewModel ovm;
        
        internal bool ShowGrid;
        internal bool Positions;
        
        GamePageViewModel GPVM;
        GamePage CurrentGame;

        //=============================================================

        /*----------------------------------
         * 
         * Constructor
         * 
         * -------------------------------*/

        public OptionsPage(GamePageViewModel gpvm, GamePage current_game)
        {
            ovm = new OptionsPageViewModel();
            ovm.deviceScreen = new Screen();

            BackgroundImage = ovm.GetImageSource();

            InitializeComponent();
        
            BindingContext = ovm;

            GPVM = gpvm;
            CurrentGame = current_game;
        }
            
        //=============================================================

        /*------------------------------
         * 
         * Turn grid on or off
         * 
         * ----------------------------*/

        private void Button_Clicked(object sender, EventArgs e)
        {
            //-- if on, turn off

            ShowGrid = GPVM.deviceScreen.ShowGrid;

            if (ShowGrid)
            {
                ShowGrid = false;
                ovm.LinesEnabled = "OFF";
            }

            //-- otherwise turn on 

            else
            {
                ShowGrid = true;
                ovm.LinesEnabled = "ON";
            }

            GPVM.deviceScreen.ShowGrid = ShowGrid;
        }

        //================================================================

        /*----------------------------------
         * 
         * OnSizeAllocated override
         * 
         * -------------------------------*/

        protected override void OnSizeAllocated(double width, double height)
        {
            ovm.deviceScreen.GetScreenDetails();
            BackgroundImage = ovm.GetImageSource();

            base.OnSizeAllocated(width, height);
        }

        //================================================================

        /*---------------------------------
         * 
         * Turn on position data
         * 
         * ------------------------------*/

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Positions = GPVM.Trouble_Visible;

            //-- if positions are on, turn them off

            if (Positions)
            {
                Positions = false;
                CurrentGame.Trouble_Visible = false;
                ovm.PositionsEnabled = "OFF";
            }

            //-- if they are off, turn them on 

            else
            {
                Positions = true;
                CurrentGame.Trouble_Visible = true;
                ovm.PositionsEnabled = "ON";
            }

            GPVM.Trouble_Visible = Positions;
        }
    }
}