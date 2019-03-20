using SkiaSharp;
using System;
using System.IO;
using System.Reflection;
using Valkyrie.GL;

/*===============================================================
 *  Adam Coville
 *  adam.coville@gmail.com
 * 
 *  Upskilled ICT401515 / Core Infrastructure Mobile Project 
 *  
 *  The Sprite class represents a static image that will not  
 *  change in response to events and game logic. These will be used
 *  for example, to render static obstacles in the foreground that 
 *  players can stand on and collide with.
 *  
 * ============================================================*/

namespace DarkValkyrie.Graphics
{
    public class Tile
    {
        //-- indicators that the sprite is OK to draw on-screen

        public bool Ready { get; }
        public string LoadMsg { get; }

        //-- the complete embedded path, with filename

        public string ImageSource { get; set; }

        //-- the embedded resource directory associated with this sprite

        public string SourceDirectory { get; set; }

        //--------- SkiaSharp pixel coordinate

        internal SKPoint _sKPosition;
        public SKPoint SkiaPosition
        {
            get => _sKPosition;
            set => _sKPosition = value;
        }

        //---------- Game Logic Block coordinate

        internal Block _blockPosition;
        public Block BlockPosition
        {
            get => _blockPosition;
            set => _blockPosition = value;
        }

        internal SKBitmap displayImage;
        public ref SKBitmap Image
        {
            get
            {
                return ref displayImage;
            }
        }

        //===================================================

        /*------------------------------------
         * 
         * Translate function (SkiaSharp pixels)
         * 
         * ----------------------------------*/

        public void Translate(float deltaX, float deltaY)
        {
            _sKPosition.X += deltaX;
            _sKPosition.Y += deltaY;
        }

        //====================================================================

        /*--------------------------------
         * 
         * Constructor
         * 
         * ---------------------------------*/

        public Tile()
        {
            LoadMsg = "No source directory provided.";
            Ready = false;
            _sKPosition = new SKPoint(0, 0);

            ImageSource = string.Empty;
            SourceDirectory = string.Empty;
            displayImage = new SKBitmap();
        }

        //===================================================

        /*--------------------------------------
         * 
         * Constructor overload accepting a 
         * string literal to the embedded resource
         * 
         * -----------------------------------*/

        public Tile(string source)
        {
            ImageSource = source;

            LoadMsg = string.Empty;

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(source))
                {
                    displayImage = SKBitmap.Decode(stream);
                    Ready = true;

                    LoadMsg = "Ready";
                }
            }
            catch (Exception ex)
            {
                // if an error message was attached for modification 

                LoadMsg = ex.ToString() + Environment.NewLine;
                LoadMsg += "Error, could not load: " + source;
            }
        }
    }
}
