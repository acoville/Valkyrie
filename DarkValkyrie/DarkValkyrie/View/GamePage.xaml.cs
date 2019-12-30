using DarkValkyrie.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

/*============================================================================
 * 
 * Valkyrie 
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * 
 * GamePage represents an instance of the game engine.
 * It will load a map into memory and all the actors, blocks 
 * and events represented in it, then continue to redraw until 
 * victory or other conditions force exit / transition.
 * 
 * =========================================================================*/

namespace DarkValkyrie.View
{
    // event handler delegates

    delegate void RedrawHandler();
    
    //========================================================================
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        event RedrawHandler RedrawScreen;

        public GamePageViewModel gpvm { get; set; }

        //=====================================================

        /*-----------------------------------
         * 
         * Position data visible
         * 
         * this is optional debug data to 
         * assist the developer
         * 
         * --------------------------------*/

        public bool Trouble_Visible
        {
            get
            {
                return gpvm.Trouble_Visible;
            }
            set
            {
                if (gpvm.Trouble_Visible == value)
                    return;

                gpvm.Trouble_Visible = value;
                Troubleshooting_Layout.IsVisible = value;
            }
        }

        //===========================================================

        /*-----------------------------------------
         * 
         * Constructor
         * 
         * an optional boolean bit will continue 
         * from the last location 
         * 
         * -----------------------------------------*/

        public GamePage(bool ResumeGame = false)
        {
            InitializeComponent();
            gpvm = new GamePageViewModel(ResumeGame);
            BindingContext = gpvm;
            RedrawScreen = new RedrawHandler(OnRedraw);
        }

        //===========================================================

        /*-----------------------------------------
         * 
         * If this is a mobile device and the 
         * screen rotates, this will call a redraw
         * to flip between portrait / landscape
         * 
         * ---------------------------------------*/

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            gpvm.deviceScreen_.GetScreenDetails();
        }

        //===========================================================

        /*-------------------------------------------
         * 
         * Pause the game if player backs out to 
         * main menu
         * 
         * -----------------------------------------*/

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            gpvm.Paused = true;
            PauseMenu.IsVisible = true;
        }

        //===========================================================

        /*-----------------------------------------------
         * 
         * On Appearing
         * 
         * Main loop responsible for evaluating
         * timed events (these need to be performed 
         * in the UI thread)
         * 
         * --------------------------------------------*/

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.StartTimer(TimeSpan.FromMilliseconds(gpvm.GameSpeed), () =>
            {
                if (gpvm.Paused == false)
                {
                    gpvm.EvaluateMovement();
                    RedrawScreen();
                }

                return true;
            });
        }

        //===========================================================

        /*-------------------------------------
         * 
         * Function to call SkiaSharp View's
         * Invalidate Surface ~ 20 fps 
         * 
         * -----------------------------------*/

        public void OnRedraw()
        {
            SKView.InvalidateSurface();
        }

        //=============================================================

        /*------------------------------------------------------
         * 
         * Pause Button Clicked
         * 
         * Apparently 2-way communication between the 
         * View and ViewModel is frowned upon in MVVM,
         * so to manipulate the pauseMenu XAML tag in the View
         * I have to handle some events here.
         * 
         * ----------------------------------------------------*/

        private void Button_Clicked_2(object sender, System.EventArgs e)
        {
            //-- if paused, then unpause

            if (gpvm.Paused)
            {
                PauseMenu.IsVisible = false;
                gpvm.Paused = false;
            }

            //-- if unpaused, then pause

            else
            {
                gpvm.Paused = true;
                PauseMenu.IsVisible = true;
            }
        }

        //==================================================================

        /*--------------------------------------
         * 
         * Main Menu Button in the Pause Menu
         * 
         * -------------------------------------*/

        private void Button_Pressed(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new OptionsPage(this.gpvm, this));
        }
    }
}