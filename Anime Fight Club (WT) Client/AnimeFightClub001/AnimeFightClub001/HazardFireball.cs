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

namespace AnimeFightClub001
{
    class HazardFireball : HazardObj
    {

        public HazardFireball()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(25, 25);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 1f;
            m_drawableObj = new Sprite("BombFire", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
        }

        public HazardFireball(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 15, -1, 1, 3000)
        {
            m_physicsObj.Rect.SetDimensions(25, 25);

            SetStartingSpeed(speed);
            m_physicsObj.VelY = 200;

            m_physicsObj.Restitution = 1f;
            m_drawableObj = new Sprite("BombFire", Color.Wheat, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
        }


    

    }
}
