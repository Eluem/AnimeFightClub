//******************************************************
// File: DeadPlayer.cs
//
// Purpose: An object used to display the death
// animation of players
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
    class DeadPlayer : SpecialEnvironmentalObj
    {
        #region Declarations
        protected string m_deathAnimation;
        protected int m_deathTimer;
        #endregion

        #region Constants
        const int DYING_TIME = 1000;
        const int DELETE_TIME = 5000;
        #endregion

        //****************************************************
        // Method: NetSendMe
        //
        // Purpose: Accepts a NetOutgoingMessage and writes
        // the approraite data to it
        //****************************************************
        public override void NetSendMe(NetOutgoingMessage sendMsg)
        {
            base.NetSendMe(sendMsg);
            sendMsg.Write(m_deathAnimation);
            sendMsg.Write(m_deathTimer);
        }

        //******************************************************
        // Method: DeadPlayer
        //
        // Purpose: Default DeadPlayer Constructor
        //******************************************************
        public DeadPlayer()
            : base()
        {
            m_physicsObj.Restitution = .1f;
            m_physicsObj.Mass = .25f;
            m_physicsObj.Friction = .9f;

            m_state = 0;
            m_deathTimer = 0;
            m_physicsObj.Rect.SetDimensions(40, 100);
        }

        //******************************************************
        // Method: DeadPlayer
        //
        // Purpose: Default DeadPlayer Constructor
        //******************************************************
        public DeadPlayer(PlayerObj player)
            : base()
        {
            m_physicsObj.Restitution = .1f;
            m_physicsObj.Mass = .25f;
            m_physicsObj.Friction = .9f;

            m_state = 0;
            m_deathTimer = 0;

            m_deathAnimation = player.Loadout.Avatar + "Death";
            m_physicsObj.Rect.SetDimensions(player.PhysicsObj.Rect.Width, player.PhysicsObj.Rect.Height);

            m_direction = player.Direction;

            m_drawableObj.SetDimensions(40, 100);

            m_physicsObj.PosX = player.Pos.X;
            m_physicsObj.PosY = player.Pos.Y;// +player.PhysicsObj.Rect.Height - 18;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            m_deathTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (m_deathTimer >= DYING_TIME)
            {
                float tempBottom = m_physicsObj.Rect.Bottom;

                m_physicsObj.Rect.SetDimensions(100, 18);

                m_physicsObj.PosY = tempBottom - 18;
                m_state = 1;
            }
            if (m_deathTimer >= DELETE_TIME)
            {
                Delete();
            }

            m_physicsObj.Update(gameTime, viewport);
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