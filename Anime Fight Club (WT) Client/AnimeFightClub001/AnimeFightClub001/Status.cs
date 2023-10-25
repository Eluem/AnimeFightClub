//******************************************************
// File: Status.cs
//
// Purpose: Defines the Status base class which will
// be inherited by all status effect classes.
//
// Written By: Rob Maggio, Salvatore Hanusiewicz
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
    class Status : iDrawableObj
    {
        #region Declarations
        protected StatusName m_name;
        protected int m_severity; //May not apply to all statuses.
        protected int m_duration; //Duration of the status in milliseconds.
        protected int m_tick;
        protected int m_timer;
        protected bool m_active;
        protected UInt16 m_statusID;
        protected PlayerObj m_ownerObj;
        protected PlayerObj m_inflictorObj;
        protected Vector2 m_offSet;
        protected int m_state;
        protected int m_prevState;

        protected Sprite m_drawableObj;
        #endregion

        //****************************************************
        // Method: Status
        //
        // Purpose: Constructor for Status
        //****************************************************
        public Status(UInt16 statusID, PlayerObj inflictorObj, PlayerObj ownerObj, StatusName name, int severity = 0, int duration = 0, int tick = 500, string spriteName = "None", ObjectLayer objectLayer = ObjectLayer.StatusAbove)
        {
            m_statusID = statusID;
            m_ownerObj = ownerObj;
            m_inflictorObj = inflictorObj;
            m_name = name;
            m_severity = severity;
            m_duration = duration;
            m_tick = tick;
            m_timer = 0;

            m_offSet = Vector2.Zero;

            m_state = 0;
            m_prevState = 0;

            m_drawableObj = new Sprite(spriteName, Color.White, this, GlobalVariables.GetLayer(objectLayer));
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Reduces duration. 
        //****************************************************
        public void Update(GameTime gameTime)
        {
            m_duration -= gameTime.ElapsedGameTime.Milliseconds;
            m_timer += gameTime.ElapsedGameTime.Milliseconds;

            if (m_timer >= m_tick)
            {
                m_timer -= m_tick;
                TickHook(gameTime);
            }


            UpdateHook(gameTime);
            m_drawableObj.Update(gameTime);

            m_prevState = m_state;
        }


        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the end of the update
        // function
        //****************************************************
        public virtual void UpdateHook(GameTime gameTime)
        {

        }

        //****************************************************
        // Method: TickHook
        //
        // Purpose: Hooks into the part of the update
        // function that decreases the timer
        //****************************************************
        public virtual void TickHook(GameTime gameTime)
        {

        }

        //****************************************************
        // Method: WearOffHook
        //
        // Purpose: Hooks into the part of ApplyStatusEffects
        // function that removes the status due to
        // it's duration expiring
        //****************************************************
        public virtual void WearOffHook(GameTime gameTime)
        {

        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the object and anything else
        // that it may need to draw.
        //****************************************************
        public void Draw(SpriteBatch spriteBatch)
        {
            m_drawableObj.Draw(spriteBatch);
        }

        #region Properties
        public int Severity
        {
            get
            {
                return m_severity;
            }
        }

        public int Duration
        {
            get
            {
                return m_duration;
            }
            set
            {
                m_duration = value;
            }
        }

        public StatusName Name
        {
            get
            {
                return m_name;
            }
        }

        public PlayerObj OwnerObj
        {
            get
            {
                return m_ownerObj;
            }
        }

        public PlayerObj InflictorObj
        {
            get
            {
                return m_inflictorObj;
            }
            set
            {
                m_inflictorObj = value;
            }
        }

        public UInt16 StatusID
        {
            get
            {
                return m_statusID;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(m_ownerObj.Center.X + m_offSet.X, m_ownerObj.Center.Y + m_offSet.Y);
            }
        }

        public Direction Direction
        {
            get
            {
                return Direction.Left;
            }
            set
            {
            }
        }

        public int State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }

        public int PrevState
        {
            get
            {
                return m_prevState;
            }
        }
        #endregion

    }
}
