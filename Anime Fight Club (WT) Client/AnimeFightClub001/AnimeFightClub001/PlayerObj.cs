//******************************************************
// File: Player.cs
//
// Purpose: Defines a player object based on a basic
// object. This player object is controlled by
// the player, either directly via a gamepad or
// over the network via instructions sent by
// the server
//
// Written By: Salvatore Hanusiewicz, Rob Maggio
//******************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;

namespace AnimeFightClub001
{
    enum PlayerState { Standing, Running, Jumping, DoubleJumping, Jumped, Sliding, Walking, LedgeGrabbing, Slashing, SlashingCharge, SlashingHoldUp, SlashingHoldDown, SlashingRelease, Stabbing, StabbingCharge, StabbingHoldIn, StabbingHoldOut, StabbingRelease, ShootingForward, ShootingForwardHoldOut, ShootingForwardHoldIn, ShootingUp, ShootingDown, ShootingForwardUp, ShootingForwardUpHoldOut, ShootingForwardUpHoldIn, ShootingForwardDown, OpenPalmForward, OpenPalmForwardCharge, OpenPlamForwardHoldIn, OpenPlamForwardHoldOut, OpenPalmForwardRelease, OpenPalmUp, OpenPalmDown, OpenPalmForwardUp, OpenPalmForwardDown, ShootingForwardWalking, ShootingUpWalking, ShootingDownWalking, ShootingForwardUpWalking, ShootingForwardDownWalking, OpenPalmForwardWalking, ShootingForwardRunning, ShootingUpRunning, ShootingDownRunning, ShootingForwardUpRunning, ShootingForwardDownRunning, OpenPalmForwardRunning }
    enum PlayerAction { None, Slashing, SlashingCharge, SlashingHoldUp, SlashingHoldDown, SlashingRelease, Stabbing, StabbingCharge, StabbingHoldIn, StabbingHoldOut, StabbingRelease, ShootingForward, ShootingForwardHoldOut, ShootingForwardHoldIn, ShootingUp, ShootingDown, ShootingForwardUp, ShootingForwardUpHoldOut, ShootingForwardUpHoldIn, ShootingForwardDown, OpenPalmForward, OpenPalmForwardCharge, OpenPlamForwardHoldIn, OpenPlamForwardHoldOut, OpenPalmForwardRelease, OpenPalmUp, OpenPalmDown, OpenPalmForwardUp, OpenPalmForwardDown }
    enum StatusName { Damage, Stun, Poison, Burn, Slow, Death, ShieldAbility, KnockBack }
    class PlayerObj : BasicObj
    {
        #region Declarations
        protected long m_playerID; //Used to identify the player (in multiplayer games this is the RemoteUniqueIdentifier)
        protected string m_userName; //Stores the user's name
        protected Controller m_ctrl; //Used to store the abstracted controller
        protected int m_jumpCounter; //Used to count frames, limiting jump height.
        protected int m_grabCounter; //Prevents a new ledgegrab immediately after releasing one.
        protected int m_hp;
        protected int m_mp;
        protected int m_attack;
        protected int m_defense;
        protected bool m_canDoubleJump;
        protected bool m_oldJump; //Determines whether the current jump keypress is for a new jump or the continuation of one.
        protected bool m_chainingAllowed;

        protected StatusHandler m_statusHandler;


        protected CharacterLoadout m_loadout; //This stores all the abilities that the player is using

        protected int m_landedTimer; //Determines how much longer the player should be "landed stunned"
        protected int m_slideTimer; //Used to count how many miliseconds the player has been sliding

        protected PlayerAction m_action; //Stores the player action (isn't stored as an int because only the player has this)

        protected int m_kills; //Stores the number of kills a player has

        protected Vector2 m_ledgeGrabPos; //position the player locks to during ledge grab

        #endregion

        #region Constants
        const int WALKSPEED = 280;

        const int RUNSPEED = 360;

        const int AIRACCEL = 1200;
        const int WALLJUMPXVEL = 300;
        const int WALLJUMPYVEL = 650;
        const int JUMPLENGTHMS = 166;
        const int JUMPMINVEL = 100;
        const int JUMPACCEL = 3800;
        const int LEDGEJUMPVEL = 450;
        const float RUNMULTIPLIER = 2; //unused
        const int CLIMBDELAYMS = 200; //unused

        const int SLIDE_DURATION = 400;
        const float SLIDE_VEL = 450;

        public const float NORM_PHYS_WIDTH = 40; //Normal width of the player's collision box
        public const float NORM_PHYS_HEIGHT = 100; //Normal height of the player's collision box

        public const float SLIDING_PHYS_WIDTH = 100; //Sliding width of the player's collision box
        public const float SLIDING_PHYS_HEIGHT = 60; //Sliding height of the player's collision box
        #endregion


        //****************************************************
        // Method: NetInitialize
        //
        // Purpose: Initializes values based on the 
        // mesage sent by the server
        //****************************************************
        public override void NetInitialize(NetIncomingMessage msg)
        {
            base.NetInitialize(msg);
            m_playerID = msg.ReadInt64();
            m_userName = msg.ReadString();

            HP = msg.ReadInt32();
            MP = msg.ReadInt32();

            m_ledgeGrabPos.X = msg.ReadFloat();
            m_ledgeGrabPos.Y = msg.ReadFloat();

            m_loadout.NetInitialize(msg);

            m_statusHandler.NetInitialize(msg);
        }

        //****************************************************
        // Method: PlayerObj
        //
        // Purpose: Default Constructor for PlayerObj
        //****************************************************
        public PlayerObj()
        {
            m_drawableObj = new Sprite("NaniNone", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Player));

            m_attack = GlobalVariables.Settings.startingAttack;
            m_defense = GlobalVariables.Settings.startingDefense;
            m_hp = GlobalVariables.Settings.startingHealth;

            m_ctrl = new AnimeFightClub001.Controller(PlayerIndex.One, GlobalVariables.Settings.SelControllerType[PlayerIndex.One]);
            m_ctrl.Networked = true;
            m_physicsObj.Rect.SetDimensions(NORM_PHYS_WIDTH, NORM_PHYS_HEIGHT);

            m_chainingAllowed = true;
            m_physicsObj.Mass = 1;
            m_physicsObj.Restitution = 0f;

            m_direction = Direction.Left;
            m_state = (int)PlayerState.Standing;
            m_action = PlayerAction.None;
            m_landedTimer = 0;
            m_slideTimer = 0;

            m_kills = 0;

            m_ledgeGrabPos = Vector2.Zero;

            m_loadout = new CharacterLoadout(this);

            m_statusHandler = new StatusHandler(this);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the player
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            m_statusHandler.Update(gameTime);
            if (!m_statusHandler.HasStatus(StatusName.Death))
            {
                if (m_statusHandler.HasStatus(StatusName.Stun))
                {
                    for(int i = 0; i < Controller.ControllerState.ControlArray.Length; ++i)
                    {
                        Controller.ControllerState.ControlArray[i] = false;
                    }
                }
                HandleControlInput(gameTime);

                UpdateAbilities(gameTime);
                UpdateDrawable(gameTime);

                m_mp += gameTime.ElapsedGameTime.Milliseconds/2;
                if (m_mp > 10000)
                {
                    m_mp = 10000;
                }

                PhysicsObj.Update(gameTime, viewport);

                m_prevState = m_state;
            }
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the object any anything else
        // that it may need to draw.
        //****************************************************
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!m_statusHandler.HasStatus(StatusName.Death))
            {
                base.Draw(spriteBatch);

                #region Colored Name
                Vector2 tempStringMeasure = GlobalVariables.FontDict["Arial10"].MeasureString(m_userName);
                int red;
                int green;
                if (HP > (float)GlobalVariables.Settings.startingHealth / 2)
                {
                    green = 255;
                    red = (int)(510 * (1 - (HP / (float)GlobalVariables.Settings.startingHealth)));
                }

                else
                {
                    red = 255;
                    green = (int)(400 * (HP / (float)GlobalVariables.Settings.startingHealth));
                }

                Color tempColor = new Color(red, green, 0);

                spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], m_userName, new Vector2(Center.X - tempStringMeasure.X / 2, Pos.Y - tempStringMeasure.Y - 3), tempColor);
                #endregion

                m_statusHandler.Draw(spriteBatch);

                foreach (KeyValuePair<string, Ability> ability in m_loadout.AbilityDict)
                {
                    ability.Value.Draw(spriteBatch);
                }

            }
        }

        //****************************************************
        // Method: UpdateAbilities
        //
        // Purpose: Updates all the abilities that the player
        // has in their loadout.
        //****************************************************
        public virtual void UpdateAbilities(GameTime gameTime)
        {
            foreach (KeyValuePair<string, Ability> ability in m_loadout.AbilityDict)
            {
                ability.Value.Update(gameTime);
            }
        }

        //****************************************************
        // Method: UpdateDrawable
        //
        // Purpose: Updates the details of the objects
        // drawbleObj
        //****************************************************
        public override void UpdateDrawable(GameTime gameTime)
        {
            m_drawableObj.Update(gameTime);
        }

        //****************************************************
        // Method: HandleControlInput
        //
        // Purpose: Modifies the player based on the 
        // player's input.  Incomplete.
        //****************************************************
        public void HandleControlInput(GameTime gameTime)
        {
            if ((PlayerState)m_state == PlayerState.LedgeGrabbing)
            {
                m_jumpCounter = 0;
                m_landedTimer = 0;
                m_oldJump = false;
                m_canDoubleJump = true;
                m_grabCounter = 100;

                m_physicsObj.VelX = 0;
                m_physicsObj.VelY = 0;
                m_physicsObj.AccelX = 0;
                m_physicsObj.AccelY = 0;

                m_physicsObj.PosX = m_ledgeGrabPos.X;
                m_physicsObj.PosY = m_ledgeGrabPos.Y;

                if (ControllerState.isControlPressed(Control.Jump))
                {
                    m_oldJump = true;
                    m_physicsObj.VelY = -LEDGEJUMPVEL;
                    m_physicsObj.VelX = 0;
                    m_jumpCounter = 0; //The jump counter measures the duration of the jump.

                    m_state = ObtainPlayerState(PlayerState.Jumping, m_action);
                }
                else if (ControllerState.isControlPressed(Control.Crouch))
                {
                    m_state = ObtainPlayerState(PlayerState.Standing, m_action);
                }
            }
            else
            {
                if (m_grabCounter > 0)
                {
                    m_grabCounter -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if ((PlayerState)m_state != PlayerState.Sliding)
                {
                    #region Abilities
                    #region Press
                    if (!m_loadout.Casting && (ControllerState.isControlPressed(Control.MainHand) ^ ControllerState.isControlPressed(Control.OffHand) ^ ControllerState.isControlPressed(Control.AbilityOne) ^ ControllerState.isControlPressed(Control.AbilityTwo) ^ ControllerState.isControlPressed(Control.AbilityThree)))
                    {
                        if (ControllerState.isControlPressed(Control.MainHand) && !Controller.PrevControllerState.isControlPressed(Control.MainHand))
                            m_loadout.MainHand.PressedAbility();

                        if (ControllerState.isControlPressed(Control.OffHand) && !Controller.PrevControllerState.isControlPressed(Control.OffHand))
                            m_loadout.OffHand.PressedAbility();

                        if (ControllerState.isControlPressed(Control.AbilityOne) && !Controller.PrevControllerState.isControlPressed(Control.AbilityOne))
                            m_loadout.Special1.PressedAbility();

                        if (ControllerState.isControlPressed(Control.AbilityTwo) && !Controller.PrevControllerState.isControlPressed(Control.AbilityTwo))
                            m_loadout.Special2.PressedAbility();

                        if (ControllerState.isControlPressed(Control.AbilityThree) && !Controller.PrevControllerState.isControlPressed(Control.AbilityThree))
                            m_loadout.Special3.PressedAbility();
                    }
                    #endregion

                    #region Release
                    if (!ControllerState.isControlPressed(Control.MainHand) && Controller.PrevControllerState.isControlPressed(Control.MainHand))
                        m_loadout.MainHand.ReleasedAbility();

                    if (!ControllerState.isControlPressed(Control.OffHand) && Controller.PrevControllerState.isControlPressed(Control.OffHand))
                        m_loadout.OffHand.ReleasedAbility();

                    if (!ControllerState.isControlPressed(Control.AbilityOne) && Controller.PrevControllerState.isControlPressed(Control.AbilityOne))
                        m_loadout.Special1.ReleasedAbility();

                    if (!ControllerState.isControlPressed(Control.AbilityTwo) && Controller.PrevControllerState.isControlPressed(Control.AbilityTwo))
                        m_loadout.Special2.ReleasedAbility();

                    if (!ControllerState.isControlPressed(Control.AbilityThree) && Controller.PrevControllerState.isControlPressed(Control.AbilityThree))
                        m_loadout.Special3.ReleasedAbility();
                    #endregion
                    #endregion
                }

                #region Movement
                //All ground movement.  Friction is applied, and movement is velocity-based, offering more precision.
                if (m_physicsObj.Sided[Side.bottom])
                {
                    #region On Ground
                    m_physicsObj.AccelX = 0;
                    m_canDoubleJump = true;
                    m_physicsObj.VelX *= .5f;  //Not the most realistic friction, but it works.

                    #region Update Sliding
                    if ((PlayerState)m_state == PlayerState.Sliding)
                    {
                        m_slideTimer -= gameTime.ElapsedGameTime.Milliseconds;


                        if (m_direction == AnimeFightClub001.Direction.Left)
                        {
                            m_physicsObj.VelX = -SLIDE_VEL;
                        }
                        else
                        {
                            m_physicsObj.VelX = SLIDE_VEL;
                        }

                        if (m_slideTimer <= 0)
                        {
                            //Doesn't seem to do it anymore (maybe it's different from original?)
                            #region GLITCHY VERSION TO LOOK INTO
                            /*
                        m_state = (int)PlayerState.Running;
                        m_physicsObj.Rect.Width = NORM_PHYS_WIDTH;
                        m_physicsObj.Rect.Height = NORM_PHYS_HEIGHT;
                        m_physicsObj.PosY = m_physicsObj.PosY - m_physicsObj.Rect.Height + SLIDING_PHYS_HEIGHT; 
                        */
                            #endregion

                            //Comment out to test glitch version
                            float tempBottom = m_physicsObj.Rect.Bottom;
                            m_state = ObtainPlayerState(PlayerState.Running, m_action);
                            m_physicsObj.Rect.Width = NORM_PHYS_WIDTH;
                            m_physicsObj.Rect.Height = NORM_PHYS_HEIGHT;
                            m_physicsObj.PosY = tempBottom - NORM_PHYS_HEIGHT;
                            //^^^Comment out to test glitch version^^^
                        }
                    }
                    #endregion

                    #region Begin Sliding
                    if ((PlayerState)m_state == PlayerState.Running && ControllerState.isControlPressed(Control.Crouch) && !PrevControllerState.isControlPressed(Control.Crouch))
                    {
                        //Doesn't seem to do it anymore (maybe it's different from original?)
                        #region GLITCHY VERSION TO LOOK INTO
                        /*
                    m_state = (int)PlayerState.Sliding;
                    m_slideTimer = SLIDE_DURATION;
                    m_physicsObj.Rect.Width = SLIDING_PHYS_WIDTH;
                    //m_physicsObj.PosY =  m_physicsObj.PosY + m_physicsObj.Rect.Height - SLIDING_PHYS_HEIGHT; //This one "worked"
                    m_physicsObj.PosY =  m_physicsObj.PosY + NORM_PHYS_HEIGHT - SLIDING_PHYS_HEIGHT; //This one bugged
                    m_physicsObj.Rect.Height = SLIDING_PHYS_HEIGHT;
                    */
                        #endregion

                        //Comment out to test glitch version
                        float tempBottom = m_physicsObj.Rect.Bottom;
                        m_state = ObtainPlayerState(PlayerState.Sliding, m_action);
                        m_slideTimer = SLIDE_DURATION;
                        m_physicsObj.Rect.Width = SLIDING_PHYS_WIDTH;
                        m_physicsObj.Rect.Height = SLIDING_PHYS_HEIGHT;
                        m_physicsObj.PosY = tempBottom - SLIDING_PHYS_HEIGHT;
                        //^^^Comment out to test glitch version^^^
                    }
                    #endregion

                    if ((PlayerState)m_state != PlayerState.Sliding)
                    {
                        #region Walk/Run/Stand Left/Right
                        if (ControllerState.isControlPressed(Control.MoveRight) && !IsCurrStateChargingLockMovement())
                        {
                            m_direction = Direction.Right;

                            if (ControllerState.isControlPressed(Control.Run))
                            {
                                m_physicsObj.VelX = RUNSPEED;
                                m_state = ObtainPlayerState(PlayerState.Running, m_action);
                            }
                            else
                            {
                                m_physicsObj.VelX = WALKSPEED;
                                m_state = ObtainPlayerState(PlayerState.Walking, m_action);
                            }
                        }

                        else if (ControllerState.isControlPressed(Control.MoveLeft) && !IsCurrStateChargingLockMovement())
                        {
                            m_direction = Direction.Left;
                            if (ControllerState.isControlPressed(Control.Run))
                            {
                                m_physicsObj.VelX = -RUNSPEED;
                                m_state = ObtainPlayerState(PlayerState.Running, m_action);
                            }
                            else
                            {
                                m_physicsObj.VelX = -WALKSPEED;
                                m_state = ObtainPlayerState(PlayerState.Walking, m_action);
                            }
                        }
                        else if (m_physicsObj.Sided[Side.bottom] && m_physicsObj.Vel.X < 5 && m_physicsObj.Vel.Y < 5)
                        {
                            m_state = ObtainPlayerState(PlayerState.Standing, m_action);
                            if (ControllerState.isControlPressed(Control.MoveRight))
                            {
                                m_direction = Direction.Right;
                            }
                            else if (ControllerState.isControlPressed(Control.MoveLeft))
                            {
                                m_direction = Direction.Left;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else //Midair movement is acceleration-based, making it slightly less precise, but giving a feeling of momentum to the jumps.
                {
                    #region In Air
                    if (ControllerState.isControlPressed(Control.MoveRight))
                    {
                        m_physicsObj.AccelX = AIRACCEL;
                    }
                    else if (ControllerState.isControlPressed(Control.MoveLeft))
                    {
                        m_physicsObj.AccelX = -AIRACCEL;
                    }
                    else
                    {
                        m_physicsObj.AccelX = 0; //Adds a bit more control, allowing releasing the direction for a bit less horizontal acceleration. 
                    }

                    if (m_physicsObj.VelX > WALKSPEED) //Speed is capped at the same value as on the ground.
                    {
                        m_physicsObj.VelX = WALKSPEED;
                    }
                    else if (m_physicsObj.VelX < -WALKSPEED)
                    {
                        m_physicsObj.VelX = -WALKSPEED;
                    }


                    if (m_physicsObj.VelX > 0)
                        m_direction = AnimeFightClub001.Direction.Right;
                    else if (m_physicsObj.VelX < 0)
                        m_direction = AnimeFightClub001.Direction.Left;
                    #endregion
                }

                //All jump physics are here.
                #region Jump Physics
                //Wall jumping
                if (m_state != (int)PlayerState.Sliding && !IsCurrStateChargingLockMovement() && ControllerState.isControlPressed(Control.Jump) && !m_oldJump && !m_physicsObj.Sided[Side.bottom] && (m_physicsObj.Sided[Side.left] || m_physicsObj.Sided[Side.right]))
                {
                    m_physicsObj.AccelX = 0;
                    m_oldJump = true; //The oldJump variable prevents one press of the jump button from being registered multiple times.

                    if (m_chainingAllowed) //Jump chaining is an interesting but possibly broken mechanic allowing double jumps to be performed from wall jumps without limit.
                    {
                        m_canDoubleJump = true;
                    }

                    if (m_physicsObj.Sided[Side.left]) //Determine the direction to kick off in.
                    {
                        m_physicsObj.VelX = WALLJUMPXVEL;
                        m_direction = Direction.Right;
                    }
                    else
                    {
                        m_physicsObj.VelX = -WALLJUMPXVEL;
                        m_direction = Direction.Left;
                    }

                    m_physicsObj.VelY = -WALLJUMPYVEL;

                    m_state = ObtainPlayerState(PlayerState.Jumping, m_action);

                }

                //Beginning of a jump.  Handles both vanilla (from the ground) jumps and double jumps.  Mutually exclusive with wall jumping.
                else if (m_state != (int)PlayerState.Sliding && !IsCurrStateChargingLockMovement() && ControllerState.isControlPressed(Control.Jump) && !m_oldJump && (m_physicsObj.Sided[Side.bottom] || m_canDoubleJump))
                {
                    m_oldJump = true;

                    if (!m_physicsObj.Sided[Side.bottom]) //If this is a double jump, essentially.
                    {
                        m_canDoubleJump = false;
                        m_physicsObj.VelX = 0;
                    }

                    m_physicsObj.VelY = -JUMPMINVEL;
                    m_physicsObj.AccelY = -JUMPACCEL;
                    m_jumpCounter = 0; //The jump counter measures the duration of the jump.

                    m_state = ObtainPlayerState(PlayerState.Jumping, m_action);
                }

                if (ControllerState.isControlPressed(Control.Jump))
                {
                    m_jumpCounter += gameTime.ElapsedGameTime.Milliseconds; //Keeps track of the time spent jumping.
                }
                else
                {
                    m_oldJump = false; //When the jump button is released, the next time it is pressed, register it as a new jump.
                }


                //Ending a jump.  Now the player starts falling, also reset the jump counter.
                if ((!ControllerState.isControlPressed(Control.Jump) || m_jumpCounter >= JUMPLENGTHMS) && !m_physicsObj.Sided[Side.bottom])
                {
                    //m_physicsObj.AccelY = GlobalVariables.Settings.Gravity; This isn't needed because gravity is applied separately from accel
                    m_physicsObj.AccelY = 0;
                    m_jumpCounter = 0;

                    m_state = ObtainPlayerState(PlayerState.Jumped, m_action);
                }
                #endregion

                #endregion

            }

            m_ctrl.PrevControllerState.Copy(m_ctrl.ControllerState);

            #region Cludge
            if ((PlayerState)m_state != PlayerState.Sliding)
            {
                m_physicsObj.Rect.Width = NORM_PHYS_WIDTH;
                m_physicsObj.Rect.Height = NORM_PHYS_HEIGHT;
            }
            #endregion

        }


        //****************************************************
        // Method: Respawn
        //
        // Purpose: Resets the player's health and mana, 
        // clears their statuses, restarts their state and
        // action and places them at one of the map's
        // spawn points
        //****************************************************
        public void Respawn()
        {
            HP = GlobalVariables.Settings.startingHealth;
            MP = GlobalVariables.Settings.startingMana;

            //Randomize between map positions (put map positions into a list in global variables)
            //SetPos(0, 0);
            //Lets server do it

            m_action = PlayerAction.None;
            m_state = (int)PlayerState.Standing;

            m_statusHandler.Clear();

            m_loadout.FullReset();

            m_physicsObj.Rect.Height = NORM_PHYS_HEIGHT;
            m_physicsObj.Rect.Width = NORM_PHYS_WIDTH;

            m_physicsObj.VelX = 0;
            m_physicsObj.VelY = 0;

            m_physicsObj.AccelX = 0;
            m_physicsObj.AccelY = 0;
        }

        //****************************************************
        // Method: ObtainPlayerState
        //
        // Purpose: This function is desgined to accept the
        // state you intend on setting the player to be in
        // and the action the player is doing to produce 
        // the state they are actually doing. This is due
        // to very cludgy animation code.
        //****************************************************
        public int ObtainPlayerState(PlayerState state, PlayerAction action)
        {
            if (action == PlayerAction.None)
                return (int)state;

            //Just do the action
            if (state == PlayerState.Standing || state == PlayerState.Jumping || state == PlayerState.Jumped || state == PlayerState.DoubleJumping)
            {
                return (int)((PlayerState)Enum.Parse(typeof(PlayerState), action.ToString()));
            }

            if (action >= PlayerAction.OpenPalmForward && action <= PlayerAction.OpenPalmForwardDown && !(action == PlayerAction.OpenPalmForwardCharge && action <= PlayerAction.OpenPalmForwardRelease && m_action <= PlayerAction.OpenPalmForwardRelease && m_action != PlayerAction.OpenPlamForwardHoldOut))
            {
                if (state == PlayerState.Walking)
                    return (int)PlayerState.OpenPalmForwardWalking;
                else if (state == PlayerState.Running)
                    return (int)PlayerState.OpenPalmForwardRunning;
            }

            if (action >= PlayerAction.ShootingForward && action <= PlayerAction.ShootingForwardDown)
            {
                if (state == PlayerState.Walking)
                    return (int)PlayerState.ShootingForwardWalking;
                else if (state == PlayerState.Running)
                    return (int)PlayerState.ShootingForwardRunning;
            }

            if (state != PlayerState.LedgeGrabbing && action >= PlayerAction.Slashing && action <= PlayerAction.StabbingRelease)
            {
                return (int)((PlayerState)Enum.Parse(typeof(PlayerState), action.ToString()));
            }

            if (state == PlayerState.LedgeGrabbing)
            {
                m_loadout.Interrupt();
            }

            return (int)state;
        }

        //****************************************************
        // Method: IsCurrStateChargingLockMovement
        //
        // Purpose: Checks if the current state is a melee
        // state which should lock movement
        //****************************************************
        public bool IsCurrStateChargingLockMovement()
        {
            return ((m_action >= PlayerAction.Slashing && m_action <= PlayerAction.SlashingRelease) || (m_action >= PlayerAction.OpenPalmForwardCharge && m_action <= PlayerAction.OpenPalmForwardRelease && m_action != PlayerAction.OpenPlamForwardHoldOut));
        }

        #region Collide Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles damage to the player.
        // Will also perform knockback later.
        //****************************************************
        public override void CollideBefore(HazardObj hazard)
        {
            if (hazard.GetType().ToString().Split('.')[1] == "HazardBombExplosion")
                return;
            m_statusHandler.Inflict(StatusName.Damage, hazard.Damage, hazard.OwnerID);
            Status tempStatus = m_statusHandler.FindStatus(StatusName.ShieldAbility);
            if (tempStatus != null)
                m_statusHandler.DeleteStatus(tempStatus);
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles player collision with
        // and EnvironmentalObj
        //****************************************************
        public override void CollideAfter(EnvironmentalObj environmentalObj)
        {
            if (environmentalObj.PhysicsObj.Rect.Top <= (Center.Y - 15) && environmentalObj.PhysicsObj.Rect.Top > m_physicsObj.Rect.Top && m_grabCounter <= 0)
            {
                if (m_direction == AnimeFightClub001.Direction.Right && ControllerState.isControlPressed(Control.MoveRight) && m_physicsObj.Sided[Side.right] && environmentalObj.PhysicsObj.Rect.Left <= m_physicsObj.Rect.Right)
                {
                    m_ledgeGrabPos.X = environmentalObj.PhysicsObj.Rect.Left - m_physicsObj.Rect.Width;
                    m_ledgeGrabPos.Y = environmentalObj.PhysicsObj.Rect.Top;
                    m_state = ObtainPlayerState(PlayerState.LedgeGrabbing, m_action);
                }
                else if (m_direction == AnimeFightClub001.Direction.Left && ControllerState.isControlPressed(Control.MoveLeft) && m_physicsObj.Sided[Side.left] && environmentalObj.PhysicsObj.Rect.Right >= m_physicsObj.Rect.Left)
                {
                    m_ledgeGrabPos.X = environmentalObj.PhysicsObj.Rect.Right;
                    m_ledgeGrabPos.Y = environmentalObj.PhysicsObj.Rect.Top;
                    m_state = ObtainPlayerState(PlayerState.LedgeGrabbing, m_action);
                }
            }
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles player collision with
        // a PlayerObj
        //****************************************************
        public override void CollideBefore(PlayerObj playerObj)
        {
        }

        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return m_userName;
            }
        }

        public long PlayerID
        {
            get
            {
                return m_playerID;
            }
        }

        public ControllerState ControllerState
        {
            get
            {
                return m_ctrl.ControllerState;
            }
        }

        public ControllerState PrevControllerState
        {
            get
            {
                return m_ctrl.PrevControllerState;
            }
        }

        public Controller Controller
        {
            get
            {
                return m_ctrl;
            }
        }

        public PlayerAction Action
        {
            get
            {
                return m_action;
            }
            set
            {
                m_action = value;
            }
        }

        public int HP
        {
            get
            {
                return m_hp;
            }

            set
            {
                if (value < m_hp)
                {
                    GlobalVariables.PlaySound("Hit Marker");
                }
                m_hp = value;
            }
        }


        public int MP
        {
            get
            {
                return m_mp;
            }

            set
            {
                m_mp = value;
            }
        }

        public StatusHandler StatusHandler
        {
            get
            {
                return m_statusHandler;
            }
        }

        public int Kills
        {
            get
            {
                return m_kills;
            }
            set
            {
                m_kills = value;
            }
        }

        public CharacterLoadout Loadout
        {
            get
            {
                return m_loadout;
            }
        }

        public Vector2 LedgeGrabbingPos
        {
            get
            {
                return m_ledgeGrabPos;
            }
            set
            {
                m_ledgeGrabPos = value;
            }
        }
        #endregion
    }
}
