//******************************************************
// File: Settings.cs
//
// Purpose: Contains all the general settings.
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
    class Settings
    {
        #region Declarations
        protected float m_gravityAccel; //Stores acceleration due to gravity
        protected int m_fps; // Stores the frames per second
        protected int m_startingHealth; //Stores the starting health for all players
        protected int m_startingMana;  //Stores the starting mana for all players
        protected int m_startingStamina;  //Stores the starting stamina for all players
        protected int m_startingAttack; //Stores the starting attack for all players
        protected int m_startingDefense; //Stores the starting defense for all players
        protected int m_startingLives; //Stores the starting lives for all players
        protected int m_startingRespawnTimer; //Time that the respawn timer for a player gets reset to
        protected Boolean m_gore; //Flag for whether or not gore is enabled

        protected string m_masterServerIP; //Stores the ip address (or domain name?) of the master server

        protected Dictionary<PlayerIndex, Dictionary<Control, Keys>> m_keyBoardBindings;
        protected Dictionary<PlayerIndex, Dictionary<Control, Buttons>> m_gamePadBindings;
        protected Dictionary<PlayerIndex, ControllerType> m_selControllerType;
        #endregion

        #region Server Settings
        protected int m_posUpdateSendFrame = 10;
        protected int m_velUpdateSendFrame = 5;
        protected int m_accelUpdateSendFrame = 5;

        protected string m_serverName = "Default";
        protected int m_serverPort = 37337;

        protected int m_killLimit = 20;
        #endregion


        //****************************************************
        // Method: Settings
        //
        // Purpose: Settings constructor
        //****************************************************
        public Settings()
        {
            //gravityAccel = 9.82F;
            m_gravityAccel = 900F;
            m_fps = 60;
            m_startingHealth = 100;
            m_startingMana = 100;
            m_startingStamina = 100;
            m_startingAttack = 20;
            m_startingDefense = 20;
            m_startingLives = 5;
            m_startingRespawnTimer = 1300;
            m_gore = true;

            //Initialize Controls
            m_keyBoardBindings = new Dictionary<PlayerIndex, Dictionary<Control, Keys>>();
            m_gamePadBindings = new Dictionary<PlayerIndex, Dictionary<Control, Buttons>>();
            m_selControllerType = new Dictionary<PlayerIndex, ControllerType>();

            m_gamePadBindings.Add(PlayerIndex.One, new Dictionary<Control, Buttons>());
            m_keyBoardBindings.Add(PlayerIndex.One, new Dictionary<Control, Keys>());

            //SetCurrControlsToHardCoded();
            ReadFromConfig();

        }

        //****************************************************
        // Method: Default
        //
        // Purpose: Resets all the settings
        //****************************************************
        public void Default()
        {
            m_gravityAccel = 50F;
            m_fps = 60;
            m_startingHealth = 100;
            m_startingMana = 0;
            m_startingStamina = 100;
            m_startingLives = 5;
            m_startingRespawnTimer = 3000;
            m_gore = true;
        }


        //****************************************************
        // Method: ReadFromConfig
        //
        // Purpose: Reads all the settings from a config.txt
        // file
        //****************************************************
        public void ReadFromConfig()
        {
            string[] lines = System.IO.File.ReadAllLines("config.txt");
            string[] splitLine;
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            for (int i = 0; i < lines.Length; ++i)
            {
                splitLine = lines[i].Split('=');
                configDict.Add(splitLine[0].Trim(), splitLine[1].Trim());
            }
            foreach (KeyValuePair<string, string> pair in configDict)
            {
                if (pair.Key.StartsWith("Player") && pair.Key.EndsWith("Control Type"))
                {
                    m_selControllerType.Add((PlayerIndex)Enum.Parse(typeof(PlayerIndex), pair.Key.Split(' ')[1]), (ControllerType)Enum.Parse(typeof(ControllerType), pair.Value)); 
                }
                else if (pair.Key.StartsWith("Player") && pair.Key.Split(' ')[2] == "Keyboard")
                {
                    m_keyBoardBindings[(PlayerIndex)Enum.Parse(typeof(PlayerIndex), pair.Key.Split(' ')[1])][(Control)Enum.Parse(typeof(Control), pair.Key.Split(' ')[pair.Key.Split(' ').Length - 1])] = (Keys)Enum.Parse(typeof(Keys), pair.Value);
                }
                else if (pair.Key.StartsWith("Player") && pair.Key.Split(' ')[2] == "Gamepad")
                {
                    m_gamePadBindings[(PlayerIndex)Enum.Parse(typeof(PlayerIndex), pair.Key.Split(' ')[1])][(Control)Enum.Parse(typeof(Control), pair.Key.Split(' ')[pair.Key.Split(' ').Length - 1])] = (Buttons)Enum.Parse(typeof(Buttons), pair.Value);
                }
                else if (pair.Key == "Position Send Time")
                {
                    m_posUpdateSendFrame = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == "Velocity Send Time")
                {
                    m_velUpdateSendFrame = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == "Acceleration Send Time")
                {
                    m_accelUpdateSendFrame = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == "Master Server IP")
                {
                    m_masterServerIP = pair.Value;
                }
                else if (pair.Key == "Server Name")
                {
                    m_serverName = pair.Value;
                }
                else if (pair.Key == "Server Port")
                {
                    m_serverPort = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == "Kill Limit")
                {
                    m_killLimit = Convert.ToInt32(pair.Value);
                }
            }
        }


        //****************************************************
        // Method: SetCurrControlsToHardCoded
        //
        // Purpose: Sets the current controls to a hardcoded
        // control scheme (mostly for debugging)
        //****************************************************
        public void SetCurrControlsToHardCoded()
        {
            //Player One Game Pad
            m_gamePadBindings.Add(PlayerIndex.One, new Dictionary<Control,Buttons>());
            m_gamePadBindings[PlayerIndex.One][Control.OffHand] = Buttons.LeftTrigger;
            m_gamePadBindings[PlayerIndex.One][Control.MainHand] = Buttons.RightTrigger;
            m_gamePadBindings[PlayerIndex.One][Control.UseItem] = Buttons.LeftShoulder;
            m_gamePadBindings[PlayerIndex.One][Control.GrabThrowItem] = Buttons.RightShoulder;
            m_gamePadBindings[PlayerIndex.One][Control.Jump] = Buttons.A;
            m_gamePadBindings[PlayerIndex.One][Control.AbilityOne] = Buttons.X;
            m_gamePadBindings[PlayerIndex.One][Control.AbilityTwo] = Buttons.B;
            m_gamePadBindings[PlayerIndex.One][Control.AbilityThree] = Buttons.Y;
            m_gamePadBindings[PlayerIndex.One][Control.MoveLeft] = Buttons.LeftThumbstickLeft;
            m_gamePadBindings[PlayerIndex.One][Control.MoveRight] = Buttons.LeftThumbstickRight;
            m_gamePadBindings[PlayerIndex.One][Control.Crouch] = Buttons.LeftThumbstickDown;
            m_gamePadBindings[PlayerIndex.One][Control.Run] = Buttons.LeftStick;
            m_gamePadBindings[PlayerIndex.One][Control.AimLeft] = Buttons.RightThumbstickLeft;
            m_gamePadBindings[PlayerIndex.One][Control.AimRight] = Buttons.RightThumbstickRight;
            m_gamePadBindings[PlayerIndex.One][Control.AimUp] = Buttons.RightThumbstickUp;
            m_gamePadBindings[PlayerIndex.One][Control.AimDown] = Buttons.RightThumbstickDown;
            m_gamePadBindings[PlayerIndex.One][Control.Start] = Buttons.Start;
            m_gamePadBindings[PlayerIndex.One][Control.Back] = Buttons.Back;

            m_keyBoardBindings.Add(PlayerIndex.One, new Dictionary<Control, Keys>());
            m_keyBoardBindings[PlayerIndex.One][Control.OffHand] = Keys.RightControl;
            m_keyBoardBindings[PlayerIndex.One][Control.MainHand] = Keys.RightShift;
            m_keyBoardBindings[PlayerIndex.One][Control.UseItem] = Keys.NumPad0;
            m_keyBoardBindings[PlayerIndex.One][Control.GrabThrowItem] = Keys.PageDown;
            m_keyBoardBindings[PlayerIndex.One][Control.Jump] = Keys.W;
            m_keyBoardBindings[PlayerIndex.One][Control.AbilityOne] = Keys.NumPad1;
            m_keyBoardBindings[PlayerIndex.One][Control.AbilityTwo] = Keys.NumPad2;
            m_keyBoardBindings[PlayerIndex.One][Control.AbilityThree] = Keys.NumPad3;
            m_keyBoardBindings[PlayerIndex.One][Control.MoveLeft] = Keys.A;
            m_keyBoardBindings[PlayerIndex.One][Control.MoveRight] = Keys.D;
            m_keyBoardBindings[PlayerIndex.One][Control.Crouch] = Keys.S;
            m_keyBoardBindings[PlayerIndex.One][Control.Run] = Keys.LeftShift;
            m_keyBoardBindings[PlayerIndex.One][Control.AimLeft] = Keys.Left;
            m_keyBoardBindings[PlayerIndex.One][Control.AimRight] = Keys.Right;
            m_keyBoardBindings[PlayerIndex.One][Control.AimUp] = Keys.Up;
            m_keyBoardBindings[PlayerIndex.One][Control.AimDown] = Keys.Down;
            m_keyBoardBindings[PlayerIndex.One][Control.Start] = Keys.Enter;
            m_keyBoardBindings[PlayerIndex.One][Control.Back] = Keys.Escape;
        }


        //Properties
        #region Properties
        public float Gravity
        {
            get
            {
                return m_gravityAccel;
            }
            set
            {
                m_gravityAccel = value;
            }
        }

        public int fps
        {
            get
            {
                return m_fps;
            }
            set
            {
                m_fps = value;
            }
        }

        public int startingHealth
        {
            get
            {
                return m_startingHealth;
            }
            set
            {
                m_startingHealth = value;
            }
        }

        public int startingMana
        {
            get
            {
                return m_startingMana;
            }
            set
            {
                m_startingMana = value;
            }
        }

        public int startingStamina
        {
            get
            {
                return m_startingStamina;
            }
            set
            {
                m_startingStamina = value;
            }
        }

        public int startingAttack
        {
            get
            {
                return m_startingAttack;
            }
            set
            {
                m_startingAttack = value;
            }
        }

        public int startingDefense
        {
            get
            {
                return m_startingDefense;
            }
            set
            {
                m_startingDefense = value;
            }
        }

        public int startingLives
        {
            get
            {
                return m_startingLives;
            }
            set
            {
                m_startingLives = value;
            }
        }

        public int startingRespawnTimer
        {
            get
            {
                return m_startingRespawnTimer;
            }
            set
            {
                m_startingRespawnTimer = value;
            }
        }

        public Boolean gore
        {
            get
            {
                return m_gore;
            }
            set
            {
                m_gore = value;
            }
        }

        public Dictionary<PlayerIndex, Dictionary<Control, Keys>> KeyboardBindings
        {
            get
            {
                return m_keyBoardBindings;
            }
        }

        public Dictionary<PlayerIndex, Dictionary<Control, Buttons>> GamePadBindings
        {
            get
            {
                return m_gamePadBindings;
            }
        }

        public Dictionary<PlayerIndex, ControllerType> SelControllerType
        {
            get
            {
                return m_selControllerType;
            }
        }

        public string MasterServerIP
        {
            get
            {
                return m_masterServerIP;
            }
        }

        public int PosUpdateSendFrame
        {
            get
            {
                return m_posUpdateSendFrame;
            }
        }

        public int VelUpdateSendFrame
        {
            get
            {
                return m_velUpdateSendFrame;
            }
        }

        public int AccelUpdateSendFrame
        {
            get
            {
                return m_accelUpdateSendFrame;
            }
        }

        public string ServerName
        {
            get
            {
                return m_serverName;
            }
            set
            {
                m_serverName = value;
            }
        }

        public int ServerPort
        {
            get
            {
                return m_serverPort;
            }
            set
            {
                m_serverPort = value;
            }
        }

        public int KillLimit
        {
            get
            {
                return m_killLimit;
            }
            set
            {
                m_killLimit = value;
            }
        }
        #endregion

    }
}
