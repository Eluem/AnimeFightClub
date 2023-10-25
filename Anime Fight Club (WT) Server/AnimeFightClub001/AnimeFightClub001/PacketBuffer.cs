//******************************************************
// File: PacketBuffer.cs
//
// Purpose: The PacketBuffer class will be used to
// store packets that have been read in.
// Based on the time stamp of the packet it will either
// be dropped, used, or held until it is useable.
// This should help to reduce jitter.
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
    class PacketBuffer
    {
    }
}
