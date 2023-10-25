//******************************************************
// File: StatusKnockBack.cs
//
// Purpose: The player is knocked back
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
    class StatusKnockBack : Status
    {

        //****************************************************
        // Method: StatusKnockBack
        //
        // Purpose: Constructor for StatusKnockBack
        //****************************************************
        public StatusKnockBack(UInt16 statusID, PlayerObj inflictorObj, PlayerObj ownerObj, int severity, int duration)
            : base(statusID, inflictorObj, ownerObj, StatusName.KnockBack, severity, duration, 500)
        {
        }

        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the end of the update
        // function
        //****************************************************
        public override void UpdateHook(GameTime gameTime)
        {
            m_ownerObj.PhysicsObj.VelX = m_severity;
        }

    }
}
