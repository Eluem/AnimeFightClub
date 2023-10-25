//******************************************************
// File: Menu.cs
//
// Purpose: Defines a basic Menu object which is a
// MenuItem. It will function very much like a
// "Form" in winForms.
// Menus will be designed to contain a list of
// MenuItems which will all be drawn on to the menu.
// The menu will be enabled to handle controls from
// player and, depending on the way the Menu is coded
// it should be able to transfer focus from one item
// to another. It will also be able to cause events
// such as the Click event to fire, and will be able to
// assign different functions to those events.
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
    class Menu : MenuItem
    {
        #region Declarations
        //List of MenuItems which are on this menu. It is 2D so that Menu items can have 2D indexes
        protected List<List<MenuItem>> m_menuItems;

        protected List<MessageBox> m_messageBoxList; //List of message boxes which are automatically drawn on top of all content
        protected List<MessageBox> m_messageBoxDelList; //List of message boxes which need to be deleted
        protected List<MessageBox> m_messageBoxAddList; //List of message boxes which need to be added

        protected Point m_currSelection;
        protected Point m_prevSelection;
        protected GameState m_initState; //Gamestate which corresponds to the initialization of this menu
        protected GameState m_menuState; //Gamestate which corresponds to the running of this menu
        #endregion

        //****************************************************
        // Method: Menu
        //
        // Purpose: Default constructor for Menu
        //****************************************************
        public Menu(Menu ownerItem, Game1 game1, string name, GameState initState, GameState menuState, Rectangle drawRect, Color backgroundColor, Color backgroundColorFocus, Color borderColor, Color borderColorFocus, string backgroundImage = "Background001", string borderImage = "Background001", bool startingFocus = false, bool bkgFullStretch = true)
            : base(ownerItem, game1, name, drawRect, backgroundImage, backgroundColor, backgroundColorFocus, borderImage, borderColor, borderColorFocus, startingFocus, bkgFullStretch)
        {
            m_initState = initState;
            m_menuState = menuState;

            m_menuItems = new List<List<MenuItem>>();
            m_messageBoxList = new List<MessageBox>();
            m_messageBoxAddList = new List<MessageBox>();
            m_messageBoxDelList = new List<MessageBox>();

            m_innerShadowSize[(int)BorderSide.Left] = 19;
            m_innerShadowSize[(int)BorderSide.Right] = 19;
            m_innerShadowSize[(int)BorderSide.Top] = 19;
            m_innerShadowSize[(int)BorderSide.Bottom] = 19;

            m_currSelection = Point.Zero;
            m_prevSelection = Point.Zero;
        }


        //****************************************************
        // Method: FindItem
        //
        // Purpose: Searches the MenuItem matrix by name
        //****************************************************
        public MenuItem FindItem(string name)
        {
            foreach (List<MenuItem> menuItemList in m_menuItems)
            {
                foreach (MenuItem menuItem in menuItemList)
                {
                    if (menuItem.Name == name)
                        return menuItem;
                }
            }
            return null;
        }

        //****************************************************
        // Method: FindMessagebox
        //
        // Purpose: Searches the m_messageBoxList for a
        // particular messagebox
        //****************************************************
        public MessageBox FindMessagebox(string name)
        {

            foreach (MessageBox messageBox in m_messageBoxList)
            {
                if (messageBox.Name == name)
                    return messageBox;
            }
            return null;
        }

        //****************************************************
        // Method: Up
        //
        // Purpose: Called when the player hits up
        //****************************************************
        public virtual void Up()
        {
            if (m_menuItems.Count > 0 && m_menuItems[0].Count > 0)
            {
                m_prevSelection = m_currSelection;
                DeselectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item
                --m_currSelection.Y;
                //If we entered garbage, go back and tehn keep looping back until we find the last contiguous item
                if (m_currSelection.Y < 0 || m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                {
                    ++m_currSelection.Y;
                    //Loops back to last contiguous item in this row
                    int i = m_currSelection.Y;
                    while (true)
                    {
                        if (i >= m_menuItems[m_currSelection.X].Count || m_menuItems[m_currSelection.X][i].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][i].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                        {
                            --i;
                            break;
                        }

                        ++i;
                    }

                    m_currSelection.Y = i;
                }
                SelectItem(m_currSelection.X, m_currSelection.Y); //Selects the current item
            }
        }

        //****************************************************
        // Method: Down
        //
        // Purpose: Called when the player hits down
        //****************************************************
        public virtual void Down()
        {
            if (m_menuItems.Count > 0 && m_menuItems[0].Count > 0)
            {
                m_prevSelection = m_currSelection;
                DeselectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item
                ++m_currSelection.Y;

                //If we entered garbage, go back and tehn keep looping back until we find the last contiguous item
                if (m_currSelection.Y >= m_menuItems[m_currSelection.X].Count || m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                {
                    --m_currSelection.Y;
                    //Loops back to last contiguous item in this row
                    int i = m_currSelection.Y;
                    while (true)
                    {
                        if (i < 0 || m_menuItems[m_currSelection.X][i].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][i].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                        {
                            ++i;
                            break;
                        }

                        --i;
                    }

                    m_currSelection.Y = i;
                }


                SelectItem(m_currSelection.X, m_currSelection.Y); //Selects the current item
            }
        }

        //****************************************************
        // Method: Left
        //
        // Purpose: Called when the player hits left
        //****************************************************
        public virtual void Left()
        {
            if (m_menuItems.Count > 0 && m_menuItems[0].Count > 0)
            {
                m_prevSelection = m_currSelection;
                DeselectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item
                --m_currSelection.X;

                //If we entered garbage, go back and tehn keep looping back until we find the last contiguous item
                if (m_currSelection.X < 0 || m_currSelection.Y >= m_menuItems[m_currSelection.X].Count || m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                {
                    ++m_currSelection.X;
                    //Loops back to last contiguous item in this row
                    int i = m_currSelection.X;
                    while (true)
                    {
                        if (i >= m_menuItems.Count || m_currSelection.Y >= m_menuItems[i].Count || m_menuItems[i][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[i][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                        {
                            --i;
                            break;
                        }

                        ++i;
                    }

                    m_currSelection.X = i;
                }

                SelectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item //Selects the current item
            }
        }

        //****************************************************
        // Method: Right
        //
        // Purpose: Called when the player hits right
        //****************************************************
        public virtual void Right()
        {
            if (m_menuItems.Count > 0 && m_menuItems[0].Count > 0)
            {
                m_prevSelection = m_currSelection;
                DeselectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item
                ++m_currSelection.X;

                //If we entered garbage, go back and tehn keep looping back until we find the last contiguous item
                if (m_currSelection.X >= m_menuItems.Count || m_currSelection.Y >= m_menuItems[m_currSelection.X].Count || m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[m_currSelection.X][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                {
                    --m_currSelection.X;
                    //Loops back to last contiguous item in this row
                    int i = m_currSelection.X;
                    while (true)
                    {
                        if (i < 0 || m_currSelection.Y >= m_menuItems[i].Count || m_menuItems[i][m_currSelection.Y].GetType().ToString().Split('.')[m_menuItems[i][m_currSelection.Y].GetType().ToString().Split('.').Length - 1] == "PlaceHolderMenuItem")
                        {
                            ++i;
                            break;
                        }

                        --i;
                    }

                    m_currSelection.X = i;
                }


                SelectItem(m_currSelection.X, m_currSelection.Y); //Deselects the previous item //Selects the current item
            }
        }

        //****************************************************
        // Method: Back
        //
        // Purpose: Called when the menu determines that the
        // player wants to go "back"
        //****************************************************
        public virtual void Back()
        {
            LoseFocus(this);

            //Goes back to the previous menus state. Menus that are designed to be submenus should set their states to
            //their owner's states
            m_game1.currGameState = ((Menu)m_ownerItem).InitState;
        }

        //****************************************************
        // Method: ClickItem
        //
        // Purpose: Called when the menu determines that the
        // player tried to "click" a menu item
        //****************************************************
        public virtual void ClickItem()
        {
            if (m_currSelection.X >= 0 && m_currSelection.X < m_menuItems.Count && m_currSelection.Y >= 0 && m_currSelection.Y < MenuItems[m_currSelection.X].Count)
                m_menuItems[m_currSelection.X][m_currSelection.Y].Click(this);
        }

        //****************************************************
        // Method: SelectItem
        //
        // Purpose: Tells an item it has been selected
        // or deselected. Used to prevent validation on
        // X and Y from being scattered through out the menu.
        //****************************************************
        /// <summary>
        /// Validates that X and Y are within the correct bounds and selects the item.
        /// </summary>
        /// <param name="X">X coordinate of the item</param>
        /// <param name="Y">Y coordinate of the item</param>
        public virtual void SelectItem(int X, int Y)
        {
            if (X >= 0 && X < m_menuItems.Count && Y >= 0 && Y < MenuItems[X].Count)
            {
                m_menuItems[X][Y].Select(this);
            }
        }

        //****************************************************
        // Method: DeselectItem
        //
        // Purpose: Tells an item it has been selected
        // or deselected. Used to prevent validation on
        // X and Y from being scattered through out the menu.
        //****************************************************
        /// <summary>
        /// Validates that X and Y are within the correct bounds and deselects the item.
        /// </summary>
        /// <param name="X">X coordinate of the item</param>
        /// <param name="Y">Y coordinate of the item</param>
        public virtual void DeselectItem(int X, int Y)
        {
            if (X >= 0 && X < m_menuItems.Count && Y >= 0 && Y < MenuItems[X].Count)
            {
                m_menuItems[X][Y].Deselect(this);
            }
        }

        //****************************************************
        // Method: HandleInput
        //
        // Purpose: Modifies the HandleInput function so that
        // the sub item's HandleInput functions can be called
        // even when the menu itself doesn't have direct
        // focus. (so it can give focus to sub items)
        //****************************************************
        public override void HandleInput(Controller playerOneController)
        {
            #region Handle MessageBox input
            foreach (MessageBox messageBox in m_messageBoxList)
            {
                messageBox.HandleInput(playerOneController);
            }
            #endregion

            #region Make all sub-MenuItems handle input as well
            foreach (List<MenuItem> menuItemList in m_menuItems)
            {
                foreach (MenuItem menuItem in menuItemList)
                {
                    menuItem.HandleInput(playerOneController);
                }
            }
            #endregion

            base.HandleInput(playerOneController);
        }

        //****************************************************
        // Method: HandleInputHook
        //
        // Purpose: Handles any input from player one only.
        //****************************************************
        public override void HandleInputHook(Controller playerOneController)
        {
            #region General Controls
            if (playerOneController.ControllerState.isControlPressed(Control.AimDown) && !playerOneController.PrevControllerState.isControlPressed(Control.AimDown))
            {
                Down();
            }
            if (playerOneController.ControllerState.isControlPressed(Control.AimUp) && !playerOneController.PrevControllerState.isControlPressed(Control.AimUp))
            {
                Up();
            }
            if (playerOneController.ControllerState.isControlPressed(Control.AimLeft) && !playerOneController.PrevControllerState.isControlPressed(Control.AimLeft))
            {
                Left();
            }
            if (playerOneController.ControllerState.isControlPressed(Control.AimRight) && !playerOneController.PrevControllerState.isControlPressed(Control.AimRight))
            {
                Right();
            }
            #endregion

            //CLUDGE
            if (playerOneController.ControllerState.isControlPressed(Control.Jump) && !playerOneController.PrevControllerState.isControlPressed(Control.Jump))
            {
                ClickItem();
            }
            if (playerOneController.ControllerState.isControlPressed(Control.Start) && !playerOneController.PrevControllerState.isControlPressed(Control.Start))
            {
                ClickItem();
            }

            /*
            #region Gamepad controls
            if (playerOneController.ControllerType == ControllerType.Gamepad)
            {
                if (playerOneController.ControllerState.isControlPressed(Control.Jump) && !playerOneController.PrevControllerState.isControlPressed(Control.Jump))
                {
                    ClickItem();
                }
            }
            #endregion

            #region Keyboard Controls
            else
            {
                if (playerOneController.ControllerState.isControlPressed(Control.Start) && !playerOneController.PrevControllerState.isControlPressed(Control.Start))
                {
                    ClickItem();
                }
            }
            #endregion
            */
        }

        //****************************************************
        // Method: AddItem
        //
        // Purpose: Adds an item to the MenuItem matrix
        // At the position entered
        // (Automatically determines if a new column must be
        // added)
        //****************************************************
        /// <summary>
        /// Adds an item to the MenuItem Matrix. (Note: Automatically determines if a new column must be added)
        /// </summary>
        /// <param name="menuItem">The item you want to add.</param>
        /// <param name="index">The index of the item you want to add. (Must be a continguous index value.)</param>
        /// <param name="clickEventHandler">The function you want to run when the click event is fired</param>
        public virtual MenuItem AddItem(MenuItem menuItem, Point index, BasicMenuItemEventHandler clickEventHandler = null, BasicMenuItemEventHandler gotFocusEventHandler = null, BasicMenuItemEventHandler lostFocusEventHandler = null, BasicMenuItemEventHandler selectedEventHandler = null, BasicMenuItemEventHandler deselectedEventHandler = null)
        {
            if (index.X >= 0 && index.Y >= 0 && ((index.X < m_menuItems.Count && index.Y == m_menuItems[index.X].Count) || (index.X == m_menuItems.Count && index.Y == 0)))
            {
                //Adds a new column
                if (index.X == m_menuItems.Count)
                    m_menuItems.Add(new List<MenuItem>());

                //Binds the click event
                menuItem.Clicked += clickEventHandler;
                menuItem.GotFocus += gotFocusEventHandler;
                menuItem.LostFocus += lostFocusEventHandler;
                menuItem.Selected += selectedEventHandler;
                menuItem.Deselected += deselectedEventHandler;

                //Adds the item
                m_menuItems[index.X].Add(menuItem);
            }
            else
            {
                //Recursive place holder filling cludge
                if (index.Y > 0)
                {
                    AddItem(new PlaceHolderMenuItem(this, m_game1), new Point(index.X, index.Y - 1));
                }
                else
                {
                    AddItem(new PlaceHolderMenuItem(this, m_game1), new Point(index.X - 1, index.Y));
                }

                AddItem(menuItem, index, clickEventHandler, gotFocusEventHandler, lostFocusEventHandler, selectedEventHandler, deselectedEventHandler);

                //throw new System.ArgumentException("The index entered must be contiguous.", "index");
            }
            return menuItem;
        }


        //****************************************************
        // Method: PopUpMessage
        //
        // Purpose: Creates a popup message and gives it
        // focus.
        //****************************************************
        /// <summary>
        /// Creates a popup message and gives it focus.
        /// </summary>
        /// <param name="spawner">The menu/submenu that will gain focus when this message box closes.</param>
        /// <param name="message">The message that will be displayed.</param>
        /// <param name="type">The type of message box (ok,cancel,okay/cance,yes/no)</param>
        /// <param name="firstButtonClick">The function you want to run when the click event for the first button (okay or yes or cancel) button is fired</param>
        /// <param name="secondButtonClick">The function you want to run when the click event for the second button (cancel or no) button is fired</param>
        public virtual void PopUpMessage(Menu spawner, string name, string message, messageBoxType type, int X, int Y, BasicMenuItemEventHandler firstButtonClick = null, BasicMenuItemEventHandler secondButtonClick = null)
        {
            m_messageBoxAddList.Add(new MessageBox(spawner, m_game1, this, name, X, Y, message, type, firstButtonClick, secondButtonClick));
            ClearFocus();
            GiveFocus(m_messageBoxAddList[m_messageBoxAddList.Count - 1]);
            m_messageBoxAddList[m_messageBoxAddList.Count - 1].Initialize();
        }

        //****************************************************
        // Method: PopUpMessage
        //
        // Purpose: Creates a popup message and gives it
        // focus. Automatically centers.
        //****************************************************
        /// <summary>
        /// Creates a popup message and gives it focus.
        /// </summary>
        /// <param name="spawner">The menu/submenu that will gain focus when this message box closes.</param>
        /// <param name="message">The message that will be displayed.</param>
        /// <param name="type">The type of message box (ok,cancel,okay/cance,yes/no)</param>
        /// <param name="firstButtonClick">The function you want to run when the click event for the first button (okay or yes or cancel) button is fired</param>
        /// <param name="secondButtonClick">The function you want to run when the click event for the second button (cancel or no) button is fired</param>
        public virtual void PopUpMessage(Menu spawner, string name, string message, messageBoxType type, BasicMenuItemEventHandler firstButtonClick = null, BasicMenuItemEventHandler secondButtonClick = null)
        {
            m_messageBoxAddList.Add(new MessageBox(spawner, m_game1, this, name, message, type, firstButtonClick, secondButtonClick));
            ClearFocus();
            GiveFocus(m_messageBoxAddList[m_messageBoxAddList.Count - 1]);
            m_messageBoxAddList[m_messageBoxAddList.Count - 1].Initialize();
        }

        //****************************************************
        // Method: ClearFocus
        //
        // Purpose: Clears the focus of all menu items
        //****************************************************
        public virtual void ClearFocus()
        {
            foreach (List<MenuItem> menuItemList in m_menuItems)
            {
                foreach (MenuItem menuItem in menuItemList)
                {
                    menuItem.KillFocus(this);
                }
            }
            foreach (MessageBox messageBox in m_messageBoxList)
            {
                messageBox.KillFocus(this);
            }
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // MenuItem
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            #region Add Message Boxes
            foreach (MessageBox messageBox in m_messageBoxAddList)
            {
                m_messageBoxList.Add(messageBox);
            }
            m_messageBoxAddList.Clear();

            #endregion
            #region Delete Message Boxes
            foreach (MessageBox messageBox in m_messageBoxDelList)
            {
                m_messageBoxList.Remove(messageBox);
            }
            m_messageBoxDelList.Clear();
            #endregion
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the menu items in this menu
        // as well as the Title of the menu and any specifc
        // content this menu may need to draw
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            foreach (List<MenuItem> col in m_menuItems)
            {
                foreach (MenuItem menuItem in col)
                {
                    menuItem.Draw(spriteBatch, graphicsDevice, offset);
                }
            }
            //base.DrawContent(spriteBatch, graphicsDevice, offset);
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Menu overrides it's own draw so that it
        // can draw it's title onto it's border
        //****************************************************
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            base.Draw(spriteBatch, graphicsDevice, offset);
            base.DrawContent(spriteBatch, graphicsDevice, offset);

            #region Draw MessageBoxes
            foreach (MessageBox messageBox in m_messageBoxList)
            {
                messageBox.Draw(spriteBatch, graphicsDevice, offset);
            }
            #endregion
        }

        //****************************************************
        // Method: Initialize
        //
        // Purpose: Initializes the menu
        //****************************************************
        public virtual void Initialize()
        {
            m_messageBoxList.Clear();
            DeselectItem(m_currSelection.X, m_currSelection.Y);
            m_currSelection = Point.Zero;
            SelectItem(m_currSelection.X, m_currSelection.Y);
            GainFocus(this);
        }

        #region Properties
        public MenuItem CurrSelectedItem
        {
            get
            {
                return m_menuItems[m_currSelection.X][m_currSelection.Y];
            }
        }

        public Point CurrSelection
        {
            get
            {
                return m_currSelection;
            }
            set
            {
                m_currSelection = value;
            }
        }

        public Point PrevSelection
        {
            get
            {
                return m_prevSelection;
            }
            set
            {
                m_prevSelection = value;
            }
        }

        public List<List<MenuItem>> MenuItems
        {
            get
            {
                return m_menuItems;
            }
        }

        public string Title
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
                //m_textPos = new Vector2(((m_rect.Width - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Left]].Width - GlobalVariables.ImageDict[m_borderImage[(int)BorderSide.Right]].Width) / 2) - (GlobalVariables.FontDict[m_font].MeasureString(m_text).X / 2), -20);
                m_textPos = new Vector2(m_rect.Center.X - GlobalVariables.FontDict[m_font].MeasureString(m_text).X / 2, 45);
            }
        }

        public Vector2 TitlePos
        {
            get
            {
                return m_textPos;
            }

            set
            {
                m_textPos = value;
            }
        }

        public GameState InitState
        {
            get
            {
                return m_initState;
            }
        }

        public GameState MenuState
        {
            get
            {
                return m_menuState;
            }
        }

        public GameState PrevInitState
        {
            get
            {
                return ((Menu)m_ownerItem).InitState;
            }
        }

        public GameState PrevMenuState
        {
            get
            {
                return ((Menu)m_ownerItem).MenuState;
            }
        }

        public List<MessageBox> MessageBoxList
        {
            get
            {
                return m_messageBoxList;
            }

            set
            {
                m_messageBoxList = value;
            }
        }

        public List<MessageBox> MessageBoxAddList
        {
            get
            {
                return m_messageBoxAddList;
            }

            set
            {
                m_messageBoxAddList = value;
            }
        }

        public List<MessageBox> MessageBoxDelList
        {
            get
            {
                return m_messageBoxDelList;
            }

            set
            {
                m_messageBoxDelList = value;
            }
        }
        #endregion
    }
}
