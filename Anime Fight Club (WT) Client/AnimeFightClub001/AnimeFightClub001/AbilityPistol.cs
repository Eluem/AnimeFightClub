//******************************************************
// File: AbilityPistol.cs
//
// Purpose: Defines AbilityPistol. Basic gun...
//
// Written By: Salvatore Hanusiewicz
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;


namespace AnimeFightClub001
{
    enum AbilityPistolStateEnum { Holstered, ShootingForward, ShootingForwardHoldOut, ShootingForwardHoldIn, ShootingUp, ShootingDown, ShootingForwardUp, ShootingForwardUpHoldOut, ShootingForwardUpHoldIn, ShootingForwardDown };
    enum AbilityPistolAimingEnum { Forward, ForwardUp, ForwardDown, Up, Down }
    class AbilityPistol : Ability
    {
        #region Declarations
        protected AbilityPistolAimingEnum m_aim; //Depending on the direction of attack
        #endregion

        //*********************************************************
        // Method: AbilityPistol
        //
        // Purpose: Default constructor for AbilityPistol
        //*********************************************************
        public AbilityPistol() :
            base(null, 0, 0, 0, 0, "AbilityPistol")
        {
            m_ArbitraryFlag = false;
            m_ArbitraryTimer = 0;
            m_state = (int)AbilitySwordStateEnum.Holstered;
            m_aim = AbilityPistolAimingEnum.Forward;
        }

        //****************************************************
        // Method: PressedHook
        //
        // Purpose: Overrideable hook for PressedAbility.
        //****************************************************
        public override void PressedHook()
        {
            HazardPistolBullet added = null;
            if (!m_ArbitraryFlag)
            {
                GlobalVariables.PlaySound("Gunshot", .35f);
                switch (m_aim)
                {
                    case AbilityPistolAimingEnum.Forward:
                        //Shoot forward
                        added = new HazardPistolBullet(1000, m_ownerObj.ObjectID, "Player");
                        added.PhysicsObj.PosY -= 10;
                        GameObjectHandler.AddObject(added);
                        m_ownerObj.Action = PlayerAction.ShootingForward;
                        if (!IsPlayerWalkingOrRunning())
                            m_state = (int)AbilityPistolStateEnum.ShootingForward;
                        else
                            m_state = (int)AbilityPistolStateEnum.ShootingForwardHoldOut;
                        break;
                    case AbilityPistolAimingEnum.ForwardUp:
                        //Shoot forward up
                        added = new HazardPistolBullet(500, m_ownerObj.ObjectID, "Player");
                        added.PhysicsObj.VelY = -500;
                        added.PhysicsObj.PosY = added.PhysicsObj.PosY - 40;
                        GameObjectHandler.AddObject(added);
                        m_ownerObj.Action = PlayerAction.ShootingForwardUp;

                        m_state = (int)AbilityPistolStateEnum.ShootingForwardUp;
                        break;
                    case AbilityPistolAimingEnum.Up:
                        //Shoot up
                        added = new HazardPistolBullet(300, m_ownerObj.ObjectID, "Player");
                        added.PhysicsObj.VelY = -700;
                        added.PhysicsObj.PosY = added.PhysicsObj.PosY - 60;
                        /*
                        if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
                        {
                            added.PhysicsObj.PosX += 20;
                        }
                        else
                        {
                            added.PhysicsObj.PosX -= 20;
                        }
                        */
                        GameObjectHandler.AddObject(added);
                        m_ownerObj.Action = PlayerAction.ShootingUp;
                        m_state = (int)AbilityPistolStateEnum.ShootingUp;
                        break;
                    case AbilityPistolAimingEnum.ForwardDown:
                        //Shoot forward down
                        added = new HazardPistolBullet(250, m_ownerObj.ObjectID, "Player");
                        added.PhysicsObj.VelY = 650;
                        added.PhysicsObj.PosY = added.PhysicsObj.PosY + 32;
                        if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
                        {
                            added.PhysicsObj.PosX -= 3;
                        }
                        else
                        {
                            added.PhysicsObj.PosX += 3;
                        }
                        GameObjectHandler.AddObject(added);

                        m_ownerObj.Action = PlayerAction.ShootingForwardDown;
                        m_state = (int)AbilityPistolStateEnum.ShootingForwardDown;
                        break;
                    case AbilityPistolAimingEnum.Down:
                        //Shoot down
                        added = new HazardPistolBullet(250, m_ownerObj.ObjectID, "Player");
                        added.PhysicsObj.VelY = 650;
                        added.PhysicsObj.PosY = added.PhysicsObj.PosY + 32;
                        if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
                        {
                            added.PhysicsObj.PosX -= 3;
                        }
                        else
                        {
                            added.PhysicsObj.PosX += 3;
                        }
                        GameObjectHandler.AddObject(added);

                        m_ownerObj.Action = PlayerAction.ShootingDown;
                        m_state = (int)AbilityPistolStateEnum.ShootingDown;
                        break;
                }
                m_ArbitraryFlag = true;
                m_ArbitraryTimer = 0;
                m_castTimer = 100;
                added.PhysicsObj.VelX += m_ownerObj.Vel.X / 2.5f;
                added.PhysicsObj.VelY += m_ownerObj.Vel.Y / 2.5f;
            }
        }


        //****************************************************
        // Method: CastHook
        //
        // Purpose: Hooks into what happens when the
        // cast timer reaches 0
        //****************************************************
        public override void CastHook()
        {
            switch (m_aim)
            {
                case AbilityPistolAimingEnum.Forward:
                    //Shoot forward
                    m_ownerObj.Action = PlayerAction.ShootingForwardHoldIn;
                    if (!IsPlayerWalkingOrRunning())
                        m_state = (int)AbilityPistolStateEnum.ShootingForwardHoldIn;
                    break;
                case AbilityPistolAimingEnum.ForwardUp:
                    //Shoot forward up
                    m_ownerObj.Action = PlayerAction.ShootingForwardUpHoldIn;
                    m_state = (int)AbilityPistolStateEnum.ShootingForwardUpHoldIn;
                    break;
            }
            GlobalVariables.PlaySound("LongReload", .35f);
            m_castWaitTimer = 300;
        }

        //****************************************************
        // Method: CastWaitHook
        //
        // Purpose: Hooks into what occurs when the 
        // cast wait timer hits zero
        //****************************************************
        public override void CastWaitHook()
        {
            m_state = (int)AbilityPistolStateEnum.Holstered;
            m_ownerObj.Action = PlayerAction.None;
        }

        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the update function
        //****************************************************
        public override void UpdateHook(GameTime gameTime)
        {
            if (m_ArbitraryTimer >= 2000)
            {
                m_ArbitraryFlag = false;
            }

            if (IsPlayerWalkingOrRunning())
            {
                if (IsAimingInSomeDirectionCorrectly())
                {
                    m_state = (int)AbilityPistolStateEnum.ShootingForwardHoldOut;
                    m_ownerObj.Action = PlayerAction.ShootingForwardHoldOut;
                    m_aim = AbilityPistolAimingEnum.Forward;
                }
            }
            if (m_castTimer <= 0 && m_castWaitTimer <= 0)
            {
                if (m_ownerObj.State != (int)PlayerState.Sliding && m_ownerObj.State != (int)PlayerState.LedgeGrabbing)
                {
                    if (IsPlayerWalkingOrRunning())
                    {
                        if (IsAimingInSomeDirectionCorrectly())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingForwardHoldOut;
                            m_ownerObj.Action = PlayerAction.ShootingForwardHoldOut;
                            m_aim = AbilityPistolAimingEnum.Forward;
                        }
                    }
                    else
                    {
                        if (IsAimingUp())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingUp;
                            m_ownerObj.Action = PlayerAction.ShootingUp;
                            m_aim = AbilityPistolAimingEnum.Up;
                        }
                        else if (IsAimingDown())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingDown;
                            m_ownerObj.Action = PlayerAction.ShootingDown;
                            m_aim = AbilityPistolAimingEnum.Down;
                        }
                        else if (IsAimingForward())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingForwardHoldOut;
                            m_ownerObj.Action = PlayerAction.ShootingForwardHoldOut;
                            m_aim = AbilityPistolAimingEnum.Forward;
                        }
                        else if (IsAimingForwardUp())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingForwardUpHoldOut;
                            m_ownerObj.Action = PlayerAction.ShootingForwardUpHoldOut;
                            m_aim = AbilityPistolAimingEnum.ForwardUp;
                        }
                        else if (IsAimingForwardDown())
                        {
                            m_state = (int)AbilityPistolStateEnum.ShootingForwardDown;
                            m_ownerObj.Action = PlayerAction.ShootingForwardDown;
                            m_aim = AbilityPistolAimingEnum.ForwardDown;
                        }
                    }
                }

                if (!IsAimingInSomeDirectionCorrectly())
                {
                    if (m_state != (int)AbilitySwordStateEnum.Holstered)
                    {
                        m_state = (int)AbilitySwordStateEnum.Holstered;
                        m_ownerObj.Action = PlayerAction.None;
                    }
                    m_aim = AbilityPistolAimingEnum.Forward;
                }
            }
        }

        //****************************************************
        // Method: FullReset
        //
        // Purpose: Fully resets the ability
        //****************************************************
        public override void FullReset()
        {
            m_currAmmo = m_maxAmmo;
            m_currEnergy = m_maxEnergy;
            m_currHeat = 0;

            m_state = 0;
            m_prevState = 0;

            m_castTimer = 0;
            m_castWaitTimer = 0;
        }

        //****************************************************
        // Method: Interrupt
        //
        // Purpose: Interrupts the ability
        //****************************************************
        public override void Interrupt()
        {
            base.Interrupt();
            m_state = (int)AbilitySwordStateEnum.Holstered;
        }

        //****************************************************
        // Method: IsPlayerWalkingOrRunning
        //
        // Purpose: Checks if the player is walking or
        // running
        //****************************************************
        private bool IsPlayerWalkingOrRunning()
        {
            return m_ownerObj.State == (int)PlayerState.Running || m_ownerObj.State == (int)PlayerState.Walking || (m_ownerObj.State >= (int)PlayerState.ShootingForwardWalking && m_ownerObj.State <= (int)PlayerState.ShootingForwardDownWalking) || (m_ownerObj.State >= (int)PlayerState.ShootingForwardRunning && m_ownerObj.State <= (int)PlayerState.ShootingForwardDownRunning);
        }

        #region Aiming functions
        //****************************************************
        // Method: IsAimingUp
        //
        // Purpose: Checks if aiming straight up
        //****************************************************
        private bool IsAimingUp()
        {
            return (m_ownerObj.ControllerState.isControlPressed(Control.AimUp) && !m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimDown));
        }

        //****************************************************
        // Method: IsAimingDown
        //
        // Purpose: Checks if aiming straight down
        //****************************************************
        private bool IsAimingDown()
        {
            return (m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
        }

        //****************************************************
        // Method: IsAimingForward
        //
        // Purpose: Checks if aiming forward
        //****************************************************
        private bool IsAimingForward()
        {
            if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
            else
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
        }

        //****************************************************
        // Method: IsAimingForwardUp
        //
        // Purpose: Checks if aiming forward and up
        //****************************************************
        private bool IsAimingForwardUp()
        {
            if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
            else
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && !m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
        }

        //****************************************************
        // Method: IsAimingForwardDown
        //
        // Purpose: Checks if aiming forward and down
        //****************************************************
        private bool IsAimingForwardDown()
        {
            if (m_ownerObj.Direction == AnimeFightClub001.Direction.Left)
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
            else
            {
                return (m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && m_ownerObj.ControllerState.isControlPressed(Control.AimDown) && !m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && !m_ownerObj.ControllerState.isControlPressed(Control.AimUp));
            }
        }

        //****************************************************
        // Method: IsAimingInSomeDirectionCorrectly
        //
        // Purpose: lol
        //****************************************************
        private bool IsAimingInSomeDirectionCorrectly()
        {
            return IsAimingDown() || IsAimingUp() || IsAimingForward() || IsAimingForwardUp() || IsAimingForwardDown();
        }
        #endregion

    }
}
