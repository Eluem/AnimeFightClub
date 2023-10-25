//******************************************************
// File: Ability.cs
//
// Purpose: Defines the Ability base class which will
// be inherited by all MainWeapon, OffHand, Special
// and Passive classes.
//
// Written By: Salvatore Hanusiewicz, Rob Maggio
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
    class Ability : iDrawableObj
    {
        #region Declarations
        protected int m_maxAmmo;
        protected int m_currAmmo;
        protected int m_maxEnergy;
        protected float m_energyRegenRate;
        protected float m_currEnergy;
        protected float m_currHeat;
        protected float m_coolRate;
        protected List<int> m_heatThreshHolds;

        protected PlayerObj m_ownerObj;

        protected bool m_isPressed;
        protected bool m_ArbitraryFlag;
        protected float m_pressedDuration;
        protected float m_releasedDuration;
        protected float m_ArbitraryTimer;

        protected Vector2 m_offSet;

        protected int m_state;
        protected int m_prevState;

        protected int m_castTimer;
        protected int m_castWaitTimer; //Time to make animations look nice, action.None is automatically reapplied after this timer

        protected Sprite m_drawableObj;
        #endregion

        //****************************************************
        // Method: Ability
        //
        // Purpose: Default constructor for Ability
        //****************************************************
        public Ability(PlayerObj owner = null, int maxAmmo = 0, int maxEnergy = 0, float EnergyRegenRate = 0, float coolRate = 0, string spriteName = "None", ObjectLayer objectLayer = ObjectLayer.AbilityBelow)
        {
            m_ownerObj = owner;
            m_currAmmo = m_maxAmmo = maxAmmo;
            m_currEnergy = m_maxEnergy = maxEnergy;
            m_energyRegenRate = EnergyRegenRate;
            m_coolRate = coolRate;
            m_currHeat = 0;

            m_offSet = Vector2.Zero;

            m_state = 0;
            m_prevState = 0;

            m_castTimer = 0;
            m_castWaitTimer = 0;

            m_drawableObj = new Sprite(spriteName, Color.White, this, GlobalVariables.GetLayer(objectLayer));
        }

        //****************************************************
        // Method: FullReset
        //
        // Purpose: Fully resets the ability
        //****************************************************
        public virtual void FullReset()
        {
            m_pressedDuration = 0;
            m_releasedDuration = 0;
            m_isPressed = false;

            m_currAmmo = m_maxAmmo;
            m_currEnergy = m_maxEnergy;
            m_currHeat = 0;

            m_state = 0;
            m_prevState = 0;

            m_castTimer = 0;
            m_castWaitTimer = 0;
        }

        //****************************************************
        // Method: Interrupt
        //
        // Purpose: Interrupts the ability
        //****************************************************
        public virtual void Interrupt()
        {
            m_pressedDuration = 0;
            m_releasedDuration = 0;
            m_isPressed = false;

            m_castTimer = 0;
            m_castWaitTimer = 0;

            m_ownerObj.Action = PlayerAction.None;
        }

        //****************************************************
        // Method: PressedAbility
        //
        // Purpose: Called by the player when they begin to
        // hold the button which corresponds with this ability
        //****************************************************
        public virtual void PressedAbility()
        {
            if (!m_isPressed)
            {
                m_isPressed = true;
                PressedHook();
                m_releasedDuration = 0;
            }
        }
        //****************************************************
        // Method: PressedHook
        //
        // Purpose: Overrideable hook for PressedAbility.
        //****************************************************
        public virtual void PressedHook()
        {
            //does nothing by default.
        }

        //****************************************************
        // Method: ReleasedAbility
        //
        // Purpose: Called by the player when they release
        // the button which corresponds with this ability
        //****************************************************
        public virtual void ReleasedAbility()
        {
            if (m_isPressed)
            {
                m_isPressed = false;
                ReleasedHook();
                m_pressedDuration = 0;
            }
        }
        //****************************************************
        // Method: ReleasedHook
        //
        // Purpose: Overrideable hook for ReleasedAbility.
        //****************************************************
        public virtual void ReleasedHook()
        {
            //does nothing by default.
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Called in the player's Update function.
        // Runs all the code which will keep track of timers,
        // energy, ammo, ect.
        //****************************************************
        public virtual void Update(GameTime gameTime)
        {
            if (m_currHeat > 0)
                m_currHeat -= m_coolRate * gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (m_currEnergy < m_maxEnergy)
                m_currEnergy += m_energyRegenRate * gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (m_currHeat < 0)
                m_currHeat = 0;

            if (m_currEnergy > m_maxEnergy)
                m_currEnergy = m_maxEnergy;

            if (m_isPressed)
                m_pressedDuration += gameTime.ElapsedGameTime.Milliseconds;
            else
                m_releasedDuration += gameTime.ElapsedGameTime.Milliseconds;


            if (m_ArbitraryFlag)
                m_ArbitraryTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (m_castTimer > 0)
            {
                m_castTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (m_castTimer <= 0)
                {
                    CastHook();
                }
            }

            if (m_castWaitTimer > 0)
            {
                m_castWaitTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (m_castWaitTimer <= 0)
                {
                    CastWaitHook();
                }
            }

            UpdateHook(gameTime);

            m_drawableObj.Update(gameTime);

            m_prevState = m_state;
        }


        //****************************************************
        // Method: UpdateHook
        //
        // Purpose: Hooks into the update function
        //****************************************************
        public virtual void UpdateHook(GameTime gameTime)
        {
            //does nothing by default.
        }

        //****************************************************
        // Method: CastHook
        //
        // Purpose: Hooks into what occurs when the cast timer
        // is hits zero
        //****************************************************
        public virtual void CastHook()
        {
            //Does nothing by default
        }

        //****************************************************
        // Method: CastWaitHook
        //
        // Purpose: Hooks into what occurs when the 
        // cast wait timer hits zero
        //****************************************************
        public virtual void CastWaitHook()
        {
            //Does nothing by default
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws anything which corresponds to this
        // ability. (Not entirely sure if this should be
        // implemented)
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            m_drawableObj.Draw(spriteBatch);
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

        public Vector2 Center
        {
            get
            {
                return new Vector2(m_ownerObj.Center.X + m_offSet.X, m_ownerObj.Center.Y + m_offSet.Y);
            }
        }

        public Direction Direction
        {
            get
            {
                return m_ownerObj.Direction;
            }
            set
            {
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

        public bool Casting
        {
            get
            {
                return (m_castTimer > 0 || m_isPressed);// || m_castWaitTimer > 0);
            }
        }
        #endregion
    }
}
