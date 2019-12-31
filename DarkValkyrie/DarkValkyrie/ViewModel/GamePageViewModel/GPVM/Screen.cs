using DarkValkyrie.Graphics;
using System.ComponentModel;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //=============================================================

        //------------ screen variables

        public Screen DeviceScreen
        {
            get { return deviceScreen_; }
        }

        //----------- control buttons opacity

        public double controlOpacity
        {
            get
            {
                return controlOpacity_;
            }
            set
            {
                controlOpacity_ = value;
                RaisePropertyChanged();
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
