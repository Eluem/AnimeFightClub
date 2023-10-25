//******************************************************
// File: ImageInfo.cs
//
// Purpose: Contains the class definition for ImageInfo.
// ImageInfo contains the basic information about any
// given image. It is used to store all the different
// "Texture2d" data, along with information about
// these textures.
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
    class ImageInfo
    {
        #region Declarations
        protected Texture2D m_texture;
        protected Point m_frameSize;
        protected Point m_sheetSize;
        protected List<Point[]> m_animationLoops;
        protected List<int> m_animationLoopFrameDuration;
        #endregion


        //****************************************************
        // Method: ImageInfo
        //
        // Purpose: ImageInfo Constructor
        //****************************************************
        public ImageInfo(Texture2D texture)
        {
            m_texture = texture;
            m_frameSize = new Point(texture.Width, texture.Height);
            m_sheetSize = new Point(1, 1);
            m_animationLoops = null;
            m_animationLoopFrameDuration = null;
        }


        //****************************************************
        // Method: ImageInfo
        //
        // Purpose: ImageInfo Constructor
        //****************************************************
        public ImageInfo(Texture2D texture, Point frameSize, int frameDuration)
        {
            m_texture = texture;
            m_frameSize = frameSize;

            //Default sheet size algorithm
            if (m_frameSize.X != 0 && m_frameSize.Y != 0)
                m_sheetSize = new Point(m_texture.Width / m_frameSize.X, m_texture.Height / m_frameSize.Y);
            else
                m_sheetSize = Point.Zero;

            m_animationLoops = new List<Point[]>();
            m_animationLoops.Add(new Point[2] { Point.Zero, m_sheetSize });

            m_animationLoopFrameDuration = new List<int>();
            m_animationLoopFrameDuration.Add(frameDuration);
        }


        //****************************************************
        // Method: ImageInfo
        //
        // Purpose: ImageInfo Constructor
        //****************************************************
        public ImageInfo(Texture2D texture, Point frameSize, List<Point[]> animationLoops, int frameDuration)
        {
            m_texture = texture;
            m_frameSize = frameSize;

            //Default sheet size algorithm
            m_sheetSize = new Point(m_texture.Width / m_frameSize.X, m_texture.Height / m_frameSize.Y);

            m_animationLoops = animationLoops;

            m_animationLoopFrameDuration = new List<int>();
            for (int i = 0; i < m_animationLoops.Count; ++i)
            {
                m_animationLoopFrameDuration.Add(frameDuration);
            }
        }

        //****************************************************
        // Method: ImageInfo
        //
        // Purpose: ImageInfo Constructor
        //****************************************************
        public ImageInfo(Texture2D texture, Point frameSize, List<Point[]> animationLoops, List<int> frameDurationList)
        {
            m_texture = texture;
            m_frameSize = frameSize;

            //Default sheet size algorithm
            m_sheetSize = new Point(m_texture.Width / m_frameSize.X, m_texture.Height / m_frameSize.Y);

            m_animationLoops = animationLoops;

            m_animationLoopFrameDuration = frameDurationList;
        }

        //****************************************************
        // Method: GetLoopFrameCount
        //
        // Purpose: Returns the number of frames that exist
        // for a given animation loop
        //****************************************************
        public int GetLoopFrameCount(int loopIndex)
        {
            //return ((((m_animationLoops[loopIndex][1].Y - AnimationLoops[loopIndex][0].Y) + 1) * m_sheetSize.X) - (m_sheetSize.X - (m_sheetSize.X - m_animationLoops[loopIndex][0].X)) - (m_sheetSize.X - (m_animationLoops[loopIndex][1].X + 1)));
            return ((((m_animationLoops[loopIndex][1].Y - m_animationLoops[loopIndex][0].Y) + 1) * m_sheetSize.X) - m_animationLoops[loopIndex][0].X - (m_sheetSize.X - (m_animationLoops[loopIndex][1].X + 1)));
        }

        //****************************************************
        // Method: IsLoopForward
        //
        // Purpose: Returns true if the loop's first frame
        // is before it's last frame. Returns false otherwise.
        // This is used to determine if the loop is meant to
        // be played forward or backwards.
        //****************************************************
        public bool IsLoopForward(int loopIndex)
        {
            return ((m_animationLoops[loopIndex][0].Y * m_sheetSize.X) + m_animationLoops[loopIndex][0].X) <= ((m_animationLoops[loopIndex][1].Y * m_sheetSize.X) + m_animationLoops[loopIndex][1].X);
        }


        #region Properties
        public Texture2D Texture
        {
            get
            {
                return m_texture;
            }
            set
            {
                m_texture = value;
            }
        }

        public Point FrameSize
        {
            get
            {
                return m_frameSize;
            }
            set
            {
                m_frameSize = value;
            }
        }

        public Point SheetSize
        {
            get
            {
                return m_sheetSize;
            }
            set
            {
                m_sheetSize = value;
            }
        }

        public List<Point[]> AnimationLoops
        {
            get
            {
                return m_animationLoops;
            }
        }

        public List<int> AnimationLoopFrameDuration
        {
            get
            {
                return m_animationLoopFrameDuration;
            }
        }

        public int Height
        {
            get
            {
                return m_frameSize.Y;
            }
        }

        public int Width
        {
            get
            {
                return m_frameSize.X;
            }
        }

        #endregion
    }
}
