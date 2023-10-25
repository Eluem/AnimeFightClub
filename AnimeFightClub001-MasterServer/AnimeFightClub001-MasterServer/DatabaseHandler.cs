//**********************************************************
// File: DatabaseHandler.cs
//
// Purpose: Used to talk to the database.
//
// Written By: Salvatore Hanusiewicz
//**********************************************************
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Lidgren.Network;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;

namespace AnimeFightClub001_MasterServer
{
    class DatabaseHandler
    {
        #region Declarations
        protected DataSet m_myDataSet; //DataSet to be populated from database which changes to will be commited
        //back in to the database

        protected DataSet m_queryDataSet;

        protected string m_connectionString; //String which is used to connect to the database

        protected List<string> m_tableList; //List of tables that are communicated with
        #endregion


        //****************************************************
        // Method: DatabaseHandler
        //
        // Purpose: Constructor for DatabaseHandler
        //****************************************************
        public DatabaseHandler(string dataSource)
        {
            //Initializes the connection string
            m_connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + dataSource;
            m_tableList = new List<string>();
            m_tableList.Add("User");
            m_tableList.Add("Character");
            m_tableList.Add("Inventory");
            m_tableList.Add("Item");
            DataSetUpdater(m_tableList);
        }


        //***********************************************************
        // Method: DataSetUpdater
        //
        // Description: Used to updated the DataSet object that
        // will be used to represent data from the database.
        //***********************************************************
        public void DataSetUpdater(List<string> tableList)
        {
            m_myDataSet = new DataSet("AnimeFightClubdb");
            OleDbConnection conn = new OleDbConnection(m_connectionString);
            try
            {
                conn.Open();
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter("SELECT * FROM " + tableList[0], conn);
                foreach (string tableName in tableList)
                {
                    myDataAdapter.SelectCommand.CommandText = "SELECT * FROM [" + tableName + "]";
                    myDataAdapter.Fill(m_myDataSet, tableName);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in connection: " + ex.Message);
            }

            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }


        //***********************************************************
        // Method: RegisterNewUser
        //
        // Description: Attempts to register a new user to the
        // database. Returns a failure message if it wasn't
        // successful. (Does validation, such as checking if the
        // userName has been taken...)
        //***********************************************************
        public string RegisterNewUser(string userName, string password)
        {
            #region Check If User Name/Password are valid
            if (userName.Length < 3 || password.Length < 3)
            {
                return "User name and password must be longer than 3 characters";
            }
            #endregion

            #region Check If User Name Taken
            foreach (DataRow row in m_myDataSet.Tables["User"].Rows)
            {
                if ((string)row["UserName"] == userName)
                {
                    return "User name is taken!";
                }
            }
            #endregion

            #region Declarations/Initalizations
            /*
            OleDbConnection connection = new OleDbConnection(m_connectionString);
            OleDbCommand command = new OleDbCommand();
            OleDbTransaction transaction = null;

            // Set the Connection to the new OleDbConnection.
            command.Connection = connection;
            */
            #endregion

            #region Connect and Execute
            // Open the connection and execute the transaction.
            try
            {

                #region Add User
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    OleDbCommand command = new OleDbCommand("INSERT INTO [User]([UserName], [Password]) VALUES (@userName, @password)", connection);
                    command.Parameters.Add("@userName", OleDbType.LongVarChar);
                    command.Parameters["@userName"].Value = userName;

                    command.Parameters.Add("@password", OleDbType.LongVarChar);
                    command.Parameters["@password"].Value = password;
                    command.Connection.Open();
                    int test = command.ExecuteNonQuery();
                }
                #endregion

                #region Get User ID
                DataSetUpdater(m_tableList);
                int userID = -1;
                foreach (DataRow row in m_myDataSet.Tables["User"].Rows)
                {
                    if (row["UserName"].ToString() == userName)
                    {
                        userID = (int)row["ID"];
                    }
                }
                if (userID == -1)
                {
                    return "Error, user failed to be inserted even though it's not in the table already!?";
                }
                #endregion

                #region Add Character
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    OleDbCommand command = new OleDbCommand("Insert into [Character] ([NickName], [UserID]) VALUES ('" + userName + "', " + userID + ")", connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                #endregion

                DataSetUpdater(m_tableList);

                #region Grant User All Starting Abilities
                //Grant them the ability to use the "None" abilities
                GrantItem(userName, "MainHand", "None");
                GrantItem(userName, "MainHand", "Sword");
                GrantItem(userName, "OffHand", "None");
                GrantItem(userName, "Passive", "None");
                GrantItem(userName, "Special", "None");
                GrantItem(userName, "Special", "Fireball");
                #endregion

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            DataSetUpdater(m_tableList);
            return "Account successfully registered!";
            #endregion
        }
        //***********************************************************
        // Method: GrantItem
        //
        // Description: Gives a user ownership of an item
        //***********************************************************
        public string GrantItem(string userName, string category, string abilityName)
        {
            //Obtain character ID
            int characterID = (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["ID"];

            //Find item ID
            int itemID = (int)m_myDataSet.Tables["Item"].Select("AbilityName = '" + abilityName + "' AND Category = '" + category + "'")[0]["ID"];

            //Check if item already owned
            if (m_myDataSet.Tables["Inventory"].Select("ItemID = " + itemID + " AND CharacterID = " + characterID).Length > 0)
                return "That user already owns this item!";

            //Grant the user the item in the database and charge them
            #region Connect and Execute
            // Open the connection and execute the transaction.
            try
            {
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    #region Purchase the Item for the User
                    OleDbCommand command = new OleDbCommand("INSERT INTO [Inventory] ([CharacterID], [ItemID]) VALUES (@characterID, @itemID)", connection);
                    command.Parameters.Add("@characterID", OleDbType.Numeric);
                    command.Parameters["@characterID"].Value = characterID;

                    command.Parameters.Add("@itemID", OleDbType.Numeric);
                    command.Parameters["@itemID"].Value = itemID;
                    command.Connection.Open();
                    int test = command.ExecuteNonQuery();
                    #endregion
                }
                DataSetUpdater(m_tableList);
                return "You have successfully granted the user this item.";


            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            #endregion
        }

        //***********************************************************
        // Method: PurchaseItem
        //
        // Description: Attempts to purchase an item for a user.
        // Returns a failure message if it wasn't successful.
        // (Does validation, such as checking if the
        // had enough funds, or if the user already owned the
        // item)
        //***********************************************************
        public string PurchaseItem(string userName, string abilitySlot, string abilityName)
        {
            //Obtain name of category
            string tempCategory = abilitySlot;
            if (tempCategory == "Special1" || tempCategory == "Special2" || tempCategory == "Special3")
                tempCategory = "Special";

            //Obtain character ID
            DataRow character = m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0];
            int characterID = (int)character["ID"];
            int characterExp = (int)character["Exp"];

            //Find item ID
            DataRow item = m_myDataSet.Tables["Item"].Select("AbilityName = '" + abilityName + "' AND Category = '" + tempCategory + "'")[0];
            int itemID = (int)item["ID"];
            int itemCost = (int)item["ExpCost"];

            //Check if item already owned
            if (m_myDataSet.Tables["Inventory"].Select("ItemID = " + itemID + " AND CharacterID = " + characterID).Length > 0)
                return "You already own this item!";

            //Check if item affordable
            if (characterExp < itemCost)
                return "You cannot afford this item!";

            characterExp -= itemCost;

            //Grant the user the item in the database and charge them
            #region Connect and Execute
            // Open the connection and execute the transaction.
            try
            {
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    #region Purchase the Item for the User
                    OleDbCommand command = new OleDbCommand("INSERT INTO [Inventory] ([CharacterID], [ItemID]) VALUES (@characterID, @itemID)", connection);
                    command.Parameters.Add("@characterID", OleDbType.Numeric);
                    command.Parameters["@characterID"].Value = characterID;

                    command.Parameters.Add("@itemID", OleDbType.Numeric);
                    command.Parameters["@itemID"].Value = itemID;
                    command.Connection.Open();
                    int test = command.ExecuteNonQuery();
                    #endregion

                    #region Charge the User for the Exp
                    command = new OleDbCommand("UPDATE [Character] SET Exp = " + characterExp + " WHERE ID = " + characterID, connection);

                    test = command.ExecuteNonQuery();
                    #endregion
                }
                DataSetUpdater(m_tableList);
                return "You have successfully purchased " + abilityName;


            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            #endregion
        }

        //****************************************************
        // Method: GrantExp
        //
        // Purpose: Adds exp to a user's character
        //****************************************************
        public void GrantExp(string userName, int exp)
        {
            DataRow character = m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0];
            int characterExp = (int)character["Exp"] + exp;
            int characterID = (int)character["ID"];

            #region Connect and Execute
            // Open the connection and execute the transaction.
            try
            {
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    #region Grant the User with Exp
                    OleDbCommand command = new OleDbCommand("UPDATE [Character] SET Exp = " + characterExp + " WHERE [ID] = " + characterID, connection);

                    command.Connection.Open();
                    int test = command.ExecuteNonQuery();
                    #endregion
                }

                DataSetUpdater(m_tableList);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in GrantExp: " + ex.Message);
            }

            #endregion
        }

        //***********************************************************
        // Method: ChangeLoadout
        //
        // Description: Attempts to change the user's loadout on the
        // database. Returns a failure message if it wasn't
        // successful. (Does validation, such as checking if the
        // user has access to this ability...)
        //***********************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">Name of the user trying to change their loadout</param>
        /// <param name="category">Category of the ability they're changing</param>
        /// <param name="ability">New ability that they're selecting</param>
        /// <returns></returns>
        public string ChangeLoadout(string userName, string category, string abilityName)
        {
            int characterID;
            if (category != "Avatar")
            {
                #region Validation (Check access/valid choice)
                string tempCategory = category;
                if (tempCategory == "Special1" || tempCategory == "Special2" || tempCategory == "Special3")
                    tempCategory = "Special";
                #region Check if valid category/ability combination
                DataRow[] tempAbilityList = m_myDataSet.Tables["Item"].Select("Category = '" + tempCategory + "' and AbilityName = '" + abilityName + "'");
                if (tempAbilityList.Length <= 0)
                {
                    return "Invalid category ability combination";
                }
                #endregion

                #region Check if ability is owned
                characterID = (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["ID"];

                List<int> owned = new List<int>();
                DataRow[] dataRows = m_myDataSet.Tables["Inventory"].Select("CharacterID = " + characterID);
                foreach (DataRow row in dataRows)
                {
                    owned.Add((int)row["ItemID"]);
                }
                if (!owned.Contains((int)tempAbilityList[0]["ID"]))
                    return "You do not own this ability!";
                #endregion
                #endregion
            }
            else
            {
                characterID = (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["ID"];
            }

            #region Declarations/Initalizations
            /*
            OleDbConnection connection = new OleDbConnection(m_connectionString);
            OleDbCommand command = new OleDbCommand();
            OleDbTransaction transaction = null;

            // Set the Connection to the new OleDbConnection.
            command.Connection = connection;
            */
            #endregion

            #region Connect and Execute
            // Open the connection and execute the transaction.
            try
            {
                using (OleDbConnection connection = new OleDbConnection(m_connectionString))
                {
                    OleDbCommand command = new OleDbCommand("UPDATE [Character] SET " + category + " = '" + abilityName + "' WHERE ID = " + characterID, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                /*
                connection.Open();
                #region Update Character Loadout
                // Start a local transaction
                transaction = connection.BeginTransaction();
                // Assign transaction object for a pending local transaction.
                command.Connection = connection;
                command.Transaction = transaction;
                // Execute the commands.
                command.CommandType = CommandType.Text;
                command.CommandText =  "UPDATE [Character] SET " + category + " = '" + abilityName + "' WHERE ID = " + characterID;
                int test = command.ExecuteNonQuery();
                // Commit the transaction.
                transaction.Commit();
                #endregion
                */


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                try
                {
                    // Attempt to roll back the transaction.
                    //transaction.Rollback();
                    return "Error: " + ex.Message;
                }

                catch (Exception ex2)
                {
                    MessageBox.Show("Error: " + ex2.Message);
                    return "Error: " + ex2.Message;
                }
            }

            DataSetUpdater(m_tableList);
            //connection.Close();
            return "success";
            #endregion
        }

        //***********************************************************
        // Method: QueryAbilityList
        //
        // Description: Attempts to query the database for all the
        // abilities in the game.
        // Returns a null if it wasn't successful.
        //***********************************************************
        public List<AbilityInfo> QueryAbilityList(string userName)
        {
            List<AbilityInfo> tempAbilityInfoList = new List<AbilityInfo>();

            int characterID = (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["ID"];

            List<int> owned = new List<int>();
            DataRow[] dataRows = m_myDataSet.Tables["Inventory"].Select("CharacterID = " + characterID);
            foreach (DataRow row in dataRows)
            {
                owned.Add((int)row["ItemID"]);
            }

            foreach (DataRow row in m_myDataSet.Tables["Item"].Rows)
            {
                tempAbilityInfoList.Add(new AbilityInfo(row["AbilityName"].ToString(), row["Category"].ToString(), row["Desc"].ToString(), (int)row["ExpCost"], owned.Contains((int)row["ID"])));
            }
            return tempAbilityInfoList;
        }


        //***********************************************************
        // Method: QueryCharacterLoadout
        //
        // Description: Attempts to query the database for the
        // currently selected loadout of a particular player.
        // Returns a null if it wasn't successful.
        //***********************************************************
        public Dictionary<string, AbilityInfo> QueryCharacterLoadout(string userName)
        {
            Dictionary<string, AbilityInfo> tempCharacterLoadout = new Dictionary<string, AbilityInfo>();
            int characterID = (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["ID"];

            List<int> owned = new List<int>();
            DataRow[] dataRows = m_myDataSet.Tables["Character"].Select("ID = " + characterID);

            //The ability info class should have the ability to populate itself from the ability dictionary??? idk this structure has to be much more well designed in the future!!!
            tempCharacterLoadout.Add("MainHand", new AbilityInfo(dataRows[0]["MainHand"].ToString()));
            tempCharacterLoadout.Add("OffHand", new AbilityInfo(dataRows[0]["OffHand"].ToString()));
            tempCharacterLoadout.Add("Passive", new AbilityInfo(dataRows[0]["Passive"].ToString()));
            tempCharacterLoadout.Add("Special1", new AbilityInfo(dataRows[0]["Special1"].ToString()));
            tempCharacterLoadout.Add("Special2", new AbilityInfo(dataRows[0]["Special2"].ToString()));
            tempCharacterLoadout.Add("Special3", new AbilityInfo(dataRows[0]["Special3"].ToString()));
            tempCharacterLoadout.Add("Avatar", new AbilityInfo(dataRows[0]["Avatar"].ToString()));

            return tempCharacterLoadout;
        }

        //***********************************************************
        // Method: ValidateUser
        //
        // Description: Checks if player/password combo exists in the
        // database.
        //***********************************************************
        public bool ValidateUser(string userName, string password)
        {
            if (m_myDataSet.Tables["User"].Select("UserName = '" + userName + "' and Password = '" + password + "'").Length == 1)
                return true;

            return false;
        }


        //***********************************************************
        // Method: QueryCharacterExp
        //
        // Description: Returns this user's character's exp as a
        // unsigned 32-bit integer.
        //***********************************************************
        public int QueryCharacterExp(string userName)
        {
            return (int)m_myDataSet.Tables["Character"].Select("UserID = " + m_myDataSet.Tables["User"].Select("UserName = '" + userName + "'")[0]["ID"].ToString())[0]["Exp"];
        }


        #region Properties
        #endregion
    }
}

