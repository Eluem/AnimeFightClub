//******************************************************
// File: AbilitySpikeTrap.cs
//
// Purpose: Defines AbilitySpikeTrap.
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
    class AbilitySpikeTrap : Ability
    {
        //****************************************************
        // Method: AbilitySpikeTrap
        //
        // Purpose: Default constructor for AbilitySpikeTrap
        //****************************************************
        public AbilitySpikeTrap() :
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
            if (m_pressedDuration == 0 && m_ownerObj.MP >= 10000)
            {
                m_ownerObj.Action = PlayerAction.OpenPalmForward;
                m_ownerObj.MP -= 10000;
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
            HazardSpikeTrap added = new HazardSpikeTrap(550, m_ownerObj.ObjectID, "Player");
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
    }
}
