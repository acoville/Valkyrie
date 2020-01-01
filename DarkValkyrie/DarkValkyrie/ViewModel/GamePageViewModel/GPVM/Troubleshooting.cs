using System.ComponentModel;
using Xamarin.Essentials;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //=================================================================

        //-- these properties are for troubleshooting only

        public bool Trouble_Visible
        {
            get
            {
                return Preferences.Get("trouble_visible", false);
            }

            set
            {
                if (Preferences.Get("trouble_visible", false) == value)
                    return;

                Preferences.Set("trouble_visible", value);
                RaisePropertyChanged();
            }
        }

        internal string trouble;
        public string Trouble
        {
            get
            {
                return trouble;
            }
            set
            {
                trouble = value;
                RaisePropertyChanged();
            }
        }

        internal string trouble2;
        public string Trouble2
        {
            get
            {
                return trouble2;
            }
            set
            {
                trouble2 = value;
                RaisePropertyChanged();
            }
        }

        internal string trouble3;
        public string Trouble3
        {
            get
            {
                return trouble3;
            }
            set
            {
                trouble3 = value;
                RaisePropertyChanged();
            }
        }
    }
}
