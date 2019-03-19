using System;
using System.Collections.Generic;
using System.Text;

using DarkValkyrie.Graphics;

namespace DarkValkyrie.ViewModel
{
    public class SplashPageViewModel
    {
        internal Screen deviceScreen;
        internal Screen.Orientation orientation;

        public SplashPageViewModel()
        {
            deviceScreen = new Screen();
            orientation = deviceScreen.orientation;
        }
    }
}
