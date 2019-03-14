using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Valkyrie.GL;

namespace DarkValkyrie.Graphics
{
    public delegate void DisplayImageChangedHandler(object sender, SpriteEventArgs e);

    //=============================================================

    public class Sprite
    {
        public event DisplayImageChangedHandler FacingChanged;
        public event DisplayImageChangedHandler StatusChanged;

        //=============================================================

        internal int lateralPixelsTraveled;
        public int LateralPixelsTraveled
        {
            get
            {
                return lateralPixelsTraveled;
            }
            set
            {
                lateralPixelsTraveled = value;

                if (lateralPixelsTraveled > 75 || lateralPixelsTraveled < -75)
                {
                    //-- going right

                    if (lateralPixelsTraveled > 0)
                    {
                        GL_Position.X += 1;
                        lateralPixelsTraveled -= 75;
                    }

                    //-- going left

                    else
                    {
                        GL_Position.X -= 1;
                        lateralPixelsTraveled += 75;
                    }
                }
            }
        }

        //=============================================================

        internal int verticalPixelsTraveled;
        public int VerticalPixelsTraveled
        {
            get
            {
                return verticalPixelsTraveled;
            }
            set
            {
                verticalPixelsTraveled = value;

                if (verticalPixelsTraveled > 75 || verticalPixelsTraveled < -75)
                {
                    //-- going up

                    if (verticalPixelsTraveled > 75)
                    {
                        GL_Position.Y -= 1;
                        verticalPixelsTraveled -= 75;
                    }

                    //-- going down

                    if (verticalPixelsTraveled < -75)
                    {
                        GL_Position.Y += 1;
                        verticalPixelsTraveled += 75;
                    }
                }
            }
        }

        //==========================================================

        //-- FACING

        internal Facing facing;
        public Facing _Facing
        {
            get
            {
                return facing;
            }
            set
            {
                if (value != facing)
                {
                    facing = value;
                    OnFacingChanged(this, new SpriteEventArgs(this, status, facing));
                }
            }
        }

        //==========================================================

        /*----------------------------------------------------
         * 
         * Translate Function
         * 
         * It is tempting to invert Y here, but I feel like 
         * if I did I'd just lose track of it elsewhere.
         * 
         * ---------------------------------------------------*/

        public void Translate(double x, double y)
        {
            _skiaPosition.X += (int)x;
            LateralPixelsTraveled += (int)x;

            _skiaPosition.Y += (int)y;
            VerticalPixelsTraveled += (int)y;
        }

        //==========================================================

        /*----------------------------------
         * 
         * FacingChangedEventHandler
         * 
         * --------------------------------*/

        protected virtual void OnFacingChanged(object sender, SpriteEventArgs e)
        {
            var Actor = sender as Sprite;
            Actor.Reverse();
        }

        //=========================================================

        /*-------------------------------------------------------------------
         * 
         * Property for Status
         * 
         * Changes in Status will trigger an event to change
         * the display image. 
         * 
         * For transitions between standing and crouching, the 
         * sprite needs to be translated vertically to look correct.
         * 
         * Skia's coordinate origin is in the upper-left corner which
         * is confusing, because +Y is now in the down direction, and 
         * -Y is up.
         * 
         * I would have preferred to handle this in the UpdateImage
         * function but that doesn't track what the old state is.
         * 
         * ---------------------------------------------------------------*/

        internal Status status;
        public Status _Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    bool Standing_Up_From_Crouch = false;
                    bool Crouching_From_Standing = false;

                    // evaultate conditions before modifying

                    //------------------------------------------------------------------

                    //-- determine if we are transitioning from crouching to standing

                    if (status == Status.crouching && value != Status.crouching)
                    {
                        Standing_Up_From_Crouch = true;
                    }

                    //------------------------------------------------------------------

                    //-- determine if transitioning from any other status to crouching

                    if (status != Status.crouching && value == Status.crouching)
                    {
                        Crouching_From_Standing = true;
                    }

                    //------------------------------------------------------------------

                    // modify

                    status = value;
                    OnStatusChanged(this, new SpriteEventArgs(this, status, facing));

                    //------------------------------------------------------------------

                    // take necessary actions after modifying

                    //-- if Crouching, translate Y downwards (positive deflection)

                    if (Crouching_From_Standing)
                    {
                        double deltaY = standingImg.Height - crouchingImg.Height;

                        _skiaPosition.Y += (int)deltaY;
                    }

                    //------------------------------------------------------------------

                    //-- if standing up from a crouch, translate upwards(negative)

                    if (Standing_Up_From_Crouch)
                    {
                        double deltaY = standingImg.Height - crouchingImg.Height;

                        _skiaPosition.Y -= (int)deltaY;
                    }


                }
            }
        }

        //===================================================================

        /*----------------------------------
         * 
         * Sprite Changed Event Handler
         * 
         * -------------------------------*/

        protected virtual void OnStatusChanged(object sender, SpriteEventArgs e)
        {
            var Actor = sender as Sprite;

            Actor.UpdateDisplayImage();
        }

        //===================================================================

        //-- indicator that the sprite is OK to draw on-screen

        public bool Ready { get; }
        public string LoadMsg { get; }

        //===================================================================

        /*----------------------------------------
         * 
         * Block Position of the Sprite
         * 
         * ------------------------------------*/

        internal Block gl_Position;
        public Block GL_Position
        {
            get
            {
                return gl_Position;
            }
            set
            {
                gl_Position = value;
            }
        }

        //===========================================================================

        /*------------------------------
         * 
         * the embedded resource directory associated with this sprite
         * there should be a matching PNG or GIF associated with every status
         * 
         * ----------------------------*/

        public string SourceDirectory { get; set; }

        internal SKPoint _skiaPosition;
        public SKPoint SkiaPosition
        {
            get
            {
                return _skiaPosition;
            }
            set
            {
                _skiaPosition = value;
            }
        }

        //==========================================================================

        //-- these are the basic images common to monsters and the player

        internal SKBitmap standingImg;
        internal SKBitmap crouchingImg;
        internal SKBitmap fallingImg;
        internal SKBitmap displayImage;     // the one being used at any given time
        internal SKBitmap attackImg;

        public ref SKBitmap Image
        {
            get
            {
                return ref displayImage;
            }
        }

        //===========================================================================

        /*-------------------------------------
         * 
         * Constructors
         * 
         * ----------------------------------*/

        public Sprite()
        {
            facing = Facing.right;
            status = Status.standing;

            LoadMsg = "No source directory provided.";
            Ready = false;

            displayImage = new SKBitmap();
            fallingImg = new SKBitmap();
            crouchingImg = new SKBitmap();
            standingImg = new SKBitmap();
            attackImg = new SKBitmap();

            _skiaPosition = new SKPoint(0, 0);
            GL_Position = new Block(0, 0);

            verticalPixelsTraveled = 0;
            lateralPixelsTraveled = 0;

            //-- delegates

            FacingChanged = new DisplayImageChangedHandler(OnFacingChanged);
            StatusChanged = new DisplayImageChangedHandler(OnStatusChanged);
        }

        //==========================================================

        /*-------------------------------------------
         * 
         * Constructor accepting a source directory
         * parameter
         * 
         * ------------------------------------------*/

        public Sprite(Character character)
        {
            SourceDirectory = "DarkValkyrie.Graphics." + character.SpriteSource;

            displayImage = new SKBitmap();
            fallingImg = new SKBitmap();
            crouchingImg = new SKBitmap();
            standingImg = new SKBitmap();
            attackImg = new SKBitmap();

            LoadMsg = string.Empty;
            bool SpritesLoaded = LoadSprites(LoadMsg);

            Ready = false;

            if (SpritesLoaded)
            {
                LoadMsg = "Images Loaded, sprite ready.";
                Ready = true;
            }

            _Facing = Facing.right;
            _Status = Status.standing;

            UpdateDisplayImage();

            //-- initialize these coordinates, and then move them to where they need to be.

            GL_Position = character.BlockPosition;
            _skiaPosition = new SKPoint(0, 0);

            verticalPixelsTraveled = 0;
            lateralPixelsTraveled = 0;

            //-- wire up the delegate

            FacingChanged = new DisplayImageChangedHandler(OnFacingChanged);
            StatusChanged = new DisplayImageChangedHandler(OnStatusChanged);
        }

        //===============================================================

        /*-------------------------------------------
         * 
         * String to fetch the pixels traveled data
         * to aid in troubleshooting
         * 
         * ----------------------------------------*/

        public string Deltas()
        {
            string result = "X: " + LateralPixelsTraveled
                            + ", Y: " + VerticalPixelsTraveled;

            return result;
        }

        //===============================================================

        /*---------------------------------------------
         * 
         * ToString() method to aid in troubleshooting
         * 
         * -------------------------------------------*/

        public override string ToString()
        {
            string result = "X: " + SkiaPosition.X
                            + ", Y: " + SkiaPosition.Y;

            return result;
        }

        //===============================================================

        /*-----------------------------------------------
         * 
         * Loader helper function 
         * 
         * returns true if the load was successful
         * 
         * false if there was an error, ErrMsg will 
         * indicate which image failed if it is provided
         * 
         * ---------------------------------------------*/

        public bool LoadSprites(string ErrMsg = null)
        {
            string source_standing = SourceDirectory + ".standing.png";
            string source_crouching = SourceDirectory + ".crouching.png";
            string source_falling = SourceDirectory + ".falling.png";
            string source_attack = SourceDirectory + ".attack.png";

            bool stand_loaded = false;
            bool crouch_loaded = false;
            bool fall_loaded = false;
            bool attack_loaded = false;

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            //-------------- load the appropriate iamges from the Graphics directory

            try
            {
                //-- standing

                using (Stream stream = assembly.GetManifestResourceStream(source_standing))
                {
                    standingImg = SKBitmap.Decode(stream);
                    stand_loaded = true;
                }

                //-- crouching

                using (Stream stream = assembly.GetManifestResourceStream(source_crouching))
                {
                    crouchingImg = SKBitmap.Decode(stream);
                    crouch_loaded = true;
                }

                //-- falling 

                using (Stream stream = assembly.GetManifestResourceStream(source_falling))
                {
                    fallingImg = SKBitmap.Decode(stream);
                    fall_loaded = true;
                }

                //-- attack

                using (Stream stream = assembly.GetManifestResourceStream(source_attack))
                {
                    attackImg = SKBitmap.Decode(stream);
                    attack_loaded = true;
                }
            }

            //-- if there was a stream error

            catch (Exception ex)
            {
                //-- if an Error Message was attached for modification

                if (!string.IsNullOrEmpty(ErrMsg))
                {
                    ErrMsg = ex.ToString() + Environment.NewLine;

                    if (!stand_loaded)
                    {
                        ErrMsg += "Error, could not load: " + SourceDirectory + ".standing" + Environment.NewLine;
                    }

                    if (!crouch_loaded)
                    {
                        ErrMsg += "Error, could not load: " + SourceDirectory + ".crouching" + Environment.NewLine;
                    }

                    if (!fall_loaded)
                    {
                        ErrMsg += "Error, could not load: " + SourceDirectory + ".falling" + Environment.NewLine;
                    }
                }

                //-- either way, indicate the load failed.

                return false;
            }

            return true;
        }

        //=============================================================

        /*------------------------------------
         * 
         * Helper function to update
         * displayImage, called whenver
         * the status or facing property 
         * changes
         * 
         * ----------------------------------*/

        internal void UpdateDisplayImage()
        {
            //-- find which image to use

            switch (status)
            {
                case (Status.standing):
                    {
                        displayImage = standingImg;
                        break;
                    }

                case (Status.crouching):
                    {
                        displayImage = crouchingImg;
                        break;
                    }

                case (Status.falling):
                    {
                        displayImage = fallingImg;
                        break;
                    }

                case (Status.attack):
                    {
                        displayImage = attackImg;
                        break;
                    }

                default:
                    {
                        displayImage = standingImg;
                        break;
                    }
            }

            if (_Facing == Facing.left)
                Reverse();
        }

        //======================================================

        /*------------------------------------------
         * 
         * Simple transform which mirrors the image
         * 
         * ----------------------------------------*/

        public void Reverse()
        {
            SKBitmap newImage = new SKBitmap(displayImage.Width, displayImage.Height, false);

            for (int i = 0; i < displayImage.Height; i++)
            {
                for (int j = 0; j < displayImage.Width; j++)
                {
                    SKColor color = displayImage.GetPixel(displayImage.Width - j, i);

                    newImage.SetPixel(j, i, color);
                }
            }

            displayImage = newImage;
        }
    }

    //======================================================================

    //-- these correspond to the SideScroller.GL enums by the same name

    public enum Status
    {
        standing,
        crouching,
        falling,
        attack
    };
    
    public enum Facing { left, right };
}