//******************************************************
// File: Button.cs
//
// Purpose: Defines a basic button
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
    class Button : MenuItem
    {
        #region Declarations
        protected string m_buttonImage = ""; //stores an image that sits on top of the background, under the text..
        #endregion

        //****************************************************
        // Method: Button
        //
        // Purpose: Constructor for Button
        //****************************************************
        public Button(MenuItem ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocused, Color fontColor, Color fontColorFocused, string text = "", string font = "Arial12", string buttonImageBackground = "RoundButton", string buttonImageBorder = "RoundButton", bool bkgFullStretch = false)
            : base(ownerItem, game1, name, new Rectangle(X, Y, Width, Height), buttonImageBackground, backgroundColor, backgroundColorFocused, buttonImageBorder, backgroundColor, backgroundColorFocused)
        {
            m_outlineOffset = new Point(-9, -9);
            m_font = font;
            m_fontColor = fontColor;
            m_fontColorFocus = fontColorFocused;
            m_text = text;

            m_bkgFullStretch = bkgFullStretch;

            m_textPos = new Vector2((float)(ContentRect.Center.X - ((float)GlobalVariables.FontDict[m_font].MeasureString(m_text).X / 2)), (float)(ContentRect.Center.Y - ((float)GlobalVariables.FontDict[m_font].MeasureString(m_text).Y / 2)));
        }

        //****************************************************
        // Method: Button
        //
        // Purpose: Constructor for Button
        //****************************************************
        public Button(MenuItem ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocused, Color fontColor, Color fontColorFocused, Point outlineOffset, string text = "", string buttonImage = "", string font = "Arial12", string buttonImageBackground = "RoundButton", string buttonImageBorder = "RoundButton", bool bkgFullStretch = false)
            : base(ownerItem, game1, name, new Rectangle(X, Y, Width, Height), buttonImageBackground, backgroundColor, backgroundColorFocused, buttonImageBorder, backgroundColor, backgroundColorFocused)
        {
            m_outlineOffset = outlineOffset;
            m_font = font;
            m_fontColor = fontColor;
            m_fontColorFocus = fontColorFocused;
            m_text = text;

            m_bkgFullStretch = bkgFullStretch;

            m_textPos = new Vector2((float)(ContentRect.Center.X - ((float)GlobalVariables.FontDict[m_font].MeasureString(m_text).X / 2)), (float)(ContentRect.Center.Y - ((float)GlobalVariables.FontDict[m_font].MeasureString(m_text).Y / 2)));

            m_buttonImage = buttonImage;

        }


        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws the content of the button
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(GlobalVariables.ImageDict[m_buttonImage].Texture, new Rectangle(offset.X, offset.Y, ContentRect.Width, ContentRect.Height), Color.Wheat);
            spriteBatch.End();
            base.DrawContent(spriteBatch, graphicsDevice, offset);
        }


        #region Properties
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        public string ButtonImage
        {
            get
            {
                return m_buttonImage;
            }
            set
            {
                m_buttonImage = value;
            }
        }
        #endregion
    }
}
