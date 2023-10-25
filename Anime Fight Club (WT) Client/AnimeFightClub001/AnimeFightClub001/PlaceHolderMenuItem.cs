//******************************************************
// File: PlaceHolderMenuItem.cs
//
// Purpose: Defines a PlaceHolderMenuItem. This object
// will be used to fill up the empty space between
// items in a menu. (basically a cludge because I
// messed up when designing the menus).
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
    class PlaceHolderMenuItem : MenuItem
    {


        //****************************************************
        // Method: PlaceHolderMenuItem
        //
        // Purpose: Constructor for PlaceHolderMenuItem
        //****************************************************
        public PlaceHolderMenuItem(MenuItem ownerItem, Game1 game1)
            : base(ownerItem, game1)
        {
            this.Selected += PlaceHolderSelected;
        }


        //****************************************************
        // Method: PlaceHolderSelected
        //
        // Purpose: Fires if this place older was selected.
        // Mainly for menus... If a menu selects a
        // PlaceHolderItem it automatically pushes through
        // to the next item
        //****************************************************
        public void PlaceHolderSelected(object sender)
        {

        }


        #region Override Everything to do Nothing
        public override void DrawBackground(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
        }

        public override void DrawBorder(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
        }

        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
        }

        public override void DrawOutline(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
        }

        public override void GainFocus(object sender)
        {
        }

        public override void HandleInput(Controller playerOneController)
        {
        }

        public override void HandleInputHook(Controller playerOneController)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
        #endregion
    }
}
