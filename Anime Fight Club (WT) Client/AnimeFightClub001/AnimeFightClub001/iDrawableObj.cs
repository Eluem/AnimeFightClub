//******************************************************
// File: iDrawableObj.cs
//
// Purpose: This is the type that the DrawableObj
// will accept as it's owner. It should be implemented
// by any object that would be drawn by DrawableObj.
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
    interface iDrawableObj
    {
        Vector2 Center
        {
            get;
        }

        int State
        {
            get;
            set;
        }

        int PrevState
        {
            get;
        }

        Direction Direction
        {
            get;
            set;
        }

    }
}
