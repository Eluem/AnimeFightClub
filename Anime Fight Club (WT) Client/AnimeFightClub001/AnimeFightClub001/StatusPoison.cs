//******************************************************
// File: StatusPoison.cs
//
// Purpose: A damage over time status.
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
    class StatusPoison : Status
    {
        #region Declarations
        protected bool m_isFatal;
        #endregion

        //****************************************************
        // Method: StatusPoison
        //
        // Purpose: Constructor for StatusPoison
        //****************************************************
        public StatusPoison(UInt16 statusID, PlayerObj inflictorObj, PlayerObj ownerObj, int severity, int duration, bool isFatal)
            : base(statusID, inflictorObj, ownerObj, StatusName.Poison, severity, duration, 10000, "PoisonBubble")
        {
            m_offSet = new Vector2(0, -120);
            m_isFatal = isFatal;
        }

        //****************************************************
        // Method: TickHook
        //
        // Purpose: Hooks into the part of the update
        // function that decreases the timer
        //****************************************************
        public override void TickHook(GameTime gameTime)
        {
            if (m_ownerObj.HP > m_severity || m_isFatal)
            {
                UInt16 tempInflictorID = 0;
                if (m_inflictorObj != null)
                {
                    tempInflictorID = m_inflictorObj.ObjectID;
                }

                m_ownerObj.StatusHandler.Inflict(StatusName.Damage, m_severity, tempInflictorID);
            }
        }

    }
}
