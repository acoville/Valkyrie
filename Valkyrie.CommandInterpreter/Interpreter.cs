using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/*====================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * Command Interpreter class
 * 
 * analyzes input from the buttons and evaluates
 * weather the trigger conditions for a special move 
 * have been met 
 * 
 * =================================================*/

namespace Valkyrie.CommandInterpreter
{
    public class Interpreter
    {
        //-- special moves

        private List<Special> _commands;

        public List<Special> Moves
        {
            get
            {
                return _commands;
            }
            set
            {
                _commands = value;
            }
        }

        public int MovesSize
        {
            get
            {
                return _commands.Count;
            }
        }

        //========================================================

        /*-------------------------------------------------
         * 
         * Default Constructor
         *
         * hard-coded values since I can't seem to get 
         * the control profile.xml file to copy to the output
         * directory
         * 
         * -----------------------------------------------*/

        public Interpreter()
        {
            LoadDefaultCommands();

            //-- Special Commands

            //-- fireball

            _commands.Add(new Special("Hadouken", "D, DR, R+B"));
            _commands.Add(new Special("Hadouken", "D, DL, L+B"));

            //-- uppercut

            _commands.Add(new Special("Ryouken", "R, D, DR, R+B"));
            _commands.Add(new Special("Ryouken", "L, D, DL, L+B"));

            //-- dash 

            _commands.Add(new Special("Dash", "R, R"));
            _commands.Add(new Special("Dash", "L, L"));

            //-- dive attack

            _commands.Add(new Special("Dive", "D+B"));
        }

        //========================================================

        /*-------------------------------------------
         * 
         * Load default commands
         * 
         * clears whatever is in the control profile
         * and replaces it with the default set
         * 
         * ----------------------------------------*/

        internal void LoadDefaultCommands()
        {
            _commands = new List<Special>();

            //-- A button

            _commands.Add(new Special("Jump", "A"));

            //-- B button

            _commands.Add(new Special("Attack", "B"));

            //-- D-pad commands

            _commands.Add(new Special("UpRight", "U+R"));
            _commands.Add(new Special("Right", "R"));
            _commands.Add(new Special("DownRight", "D+R"));
            _commands.Add(new Special("Down", "D"));
            _commands.Add(new Special("DownLeft", "D+L"));
            _commands.Add(new Special("Left", "L"));
            _commands.Add(new Special("UpLeft", "U+L"));
            _commands.Add(new Special("Up", "U"));
        }

        //========================================================

        /*--------------------------------------------------
         * 
         * Constructor
         * 
         * This system is extensible, to substitute
         * other commands simply write your own 
         * character profile 
         * 
         * -----------------------------------------------*/

        public Interpreter(string InputFileName)
        {
            LoadDefaultCommands();

            //-- load the data from a profile xml file

            XmlDocument profile = new XmlDocument();
            profile.Load(InputFileName);

            foreach (XmlNode node in profile.DocumentElement.ChildNodes)
                _commands.Add(new Special(node));
        }

        //========================================================

        /*--------------------------------------------------
         * 
         * Constructor
         * 
         *      Overload accepting an XmlDocument
         * 
         * -----------------------------------------------*/

        public Interpreter(ref XmlDocument profile)
        {
            LoadDefaultCommands();

            //-- load the data from a profile xml file

            foreach (XmlNode node in profile.DocumentElement.ChildNodes)
            {
                _commands.Add(new Special(node));
            }
        }

        //=========================================================

        /*-------------------------------------------
         * 
         * If there is a match, return the name of the
         * command. Otherwise, return an empty string.
         * 
         * ----------------------------------------*/

        public bool Interpret(string data, Special output)
        {
            foreach (var command in _commands)
            {
                if (data == command.Input)
                {
                    output = command;
                    return true;
                }
            }

            return false;
        }
    }
}

