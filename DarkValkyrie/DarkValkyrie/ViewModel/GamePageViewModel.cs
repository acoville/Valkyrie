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

    public class GamePageViewModel : INotifyPropertyChanged
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

        //---------------- these properties are for troubleshooting only

        internal bool trouble_visible;
        public bool Trouble_Visible
        {
            get
            {
                return trouble_visible;
            }

            set
            {
                trouble_visible = value;
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

        internal List<Character> Actors;
        internal Character player1;

        //==========================================================

        /*-------------------------------------
         * 
         * Constructor
         * 
         * -----------------------------------*/

        public GamePageViewModel(bool ResumeGame = false)
        {
            GameSpeed = 50;

            deviceScreen = new Screen();

            Actors = new List<Character>();

            MapLoaded = LoadMap("TestMap.xml", ResumeGame);

            Opacity = .85;

            SetupPlayer1();

            if (MapLoaded && ControlProfileLoaded)
                StartGame();
        }

        //==================================================================

        /*----------------------------------------
         * 
         * Helper function to set up player1
         * 
         * ------------------------------------*/

        internal void SetupPlayer1()
        {
            int X = level.StartingLocation.X;
            int Y = level.StartingLocation.Y;

            player1 = new Character("Erina", X, Y);
            player1.SpriteSource = "Characters.Erina.Leather";

            //-- motion characteristics

            player1.MaxJumps = 1;       // 2 enables doublejump
            player1.Max_X_Speed = 12;

            //-- set up player1 in the display handler

            Sprite player1Sprite = new Sprite(player1);
            deviceScreen.AddCharacter(player1Sprite, ref player1);

            //-- set up player1 in the physics handler

            Actors.Add(player1);

            //-- set up control

            ControlProfileLoaded = LoadInputProfile("ErinaProfile.xml");
            PlayerInputChanged = new InputChangedHandler(OnInputChanged);
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

        //===================================================================

        /*----------------------------------------
         * 
         * Helper function to load the level, 
         * these should be .xml files included 
         * in the /Model/Maps/ directory of the 
         * .NET Standard Library, with the
         * Build Action: Embedded Resource
         * 
         * --------------------------------------*/

        public Level level;
        public string MapName { get; set; }
        public bool MapLoaded { get; set; }

        internal bool LoadMap(string levelName, bool ResumeGame = false)
        {
            var ResourceID = "DarkValkyrie.Model.Maps." + levelName;

            var assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(ResourceID))
            {
                XmlDocument _level = new XmlDocument();
                _level.Load(stream);

                level = new Level(_level);

                //-------------- load saved game if requested

                if (ResumeGame)
                {
                    XmlNode root = _level.DocumentElement;
                    for (int i = 0; i < root.ChildNodes.Count; i++)
                    {
                        XmlNode resume = root.ChildNodes[i];
                        if (resume.Attributes["Label"].Value == "current")
                        {
                            level.InitializeStart(resume);
                        }
                    }
                }

                BackgroundImage = level.ImageSource;

                AddTiles();
                AddMonsters();

            }

            if (level.StartingLocation.Label == "start")
                return true;
            else
                return false;
        }

        //===============================================================

        /*---------------------------------------------
         * 
         * Helper function to load all monsters
         * 
         * ------------------------------------------*/

        internal void AddMonsters()
        {
            for (int i = 0; i < level.Monsters.Count; i++)
            {
                Character mob1 = new Character(level.Monsters[i]);
                Sprite mob1Sprite = new Sprite(level.Monsters[i]);

                //-- add them to the physics handler

                Actors.Add(level.Monsters[i]);

                //-- add their sprites to the display handler

                mob1.SpriteIndex = deviceScreen.Sprites.Count;
                deviceScreen.AddCharacter(mob1Sprite, ref mob1);
            }
        }

        //===============================================================

        /*---------------------------------------------
         * 
         * Helper function to load all tiles on-screen
         * 
         * ------------------------------------------*/

        internal void AddTiles()
        {
            //-- iterate through each static obstacle in memory, checking if it is
            //-- within the visible area region

            foreach (var o in level.StaticObstacles)
            {
                for (int i = 0; i < o.Blocks.Count; i++)            // columns
                {
                    for (int j = 0; j < o.Blocks[i].Count; j++)     // rows
                    {
                        Block arg = o.Blocks[i][j];

                        Tile tile = new Tile(o.ImageSource);

                        deviceScreen.AddTile(tile, arg);
                    }
                }
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

        #region INPUT

        //============================================================

        /*----------------------------------------
         * 
         * FEATURE DELAYED
         * 
         * Helper function to load the command 
         * profile. Profiles should be .xml files
         * in the /Model/ControlProfiles/ directory
         * of the SideScroller .NET standard library.
         * Build action should be: Embedded Resource
         * 
         * --------------------------------------*/

        internal bool LoadInputProfile(string ProfileName)
        {
            //-- setup the input

            var ResourceID = "DarkValkyrie.Model.ControlProfiles." + ProfileName;

            var assembly = this.GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(ResourceID);

            XmlDocument _controlProfile = new XmlDocument();
            _controlProfile.Load(stream);

            interpretor = new Interpreter(ref _controlProfile);

            PlayerInput = string.Empty;

            //-- test the load

            Special testCommand = new Special();
            bool test = interpretor.Interpret("A", testCommand);

            return test;
        }

        //==================================================================

        /*----------------------------------------
         * 
         * FEATURE DELAYED
         * 
         * Special Commands are at this point,
         * something I need to implement later.
         * 
         * --------------------------------------*/

        //-- Input variables

        private Interpreter interpretor;
        public bool ControlProfileLoaded { get; set; }

        //============================================================================

        /*----------------------------------------------
         * 
         * Player Input
         * 
         *  This string is modified by players pressing the 
         *  virtual d-pad and action buttons. An event listener
         *  translates these inputs into commands. 
         * 
         * The event listener is static, which means all I need
         * to do for the AI is output string message traffic
         * just like player input does, except the strings will 
         * be generated by the AI's function instead of the player's 
         * button pushes.
         * 
         * -------------------------------------------*/

        private string playerInput = "";
        public string PlayerInput
        {
            get
            {
                return playerInput;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    playerInput = value;
                    PlayerInputChanged(player1, playerInput);
                    RaisePropertyChanged();
                }
            }
        }

        //=============================================================================

        /*-----------------------------
            * 
            * FEATURE DELAYED
            * 
            * --------------------------*/

        internal Special playerCommand;
        public Special PlayerCommand
        {
            get
            {
                return playerCommand;
            }
            private set
            {
                playerCommand = value;
                RaisePropertyChanged();
            }
        }

        #region BUTTONS


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

                    if (deviceScreen.Sprites[player1.SpriteIndex]._Status != Status.falling)
                    {
                        deviceScreen.Sprites[player1.SpriteIndex]._Status = Status.standing;
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

        #endregion
        #endregion
        #endregion

        #region EVENTS

        //=================================================================

        /*------------------------------------------------
         * 
         * OnInputChanged Event Handler
         * 
         * Now why.... why isn't the call to the sprites
         * working?
         * 
         * ---------------------------------------------*/

        internal int step = 15;

        public void OnInputChanged(Character actor, string e)
        {
            Sprite sprite = deviceScreen.Sprites[actor.SpriteIndex];

            switch (e)
            {
                //===================================================================

                // up key

                case "U":
                    {
                        sprite._Status = Status.standing;
                        break;
                    }

                //===================================================================

                // right key

                case "R":
                    {
                        //-- change facing if needed

                        if (sprite._Facing != Facing.right)
                        {
                            sprite._Facing = Facing.right;
                        }

                        //-- if not crouching, start moving

                        if (actor.Standing)
                        {
                            actor.xAccelerationRate = actor.Max_X_Speed / 4;
                        }

                        break;
                    }

                //===================================================================
                // lower right key

                case "DR":
                    {
                        if (sprite._Status != Status.crouching)
                            sprite._Status = Status.crouching;

                        if (sprite._Facing != Facing.right)
                            sprite._Facing = Facing.right;

                        break;
                    }

                //===================================================================

                // down key

                case "D":
                    {
                        sprite._Status = Status.crouching;
                        break;
                    }


                //===================================================================
                // lower left key

                case "DL":
                    {
                        if (sprite._Status != Status.crouching)
                            sprite._Status = Status.crouching;

                        if (sprite._Facing != Facing.left)
                            sprite._Facing = Facing.left;

                        break;
                    }

                //===================================================================

                // left key

                case "L":
                    {
                        if (sprite._Facing != Facing.left)
                        {
                            sprite._Facing = Facing.left;
                        }

                        //-- if not crouching, start moving

                        if (actor.Standing)
                        {
                            actor.xAccelerationRate = 0 - (actor.Max_X_Speed / 4);
                        }

                        break;
                    }

                //===================================================================
                //-- upper left key

                case "UL":
                    {
                        if (sprite._Status != Status.standing)
                            sprite._Status = Status.standing;

                        if (sprite._Facing != Facing.left)
                            sprite._Facing = Facing.left;

                        if (actor.Standing)
                        {
                            actor.xAccelerationRate = 0 - (actor.Max_X_Speed / 4);
                        }

                        break;
                    }

                //===================================================================
                // upper-right key

                case "UR":
                    {
                        if (sprite._Status != Status.standing)
                            sprite._Status = Status.standing;

                        if (sprite._Facing != Facing.right)
                            sprite._Facing = Facing.right;

                        if (actor.Standing)
                        {
                            actor.xAccelerationRate = actor.Max_X_Speed / 4;
                        }

                        break;
                    }

                //===================================================================

                // A key

                case "A":
                    {
                        /*-------------------------------------------
                         * 
                         * Double Jump
                         * 
                         * To enable a charcter to double-jump,
                         * set their max jumps to the desired number
                         * 
                         * -----------------------------------------*/

                        if (actor.Falling && actor.CurrentJumps < actor.MaxJumps)
                        {
                            actor.CurrentJumps++;

                            actor.yAccelerationRate = 25;
                            actor.ySpeed = actor.yAccelerationRate;
                        }

                        /*----------------------------------------
                         * 
                         * Normal Jump
                         * 
                         * -------------------------------------*/

                        //-- are we standing on something?

                        if (actor.Standing)
                        {
                            actor.yAccelerationRate = 25;
                            actor.ySpeed = actor.yAccelerationRate;

                            actor.Falling = true;

                            sprite._Status = Status.falling;

                            actor.CurrentJumps++;
                        }

                        break;
                    }

                //===================================================================

                // B key

                case "B":
                    {
                        sprite._Status = Status.attack;
                        break;
                    }

                //===================================================================

                // 

                default:
                    {
                        if (sprite._Status != Status.falling)
                            sprite._Status = Status.standing;

                        break;
                    }
            }
        }

        //==========================================================

        /*-----------------------------------------------------
         * 
         * Function to evaluate Movement of a
         * given character, apply all necessary modifications
         * to their speed and state, and translate their
         * sprites on-screen.
         * 
         * --------------------------------------------------*/

        internal void EvaluateMovement()
        {
            foreach (var actor in Actors)
            {
                Sprite sprite = deviceScreen.Sprites[actor.SpriteIndex];

                if (actor.Name == "Erina")
                {
                    Trouble = player1.BlockPosition.ToString();
                    Trouble2 = sprite.SkiaPosition.ToString();
                }

                EvaluateVerticalMovement(actor, sprite);
                EvaluateLateralMovement(actor, sprite);
            }

        }

        //============================================================

        /*--------------------------------------
         * 
         * Evalute vertical movement logic
         * for a given actor
         * 
         * -----------------------------------*/

        internal void EvaluateVerticalMovement(Character actor, Sprite sprite)
        {
            //-- translate the sprite vertically

            if (actor.Falling)
            {
                Gravitate(actor);           // changes acceleration rate
                actor.Accelerate_Y();       // adjust speed by acceleration rate

                int X = sprite.GL_Position.X;
                int Y = sprite.GL_Position.Y;

                //-- if speed is negative we are on the downwards part of the jump arc

                if (actor.ySpeed < 0)
                {
                    bool Landing = level.Blocks[X][Y - 1].IsSolid;

                    /*-------------------------------
                     * 
                     *  Are We About to Land?
                     * YES? 
                     * 
                     * Then stop falling.
                     * 
                     * ---------------------------*/

                    if (Landing)
                    {
                        actor.ySpeed = 0;
                        actor.yAccelerationRate = 0;
                        actor.Falling = false;

                        actor.Standing = true;
                        sprite._Status = Status.standing;

                        deviceScreen.MoveSprite(sprite, new Block(X, Y));
                    }

                    /*---------------------------
                     * 
                     * NO? then keep falling
                     * 
                     * ------------------------*/

                    else
                    {
                        sprite.Translate(0, (0 - actor.ySpeed) / 2);
                    }
                }

                //====================================================================

                //-- speed is positive, so we are still on the upwards part of the jump arc

                else if (actor.ySpeed > 0)
                {
                    //-- is there a block overhead? 
                    //-- I don't know why, but this will ONLY work at Y + 3.  

                    if (level.Blocks[actor.BlockPosition.X][actor.BlockPosition.Y + 3].IsSolid)
                    {
                        actor.yAccelerationRate = 0;
                        actor.ySpeed = 0;
                    }

                    else
                    {
                        sprite.Translate(0, 0 - actor.ySpeed);
                    }
                }
            }
        }

        //============================================================

        /*-------------------------------------------
         * 
         * Helper Function to subject a given 
         * character to gravity
         * 
         * terminal velocity: -25 pixels / frame
         * decleration rate: -5 pixels / frame
         * 
         * -----------------------------------------*/

        internal void Gravitate(Character actor)
        {
            //-- decelerate vertically to reflect the force of gravity

            if (actor.Falling)
            {
                if (actor.yAccelerationRate > -25.0)
                {
                    actor.yAccelerationRate -= (5.0);
                }
            }
        }

        //============================================================

        /*--------------------------------------
         * 
         * Evalute Lateral movement logic
         * for a given actor
         * 
         * -----------------------------------*/

        internal void EvaluateLateralMovement(Character actor, Sprite sprite)
        {
            int X = actor.BlockPosition.X;
            int Y = actor.BlockPosition.Y;

            bool Blocked = false;

            //-------------------------------------------------------------------
            //-- accelerate / decelerate checks

            //-- is the playerInput "L" or "R" ? Then accelerate in that direction

            if (Math.Abs(actor.xAccelerationRate) > 0)
                LateralAccelerate(actor);

            //-- otherwise, xAcceleration rate is 0, player is not inputting anything

            else
            {
                double Deceleration_Rate = actor.Max_X_Speed / 3;

                if (Math.Abs(actor.xSpeed) > 0)
                {
                    //-- are we close to 0? then just 0 it out this pass

                    if (Math.Abs(actor.xSpeed) < Deceleration_Rate)
                    {
                        actor.xSpeed = 0;
                    }

                    //-- otherwise decelerate

                    else
                    {
                        if (actor.xSpeed > 0)
                            actor.xSpeed -= Deceleration_Rate;
                        else
                            actor.xSpeed += Deceleration_Rate;
                    }
                }
            }

            //----------------------------------------------------------------
            //--- if xSpeed > 0 we are going right

            if (actor.xSpeed > 0)
            {
                //-- bounds check 

                if (X + 1 == level.HighX)
                {
                    Blocked = true;
                }

                //-- obstacle check 

                else
                {
                    if (level.Blocks[X + 1][Y].IsSolid)
                    {
                        Blocked = true;
                    }
                }
            }

            //----------------------------------------------------------------
            //--- if xSpeed < 0 we are going left

            else if (actor.xSpeed < 0)
            {
                //-- bounds check 

                if (X - 1 == 0)
                {
                    Blocked = true;
                }

                //-- obstacle check 

                else
                {
                    if (level.Blocks[X - 1][Y].IsSolid)
                    {
                        Blocked = true;
                    }
                }
            }

            //----------------------------------------------------------------

            if (Blocked)
            {
                Trouble3 = Blocked.ToString();


                actor.xSpeed = 0;
                actor.xAccelerationRate = 0;
            }
            else
            {
                sprite.Translate(actor.xSpeed, 0);
            }

            //----------------------------------------------------------------

            //-- evaluate weather we stepped off a ledge
            //-- needs a bounds check 

            if (Y - 1 <= level.LowY)
            {
                // this is where the player falls to their deaths.

                if (actor == player1)
                {
                    ResetPlayer1();
                }
                else
                {
                    Actors.Remove(actor);
                }
            }
            else
            {
                if (!level.Blocks[X][Y - 1].IsSolid)
                {
                    actor.Falling = true;
                    sprite._Status = Status.falling;
                }
            }
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

        /*-----------------------------
         * 
         * Accelerate helper function 
         * 
         * ----------------------------*/

        internal void LateralAccelerate(Character actor)
        {
            if (Math.Abs(actor.xSpeed) < actor.Max_X_Speed)
            {
                actor.xSpeed += actor.xAccelerationRate;
            }
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

        #endregion
    }
}
