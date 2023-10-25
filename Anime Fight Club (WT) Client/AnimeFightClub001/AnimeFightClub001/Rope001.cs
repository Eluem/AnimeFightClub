//******************************************************
// File: Rope001.cs
//
// Purpose: Defines a Rope001, it's a noncollidable
// object
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

using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;

namespace AnimeFightClub001
{
    class Rope001 : EnvironmentalObj
    {
        //****************************************************
        // Method: Rope001
        //
        // Purpose: Default Rope001 Constructor
        //****************************************************
        public Rope001()
            : base()
        {
            m_drawableObj = new DrawableObj("Rope001", Color.Wheat, this, .9f);
            SetDimensions(12, 481);
            m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X, 0);
            m_physicsObj.Rect.Width = 0;
            m_physicsObj.Rect.Height = 0;

            PhysicsObj.Mass = 0;
        }
    }
}