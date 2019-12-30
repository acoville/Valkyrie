/*==================================================================
 * 
 *  Game Page View Model 
 *   partial class implementation 
 * 
 *   load map and related helper functions 
 * 
 * ================================================================*/

using System;
using System.ComponentModel;
using Valkyrie.GL;
using DarkValkyrie.Graphics;
using System.Xml;
using System.IO;
using System.Reflection;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        public Level level;
        public string MapName { get; set; }
        public bool MapLoaded { get; set; }

        //===========================================================================

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

            Sprite player1Sprite = new Sprite(player1);

            //-- set up player1 in the physics handler

            Actor PL1 = new Actor(player1, player1Sprite);
            Actors.Add(PL1);
            deviceScreen.AddCharacter(PL1);

            //-- set up control

            ControlProfileLoaded = LoadInputProfile("ErinaProfile.xml");
            PlayerInputChanged = new InputChangedHandler(OnInputChanged);
        }

        //===========================================================================

        /*------------------------------------------------
         * 
         * Helper function to load the level, 
         * these should be .xml files included 
         * in the /Model/Maps/ directory of the 
         * .NET Standard Library, with the
         * Build Action: Embedded Resource
         * 
         * --------------------------------------------*/

        internal bool LoadMap(string levelName, bool ResumeGame = false)
        {
            XmlDocument _level = new XmlDocument();

            //--- if this is resuming the saved game, load that

            if (levelName == "save.xml")
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "save.xml");
                string fileContents = File.ReadAllText(fileName);

                _level.LoadXml(fileContents);
            }

            //--- otherwise, load the embedded resource

            else
            {
                var ResourceID = "DarkValkyrie.Model.Maps." + levelName;
                var assembly = GetType().GetTypeInfo().Assembly;

                using (Stream stream = assembly.GetManifestResourceStream(ResourceID))
                    _level.Load(stream);
            }

            level = new Level(_level);

            //-------------- load saved game if requested

            BackgroundImage = level.ImageSource;
            AddTiles();
            AddMonsters();

            if (level.StartingLocation.Label == "start")
                return true;
            else
                return false;
        }

        //========================================================================

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

                Actor newMob = new Actor(mob1, mob1Sprite);

                //-- add them to the physics handler

                Actors.Add(newMob);

                //-- add their sprites to the display handler

                mob1.SpriteIndex = deviceScreen.Sprites.Count;
                deviceScreen.AddCharacter(newMob);
            }
        }

        //=========================================================================

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
    }
}
