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
    class TextBox : MenuItem
    {
        #region Declarations
        protected bool m_isMultiline;
        #endregion

        //****************************************************
        // Method: TextBox
        //
        // Purpose: Default constructor for TextBox
        //****************************************************
        public TextBox(MenuItem ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocus, Color fontColor, Color fontColorFocus, string font = "Arial20", string initialText = "", bool isMultiline = false, string border = "TextBox", string background = "TextBox")
            : base(ownerItem, game1, name, new Rectangle(X, Y, Width, Height), background, backgroundColor, backgroundColorFocus, border, backgroundColor, backgroundColorFocus, false, false)
        {
            m_isMultiline = isMultiline;
            m_font = font;
            
            m_fontColor = fontColor;
            m_fontColorFocus = fontColorFocus;
            
            m_innerShadowSize[(int)BorderSide.Left] = 8;
            m_innerShadowSize[(int)BorderSide.Right] = 8;
            m_innerShadowSize[(int)BorderSide.Top] = 9;
            m_innerShadowSize[(int)BorderSide.Bottom] = 9;
            Text = initialText;
        }        

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws the text
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            base.DrawContent(spriteBatch, graphicsDevice, offset);
        }


        //****************************************************
        // Method: GainFocus
        //
        // Purpose: Runs all the code to give focus to
        // the TextBox
        //****************************************************
        public override void GainFocus(object sender)
        {
            EventInput.EventInput.CharEntered += TextInputHandler;
            base.GainFocus(sender);
        }

        //****************************************************
        // Method: KillFocus
        //
        // Purpose: Runs all the code to remove focus from
        // the TextBox
        //****************************************************
        public override void KillFocus(object sender)
        {
            EventInput.EventInput.CharEntered -= TextInputHandler;
            base.KillFocus(sender);
        }

        //****************************************************
        // Method: HandleInputHook
        //
        // Purpose: Handles any input from player one only.
        //****************************************************
        public override void HandleInput(Controller playerOneController)
        {
            //Empty because textboxes handle input through a special event handling system
        }

        //****************************************************
        // Method: TextInputHandler
        //
        // Purpose: Handles input from the keyboard
        //****************************************************
        public void TextInputHandler(object sender, CharacterEventArgs e)
        {
            //System.Console.WriteLine(e.Character + ":" + Convert.ToInt32(e.Character));
            
            if (e.Character.ToString() == "\b")
            {
                if(m_text.Length > 0)
                Text = m_text.Remove(m_text.Length - 1);
                return;
            }
            //Makes sure it's an okay character (bounds may need to be modifed)
            if(e.Character.ToString() == " " || (Convert.ToInt32(e.Character) > 32 && Convert.ToInt32(e.Character) < 126))
                Text += e.Character;
        }


        #region Properties
        public virtual string Text
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
                }
                else
                {
                    m_text = value;
                }
            }
        }

        public bool IsMultiline
        {
            get
            {
                return m_isMultiline;
            }
            set
            {
                m_isMultiline = value;
            }
        }

        public string Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }
        #endregion
    }
}
