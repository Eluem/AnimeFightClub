//******************************************************
// File: Controller.cs
//
// Purpose: The Controller class will be used to
// create an abstraction of the controller that the
// actual user is sending inputs on and the rest of
// the game. It allows the game to map different
// input (depending on the selected controller) to
// values in the controllerState class.
//
// This is the client version (sends an update to the
// server for every change)
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
    enum ControllerType { Keyboard, Gamepad };
    class Controller
    {
        #region Declarations
        //Supported input device states

        //GamePad
        private GamePadState m_currGamePadState;
        private GamePadState m_prevGamePadState;

        //Keyboard
        private KeyboardState m_currKeyboardState;
        private KeyboardState m_prevKeyboardState;

        //Used to count how long it's been since the player completed each step in initiating the double tap run
        private int m_keyboardRunTimer;

        //The step the player is at in the double tap running process -1 to 2 (not started, key down, key up, key down)
        private int m_keyboardRunStep;

        private string m_doubleTapDirection; //left or right

        private Control m_keyboardRunDirection; //MoveLeft or MoveRight, depending on the previous key pressed to begin running


        //Abstracted controller states to be used by Player objects and menus
        private ControllerState m_currControllerState;
        private ControllerState m_prevControllerState;

        //Player that this controller is bound to
        private PlayerIndex m_playerIndex;

        //Controller type
        private ControllerType m_controllerType;

        public bool Networked;
        #endregion


        //****************************************************
        // Method: Controller
        //
        // Purpose: Controller constructor
        //****************************************************
        public Controller(PlayerIndex playerIndex, ControllerType controllerType)
        {
            m_playerIndex = playerIndex;
            m_controllerType = controllerType;

            m_currControllerState = new ControllerState();
            m_prevControllerState = new ControllerState();

            m_keyboardRunTimer = 0;

            Networked = false;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the controller based on user's
        // inputs and sends any changes to the server.
        //****************************************************
        public void Update(GameTime gameTime)
        {

            int i; //To be used in for loops

            #region Initial Control State Update
            if (m_controllerType == ControllerType.Gamepad)
            {
                m_currGamePadState = GamePad.GetState(m_playerIndex);
                for (i = 0; i < GlobalVariables.NumberOfControls; ++i)
                {
                    m_currControllerState.SetControl((Control)i, m_currGamePadState.IsButtonDown(GlobalVariables.Settings.GamePadBindings[m_playerIndex][(Control)i]));
                }
                
            }
            else
            {
                m_currKeyboardState = Keyboard.GetState();

                for(i = 0; i <GlobalVariables.NumberOfControls; ++i)
                {
                    m_currControllerState.SetControl((Control)i, m_currKeyboardState.IsKeyDown(GlobalVariables.Settings.KeyboardBindings[m_playerIndex][(Control)i]));
                }
                if (m_currControllerState.isControlPressed(Control.MoveLeft) && m_currControllerState.isControlPressed(Control.MoveRight))
                {
                    m_currControllerState.SetControl(Control.MoveLeft, false);
                    m_currControllerState.SetControl(Control.MoveRight, false);
                }
                
                #region Handle Double Tap To Run
                if (m_keyboardRunStep == 1)//m_keyboardRunStep != -1 && m_keyboardRunStep != 2)
                {
                    m_keyboardRunTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (m_keyboardRunTimer >= 90)
                    {
                        m_keyboardRunStep = -1;
                        m_keyboardRunTimer = 0;
                    }
                }

                #region Right
                if ((m_currControllerState.isControlPressed(Control.MoveRight) && !m_prevControllerState.isControlPressed(Control.MoveRight) && (m_keyboardRunStep == -1 || m_keyboardRunStep == 1)) || (!m_currControllerState.isControlPressed(Control.MoveRight) && m_prevControllerState.isControlPressed(Control.MoveRight) && m_keyboardRunStep == 0)) 
                {
                    if (m_doubleTapDirection != "right")
                    {
                        m_keyboardRunStep = -1;
                        m_keyboardRunTimer = 0;
                    }

                    m_doubleTapDirection = "right";

                    if(m_keyboardRunStep < 2)
                        ++m_keyboardRunStep;
                }

                if (m_doubleTapDirection == "right")
                {
                    //Resets if either key is released
                    if (!m_currControllerState.isControlPressed(Control.MoveRight) && m_keyboardRunStep == 2)
                    {
                        m_keyboardRunStep = -1;
                        m_keyboardRunTimer = 0;
                    }
                }

                #endregion

                #region Left
                if ((m_currControllerState.isControlPressed(Control.MoveLeft) && !m_prevControllerState.isControlPressed(Control.MoveLeft) && (m_keyboardRunStep == -1 || m_keyboardRunStep == 1)) || (!m_currControllerState.isControlPressed(Control.MoveLeft) && m_prevControllerState.isControlPressed(Control.MoveLeft) && m_keyboardRunStep == 0))
                {
                    if (m_doubleTapDirection != "left")
                    {
                        m_keyboardRunStep = -1;
                        m_keyboardRunTimer = 0;
                    }

                    m_doubleTapDirection = "left";

                    if (m_keyboardRunStep < 2)
                        ++m_keyboardRunStep;
                }

                if (m_doubleTapDirection == "left")
                {
                    //Resets if either key is released
                    if (!m_currControllerState.isControlPressed(Control.MoveLeft) && m_keyboardRunStep == 2)
                    {
                        m_keyboardRunStep = -1;
                        m_keyboardRunTimer = 0;
                    }
                }
                #endregion

                if (m_keyboardRunStep == 2)
                {
                    m_currControllerState.SetControl(Control.Run, true);
                    m_keyboardRunTimer = 0;
                }

                #endregion

            }
            #endregion

            #region Network Code
            if (m_currControllerState != m_prevControllerState && Networked)
            {
                //Send controller packet to server
                NetworkingHandler.SendControllerStateUpdate(this.ControllerState.getControlAsInt16());
            }
            #endregion
        }

        #region Properties
        public ControllerState ControllerState
        {
            get
            {
                return m_currControllerState;
            }
        }

        public ControllerState PrevControllerState
        {
            get
            {
                return m_prevControllerState;
            }
        }

        public ControllerType ControllerType
        {
            get
            {
                return m_controllerType;
            }
        }
        #endregion
    }
}
