//******************************************************
// File: MenuCharacterLoadout.cs
//
// Purpose: Contains the definition of the
// CharacterLoadout. This will be used to store all of
// the information regarding the player's current
// loadout.
// (For use in menus)
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
    class MenuCharacterLoadout
    {
        #region Declarations
        protected Dictionary<string, AbilityInfo> m_abilityDict;

        protected string m_avatar; //This is the name of the avatar they're using
        protected int m_exp; //This is their current exp
        #endregion

        //********************************************************
        // Method: MenuCharacterLoadout
        //
        // Purpose: Default constructor for MenuCharacterLoadout
        //********************************************************
        public MenuCharacterLoadout()
        {
            m_abilityDict = new Dictionary<string, AbilityInfo>();
            m_abilityDict.Add("MainHand", new AbilityInfo());
            m_abilityDict.Add("OffHand", new AbilityInfo());
            m_abilityDict.Add("Passive", new AbilityInfo());

            m_abilityDict.Add("Special1", new AbilityInfo());
            m_abilityDict.Add("Special2", new AbilityInfo());
            m_abilityDict.Add("Special3", new AbilityInfo());
        }

        #region Properties
        public Dictionary<string, AbilityInfo> AbilityDict
        {
            get
            {
                return m_abilityDict;
            }
        }

        public string MainHand
        {
            get
            {
                return m_abilityDict["MainHand"].Name;
            }
            set
            {
                m_abilityDict["MainHand"] = GlobalVariables.AbilityDict["MainHand"][value];
            }
        }

        public string OffHand
        {
            get
            {
                return m_abilityDict["OffHand"].Name;
            }
            set
            {
                m_abilityDict["OffHand"] = GlobalVariables.AbilityDict["OffHand"][value];
            }
        }

        public string Passive
        {
            get
            {
                return m_abilityDict["Passive"].Name;
            }
            set
            {
                m_abilityDict["Passive"] = GlobalVariables.AbilityDict["Passive"][value];
            }
        }


        public string Special1
        {
            get
            {
                return m_abilityDict["Special1"].Name;
            }
            set
            {
                if (Special1 != "" && value != "None")
                {
                    if (m_abilityDict["Special2"].Name == value)
                    {
                        m_abilityDict["Special2"] = m_abilityDict["Special1"];
                        NetworkingHandler.UpdateSelectedAbility("Special2", Special2);
                    }
                    if (m_abilityDict["Special3"].Name == value)
                    {
                        m_abilityDict["Special3"] = m_abilityDict["Special1"];
                        NetworkingHandler.UpdateSelectedAbility("Special3", Special3);
                    }
                }

                m_abilityDict["Special1"] = GlobalVariables.AbilityDict["Special"][value];
            }
        }

        public string Special2
        {
            get
            {
                return m_abilityDict["Special2"].Name;
            }
            set
            {
                if (Special2 != "" && value != "None")
                {
                    if (m_abilityDict["Special1"].Name == value)
                    {
                        m_abilityDict["Special1"] = m_abilityDict["Special2"];
                        NetworkingHandler.UpdateSelectedAbility("Special1", Special1);
                    }
                    if (m_abilityDict["Special3"].Name == value)
                    {
                        m_abilityDict["Special3"] = m_abilityDict["Special2"];
                        NetworkingHandler.UpdateSelectedAbility("Special3", Special3);
                    }
                }

                m_abilityDict["Special2"] = GlobalVariables.AbilityDict["Special"][value];
            }
        }

        public string Special3
        {
            get
            {
                return m_abilityDict["Special3"].Name;
            }
            set
            {
                if (Special3 != "" && value != "None")
                {
                    if (m_abilityDict["Special2"].Name == value)
                    {
                        m_abilityDict["Special2"] = m_abilityDict["Special3"];
                        NetworkingHandler.UpdateSelectedAbility("Special2", Special2);
                    }
                    if (m_abilityDict["Special1"].Name == value)
                    {
                        m_abilityDict["Special1"] = m_abilityDict["Special3"];
                        NetworkingHandler.UpdateSelectedAbility("Special1", Special1);
                    }
                }

                m_abilityDict["Special3"] = GlobalVariables.AbilityDict["Special"][value];
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
        #endregion
    }
}
