//*************************************************************************************
// File: Camera2D.cs
//
// Purpose:  Contains the definition of the Camera2D class. This class will be used
// to allow for the tracking of players or other objects zooming in and out
// (mostly for use depending on resolutions). Also, eventually rotation.
//
// Written By: Salvatore Hanusiewicz (Slightly modified tutorial)
// http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
//*************************************************************************************

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
    class Camera2D
    {
        #region Declarations
        protected float m_zoom; // Camera Zoom
        protected float m_rotation; // Camera Rotation
        protected Vector2 m_pos; // Camera Position

        protected Matrix m_transform; // Matrix Transform

        protected PhysicsObj m_trackingObj; //The physics object of the object which is being tracked
        #endregion


        //****************************************************
        // Method: Camera2D
        //
        // Purpose: Default Constructor for Camera2D
        //****************************************************
        public Camera2D()
        {
            m_zoom = 1.0f;
            m_rotation = 0.0f;
            m_pos = Vector2.Zero;
        }


        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the position of the Camera2D
        // class based on the object that it is tracking
        //****************************************************
        public void Update()
        {
            if (TrackingObj.Rect.Center.X > m_pos.X + 30)
            {
                m_pos.X = TrackingObj.Rect.Center.X - 30;
            }
            else if (TrackingObj.Rect.Center.X < m_pos.X - 30)
            {
                m_pos.X = TrackingObj.Rect.Center.X + 30;
            }

            if (TrackingObj.Rect.Center.Y > m_pos.Y + 30)
            {
                m_pos.Y = TrackingObj.Rect.Center.Y - 30;
            }
            else if (TrackingObj.Rect.Center.Y < m_pos.Y - 30)
            {
                m_pos.Y = TrackingObj.Rect.Center.Y + 30;
            }
        }


        //****************************************************
        // Method: GetTransformation
        //
        // Purpose: Gets the transformation to be applied
        // to the sprite batch so that it positions, scales
        // and rotates it correctly.
        //****************************************************
        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {     
            //**************************************
            // Thanks to o KB o for this solution
            //**************************************
            
            /*
              Matrix.CreateTranslation(new Vector3((graphicsDevice.Viewport.Width * 0.5f) - (m_pos.X *m_zoom), (graphicsDevice.Viewport.Height * 0.5f) - (m_pos.Y * m_zoom), 0)) *
                                         Matrix.CreateRotationZ(m_rotation) *
                                         Matrix.CreateScale(new Vector3(m_zoom, m_zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            */

            m_transform = Matrix.CreateTranslation(new Vector3(-m_pos.X, -m_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(0) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return m_transform;
        }

        #region Properties
        public float Zoom
        {
            get
            {
                return m_zoom;
            }

            set
            {
                m_zoom = value;
            }
        }
        public float Rotation
        {
            get
            {
                return m_rotation;
            }

            set
            {
                m_rotation = value;
            }
        }
        public Vector2 Pos
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }
        public Matrix Transformation
        {
            get
            {
                return m_transform;
            }

            set
            {
                m_transform = value;
            }
        }
        public PhysicsObj TrackingObj
        {
            get
            {
                return m_trackingObj;
            }
            set
            {
                m_trackingObj = value;
            }
        }
        #endregion


    }
}
