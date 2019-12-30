using DarkValkyrie.Graphics;
using System;
using System.ComponentModel;
using Valkyrie.GL;

namespace DarkValkyrie.ViewModel
{
    public partial class GamePageViewModel : INotifyPropertyChanged
    {
        //====================================================================

        /*-----------------------------------------------------
         * 
         * Function to evaluate Movement of a
         * given character, apply all necessary modifications
         * to their speed and state, and translate their
         * sprites on-screen.
         * 
         * --------------------------------------------------*/

        internal void EvaluateMovement()
        {
            foreach (var actor in Actors)
            {
                Sprite sprite = actor.Sprite;

                //-- player / character the camera is tied to

                if (actor.Character.Name == "Erina")
                {
                    Trouble = player1.BlockPosition.ToString();
                    Trouble2 = sprite.SkiaPosition.ToString();

                    level.CurrentLocation = actor.Character.BlockPosition;
                }

                EvaluateVerticalMovement(actor);
                EvaluateLateralMovement(actor);
            }

        }

        //============================================================

        /*--------------------------------------
         * 
         * Evalute vertical movement logic
         * for a given actor
         * 
         * -----------------------------------*/

        internal void EvaluateVerticalMovement(Actor moving_actor)
        {
            Sprite sprite = moving_actor.Sprite;
            Character actor = moving_actor.Character;

            //-- translate the sprite vertically

            if (actor.Falling)
            {
                Gravitate(actor);           // changes acceleration rate
                actor.Accelerate_Y();       // adjust speed by acceleration rate

                int X = sprite.GL_Position.X;
                int Y = sprite.GL_Position.Y;

                //-- if speed is negative we are on the downwards part of the jump arc

                if (actor.ySpeed < 0)
                {
                    bool Landing = level.Blocks[X][Y - 1].IsSolid;

                    /*-------------------------------
                     * 
                     *  Are We About to Land?
                     * YES? 
                     * 
                     * Then stop falling.
                     * 
                     * ---------------------------*/

                    if (Landing)
                    {
                        actor.ySpeed = 0;
                        actor.yAccelerationRate = 0;
                        actor.Falling = false;

                        actor.Standing = true;
                        sprite._Status = Status.standing;

                        deviceScreen_.MoveSprite(sprite, new Block(X, Y));
                    }

                    /*---------------------------
                     * 
                     * NO? then keep falling
                     * 
                     * ------------------------*/

                    else
                    {
                        sprite.Translate(0, (0 - actor.ySpeed) / 2);
                    }
                }

                //====================================================================

                //-- speed is positive, so we are still on the upwards part of the jump arc

                else if (actor.ySpeed > 0)
                {
                    //-- is there a block overhead? 
                    //-- I don't know why, but this will ONLY work at Y + 3.  

                    if (level.Blocks[actor.BlockPosition.X][actor.BlockPosition.Y + 3].IsSolid)
                    {
                        actor.yAccelerationRate = 0;
                        actor.ySpeed = 0;
                    }

                    else
                    {
                        sprite.Translate(0, 0 - actor.ySpeed);
                    }
                }

                //-- speed is 0, so do nothing
            }
        }

        //=========================================================================

        /*-------------------------------------------
         * 
         * Helper Function to subject a given 
         * character to gravity
         * 
         * terminal velocity: -25 pixels / frame
         * decleration rate: -5 pixels / frame
         * 
         * -----------------------------------------*/

        internal void Gravitate(Character actor)
        {
            //-- decelerate vertically to reflect the force of gravity

            if (actor.Falling)
            {
                if (actor.yAccelerationRate > -25.0)
                {
                    actor.yAccelerationRate -= (5.0);
                }
            }
        }

        //=================================================================

        /*-----------------------------
         * 
         * Accelerate helper function 
         * 
         * ----------------------------*/

        internal void LateralAccelerate(Character actor)
        {
            if (Math.Abs(actor.xSpeed) < actor.Max_X_Speed)
            {
                actor.xSpeed += actor.xAccelerationRate;
            }
        }

        //==========================================================================

        /*--------------------------------------
         * 
         * Evalute Lateral movement logic
         * for a given actor
         * 
         * -----------------------------------*/

        //internal void EvaluateLateralMovement(Character actor, Sprite sprite)

        internal void EvaluateLateralMovement(Actor moving_actor)
        {
            Character actor = moving_actor.Character;
            Sprite sprite = moving_actor.Sprite;

            int X = actor.BlockPosition.X;
            int Y = actor.BlockPosition.Y;

            bool Blocked = false;

            //-------------------------------------------------------------------
            //-- accelerate / decelerate checks

            //-- is the playerInput "L" or "R" ? Then accelerate in that direction

            if (Math.Abs(actor.xAccelerationRate) > 0)
                LateralAccelerate(actor);

            //-- otherwise, xAcceleration rate is 0, player is not inputting anything

            else
            {
                double Deceleration_Rate = actor.Max_X_Speed / 3;

                if (Math.Abs(actor.xSpeed) > 0)
                {
                    //-- are we close to 0? then just 0 it out this pass

                    if (Math.Abs(actor.xSpeed) < Deceleration_Rate)
                    {
                        actor.xSpeed = 0;
                    }

                    //-- otherwise decelerate

                    else
                    {
                        if (actor.xSpeed > 0)
                            actor.xSpeed -= Deceleration_Rate;
                        else
                            actor.xSpeed += Deceleration_Rate;
                    }
                }
            }

            //----------------------------------------------------------------
            //--- if xSpeed > 0 we are going right

            if (actor.xSpeed > 0)
            {
                //-- bounds check 

                if (X + 1 == level.HighX)
                {
                    Blocked = true;
                }

                //-- obstacle check 

                else
                {
                    if (level.Blocks[X + 1][Y].IsSolid)
                    {
                        Blocked = true;
                    }
                }
            }

            //----------------------------------------------------------------
            //--- if xSpeed < 0 we are going left

            else if (actor.xSpeed < 0)
            {
                //-- bounds check 

                if (X - 1 == 0)
                {
                    Blocked = true;
                }

                //-- obstacle check 

                else
                {
                    if (level.Blocks[X - 1][Y].IsSolid)
                    {
                        Blocked = true;
                    }
                }
            }

            //----------------------------------------------------------------

            if (Blocked)
            {
                Trouble3 = Blocked.ToString();


                actor.xSpeed = 0;
                actor.xAccelerationRate = 0;
            }
            else
            {
                sprite.Translate(actor.xSpeed, 0);
            }

            //----------------------------------------------------------------

            //-- evaluate weather we stepped off a ledge
            //-- needs a bounds check 

            if (Y - 1 <= level.LowY)
            {
                // this is where the player falls to their deaths.

                if (actor == player1)
                {
                    ResetPlayer1();
                }
                else
                {
                    //Actors.Remove(actor);
                }
            }
            else
            {
                if (!level.Blocks[X][Y - 1].IsSolid)
                {
                    actor.Falling = true;
                    sprite._Status = Status.falling;
                }
            }
        }

    }
}
