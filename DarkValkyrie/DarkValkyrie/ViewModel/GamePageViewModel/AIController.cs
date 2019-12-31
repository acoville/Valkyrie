using System;
using System.Collections.Generic;
using System.Text;
using Valkyrie.GL;

namespace DarkValkyrie.ViewModel
{
    public class AIController
    {
        //======================================================================

        /*-----------------------------------
         * 
         *  Constructor
         * 
         * --------------------------------*/

        public AIController()
        {
        }

        //======================================================================

        /*-----------------------------------
         * 
         *  
         * 
         * ---------------------------------*/

        public void ControlCharacter(Character npc, ref List<Character> actors)
        {
            // are there hostiles in area? If yes, 

            if(HostilesInRange(npc, ref actors))
            {
                // identify nearest hostile (threat assessment)

                Character target = IdentifyNearestHostile(npc, ref actors);
                
                // if within range, attack (combat)

                

                // if not within attack range, close to range (motion planning)

            }

            // if no, do random patrol 


        }

        //=========================================================================

        internal bool HostilesInRange(Character npc, ref List<Character> actors)
        {
            Character.Team Team1 = npc.Alignment;
            Character.Team Team2;

            if (Team1 == Character.Team.good)
            {
                Team2 = Character.Team.evil;
            }
            else
            {
                Team2 = Character.Team.good;
            }

            for (int i = 0; i < actors.Count; i++)
            {
                //-- scan through the list of actors on a hostile team, 
                // if there are any, then select the nearest one. 

                if (actors[i].Alignment == Team2)
                {
                    Block P1 = npc.BlockPosition;
                    Block P2 = actors[i].BlockPosition;
                    int distance = P1.DistanceTo(P2);

                    if (distance < npc.DetectionRange)
                    {
                        //-- are there obstacles in the way? 
                        //-- is the player spectral / invisbile?

                        return true;
                    }
                }
            }

            return false;
        }

        //=========================================================================

        /*-------------------------------------------
         * 
         *  Helper function that identifies the
         *  nearest hostile to this NPC
         * 
         * ---------------------------------------*/

        internal Character IdentifyNearestHostile(Character npc, ref List<Character> actors)
        {
            Character nearest_hostile = new Character("Blank");
            int nearest = 500;          // start with a high value

            Character.Team Team1 = npc.Alignment;
            Character.Team Team2;

            if(Team1 == Character.Team.good)
            {
                Team2 = Character.Team.evil; 
            }
            else
            {
                Team2 = Character.Team.good;
            }

            for(int i = 0; i < actors.Count; i++)
            {
                //-- scan through the list of actors on a hostile team, 
                // if there are any, then select the nearest one. 

                if(actors[i].Alignment == Team2)
                {
                    Block P1 = npc.BlockPosition;
                    Block P2 = actors[i].BlockPosition;
                    int distance = P1.DistanceTo(P2);

                    if(distance < nearest)
                    {
                        nearest = distance;
                        nearest_hostile = actors[i];
                    }
                }

                //-- otherwise, engage in patrol
            }

            return nearest_hostile;
        }
    }
}
