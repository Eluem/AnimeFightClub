//******************************************************
// File: Item.cs
//
// Purpose: Contains the definition of the
// Item Object type. This type of object can be picked
// up by players and potentially used (or activated) by
// them. This will be the class inhereted by object
// types such as grenade, health potion, bomb, key,
// spear, ect.
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
    class ItemObj : BasicObj
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
        // Method: ItemObj
        //
        // Purpose: Default ItemObj Constructor
        //******************************************************
        public ItemObj(UInt16 ownerID = 0, string ownerType = "World", int despawnClock = -1)
            : base(ownerID, ownerType, despawnClock)
        {
            m_drawableObj.Layer = GlobalVariables.GetLayer(ObjectLayer.Item);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            base.Update(gameTime, viewport);
            m_drawableObj.Update(gameTime);
        }

        //****************************************************
        // Method: SetStartingSpeed
        //
        // Purpose: Sets the starting velocity of the
        // object based on the ownerObj's facing direction
        //****************************************************
        public virtual void SetStartingSpeed(float speed)
        {
            if (m_ownerType == "Player")
            {
                PlayerObj tempPlayer = GameObjectHandler.FindPlayerObjByID(m_ownerID);
                if (tempPlayer != null)
                {
                    if (tempPlayer.Direction == Direction.Right)
                    {
                        m_physicsObj.VelX = speed;
                        SetPos(tempPlayer.PhysicsObj.Rect.Right + 2, tempPlayer.PhysicsObj.Rect.Top + 40 - m_physicsObj.Rect.Height / 2);
                    }
                    else
                    {
                        m_physicsObj.VelX = -speed;
                        SetPos(tempPlayer.PhysicsObj.Rect.Left - 2 - m_physicsObj.Rect.Width, tempPlayer.PhysicsObj.Rect.Top + 40 - m_physicsObj.Rect.Height / 2);
                    }
                }
            }
            else
            {
                m_physicsObj.VelX = speed;
            }
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
