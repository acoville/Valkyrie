using DarkValkyrie.View;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=================================================================
 *
 *  This started out as my final project for a Certificate IV 
 *  in programming from Upskilled. I've decided I'd like to 
 *  expand it a bit further into at least an interactive demo
 *  to demonstrate all core concepts as a minimum viable product.
 *  
 *  This is an early alpha build, not yet licensed for distribution
 *  
 * Adam Coville
 * adam.coville@gmail.com
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
         * App Entry Point / Constructor
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
