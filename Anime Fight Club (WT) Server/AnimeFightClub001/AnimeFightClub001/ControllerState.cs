//******************************************************
// File: ControllerState.cs
//
// Purpose: The ControllerState class will be used to
// assist in the creation of an abstraction between
// the input device that the user is using and the
// Player they are controlling, different menus, ect.
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
    enum Control { MoveLeft, MoveRight, Run, Jump, AimLeft, AimRight, AimUp, AimDown, OffHand, MainHand, GrabThrowItem, UseItem, Crouch, AbilityOne, AbilityTwo, AbilityThree, Start, Back };

    class ControllerState
    {
        #region Declarations
        private bool[] m_ctrlArray;
        #endregion


        //****************************************************
        // Method: ControllerState
        //
        // Purpose: ControllerState constructor
        //****************************************************
        public ControllerState()
        {
            m_ctrlArray = new bool[GlobalVariables.NumberOfControls];
            for (int i = 0; i < GlobalVariables.NumberOfControls; ++i )
            {
                m_ctrlArray[i] = false;
            }
        }



        //****************************************************
        // Method: isControlPressed
        //
        // Purpose: Returns whether or not a particular 
        // control is being held down by the user currently
        //****************************************************
        public bool isControlPressed(Control ctrl)
        {
            return m_ctrlArray[(int)ctrl];
        }


        //****************************************************
        // Method: getControlAsInt16
        //
        // Purpose: This is used to get the control state
        // for sending over the network. It doesn't
        // include start and back
        //****************************************************
        public UInt16 getControlAsInt16()
        {
            string tempBinaryString = "";

            for (int i = 0; i < 16; ++i)
            {
                if (m_ctrlArray[i])
                {
                    tempBinaryString = "1" + tempBinaryString;
                }
                else
                {
                    tempBinaryString = "0" + tempBinaryString;
                }
            }


            return Convert.ToUInt16(tempBinaryString, 2);
        }


        //****************************************************
        // Method: setControlStateAsInt16
        //
        // Purpose: This is used to set the control state
        // by converting a 16 bit integer into an array
        // of boolean values
        //****************************************************
        public void setControlStateAsInt16(UInt16 inInt)
        {
            int i;

            //Resets all values to false
            for (i = 0; i < GlobalVariables.NumberOfNetworkedControls; ++i)
            {
                m_ctrlArray[i] = false;
            }

            i = 0;
            while (inInt > 0)
            {
                //Even number
                if (inInt % 2 == 0) 
                {
                    m_ctrlArray[i] = false;
                }
                //Odd Number
                else 
                {
                    m_ctrlArray[i] = true;
                    inInt--;
                }

                inInt /= 2;
                ++i;
            }
        }

        //****************************************************
        // Method: SetControl
        //
        // Purpose: Sets the value of a control
        //****************************************************
        public void SetControl(Control ctrl, bool value)
        {
            m_ctrlArray[(int)ctrl] = value;
        }


        //****************************************************
        // Method: Copy
        //
        // Purpose: Allows you to set one controllerState
        // equal to another controllerState by value
        //****************************************************
        public void Copy(ControllerState ctrlState)
        {
            for (int i = 0; i < GlobalVariables.NumberOfControls; ++i)
            {
                this.m_ctrlArray[i] = ctrlState.m_ctrlArray[i];
            }
        }


        //****************************************************
        // Method: ==
        //
        // Purpose: Overrides == operator
        //****************************************************
        public static bool operator ==(ControllerState ctrlState1, ControllerState ctrlState2)
        {
            for (int i = 0; i < GlobalVariables.NumberOfControls; ++i)
            {
                if (ctrlState1.m_ctrlArray[i] != ctrlState2.m_ctrlArray[i])
                    return false;
            }

            return true;
        }

        //****************************************************
        // Method: !=
        //
        // Purpose: Overrides != operator
        //****************************************************
        public static bool operator !=(ControllerState ctrlState1, ControllerState ctrlState2)
        {
            for (int i = 0; i < GlobalVariables.NumberOfControls; ++i)
            {
                if (ctrlState1.m_ctrlArray[i] != ctrlState2.m_ctrlArray[i])
                    return true;
            }

            return false;
        }

        //Properties
        public bool[] ControlArray
        {
            get
            {
                return m_ctrlArray;
            }
        }

    }
}
