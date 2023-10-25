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
    //Used for determining the side an object struck another object
    enum Side { left, right, bottom, top };

    //Used for determining the axis on which a collision occured
    enum Axis { y, x };

    class PhysicsObj
    {

        #region Declartions
        protected bool m_applyMomentum; //Used to allow objects to ignore momentum
        
        protected FloatRectangle m_rect; //Stores the collision box for the object
        protected Vector2 m_prevPos; //Stores previous position for the purpose of my current attempt at responding to collision

        protected float m_mass; //Stores object mass
        protected Vector2 m_accel; //Stores object acceleration
        protected Vector2 m_vel; //Stores object speed
        protected Vector2 m_prevVel; //Stores the objects speed from the frame before

        protected Vector2 m_contactFriction; //Stores the currect x-axis and y-axis total friction

        protected Dictionary<Side, Boolean> m_sided;  //Dictionary of flags stating whether the object is being
        //touched on any of its four sides

        protected float m_restitution; //Number between 0 and 1 that determines how much energy will be retained
        //after colliding with an object

        protected float m_friction;   //Number between 0 and 1 which determines how much friction this object has
        //Depending on the type of object and action it may affect it differently.
        //I.E. players tying to walk compared to trying to stop.
        //The greater the number, the greater the resistance.

        protected Vector2 m_airResistance;    //Stores the air resistance of the object. The airResistance will be between
        //0 and 1. The higher the number the less the resistance.


        #endregion


        //****************************************************
        // Method: PhysicsObject
        //
        // Purpose: Default PhysicsObject constructor
        //****************************************************
        public PhysicsObj()
        {
            //Position and movement initializations
            m_vel = Vector2.Zero;
            m_accel = Vector2.Zero;

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
        // Method: PhysicsObject
        //
        // Purpose: PhysicsObject constructor
        //****************************************************
        public PhysicsObj(Vector2 position, Vector2 velocity, Vector2 dimensions, Vector2 acceleration, float Mass, float friction, Vector2 airResistance, float restitution, bool applyMomentum)
        {
            //Position and movement initializations
            m_vel = velocity;
            m_accel = acceleration;

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
        // Method: PhysicsObject
        //
        // Purpose: PhysicsObject constructor for Copy
        //****************************************************
        public PhysicsObj(Vector2 velocity, Vector2 acceleration, float Mass, float friction, Vector2 airResistance, float restitution, FloatRectangle rect, Dictionary<Side, bool> sided, bool applyMomentum)
        {
            //Position and movement initializations
            m_vel = velocity;
            m_accel = acceleration;

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
        public virtual void Update(GameTime gameTime, Viewport port)
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
            // function.
            //**************************************************************************
            m_vel -= m_vel * m_airResistance; //Applies air resistance

            //Update vel based on accel
            m_vel.X += m_accel.X / GlobalVariables.Settings.fps;
            m_vel.Y += m_accel.Y / GlobalVariables.Settings.fps;

            //Update position based on vel
            m_rect.X += m_vel.X / GlobalVariables.Settings.fps;
            m_rect.Y += m_vel.Y / GlobalVariables.Settings.fps;


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
        public virtual void ApplyContactFriction()
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
        // Method: ResetPerLoopValues
        //
        // Purpose: Used to reset all local variables which
        // must be recalculated each time through the
        // loop. For example, the m_contactFriction
        // x and y values are reset so that the
        // accumulation of friction can start over.
        //**************************************************
        public virtual void ResetPerLoopValues()
        {
            //resets whether the object is touching anything on any sides or not
            m_sided[Side.bottom] = false;
            m_sided[Side.top] = false;
            m_sided[Side.left] = false;
            m_sided[Side.right] = false;

            m_contactFriction.X = 0;
            m_contactFriction.Y = 0;
        }

        //**************************************************
        // Method: Copy
        //
        // Purpose: To make a deep copy of this object so
        // that, during collisions the system can stay
        // consistent.
        //**************************************************
        public virtual PhysicsObj DeepClone()
        {
            return (new PhysicsObj(m_vel, m_accel, m_mass, m_friction, m_airResistance, m_restitution, m_rect, m_sided, m_applyMomentum));
        }

        #region Properties
        //Properties
        public float Mass
        {
            get
            {
                return m_mass;
            }

            set
            {
                m_mass = value;
            }
        }

        public virtual Vector2 Accel
        {
            get
            {
                return m_accel;
            }
        }

        public virtual float AccelY
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

        public virtual float AccelX
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

        public virtual Vector2 Vel
        {
            get
            {
                return m_vel;
            }
            set
            {
                m_vel = value;
            }
        }

        public virtual float VelY
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

        public virtual float VelX
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

        public virtual Vector2 PrevVel
        {
            get
            {
                return m_prevVel;
            }
            set
            {
                m_prevVel = value;
            }
        }

        public virtual float PrevVelY
        {
            get
            {
                return m_prevVel.Y;
            }
            set
            {
                m_prevVel.Y = value;
            }
        }

        public virtual float PrevVelX
        {
            get
            {
                return m_prevVel.X;
            }
            set
            {
                m_prevVel.X = value;
            }
        }

        public Dictionary<Side, Boolean> Sided
        {
            get
            {
                return m_sided;
            }
        }

        public float Friction
        {
            get
            {
                return m_friction;
            }
            set
            {
                m_friction = value;
            }
        }

        public float Restitution
        {
            get
            {
                return m_restitution;
            }

            set
            {
                m_restitution = value;
            }
        }

        public FloatRectangle Rect
        {
            get
            {
                return m_rect;
            }
        }

        public Vector2 PrevPos
        {
            get
            {
                return m_prevPos;
            }
        }

        public virtual Vector2 Pos
        {
            get
            {
                return new Vector2(m_rect.X, m_rect.Y);
            }
            set
            {
                m_rect.X = value.X;
                m_rect.Y = value.Y;
            }
        }

        public virtual float PosX
        {
            get
            {
                return m_rect.X;
            }
            set
            {
                m_rect.X = value;
            }
        }

        public virtual float PosY
        {
            get
            {
                return m_rect.Y;
            }
            set
            {
                m_rect.Y = value;
            }
        }

        public virtual Vector2 ContactFriction
        {
            get
            {
                return m_contactFriction;
            }

            set
            {
                m_contactFriction = value;
            }
        }

        public virtual float ContactFrictionX
        {
            get
            {
                return m_contactFriction.X;
            }

            set
            {
                m_contactFriction.X = value;
            }
        }

        public virtual float ContactFrictionY
        {
            get
            {
                return m_contactFriction.Y;
            }

            set
            {
                m_contactFriction.Y = value;
            }
        }

        public bool ApplyMomentum
        {
            get
            {
                return m_applyMomentum;
            }
            set
            {
                m_applyMomentum = value;
            }
        }
        #endregion
    }
}


