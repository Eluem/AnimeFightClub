//******************************************************
// File: MediumHorizontalBar.cs
//
// Purpose: Defines a MediumHorizontalBar
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
    class MediumHorizontalBar : EnvironmentalObj
    {
        //****************************************************
        // Method: MediumHorizontalBar
        //
        // Purpose: Default MediumHorizontalBar Constructor
        //****************************************************
        public MediumHorizontalBar()
            : base()
        {
            SetDimensions(759, 78);

            PhysicsObj.Mass = 10000;
        }
    }
}