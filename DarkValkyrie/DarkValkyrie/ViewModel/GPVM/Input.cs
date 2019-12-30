using System.ComponentModel;
using Valkyrie.GL;
using DarkValkyrie.Graphics;
using System.Reflection;
using System.IO;
using System.Xml;
using Valkyrie.CommandInterpreter;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //-- Input variables

        private Interpreter interpretor;
        public bool ControlProfileLoaded { get; set; }

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
            Sprite sprite = deviceScreen_.Sprites[actor.SpriteIndex];

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

        //==============================================================================

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

        //============================================================================

        /*--------------------------------------------------------
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
         * -------------------------------------------------------*/

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

        //==================================================================

        /*----------------------------------------
         * 
         * FEATURE DELAYED
         * 
         * Special Commands are at this point,
         * something I need to implement later.
         * 
         * --------------------------------------*/



    }
}