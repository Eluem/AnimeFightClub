//******************************************************
// File: TextBox.cs
//
// Purpose: Used to recieve text input
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

using EventInput;

namespace AnimeFightClub001
{
    class PasswordTextBox: TextBox
    {
        #region Declarations
        protected string m_displayText; //The text that will be displayed (replaces all text with dots)
        #endregion

        //****************************************************
        // Method: PasswordTextBox
        //
        // Purpose: Constructor for PasswordTextBox
        //****************************************************
        public PasswordTextBox(MenuItem ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocus, Color fontColor, Color fontColorFocus, bool isMultiline = false, string border = "TextBox", string background = "TextBox")
            : base(ownerItem, game1, name, X, Y, Width, Height, backgroundColor, backgroundColorFocus, fontColor, fontColorFocus, "Arial20", "", isMultiline, border, background)
        {
            m_displayText = "";
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws the text
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            if (m_hasFocus)
            {
                spriteBatch.DrawString(GlobalVariables.FontDict[m_font], m_displayText, new Vector2(m_textPos.X + offset.X, m_textPos.Y + offset.Y), m_fontColorFocus);
            }
            else
            {
                spriteBatch.DrawString(GlobalVariables.FontDict[m_font], m_displayText, new Vector2(m_textPos.X + offset.X, m_textPos.Y + offset.Y), m_fontColor);
            }
            spriteBatch.End();
        }

        #region Properties
         public override string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                if (m_isMultiline)
                {
                    int workingTextWidth = ContentRect.Width - (int)Math.Round(m_textPos.X); //Used to determine the max width of the string
                    #region Calculate lines
                    string[] words = value.Split(' ');
                    string currentLine = words[0]; //Used to build one line at a time

                    m_text = "";
                    for (int i = 1; i < words.Length; ++i)
                    {
                        if (GlobalVariables.FontDict[m_font].MeasureString(currentLine + ' '+ words[i]).X < workingTextWidth)
                        {
                            currentLine += ' ' + words[i];
                        }
                        else
                        {
                            m_text += currentLine + '\n';
                            currentLine = words[i];
                        }
                    }
                    m_text += currentLine; //Adds whatever was left
                    #endregion

                    m_displayText = "";
                    for(int i = 0; i < m_text.Length; ++i)
                    {
                        m_displayText += "*";
                    }
                }
                else
                {
                    m_text = value;

                    m_displayText = "";
                    for (int i = 0; i < m_text.Length; ++i)
                    {
                        m_displayText += "*";
                    }
                }
            }
        }
        #endregion
    }
}
