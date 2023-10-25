//******************************************************
// File: ShortVerticleBar.cs
//
// Purpose: Defines a ShortVerticleBar
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
    class ShortVerticleBar : EnvironmentalObj
    {
        //****************************************************
        // Method: ShortVerticleBar
        //
        // Purpose: Default ShortVerticleBar Constructor
        //****************************************************
        public ShortVerticleBar()
            : base()
        {
            m_drawableObj = new DrawableObj("ShortVerticleBar", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Environmental));
            SetDimensions(78, 192);

            PhysicsObj.Mass = 10000;
        }
    }
}