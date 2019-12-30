using DarkValkyrie.Graphics;
using System.ComponentModel;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //=============================================================

        //------------ screen variables

        internal Screen deviceScreen_;
        public Screen DeviceScreen
        {
            get { return deviceScreen_; }
        }

        //----------- control buttons opacity

        internal double controlOpacity_ = 0.85;
        public double controlOpacity
        {
            get
            {
                return controlOpacity_;
            }
            set
            {
                controlOpacity_ = value;
            }
        }

        //----------- background image

        internal string backgroundImage_;
        public string BackgroundImage
        {
            get
            {
                return backgroundImage_;
            }
            set
            {
                backgroundImage_ = value;
                RaisePropertyChanged();
            }
        }
    }
}
