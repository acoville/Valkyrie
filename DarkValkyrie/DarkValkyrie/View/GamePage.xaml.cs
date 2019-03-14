using DarkValkyrie.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Valkyrie.GL;
using DarkValkyrie.Graphics;
using System.Collections.Generic;
using System.ComponentModel;

/*============================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upskilled ICT401515 / Core Infrastructure Mobile Project
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

        //=================================================================

        //-----------------------------------------------------
        // wish I could find where to get rid of the 
        // auto-generated code linking to these...

        //private void Button_Clicked(object sender, System.EventArgs e)
        //{

        //}

        //private void Button_Clicked_3(object sender, System.EventArgs e)
        //{

        //}

        //private void Button_Clicked_1(object sender, System.EventArgs e)
        //{

        //}
    }
}