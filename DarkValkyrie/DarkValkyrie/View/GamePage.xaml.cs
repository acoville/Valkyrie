using DarkValkyrie.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

/*============================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * GamePage represents an instance of a game. 
 * 
 * =========================================================================*/

namespace DarkValkyrie.View
{
    delegate void RedrawHandler();
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        event RedrawHandler RedrawScreen;

        public GamePageViewModel gpvm { get; set; }

        public bool Trouble_Visible
        {
            get
            {
                return gpvm.trouble_visible;
            }
            set
            {
                gpvm.trouble_visible = value;

                if (Trouble_Visible)
                {
                    Troubleshooting_Layout.IsVisible = true;
                }

                if (!Trouble_Visible)
                {
                    Troubleshooting_Layout.IsVisible = false;
                }
            }
        }

        //===========================================================

        /*----------------------------------------
         * 
         * Constructor
         * 
         * an optional boolean bit will continue 
         * from the last location 
         * 
         * -------------------------------------*/

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
         * Ok, this is working, but in landscape
         * the results are horrible.
         * 
         * ---------------------------------------*/

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            gpvm.deviceScreen.GetScreenDetails();
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

        /*-------------------------------
         * 
         * Function to call SkiaSharp View's
         * Invalidate Surface ~ 20 fps 
         * 
         * -----------------------------*/

        public void OnRedraw()
        {
            SKView.InvalidateSurface();
        }

        //=============================================================

        /*----------------------------------------------
         * 
         * Pause Button Clicked
         * 
         * Apparently 2-way communication between the 
         * View and ViewModel is frowned upon in MVVM,
         * so to manipulate the pauseMenu XAML tag in the View
         * I have to handle some events here.
         * 
         * -------------------------------------------*/

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

        /*-----------------------------------
         * 
         * Main Menu Button in the Pause Menu
         * 
         * --------------------------------*/

        private void Button_Pressed(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new OptionsPage(this.gpvm, this));
        }
    }
}