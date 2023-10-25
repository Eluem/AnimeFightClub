//******************************************************
// File: SpecialEnvironmental.cs
//
// Purpose: Contains the definition of the
// SpecialEnvironmental Object type. This object type
// ineherets from Environmental and is definied for
// any environmental object that may have some special
// attributes that would cause it to want to be able to
// collide with everything in the game. The normal
// Environmental objects will not collide with each
// other as they have no reason to.
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
    class SpecialEnvironmentalObj : EnvironmentalObj
    {
        //****************************************************
        // Method: NetInitialize
        //
        // Purpose: Initializes values based on the 
        // mesage sent by the server
        //****************************************************
        public override void NetInitialize(NetIncomingMessage msg)
        {
            base.NetInitialize(msg);
        }

        //******************************************************
        // Method: SpecialEnvironmentalObj
        //
        // Purpose: Default SpecialEnvironmentalObj Constructor
        //******************************************************
        public SpecialEnvironmentalObj()
            : base()
        {
            m_physicsObj.ApplyMomentum = true;
            m_drawableObj.Layer = GlobalVariables.GetLayer(ObjectLayer.SpecialEnvironmental);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            m_drawableObj.Update(gameTime);
        }

        //****************************************************
        // Method: Delete
        //
        // Purpose: Deletes the object by adding it to the
        // appropraite delete list and setting the deleted
        // boolean to true.
        //****************************************************
        public override void Delete()
        {
            m_deleted = true;
            GameObjectHandler.DeleteObject(this);
        }
    }
}
