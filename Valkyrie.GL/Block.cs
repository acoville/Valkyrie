using System;
using System.Xml;

/*============================================================
 * 
 *  Adam Coville
 *  adam.coville@gmail.com
 *  Upsilled ICT_CIV_PROG_201810 
 *  Mobile Project
 * 
 *  Blocks are the smallest unit of  
 *  concern to the game's engine. Anything
 *  which interacts with the player is going
 *  to occupy a block or combination of blocks
 *  (Region class). 
 * 
 * ==========================================================*/

namespace Valkyrie.GL
{
    public class Block
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Label { get; set; }

        public bool IsSolid { get; set; }

        //===========================================================

        /*--------------------------------------
         * 
         * Constructors
         *    default
         *    parameterized (x, y)
         *    parameterized (x, y, label)
         * 
         * -----------------------------------*/

        public Block()
        {
            X = 0;
            Y = 0;
            Label = string.Empty;

            IsSolid = false;
        }

        //--

        public Block(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            Label = string.Empty;

            IsSolid = false;
        }

        //--

        public Block(string L, int newX, int newY)
        {
            X = newX;
            Y = newY;
            Label = L;

            IsSolid = false;
        }

        //===========================================================

        /*--------------------------------------
         * 
         * ToString
         * 
         * -----------------------------------*/

        public override string ToString()
        {
            var result = string.Empty;

            result = Label +
                     ": X: " + X
                      + ", Y: " + Y;

            return result;
        }

        //===========================================================

        public string ToXml()
        {
            string text;

            text = "<Block" + Environment.NewLine;
            text += "Label=\"" + Label + "\""   + Environment.NewLine;
            text += "X=\"" + X + "\""           +Environment.NewLine;
            text += "Y=\"" + Y + "\" />"        + Environment.NewLine;

            text += Environment.NewLine;

            return text;
        }

        //===========================================================

        /*--------------------------------------
         * 
         * Equality override
         * 
         * -----------------------------------*/

        public bool Equals(Block other)
        {
            bool result = false;

            if ((other == null) || !this.GetType().Equals(other.GetType()))
            {
                result = false;
            }
            else
            {
                if (X == other.X)
                    if (Y == other.Y)
                        if (Label == other.Label)
                            result = true;
            }

            return result;
        }

        //===========================================================

        /*-------------------------------------------
         * 
         * Microsoft recommends override GetHash 
         * anytime IEquatable<T> is implemented to 
         * avoid boxing / unboxing.
         * 
         * ----------------------------------------*/

        public override int GetHashCode()
        {
            return (X ^ Y) + Label.GetHashCode() + IsSolid.GetHashCode();
        }

        //============================================================

        /*--------------------------------------
         * 
         * Function to resolve difference between
         * two blocks
         * 
         * ------------------------------------*/

        public int DistanceTo(Block other)
        {
            //-- if they are co-axial (X axis)

            if (this.X == other.X)
            {
                return Math.Abs(this.Y - other.Y);
            }

            //-- if they are co-axial (Y axis)

            if (this.Y == other.Y)
            {
                return Math.Abs(this.X - other.X);
            }

            //-- if they are not co-axial, apply pythagorean theorem

            double A = Math.Abs(this.X - other.X);
            double B = Math.Abs(this.Y - other.Y);

            return (int)Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
        }
    }
}
