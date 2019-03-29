using DarkValkyrie.ViewModel;
using System;
using System.IO;
using System.Xml;
using Valkyrie.GL;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 *  Upskilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * The Menu Page is where the user starts or continues a game
 * and navigates to the options menu
 * 
 * ==========================================================*/

namespace DarkValkyrie.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        MenuPageViewModel vms;

        OptionsPage _options;

        internal GamePage currentGame;

        //==================================================

        /*--------------------------------------
         * 
         *  Constructor
         * 
         * -----------------------------------*/

        public MenuPage()
        {
            InitializeComponent();

            vms = new MenuPageViewModel();
            BindingContext = vms;
            BackgroundImage = vms.GetImageSource();
        }

        //==================================================

        /*---------------------------------
         * 
         * Event to update the background image
         * if the device orientation changes
         * 
         * -------------------------------*/

        protected override void OnSizeAllocated(double width, double height)
        {
            vms.DeviceScreen.GetScreenDetails();

            BackgroundImage = vms.GetImageSource();

            vms.ButtonHeight = (int)vms.DeviceScreen.Height / 4;

            base.OnSizeAllocated(width, height);
        }

        //========================================================

        /*--------------------------
         * 
         * New Game
         * 
         * -----------------------*/

        private void Newgame_Clicked(object sender, EventArgs e)
        {
            currentGame = new GamePage();
            Navigation.PushAsync(currentGame);

            //-- enable the other buttons

            Save_Btn.IsEnabled = true;
            Continue_Btn.IsEnabled = true;
            Options_Btn.IsEnabled = true;

            _options = new OptionsPage(currentGame.gpvm, currentGame);
        }

        //===========================================================

        /*---------------------------------------------
         * 
         * Continue Game
         * 
         * resumes the paused game that's already
         * running.
         * 
         * ------------------------------------------*/

        private void Continue_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(currentGame);
        }

        //===========================================================

        /*--------------------------
         * 
         * Options Menu
         * 
         * -----------------------*/

        private void Options_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(_options);
        }
        
        //==============================================================================

        /*--------------------------------
         * 
         * Save Game
         * 
         * -----------------------------*/

        private void Save_Clicked(object sender, EventArgs e)
        {
            XmlDocument SaveState = SetupDocument();

            SaveObstacles(SaveState);
            SaveMonsters(SaveState);
            SaveGravity(SaveState);
            SaveBackground(SaveState);
            SaveMeta(SaveState);
            SaveCurrent(SaveState);

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "save.xml");
            
            SaveState.Save(fileName);

            DisplayAlert("", "Game Saved Successfully.", "OK");

            if (!vms.SavedStateExists)
            {
                vms.SavedStateExists = true;
            }

            if(Load_Btn.IsEnabled == false)
            {
                Load_Btn.IsEnabled = true;
            }
        }

        //============================================================================

        /*---------------------------------
         * 
         * Setup the XmlDoucment and add 
         * its Meta element
         * 
         * -------------------------------*/

        internal void SaveMeta(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            int H = currentGame.gpvm.level.height;
            int W = currentGame.gpvm.level.width;

            XmlElement Meta = SaveState.CreateElement("Meta");
            Meta.SetAttribute("Height", H.ToString());
            Meta.SetAttribute("Width", W.ToString());

            root.AppendChild(Meta);
        }

        //============================================================================

        /*---------------------------------
         * 
         * Setup the XmlDoucment and add 
         * its Current position element
         * 
         * -------------------------------*/

        internal void SaveCurrent(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            int X = currentGame.gpvm.level.CurrentLocation.X;
            int Y = currentGame.gpvm.level.CurrentLocation.Y;

            XmlElement Current = SaveState.CreateElement("Block");
            Current.SetAttribute("Label", "current");
            Current.SetAttribute("X", X.ToString());
            Current.SetAttribute("Y", Y.ToString());

            root.AppendChild(Current);
        }
        
        //============================================================================

        /*---------------------------------
         * 
         * Setup the XmlDoucment and add 
         * its root element
         * 
         * -------------------------------*/

        internal void SaveGravity(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            double rate = currentGame.gpvm.level.Gravity;

            XmlElement Gravity = SaveState.CreateElement("Gravity");
            Gravity.SetAttribute("Rate", rate.ToString());

            root.AppendChild(Gravity);
        }

        //============================================================================

        /*---------------------------------
         * 
         * Setup the XmlDoucment and add 
         * Background Image element
         * 
         * -------------------------------*/

        internal void SaveBackground(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            string Source = currentGame.gpvm.BackgroundImage;

            XmlElement Background = SaveState.CreateElement("Background");
            Background.SetAttribute("Image", Source);

            root.AppendChild(Background);
        }

        //============================================================================

        /*---------------------------------
         * 
         * Setup the XmlDoucment and add 
         * its root element
         * 
         * -------------------------------*/

        internal XmlDocument SetupDocument()
        {
            XmlDocument SaveState = new XmlDocument();

            Block start = currentGame.gpvm.player1.BlockPosition;
            start.Label = "start";

            //-- set up the root of the document

            XmlDeclaration declaration = SaveState.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootNode = SaveState.CreateElement("Level");
            SaveState.InsertBefore(declaration, SaveState.DocumentElement);
            SaveState.AppendChild(rootNode);

            //-- set up the starting location element

            XmlElement level_start = SaveState.CreateElement("Block");
            level_start.SetAttribute("Label", start.Label);
            level_start.SetAttribute("X", start.X.ToString());
            level_start.SetAttribute("Y", start.Y.ToString());

            XmlNode root = SaveState.DocumentElement;

            root.AppendChild(level_start);

            return SaveState;
        }

        //============================================================================

        /*---------------------------------
         * 
         * Copy Obstacles
         * 
         * -------------------------------*/

        internal void SaveObstacles(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            XmlElement staticFeatures = SaveState.CreateElement("StaticFeatures");
            root.AppendChild(staticFeatures);

            Level level = currentGame.gpvm.level;
            
            foreach(var obstacle in level.StaticObstacles)
            {
                XmlElement ObstacleXML = SaveState.CreateElement("Obstacle");
                ObstacleXML.SetAttribute("Label", obstacle.label);

                //-- image source needs the Directory . notation stripped

                string path = "DarkValkyrie.Graphics.Tiles.";
                int index = path.Length;

                string skin = obstacle.ImageSource.Substring(index);
                
                ObstacleXML.SetAttribute("Skin", skin);

                //-- X1 

                int x1 = obstacle.LowX;
                ObstacleXML.SetAttribute("X1", obstacle.LowX.ToString());

                //-- X2

                int x2 = 0;

                if(obstacle.width == 1)
                {
                    x2 = x1;
                }
                else
                {
                    x2 = x1 + obstacle.width - 1;
                }
                
                ObstacleXML.SetAttribute("X2", x2.ToString());

                //-- Y1 

                int y1 = obstacle.LowY;
                ObstacleXML.SetAttribute("Y1", y1.ToString());

                //-- Y2

                int y2 = 0;

                if(obstacle.height == 1)
                {
                    y2 = y1;
                }
                else
                {
                    y2 = y1 + obstacle.height - 1;
                }

                ObstacleXML.SetAttribute("Y2", y2.ToString());
                
                //--- append

                staticFeatures.AppendChild(ObstacleXML);
            }
        }
        
        //============================================================================

        /*---------------------------------
         * 
         * Copy Actors into the Monsters
         * 
         * this will ignore dead monsters
         * 
         * -------------------------------*/

        internal void SaveMonsters(XmlDocument SaveState)
        {
            XmlNode root = SaveState.DocumentElement;

            XmlElement Monsters = SaveState.CreateElement("Monsters");
            root.AppendChild(Monsters);

            foreach(var actor in currentGame.gpvm.Actors)
            {
                //-- ignore the player character

                if(actor.Name != "Erina")
                {
                    XmlElement actorXML = SaveState.CreateElement("Monster");
                    actorXML.SetAttribute("Name", actor.Name);
                    actorXML.SetAttribute("SpriteSource", actor.SpriteSource);
                    actorXML.SetAttribute("X", actor.BlockPosition.X.ToString());
                    actorXML.SetAttribute("Y", actor.BlockPosition.Y.ToString());
                    actorXML.SetAttribute("HP", actor.maxHP.ToString());
                    actorXML.SetAttribute("Speed", actor.Max_X_Speed.ToString());

                    Monsters.AppendChild(actorXML);
                }
            }
        }

        //============================================================================

        /*--------------------------------
         * 
         * Load Game
         * 
         * -----------------------------*/

        private void Load_Clicked(object sender, EventArgs e)
        {
            //-------------------- start the game

            currentGame = new GamePage(true);
            
            Navigation.PushAsync(currentGame);

            //-- enable the other buttons

            Save_Btn.IsEnabled = true;
            Continue_Btn.IsEnabled = true;
            Options_Btn.IsEnabled = true;

            _options = new OptionsPage(currentGame.gpvm, currentGame);
        }
    }
}