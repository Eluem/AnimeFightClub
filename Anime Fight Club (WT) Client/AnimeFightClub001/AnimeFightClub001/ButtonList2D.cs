//******************************************************
// File: ButtonList2D.cs
//
// Purpose: Defines a ButtonList2D. ButtonList2D
// will be used to create 2d list of buttons with
// constant (hard coded, for now) spacing. Mainly
// coded for the customization menu.
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
    class ButtonList2D : Menu
    {
        #region Declaration
        protected Color m_buttonColor;
        protected Color m_buttonColorFocus;
        protected Color m_buttonFontColor;
        protected Color m_buttonFontColorFocus;
        protected string m_buttonBorderImage;
        protected string m_buttonBackgroundImage;
        protected string m_buttonFont;

        protected bool m_buttonBkgFullStretch;
        protected Point m_buttonOutlineOffset;

        protected Point m_buttonDimensions;
        protected int m_vertSpacing; //Spacing between buttons
        protected int m_initVertSpacing; //Spacing between first button and top of list

        protected int m_horiSpacing; //Spacing between buttons
        protected int m_initHoriSpacing; //Spacing between first button and left of list

        //Stores the number of maximum columns based on the size of the content window and the size and spacing of the buttons
        protected int m_maxCols;

        protected Point m_nextIndex; //Stores the index that the next element will be inserted at
        
        #endregion

        //****************************************************
        // Method: ButtonList2D
        //
        // Purpose: Constructor for ButtonList2D
        //****************************************************
        public ButtonList2D(Menu ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocus, Color borderColor, Color borderColorFocus, Color buttonColor, Color buttonColorFocus, Color buttonFontColor, Color buttonFontColorFocus, int buttonWidth = 60, int buttonHeight = 60, int vertSpacing = 10, int initVertSpacing = 10, int horiSpacing = 10, int initHoriSpacing = 10, string buttonBorderImage = "RoundButton", string buttonBackgroundImage = "RoundButton", string buttonFont = "Arial20", bool buttonBkgFullStretch = false)
            : base(ownerItem, game1, name, ownerItem.InitState, ownerItem.MenuState, new Rectangle(X, Y, Width, Height), backgroundColor, backgroundColorFocus, borderColor, borderColorFocus, "None", "Border001")
        {
            m_innerShadowSize[(int)BorderSide.Left] = 7;
            m_innerShadowSize[(int)BorderSide.Right] = 7;
            m_innerShadowSize[(int)BorderSide.Top] = 7;
            m_innerShadowSize[(int)BorderSide.Bottom] = 7;

            m_buttonColor = buttonColor;
            m_buttonColorFocus = buttonColorFocus;
            m_buttonFontColor = buttonFontColor;
            m_buttonFontColorFocus = buttonFontColorFocus;
            m_buttonBorderImage = buttonBorderImage;
            m_buttonBackgroundImage = buttonBackgroundImage;
            m_buttonFont = buttonFont;

            m_buttonDimensions = new Point(buttonWidth, buttonHeight);
            m_vertSpacing = vertSpacing;
            m_initVertSpacing = initVertSpacing;

            m_horiSpacing = horiSpacing;
            m_initHoriSpacing = initHoriSpacing;

            m_menuItems.Add(new List<MenuItem>());

            m_buttonBkgFullStretch = buttonBkgFullStretch;

            m_maxCols = ((ContentRect.Width - 2 * m_initHoriSpacing - buttonWidth) / (buttonWidth + horiSpacing));
            m_nextIndex = Point.Zero;
        }

        //****************************************************
        // Method: ButtonList2D
        //
        // Purpose: Constructor for ButtonList2D
        //****************************************************
        public ButtonList2D(Menu ownerItem, Game1 game1, string name, int X, int Y, int Width, int Height, Color backgroundColor, Color backgroundColorFocus, Color borderColor, Color borderColorFocus, Color buttonColor, Color buttonColorFocus, Color buttonFontColor, Color buttonFontColorFocus, Point buttonOutlineOffset, int buttonWidth = 60, int buttonHeight = 60, int vertSpacing = 10, int initVertSpacing = 10, int horiSpacing = 10, int initHoriSpacing = 10, string buttonBorderImage = "RoundButton", string buttonBackgroundImage = "RoundButton", string buttonFont = "Arial20", bool buttonBkgFullStretch = false)
            : base(ownerItem, game1, name, ownerItem.InitState, ownerItem.MenuState, new Rectangle(X, Y, Width, Height), backgroundColor, backgroundColorFocus, borderColor, borderColorFocus, "None", "Border001")
        {
            m_innerShadowSize[(int)BorderSide.Left] = 7;
            m_innerShadowSize[(int)BorderSide.Right] = 7;
            m_innerShadowSize[(int)BorderSide.Top] = 7;
            m_innerShadowSize[(int)BorderSide.Bottom] = 7;

            m_buttonColor = buttonColor;
            m_buttonColorFocus = buttonColorFocus;
            m_buttonFontColor = buttonFontColor;
            m_buttonFontColorFocus = buttonFontColorFocus;
            m_buttonBorderImage = buttonBorderImage;
            m_buttonBackgroundImage = buttonBackgroundImage;
            m_buttonFont = buttonFont;

            m_buttonDimensions = new Point(buttonWidth, buttonHeight);
            m_vertSpacing = vertSpacing;
            m_initVertSpacing = initVertSpacing;

            m_horiSpacing = horiSpacing;
            m_initHoriSpacing = initHoriSpacing;

            m_menuItems.Add(new List<MenuItem>());

            m_buttonBkgFullStretch = buttonBkgFullStretch;

            m_buttonOutlineOffset = buttonOutlineOffset;

            m_maxCols = ((ContentRect.Width - 2 * m_initHoriSpacing - buttonWidth) / (buttonWidth + horiSpacing));
            m_nextIndex = Point.Zero;
        }

        //****************************************************
        // Method: AddButton
        //
        // Purpose: Adds a new button to the verticle button
        // list
        //****************************************************
        public void AddButton(string text = "", string buttonImage = "", BasicMenuItemEventHandler clickEventHandler = null, BasicMenuItemEventHandler gotFocusEventHandler = null, BasicMenuItemEventHandler lostFocuseventHandler = null, BasicMenuItemEventHandler selectedEventHandler = null, BasicMenuItemEventHandler deselectedEventHandler = null)
        {
            Rectangle tempButtonBounds = new Rectangle();
            tempButtonBounds.X = ContentRect.Left + m_initHoriSpacing + ((m_buttonDimensions.X + m_horiSpacing) * m_nextIndex.X);
            tempButtonBounds.Y = ContentRect.Top + m_initVertSpacing + ((m_buttonDimensions.Y + m_vertSpacing) * m_nextIndex.Y);
            tempButtonBounds.Width = m_buttonDimensions.X;
            tempButtonBounds.Height = m_buttonDimensions.Y;

            Button tempButton = new Button(this, m_game1, "Button_" + (m_nextIndex.Y * m_maxCols + m_nextIndex.X), tempButtonBounds.X, tempButtonBounds.Y, tempButtonBounds.Width, tempButtonBounds.Height, m_buttonColor, m_buttonColorFocus, m_buttonFontColor, m_buttonFontColorFocus, m_buttonOutlineOffset, text, buttonImage, m_buttonFont, m_buttonBackgroundImage, m_buttonBorderImage, m_buttonBkgFullStretch);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, m_nextIndex, clickEventHandler, gotFocusEventHandler, lostFocuseventHandler, selectedEventHandler, deselectedEventHandler);

            ++m_nextIndex.X;
            if(m_nextIndex.X > m_maxCols)
            {
                m_nextIndex.X = 0;
                ++m_nextIndex.Y;
            }
        }


        //****************************************************
        // Method: ClearList
        //
        // Purpose: Clears the list of buttons
        //****************************************************
        public void ClearList()
        {
            m_menuItems.Clear();
            m_nextIndex = Point.Zero;
        }

        //****************************************************
        // Method: HandleInputHook
        //
        // Purpose: Handles any input from player one only.
        //****************************************************
        public override void HandleInputHook(Controller playerOneController)
        {
            base.HandleInputHook(playerOneController);
            //Cludge
            if (playerOneController.ControllerState.isControlPressed(Control.AbilityThree) && !playerOneController.PrevControllerState.isControlPressed(Control.AbilityThree))
            {
                Back();
            }
            if (playerOneController.ControllerState.isControlPressed(Control.Back) && !playerOneController.PrevControllerState.isControlPressed(Control.Back))
            {
                Back();
            }
            /*
            #region Gamepad Controls
            if (playerOneController.ControllerType == ControllerType.Gamepad)
            {
                if (playerOneController.ControllerState.isControlPressed(Control.AbilityThree) && !playerOneController.PrevControllerState.isControlPressed(Control.AbilityThree))
                {
                    Back();
                }
            }
            #endregion

            #region Keyboard Controls
            else
            {
                if (playerOneController.ControllerState.isControlPressed(Control.Back) && !playerOneController.PrevControllerState.isControlPressed(Control.Back))
                {
                    Back();
                }
            }
            #endregion
            */
        }

        //****************************************************
        // Method: Back
        //
        // Purpose: Called when the menu determines that the
        // player wants to go "back"
        // (This should be handled automatically because
        // I should make a different class that this
        // inherits from such as (subMenu)
        //****************************************************
        public override void Back()
        {
            LoseFocus(this);
            DeselectItem(m_currSelection.X, m_currSelection.Y);
        }

        #region Properties

        public int ButtonCount
        {
            get
            {
                int count = 0;
                foreach (List<MenuItem> buttonColumn in m_menuItems)
                {
                    foreach (MenuItem button in buttonColumn)
                    {
                        ++count;
                    }
                }
                return count;
            }
        }

        public Point ButtonOutlineOffset
        {
            get
            {
                return m_buttonOutlineOffset;
            }
            set
            {
                m_buttonOutlineOffset = value;
            }
        }

        public bool ButtonBkgFullStretch
        {
            get
            {
                return m_buttonBkgFullStretch;
            }
            set
            {
                m_buttonBkgFullStretch = value;
            }
        }

        public Color ButtonColor
        {
            get
            {
                return m_buttonColor;
            }
            set
            {
                m_buttonColor = value;
            }
        }

        public Color ButtonColorFocus
        {
            get
            {
                return m_buttonColorFocus;
            }
            set
            {
                m_buttonColorFocus = value;
            }
        }

        public Color ButtonFontColor
        {
            get
            {
                return m_buttonFontColor;
            }
            set
            {
                m_buttonFontColor = value;
            }
        }

        public Color ButtonFontColorFocus
        {
            get
            {
                return m_buttonFontColorFocus;
            }
            set
            {
                m_buttonFontColorFocus = value;
            }
        }

        public string ButtonBorderImage
        {
            get
            {
                return m_buttonBorderImage;
            }
            set
            {
                m_buttonBorderImage = value;
            }
        }

        public string ButtonBackgroundImage
        {
            get
            {
                return m_buttonBackgroundImage;
            }
            set
            {
                m_buttonBackgroundImage = value;
            }
        }

        public string ButtonFont
        {
            get
            {
                return m_buttonFont;
            }
            set
            {
                m_buttonFont = value;
            }
        }

        public int ButtonWidth
        {
            get
            {
                return m_buttonDimensions.X;
            }
            set
            {
                m_buttonDimensions.X = value;
            }
        }

        public int ButtonHeight
        {
            get
            {
                return m_buttonDimensions.Y;
            }
            set
            {
                m_buttonDimensions.Y = value;
            }
        }

        public int VertSpacing
        {
            get
            {
                return m_vertSpacing;
            }
            set
            {
                m_vertSpacing = value;
            }
        }

        public int InitVertSpacing
        {
            get
            {
                return m_initVertSpacing;
            }
            set
            {
                m_initVertSpacing = value;
            }
        }

        #endregion
    }
}
