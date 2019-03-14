using DarkValkyrie.View;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * App.cs
 * 
 * ===============================================================*/

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DarkValkyrie
{
    public partial class App : Application
    {
        //===========================================================

        /*----------------------------------------
         * 
         * App Constructor
         * 
         * shows a splash page crediting the 
         * development team for 3 seconds,
         * then transitions to main menu
         * 
         * -------------------------------------*/

            //---------------------------------------------

            public App()
            {
                InitializeComponent();

                MainPage = new NavigationPage(new SplashPage());
            }

            //======================================================

            protected async override void OnStart()
            {
                await Task.Delay(TimeSpan.FromSeconds(4));

                MainPage = new NavigationPage(new MenuPage());
            }

            //======================================================

            protected override void OnSleep()
            {
                // Handle when your app sleeps
            }

            //======================================================

            protected override void OnResume()
            {
                // Handle when your app resumes
            }

        }
    }
