using System;
using System.Collections.Generic;

/*=========================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 *  Regions are a collection of blocks which govern in-game
 *  behavior such as reaching a save point, collisions, 
 *  combat and obstacles. The origin point (0, 0) is in the lower
 *  left corner
 *  
 *  If this were in C++ I would have liked to be able to make
 *  this a 2-dimensional vector, as it would allow dynamic resizing
 *  and easy indexing. As matters stand it appears my best options are:
 *  
 *  1 - ArrayList, which Microsoft recommends replacing with generic Lists
 *  2 - a nested List? 
 *  3 - Possibly a data table structure?
 *  
 *  I would like them to be resizeable so that regions may shrink or grow
 *  in response to in-game behavior. I could handle shrinkage by designating
 *  some blocks as being in a dead-band to ignore but to grow it would require
 *  re-initializing a new array which seems wasteful to me..
 * 
 * ======================================================================*/

namespace Valkyrie.GL
{
    public class Region
    {
        //-------------------------------------------------

        private List<List<Block>> blocks;
        public List<List<Block>> Blocks
        {
            get
            {
                return blocks;
            }
            set
            {
                blocks = value;
            }
        }

        //------------------------------------------------

        public string label { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int area { get; set; }

        //--- X boundaries

        public int LowX
        {
            get
            {
                return Blocks[0][0].X;
            }
        }

        public int HighX
        {
            get
            {

                return width;
            }
        }

        //--- Y boundaries

        private int lowY = 0;
        public int LowY
        {
            get
            {
                return Blocks[0][0].Y;
            }
        }

        private int highY = 0;
        public int HighY
        {
            get
            {
                return height;
            }
        }

        //--- origin (block in the lower-left corner)

        private Block origin;
        public Block Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        //--- boolean flag indicating weather it is in the visible area or not

        public bool Visible { get; set; }

        //====================================================================

        /*--------------------------------------------
         * 
         * Constructors
         *   - default
         *   - origin, height, width, origin
         *   - origin, height, width, origin, label
         * 
         * -----------------------------------------*/

        public Region()
        {
            label = string.Empty;
            height = 0;
            width = 0;
            area = 0;
            blocks = new List<List<Block>>();
            Visible = false;
            origin = new Block();
        }

        public Region(int H, int W, Block o)
        {
            height = H;
            width = W;

            area = height * width;

            origin = o;
            label = string.Empty;

            Initialize();
            Visible = false;
        }

        public Region(int H, int W, Block o, string L)
        {
            height = H;
            width = W;
            area = height * width;

            origin = o;
            label = L;

            Initialize();
            Visible = false;
        }

        //====================================================================

        /*----------------------------------
         * 
         * ToString method
         * 
         * ---------------------------------*/

        public override string ToString()
        {
            var result = string.Empty;

            for (int i = 0; i < HighX; i++)
            {
                for (int j = 0; j < HighY; j++)
                {
                    if (Blocks[i][j].IsSolid)
                    {
                        result += "[x]";
                    }
                    else
                    {
                        result += "[ ]";
                    }
                }

                result += Environment.NewLine;
            }

            return result;
        }

        //====================================================================

        /*----------------------------------
         *
         * Getter for the List's count
         * 
         * -------------------------------*/

        public int Count()
        {
            return blocks.Count;
        }

        //====================================================================

        /*------------------------------------------
         * 
         * Initialize helper function
         * 
         * optional bool parameter will set 
         * IsSolid to True for all blocks.
         * 
         * The nested List is built columns
         * first, rows second. This will allow
         * me to access its elements using 
         * [x][y] notation, which is necessary 
         * for my sanity.
         * 
         * ---------------------------------------*/

        public void Initialize(bool Solid = false)
        {
            blocks = new List<List<Block>>();
            blocks.Clear();

            //-- and now for a ghetto 2d List

            for (int i = 0; i < width; i++)
            {
                List<Block> column = new List<Block>();

                for (int j = 0; j < height; j++)
                {
                    Block temp = new Block();

                    temp.X = origin.X + i;
                    temp.Y = origin.Y + j;

                    temp.Label = label + "[" + i + "]" + "[" + j + "]";

                    temp.IsSolid = Solid;

                    column.Add(temp);
                }

                blocks.Add(column);
            }
        }

        //======================================================================

        /*---------------------------------------------
         * 
         * Translate Function
         * 
         * lateral translation in a manually defined
         * slope
         * 
         * ------------------------------------------*/

        public void Translate(int deltaX, int deltaY)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    blocks[i][j].X += deltaX;
                    blocks[i][j].Y += deltaY;
                }
            }

            //-- update origin point

            origin.X += deltaX;
            origin.Y += deltaY;
        }

        //======================================================================

        /*---------------------------------------------
         * 
         * ContainsBlock function
         * 
         * determines if a block is inside the region
         * 
         * I am betting that Linq can do this faster
         * than a naive algorithm and the framerate 
         * seems pretty choppy right now so here 
         * goes nothing.
         * 
         * V2 of this simply accesses the appropriate
         * index. Sorry LINQ, there's no way you're 
         * faster than that.
         * 
         * ------------------------------------------*/

        public bool ContainsBlock(Block b)
        {
            bool result = false;

            int x = b.X;
            int y = b.Y;

            if (x > 0 && x < HighX)
            {
                if (y > 0 && y < HighY)
                {
                    result = true;
                }
            }

            return result;
        }

        //======================================================================

        /*----------------------------------------------------
         * 
         * Intersects
         *
         * function determines if there is any overlap 
         * with another region
         * 
         * Compares high and low X, Y values to avoid
         * having to iterate through the lists
         * 
         * By using the high-low ranges I hope to be able
         * to solve this question in 5 evaluations rather
         * than n^2 evaluations.
         * 
         * -------------------------------------------------*/

        public static bool Intersects(Region a, Region b)
        {
            bool result = false;

            /*-------------------------------------------
             * 
             * Is there overlap in the X axis?
             * 
             * For this to be true one of the edges
             * of region B must be within A's range
             * 
             * ----------------------------------------*/

            bool x_overlap = false;

            //-- the left edge of b is within A's band

            if (b.LowX >= a.LowX && b.HighX <= a.HighX)
                x_overlap = true;

            //-- the right edge of b is within A's band

            if (b.HighX <= a.HighX && b.HighX >= a.LowX)
                x_overlap = true;

            /*-------------------------------------------
            * 
            * Is there overlap in the Y axis?
            * 
            * For this to be true one of the edges
            * of region B must be within A's range
            * 
            * ----------------------------------------*/

            bool y_overlap = false;

            //-- the left edge of b is within A's band

            if (b.lowY >= a.lowY && b.lowY <= a.highY)
                y_overlap = true;

            //-- the right edge of b is within A's band

            if (b.highY <= a.highY && b.highY >= a.lowY)
                y_overlap = true;

            //-- if both axes overlap then they must intersect

            if (x_overlap && y_overlap)
                result = true;

            return result;
        }
    }
}
