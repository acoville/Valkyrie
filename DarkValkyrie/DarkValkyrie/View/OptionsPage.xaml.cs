/*=================================================================
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
 * ===============================================================*/

using DarkValkyrie.Graphics;
using DarkValkyrie.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DarkValkyrie.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OptionsPage : ContentPage
	{
        internal OptionsPageViewModel ovm;
        GamePageViewModel GPVM;
        GamePage CurrentGame;
        
        internal bool showGrid_;
        internal bool positions_;
        internal double opacity_ = 0.85;

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

            BackgroundImageSource = ovm.GetImageSource();

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

            showGrid_ = GPVM.deviceScreen_.ShowGrid;

            if (showGrid_)
            {
                showGrid_ = false;
                ovm.LinesEnabled = "OFF";
            }

            //-- otherwise turn on 

            else
            {
                showGrid_ = true;
                ovm.LinesEnabled = "ON";
            }

            GPVM.deviceScreen_.ShowGrid = showGrid_;
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
            BackgroundImageSource = ovm.GetImageSource();

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
            positions_ = GPVM.Trouble_Visible;

            //-- if positions are on, turn them off

            if (positions_)
            {
                positions_ = false;
                CurrentGame.Trouble_Visible = false;
                ovm.PositionsEnabled = "OFF";
            }

            //-- if they are off, turn them on 

            else
            {
                positions_ = true;
                CurrentGame.Trouble_Visible = true;
                ovm.PositionsEnabled = "ON";
            }
            
            GPVM.Trouble_Visible = positions_;
        }

        //===========================================================================

        /*-----------------------------------
         * 
         * Event Handler for Opacity Slider
         * 
         * --------------------------------*/

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            opacity_ = opacityController.Value;
            GPVM.controlOpacity = opacity_;
        }
    }
}