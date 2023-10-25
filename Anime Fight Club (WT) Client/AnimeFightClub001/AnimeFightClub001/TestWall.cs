//******************************************************
// File: TestWall.cs
//
// Purpose: Defines the TestWall class. Used in
// very very early testing of networking and physics
// code.
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
    class TestWall : EnvironmentalObj
    {
        //****************************************************
        // Method: TestWall
        //
        // Purpose: Default TestPlatform Constructor
        //****************************************************
        public TestWall()
            : base()
        {
            PhysicsObj.Rect.SetDimensions((float)GlobalVariables.ImageDict["TestPlatform2"].Width,  (float)GlobalVariables.ImageDict["TestPlatform2"].Height);
            m_drawableObj = new DrawableObj("TestPlatform2", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Environmental));

            PhysicsObj.Mass = 10000;
        }
    }
}
