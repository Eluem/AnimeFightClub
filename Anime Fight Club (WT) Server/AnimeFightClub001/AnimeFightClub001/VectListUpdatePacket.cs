//******************************************************
// File: VectListUpdatePacket.cs
//
// Purpose: The VectListUpdatePacket class will be used
// to store a list of either position updates,
// velocity updates, or acceleration updates.
// It will also store the object id of all objects
// that these updates apply to, along with the
// number of each type of object which is being.
// It will also store a time stamp.
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
    class VectListUpdatePacket
    {
        float timeStamp; //The time, translated into local time, that this packet was sent

    }
}
