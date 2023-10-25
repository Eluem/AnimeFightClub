//******************************************************
// File: AbilityShield.cs
//
// Purpose: Defines AbilityShield. This is an offhand
// that allows the player to buff themself with a shield
// for a short period of time. This shield can absorb
// one shot.
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
    class AbilityShield : Ability
    {
        //*********************************************************
        // Method: AbilityShield
        //
        // Purpose: Default constructor for AbilityShield
        //*********************************************************
        public AbilityShield() :
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
            if (!m_ArbitraryFlag)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = true;
                m_ownerObj.StatusHandler.Inflict(StatusName.ShieldAbility, 450, m_ownerObj.ObjectID);
            }
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
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = false;
            }
        }
    }
}
