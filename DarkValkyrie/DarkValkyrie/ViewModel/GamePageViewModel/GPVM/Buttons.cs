using DarkValkyrie.Graphics;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //=============================================================================

        /*---------------------------------------
         * 
         * Null Command will be the default state
         * player input returns to when the player
         * stops pressing buttons
         * 
         * -----------------------------------*/

        private Command nullCommand;
        public ICommand NullCommand
        {
            get
            {
                return (ICommand)nullCommand ?? (nullCommand = new Command(() =>
                {
                    PlayerInput = "";

                    //-- adjust status if necessary
                    //-- if player is falling, no adjustment will be done yet.

                    if (deviceScreen_.Sprites[player1.SpriteIndex]._Status != Status.falling)
                    {
                        deviceScreen_.Sprites[player1.SpriteIndex]._Status = Status.standing;
                    }

                    player1.xAccelerationRate = 0;
                }));
            }
        }

        //================================================================

        //-- A button

        private Command aCommand;
        public ICommand ACommand
        {
            get
            {
                return (ICommand)aCommand ?? (aCommand = new Command(() =>
                {
                    PlayerInput = "A";
                }));
            }
        }

        //================================================================

        //-- B button

        private Command bCommand;
        public ICommand BCommand
        {
            get
            {
                return (ICommand)bCommand ?? (bCommand = new Command(() =>
                {
                    PlayerInput = "B";
                }));
            }
        }

        #region DPAD

        //================================================================

        //-- D-Pad up button

        private Command upCommand;
        public ICommand UpCommand
        {
            get
            {
                return (ICommand)upCommand ?? (upCommand = new Command(() =>
                {
                    PlayerInput = "U";
                }));
            }
        }

        //================================================================

        //-- D-Pad up-right button

        private Command uprightCommand;
        public ICommand UpRightCommand
        {
            get
            {
                return (ICommand)uprightCommand ?? (uprightCommand = new Command(() =>
                {
                    PlayerInput = "UR";
                }));
            }
        }

        //================================================================

        //-- D-Pad right button

        private Command rightCommand;
        public ICommand RightCommand
        {
            get
            {
                return (ICommand)rightCommand ?? (rightCommand = new Command(() =>
                {
                    PlayerInput = "R";
                }));
            }
        }

        //================================================================

        //-- D-Pad down-right button

        private Command downrightCommand;
        public ICommand DownRightCommand
        {
            get
            {
                return (ICommand)downrightCommand ?? (downrightCommand = new Command(() =>
                {
                    PlayerInput = "DR";
                }));
            }
        }

        //================================================================

        //-- D-Pad down button

        private Command downCommand;
        public ICommand DownCommand
        {
            get
            {
                return (ICommand)downCommand ?? (downCommand = new Command(
                    () =>
                    {
                        PlayerInput = "D";
                    }));
            }
        }

        //================================================================

        //-- D-Pad down-left button

        private Command downleftCommand;
        public ICommand DownLeftCommand
        {
            get
            {
                return (ICommand)downleftCommand ?? (downleftCommand = new Command(
                    () =>
                    {
                        PlayerInput = "DL";
                    }));
            }
        }

        //================================================================

        //-- D-Pad left button

        private Command leftCommand;
        public ICommand LeftCommand
        {
            get
            {
                return (ICommand)leftCommand ?? (leftCommand = new Command(() =>
                {
                    PlayerInput = "L";
                }));
            }
        }

        //================================================================

        //-- D-Pad up-left button

        private Command upleftCommand;
        public ICommand UpLeftCommand
        {
            get
            {
                return (ICommand)upleftCommand ?? (upleftCommand = new Command(() =>
                {
                    PlayerInput = "UL";
                }));
            }
        }
    }

    #endregion
}
