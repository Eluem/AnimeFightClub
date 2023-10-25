//******************************************************
// File: GlobalVariables.cs
//
// Purpose: Contains the class definition for
// GlobalVariables. GlobalVariables is used to store
// any variables which all classes may need access to.
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
    static class GlobalVariables
    {
        //Defines the length of the control array (all control states must be boolean for this game)
        public const int NumberOfControls = 18;

        //Defines the number of controls in the control array which are to be sent to over network
        //Controls which will be networked must be first in the array
        public const int NumberOfNetworkedControls = 18;

        //I determined that this would be a good way to solve certain problems
        //such as having two objects with the exact same rectangle size, speed, and acceleration
        //collide and land on each other with their centers.
        private static Random m_randomizer = new Random(DateTime.Now.Millisecond);

        //Used for counting up the different objectIDs as they're added to the game
        private static UInt16 m_playerObjectIDCounter = 0;
        private static UInt16 m_itemObjectIDConter = 0;
        private static UInt16 m_hazardObjectIDConter = 0;
        private static UInt16 m_environmentalObjectIDConter = 0;
        private static UInt16 m_specialEnvironmentalObjectIDConter = 0;
        private static UInt16 m_basicObjectIDConter = 0;

        private static UInt32 m_currFrame = 0;

        #region Layer Count System
        //*********************************************************************
        // Explanation: Objects are drawn to one of 9 layer groups
        // (.1000, .2000, .3000, .4000, .5000, .6000, .7000, .8000, .9000)
        // There is a counter for each one of them which counts from 1 to
        // 998. This is done just incase I want to force something to be at the
        // bottom or top of that layer group. layer 0 and layer 1 are also
        // reserved.
        //*********************************************************************
        //I created this system to hopefully help with layering issues
        private static int[] layerCounter = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        
        //Add layer group enum?
        public static float GetLayer(int layerGroup)
        {
            float tempLayer = -1;
            if (layerGroup > 9 || layerGroup < 1)
                throw new SystemException("Layergroup out of range!");

            ++layerCounter[layerGroup - 1];
            if (layerCounter[layerGroup - 1] >= 999)
                layerCounter[layerGroup - 1] = 1;

            tempLayer = (layerGroup / 10f) + (layerCounter[layerGroup - 1] / 10000f);

            return tempLayer;
        }
        #endregion


        //***********************************************************************
        // Method: IncrementFrame
        //
        // Purpose: This increments the current frame by the
        // frameDelta value. This is used to allow for the
        // Bresenham Line Drawing concept that I read about
        // here: http://gafferongames.com/game-physics/networked-physics/
        //***********************************************************************
        public static void IncrementFrame()
        {
            ++m_currFrame;
        }

        #region Use Object ID Counters
        //**********************************************************
        // Method: UsePlayerObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UsePlayerObjectIDCounter()
        {
            ++m_playerObjectIDCounter;
            return m_playerObjectIDCounter;
        }

        //**********************************************************
        // Method: UseHazardObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UseHazardObjectIDCounter()
        {
            ++m_hazardObjectIDConter;
            return m_hazardObjectIDConter;
        }

        //**********************************************************
        // Method: UseEnvironmentalObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UseEnvironmentalObjectIDCounter()
        {
            ++m_environmentalObjectIDConter;
            return m_environmentalObjectIDConter;
        }

        //**********************************************************
        // Method: UseSpecialEnvironmentalObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UseSpecialEnvironmentalObjectIDCounter()
        {
            ++m_specialEnvironmentalObjectIDConter;
            return m_specialEnvironmentalObjectIDConter;
        }

        //**********************************************************
        // Method: UseItemObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UseItemObjectIDCounter()
        {
            ++m_itemObjectIDConter;
            return m_itemObjectIDConter;
        }

        //**********************************************************
        // Method: UseBasicObjectIDCounter
        //
        // Purpose: Increments and returns the id for the
        // class of objects
        //**********************************************************
        public static UInt16 UseBasicObjectIDCounter()
        {
            ++m_basicObjectIDConter;
            return m_basicObjectIDConter;
        }
        #endregion


        //Audio initializations
        /*
        static AudioEngine audioEngine = new AudioEngine(@"Content\SideScrollerSounds.xgs");
        static WaveBank waveBank = new WaveBank(audioEngine, @"Content\Wave Bank001.xwb");
        static SoundBank soundBank = new SoundBank(audioEngine, @"Content\Sound Bank001.xsb");
        */

        //Global dictionary used to access pointers to textures
        private static Dictionary<string, Texture2D> m_imageDict = new Dictionary<string, Texture2D>();

        static Dictionary<string, SpriteFont> m_fontDict = new Dictionary<string, SpriteFont>();

        //Global settings class (which isn't used as much as I wanted to...)
        private static Settings m_settings = new Settings();


        //****************************************************
        // Method: ConvertStringToEnum
        //
        // Purpose: Finds the integer in an enum which
        // corresponds with a string and returns it
        //****************************************************
        public static int ConvertStringToEnum(string value, Type enumType)
        {
            string[] enumArray = Enum.GetNames(enumType);
            int i;
            for (i = 0; i < enumArray.Length; ++i)
            {
                if (value == enumArray[i])
                    break;
            }
            return i;
        }


        //****************************************************
        // Method: RemoveWhiteSpaces
        //
        // Purpose: Accepts a string, removes all white
        // spacing and returns it
        //****************************************************
        public static string RemoveWhiteSpaces(string inString)
        {
            string[] tempStringArray = inString.Split(' ');
            inString = "";
            foreach (string s in tempStringArray)
            {
                inString += s;
            }
            return inString;
        }

        //******************************************************
        // Method: GetAbilityTypeFromString
        //
        // Purpose: Accepts a string, removes all white
        // spacing, appends AnimeFightClub001.Ability to the
        // beginning, attempts to get the type. If it can't
        // it returns it as type AnimeFightClub001.AbilityNone
        //******************************************************
        public static Type GetAbilityTypeFromString(string abilityName)
        {
            Type tempType = Type.GetType("AnimeFightClub001.Ability" + GlobalVariables.RemoveWhiteSpaces(abilityName));
            if (tempType == null)
                return Type.GetType("AnimeFightClub001.AbilityNone");
            return tempType;
        }


        #region Properties
        //Properties

        public static Settings Settings
        {
            get
            {
                return m_settings;
            }
        }

        public static Dictionary<string, Texture2D> ImageDict
        {
            get
            {
                return m_imageDict;
            }
        }

        public static Dictionary<string, SpriteFont> FontDict
        {
            get
            {
                return m_fontDict;
            }
        }

        public static Random Randomizer
        {
            get
            {
                return m_randomizer;
            }
        }


        public static UInt16 PlayerObjectIDCounter
        {
            get
            {
                return m_playerObjectIDCounter;
            }
        }

        public static UInt16 ItemObjectIDConter
        {
            get
            {
                return m_itemObjectIDConter;
            }
        }

        public static UInt16 HazardObjectIDConter
        {
            get
            {
                return m_hazardObjectIDConter;
            }
        }

        public static UInt16 EnvironmentalObjectIDConter
        {
            get
            {
                return m_environmentalObjectIDConter;
            }
        }

        public static UInt16 SpecialEnvironmentalObjectIDConter
        {
            get
            {
                return m_specialEnvironmentalObjectIDConter;
            }
        }

        public static UInt16 BasicObjectIDConter
        {
            get
            {
                return m_basicObjectIDConter;
            }
        }

        public static UInt32 CurrentFrame
        {
            get
            {
                return m_currFrame;
            }
        }

        /*
        public static WaveBank WaveBank
        {
            get
            {
                return m_waveBank;
            }
        }
        */

        /*
        public static SoundBank SoundBank
        {
            get
            {
                return m_soundBank;
            }
        }
        */

        /*
        public static AudioEngine AudioEngine
        {
            get
            {
                return m_audioEngine;
            }
        }
        */

        #endregion
    }
}
