//******************************************************
// File: StatusDeath.cs
//
// Purpose: The player is dead, but respawns after
// a set time.
//
// Written By: Rob Maggio, Salvatore Hanusiewicz
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
    class StatusDeath : Status
    {

        //****************************************************
        // Method: StatusDeath
        //
        // Purpose: Constructor for StatusDeath
        //****************************************************
        public StatusDeath(UInt16 statusID, PlayerObj inflictorObj, PlayerObj ownerObj, int severity, int duration)
            : base(statusID, inflictorObj, ownerObj, StatusName.Death, severity, duration)
        {
        }

        //****************************************************
        // Method: WearOffHook
        //
        // Purpose: Hooks into the part of ApplyStatusEffects
        // function that removes the status due to
        // it's duration expiring
        //****************************************************
        public override void WearOffHook(GameTime gameTime)
        {
            m_ownerObj.Respawn();
        }

    }
}
