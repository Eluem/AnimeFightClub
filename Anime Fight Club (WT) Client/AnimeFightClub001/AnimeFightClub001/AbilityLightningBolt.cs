//******************************************************
// File: AbilityLightningBolt.cs
//
// Purpose: Defines AbilityLightningBolt. This is a
// basic special that allows the player to shoot
// lightning bolts
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
    class AbilityLightningBolt : Ability
    {
        enum AbilityLightningBoltStateEnum { Holstered, PullingBack, HoldBack, HoldBackCharged, Release, HoldOut };

        //*********************************************************
        // Method: AbilityLightningBolt
        //
        // Purpose: Default constructor for AbilityLightningBolt
        //*********************************************************
        public AbilityLightningBolt() :
            base(null, 0, 0, 0, 0, "AbilityLightningBolt")
        {
            m_ArbitraryFlag = false;
            m_ArbitraryTimer = 0;

        }

        //****************************************************
        // Method: PressedHook
        //
        // Purpose: Overrideable hook for PressedAbility.
        //****************************************************
        public override void PressedHook()
        {
            if (!m_ArbitraryFlag && m_pressedDuration == 0 && m_ownerObj.MP >= 4000)
            {
                m_ownerObj.Action = PlayerAction.OpenPalmForwardCharge;
                m_state = (int)AbilityLightningBoltStateEnum.PullingBack;
            }
        }

        //****************************************************
        // Method: ReleasedHook
        //
        // Purpose: Overrideable hook for ReleasedAbility.
        //****************************************************
        public override void ReleasedHook()
        {
            if (m_pressedDuration >= 800 && m_ownerObj.MP >= 4000)
            {
                GlobalVariables.PlaySound("Thunder");
                m_ownerObj.MP -= 4000;
                m_ownerObj.Action = PlayerAction.OpenPalmForwardRelease;
                m_state = (int)AbilityLightningBoltStateEnum.Release;
                m_castTimer = 240;
            }
            else
            {
                m_state = (int)AbilityLightningBoltStateEnum.Holstered;
                m_ownerObj.Action = PlayerAction.None;
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
            HazardLightningBolt added = new HazardLightningBolt(1200, m_ownerObj.ObjectID, "Player");
            GameObjectHandler.AddObject(added);
            m_state = (int)AbilityLightningBoltStateEnum.HoldOut;
            m_ownerObj.Action = PlayerAction.OpenPlamForwardHoldOut;
            m_castWaitTimer = 200;

            m_ArbitraryFlag = true;
            m_ArbitraryTimer = 0;
        }

        //****************************************************
        // Method: CastWaitHook
        //
        // Purpose: Hooks into what occurs when the 
        // cast wait timer hits zero
        //****************************************************
        public override void CastWaitHook()
        {
            m_state = (int)AbilityLightningBoltStateEnum.Holstered;
            m_ownerObj.Action = PlayerAction.None;
        }

        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the update function
        //****************************************************
        public override void UpdateHook(GameTime gameTime)
        {
            if (m_pressedDuration >= 800 && m_state == (int)AbilityLightningBoltStateEnum.HoldBack)
            {
                m_state = (int)AbilityLightningBoltStateEnum.HoldBackCharged;
                m_ownerObj.Action = PlayerAction.OpenPlamForwardHoldIn;
            }

            if (m_pressedDuration >= 200 && m_state == (int)AbilityLightningBoltStateEnum.PullingBack)
            {
                m_state = (int)AbilityLightningBoltStateEnum.HoldBack;
                m_ownerObj.Action = PlayerAction.OpenPlamForwardHoldIn;
            }

            if (m_ArbitraryTimer >= 1000)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = false;
            }
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

    }
}
