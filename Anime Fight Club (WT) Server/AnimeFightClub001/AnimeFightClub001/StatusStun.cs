//******************************************************
// File: StatusStun.cs
//
// Purpose: The player is stunned
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
    class StatusStun : Status
    {

        //****************************************************
        // Method: StatusStun
        //
        // Purpose: Constructor for StatusStun
        //****************************************************
        public StatusStun(UInt16 statusID, PlayerObj inflictorObj, PlayerObj ownerObj, int severity, int duration)
            : base(statusID, inflictorObj, ownerObj, StatusName.Stun, severity, duration, 500)
        {
        }

    }
}
