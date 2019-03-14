using Xamarin.Essentials;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Valkyrie.GL;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;

/*============================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * The Screen class is responsible for obtaining information about the 
 * native display, and rendering the game level
 * 
 * =========================================================================*/

namespace DarkValkyrie.Graphics
{
    public class Screen
    {
        public Command Redraw { get; set; }

        public bool ShowGrid { get; set; }

        internal bool Initialized;

        //========================================================

        //-- data relating to the sprites displayed on-screen

        internal ObservableCollection<Sprite> Sprites;
        public ObservableCollection<Sprite> _Sprites
        {
            get
            {
                return Sprites;
            }
            set
            {
                Sprites = value;
            }
        }

        //========================================================

        //-- data relating to the obstacles displayed on-screen

        internal ObservableCollection<Tile> _tiles;
        public ObservableCollection<Tile> Tiles
        {
            get
            {
                return _tiles;
            }
            set
            {
                _tiles = value;
            }
        }

        //========================================================

        //-- data relating to the screen's orientation

        public enum Orientation { portrait, landscape, square };
        internal Orientation orientation;
        public Orientation ScreenOrientation
        {
            get
            {
                return orientation;
            }
        }

        //========================================================

        //-- data relating to the screen's height

        internal double height;     // in pixels
        public double Height
        {
            get { return height; }
        }

        //========================================================

        //-- data relating to the screen's width

        internal double width;      // in pixels
        public double Width
        {
            get { return width; }
        }

        internal double resolution;             // # of pixels per block

        //==========================================================

        //- Everything I need to instantiate an SKSurfaceEventArgs object

        public SKSurface Surface { get; set; }
        internal SKImageInfo ScreenDetails()
        {
            SKImageInfo info = new SKImageInfo((int)width, (int)height);
            return info;
        }

        //==========================================================

        /*-------------------------------------------------
         * 
         * .AddCharacterSprite()
         * 
         * this was necessary to include since SkiaPoint 
         * coordinate origin is in the upper-left corner and 
         * Game Logic's block coordinate origin is in the 
         * lower-left corner. Y has to be inverted to "climb up" 
         * from the bottom rather than drip down from the top.
         * 
         * -----------------------------------------------*/

        public void AddCharacter(Sprite newSprite, ref Character newCharacter)
        {
            MoveSprite(newSprite, newCharacter.BlockPosition);

            newSprite.facing = Facing.right;

            Sprites.Add(newSprite);

            newCharacter.SpriteIndex = Sprites.Count - 1;
        }

        //==========================================================

        //-- Function to add a tile 

        public void AddTile(Tile tile, Block target)
        {
            MoveTile(tile, target);

            Tiles.Add(tile);
        }

        //==========================================================

        /*-----------------------------------------
         * 
         * Translates a tile
         * overload accepting a block argument
         * 
         * --------------------------------------*/

        internal void MoveTile(Tile sprite, Block target)
        {
            float newX = (float)(target.X * resolution);
            float newY = (float)(height - (target.Y * resolution));

            SKPoint newPoint = new SKPoint(newX, newY);

            sprite.SkiaPosition = newPoint;

            if (sprite.BlockPosition != target)
            {
                sprite.BlockPosition = target;
            }
        }

        //==========================================================

        /*------------------------------------------------
         * 
         * Translate a Character Sprite to a set block
         *  
         * ---------------------------------------------*/

        internal void MoveSprite(Sprite sprite, Block target)
        {
            //--- SkPoint X coordinate

            float new_x_Sk = target.X * (float)resolution;

            //-- trying to center a sprite on the block 

            if (sprite.Image.Width > 75)
            {
                int center = sprite.Image.Width / 2;
                new_x_Sk -= (center / 2);
            }

            //--- SKPoint Y coordinate

            float new_y_Sk = (float)(height - ((target.Y - 1) * resolution));

            new_y_Sk -= sprite.displayImage.Height;

            sprite.SkiaPosition = new SKPoint(new_x_Sk, new_y_Sk);

            sprite.VerticalPixelsTraveled = 0;
            sprite.LateralPixelsTraveled = 0;
        }

        //==========================================================

        /*--------------------------------------
        * 
        * Constructor
        * 
        * ------------------------------------*/

        public Screen()
        {
            Initialized = false;

            GetScreenDetails();

            Redraw = new Command<SKPaintSurfaceEventArgs>(OnPainting);
            PaintCommand = Redraw;

            Sprites = new ObservableCollection<Sprite>();
            Tiles = new ObservableCollection<Tile>();

            Initialized = true;
        }

        //==========================================================

        /*------------------------------------------
        * 
        * Helper function to detect screen details 
        * of the native display device
        * 
        * ----------------------------------------*/

        public void GetScreenDetails()
        {
            var metrics = DeviceDisplay.MainDisplayInfo;

            // orientation (landscape, portrait, square, unkn)

            width = metrics.Width;
            height = metrics.Height;
            height *= .75;

            resolution = 75;

            /*-----------------------------------------
            * 
            * In portrait mode, a block should appear 
            * as large 1x1 as if it were in landscape,
            * and just render more blocks vertically
            * 
            * The resolution will be determined using the
            * larger of height or width
            * 
            * ---------------------------------------*/

            if (height > width)
            {
                orientation = Orientation.portrait;
            }

            /*-----------------------------------------
            * 
            * In landscape mode, a 1x1 
            * width is larger, and a wider area is 
            * rendered
            * 
            * --------------------------------------*/

            else if (height < width)
            {
                orientation = Orientation.landscape;
            }

            /*---------------------------------------
            * 
            * In square orientation, either dimension
            * is fine
            * 
            * -------------------------------------*/

            else if (height == width)
            {
                orientation = Orientation.square;
            }

            /*---------------------------------------
             * 
             * If Sprites and Tiles are populated, 
             * reposition them as needed
             * 
             * ------------------------------------*/

            if (Initialized)
            {
                if (Sprites.Count > 0)
                {
                    foreach (var sprite in Sprites)
                    {
                        MoveSprite(sprite, sprite.GL_Position);
                    }
                }

                if (Tiles.Count > 0)
                {
                    foreach (var tile in Tiles)
                    {
                        MoveTile(tile, tile.BlockPosition);
                    }
                }
            }

        }

        //=================================================================

        /*---------------------------------------
        * 
        * OnCanvasViewPaintSurface
        * 
        * -------------------------------------*/

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
        }

        //===============================================================================

        //-- everything needed to grab the ImageInfo of the screen.

        public SKImageInfo Info()
        {
            SKImageInfo info = new SKImageInfo((int)width, (int)height);
            return info;
        }

        //===============================================================================

        //-------- Paint colors

        //-- ClearPaint has an alpha channel of 0, making the empty pixels 
        // in this image transparent so the GamePage's background image
        // may be seen.


        internal SKColor ClearPaint = new SKColor(255, 255, 255, 0);

        //===============================================================================

        /*---------------------------------------
         * 
         * Event Handler to redraw the screen
         * 
         * ------------------------------------*/

        public ICommand PaintCommand { get; set; }
        public void OnPainting(SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(ClearPaint);

            //-- draw all sprites in the visiblearea region

            foreach (var s in Sprites)
            {
                canvas.DrawBitmap(s.Image, s.SkiaPosition);
            }

            //-- draw the static obstacles in the visiblearea region

            foreach (var t in Tiles)
            {
                canvas.DrawBitmap(t.Image, t.SkiaPosition);
            }

            //------ test code ------------------

            if (ShowGrid)
            {
                //======================================================

                //-- y axis

                int block = (int)height / 75 + 2;

                for (int i = 0; i < height; i++)
                {
                    if (i % 75 == 0)
                    {
                        string arg = "[" + block + "]";

                        SKPoint point = new SKPoint(25, i);
                        SKPaint line = new SKPaint();
                        line.TextSize = 25;
                        line.Color = new SKColor(200, 200, 200, 200);

                        canvas.DrawText(arg, point, line);

                        SKPoint origin = new SKPoint(50, i);
                        SKPoint term = new SKPoint((float)width, i);

                        canvas.DrawLine(origin, term, line);

                        block--;
                    }
                }

                //======================================================

                block = 0;

                //-- x axis

                for (int i = 0; i < width; i++)
                {
                    if (i % 75 == 0)
                    {
                        string arg = "[" + block + "]";

                        SKPoint point = new SKPoint(i, 25);
                        SKPaint line = new SKPaint();
                        line.TextSize = 25;
                        line.Color = new SKColor(175, 175, 175, 200);

                        canvas.DrawText(arg, point, line);

                        SKPoint origin = new SKPoint(i, 50);
                        SKPoint term = new SKPoint(i, (float)height);

                        canvas.DrawLine(origin, term, line);

                        block++;
                    }
                }
            }
        }
    }
}
