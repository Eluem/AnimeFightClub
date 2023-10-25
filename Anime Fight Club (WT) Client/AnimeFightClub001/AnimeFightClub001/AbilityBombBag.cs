//******************************************************
// File: AbilityBombBag.cs
//
// Purpose: Defines AbilityBombBag.
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
    class AbilityBombBag : Ability
    {
        //****************************************************
        // Method: AbilityBombBag
        //
        // Purpose: Default constructor for AbilityBombBag
        //****************************************************
        public AbilityBombBag() :
            base(null, 0, 0, 0, 0)
        {


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
                m_ArbitraryFlag = true;
                GlobalVariables.PlaySound("Throw");
                m_ownerObj.Action = PlayerAction.OpenPalmForward;
                m_castTimer = 200;
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
            ItemBomb added = new ItemBomb(550, m_ownerObj.ObjectID, "Player");
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
            if (m_ArbitraryTimer >= 10000)
            {
                m_ArbitraryTimer = 0;
                m_ArbitraryFlag = false;
            }
        }
    }
}
