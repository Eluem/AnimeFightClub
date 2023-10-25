//******************************************************
// File: CharacterLoadout.cs
//
// Purpose: Contains the definition of the
// CharacterLoadout. This will be used to store all of
// the information regarding the player's current
// loadout.
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
    class CharacterLoadout
    {
        #region Declarations
        protected Dictionary<string, Ability> m_abilityDict;

        protected string m_avatar; //This is the name of the avatar they're using
        protected int m_exp; //This is their current exp

        protected PlayerObj m_ownerObj;
        #endregion

        //****************************************************
        // Method: NetInitialize
        //
        // Purpose: Initializes values based on the 
        // mesage sent by the master server
        //****************************************************
        public virtual void NetInitialize(NetIncomingMessage msg)
        {
            m_abilityDict["MainHand"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["MainHand"].OwnerObj = m_ownerObj;

            m_abilityDict["OffHand"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["OffHand"].OwnerObj = m_ownerObj;

            m_abilityDict["Passive"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["Passive"].OwnerObj = m_ownerObj;

            m_abilityDict["Special1"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["Special1"].OwnerObj = m_ownerObj;

            m_abilityDict["Special2"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["Special2"].OwnerObj = m_ownerObj;

            m_abilityDict["Special3"] = (Ability)Activator.CreateInstance(GlobalVariables.GetAbilityTypeFromString(msg.ReadString()));
            m_abilityDict["Special3"].OwnerObj = m_ownerObj;

            m_avatar = GlobalVariables.RemoveWhiteSpaces(msg.ReadString());
        }

        //****************************************************
        // Method: NetSendMe
        //
        // Purpose: Accepts a NetOutgoingMessage and writes
        // the approraite data to it
        //****************************************************
        public virtual void NetSendMe(NetOutgoingMessage sendMsg)
        {
            sendMsg.Write(m_abilityDict["MainHand"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_abilityDict["OffHand"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_abilityDict["Passive"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_abilityDict["Special1"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_abilityDict["Special2"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_abilityDict["Special3"].GetType().ToString().Split('.')[1]);
            sendMsg.Write(m_avatar);
        }

        //****************************************************
        // Method: CharacterLoadout
        //
        // Purpose: Default constructor for CharacterLoadout
        //****************************************************
        public CharacterLoadout(PlayerObj ownerObj)
        {
            m_ownerObj = ownerObj;
            m_abilityDict = new Dictionary<string, Ability>();
            m_abilityDict.Add("MainHand", null);
            m_abilityDict.Add("OffHand", null);
            m_abilityDict.Add("Passive", null);

            m_abilityDict.Add("Special1", null);
            m_abilityDict.Add("Special2", null);
            m_abilityDict.Add("Special3", null);
        }

        //****************************************************
        // Method: FullReset
        //
        // Purpose: Fully resets all abilities
        //****************************************************
        public void FullReset()
        {
            foreach (KeyValuePair<string, Ability> stringAbilityPair in m_abilityDict)
            {
                stringAbilityPair.Value.FullReset();
            }
        }

        //****************************************************
        // Method: Interrupt
        //
        // Purpose: Interrupts all abilities
        //****************************************************
        public void Interrupt()
        {
            foreach (KeyValuePair<string, Ability> stringAbilityPair in m_abilityDict)
            {
                stringAbilityPair.Value.Interrupt();
            }
        }

        #region Properties
        public Dictionary<string, Ability> AbilityDict
        {
            get
            {
                return m_abilityDict;
            }
        }

        public Ability MainHand
        {
            get
            {
                return m_abilityDict["MainHand"];
            }
            set
            {
                m_abilityDict["MainHand"] = value;
            }
        }

        public Ability OffHand
        {
            get
            {
                return m_abilityDict["OffHand"];
            }
            set
            {
                m_abilityDict["OffHand"] = value;
            }
        }

        public Ability Passive
        {
            get
            {
                return m_abilityDict["Passive"];
            }
            set
            {
                m_abilityDict["Passive"] = value;
            }
        }

        public Ability Special1
        {
            get
            {
                return m_abilityDict["Special1"];
            }
            set
            {
                m_abilityDict["Special1"] = value;
            }
        }

        public Ability Special2
        {
            get
            {
                return m_abilityDict["Special2"];
            }
            set
            {
                m_abilityDict["Special2"] = value;
            }
        }

        public Ability Special3
        {
            get
            {
                return m_abilityDict["Special3"];
            }
            set
            {
                m_abilityDict["Special3"] = value;
            }
        }

        public string Avatar
        {
            get
            {
                return m_avatar;
            }
            set
            {
                m_avatar = value;
            }
        }

        public int Exp
        {
            get
            {
                return m_exp;
            }

            set
            {
                m_exp = value;
            }
        }

        public bool Casting
        {
            get
            {
                bool tempBool = false;
                foreach (KeyValuePair<string, Ability> stringAbilityPair in m_abilityDict)
                {
                    if (stringAbilityPair.Value.Casting)
                    {
                        tempBool = true;
                        break;
                    }
                }
                return tempBool;
            }
        }
        #endregion
    }
}
