//******************************************************
// File: DrawableObj.cs
//
// Purpose: Contains the class definition for
// DrawableObj. DrawableObj will be inherited by any
// class that will control actual drawing actions.
// Any game object which should be drawn to the screen
// should declare an instance of Drawable
// or some child of drawable.
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
    class DrawableObj
    {
        #region Declarations
        protected string m_imageName;
        protected Color m_color;
        protected Rectangle m_sourceRect;
        protected float m_rotation;
        protected Vector2 m_scale;
        protected Vector2 m_origin;
        protected SpriteEffects m_spriteEffect;
        protected float m_layer;
        protected Point m_setDimensions; //Stores the last dimensions that were set. Defaults to the dimensions of the image.
        protected iDrawableObj m_ownerObj; //Stores a reference to the object that this image belongs to
        #endregion

        //****************************************************
        // Method: DrawableObj
        //
        // Purpose: Default DrawableObj constructor
        //****************************************************
        public DrawableObj(iDrawableObj basicObj, float layer)
        {
            m_ownerObj = basicObj;

            m_imageName = "Default";
            m_color = Color.Wheat;


            m_setDimensions = new Point(GlobalVariables.ImageDict[m_imageName].Width, GlobalVariables.ImageDict[m_imageName].Height);

            m_sourceRect = new Rectangle(0, 0, GlobalVariables.ImageDict[m_imageName].Width, GlobalVariables.ImageDict[m_imageName].Height);
            m_origin = new Vector2((float)m_sourceRect.Width / 2, (float)m_sourceRect.Height / 2);

            m_rotation = 0f;
            m_scale = new Vector2(1, 1);
            m_spriteEffect = SpriteEffects.None;
            m_layer = layer;
        }

        //****************************************************
        // Method: DrawableObj
        //
        // Purpose: DrawableObj constructor
        //****************************************************
        public DrawableObj(string imageName, Color color, iDrawableObj basicObj, float layer)
        {
            m_layer = layer;

            m_ownerObj = basicObj;

            m_imageName = imageName;
            m_color = color;
            
            m_sourceRect = new Rectangle(0, 0, GlobalVariables.ImageDict[m_imageName].Width, GlobalVariables.ImageDict[m_imageName].Height);
            m_origin = new Vector2((float)m_sourceRect.Width / 2, (float)m_sourceRect.Height / 2);

            m_rotation = 0f;
            m_scale = new Vector2(1, 1);
            m_spriteEffect = SpriteEffects.None;
            m_layer = layer;
        }


        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the drawableObj based on
        // the gametime (generally only used for sprites)
        //****************************************************
        public virtual void Update(GameTime gameTime)
        {
        }


        //****************************************************
        // Method: SetDimensions
        //
        // Purpose: Changes the dimensions of the rectangle.
        //****************************************************
        public virtual void SetDimensions(int Width, int Height)
        {

            m_setDimensions = new Point(Width, Height);

            m_scale.X = Width / (float)GlobalVariables.ImageDict[m_imageName].Width;
            m_scale.Y = Height / (float)GlobalVariables.ImageDict[m_imageName].Height;
        }

        //****************************************************
        // Method: SetImage
        //
        // Purpose: Changes the image selected, and keeps
        // the same dimensions as before.
        //****************************************************
        public virtual void SetImage(string imageName)
        {
            m_imageName = imageName;
            SetDimensions(m_setDimensions.X, m_setDimensions.Y); //Change the scaling to preserve the dimensions
        }




        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the appropraite image to the
        // rectangle, with the appropriate color
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GlobalVariables.ImageDict[m_imageName].Texture, m_ownerObj.Center, m_sourceRect, m_color, m_rotation, m_origin, m_scale, m_spriteEffect, m_layer);
        }

        #region Properties
        public string ImageName
        {
            get
            {
                return m_imageName;
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

        public Vector2 Scale
        {
            get
            {
               return m_scale;
            }
            set
            {
                m_scale = value;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return m_origin;
            }
            set
            {
                m_origin = value;
            }
        }

        public SpriteEffects SpriteEffect
        {
            get
            {
                return m_spriteEffect;
            }
            set
            {
                m_spriteEffect = value;
            }
        }

        public float Layer
        {
            get
            {
                return m_layer;
            }
            set
            {
                m_layer = value;
            }
        }

        public Rectangle SourceRect
        {
            get
            {
                return m_sourceRect;
            }
            set
            {
                m_sourceRect = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        public BasicObj OwnerObj
        {
            get
            {
                return (BasicObj)m_ownerObj;
            }
            set
            {
                m_ownerObj = value;
            }
        }
        #endregion
    }
}
