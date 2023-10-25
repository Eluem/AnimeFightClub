//******************************************************
// File: HUD.cs
//
// Purpose: Drawn to the bottom of the screen during
// gameplay to display important information to the
// player.
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;
namespace AnimeFightClub001
{
    class HUD
    {
        #region Declarations
        protected PlayerObj m_ownerObj;
        protected Game1 m_game1;
        protected TextBox m_background; //Used for background image
        protected TextBox m_healthBackground; //Used for background image of health
        protected TextBox m_manaBackground; //Used for background image of mana
        #endregion

        #region Constants
        const int BACKGROUND_WIDTH = 320;
        #endregion

        //****************************************************
        // Method: HUD
        //
        // Purpose: Default Constructor for HUD
        //****************************************************
        public HUD(Game1 game1)
        {
            m_game1 = game1;
            m_ownerObj = null;

            Rectangle tempRect = new Rectangle();
            tempRect.Width = BACKGROUND_WIDTH;
            tempRect.Height = 80;
            tempRect.X = game1.GraphicsDevice.Viewport.Bounds.Center.X - tempRect.Width / 2;
            tempRect.Y = game1.GraphicsDevice.Viewport.Bounds.Bottom - tempRect.Height;

            m_background = new TextBox(null, game1, "background", tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Arial10", "", false, "IconFrame001", "Background001");
            m_background.InnerShadowSize[(int)BorderSide.Left] = 4;
            m_background.InnerShadowSize[(int)BorderSide.Right] = 11;
            m_background.InnerShadowSize[(int)BorderSide.Top] = 11;
            m_background.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            m_healthBackground = new TextBox(null, game1, "healthBackground", m_background.Bounds.Left + 12 + (int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").X, m_background.Bounds.Top + 10, 210, 24, Color.Wheat, Color.Wheat, Color.Black, Color.Black);

            m_manaBackground = new TextBox(null, game1, "manaBackground", m_background.Bounds.Left + 12 + (int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").X, m_background.Bounds.Top + 55, 210, 24, Color.Wheat, Color.Wheat, Color.Black, Color.Black);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the player
        //****************************************************
        public virtual void Update(GameTime gameTime, Viewport viewport)
        {
            Rectangle tempRect = new Rectangle();
            tempRect.Width = BACKGROUND_WIDTH;
            tempRect.Height = 80;
            tempRect.X = viewport.Bounds.Center.X - tempRect.Width / 2;
            tempRect.Y = viewport.Bounds.Bottom - tempRect.Height;

            m_background.Bounds = tempRect;

            m_healthBackground.X = m_background.Bounds.Left + 12 + (int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").X;
            m_healthBackground.Y = m_background.Bounds.Top + 15;

            m_manaBackground.X = m_background.Bounds.Left + 12 + (int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").X;
            m_manaBackground.Y = m_background.Bounds.Top + 45;
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the object any anything else
        // that it may need to draw.
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            m_background.DrawBackground(spriteBatch, m_game1.GraphicsDevice, Point.Zero);
            m_background.DrawBorder(spriteBatch, m_game1.GraphicsDevice, Point.Zero);

            #region Health Color
            int red;
            int green;
            int blue;
            if (m_ownerObj.HP > (float)GlobalVariables.Settings.startingHealth / 2)
            {
                green = 255;
                red = (int)(510 * (1 - (m_ownerObj.HP / (float)GlobalVariables.Settings.startingHealth)));
            }

            else
            {
                red = 255;
                green = (int)(400 * (m_ownerObj.HP / (float)GlobalVariables.Settings.startingHealth));
            }

            Color tempColor = new Color(red, green, 0);
            #endregion

            #region Mana Color
            if (m_ownerObj.MP > 10000f / 2)
            {
                red = 0;
                blue = 255;
                green = (int)(255 * ((float)m_ownerObj.MP / 10000f - .5));
            }

            else
            {
                green = 0;
                
                red = (int)(255 * (1 - ((float)m_ownerObj.MP / 10000f + .5)));
                blue = 255;
            }
            Color manaColor = new Color(red, green, blue);
            #endregion
            
            m_healthBackground.DrawBackground(spriteBatch, m_game1.GraphicsDevice, Point.Zero);
            m_healthBackground.DrawBorder(spriteBatch, m_game1.GraphicsDevice, Point.Zero);

            m_manaBackground.DrawBackground(spriteBatch, m_game1.GraphicsDevice, Point.Zero);
            m_manaBackground.DrawBorder(spriteBatch, m_game1.GraphicsDevice, Point.Zero);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Health", new Vector2(m_healthBackground.Bounds.X - (int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").X - 2, m_healthBackground.Bounds.Center.Y - ((int)GlobalVariables.FontDict["Arial10"].MeasureString("Health").Y / 2)), Color.Black);
            spriteBatch.Draw(GlobalVariables.ImageDict["Default"].Texture, new Rectangle(m_healthBackground.Bounds.X + 5, m_healthBackground.Bounds.Y + 5, (int)(200 * ((float)m_ownerObj.HP / GlobalVariables.Settings.startingHealth)), m_healthBackground.Bounds.Height-10), tempColor);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Mana", new Vector2(m_manaBackground.Bounds.X - (int)GlobalVariables.FontDict["Arial10"].MeasureString("Mana").X - 2, m_manaBackground.Bounds.Center.Y - ((int)GlobalVariables.FontDict["Arial10"].MeasureString("Mana").Y / 2)), Color.Black);
            spriteBatch.Draw(GlobalVariables.ImageDict["Default"].Texture, new Rectangle(m_manaBackground.Bounds.X + 5, m_manaBackground.Bounds.Y + 5, (int)(200 * ((float)m_ownerObj.MP / 10000 /*GlobalVariables.Settings.startingMana*/)), m_manaBackground.Bounds.Height - 10), manaColor);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Kills", new Vector2(m_background.Bounds.Right - (int)GlobalVariables.FontDict["Arial10"].MeasureString("Kills").X - 20, m_background.Bounds.Top + 10), Color.Black);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], m_ownerObj.Kills.ToString(), new Vector2((m_background.Bounds.Right - (int)GlobalVariables.FontDict["Arial10"].MeasureString("Kills").X/2 - 20) - ((int)GlobalVariables.FontDict["Arial10"].MeasureString(m_ownerObj.Kills.ToString()).X/2), m_background.Bounds.Top + 12 + (int)GlobalVariables.FontDict["Arial10"].MeasureString("Kills").Y), Color.Black);
        }

        #region Properties
        public PlayerObj OwnerObj
        {
            get
            {
                return m_ownerObj;
            }
            set
            {
                m_ownerObj = value;
            }
        }
        #endregion
    }
}
