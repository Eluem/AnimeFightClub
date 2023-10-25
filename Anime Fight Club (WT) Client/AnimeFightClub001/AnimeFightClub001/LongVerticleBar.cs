//******************************************************
// File: LongVerticleBar.cs
//
// Purpose: Defines a LongVerticleBar
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
    class LongVerticleBar : EnvironmentalObj
    {
        //****************************************************
        // Method: LongVerticleBar
        //
        // Purpose: Default LongVerticleBar Constructor
        //****************************************************
        public LongVerticleBar()
            : base()
        {
            m_drawableObj = new DrawableObj("LongVerticleBar", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Environmental));
            SetDimensions(78, 886);

            PhysicsObj.Mass = 10000;
        }
    }
}