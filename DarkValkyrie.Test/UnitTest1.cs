using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Valkyrie.GL;

/*=================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * UpSkilled ICT_CIV_PROG_201810
 * Mobile Project
 * 
 * Test for the .ToXml() functions used to save game
 * 
 * ==============================================*/

namespace DarkValkyrie.Test
{
    [TestClass]
    public class UnitTest1
    {
        //===================================================

        /*----------------------------
         * 
         * Test for Block.ToXml()
         * 
         * ------------------------*/

        [TestMethod]
        public void Block_ToXml()
        {
            Block block = new Block("start", 5, 2);

            string output = block.ToXml();

            Assert.AreEqual(output, "<Block>");
        }

        //===============================================================

        /*------------------------------
         * 
         * Test for Obstacle.ToXml()
         * 
         * ---------------------------*/

        [TestMethod]
        public void Obstacle_ToXml()
        {
            Obstacle o = new Obstacle();

            o.label = "floor1";
            o.Origin = new Block(0, 1);
            o.width = 29;
            o.height = 1;
            o.Initialize();

            string output = o.ToXml();

            Assert.AreEqual("<Obstacle>", output);
        }
    }
}
