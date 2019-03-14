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
 * Special Ability Class
 * 
 * These are what the Input Command Interpreter evaluates
 * input stream data against to determine weather to initiate
 * the command.
 * 
 * ================================================*/

namespace Valkyrie.CommandInterpreter
{
    public class Special
    {
        public string Name { get; set; }
        public string Input { get; set; }
        public bool Enabled { get; set; }

        //=====================================================

        /*-----------------------------
         * 
         * Default Constructor
         * 
         * ---------------------------*/

        public Special()
        {
            Name = string.Empty;
            Enabled = false;
            Input = string.Empty;
        }

        //=====================================================

        /*-----------------------------
         * 
         * Parameterized Constructor
         * 
         * --------------------------*/

        public Special(string N, string I)
        {
            Name = N;
            Input = I;
            Enabled = true;
        }

        //======================================================

        /*----------------------------
         * 
         * Xml Constructor
         * 
         * -------------------------*/

        public Special(XmlNode node)
        {
            Name = node.Attributes["Title"].Value;
            Input = node.Attributes["Input"].Value;
            Enabled = true;
        }
    }
}
