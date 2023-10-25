//******************************************************
// File: ItemBomb.cs
//
// Purpose: Defines ItemBomb.
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
    class ItemBomb : ItemObj
    {
        #region Declarations
        protected int m_fuseTime;
        #endregion
        //*******************************************************
        // Method: ItemBomb
        //
        // Purpose: Default constructor for ItemBomb
        //*******************************************************
        public ItemBomb()
            : base()
        {
            m_fuseTime = 2000;
            m_physicsObj.Rect.SetDimensions(45, 55);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = .6f;
            m_physicsObj.Friction = .6f;
            m_physicsObj.Mass = 5;
        }

        //****************************************************
        // Method: ItemBomb
        //
        // Purpose: Constructor for ItemBomb
        //****************************************************
        public ItemBomb(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType)
        {
            m_fuseTime = 2000;
            m_direction = GameObjectHandler.FindPlayerObjByID(ownerID).Direction;
            m_physicsObj.Rect.SetDimensions(45, 55);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = .6f;
            m_physicsObj.Friction = .6f;
            m_physicsObj.Mass = 5;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            base.Update(gameTime, viewport);
            m_fuseTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (m_fuseTime <= 0)
            {
                Explode();
            }
        }

        //****************************************************
        // Method: Explode
        //
        // Purpose: Makes the bomb explode
        //****************************************************
        public virtual void Explode()
        {
            Delete();
            HazardBombExplosion added = new HazardBombExplosion(0, m_ownerID, "Player");
            added.SetPos(m_physicsObj.Rect.Center.X - added.PhysicsObj.Rect.Width / 2, m_physicsObj.Rect.Center.Y - added.PhysicsObj.Rect.Height / 2);
            GameObjectHandler.AddObject(added);
        }
    }
}
