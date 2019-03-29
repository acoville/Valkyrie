using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

/*==============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * Level Class
 * 
 * ===========================================================*/

namespace Valkyrie.GL
{
    public class Level : Region
    {
        public Block StartingLocation { get; set; }

        //--- properties describing the limits of the map

        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Top { get; set; }

        //-----------------------------------------------------

        internal double _gravity;
        public double Gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        //-----------------------------------------------------

        //-- spot the player is occupying
        public Block CurrentLocation { get; set; }

        //-----------------------------------------------------

        //-- Background Image
        public string ImageSource { get; set; }

        //-----------------------------------------------------

        //-- part of the map to be rendered
        public Region VisibleArea { get; set; }

        //-----------------------------------------------------

        //-- level name property

        private string levelName;
        public string LevelName
        {
            get
            {
                return levelName;
            }
            set
            {
                levelName = value;
            }
        }

        //-----------------------------------------------------

        //-- A 2-dimensional array equivalent of blocks representing
        //-- the entire level

        //private List<List<Block>> blocks;

        //-----------------------------------------------------

        //--- property controlling the List<Obstacle> of static obstacles

        private List<Obstacle> staticObstacles;
        public List<Obstacle> StaticObstacles
        {
            get => staticObstacles;
            set => staticObstacles = value;
        }

        //-----------------------------------------------------

        //--- property controlling the List<Character> of monsters

        private List<Character> monsters;
        public List<Character> Monsters
        {
            get => monsters;
            set => monsters = value;
        }

        //-----------------------------------------------------

        //--- getter for the Count of static obstacles in memory

        public int NbrOfObstacles
        {
            get
            {
                return staticObstacles.Count;
            }
        }

        //=================================================================

        /*---------------------------------------------------------------------
         * 
         * IsOccupied function
         * 
         * Now all I have to do is access the indexed block and return 
         * its bit. Simplest time-complexity solution to detecting collisions,
         * IMO. But I kind of wish I had gone about this another way now
         * that blocks are giving me some trouble.
         * 
         * This is proving a bit problematic, it seems to be missing 1/3 to 1/2 
         * of the time.
         * 
         * -----------------------------------------------------------------*/

        public bool IsOccupied(int x, int y)
        {
            bool result = false;
            Block arg = new Block(x, y);

            if (this.ContainsBlock(arg))
            {
                result = Blocks[x][y].IsSolid;
            }

            return result;
        }

        //=================================================================

        /*--------------------------------------
         * 
         * IsOccupied returns TRUE if this
         * block is part of a static obstacle
         * 
         * -----------------------------------*/

        public bool IsOccupied(Block b)
        {
            bool result = false;


            if (this.ContainsBlock(b))
            {
                result = Blocks[b.X][b.Y].IsSolid;
            }

            return result;
        }

        //=================================================================

        /*-------------------------------------------------------------
         * 
         * Constructor accepting an XmlDocument
         * 
         * note, this one only works with filepaths defined
         * in the local machine. Xamarin embedded resources should
         * be addressed using the framework's . notation with the 
         * build action: Embedded Resource. Also the constructor
         * accepting the stream argument should be used for production.
         * 
         * POSSIBLE EXPLANATION FOR MY AGONY
         * 
         * derived class constructor is called first, however the compiler
         * inserts a call to the base class constructor as the first
         * statement.
         * 
         * In that case the region base-class would be initialized to a 
         * new nested list, however it would have to be re-initialized 
         * and the .IsSolid values manually modified as data is read in 
         * from the source document.
         * 
         * ---------------------------------------------------------*/

        public Level(XmlDocument source)
        {
            XmlNode root = source.DocumentElement;

            //-- starting position Child[0]

            XmlNode start = root.ChildNodes[0];
            InitializeStart(start);

            CurrentLocation = StartingLocation;

            //-- parse each staticObstacle Child[1]

            XmlNode obsRoot = root.ChildNodes[1];
            InitializeObstacles(obsRoot);

            //-- parse each monster Child[2]

            XmlNode mobRoot = root.ChildNodes[2];
            InitializeMonsters(mobRoot);

            //-- retrieve the gravity tag

            XmlNode grav = root.ChildNodes[3];
            InitializeGravity(grav);

            //-- retrieve the height and width, populate the blocks 2d array

            XmlNode metaNode = root.ChildNodes[5];
            InitializeBlocks(metaNode);
        }

        //====================================================================

        /*---------------------------------------
         * 
         * Export to Xml creates an XML document
         * 
         * 
         * -------------------------------------*/

        //====================================================================

        /*--------------------------------------
         * 
         * Helper function to initialize the
         * gravity setting for the level 
         * 
         * * to be implemented later
         * 
         * ------------------------------------*/

        internal void InitializeGravity(XmlNode grav)
        {
            string gravString = grav.Attributes["Rate"].Value;

            double.TryParse(gravString, out _gravity);
        }

        //=====================================================================

        /*------------------------------------
         * 
         * Helper function to initialize the 
         * starting location 
         * 
         * ----------------------------------*/

        public void InitializeStart(XmlNode start)
        {
            StartingLocation = new Block(start.Attributes["Label"].Value,
                                            int.Parse(start.Attributes["X"].Value),
                                            int.Parse(start.Attributes["Y"].Value));
        }

        //=====================================================================

        /*---------------------------------------
         * 
         * Helper Function to initialize
         * the static obstacles
         * 
         * has to be run before InitializeBlocks
         * 
         * -------------------------------------*/

        internal void InitializeObstacles(XmlNode obsRoot)
        {
            StaticObstacles = new List<Obstacle>();

            foreach (XmlNode node in obsRoot.ChildNodes)
            {
                StaticObstacles.Add(new Obstacle(node));
            }
        }

        //=====================================================================

        /*---------------------------------
         * 
         * Helper function to initialize
         * the monsters
         * 
         * -------------------------------*/

        internal void InitializeMonsters(XmlNode mobRoot)
        {
            monsters = new List<Character>();

            foreach (XmlNode node in mobRoot.ChildNodes)
                monsters.Add(new Character(node));
        }

        //=====================================================================

        /*----------------------------------
         * 
         * Save Level function 
         * 
         * ------------------------------*/

        public void SaveLevel(Character player1)
        {
            Block b = player1.BlockPosition;

            var ResourceID = "DarkValkyrie.Model.Maps.TestMap.xml";
            var assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(ResourceID))
            {
                XmlDocument source = new XmlDocument();
                source.Load(stream);

                XmlNode root = source.DocumentElement;
                XmlNode current = root.ChildNodes[6];

                source.RemoveChild(current);

                var stringNode = "Label=\"current\"" + Environment.NewLine
                                + "X=\"" + b.X + "\"" + Environment.NewLine
                                + "Y=\"" + b.Y + "\"" + "/>";

                source.CreateNode("Block", "current", stringNode);
            }

        }

        //=====================================================================

        /*---------------------------------------------
         * 
         * Helper function to initialize
         * the nested List of Blocks 
         * 
         * ------------------------------------------*/

        internal void InitializeBlocks(XmlNode metaNode)
        {
            this.height = int.Parse(metaNode.Attributes["Height"].Value);
            this.width = int.Parse(metaNode.Attributes["Width"].Value);
            this.Origin = new Block(0, 0);
            this.area = height * width;

            this.Initialize();      // ok everything should be allocated by now, which is true level is 1200 square blocks at runtime....

            //-- set every block in a static obstacle's .IsOccupied to TRUE

            foreach (var O in StaticObstacles)
            {
                for (int i = 0; i < O.width; i++)
                {
                    for (int j = 0; j < O.height; j++)
                    {
                        Block block = new Block(O.Blocks[i][j].X, O.Blocks[i][j].Y);

                        Blocks[block.X][block.Y].IsSolid = true;
                    }
                }
            }
        }
    }
}
