//******************************************************
// File: FloatRectangle.cs
//
// Purpose: Contains the class definition of
// FloatRectangle. FloatRectangle is an
// implementation of a rectangle that uses floating
// point values to store its information.
// (Note: This rectangle only implements basic
//  properties because that's all I use it for.)
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

namespace AnimeFightClub001
{
    class FloatRectangle
    {

        #region Declarations
        protected float m_width; //Stores the width of the rectangle
        protected float m_height; //Stores the height of the rectangle
        protected float m_x;      //stores the x component of the position of the rectangle
        protected float m_y;      //stores the y component of the position of the rectangle
        protected Vector2 m_origin; //stores the center position of
        //the rectangle (used to prevent wasting cpu)
        #endregion

        //****************************************************
        // Method: FloatRectangle
        //
        // Purpose: FloatRectangle default constructor
        //****************************************************
        public FloatRectangle()
        {
            //Default initializations
            m_width = 0F;
            m_height = 0F;
            m_x = 0F;
            m_y = 0F;
            m_origin = Vector2.Zero;
        }

        //****************************************************
        // Method: FloatRectangle
        //
        // Purpose: FloatRectangle overloaded constructor
        //****************************************************
        public FloatRectangle(float X, float Y, float Width, float Height)
        {
            //Default initializations
            m_width = Width;
            m_height = Height;
            m_x = X;
            m_y = Y;
            m_origin = new Vector2(m_width / 2, m_height / 2);
        }

        //****************************************************
        // Method: SetDimensions
        //
        // Purpose: SetDimensions used to modify the
        // dimensions of the rectangle
        //****************************************************
        public void SetDimensions(float Width, float Height)
        {
            //Default initializations
            m_width = Width;
            m_height = Height;
            m_origin = new Vector2(m_width / 2, m_height / 2);
        }


        //Properties
        #region Properties
        public float X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }
        public float Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_x -= (value - m_width)/2;
                m_width = value;
                m_origin = new Vector2(m_width / 2, m_height / 2);
            }
        }
        public float Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_y -= (value - m_height) / 2;
                m_height = value;
                m_origin = new Vector2(m_width / 2, m_height / 2);
            }
        }

        public float Top
        {
            get
            {
                return m_y;
            }
        }

        public float Bottom
        {
            get
            {
                return m_y + m_height;
            }
        }

        public float Left
        {
            get
            {
                return m_x;
            }
        }

        public float Right
        {
            get
            {
                return m_x + m_width;
            }
        }
        public Vector2 Center
        {
            get
            {
                return new Vector2(m_origin.X + m_x, m_origin.Y + m_y);
            }
        }
        #endregion
    }
}
