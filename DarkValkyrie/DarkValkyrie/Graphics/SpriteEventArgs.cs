using System;
using System.Collections.Generic;
using System.Text;

/*============================================================
 * 
 *  Adam Coville
 *  adam.coville@gmail.com
 *  Upsilled ICT_CIV_PROG_201810 
 *  Mobile Project
 * 
 *  Custom event args for handling screen orientation changes
 * 
 * ==========================================================*/

namespace DarkValkyrie.Graphics
{
    public class SpriteEventArgs : EventArgs
    {
        //==================================================================

        public SpriteEventArgs(object sender, Status status, Facing facing)
        {
            var Sprite = sender as Sprite;

            OldStatus = Sprite.status;
            OldFacing = Sprite.facing;

            NewStatus = status;
            NewFacing = facing;
        }

        //==================================================================

        public Status NewStatus { get; set; }
        public Status OldStatus { get; set; }
        public Facing OldFacing { get; set; }
        public Facing NewFacing { get; set; }
    }
}
