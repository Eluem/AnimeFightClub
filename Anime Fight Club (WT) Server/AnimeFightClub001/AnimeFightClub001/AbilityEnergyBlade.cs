//******************************************************
// File: AbilityEnergyBlade.cs
//
// Purpose: Defines AbilityEnergyBlade. Basic MainHand
// meleeish weapon
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
    class AbilityEnergyBlade : Ability
    {
        //*********************************************************
        // Method: AbilityEnergyBlade
        //
        // Purpose: Default constructor for AbilityEnergyBlade
        //*********************************************************
        public AbilityEnergyBlade() :
            base(null, 0, 0, 0, 0)
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
            if (!m_ArbitraryFlag && m_ownerObj.MP >= 3000)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = true;
                m_ownerObj.MP -= 3000;
                m_ownerObj.Action = PlayerAction.OpenPalmForward;
                m_castTimer = 100;
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
            HazardEnergyBlade added = new HazardEnergyBlade(800, m_ownerObj.ObjectID, "Player");
            GameObjectHandler.AddObject(added);
            m_ownerObj.Action = PlayerAction.OpenPlamForwardHoldOut;
            m_castWaitTimer = 200;
        }

        //****************************************************
        // Method: CastWaitHook
        //
        // Purpose: Hooks into what occurs when the 
        // cast wait timer hits zero
        //****************************************************
        public override void CastWaitHook()
        {
            m_ownerObj.Action = PlayerAction.None;
        }

        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the update function
        //****************************************************
        public override void UpdateHook(GameTime gameTime)
        {
            if (m_ArbitraryTimer >= 1000)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = false;
            }
        }
    }
}
