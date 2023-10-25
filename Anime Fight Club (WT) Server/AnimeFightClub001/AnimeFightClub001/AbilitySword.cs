//******************************************************
// File: AbilitySword.cs
//
// Purpose: Defines AbilitySword. Basic MainHand melee
// weapon
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
    enum AbilitySwordStateEnum { Holstered, Slashing, SlashingCharge, SlashingHoldUp, SlashingHoldUpCharged, SlashingHoldDown, SlashingRelease, SlashingReleaseCharged, Stabbing, StabbingCharge, StabbingHoldIn, StabbingHoldOut, StabbingRelease, ShootingForward, ShootingUp, ShootingDown, ShootingForwardUp, ShootingForwardDown };
    enum AbilitySwordAttackChoice { None, DefaultSlash, StrongSlash, Stab, Block }
    class AbilitySword : Ability
    {
        #region Declarations
        protected AbilitySwordAttackChoice m_attackChoice; //Depending on the direction of attack
        protected bool m_abilityFired; //Stores whether or not the ability already fired
        #endregion

        //*********************************************************
        // Method: AbilitySword
        //
        // Purpose: Default constructor for AbilitySword
        //*********************************************************
        public AbilitySword() :
            base(null, 0, 0, 0, 0)
        {
            m_ArbitraryFlag = false;
            m_ArbitraryTimer = 0;
            m_attackChoice = AbilitySwordAttackChoice.None;
            m_state = (int)AbilitySwordStateEnum.Holstered;
        }

        //****************************************************
        // Method: PressedHook
        //
        // Purpose: Overrideable hook for PressedAbility.
        //****************************************************
        public override void PressedHook()
        {
            //Aiming down is a block
            if (m_ownerObj.ControllerState.isControlPressed(Control.AimDown))
            {
                if (!m_ArbitraryFlag)
                {
                    m_ArbitraryTimer = 0;
                    m_ArbitraryFlag = true;
                    m_ownerObj.StatusHandler.Inflict(StatusName.ShieldAbility, 250, m_ownerObj.ObjectID);
                    //m_attackChoice = AbilitySwordAttackChoice.Block;
                }
            }
            //Aiming backwards (or forwards?) is a stab
            else if ((m_ownerObj.ControllerState.isControlPressed(Control.AimLeft) && m_ownerObj.Direction == AnimeFightClub001.Direction.Right) || (m_ownerObj.ControllerState.isControlPressed(Control.AimRight) && m_ownerObj.Direction == AnimeFightClub001.Direction.Left))
            {
                /*
                m_attackChoice = AbilitySwordAttackChoice.Stab;
                m_ownerObj.Action = PlayerAction.StabbingCharge;
                m_state = (int)AbilitySwordStateEnum.StabbingCharge;
                m_abilityFired = false;
                */
            }
            //Anything else doesn't matter, you're doing a normal slash
            else
            {
                m_attackChoice = AbilitySwordAttackChoice.DefaultSlash;
                m_ownerObj.Action = PlayerAction.SlashingCharge;
                m_state = (int)AbilitySwordStateEnum.SlashingCharge;
                m_abilityFired = false;
            }
        }

        //****************************************************
        // Method: ReleasedHook
        //
        // Purpose: Overrideable hook for ReleasedAbility.
        //****************************************************
        public override void ReleasedHook()
        {
            switch (m_attackChoice)
            {
                case AbilitySwordAttackChoice.Stab:
                    break;
                case AbilitySwordAttackChoice.DefaultSlash:
                    //Charged slash
                    if (m_pressedDuration > 650)
                    {
                        m_attackChoice = AbilitySwordAttackChoice.StrongSlash;
                        m_state = (int)AbilitySwordStateEnum.SlashingReleaseCharged; //Change to strong slash
                        m_ownerObj.Action = PlayerAction.SlashingRelease;
                        m_castTimer = 120; //Duration of release animation
                    }
                    //Normal slash
                    else if (m_pressedDuration > 210)
                    {
                        m_state = (int)AbilitySwordStateEnum.SlashingRelease;
                        m_ownerObj.Action = PlayerAction.SlashingRelease;
                        m_castTimer = 120; //Duration of release animation
                    }
                    else
                    {
                        m_state = (int)AbilitySwordStateEnum.Holstered;
                        m_ownerObj.Action = PlayerAction.None;
                        m_attackChoice = AbilitySwordAttackChoice.None;
                    }
                    break;
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
            m_state = (int)AbilitySwordStateEnum.SlashingHoldDown;
            m_ownerObj.Action = PlayerAction.SlashingHoldDown;
            m_castWaitTimer = 150; //Basically the cool down
        }

        //****************************************************
        // Method: CastWaitHook
        //
        // Purpose: Hooks into what occurs when the 
        // cast wait timer hits zero
        //****************************************************
        public override void CastWaitHook()
        {
            m_state = (int)AbilitySwordStateEnum.Holstered;
            m_ownerObj.Action = PlayerAction.None;
        }

        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the update function
        //****************************************************
        public override void UpdateHook(GameTime gameTime)
        {
            //Cool down for block
            if (m_ArbitraryTimer >= 2000)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = false;
            }

            if (m_pressedDuration > 210 && m_attackChoice != AbilitySwordAttackChoice.None) //Duration of the charging animation
            {
                m_ownerObj.Action = PlayerAction.SlashingHoldUp;
                m_state = (int)AbilitySwordStateEnum.SlashingHoldUp;
            }

            if (m_pressedDuration > 650 && m_attackChoice != AbilitySwordAttackChoice.None)
            {
                m_state = (int)AbilitySwordStateEnum.SlashingHoldUpCharged;
            }

            if (m_pressedDuration > 1150 && m_state == (int)AbilitySwordStateEnum.SlashingHoldUpCharged && m_ownerObj.PhysicsObj.Sided[Side.bottom])
            {
                Interrupt();
            }

            if (!m_abilityFired)
            {
                //After release, attack fires out
                switch (m_attackChoice)
                {
                    case AbilitySwordAttackChoice.Stab:
                        m_abilityFired = true;
                        break;
                    case AbilitySwordAttackChoice.DefaultSlash:
                        //Normal slash
                        if (m_releasedDuration > 40) //Time to let the animation play before spawning slash
                        {
                            HazardSwordSlash added = new HazardSwordSlash(500, m_ownerObj.ObjectID, "Player");
                            GameObjectHandler.AddObject(added);
                            m_abilityFired = true;
                        }
                        break;
                    case AbilitySwordAttackChoice.StrongSlash:
                        //Charged slash
                        if (m_releasedDuration > 40)
                        {
                            HazardSwordSlashStrong added = new HazardSwordSlashStrong(500, m_ownerObj.ObjectID, "Player");
                            GameObjectHandler.AddObject(added);
                            m_abilityFired = true;
                        }
                        break;
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

            m_attackChoice = AbilitySwordAttackChoice.None;
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
            m_attackChoice = AbilitySwordAttackChoice.None;
        }

    }
}
