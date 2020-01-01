/*========================================================================
 * 
 *  Game Page View Model 
 *   partial class implementation 
 * 
 *   load map and related helper functions 
 *   
 *   LoadMap must be invoked passed a mapname as a string argument, 
 *   this will by default be searching for a matching filename in the 
 *   "/Model/maps/" directory. 
 *   
 *   NOT QUITE WORKING YET
 *   
 *   An optional second paramter of type bool will indicate weather
 *   the player is requesting to restore a save state or not, in 
 *   which case a ".save.xml" is appended to the map name. 
 *   
 *   The boolean return value of LoadMap indicates to the GamePage
 *   ViewModel that the load was successful, and the game can be started. 
 *   
 *   Load Order: 
 *   
 *   1- 
 *   
 * =======================================================================*/

using System;
using System.ComponentModel;
using Valkyrie.GL;
using DarkValkyrie.Graphics;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        public bool MapLoaded { get; set; }

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
            BackgroundImage = level.ImageSource;
            
            AddTiles();
            SetupPlayer1();
            AddMonsters();

            //-- returning a true result of this function will 
            // allow the game to start

            if (level.StartingLocation.Label == "start")
                return true;
            else
                return false;
        }
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

            player1.MaxJumps = 2;       // 2 enables doublejump
            player1.Max_X_Speed = 12;

            Sprite player1Sprite = new Sprite(player1);

            //-- set up player1 in the physics handler

            Actor PL1 = new Actor(player1, player1Sprite);
            Actors.Add(PL1);
            deviceScreen_.AddCharacter(PL1);

            //-- set up control

            player1.ControlStatus = "Player Controlled";
            player1.Alignment = Character.Team.good;
            PlayerInputChanged = new InputChangedHandler(OnInputChanged);
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

                mob1.SpriteIndex = deviceScreen_.Sprites.Count;
                deviceScreen_.AddCharacter(newMob);

                //-- set up control

                mob1.ControlStatus = "AI controlled monster NPC";
                mob1.Alignment = Character.Team.evil;
                
                //NPCInputChanged = new InputChangedHandler(OnInputChanged);
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

                        deviceScreen_.AddTile(tile, arg);
                    }
                }
            }
        }
    }
}
