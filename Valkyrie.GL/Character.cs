using System;
using System.Xml;

/*=========================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * Character class
 * 
 * ======================================================*/

namespace Valkyrie.GL
{
    
    public class Character : Region
    {
        public enum Team { good, evil };

        public string Name { get; set; }

        public Block BlockPosition { get; set; }

        //=============================================================

        /*-------------------------------------
         * 
         *  Motion Properties
         * 
         * -----------------------------------*/

        public double xSpeed { get; set; }              // + for right, - for left
        public double ySpeed { get; set; }              // + for up, - for down

        public double xAccelerationRate { get; set; }   // in pixels / frame
        public double yAccelerationRate { get; set; }   // in pixels / frame

        public int CurrentJumps { get; set; }
        public int MaxJumps { get; set; }
        public Region CollisionZone { get; set; }

        //======================================================

        public void Accelerate_Y()
        {
            ySpeed += yAccelerationRate;
        }

        public void Accelerate_X()
        {
            xSpeed += xAccelerationRate;
        }

        //======================================================

        internal bool falling = false;
        public bool Falling
        {
            get
            {
                return falling;
            }
            set
            {
                falling = value;

                //------ if falling is true

                if (value == true)
                {
                    standing = false;
                }

                //------ if falling is false

                else
                {
                    CurrentJumps = 0;
                    ySpeed = 0;
                    yAccelerationRate = 0;
                }
            }
        }

        //======================================================

        internal bool standing = true;
        public bool Standing
        {
            get
            {
                return standing;
            }
            set
            {
                standing = value;

                if (value == true)
                    falling = false;
            }
        }

        //===========================================================

        //-- Control variables

        public String ControlStatus { get; set; }
        public Team Alignment { get; set; }

        //===========================================================

        //-- Combat variables

        public int DetectionRange { get; set; }

        public int HP { get; private set; }
        public int maxHP { get; set; }

        //-- display variables

        public string SpriteSource { get; set; }
        public int SpriteIndex { get; set; }

        //===========================================================

        /*------------------------------------
         * 
         * Constructor:
         * 
         * character name as a paramter
         *
         * ---------------------------------*/

        public Character(string L)
        {
            Name = L;
            HP = 100;
            maxHP = 100;

            xSpeed = 0;
            ySpeed = 0;
            xAccelerationRate = 0;
            yAccelerationRate = 0;
            BlockPosition = new Block(0, 0);
            CurrentJumps = 0;
            MaxJumps = 1;

            //InitializeCollisionZone();
        }

        //============================================================

        /*-------------------------------------
         * 
         * Copy Constructor
         * 
         * used during GamePageViewModel.AddMonsters()
         * 
         * ----------------------------------*/

        public Character(Character orig)
        {
            Name = orig.Name;
            HP = orig.HP;
            maxHP = orig.HP;

            xSpeed = 0;
            ySpeed = 0;
            xAccelerationRate = 0;
            yAccelerationRate = 0;

            BlockPosition = orig.BlockPosition;

            CurrentJumps = 0;
            MaxJumps = 1;
        }

        //============================================================

        /*---------------------------------------------
         * 
         * Constructor accepting a Block
         * as a starting point 
         * 
         * Damned if I know why but this constructor
         * always was adding X + 1 and Y - 1.
         * 
         * For the life of me, I do not understand.
         * 
         * ------------------------------------------*/

        public Character(string L, int X, int Y)
        {
            Name = L;
            HP = 100;
            maxHP = 100;

            xSpeed = 0;
            ySpeed = 0;
            xAccelerationRate = 0;
            yAccelerationRate = 0;

            BlockPosition = new Block(X, Y);

            CurrentJumps = 0;
            MaxJumps = 1;
        }

        //=======================================================

        /*-----------------------------------
         * 
         * Constructor accepting an XmlNode
         * 
         * --------------------------------*/

        public Character(XmlNode node)
        {
            Name = node.Attributes["Name"].Value;
            SpriteSource = node.Attributes["SpriteSource"].Value;

            //-- position 

            int PosX = int.Parse(node.Attributes["X"].Value);
            int PosY = int.Parse(node.Attributes["Y"].Value);

            BlockPosition = new Block(PosX, PosY);

            //-- combat 

            DetectionRange = Int32.Parse(node.Attributes["DetectionRange"].Value);
            maxHP = Int32.Parse(node.Attributes["HP"].Value);
            HP = maxHP;

            //-- movement (optional fields)

            xSpeed = 0;
            ySpeed = 0;
            xAccelerationRate = 0;
            yAccelerationRate = 0;
            Max_X_Speed = int.Parse(node.Attributes["Speed"].Value);

            CurrentJumps = 0;
            MaxJumps = 1;

            //InitializeCollisionZone();
        }

        //=================================================================

        /*------------------------------------
         * 
         * Player's maximum lateral speed
         * 
         * ----------------------------------*/

        internal int max_X_Speed;
        public int Max_X_Speed
        {
            get
            {
                return max_X_Speed;
            }
            set
            {
                max_X_Speed = value;
            }
        }

        //===========================================================

        /*------------------------------------
         * 
         * function to check distance from 
         * a given line on the X coordinate
         * 
         * ----------------------------------*/

        public int HorizontalDistanceTo(int x)
        {
            return Math.Abs(BlockPosition.X - x);
        }

        //===========================================================

        /*------------------------------------
         * 
         * function to check distance from 
         * a given line on the X coordinate
         * 
         * ---------------------------------*/

        public int VerticalDistanceTo(int y)
        {
            return Math.Abs(BlockPosition.X - y);
        }

        //===========================================================

        /*-----------------------------------
         * 
         * function to check weather
         * a player is within x blocks of
         * another block
         * 
         * ---------------------------------*/

        public bool Distance(Block arg, int d)
        {
            bool result = BlockPosition.DistanceTo(arg) <= d ? true : false;

            return result;
        }

        //===========================================================

        /*-------------------------------------
         * 
         * Translate function
         * 
         * moves the block position, at the 
         * gamelogic level, I am unconcerned
         * about what is happening with the 
         * pixels on-screen.
         * 
         * ---------------------------------*/

        public void BlockTranslate(int deltaX, int deltaY)
        {
            BlockPosition.X += deltaX;
            BlockPosition.Y += deltaY;

            //-- now update the collision zone

            CollisionZone.Translate(deltaX, deltaY);
        }

        //==========================================================

        /*----------------------------
         * 
         * Damage Function
         * 
         * -------------------------*/

        public void Damage(int damage)
        {
            if (HP - damage < 0)
                HP = 0;
            else
                HP -= damage;
        }

        //==========================================================

        /*----------------------------
         * 
         * Heal Function 
         * 
         * --------------------------*/

        public void Heal(int healing)
        {
            if (HP + healing > maxHP)
                HP = maxHP;
            else
                HP += healing;
        }
    }
}
