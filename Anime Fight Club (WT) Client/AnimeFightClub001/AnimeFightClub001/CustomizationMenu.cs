//******************************************************
// File: CustomizationMenu.cs
//
// Purpose: Defines the customization menu which will
// allow players to choose their loadout, avatar, and
// buy new abilities/weapons.
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
    class CustomizationMenu : Menu
    {
        #region Declarations
        ButtonList2D m_buttonList2d; //Stores a 2d matrix of all the weapons/abilities the player can choose
        TextBox m_descBox; //Stores a textbox used to display the description of an ability
        #endregion

        //****************************************************
        // Method: CustomizationMenu
        //
        // Purpose: Constructor for CustomizationMenu
        //****************************************************
        public CustomizationMenu(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "CustomizationMenu", GameState.InitializeCustomizationMenu, GameState.CustomizationMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            Title = "Customize";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;

            m_menuItems.Add(new List<MenuItem>());

            #region Main Hand Button
            Button tempButton = new Button(this, m_game1, "MainHandButton", 50, 70, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(0, 0), ClickAbility, null, null, SelectAbility);
            #endregion

            #region Off Hand Button
            tempButton = new Button(this, m_game1, "OffHandButton", 50, 155, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(0, 1), ClickAbility, null, null, SelectAbility);
            #endregion

            #region Passive Hand Button
            tempButton = new Button(this, m_game1, "PassiveButton", 50, 240, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(0, 2), ClickAbility, null, null, SelectAbility);
            #endregion

            #region Special 1 Button
            tempButton = new Button(this, m_game1, "Special1Button", 120, 70, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(1, 0), ClickAbility, null, null, SelectAbility);
            #endregion

            #region Special 2 Button
            tempButton = new Button(this, m_game1, "Special2Button", 120, 155, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(1, 1), ClickAbility, null, null, SelectAbility);
            #endregion

            #region Special 3 Button
            tempButton = new Button(this, m_game1, "Special3Button", 120, 240, 60, 60, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, Point.Zero, "", "", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(1, 2), ClickAbility, null, null, SelectAbility);
            #endregion


            #region Avatar Button
            tempButton = new Button(this, m_game1, "AvatarButton", 200, 70, 170, 246, Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, Point.Zero, "", "None", "Arial20", "None", "IconFrame001", true);
            tempButton.InnerShadowSize[(int)BorderSide.Left] = 4;
            tempButton.InnerShadowSize[(int)BorderSide.Right] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Top] = 11;
            tempButton.InnerShadowSize[(int)BorderSide.Bottom] = 4;

            AddItem(tempButton, new Point(2, 0), CycleAvatar);
            m_menuItems[2].Add(tempButton); //2,1
            m_menuItems[2].Add(tempButton); //2,2
            #endregion


            tempButton = (Button)AddItem(new Button(this, m_game1, "BackButton", 41, 316, 150, 65, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Back"), new Point(0, 3), GoBack);
            m_menuItems[1].Add(tempButton); //1,3


            m_descBox = new TextBox(this, m_game1, "DescBox", 200, 320, 490, 70, Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, "Arial10", "This is a test message of the description of some item that will be in the database by the time this game is completeld", true);


            m_buttonList2d = new ButtonList2D(this, m_game1, "ButtonList2d", 380, 70, 311, 246, Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, Color.Transparent, Color.Transparent, 60, 60, 5, 10, 5, 10, "IconFrame001", "None", "Arial18", true);
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the content
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Main", new Vector2(80 - GlobalVariables.FontDict["Arial10"].MeasureString("Main").X / 2, 132), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Off-Hand", new Vector2(80 - GlobalVariables.FontDict["Arial10"].MeasureString("Off-Hand").X / 2, 217), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Passive", new Vector2(80 - GlobalVariables.FontDict["Arial10"].MeasureString("Passive").X / 2, 302), Color.Black);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Special #1", new Vector2(150 - GlobalVariables.FontDict["Arial10"].MeasureString("Special #1").X / 2, 132), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Special #2", new Vector2(150 - GlobalVariables.FontDict["Arial10"].MeasureString("Special #2").X / 2, 217), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Special #3", new Vector2(150 - GlobalVariables.FontDict["Arial10"].MeasureString("Special #3").X / 2, 302), Color.Black);

            spriteBatch.DrawString(GlobalVariables.FontDict["Arial10"], "Exp: " + GlobalVariables.CharLoadout.Exp, new Vector2(50, 66 - GlobalVariables.FontDict["Arial10"].MeasureString("Exp: " + GlobalVariables.CharLoadout.Exp).Y), Color.Black);
            spriteBatch.End();
            m_descBox.Draw(spriteBatch, graphicsDevice, offset);
            m_buttonList2d.Draw(spriteBatch, graphicsDevice, offset);
            //base.DrawContent(spriteBatch, graphicsDevice, offset);

            int i = 0;
            int j;
            foreach (List<MenuItem> col in m_menuItems)
            {
                j = 0;
                foreach (MenuItem menuItem in col)
                {
                    if(!((i == 2 && j == 1) || (i == 2 && j == 2) || (i == 1 && j == 3)))
                        menuItem.Draw(spriteBatch, graphicsDevice, offset);
                    ++j;
                }
                ++i;
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
            //Handle the 2d button list input
            m_buttonList2d.HandleInput(playerOneController);
            base.HandleInput(playerOneController);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // customization menu 
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Button tempButton;

            tempButton = ((Button)FindItem("MainHandButton"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.MainHand + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["MainHand"].Name + "," + GlobalVariables.CharLoadout.MainHand;

            tempButton = ((Button)FindItem("OffHandButton"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.OffHand + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["OffHand"].Name + "," + GlobalVariables.CharLoadout.OffHand;

            tempButton = ((Button)FindItem("PassiveButton"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.Passive + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["Passive"].Name + "," + GlobalVariables.CharLoadout.Passive;

            tempButton = ((Button)FindItem("Special1Button"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.Special1 + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["Special1"].Name + "," + GlobalVariables.CharLoadout.Special1;

            tempButton = ((Button)FindItem("Special2Button"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.Special2 + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["Special2"].Name + "," + GlobalVariables.CharLoadout.Special2;

            tempButton = ((Button)FindItem("Special3Button"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.Special3 + "_Icon";
            tempButton.Text = GlobalVariables.CharLoadout.AbilityDict["Special3"].Name + "," + GlobalVariables.CharLoadout.Special3;
            
            
            tempButton = ((Button)FindItem("AvatarButton"));
            tempButton.ButtonImage = GlobalVariables.CharLoadout.Avatar + "_Icon";
            if (NetworkingHandler.ServerPurchaseRequestResponse != "")
            {
                FindMessagebox("WaitForPurchaseRequestResponse").ClosePopUp(this);
                PopUpMessage(m_buttonList2d, "PurchaseRequestResponse", NetworkingHandler.ServerPurchaseRequestResponse, messageBoxType.ok, SelectAbility2dMenu);
                if(NetworkingHandler.ServerPurchaseRequestResponse.StartsWith("You have successfully purchased "))
                {
                    string[] categoryAndName = ((Button)m_buttonList2d.CurrSelectedItem).Text.Split(',');
                    GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]].Owned = true;
                    GlobalVariables.CharLoadout.Exp -= GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]].Cost;
                }
                NetworkingHandler.ServerPurchaseRequestResponse = "";
            }
        }

        //****************************************************
        // Method: Initialize
        //
        // Purpose: Initializes the menu
        //****************************************************
        public override void Initialize()
        {
            m_buttonList2d.ClearList();
            base.Initialize();
        }

        //****************************************************
        // Method: ClearFocus
        //
        // Purpose: Clears the focus of all menu items
        //****************************************************
        public override void ClearFocus()
        {
            base.ClearFocus();
            m_buttonList2d.KillFocus(this);
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

        #region Menu Item Event Handlers
        //****************************************************
        // Method: ClickAbility
        //
        // Purpose: When an 'ability' (or weapon) category
        // is clicked on, it moves to the m_buttonList2d
        // to allow the user to make their selection.
        //****************************************************
        public void ClickAbility(object sender)
        {
            m_buttonList2d.Initialize();
            GiveFocus(m_buttonList2d);
        }

        //****************************************************
        // Method: SelectAbility
        //
        // Purpose: When an 'ability' (or weapon) category
        // 'selected' or 'hovered over', request the list.
        // of abilities/weapons/passives that correspond to
        // that category
        //****************************************************
        public void SelectAbility(object sender)
        {
            List<List<string>> categories = new List<List<string>>();
            categories.Add(new List<string>());
            categories.Add(new List<string>());

            categories[0].Add("MainHand");
            categories[0].Add("OffHand");
            categories[0].Add("Passive");

            categories[1].Add("Special");
            categories[1].Add("Special");
            categories[1].Add("Special");

            m_descBox.Text = ""; //Clears the text box
            m_buttonList2d.ClearList();
            foreach (KeyValuePair<string, AbilityInfo> pair in GlobalVariables.AbilityDict[categories[m_currSelection.X][m_currSelection.Y]])
            {
                m_buttonList2d.AddButton(categories[m_currSelection.X][m_currSelection.Y] + "," + pair.Key, pair.Key + "_Icon", PickAbility, null, null, SelectAbility2dMenu);
            }

            categories.Clear();
            categories.Add(new List<string>());
            categories.Add(new List<string>());

            categories[0].Add("MainHand");
            categories[0].Add("OffHand");
            categories[0].Add("Passive");

            categories[1].Add("Special1");
            categories[1].Add("Special2");
            categories[1].Add("Special3");

            AbilityInfo tempAbility = GlobalVariables.CharLoadout.AbilityDict[categories[m_currSelection.X][m_currSelection.Y]];
            m_descBox.Text = "Name: " + tempAbility.Name;
            SpriteFont tempFont = GlobalVariables.FontDict[m_descBox.Font];
            string priceString;
            if (tempAbility.Owned)
                priceString = "Price: Owned!";
            else
                priceString = "Price: " + tempAbility.Cost.ToString();

            int tempSpaceCount = 0;
            tempSpaceCount = (m_descBox.ContentRect.Width - ((int)tempFont.MeasureString(tempAbility.Name + priceString + "Name: ").X)) / ((int)Math.Round(tempFont.MeasureString(" ").X));

            for (int i = 0; i < tempSpaceCount; ++i)
                m_descBox.Text += " ";
            if (tempFont.MeasureString(m_descBox.Text + priceString).X >= m_descBox.ContentRect.Width)
            {
                int tempRemoveSpaces = ((int)Math.Ceiling(tempFont.MeasureString(m_descBox.Text + priceString).X) - m_descBox.ContentRect.Width) / ((int)tempFont.MeasureString(" ").X);
                m_descBox.Text = m_descBox.Text.Remove(m_descBox.Text.Length - tempRemoveSpaces - 1);
            }

            m_descBox.Text += priceString + "\n" + tempAbility.Description;

        }

        //****************************************************
        // Method: PickAbility
        //
        // Purpose: When an 'ability' in the 2d menu is
        // clicked, the player's choice is changed.
        //****************************************************
        public void PickAbility(object sender)
        {
            List<List<string>> categories = new List<List<string>>();
            categories.Add(new List<string>());
            categories.Add(new List<string>());

            categories[0].Add("MainHand");
            categories[0].Add("OffHand");
            categories[0].Add("Passive");

            categories[1].Add("Special1");
            categories[1].Add("Special2");
            categories[1].Add("Special3");
            string[] categoryAndName = ((Button)m_buttonList2d.CurrSelectedItem).Text.Split(',');
            if (GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]].Owned)
            {
                switch(categories[m_currSelection.X][m_currSelection.Y])
                {
                    case "Special1":
                        GlobalVariables.CharLoadout.Special1 = categoryAndName[1];
                        break;
                    case "Special2":
                        GlobalVariables.CharLoadout.Special2 = categoryAndName[1];
                        break;
                    case "Special3":
                        GlobalVariables.CharLoadout.Special3 = categoryAndName[1];
                        break;
                    default:
                        GlobalVariables.CharLoadout.AbilityDict[categories[m_currSelection.X][m_currSelection.Y]] = GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]];
                        break;
                }

                NetworkingHandler.UpdateSelectedAbility(categories[m_currSelection.X][m_currSelection.Y], categoryAndName[1]);

                m_buttonList2d.Back();
            }
            else if (GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]].Cost > GlobalVariables.CharLoadout.Exp)
            {
                PopUpMessage(m_buttonList2d, "CannotAfford", "You cannot afford this ability!", messageBoxType.ok);
            }
            else
            {
                PopUpMessage(m_buttonList2d, "PurchaseDecision", "Would you like to purchase this ability?", messageBoxType.yesNo, PurchaseAbility);
            }
        }

        //****************************************************
        // Method: SelectAbility2dMenu
        //
        // Purpose: When an 'ability' in the 2d menu is
        // selected, the description changes
        //****************************************************
        public void SelectAbility2dMenu(object sender)
        {
            string[] categoryAndName = ((Button)m_buttonList2d.CurrSelectedItem).Text.Split(',');

            AbilityInfo tempAbility = GlobalVariables.AbilityDict[categoryAndName[0]][categoryAndName[1]];
            m_descBox.Text = "Name: " + tempAbility.Name;
            SpriteFont tempFont = GlobalVariables.FontDict[m_descBox.Font];
            string priceString;
            if (tempAbility.Owned)
                priceString = "Price: Owned!";
            else
                priceString = "Price: " + tempAbility.Cost.ToString();

            int tempSpaceCount = 0;
            tempSpaceCount = (m_descBox.ContentRect.Width - ((int)tempFont.MeasureString(tempAbility.Name + priceString + "Name: ").X)) / ((int)Math.Round(tempFont.MeasureString(" ").X));

            for (int i = 0; i < tempSpaceCount; ++i)
                m_descBox.Text += " ";
            if (tempFont.MeasureString(m_descBox.Text + priceString).X >= m_descBox.ContentRect.Width)
            {
                int tempRemoveSpaces = ((int)Math.Ceiling(tempFont.MeasureString(m_descBox.Text + priceString).X) - m_descBox.ContentRect.Width) / ((int)tempFont.MeasureString(" ").X);
                m_descBox.Text = m_descBox.Text.Remove(m_descBox.Text.Length - tempRemoveSpaces - 1);
            }

            m_descBox.Text += priceString + "\n" + tempAbility.Description;
        }

        //****************************************************
        // Method: PurchaseAbility
        //
        // Purpose: When the user decides that they want to
        // purchase an ability via the popup menu, this
        // fires.
        //****************************************************
        public void PurchaseAbility(object sender)
        {
            List<List<string>> categories = new List<List<string>>();
            categories.Add(new List<string>());
            categories.Add(new List<string>());

            categories[0].Add("MainHand");
            categories[0].Add("OffHand");
            categories[0].Add("Passive");

            categories[1].Add("Special1");
            categories[1].Add("Special2");
            categories[1].Add("Special3");

            string[] categoryAndName = ((Button)m_buttonList2d.CurrSelectedItem).Text.Split(',');
            NetworkingHandler.PurchaseAbility(categories[m_currSelection.X][m_currSelection.Y], categoryAndName[1]);

            NetworkingHandler.ServerPurchaseRequestResponse = "";

            PopUpMessage(m_buttonList2d, "WaitForPurchaseRequestResponse", "Purchasing ability...", messageBoxType.locked);
        }

        //****************************************************
        // Method: CycleAvatar
        //
        // Purpose: When the avatar button is clicked on, 
        // it'll cycle the avatar the player has selected.
        //****************************************************
        public void CycleAvatar(object sender)
        {
            for (int i = 0; i < GlobalVariables.AvatarList.Count; ++i)
            {
                if (GlobalVariables.AvatarList[i] == GlobalVariables.CharLoadout.Avatar)
                {
                    ++i;
                    if (i >= GlobalVariables.AvatarList.Count)
                        i = 0;

                    GlobalVariables.CharLoadout.Avatar = GlobalVariables.AvatarList[i];
                }
            }

            NetworkingHandler.UpdateSelectedAbility("Avatar", GlobalVariables.CharLoadout.Avatar);
        }

        //****************************************************
        // Method: GoBack
        //
        // Purpose: Event handler for BackButton
        //****************************************************
        public void GoBack(object sender)
        {
            Back();
        }
        #endregion

    }
}

