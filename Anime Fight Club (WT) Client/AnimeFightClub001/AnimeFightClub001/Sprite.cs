//******************************************************
// File: Sprite.cs
//
// Purpose: Contains the class definition for Sprite.
// Sprite is a DrawableObj which is animated.
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
    class Sprite : DrawableObj
    {
        #region Declarations
        protected Point m_currentFrame; //The X and Y position of your current frame
        protected int m_currentFrameTime;   //Amount of time spent at current frame
        #endregion

        //****************************************************
        // Method: Sprite
        //
        // Purpose: Sprite constructor
        //****************************************************
        public Sprite(string ImageName, Color Color, iDrawableObj ownerObj, float layer)
            : base(ImageName, Color, ownerObj, layer)
        {
            m_sourceRect = new Rectangle(0, 0, GlobalVariables.ImageDict[ImageName].Width, GlobalVariables.ImageDict[ImageName].Height);

            m_origin = new Vector2((float)m_sourceRect.Width / 2, (float)m_sourceRect.Height / 2);

            m_currentFrameTime = 0;
            m_currentFrame = Point.Zero;
            m_rotation = 0f;

            m_currentFrameTime = 0;
            m_currentFrame.X = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].X;
            m_currentFrame.Y = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].Y;
            m_sourceRect.X = m_currentFrame.X * m_sourceRect.Width;
            m_sourceRect.Y = m_currentFrame.Y * m_sourceRect.Height;
        }

        public override void SetImage(string imageName)
        {
            base.SetImage(imageName);

        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the drawableObj based on
        // the gametime (generally only used for sprites)
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            #region Sprite Loop
            m_currentFrameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (m_ownerObj.State != m_ownerObj.PrevState)
            {
                m_currentFrameTime = 0;
                m_currentFrame.X = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].X;
                m_currentFrame.Y = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].Y;
                m_sourceRect.X = m_currentFrame.X * m_sourceRect.Width;
                m_sourceRect.Y = m_currentFrame.Y * m_sourceRect.Height;
            }

            if (m_currentFrameTime >= GlobalVariables.ImageDict[m_imageName].AnimationLoopFrameDuration[m_ownerObj.State])
            {
                if (GlobalVariables.ImageDict[m_imageName].IsLoopForward(m_ownerObj.State))
                {
                    #region Forward Animation
                    ++m_currentFrame.X;
                    if (m_currentFrame.X > GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][1].X && m_currentFrame.Y >= GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][1].Y)
                    {
                        m_currentFrame.X = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].X;
                        m_currentFrame.Y = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].Y;
                    }
                    else if (m_currentFrame.X >= GlobalVariables.ImageDict[m_imageName].SheetSize.X)
                    {
                        ++m_currentFrame.Y;
                        if (m_currentFrame.Y >= GlobalVariables.ImageDict[m_imageName].SheetSize.Y)
                        {
                            m_currentFrame.Y = 0;
                        }
                        m_currentFrame.X = 0;
                    }
                    #endregion
                }
                else
                {
                    #region Backward Animation
                    --m_currentFrame.X;
                    if (m_currentFrame.X < GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][1].X && m_currentFrame.Y <= GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][1].Y)
                    {
                        m_currentFrame.X = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].X;
                        m_currentFrame.Y = GlobalVariables.ImageDict[m_imageName].AnimationLoops[m_ownerObj.State][0].Y;
                    }
                    else if (m_currentFrame.X < 0)
                    {
                        --m_currentFrame.Y;
                        if (m_currentFrame.Y < 0)
                        {
                            m_currentFrame.Y = GlobalVariables.ImageDict[m_imageName].SheetSize.Y - 1;
                        }
                        m_currentFrame.X = GlobalVariables.ImageDict[m_imageName].SheetSize.X - 1;
                    }
                    #endregion
                }

                m_currentFrameTime = 0;

                m_sourceRect.X = m_currentFrame.X * m_sourceRect.Width;
                m_sourceRect.Y = m_currentFrame.Y * m_sourceRect.Height;
            }

            #endregion
            #region Handle Object Changes
            if (m_ownerObj.Direction == Direction.Left)
            {
                m_spriteEffect = SpriteEffects.None;
            }
            else if (m_ownerObj.Direction == Direction.Right)
            {
                m_spriteEffect = SpriteEffects.FlipHorizontally;
            }
            #endregion

        }

        #region Properties

        #endregion
    }
}
