//******************************************************
// File: AbilityFireball.cs
//
// Purpose: Defines AbilityFireball. This is a basic
// special that can be used to shoot fireballs.
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;


namespace AnimeFightClub001
{
    class AbilityFireball : Ability
    {
        //****************************************************
        // Method: AbilityFireball
        //
        // Purpose: Default constructor for AbilityFireball
        //****************************************************
        public AbilityFireball() :
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
            if (m_pressedDuration == 0 && m_ownerObj.MP >= 1800)
            {
                //Doesn't work for some reason.. something to do with non-PCM data
                GlobalVariables.PlaySound("FireBall");
                m_ownerObj.Action = PlayerAction.OpenPalmForward;
                m_ownerObj.MP -= 1800;
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
            HazardFireball added = new HazardFireball(550, m_ownerObj.ObjectID, "Player");
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
