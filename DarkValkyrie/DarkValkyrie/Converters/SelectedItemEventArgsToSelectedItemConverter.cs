using System;
using System.Globalization;
using Xamarin.Forms;

/*=======================================================================
 * 
 * Event to Command Behavior
=========================

In the context of commanding, behaviors are a useful approach for 
connecting a control to a command. In addition, they can also be 
used to associate commands with controls that were not designed to 
interact with commands. This sample demonstrates using a behavior 
to invoke a command when an event fires.

For more information about this sample, see [Reusable EventToCommandBehavior]
(https://docs.microsoft.com/xamarin/xamarin-forms/app-fundamentals/behaviors/reusable/event-to-command-behavior).

Author
------

David Britch

 * =====================================================================*/

/*==================================================
* 
* Adam Coville
* adam.coville@gmail.com
* Upsilled ICT_CIV_PROG_201810 
* Mobile Project
* 
* =================================================*/

namespace DarkValkyrie.Converters
{
    class SelectedItemEventArgsToSelectedItemConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var eventArgs = value as SelectedPositionChangedEventArgs;
            return eventArgs.SelectedPosition;
        }

        //=========================================================

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
