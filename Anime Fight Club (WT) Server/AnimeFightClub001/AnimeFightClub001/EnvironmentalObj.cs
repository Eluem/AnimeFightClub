//******************************************************
// File: Environmental.cs
//
// Purpose:  Contains the definition of the
// Environmental Object type. This type of object
// will be used for stationary objects that everything
// in the game interacts with. They will not respond
// to any form of collision. They will be the platforms
// and walls that players and objects interact with.
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
    class EnvironmentalObj : BasicObj
    {
        //****************************************************
        // Method: EnvironmentalObj
        //
        // Purpose: Default EnvironmentalObj Constructor
        //****************************************************
        public EnvironmentalObj() : base()
        {
            m_physicsObj.Friction = .000001f;
            m_physicsObj.ApplyMomentum = false;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            
        }
    }
}
