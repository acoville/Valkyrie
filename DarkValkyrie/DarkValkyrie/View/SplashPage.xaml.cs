using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DarkValkyrie.ViewModel;
using DarkValkyrie.Graphics;

namespace DarkValkyrie.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        internal SplashPageViewModel spvm;

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
    }
}