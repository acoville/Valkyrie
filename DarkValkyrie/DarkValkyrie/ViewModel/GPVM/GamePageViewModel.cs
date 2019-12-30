using System.ComponentModel;
/*================================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * View Model for the GamePage
 * 
 * ============================================================================*/

using System.Runtime.CompilerServices;
using Valkyrie.GL;
using DarkValkyrie.Graphics;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace DarkValkyrie.ViewModel
{
    public delegate void InputChangedHandler(Character c, string e);

    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        public Level level;
        internal List<Actor> Actors;
        internal Character player1;
        
        //----------------------------------------------------

        public event InputChangedHandler PlayerInputChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        //-- Gameplay Control variables

        public bool Paused { get; set; }
        public int Lives { get; set; }
        public bool VictoryCondition { get; set; }

        //==================================================================

        //-- interface control variables

        /*----------------------------------------------

         * The Skia image is redrawn at a rate of
         *
         * 1,000 ms / 30 frames per second = 30.30
         * 1,000 ms / 20 frames per second = 50 
       
        --------------------------------------------*/

        internal double gameSpeed_;
        public double GameSpeed
        {
            get
            {
                return gameSpeed_;
            }
            set
            {
                //-- if within range

                if (!(value > 1000) && !(value < 15))
                {
                    gameSpeed_ = value;
                }

                //-- slowest speed: 1 frame / second

                else if (value > 1000)
                {
                    value = 1000;
                    gameSpeed_ = value;
                }

                //-- fastest speed: 66 frames / second

                else if (value < 15)
                {
                    value = 15;
                    gameSpeed_ = value;
                }

                RaisePropertyChanged();
            }
        }

        //=================================================================

        /*-------------------------------------
         * 
         * Constructor
         * 
         * -----------------------------------*/

        public GamePageViewModel(bool ResumeGame = false)
        {
            deviceScreen_ = new Screen();
            Actors = new List<Actor>();
            GameSpeed = 50;

            if (ResumeGame)
            {
                MapLoaded = LoadMap("save.xml", ResumeGame);
            }
            else
            {
                MapLoaded = LoadMap("TestMap.xml", ResumeGame);
            }
 
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

        //=================================================================

        /*--------------------------------
         * 
         * Helper function to reset player1
         * 
         * -----------------------------*/

        internal void ResetPlayer1()
        {
            Sprite player1_sprite = deviceScreen_.Sprites[player1.SpriteIndex];
            Block start = level.StartingLocation;

            deviceScreen_.MoveSprite(player1_sprite, start);

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
