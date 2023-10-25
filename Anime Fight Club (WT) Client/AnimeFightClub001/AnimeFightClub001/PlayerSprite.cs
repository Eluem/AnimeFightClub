//******************************************************
// File: PlayerSprite.cs
//
// Purpose: Contains the class definition for
// PlayerSprite. PlayerSprite will be responsible for
// animating the complexities of a PlayerObj.
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
    class PlayerSprite : Sprite
    {
        #region Declarations
        string m_avatarName;
        #endregion

        //****************************************************
        // Method: PlayerSprite
        //
        // Purpose: PlayerSprite constructor
        //****************************************************
        public PlayerSprite(string avatarName, Color Color, BasicObj ownerObj) : base(avatarName + "None", Color, ownerObj, GlobalVariables.GetLayer(ObjectLayer.Player))
        {
            m_avatarName = avatarName;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the drawableObj based on
        // the gametime (generally only used for sprites)
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            m_imageName = m_avatarName + ((PlayerObj)m_ownerObj).Action.ToString(); //Finds the sprite sheet that correlates to the avatar/action
            base.Update(gameTime);

        }

    }
}
