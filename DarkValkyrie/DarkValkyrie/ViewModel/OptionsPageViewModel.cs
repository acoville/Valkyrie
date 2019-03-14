using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

/*=======================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * ====================================================*/

namespace DarkValkyrie.ViewModel
{
    public class OptionsPageViewModel : INotifyPropertyChanged
    {
        internal string linesEnabled;
        public string LinesEnabled
        {
            get
            {
                return linesEnabled;
            }
            set
            {
                linesEnabled = value;
                RaisePropertyChanged();
            }
        }

        public OptionsPageViewModel()
        {
            LinesEnabled = "OFF";


        }

        //=================================================================

        public event PropertyChangedEventHandler PropertyChanged;

        //=================================================================

        /*------------------------------------------
         * 
         * Event Handler to raise propertyChanged
         * 
         * ---------------------------------------*/

        protected void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
