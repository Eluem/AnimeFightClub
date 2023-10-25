//******************************************************
// File: LongHorizontalBar.cs
//
// Purpose: Defines a LongHorizontalBar
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
    class LongHorizontalBar : EnvironmentalObj
    {
        //****************************************************
        // Method: LongHorizontalBar
        //
        // Purpose: Default LongHorizontalBar Constructor
        //****************************************************
        public LongHorizontalBar()
            : base()
        {
            m_drawableObj = new DrawableObj("LongHorizontalBar", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Environmental));
            SetDimensions(1772, 78);

            PhysicsObj.Mass = 10000;
        }
    }
}