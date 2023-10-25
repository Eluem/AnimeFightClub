//******************************************************
// File: PhysicsObj.cs
//
// Purpose: Contains the class definition of
// PhysicsObj. An instance of PhysicsObj will be
// declared in any object which should be able
// to handle movement and other 'physical' activities.
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
    class PlayerPhysicsObj : PhysicsObj
    {

        #region Declartions
        protected Vector2 m_playerVel;
        protected Vector2 m_playerAccel;
        #endregion


        //****************************************************
        // Method: PlayerPhysicsObj
        //
        // Purpose: Default PlayerPhysicsObj constructor
        //****************************************************
        public PlayerPhysicsObj()
        {
            //Position and movement initializations
            m_vel = Vector2.Zero;
            m_accel = Vector2.Zero;

            m_playerVel = Vector2.Zero;
            m_playerAccel = Vector2.Zero;

            //Property initializations
            m_mass = 0;
            m_friction = 0;
            m_airResistance = Vector2.Zero;
            m_contactFriction = Vector2.Zero;
            m_restitution = 0;

            m_rect = new FloatRectangle();

            m_applyMomentum = true;

            //Object touched on side initializations
            m_sided = new Dictionary<Side, bool>();
            m_sided.Add(Side.left, false);
            m_sided.Add(Side.right, false);
            m_sided.Add(Side.top, false);
            m_sided.Add(Side.bottom, false);
        }

        //****************************************************
        // Method: PlayerPhysicsObj
        //
        // Purpose: PlayerPhysicsObj constructor
        //****************************************************
        public PlayerPhysicsObj(Vector2 position, Vector2 velocity, Vector2 dimensions, Vector2 acceleration, float Mass, float friction, Vector2 airResistance, float restitution, bool applyMomentum)
        {
            //Position and movement initializations
            m_vel = velocity;
            m_accel = acceleration;

            m_playerVel = Vector2.Zero;
            m_playerAccel = Vector2.Zero;

            //Property initializations
            m_mass = Mass;
            m_friction = friction;
            m_airResistance = airResistance;
            m_restitution = restitution;

            m_rect = new FloatRectangle(position.X, position.Y, dimensions.X, dimensions.Y);

            m_applyMomentum = applyMomentum;

            //Object touched on side initializations
            m_sided = new Dictionary<Side, bool>();
            m_sided.Add(Side.left, false);
            m_sided.Add(Side.right, false);
            m_sided.Add(Side.top, false);
            m_sided.Add(Side.bottom, false);
        }

        //****************************************************
        // Method: PlayerPhysicsObj
        //
        // Purpose: PlayerPhysicsObj constructor for Copy
        //****************************************************
        public PlayerPhysicsObj(Vector2 velocity, Vector2 acceleration, float Mass, float friction, Vector2 airResistance, float restitution, FloatRectangle rect, Dictionary<Side, bool> sided, bool applyMomentum, Vector2 playerVelocity, Vector2 playerAcceleration)
        {
            //Position and movement initializations
            m_vel = velocity;
            m_accel = acceleration;

            m_playerVel = playerVelocity;
            m_playerAccel = playerAcceleration;

            //Property initializations
            m_mass = Mass;
            m_friction = friction;
            m_airResistance = airResistance;
            m_restitution = restitution;


            //Object touched on side initializations
            m_sided = new Dictionary<Side, bool>();
            m_sided.Add(Side.left, false);
            m_sided.Add(Side.right, false);
            m_sided.Add(Side.top, false);
            m_sided.Add(Side.bottom, false);

            m_applyMomentum = applyMomentum;

            m_rect = rect;
            m_sided = sided;
        }
      
        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object.
        //****************************************************
        public override void Update(GameTime gameTime, Viewport port)
        {
            ApplyContactFriction();
            //REQUIRES WORK

            m_prevPos.X = m_rect.X; //Updates the previous position
            m_prevPos.Y = m_rect.Y; //Updates the previous position

            //Updates verticle accel based on gravity if the object isn't on the ground
            if (!m_sided[Side.bottom])
            {
                m_vel.Y += GlobalVariables.Settings.Gravity / GlobalVariables.Settings.fps;
            }
            //To prevent objects which are sliding on the ground from sliding at very small speeds
            //for a long period of time I just set the speed to 0 when it's moving too slowly.
            else
            {
                if (Math.Abs(m_vel.X) < 1)
                {
                    m_vel.X = 0;

                }
                //This is done to prevent objects from bouncing off of each other forever (not perfect solution....)
                if (Math.Abs(m_vel.Y) < 30)
                {
                    m_vel.Y = 0;
                }
            }

            //**************************************************************************
            // The line of code below applies the air resitance on the object.
            // However, it might be a better idea to simply add the airResistance
            // values to the m_contactFriction values in the ApplyContactFriction
            // function. (should this be moved down, after apply accel.
            // Also, should this be applied to the playerVel?
            //**************************************************************************
            m_vel -= m_vel * m_airResistance; //Applies air resistance

            //Update vel based on accel
            m_vel.X += m_accel.X / GlobalVariables.Settings.fps;
            m_vel.Y += m_accel.Y / GlobalVariables.Settings.fps;

            //Update playerVel based on playerAccel
            m_playerVel.X += m_playerAccel.X / GlobalVariables.Settings.fps;
            m_playerVel.Y += m_playerAccel.Y / GlobalVariables.Settings.fps;

            //Update position based on vel
            m_rect.X += (m_vel.X + m_playerVel.X) / GlobalVariables.Settings.fps;
            m_rect.Y += (m_vel.Y + m_playerVel.Y) / GlobalVariables.Settings.fps;


            m_prevVel = m_vel; //Update the previous velocity

            ResetPerLoopValues();
        }

        //**************************************************
        // Method: ApplyContactFriction
        //
        // Purpose: Used to apply the friction on the
        // object's x and y axis which was accumulated
        // during collision checking
        //**************************************************
        public override void ApplyContactFriction()
        {
            //Restricts the x and y contact friction values to a number between 0 and 1, inclusive
            MathHelper.Clamp(m_contactFriction.Y, 0, 1);
            MathHelper.Clamp(m_contactFriction.X, 0, 1);
            #region Apply Friction To Y Axis
            //Determines the direction the object is moving in.
            if(m_vel.Y > 0)
            {
                //**********************************************************************
                // This calculates the change in speed due to friction.
                // totalFriction acts as the coefficent of friction for the equation
                // which calculates frictional force. The rest of the equation I
                // created by looking up and remembering the physics equations for
                // impulse and force, then I divided by mass to find the accleration
                // that I should apply.
                //**********************************************************************
                m_vel.Y -= (m_contactFriction.Y * (m_prevVel.X - m_vel.X + m_accel.X)) / GlobalVariables.Settings.fps;
                if (m_vel.Y < 0)
                {
                    m_vel.Y = 0; //This prevents friction from changing an object's direction of movement
                }
            }
            else if (m_vel.Y < 0)
            {
                //**********************************************************************
                // This calculates the change in speed due to friction.
                // totalFriction acts as the coefficent of friction for the equation
                // which calculates frictional force. The rest of the equation I
                // created by looking up and remembering the physics equations for
                // impulse and force, then I divided by mass to find the accleration
                // that I should apply.
                //**********************************************************************
                m_vel.Y += (m_contactFriction.Y * (m_prevVel.X - m_vel.X + m_accel.X)) / GlobalVariables.Settings.fps;
                if (m_vel.Y > 0)
                {
                    m_vel.Y = 0; //This prevents friction from changing an object's direction of movement
                }
            }
            #endregion

            #region Apply Friction To X Axis
            //Determines the direction the object is moving in.
            if (m_vel.X > 0)
            {
                //**********************************************************************
                // This calculates the change in speed due to friction.
                // totalFriction acts as the coefficent of friction for the equation
                // which calculates frictional force. The rest of the equation I
                // created by looking up and remembering the physics equations for
                // impulse and force, then I divided by mass to find the accleration
                // that I should apply.
                //**********************************************************************
                m_vel.X -= (m_contactFriction.X * (m_prevVel.Y - m_vel.Y + m_accel.Y)) / GlobalVariables.Settings.fps;
                if (m_vel.X < 0)
                {
                    m_vel.X = 0; //This prevents friction from changing an object's direction of movement
                }
            }
            else if (m_vel.X < 0)
            {
                //**********************************************************************
                // This calculates the change in speed due to friction.
                // totalFriction acts as the coefficent of friction for the equation
                // which calculates frictional force. The rest of the equation I
                // created by looking up and remembering the physics equations for
                // impulse and force, then I divided by mass to find the accleration
                // that I should apply.
                //**********************************************************************
                m_vel.X += (m_contactFriction.X * (m_prevVel.Y - m_vel.Y + m_accel.Y)) / GlobalVariables.Settings.fps;
                if (m_vel.X > 0)
                {
                    m_vel.X = 0; //This prevents friction from changing an object's direction of movement
                }
            }
            #endregion
        }

        //**************************************************
        // Method: Copy
        //
        // Purpose: To make a deep copy of this object so
        // that, during collisions the system can stay
        // consistent.
        //**************************************************
        public override PhysicsObj DeepClone()
        {
            return (new PlayerPhysicsObj(m_vel, m_accel, m_mass, m_friction, m_airResistance, m_restitution, m_rect, m_sided, m_applyMomentum, m_playerVel, m_playerAccel));
        }

        #region Properties

        #region Accel
        public override Vector2 Accel
        {
            get
            {
                return (m_accel + m_playerAccel);
            }
        }

        public override float AccelY
        {
            get
            {
                return m_accel.Y;
            }
            set
            {
                m_accel.Y = value;
            }
        }

        public override float AccelX
        {
            get
            {
                return m_accel.X;
            }
            set
            {
                m_accel.X = value;
            }
        }
        #endregion

        #region Vel
        public override Vector2 Vel
        {
            get
            {
                return (m_vel + m_playerVel);
            }
            set
            {
                m_vel = value;
            }
        }

        public override float VelY
        {
            get
            {
                return m_vel.Y;
            }
            set
            {
                m_vel.Y = value;
            }
        }

        public override float VelX
        {
            get
            {
                return m_vel.X;
            }
            set
            {
                m_vel.X = value;
            }
        }
        #endregion

        #region Player Accel
        public virtual Vector2 PlayerAccel
        {
            get
            {
                return m_playerAccel;
            }
        }

        public virtual float PlayerAccelY
        {
            get
            {
                return m_playerAccel.Y;
            }
            set
            {
                m_playerAccel.Y = value;
            }
        }

        public virtual float PlayerAccelX
        {
            get
            {
                return m_playerAccel.X;
            }
            set
            {
                m_playerAccel.X = value;
            }
        }
        #endregion

        #region Player Vel
        public virtual Vector2 PlayerVel
        {
            get
            {
                return m_playerVel;
            }
            set
            {
                m_playerVel = value;
            }
        }

        public virtual float PlayerVelY
        {
            get
            {
                return m_playerVel.Y;
            }
            set
            {
                m_playerVel.Y = value;
            }
        }

        public virtual float PlayerVelX
        {
            get
            {
                return m_playerVel.X;
            }
            set
            {
                m_playerVel.X = value;
            }
        }
        #endregion

        #endregion
    }
}
