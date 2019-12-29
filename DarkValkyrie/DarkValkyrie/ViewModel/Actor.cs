/*==========================================================================
 * 
 *  Valkyrie v0.2 alpha
 * 
 *  Actor class
 *  
 *  convenience container class that will allow the 
 *  GamePageView Model to direclty associate sprites passed
 *  to the Skia canvas with Game Logic character objects. Before
 *  this approach, I was maintaining differnet lists of canvas 
 *  sprites and GL characters, which gets confusing even with 
 *  3 characters on-screen.
 * 
 * ========================================================================*/

using DarkValkyrie.Graphics;
using Valkyrie.GL;

namespace DarkValkyrie.ViewModel
{
    public class Actor
    {
        internal

        Character character_;
        Sprite sprite_;
        string control_command_;

        //==================================================

        /*------------------------------------------
         * 
         * This class combines a .Game Logic
         * character with a graphics sprite, 
         * and will be responsible for passing 
         * information between them 
         * 
         * ---------------------------------------*/

        public Actor(Character GLchar, Sprite graphicsSprite)
        {
            character_ = GLchar;
            sprite_ = graphicsSprite;
            control_command_ = "";
        }

        //============================================================================

        public Sprite Sprite
        {
            get
            {
                return sprite_;
            }
            set
            {
                sprite_ = value;
            }
        }

        //==========================================================================

        public Character Character 
        {
            get
            {
                return character_;
            }
            set
            {
                character_ = value;
            }
        }

        //==========================================================================

        public string Command
        {
            get
            {
                return control_command_;
            }
            set
            {
                control_command_ = value;
            }
        }

    }
}
