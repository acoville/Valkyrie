using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Xml;

using Xamarin.Forms;

using Valkyrie.GL;
using Valkyrie.CommandInterpreter;
using DarkValkyrie.Graphics;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using Xamarin.Essentials;

/*================================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * View Model for the GamePage
 *  
 *      - reads a Control Profile .xml to map buttons to commands and 
 *        enable special moves such as double jump in response to user input
 *        
 *       - reads a Map.xml file into memory for rendering and interaction 
 * 
 *      - calls the SKSurface methods to refresh the screen and render all 
 *        sprites in the frame
 * 
 *      - handles map/game related events such as victory conditions, player 
 *      damage, etc..
 * 
 * ============================================================================*/

namespace DarkValkyrie.ViewModel
{
    public delegate void InputChangedHandler(Character c, string e);

    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //----------------------------------------------------

        public event InputChangedHandler PlayerInputChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        //-- Gameplay Control variables

        public bool Paused { get; set; }
        public int Lives { get; set; }
        public bool VictoryCondition { get; set; }

        //=================================================================

        public double Opacity { get; set; }

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

        //==================================================================

        //-- interface control variables

        /*----------------------------------------------

         * The Skia image is redrawn at a rate of
         *
         * 1,000 ms / 30 frames per second = 30.30
         * 1,000 ms / 20 frames per second = 50 
       
        --------------------------------------------*/

        internal double gameSpeed;
        public double GameSpeed
        {
            get
            {
                return gameSpeed;
            }
            set
            {
                //-- if within range

                if (!(value > 1000) && !(value < 15))
                {
                    gameSpeed = value;
                }

                //-- slowest speed: 1 frame / second

                else if (value > 1000)
                {
                    value = 1000;
                    gameSpeed = value;
                }

                //-- fastest speed: 66 frames / second

                else if (value < 15)
                {
                    value = 15;
                    gameSpeed = value;
                }

                RaisePropertyChanged();
            }
        }

        //=================================================================

        internal List<Actor> Actors;
        internal Character player1;

        /*-------------------------------------
         * 
         * Constructor
         * 
         * -----------------------------------*/

        public GamePageViewModel(bool ResumeGame = false)
        {
            GameSpeed = 50;
            deviceScreen = new Screen();
            Actors = new List<Actor>();

            if (ResumeGame)
            {
                MapLoaded = LoadMap("save.xml", ResumeGame);
            }
            else
            {
                MapLoaded = LoadMap("TestMap.xml", ResumeGame);
            }
            
            Opacity = .85;

            SetupPlayer1();

            if (MapLoaded && ControlProfileLoaded)
                StartGame();
        }

        //===================================================================

        /*-----------------------------------------
         * 
         * Start Game
         * 
         * ----------------------------------------*/

        internal void StartGame()
        {
            Lives = 3;
            VictoryCondition = false;

            Paused = false;
        }

        #region LEVEL

        //===================================================================

        internal string backgroundImage;
        public string BackgroundImage
        {
            get
            {
                return backgroundImage;
            }
            set
            {
                backgroundImage = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        //=============================================================

        //----- screen variables

        internal Screen deviceScreen;
        public Screen DeviceScreen
        {
            get { return deviceScreen; }
        }

        //=================================================================

        /*--------------------------------
         * 
         * Helper function to reset player1
         * 
         * -----------------------------*/

        internal void ResetPlayer1()
        {
            Sprite player1_sprite = deviceScreen.Sprites[player1.SpriteIndex];
            Block start = level.StartingLocation;

            deviceScreen.MoveSprite(player1_sprite, start);

            player1.BlockPosition = level.StartingLocation;

            player1.yAccelerationRate = 0;
            player1.ySpeed = 0;

            player1.Standing = true;
        }

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
