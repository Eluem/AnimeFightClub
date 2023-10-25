//******************************************************
// File: ShortHorizontalBar.cs
//
// Purpose: Defines a ShortHorizontalBar
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
    class ShortHorizontalBar : EnvironmentalObj
    {
        //****************************************************
        // Method: ShortHorizontalBar
        //
        // Purpose: Default ShortHorizontalBar Constructor
        //****************************************************
        public ShortHorizontalBar()
            : base()
        {
            SetDimensions(200, 78);

            PhysicsObj.Mass = 10000;
        }
    }
}