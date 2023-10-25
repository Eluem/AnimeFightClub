//******************************************************
// File: MenuItem.cs
//
// Purpose: Defines a MenuItem. MenuItems will be
// analogous to Controls in winforms.
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
    public delegate void BasicMenuItemEventHandler(object sender);

    enum BorderSide { Left, Right, Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight };
    class MenuItem
    {
        #region Declarations
        protected Game1 m_game1; //Gives access to all information in game1

        protected bool m_isSelected; //Whether or not the MenuItem is "selected"

        protected bool m_hasFocus; //Whether or not the MenuItem has control focus
        protected bool m_hadFocus = false; //Used to determine if you had focus last frame

        protected string m_name; //The name of the MenuItems
        protected MenuItem m_ownerItem; //The item that spawned this item

        protected MenuItem m_gaveFocus; //The menuItem that gave this item focus (useful for bread crumb backing)

        protected List<MessageBox> m_messageBoxList; //List of message boxes which are automatically drawn on top of all content

        #region Text
        protected string m_text;
        protected Vector2 m_textPos;
        protected string m_font;
        protected Color m_fontColor;
        protected Color m_fontColorFocus;
        #endregion

        public int[] m_innerShadowSize; //The width/height of shadows that come into the content area of the left,right,top, and bottom

        #region Background
        //Determines whether the background should be stretched fully, or just to the bounds of the content
        //true = fully, false = content. Default = false (menus default to true)
        protected bool m_bkgFullStretch;

        protected string m_bkgImage; //The name of the background image
        protected string m_bkgImageFocus; //The name of the background image when this has focus

        protected Color m_bkgColor; //The color of the background
        protected Color m_bkgColorFocus; //The color of the background when it has focus
        #endregion

        #region Border
        protected string[] m_borderImage; //The names of the pieces of the border images
        protected string[] m_borderImageFocus; //The name of the pieces of the border images when this has focus

        protected Color m_borderColor; //The color of the background
        protected Color m_borderColorFocus; //The color of the background when it has focus
        #endregion

        #region Outline
        protected Point m_outlineOffset; //The offset of the outline (Mostly used for when menu items have a drop shadow)
        protected string[] m_outlineImage; //The names of the pieces of the outline images
        protected Color m_outlineColor; //The color of the outline
        #endregion


        //The rectangle within which the MenuItem will be drawn (uses viewport, relative to whatever the current viewport is)
        protected Rectangle m_rect;
        #endregion

        //****************************************************
        // Method: MenuItem
        //
        // Purpose: Default constructor for MenuItem
        //****************************************************
        public MenuItem(MenuItem ownerItem, Game1 game1)
        {
            m_ownerItem = ownerItem;
            m_game1 = game1;
            m_name = "";

            m_rect = new Rectangle(0, 0, 800, 600);

            m_bkgFullStretch = false;
            m_bkgImage = "None";
            m_bkgImageFocus = "None";
            m_bkgColor = Color.Wheat;
            m_bkgColorFocus = Color.Wheat;

            m_borderImage = new string[8] { "None", "None", "None", "None", "None", "None", "None", "None" };
            m_borderImageFocus = m_borderImage;
            m_borderColor = m_bkgColor;
            m_borderColorFocus = m_bkgColor;

            #region Outline
            m_outlineOffset = Point.Zero;

            m_outlineImage = new string[8];
            string[] tempStringArray = new string[8] { "_L", "_R", "_T", "_B", "_TL", "_TR", "_BL", "_BR" };
            string tempString = "Outline001";
            for (int i = 0; i < 8; ++i)
            {
                m_outlineImage[i] = tempString + tempStringArray[i];
            }
            m_outlineColor = Color.Wheat;
            #endregion

            m_isSelected = false;

            m_hasFocus = false;
            m_hadFocus = false;

            m_textPos = Vector2.Zero;
            m_font = "Penshurst40";
            m_text = "";

            m_innerShadowSize = new int[4] { 0, 0, 0, 0 };
        }

        //****************************************************
        // Method: MenuItem
        //
        // Purpose: Constructor for MenuItem
        //****************************************************
        public MenuItem(MenuItem ownerItem, Game1 game1, string name, Rectangle drawRect, string backgroundImage, string backgroundImageFocus, Color backgroundColor, Color backgroundColorFocus, string[] borderImage, string[] borderImageFocus, Color borderColor, Color borderColorFocus, bool startingFocus = false, bool bkgFullStretch = false)
        {
            if (borderImage.Length != 8)
                throw new System.ArgumentException("Must have 8 elements", "borderImage");
            
            m_ownerItem = ownerItem;
            m_game1 = game1;

            m_name = name;

            m_rect = drawRect;

            m_bkgFullStretch = bkgFullStretch;
            m_bkgImage = backgroundImage;
            m_bkgImageFocus = backgroundImageFocus;
            m_bkgColor = backgroundColor;
            m_bkgColorFocus = backgroundColorFocus;

            m_borderImage = borderImage;
            m_borderImageFocus = borderImageFocus;
            m_borderColor = borderColor;
            m_borderColorFocus = borderColorFocus;

            #region Outline
            m_outlineOffset = Point.Zero;

            m_outlineImage = new string[8];
            string[] tempStringArray = new string[8] { "_L", "_R", "_T", "_B", "_TL", "_TR", "_BL", "_BR" };
            string tempString = "Outline001";
            for (int i = 0; i < 8; ++i)
            {
                m_outlineImage[i] = tempString + tempStringArray[i];
            }
            m_outlineColor = Color.Wheat;
            #endregion

            m_isSelected = false;

            m_hasFocus = startingFocus;
            m_hadFocus = false;

            m_textPos = Vector2.Zero;
            m_font = "Penshurst40";
            m_text = "";

            m_innerShadowSize = new int[4] { 0, 0, 0, 0 };
        }

        //****************************************************
        // Method: MenuItem
        //
        // Purpose: Constructor for MenuItem
        //****************************************************
        public MenuItem(MenuItem ownerItem, Game1 game1, string name, Rectangle drawRect, string backgroundImage, Color backgroundColor, Color backgroundColorFocus, string borderImage, Color borderColor, Color borderColorFocus, bool startingFocus = false, bool bkgFullStretch = false)
        {
            m_ownerItem = ownerItem;
            m_game1 = game1;
            m_name = name;

            m_rect = drawRect;

            m_bkgFullStretch = bkgFullStretch;
            m_bkgImage = backgroundImage;
            m_bkgImageFocus = backgroundImage + "_Focus";
            m_bkgColor = backgroundColor;
            m_bkgColorFocus = backgroundColorFocus;

            #region Autogenerate Border Image Names from Base Name
            m_borderImage = new string[8];
            m_borderImageFocus = new string[8];

            m_outlineOffset = Point.Zero;
            m_outlineImage = new string[8];
            string tempString = "Outline001";

            string[] tempStringArray = new string[8] { "_L", "_R", "_T", "_B", "_TL", "_TR", "_BL", "_BR" };
            for (int i = 0; i < 8; ++i)
            {
                m_borderImage[i] = borderImage + tempStringArray[i];
                m_borderImageFocus[i] = borderImage + "_Focus" + tempStringArray[i];
                m_outlineImage[i] = tempString + tempStringArray[i];

                if (!GlobalVariables.ImageDict.ContainsKey(m_borderImage[i]) || !GlobalVariables.ImageDict.ContainsKey(m_borderImageFocus[i]))
                {
                    throw new System.ArgumentException("Cannot autogenerate names from the base name: " + borderImage, borderImage);
                }
            }
            #endregion

            m_outlineColor = Color.Wheat;

            m_borderColor = borderColor;
            m_borderColorFocus = borderColorFocus;

            m_hasFocus = startingFocus;
            m_hadFocus = false;

            m_textPos = Vector2.Zero;
            m_font = "Penshurst40";
            m_text = "";

            m_innerShadowSize = new int[4] { 0, 0, 0, 0 };
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the MenuItem within the correct
        // viewport.
        // This is meant to be accessed via three "hooks"
        // named DrawBackground, DrawBorder, and DrawContent.
        // However, it can be overwritten.
        //
        //(Note: Don't begin spritebatch before calling this.)
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            #region Create and Backup Viewports
            Viewport bkViewport = graphicsDevice.Viewport;
            Viewport contentViewport = graphicsDevice.Viewport;

            #region Calculate Content Viewport

            #region Determine Width and Height taken by Border
            int[] tempWidth;
            int[] tempHeight;
            if (m_hasFocus)
            {
                tempWidth = new int[2] { GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Width - m_innerShadowSize[(int)BorderSide.Left], GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Width - m_innerShadowSize[(int)BorderSide.Right] };
                tempHeight = new int[2] { GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Height - m_innerShadowSize[(int)BorderSide.Top], GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Height - m_innerShadowSize[(int)BorderSide.Bottom] };
            }
            else
            {
                tempWidth = new int[2] { GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Left]].Width - m_innerShadowSize[(int)BorderSide.Left], GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Right]].Width - m_innerShadowSize[(int)BorderSide.Right] };
                tempHeight = new int[2] { GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Top]].Height - m_innerShadowSize[(int)BorderSide.Top], GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Bottom]].Height - m_innerShadowSize[(int)BorderSide.Bottom] };
            }
            #endregion

            Rectangle tempRect = new Rectangle(
                (int)MathHelper.Clamp((float)(offset.X + m_rect.X + tempWidth[0]  + bkViewport.Bounds.X), (float)bkViewport.Bounds.X, (float)(bkViewport.Bounds.X + bkViewport.Bounds.Width)),
                (int)MathHelper.Clamp((float)(offset.Y + m_rect.Y + tempHeight[0] + bkViewport.Bounds.Y), (float)bkViewport.Bounds.Y, (float)(bkViewport.Bounds.Y + bkViewport.Bounds.Height)),
                m_rect.Width - tempWidth[0] - tempWidth[1],
                m_rect.Height - tempHeight[0] - tempHeight[1]);
            int tempWidthDiff = (tempRect.Right - bkViewport.Bounds.Right);
            if (tempWidthDiff < 0)
            {
                tempWidthDiff = 0;
            }
            tempRect.Width = tempRect.Width - tempWidthDiff;

            if (m_rect.X + tempWidth[0] < 0)
                tempRect.Width += m_rect.X + tempWidth[0];

            if (tempRect.Width < 0)
                tempRect.Width = 0;

            int tempHeightDiff = (tempRect.Bottom - bkViewport.Bounds.Bottom);
            if (tempHeightDiff < 0)
            {
                tempHeightDiff = 0;
            }
            tempRect.Height = tempRect.Height - tempHeightDiff;

            if (m_rect.Y + tempHeight[0] < 0)
                tempRect.Height += m_rect.Y + tempHeight[0];

            if (tempRect.Height < 0)
                tempRect.Height = 0;

            contentViewport.Bounds = tempRect;
            #endregion

            #endregion

            #region Draw Background
            spriteBatch.Begin();
            DrawBackground(spriteBatch, graphicsDevice, offset);
            spriteBatch.End();
            #endregion

            #region Draw Content
            if (contentViewport.Width > 0 && contentViewport.Height > 0)
            {
                graphicsDevice.Viewport = contentViewport;
                
                Point tempOffset = Point.Zero;
                if(m_rect.X + tempWidth[0] < 0)
                    tempOffset.X = m_rect.X + tempWidth[0];
                if(m_rect.Y + tempHeight[0] < 0)
                    tempOffset.Y = m_rect.Y + tempHeight[0];
                
                DrawContent(spriteBatch, graphicsDevice, new Point(offset.X + tempOffset.X, offset.Y + tempOffset.Y));
            }
            #endregion

            graphicsDevice.Viewport = bkViewport; //Restore original viewport
            #region Draw Border
            spriteBatch.Begin();
            DrawBorder(spriteBatch, graphicsDevice, offset);
            spriteBatch.End();
            #endregion
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws the "content" in the MenuItem.
        // Defaults to drawing the MenuItem's text
        //****************************************************
        public virtual void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            if (m_hasFocus)
            {
                spriteBatch.DrawString(GlobalVariables.FontDict[m_font], m_text, new Vector2(m_textPos.X + offset.X, m_textPos.Y + offset.Y), m_fontColorFocus);
            }
            else
            {
                spriteBatch.DrawString(GlobalVariables.FontDict[m_font], m_text, new Vector2(m_textPos.X + offset.X, m_textPos.Y + offset.Y), m_fontColor);
            }
            spriteBatch.End();
        }

        //****************************************************
        // Method: DrawBackground
        //
        // Purpose: Draws the background of the MenuItem
        //****************************************************
        public virtual void DrawBackground(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            int[] contentBoundsOffset = new int[4]{0,0,0,0};
            if (!m_bkgFullStretch)
            {
                if(m_hasFocus)
                {
                    contentBoundsOffset[0] = GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Width;
                    contentBoundsOffset[1] = GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Height;
                    contentBoundsOffset[2] = -(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Width + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Width);
                    contentBoundsOffset[3] = -(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Height + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Height);
                }
                else
                {
                    contentBoundsOffset[0] = GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Left]].Width;
                    contentBoundsOffset[1] = GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Top]].Height;
                    contentBoundsOffset[2] = -(GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Left]].Width + GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Right]].Width);
                    contentBoundsOffset[3] = -(GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Top]].Height + GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Bottom]].Height);
                }
            }
            if (m_hasFocus)
            {
                spriteBatch.Draw(GlobalVariables.ImageDict[m_bkgImageFocus].Texture, new Rectangle(m_rect.X + offset.X + contentBoundsOffset[0] - m_innerShadowSize[(int)BorderSide.Left], m_rect.Y + offset.Y + contentBoundsOffset[1] - m_innerShadowSize[(int)BorderSide.Top], m_rect.Width + contentBoundsOffset[2] + m_innerShadowSize[(int)BorderSide.Left] + m_innerShadowSize[(int)BorderSide.Right], m_rect.Height + contentBoundsOffset[3] + m_innerShadowSize[(int)BorderSide.Bottom] + m_innerShadowSize[(int)BorderSide.Top]), m_bkgColorFocus);
            }
            else
            {
                spriteBatch.Draw(GlobalVariables.ImageDict[m_bkgImage].Texture, new Rectangle(m_rect.X + offset.X + contentBoundsOffset[0] - m_innerShadowSize[(int)BorderSide.Left], m_rect.Y + offset.Y + contentBoundsOffset[1] - m_innerShadowSize[(int)BorderSide.Top], m_rect.Width + contentBoundsOffset[2] + m_innerShadowSize[(int)BorderSide.Left] + m_innerShadowSize[(int)BorderSide.Right], m_rect.Height + contentBoundsOffset[3] + m_innerShadowSize[(int)BorderSide.Bottom] + m_innerShadowSize[(int)BorderSide.Top]), m_bkgColor);
            }
        }

        //****************************************************
        // Method: DrawBorder
        //
        // Purpose: Draws the border of the MenuItem.
        // Always draws in 8 pieces
        //****************************************************
        public virtual void DrawBorder(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            #region Draw Outline
            if (m_isSelected)
            {
                DrawOutline(spriteBatch, graphicsDevice, offset);
            }
            #endregion

            #region Draw Border
            if (m_hasFocus)
            {
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Height, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Width, m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Width, offset.Y + m_rect.Y + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Height, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Width, m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Texture, new Rectangle(offset.X + m_rect.X + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Width, offset.Y + m_rect.Y, m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Texture, new Rectangle(offset.X + m_rect.X + GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Width, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Height, m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Height), m_borderColorFocus);

                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopLeft]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Width, offset.Y + m_rect.Y, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.TopRight]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Height, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomLeft]].Height), m_borderColorFocus);
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Width, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Height, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Width, GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.BottomRight]].Height), m_borderColorFocus);
            }
            else
            {

                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Left]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y + GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Height, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Left]].Width, m_rect.Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Right]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Right]].Width, offset.Y + m_rect.Y + GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Height, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Right]].Width, m_rect.Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Top]].Texture, new Rectangle(offset.X + m_rect.X + GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Width, offset.Y + m_rect.Y, m_rect.Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Top]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Bottom]].Texture, new Rectangle(offset.X + m_rect.X + GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Width, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Bottom]].Height, m_rect.Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.Bottom]].Height), m_borderColor );

                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopLeft]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Width, offset.Y + m_rect.Y, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.TopRight]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Texture, new Rectangle(offset.X + m_rect.X, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Height, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomLeft]].Height), m_borderColor );
                spriteBatch.Draw(GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Texture, new Rectangle(offset.X + m_rect.X + m_rect.Width - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Width, offset.Y + m_rect.Y + m_rect.Height - GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Height, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Width, GlobalVariables.ImageDict[m_borderImage [(int)BorderSide.BottomRight]].Height), m_borderColor );

            }
            #endregion
        }

        //****************************************************
        // Method: DrawOutline
        //
        // Purpose: Draws a selected "glow" around/under the
        // border. It's meant to be called from within
        // the DrawBorder function
        //****************************************************
        public virtual void DrawOutline(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            //Used to tweak how far from under the border this should be displayed. (0,0) means it's completely under the border.
            //(1,1) means that there should be 1 pixel showing all around. (0,1) should mean that there's 1 pixel above and below....
            Point outlineOffset = new Point(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Left]].Width + m_outlineOffset.X, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Top]].Height + m_outlineOffset.Y);

            #region Calculate Outline Rectangle
            Rectangle outlineRect = m_rect;
            outlineRect.X -= outlineOffset.X;
            outlineRect.Y -= outlineOffset.Y;
            outlineRect.Width += 2 * outlineOffset.X;
            outlineRect.Height += 2 * outlineOffset.Y;
            #endregion

            #region Draw Outline
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Left]].Texture, new Rectangle(offset.X + outlineRect.X, offset.Y + outlineRect.Y + GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Height, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Left]].Width, outlineRect.Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Right]].Texture, new Rectangle(offset.X + outlineRect.X + outlineRect.Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Right]].Width, offset.Y + outlineRect.Y + GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Height, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Right]].Width, outlineRect.Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Top]].Texture, new Rectangle(offset.X + outlineRect.X + GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Width, offset.Y + outlineRect.Y, outlineRect.Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Top]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Bottom]].Texture, new Rectangle(offset.X + outlineRect.X + GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Width, offset.Y + outlineRect.Y + outlineRect.Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Bottom]].Height, outlineRect.Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.Bottom]].Height), m_outlineColor);

            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Texture, new Rectangle(offset.X + outlineRect.X, offset.Y + outlineRect.Y, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopLeft]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Texture, new Rectangle(offset.X + outlineRect.X + outlineRect.Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Width, offset.Y + outlineRect.Y, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.TopRight]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Texture, new Rectangle(offset.X + outlineRect.X, offset.Y + outlineRect.Y + outlineRect.Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Height, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomLeft]].Height), m_outlineColor);
            spriteBatch.Draw(GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Texture, new Rectangle(offset.X + outlineRect.X + outlineRect.Width - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Width, offset.Y + outlineRect.Y + outlineRect.Height - GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Height, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Width, GlobalVariables.ImageDict[m_outlineImage[(int)BorderSide.BottomRight]].Height), m_outlineColor);
            #endregion
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // MenuItem (probably won't be implemented at all)
        //****************************************************
        public virtual void Update(GameTime gameTime)
        {
        }

        //****************************************************
        // Method: HandleInput
        //
        // Purpose: Handles any input from player one only.
        // Should only be modified via a hook
        //****************************************************
        public virtual void HandleInput(Controller playerOneController)
        {
            if (m_hasFocus)
            {
                if (m_hadFocus)
                {
                    HandleInputHook(playerOneController);
                }
                else
                {
                    m_hadFocus = true;
                }
            }
        }

        //****************************************************
        // Method: HandleInputHook
        //
        // Purpose: Handles any input from player one only.
        //****************************************************
        public virtual void HandleInputHook(Controller playerOneController)
        {
            #region Gamepad controls
            if (playerOneController.ControllerType == ControllerType.Gamepad)
            {
                if (playerOneController.ControllerState.isControlPressed(Control.AbilityThree) && !playerOneController.PrevControllerState.isControlPressed(Control.AbilityThree))
                {
                    LoseFocus(this);
                }
            }
            #endregion

            #region Keyboard Controls
            else
            {
                if (playerOneController.ControllerState.isControlPressed(Control.Back) && !playerOneController.PrevControllerState.isControlPressed(Control.Back))
                {
                    LoseFocus(this);
                }
            }
            #endregion
        }

        //****************************************************
        // Method: GiveFocus
        //
        // Purpose: Gives focus to another item by removing
        // the focus from this item and giving it to the item
        // passed in the parameter.
        //****************************************************
        public virtual void GiveFocus(MenuItem menuItem)
        {
            KillFocus(this);
            if(LostFocus != null)
                LostFocus(this);
            menuItem.GainFocus(this);
        }

        #region Event Handlers
        //****************************************************
        // Method: GainFocus
        //
        // Purpose: Runs all the code to give focus to
        // the MenuItem
        //****************************************************
        public virtual void GainFocus(object sender)
        {
            m_hasFocus = true;

            m_gaveFocus = (MenuItem)sender;

            if(GotFocus != null)
                GotFocus(sender);
        }

        //****************************************************
        // Method: LoseFocus
        //
        // Purpose: Runs all the code to remove focus from
        // the MenuItem
        //****************************************************
        public virtual void LoseFocus(object sender)
        {
            KillFocus(sender);

            if (m_ownerItem != null)
                m_ownerItem.GainFocus(this);
        }

        //****************************************************
        // Method: KillFocus
        //
        // Purpose: Runs all the code to remove focus from
        // the MenuItem and not return it to the ownerItem
        //****************************************************
        public virtual void KillFocus(object sender)
        {
            m_hasFocus = false;
            m_hadFocus = false;
            if (LostFocus != null)
                LostFocus(sender);
        }

        //****************************************************
        // Method: Select
        //
        // Purpose: Runs all the code to "select" this menu
        // item
        //****************************************************
        public virtual void Select(object sender)
        {
            m_isSelected = true;
            if (Selected != null)
                Selected(sender);
        }

        //****************************************************
        // Method: Deselect
        //
        // Purpose: Runs all the code to "deselect" this menu
        // item
        //****************************************************
        public virtual void Deselect(object sender)
        {
            m_isSelected = false;
            if (Deselected != null)
                Deselected(sender);
        }

        //****************************************************
        // Method: Click
        //
        // Purpose: Fires the Clicked event
        //****************************************************
        public void Click(object sender)
        {
            if(Clicked != null)
                Clicked(sender);
        }

        #endregion

        #region Events
        /// <summary>
        /// 
        /// Event raised when the menuItem has been "Clicked"
        /// 
        /// </summary>
        public event BasicMenuItemEventHandler Clicked;

        /// <summary>
        /// 
        /// Event raised when the menuItem has gained focus
        /// 
        /// </summary>
        public event BasicMenuItemEventHandler GotFocus;

        /// <summary>
        /// 
        /// Event raised when the menuItem has lost focus
        /// 
        /// </summary>
        public event BasicMenuItemEventHandler LostFocus;

        /// <summary>
        /// 
        /// Event raised when the menuItem has been "Selected"
        /// 
        /// </summary>
        public event BasicMenuItemEventHandler Selected;

        /// <summary>
        /// 
        /// Event raised when the menuItem has been "Deselected"
        /// 
        /// </summary>
        public event BasicMenuItemEventHandler Deselected;

        #endregion

        #region Properties
        #region Misc
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public bool HasFocus
        {
            get
            {
                return m_hasFocus;
            }
        }

        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
        }

        //Returns a rectangle which represents the borders of it's content (basically like the viewport)
        public Rectangle ContentRect
        {
            get
            {
                Rectangle tempContentRect = new Rectangle();
                tempContentRect.X = 0;
                tempContentRect.Y = 0;
                if (m_hasFocus)
                {
                    tempContentRect.Width = m_rect.Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Left]].Width - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Right]].Width + m_innerShadowSize[(int)BorderSide.Left]  + m_innerShadowSize[(int)BorderSide.Right];
                    tempContentRect.Height = m_rect.Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Top]].Height - GlobalVariables.ImageDict[m_borderImageFocus[(int)BorderSide.Bottom]].Height + m_innerShadowSize[(int)BorderSide.Top] + m_innerShadowSize[(int)BorderSide.Bottom];
                }
                else
                {
                    tempContentRect.Width = m_rect.Width - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Left]].Width - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Right]].Width + m_innerShadowSize[(int)BorderSide.Left] + m_innerShadowSize[(int)BorderSide.Right];
                    tempContentRect.Height = m_rect.Height - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Top]].Height - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Bottom]].Height + m_innerShadowSize[(int)BorderSide.Top] + m_innerShadowSize[(int)BorderSide.Bottom];
                }
                return tempContentRect;
            }
        }
        #endregion

        #region Background and Border
        public bool BkgFullStretch
        {
            get
            {
                return m_bkgFullStretch;
            }
            set
            {
                m_bkgFullStretch = value;
            }
        }

        public string BackgroundImage
        {
            get
            {
                return m_bkgImage;
            }

            set
            {
                m_bkgImage = value;
            }
        }

        public string BackgroundImageFocus
        {
            get
            {
                return m_bkgImageFocus;
            }

            set
            {
                m_bkgImageFocus = value;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return m_bkgColor;
            }
            set
            {
                m_bkgColor = value;
            }
        }

        public Color BackgroundColorFocus
        {
            get
            {
                return m_bkgColorFocus;
            }
            set
            {
                m_bkgColorFocus = value;
            }
        }

        public string[] BorderImage
        {
            get
            {
                return m_borderImage;
            }

            set
            {
                m_borderImage = value;
            }
        }

        public string[] BorderImageFocus
        {
            get
            {
                return m_borderImageFocus;
            }

            set
            {
                m_borderImageFocus = value;
            }
        }

        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;
            }
        }

        public Color BorderColorFocus
        {
            get
            {
                return m_borderColorFocus;
            }
            set
            {
                m_borderColorFocus = value;
            }
        }
        #endregion

        #region Outline
        public Point OutlineOffset
        {
            get
            {
                return m_outlineOffset;
            }

            set
            {
                m_outlineOffset = value;
            }
        }

        public string[] OutlineImage
        {
            get
            {
                return m_outlineImage;
            }
            set
            {
                m_outlineImage = value;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return m_outlineColor;
            }
            set
            {
                m_outlineColor = value;
            }
        }
        #endregion

        #region Font
        public string Font
        {
            get
            {
                return m_font;
            }
            set
            {
                if (GlobalVariables.FontDict.ContainsKey(value))
                    m_font = value;
            }
        }

        public Color FontColor
        {
            get
            {
                return m_fontColor;
            }
            set
            {
                m_fontColor = value;
            }
        }

        public Color FontColorFocus
        {
            get
            {
                return m_fontColorFocus;
            }
            set
            {
                m_fontColorFocus = value;
            }
        }
        #endregion

        #region Bounds
        public Rectangle Bounds
        {
            get
            {
                return m_rect;
            }
            set
            {
                m_rect = value;
            }
        }

        public int X
        {
            get
            {
                return m_rect.X;
            }
            set
            {
                m_rect.X = value;
            }
        }

        public int Y
        {
            get
            {
                return m_rect.Y;
            }
            set
            {
                m_rect.Y = value;
            }
        }

        public int Width
        {
            get
            {
                return m_rect.Width;
            }
            set
            {
                m_rect.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return m_rect.Height;
            }
            set
            {
                m_rect.Height = value;
            }
        }
        #endregion

        public int[] InnerShadowSize
        {
            get
            {
                return m_innerShadowSize;
            }
            set
            {
                m_innerShadowSize = value;
            }
        }

        public bool HadFocus
        {
            get
            {
                return m_hadFocus;
            }
        }

        public MenuItem OwnerItem
        {
            get
            {
                return m_ownerItem;
            }

            set
            {
                m_ownerItem = value;
            }
        }
        #endregion
    }
}
