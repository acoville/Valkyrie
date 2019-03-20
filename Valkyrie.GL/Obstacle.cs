using System.Xml;

/*====================================================
 * 
 *  Adam Coville
 *  adam.coville@gmail.com
 *  Upsilled ICT_CIV_PROG_201810 
 *  Mobile Project
 * 
 *  Obstacle class
 * 
 * ==================================================*/

namespace Valkyrie.GL
{
    public class Obstacle : Region
    {
        public string Image { get; set; }       // 

        public bool Stationary { get; set; }    // controls weather it is static or mobile

        public bool Tangible { get; set; }      // turns collisions on / off 

        public string ImageSource { get; set; }

        //======================================================

        /*----------------------------------
         * 
         * Constructor
         * 
         * -------------------------------*/

        public Obstacle()
            : base()
        {
            Tangible = true;
            Visible = true;
            Stationary = true;
            Image = string.Empty;
        }

        //======================================================

        /*---------------------------------
         * 
         * Constructor accepting an XmlNode
         * as an argument
         * 
         * ------------------------------*/

        public Obstacle(XmlNode node)
            : base()
        {
            label = node.Attributes["Label"].Value;

            Block origin = new Block();

            origin.X = int.Parse(node.Attributes["X1"].Value);
            origin.Y = int.Parse(node.Attributes["Y1"].Value);

            int x2 = int.Parse(node.Attributes["X2"].Value);
            int y2 = int.Parse(node.Attributes["Y2"].Value);

            Image = node.Attributes["Skin"].Value;

            ImageSource = "DarkValkyrie.Graphics.Tiles." + node.Attributes["Skin"].Value;

            //-- ok so if an Obstacle starts and stops on the same block then 
            //-- it has a height and width of 1, not 0. 

            //-- otherwise, it has however many blocks wide or high it is.

            width = x2 - origin.X + 1;

            height = y2 - origin.Y + 1;

            area = height * width;
            Origin = origin;

            Initialize(true);
        }
    }
}
