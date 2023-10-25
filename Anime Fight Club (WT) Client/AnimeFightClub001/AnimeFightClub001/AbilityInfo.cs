//******************************************************
// File: AbilityInfo.cs
//
// Purpose: Used to store basic information about
// abilties. Primary use is to be stored in a list of
// abilities for the customization menu.
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
    class AbilityInfo
    {
        #region Declarations
        protected string m_name; //Name of the ability
        protected string m_category; //Category that the ability belongs to
        protected string m_description; //Description of the ability
        protected int m_cost; //Cost to purchase the ability
        protected bool m_owned; //Flag indicating whether or not this user owns the ability

        #endregion

        //****************************************************
        // Method: AbilityInfo
        //
        // Purpose: Constructor for AbilityInfo
        //****************************************************
        public AbilityInfo(string name = "", string category = "", string description = "", int cost = 0, bool owned = false)
        {
            m_name = name;
            m_category = category;
            m_description = description;
            m_cost = cost;
            m_owned = owned;
        }


        #region Properties
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public string Category
        {
            get
            {
                return m_category;
            }
            set
            {
                m_category = value;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        public int Cost
        {
            get
            {
                return m_cost;
            }
            set
            {
                m_cost = value;
            }
        }

        public bool Owned
        {
            get
            {
                return m_owned;
            }
            set
            {
                m_owned = value;
            }
        }
        #endregion
    }
}
