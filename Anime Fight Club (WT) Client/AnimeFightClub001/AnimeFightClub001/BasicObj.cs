//******************************************************
// File: BasicObj.cs
//
// Purpose: Contains the class definition for BasicObj.
// BasicObj contains base functions which nearly
// every object in the game will call.
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
    enum Direction { Left, Right }

    class BasicObj : iDrawableObj
    {
        #region Declarations
        protected bool m_deleted; //Used for deletion testing
        protected int m_despawnClock;
        protected int m_startingDespawnClock;
        protected PhysicsObj m_physicsObj; //Used to manage physics
        protected DrawableObj m_drawableObj; //Used to manage drawing

        //**********************************************************************
        // Stores the id of the object. The id of an object is used to identify
        // certain things such as who damaged a player. All objects which are
        // not players, for now, will be considered to be "game objects" and
        // will be given an id of -1. Players are given ids that count up
        // from there.
        // (May not be fully implemented by the end of the class)
        //**********************************************************************
        protected UInt16 m_objectID;

        //**********************************************************************
        // Stores the id of the object that owns/created this object.
        // All objects which are not owned by a player will be owned by the
        // game (which has the -1 id). Objects that a player owns, such as
        // a sword slash or projectile that a player may shoot will be given
        // an ownerID equal to that player's objectID. The object will still
        // be considered a game object, however and will get an objectID of -1.
        // (May not be fully implemented by the end of the class)
        //**********************************************************************
        protected UInt16 m_ownerID;

        protected BasicObj m_ownerObj; //Used to make this more efficient if you choose to initialize it

        protected string m_ownerType; //Stores the class type of the object that owns this object

        //State Variables
        protected Direction m_direction;

        protected int m_state; //Stored as an integer so that every class can have it's own enumeration.
        protected int m_prevState;


        #endregion

        //****************************************************
        // Method: NetInitialize
        //
        // Purpose: Initializes values based on the 
        // mesage sent by the server
        //****************************************************
        public virtual void NetInitialize(NetIncomingMessage msg)
        {
            //Read ID
            m_objectID = msg.ReadUInt16();

            //Read owner ID
            m_ownerID = msg.ReadUInt16();

            //Read owner type
            m_ownerType = msg.ReadString();

            //Read position
            SetPos(msg.ReadFloat(), msg.ReadFloat());

            //Read velocity
            m_physicsObj.VelX = msg.ReadFloat();
            m_physicsObj.VelY = msg.ReadFloat();

            //Read Direction
            m_direction = (AnimeFightClub001.Direction)msg.ReadByte();
        }

        //****************************************************
        // Method: BasicObj
        //
        // Purpose: Default BasicObj Constructor
        //****************************************************
        public BasicObj()
        {
            m_deleted = false;
            m_physicsObj = new PhysicsObj();
            m_drawableObj = new DrawableObj(this, GlobalVariables.GetLayer(ObjectLayer.Hazard));

            m_objectID = 0;
            m_ownerID = 0;
            m_ownerType = "World";

            m_direction = Direction.Left;
            m_state = 0;
            m_despawnClock = -1;
            m_startingDespawnClock = -1;
        }

        //****************************************************
        // Method: BasicObj
        //
        // Purpose: Default BasicObj Constructor
        //****************************************************
        public BasicObj(UInt16 ownerID = 0, string ownerType = "World", int despawnClock = -1)
        {
            m_deleted = false;
            m_physicsObj = new PhysicsObj();
            m_drawableObj = new DrawableObj(this, GlobalVariables.GetLayer(ObjectLayer.Hazard));

            m_objectID = 0;
            m_ownerID = ownerID;
            m_ownerType = ownerType;

            m_direction = Direction.Left;
            m_state = 0;

            m_despawnClock = despawnClock;
            m_startingDespawnClock = despawnClock;
        }

        //****************************************************
        // Method: BasicObj
        //
        // Purpose: BasicObj Constructor
        //****************************************************
        public BasicObj(PhysicsObj physicsObj, DrawableObj drawableObj, UInt16 ownerID = 0, string ownerType = "World", int despawnClock = -1)
        {
            m_deleted = false;
            m_physicsObj = physicsObj;
            m_drawableObj = drawableObj;

            m_objectID = 0;
            m_ownerID = ownerID;
            m_ownerType = ownerType;

            m_direction = Direction.Left;
            m_state = 0;

            m_despawnClock = despawnClock;
            m_startingDespawnClock = despawnClock;
        }

        //****************************************************
        // Method: Delete
        //
        // Purpose: Deletes the object by adding it to the
        // appropraite delete list and setting the deleted
        // boolean to true.
        //****************************************************
        public virtual void Delete()
        {
            m_deleted = true;
            GameObjectHandler.DeleteObject(this);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public virtual void Update(GameTime gameTime, Viewport viewport)
        {
            m_physicsObj.Update(gameTime, viewport);
            UpdateDrawable(gameTime);
        }

        //****************************************************
        // Method: UpdateDrawable
        //
        // Purpose: Updates the details of the objects
        // drawbleObj
        //****************************************************
        public virtual void UpdateDrawable(GameTime gameTime)
        {
        }

        //**************************************************
        // Method: Copy
        //
        // Purpose: To make a deep copy of this object so
        // that, during collisions the system can stay
        // consistent.
        //**************************************************
        public virtual BasicObj DeepClone()
        {
            return (new BasicObj()); //Change so that it returns an actual copy
        }

        //****************************************************
        // Method: SetPos
        //
        // Purpose: Allows the position to be set from
        // an outside source without needing to update
        // several other dependent positions afterward
        //****************************************************
        public virtual void SetPos(float x, float y)
        {
            m_physicsObj.PosX = x;
            m_physicsObj.PosY = y;
        }

        //****************************************************
        // Method: SetPos
        //
        // Purpose: Allows the position to be set from
        // an outside source without needing to update
        // several other dependent positions afterward
        //****************************************************
        public virtual void SetPos(Point point)
        {
            m_physicsObj.PosX = point.X;
            m_physicsObj.PosY = point.Y;
        }

        //****************************************************
        // Method: SetDimensions
        //
        // Purpose: Changes the dimensions of the drawableObj 
        // and collision box
        //****************************************************
        public virtual void SetDimensions(float Width, float Height)
        {
            m_physicsObj.Rect.SetDimensions(Width, Height);
            m_drawableObj.SetDimensions((int)Width, (int)Height);
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the object any anything else
        // that it may need to draw.
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            m_drawableObj.Draw(spriteBatch);
        }

        //****************************************************
        // Method: ResetDespawnTimer
        //
        // Purpose: Resets the object's despawn timer
        //****************************************************
        public virtual void ResetDespawnClock()
        {
            m_despawnClock = m_startingDespawnClock;
        }

        #region Collide Functions
        #region Before
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // BasicObj overload.
        //****************************************************
        public virtual void CollideBefore(BasicObj basicObj)
        {
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // PlayerObj overload.
        //****************************************************
        public virtual void CollideBefore(PlayerObj player)
        {
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // HazardObj overload.
        //****************************************************
        public virtual void CollideBefore(HazardObj hazard)
        {
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // EnvironmentalObj overload.
        //****************************************************
        public virtual void CollideBefore(EnvironmentalObj environmentalObj)
        {
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // SpecialEnvironmentalObj overload.
        //****************************************************
        public virtual void CollideBefore(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // ItemObj overload.
        //****************************************************
        public virtual void CollideBefore(ItemObj item)
        {
        }
        #endregion

        #region After
        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // BasicObj overload.
        //****************************************************
        public virtual void CollideAfter(BasicObj basicObj)
        {
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // PlayerObj overload.
        //****************************************************
        public virtual void CollideAfter(PlayerObj player)
        {
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // HazardObj overload.
        //****************************************************
        public virtual void CollideAfter(HazardObj hazard)
        {
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // EnvironmentalObj overload.
        //****************************************************
        public virtual void CollideAfter(EnvironmentalObj environmentalObj)
        {
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // SpecialEnvironmentalObj overload.
        //****************************************************
        public virtual void CollideAfter(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
        }

        //****************************************************
        // Method: CollideAfter
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // ItemObj overload.
        //****************************************************
        public virtual void CollideAfter(ItemObj item)
        {
        }
        #endregion
        #endregion

        #region Properties
        public bool Deleted
        {
            get
            {
                return m_deleted;
            }
        }

        public PhysicsObj PhysicsObj
        {
            get
            {
                return m_physicsObj;
            }
        }

        public DrawableObj DrawableObj
        {
            get
            {
                return m_drawableObj;
            }
            set
            {
                m_drawableObj = value;
            }
        }

        public UInt16 OwnerID
        {
            get
            {
                return m_ownerID;
            }
            set
            {
                m_ownerID = value;
            }
        }

        public UInt16 ObjectID
        {
            get
            {
                return m_objectID;
            }
            set
            {
                m_objectID = value;
            }
        }

        public Vector2 Center
        {
            get
            {
                return m_physicsObj.Rect.Center;
            }
        }

        public Vector2 Pos
        {
            get
            {
                return m_physicsObj.Pos;
            }
        }

        public Vector2 Vel
        {
            get
            {
                return m_physicsObj.Vel;
            }
        }

        public Vector2 Accel
        {
            get
            {
                return m_physicsObj.Accel;
            }
        }

        public Direction Direction
        {
            get
            {
                return m_direction;
            }
            set
            {
                m_direction = value;
            }
        }

        public int State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }

        public int PrevState
        {
            get
            {
                return m_prevState;
            }
        }

        public int DespawnClock
        {
            get
            {
                return m_despawnClock;
            }
            set
            {
                m_despawnClock = value;
            }
        }
        #endregion
    }
}
