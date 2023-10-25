//******************************************************
// File: GameObjectHandler.cs
//
// Purpose: Contains the definition of the
// GameObjectHandler. This is a static class which
// will be used to handle all updates and object
// interactions.
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
    static class GameObjectHandler
    {
        #region Declarations
        //***************************************************************************************************************************
        //NOTE: Eventually I'd like to replace the useage of these lists with my own class/struct that uses an array of X elements
        //to store objects. Deletion will be completed by marking the object as deleted. When and update or collide code gets
        //to it, it'll be skipped (slightly inefficent). Any deleted object will have it's index added to another list. This list
        //will be used to determine where to place the next added object. If the overwriteable index list is empty, the object goes
        //to the last open slot. If the list isn't empty, it is inserted into the index that is stored in the last slot of the
        //overwriteable index list
        //***************************************************************************************************************************


        //Lists that store all the different types of objects in the game
        static private LinkedList<PlayerObj> m_playerObjList = new LinkedList<PlayerObj>();
        static private LinkedList<HazardObj> m_hazardObjList = new LinkedList<HazardObj>();
        static private LinkedList<EnvironmentalObj> m_environmentalObjList = new LinkedList<EnvironmentalObj>();
        static private LinkedList<SpecialEnvironmentalObj> m_specialEnvironmentalObjList = new LinkedList<SpecialEnvironmentalObj>();
        static private LinkedList<ItemObj> m_itemObjList = new LinkedList<ItemObj>();
        static private LinkedList<BasicObj> m_basicObjList = new LinkedList<BasicObj>(); //Not likely to be used

        //Lists that are used to delete objects from the corresponding object lists
        static private LinkedList<PlayerObj> m_playerObjDeleteList = new LinkedList<PlayerObj>();
        static private LinkedList<HazardObj> m_hazardObjDeleteList = new LinkedList<HazardObj>();
        static private LinkedList<EnvironmentalObj> m_environmentalObjDeleteList = new LinkedList<EnvironmentalObj>();
        static private LinkedList<SpecialEnvironmentalObj> m_specialEnvironmentalObjDeleteList = new LinkedList<SpecialEnvironmentalObj>();
        static private LinkedList<ItemObj> m_itemObjDeleteList = new LinkedList<ItemObj>();
        static private LinkedList<BasicObj> m_basicObjDeleteList = new LinkedList<BasicObj>(); //Not likely to be used

        //Lists that are used to add objects to the corresponding object lists
        static private LinkedList<PlayerObj> m_playerObjAddList = new LinkedList<PlayerObj>();
        static private LinkedList<HazardObj> m_hazardObjAddList = new LinkedList<HazardObj>();
        static private LinkedList<EnvironmentalObj> m_environmentalObjAddList = new LinkedList<EnvironmentalObj>();
        static private LinkedList<SpecialEnvironmentalObj> m_specialEnvironmentalObjAddList = new LinkedList<SpecialEnvironmentalObj>();
        static private LinkedList<ItemObj> m_itemObjAddList = new LinkedList<ItemObj>();
        static private LinkedList<BasicObj> m_basicObjAddList = new LinkedList<BasicObj>(); //Not likely to be used

        static private Camera2D m_trackingCamera = new Camera2D();


        static private List<Point> m_respawnPoints = new List<Point>();
        static private int m_lastRespawnPointUsed = -1; //Used to indicate the last respawn point that was generated, to prevent clustering
        //static private List<int> m_respawnPointLockTimer = new List<int>();

        //The x width +/- 0 that causes a player to die and the lowest point of the map that a player cna exist at before dying
        static private Point m_mapBounds = new Point(10000, 3000);

        #endregion

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates all the objects in the game
        // and checks for collisions.
        //****************************************************
        static public void Update(GameTime gameTime, Viewport viewport)
        {
            #region Get/Apply Networking Updates
            #endregion

            #region Add All Objects Scheduled To Be Added
            foreach (PlayerObj player in m_playerObjAddList)
            {
                m_playerObjList.AddLast(player);
            }

            foreach (HazardObj hazard in m_hazardObjAddList)
            {
                m_hazardObjList.AddLast(hazard);
            }

            foreach (EnvironmentalObj environmentalObj in m_environmentalObjAddList)
            {
                m_environmentalObjList.AddLast(environmentalObj);
            }

            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in m_specialEnvironmentalObjAddList)
            {
                m_specialEnvironmentalObjList.AddLast(specialEnvironmentalObj);
            }
            foreach (ItemObj item in m_itemObjAddList)
            {
                m_itemObjList.AddLast(item);
            }

            m_playerObjAddList.Clear();
            m_hazardObjAddList.Clear();
            m_environmentalObjAddList.Clear();
            m_specialEnvironmentalObjAddList.Clear();
            m_itemObjAddList.Clear();
            #endregion

            #region Delete All Objects Scheduled To Be Deleted
            foreach (PlayerObj player in m_playerObjDeleteList)
            {
                m_playerObjList.Remove(player);
            }

            foreach (HazardObj hazard in m_hazardObjDeleteList)
            {
                m_hazardObjList.Remove(hazard);
            }

            foreach (EnvironmentalObj environmentalObj in m_environmentalObjDeleteList)
            {
                m_environmentalObjList.Remove(environmentalObj);
            }

            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in m_specialEnvironmentalObjDeleteList)
            {
                m_specialEnvironmentalObjList.Remove(specialEnvironmentalObj);
            }
            foreach (ItemObj item in m_itemObjDeleteList)
            {
                m_itemObjList.Remove(item);
            }

            m_playerObjDeleteList.Clear();
            m_hazardObjDeleteList.Clear();
            m_environmentalObjDeleteList.Clear();
            m_specialEnvironmentalObjDeleteList.Clear();
            m_itemObjDeleteList.Clear();
            #endregion

            #region Update All Object Types
            foreach (PlayerObj player in m_playerObjList)
            {
                player.Update(gameTime, viewport);
            }

            foreach (HazardObj hazard in m_hazardObjList)
            {
                hazard.Update(gameTime, viewport);
            }

            foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
            {
                environmentalObj.Update(gameTime, viewport);
            }

            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in m_specialEnvironmentalObjList)
            {
                specialEnvironmentalObj.Update(gameTime, viewport);
            }
            foreach (ItemObj item in m_itemObjList)
            {
                item.Update(gameTime, viewport);
            }
            #endregion


            #region Checks For And Applies All Collisions
            //*******************************************************************************************
            // Loop Logic Explanation: 
            // The outer loop for every object type loops through all the objects in the specified list
            // (excluding the last element due to the combinumetrics) and will check to the objects in
            // the inner loop. When collide checking an object type to itself you need to use a pointer
            // to store the first element which should be looped through.
            // This element should be incremented to the second element immediately.
            // Then, a second pointer should be set equal to it and should be used to loop through
            // the rest of the objects. On the next loop through, the pointer which is pointing to the
            // first element of the inner loop should be incremented and the process should be repeated.
            // This is done to prevent having objects collide check themselves.
            //*******************************************************************************************
            Vector2 overLap = new Vector2(); //Used to pass the overlap from function to function
            int currentElement; //Used to prevent the outer loop from collide checking the last element

            #region SpecialEnvironmentalObj Colllision Loop
            //Used to point to the first element in the inner loop
            LinkedListNode<SpecialEnvironmentalObj> firstInnerSpecialEnvironmentalObj = null;
            if (m_specialEnvironmentalObjList.Count > 1)
                firstInnerSpecialEnvironmentalObj = m_specialEnvironmentalObjList.First.Next;
            //Used to point to the current element in the inner loop
            LinkedListNode<SpecialEnvironmentalObj> currentInnerSpecialEnvironmentalObj;

            currentElement = 0;
            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in m_specialEnvironmentalObjList)
            {
                #region SpecialEnvironmentalObj to SpecialEnvironmentalObj
                //Loop through all other objects in this list
                if (currentElement + 1 < m_specialEnvironmentalObjList.Count)
                {
                    currentInnerSpecialEnvironmentalObj = firstInnerSpecialEnvironmentalObj;

                    for (int currentInnerElement = currentElement + 1; currentInnerElement < m_specialEnvironmentalObjList.Count; ++currentInnerElement)
                    {
                        if (CollideCheck(specialEnvironmentalObj.PhysicsObj, currentInnerSpecialEnvironmentalObj.Value.PhysicsObj, ref overLap))
                        {
                            //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                            specialEnvironmentalObj.CollideBefore(currentInnerSpecialEnvironmentalObj.Value);
                            currentInnerSpecialEnvironmentalObj.Value.CollideBefore(specialEnvironmentalObj);

                            //Transfer momentum
                            MomentumTransfer(specialEnvironmentalObj.PhysicsObj, currentInnerSpecialEnvironmentalObj.Value.PhysicsObj, overLap, gameTime);

                            specialEnvironmentalObj.CollideAfter(currentInnerSpecialEnvironmentalObj.Value);
                            currentInnerSpecialEnvironmentalObj.Value.CollideAfter(specialEnvironmentalObj);
                        }
                        currentInnerSpecialEnvironmentalObj = currentInnerSpecialEnvironmentalObj.Next;
                    }

                    firstInnerSpecialEnvironmentalObj = firstInnerSpecialEnvironmentalObj.Next;
                }
                #endregion

                #region SpecialEnvironmentalObj to PlayerObj
                foreach (PlayerObj player in m_playerObjList)
                {
                    if (!player.StatusHandler.HasStatus(StatusName.Death) && CollideCheck(specialEnvironmentalObj.PhysicsObj, player.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        specialEnvironmentalObj.CollideBefore(player);
                        player.CollideBefore(specialEnvironmentalObj);

                        //Transfer momentum
                        MomentumTransfer(specialEnvironmentalObj.PhysicsObj, player.PhysicsObj, overLap, gameTime);

                        specialEnvironmentalObj.CollideAfter(player);
                        player.CollideAfter(specialEnvironmentalObj);
                    }
                }
                #endregion

                #region SpecialEnvironmentalObj to ItemObj
                foreach (ItemObj item in m_itemObjList)
                {
                    if (CollideCheck(specialEnvironmentalObj.PhysicsObj, item.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        specialEnvironmentalObj.CollideBefore(item);
                        item.CollideBefore(specialEnvironmentalObj);

                        //Transfer momentum
                        MomentumTransfer(specialEnvironmentalObj.PhysicsObj, item.PhysicsObj, overLap, gameTime);
                        specialEnvironmentalObj.CollideAfter(item);
                        item.CollideAfter(specialEnvironmentalObj);
                    }
                }
                #endregion

                #region SpecialEnvironmentalObj to HazardObj
                foreach (HazardObj hazard in m_hazardObjList)
                {

                    if (CollideCheck(specialEnvironmentalObj.PhysicsObj, hazard.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        specialEnvironmentalObj.CollideBefore(hazard);
                        hazard.CollideBefore(specialEnvironmentalObj);

                        //First check if hazard is "solid enough"
                        if (hazard.Solidity == HazardSolidity.Full || hazard.Solidity == HazardSolidity.Semi)
                        {
                            //Transfer momentum
                            MomentumTransfer(specialEnvironmentalObj.PhysicsObj, hazard.PhysicsObj, overLap, gameTime);
                        }

                        specialEnvironmentalObj.CollideAfter(hazard);
                        hazard.CollideAfter(specialEnvironmentalObj);
                    }
                }
                #endregion

                #region SpecialEnvironmentalObj to EnvironmentalObj
                foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
                {
                    if (CollideCheck(specialEnvironmentalObj.PhysicsObj, environmentalObj.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        specialEnvironmentalObj.CollideBefore(environmentalObj);
                        environmentalObj.CollideBefore(specialEnvironmentalObj);

                        //Transfer momentum
                        MomentumTransfer(specialEnvironmentalObj.PhysicsObj, environmentalObj.PhysicsObj, overLap, gameTime);

                        specialEnvironmentalObj.CollideAfter(environmentalObj);
                        environmentalObj.CollideAfter(specialEnvironmentalObj);
                    }
                }
                #endregion


                ++currentElement;
            }
            #endregion

            #region ItemObj Colllision Loop
            //Used to point to the first element in the inner loop
            LinkedListNode<ItemObj> firstInnerItemObj = null;
            if (m_itemObjList.Count > 1)
                firstInnerItemObj = m_itemObjList.First.Next;

            //Used to point to the current element in the inner loop
            LinkedListNode<ItemObj> currentInnerItemObj;

            currentElement = 0;
            foreach (ItemObj item in m_itemObjList)
            {
                #region ItemObj to ItemObj
                //Loop through all other objects in this list
                if (currentElement + 1 < m_itemObjList.Count)
                {
                    currentInnerItemObj = firstInnerItemObj;
                    for (int currentInnerElement = currentElement + 1; currentInnerElement < m_itemObjList.Count; ++currentInnerElement)
                    {
                        if (CollideCheck(item.PhysicsObj, currentInnerItemObj.Value.PhysicsObj, ref overLap))
                        {
                            //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                            item.CollideBefore(currentInnerItemObj.Value);
                            currentInnerItemObj.Value.CollideBefore(item);

                            //Transfer momentum
                            MomentumTransfer(item.PhysicsObj, currentInnerItemObj.Value.PhysicsObj, overLap, gameTime);

                            item.CollideAfter(currentInnerItemObj.Value);
                            currentInnerItemObj.Value.CollideAfter(item);
                        }
                        currentInnerItemObj = currentInnerItemObj.Next;
                    }

                    firstInnerItemObj = firstInnerItemObj.Next;
                }
                #endregion

                #region ItemObj to PlayerObj
                foreach (PlayerObj player in m_playerObjList)
                {
                    if (!player.StatusHandler.HasStatus(StatusName.Death) && CollideCheck(item.PhysicsObj, player.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        item.CollideBefore(player);
                        player.CollideBefore(item);

                        //Transfer momentum
                        MomentumTransfer(item.PhysicsObj, player.PhysicsObj, overLap, gameTime);

                        item.CollideAfter(player);
                        player.CollideAfter(item);
                    }
                }
                #endregion

                #region ItemObj to HazardObj
                foreach (HazardObj hazard in m_hazardObjList)
                {

                    if (CollideCheck(item.PhysicsObj, hazard.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        item.CollideBefore(hazard);
                        hazard.CollideBefore(item);

                        //First check if hazard is "solid enough"
                        if (hazard.Solidity == HazardSolidity.Full || hazard.Solidity == HazardSolidity.Semi)
                        {
                            //Transfer momentum
                            MomentumTransfer(item.PhysicsObj, hazard.PhysicsObj, overLap, gameTime);
                        }

                        item.CollideAfter(hazard);
                        hazard.CollideAfter(item);
                    }
                }
                #endregion

                #region ItemObj to EnvironmentalObj
                foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
                {
                    if (CollideCheck(item.PhysicsObj, environmentalObj.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        item.CollideBefore(environmentalObj);
                        environmentalObj.CollideBefore(item);

                        //Transfer momentum
                        MomentumTransfer(item.PhysicsObj, environmentalObj.PhysicsObj, overLap, gameTime);

                        item.CollideAfter(environmentalObj);
                        environmentalObj.CollideAfter(item);
                    }
                }
                #endregion


                ++currentElement;
            }
            #endregion

            #region PlayerObj Colllision Loop
            //Used to point to the first element in the inner loop
            LinkedListNode<PlayerObj> firstInnerPlayerObj = null;
            if (m_playerObjList.Count > 1)
                firstInnerPlayerObj = m_playerObjList.First.Next;

            //Used to point to the current element in the inner loop
            LinkedListNode<PlayerObj> currentInnerPlayerObj;

            currentElement = 0;
            foreach (PlayerObj player in m_playerObjList)
            {
                #region PlayerObj to PlayerObj
                //Loop through all other objects in this list
                if (currentElement + 1 < m_playerObjList.Count)
                {
                    currentInnerPlayerObj = firstInnerPlayerObj;
                    for (int currentInnerElement = currentElement + 1; currentInnerElement < m_playerObjList.Count; ++currentInnerElement)
                    {
                        if (!player.StatusHandler.HasStatus(StatusName.Death) && !currentInnerPlayerObj.Value.StatusHandler.HasStatus(StatusName.Death) && CollideCheck(player.PhysicsObj, currentInnerPlayerObj.Value.PhysicsObj, ref overLap))
                        {
                            //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                            player.CollideBefore(currentInnerPlayerObj.Value);
                            currentInnerPlayerObj.Value.CollideBefore(player);

                            //Transfer momentum
                            MomentumTransfer(player.PhysicsObj, currentInnerPlayerObj.Value.PhysicsObj, overLap, gameTime);

                            player.CollideAfter(currentInnerPlayerObj.Value);
                            currentInnerPlayerObj.Value.CollideAfter(player);
                        }
                        currentInnerPlayerObj = currentInnerPlayerObj.Next;
                    }

                    firstInnerPlayerObj = firstInnerPlayerObj.Next;
                }
                #endregion

                #region PlayerObj to HazardObj
                foreach (HazardObj hazard in m_hazardObjList)
                {
                    //First check if hazard is owned by the player and is still safe
                    if (!player.StatusHandler.HasStatus(StatusName.Death) && !(hazard.SafeClock > 0 && hazard.OwnerID == player.ObjectID))
                    {
                        if (CollideCheck(player.PhysicsObj, hazard.PhysicsObj, ref overLap))
                        {
                            //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                            player.CollideBefore(hazard);
                            hazard.CollideBefore(player);

                            //First check if hazard is "solid enough"
                            if (hazard.Solidity == HazardSolidity.Full)
                            {
                                //Transfer momentum
                                MomentumTransfer(player.PhysicsObj, hazard.PhysicsObj, overLap, gameTime);
                            }

                            player.CollideAfter(hazard);
                            hazard.CollideAfter(player);
                        }
                    }
                }
                #endregion

                #region PlayerObj to EnvironmentalObj
                foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
                {
                    if (!player.StatusHandler.HasStatus(StatusName.Death) && CollideCheck(player.PhysicsObj, environmentalObj.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        player.CollideBefore(environmentalObj);
                        environmentalObj.CollideBefore(player);

                        //Transfer momentum
                        MomentumTransfer(player.PhysicsObj, environmentalObj.PhysicsObj, overLap, gameTime);

                        player.CollideAfter(environmentalObj);
                        environmentalObj.CollideAfter(player);
                    }
                }
                #endregion


                ++currentElement;
            }
            #endregion

            #region HazardObj Colllision Loop

            #region Part of Hazard on Hazard Collision Detection, Uncomment to enable (remove region as well)
            
            //Used to point to the first element in the inner loop
            LinkedListNode<HazardObj> firstInnerHazardObj = null;
            if(m_hazardObjList.Count > 1)
            firstInnerHazardObj = m_hazardObjList.First.Next;

            //Used to point to the current element in the inner loop
            LinkedListNode<HazardObj> currentInnerHazardObj;

            currentElement = 0;
            
            #endregion
            foreach (HazardObj hazard in m_hazardObjList)
            {
                //IF IMPLEMENTED HOW SHOULD SOLIDITY WORK? (both must be >= semi?)
                #region HazardObj to HazardObj
                //Commented out to disable Hazard on Hazard Collisions
                
                //Loop through all other objects in this list
                if (currentElement + 1 < m_hazardObjList.Count)
                {
                    currentInnerHazardObj = firstInnerHazardObj;
                    for (int currentInnerElement = currentElement + 1; currentInnerElement < m_hazardObjList.Count; ++currentInnerElement)
                    {
                        if (CollideCheck(hazard.PhysicsObj, currentInnerHazardObj.Value.PhysicsObj, ref overLap))
                        {
                            //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                            hazard.CollideBefore(currentInnerHazardObj.Value);
                            currentInnerHazardObj.Value.CollideBefore(hazard);

                            //Transfer momentum
                            //MomentumTransfer(hazard.PhysicsObj, currentInnerHazardObj.Value.PhysicsObj, overLap, gameTime);
                            
                            hazard.CollideAfter(currentInnerHazardObj.Value);
                            currentInnerHazardObj.Value.CollideAfter(hazard);
                        }
                        currentInnerHazardObj = currentInnerHazardObj.Next;
                    }

                    firstInnerHazardObj = firstInnerHazardObj.Next;
                }
                #endregion

                #region HazardObj to EnvironmentalObj
                foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
                {
                    if (CollideCheck(hazard.PhysicsObj, environmentalObj.PhysicsObj, ref overLap))
                    {
                        //Tell objects that they collided with each other (add DeepCloning to keep consistency?)
                        hazard.CollideBefore(environmentalObj);
                        environmentalObj.CollideBefore(hazard);

                        //First check if hazard is "solid enough"
                        if (hazard.Solidity == HazardSolidity.Full || hazard.Solidity == HazardSolidity.Semi)
                        {
                            //Transfer momentum
                            MomentumTransfer(hazard.PhysicsObj, environmentalObj.PhysicsObj, overLap, gameTime);
                        }

                        hazard.CollideAfter(environmentalObj);
                        environmentalObj.CollideAfter(hazard);
                    }
                }
                #endregion

                ++currentElement;
            }
            #endregion

            #endregion

            RemoveOutOfBoundsObjects();
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws all the objects
        //****************************************************
        static public void Draw(SpriteBatch spriteBatch)
        {
            #region Draw All Object Types
            foreach (PlayerObj player in m_playerObjList)
            {
                player.Draw(spriteBatch);
            }

            foreach (HazardObj hazard in m_hazardObjList)
            {
                hazard.Draw(spriteBatch);
            }

            foreach (EnvironmentalObj environmentalObj in m_environmentalObjList)
            {
                environmentalObj.Draw(spriteBatch);
            }

            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in m_specialEnvironmentalObjList)
            {
                specialEnvironmentalObj.Draw(spriteBatch);
            }
            foreach (ItemObj item in m_itemObjList)
            {
                item.Draw(spriteBatch);
            }
            #endregion
        }

        //****************************************************
        // Method: LoadMap
        //
        // Purpose: Loads the selected map
        //****************************************************
        static public void LoadMap(string mapName)
        {
            m_mapBounds.X = 10000;
            m_mapBounds.Y = 3000;
            m_respawnPoints.Clear();
            switch (mapName)
            {
                #region One
                case "One":
                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(0, 192);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(0, 1093);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2126, 192);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2126, 1093);


                    GameObjectHandler.DirectAddObject(new ShortVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(199, 0);

                    GameObjectHandler.DirectAddObject(new ShortVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(199, 1171);

                    GameObjectHandler.DirectAddObject(new ShortVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2049, 0);

                    GameObjectHandler.DirectAddObject(new ShortVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2049, 1171);


                    GameObjectHandler.DirectAddObject(new SquareBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(200, 192);

                    GameObjectHandler.DirectAddObject(new SquareBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(200, 1093);

                    GameObjectHandler.DirectAddObject(new SquareBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2049, 192);

                    GameObjectHandler.DirectAddObject(new SquareBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2049, 1093);

                    
                    GameObjectHandler.DirectAddObject(new LongVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(199, 207);

                    GameObjectHandler.DirectAddObject(new LongVerticleBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2049, 207);
                    

                    GameObjectHandler.DirectAddObject(new MediumHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(377, 192);
                     
                    GameObjectHandler.DirectAddObject(new MediumHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1136, 192);

                    
                    GameObjectHandler.DirectAddObject(new LongHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(277, 1093);

                    //Add rope
                    GameObjectHandler.DirectAddObject(new Rope001());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(805, 260);

                    GameObjectHandler.DirectAddObject(new Rope001());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1520, 260);


                    //Add center platform
                    GameObjectHandler.DirectAddObject(new MediumHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(783, 730);
                    

                    //Add lower platforms
                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(0, 1502);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(277, 1602);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(527, 1652);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1087, 1752);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1087, 1393);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1627, 1652);
                     
                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(1877, 1602);

                    GameObjectHandler.DirectAddObject(new ShortHorizontalBar());
                    GameObjectHandler.EnvironmentalObjList.Last.Value.SetPos(2154, 1502);


                    //Set respawn points
                    AddRespawnPoint(300, 973);
                    AddRespawnPoint(1986, 973);
                    AddRespawnPoint(1163, 973);
                    break;
                #endregion
            }
        }

        //****************************************************
        // Method: NextMap
        //
        // Purpose: Runs the code to select and load the next
        // map
        //****************************************************
        static public void NextMap()
        {
            LoadMap("One");
            //Spawn players to proper locations
            foreach (PlayerObj player in m_playerObjList)
            {
                player.Respawn();
            }
        }

        //****************************************************
        // Method: BetweenMatchReset
        //
        // Purpose: Deletes everything in the game
        // and empties all the lists. (Except for the players)
        //****************************************************
        static public void BetweenMatchReset()
        {
            //Lists that store all the different types of objects in the game
            m_hazardObjList.Clear();
            m_environmentalObjList.Clear();
            m_specialEnvironmentalObjList.Clear();
            m_itemObjList.Clear();
            m_basicObjList.Clear();

            //Lists that are used to delete objects from the corresponding object lists
            m_hazardObjDeleteList.Clear();
            m_environmentalObjDeleteList.Clear();
            m_specialEnvironmentalObjDeleteList.Clear();
            m_itemObjDeleteList.Clear();
            m_basicObjDeleteList.Clear();

            //Lists that are used to add objects to the corresponding object lists
            m_hazardObjAddList.Clear();
            m_environmentalObjAddList.Clear();
            m_specialEnvironmentalObjAddList.Clear();
            m_itemObjAddList.Clear();
            m_basicObjAddList.Clear();

            //Clean up player objects
            foreach (PlayerObj player in m_playerObjList)
            {
                //Clear kills
                player.Kills = 0;

                //Runs the player's respawn code, to clear any of those stats before sending the worldInit out
                //(implemented mainly to fix issues with status handlers
                player.Respawn();

                //Resets all the player controllers... (will cause glitch where if someone continues to try
                //to run forward at the end of a match, the server won't respond to it, because the player has to stop trying to do the
                //action and repress the button to send a control update) However, this fixes a bug where players continue to do
                //what they were previously doing (regardless of whether or not they're still doing it) until they press something.

                for (int i = 0; i < player.ControllerState.ControlArray.Length; ++i)
                {
                    player.ControllerState.ControlArray[i] = false;
                }
            }



            m_respawnPoints.Clear();
        }

        //****************************************************
        // Method: CheckForWinCondition
        //
        // Purpose: Checks if any players have won, if someone
        // has won, then it runs all the code to complete
        // a map cycle.
        //****************************************************
        static public void CheckForWinCondition(Game1 game1)
        {
            #region Check For Winner
            bool winner = false;
            foreach (PlayerObj player in m_playerObjList)
            {
                if (player.Kills >= GlobalVariables.Settings.KillLimit)
                {
                    winner = true;
                    break;
                }
            }
            #endregion

            if (winner)
            {
                game1.currGameState = GameState.InitalizeBetweenMatches;
            }
        }

        //****************************************************
        // Method: GetRespawnPoint
        //
        // Purpose: Returns a random respawn point
        // if all spawn points are taken, returns null
        //****************************************************
        static public Point GetRespawnPoint()
        {
            if (m_respawnPoints.Count > 0)
            {
                int randNum = GlobalVariables.Randomizer.Next(0, m_respawnPoints.Count);
                while (randNum == m_lastRespawnPointUsed)
                {
                    randNum = GlobalVariables.Randomizer.Next(0, m_respawnPoints.Count);
                }
                m_lastRespawnPointUsed = randNum;
                return m_respawnPoints[randNum];
            }
            else
            {
                return Point.Zero;
            }
        }

        //****************************************************
        // Method: AddRespawnPoint
        //
        // Purpose: Adds a spawn point to the spawn point list
        //****************************************************
        static private void AddRespawnPoint(int x, int y)
        {
            m_respawnPoints.Add(new Point(x, y));
        }

        //****************************************************
        // Method: RemoveOutOfBoundsObjects
        //
        // Purpose: Removes all objects (just kills players
        // for now) that are out of bounds
        //****************************************************
        static private void RemoveOutOfBoundsObjects()
        {
            foreach (PlayerObj player in m_playerObjList)
            {
                if (player.HP > 0)
                {
                    if (player.Pos.Y > m_mapBounds.Y || player.Pos.X > m_mapBounds.X || player.PhysicsObj.Rect.Right < -m_mapBounds.X)
                    {
                        player.StatusHandler.Inflict(StatusName.Damage, player.HP, player.ObjectID);
                    }
                }
            }
        }

        #region Find By ID Functions
        //****************************************************
        // Method: FindPlayerObjByPlayerID
        //
        // Purpose: Searches for the player by its player
        // id and returns the player if it was found.
        // Otherwise it returns null.
        //****************************************************
        static public BasicObj FindObjByTypeAndID(UInt16 id, string objType)
        {
            switch (objType)
            {
                case "Player":
                    return FindPlayerObjByID(id);
                case "Hazard":
                    return FindHazardObjByID(id);
                case "Environmental":
                    return FindEnvironmentalObjByID(id);
                case "SpecialEnvironmental":
                    return FindSpecialEnvironmentalObjByID(id);
                case "Item":
                    return FindItemObjByID(id);
                case "Basic":
                    return FindBasicObjByID(id);
                case "World":
                    return null;
                default:
                    return null;
            }
        }

        //****************************************************
        // Method: FindPlayerObjByPlayerID
        //
        // Purpose: Searches for the player by its player
        // id and returns the player if it was found.
        // Otherwise it returns null.
        //****************************************************
        static public PlayerObj FindPlayerObjByPlayerID(long playerID)
        {
            PlayerObj tempObj = null;
            foreach (PlayerObj obj in m_playerObjList)
            {
                if (obj.PlayerID == playerID)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindPlayerObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public PlayerObj FindPlayerObjByID(UInt16 id)
        {
            PlayerObj tempObj = null;
            foreach (PlayerObj obj in m_playerObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindHazardObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public HazardObj FindHazardObjByID(UInt16 id)
        {
            HazardObj tempObj = null;
            foreach (HazardObj obj in m_hazardObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindEnvironmentalObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public EnvironmentalObj FindEnvironmentalObjByID(UInt16 id)
        {
            EnvironmentalObj tempObj = null;
            foreach (EnvironmentalObj obj in m_environmentalObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindSpecialEnvironmentalObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public SpecialEnvironmentalObj FindSpecialEnvironmentalObjByID(UInt16 id)
        {
            SpecialEnvironmentalObj tempObj = null;
            foreach (SpecialEnvironmentalObj obj in m_specialEnvironmentalObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindItemObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public ItemObj FindItemObjByID(UInt16 id)
        {
            ItemObj tempObj = null;
            foreach (ItemObj obj in m_itemObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }

        //****************************************************
        // Method: FindBasicObjByID
        //
        // Purpose: Searches for the object by id and returns
        // it if found. Otherwise it returns null.
        //****************************************************
        static public BasicObj FindBasicObjByID(UInt16 id)
        {
            BasicObj tempObj = null;
            foreach (BasicObj obj in m_basicObjList)
            {
                if (obj.ObjectID == id)
                {
                    tempObj = obj;
                }
            }
            return tempObj;
        }
        #endregion

        #region Add Object Functions
        #region AddObject Functions
        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds a player
        // to the m_playerObjAddList
        //****************************************************
        static public void AddObject(PlayerObj player)
        {
            player.ObjectID = GlobalVariables.UsePlayerObjectIDCounter();
            m_playerObjAddList.AddLast(player);
            NetworkingHandler.SendAddObjMessage(player);
        }

        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds a hazard
        // to the m_hazardObjAddList
        //****************************************************
        static public void AddObject(HazardObj hazard)
        {
            hazard.ObjectID = GlobalVariables.UseHazardObjectIDCounter();
            m_hazardObjAddList.AddLast(hazard);
            NetworkingHandler.SendAddObjMessage(hazard);
        }

        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds
        // an environmental object to the
        // m_environmentalObjAddList
        //****************************************************
        static public void AddObject(EnvironmentalObj environmentalObj)
        {
            environmentalObj.ObjectID = GlobalVariables.UseEnvironmentalObjectIDCounter();
            m_environmentalObjAddList.AddLast(environmentalObj);
            NetworkingHandler.SendAddObjMessage(environmentalObj);
        }

        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds a 
        // special environmental object
        // to the m_specialEnvironmentalObjAddList
        //****************************************************
        static public void AddObject(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            specialEnvironmentalObj.ObjectID = GlobalVariables.UseSpecialEnvironmentalObjectIDCounter();
            m_specialEnvironmentalObjAddList.AddLast(specialEnvironmentalObj);
            NetworkingHandler.SendAddObjMessage(specialEnvironmentalObj);
        }

        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds an item
        // to the m_itemObjAddList
        //****************************************************
        static public void AddObject(ItemObj item)
        {
            item.ObjectID = GlobalVariables.UseItemObjectIDCounter();
            m_itemObjAddList.AddLast(item);
            NetworkingHandler.SendAddObjMessage(item);
        }

        //****************************************************
        // Method: AddObject
        //
        // Purpose: Overload of AddObject which adds an item
        // to the m_basicObjAddList (unlikely to use)
        //****************************************************
        static public void AddObject(BasicObj basicObj)
        {
            basicObj.ObjectID = GlobalVariables.UseBasicObjectIDCounter();
            m_basicObjAddList.AddLast(basicObj);
            //NetworkingHandler.SendAddObjMessage(basicObj);
        }

        #endregion

        #region DirectAddObject Functions
        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds a player
        // to the m_playerObjList
        //**********************************************************
        static public void DirectAddObject(PlayerObj player)
        {
            player.ObjectID = GlobalVariables.UsePlayerObjectIDCounter();
            m_playerObjList.AddLast(player);
        }

        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds a hazard
        // to the m_hazardObjList
        //**********************************************************
        static public void DirectAddObject(HazardObj hazard)
        {
            hazard.ObjectID = GlobalVariables.UseHazardObjectIDCounter();
            m_hazardObjList.AddLast(hazard);
        }

        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds
        // an environmental object to the
        // m_environmentalObjAddList
        //**********************************************************
        static public void DirectAddObject(EnvironmentalObj environmentalObj)
        {
            environmentalObj.ObjectID = GlobalVariables.UseEnvironmentalObjectIDCounter();
            m_environmentalObjList.AddLast(environmentalObj);
        }

        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds a 
        // special environmental object
        // to the m_specialEnvironmentalObjList
        //**********************************************************
        static public void DirectAddObject(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            specialEnvironmentalObj.ObjectID = GlobalVariables.UseSpecialEnvironmentalObjectIDCounter();
            m_specialEnvironmentalObjList.AddLast(specialEnvironmentalObj);
        }

        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds an item
        // to the m_itemObjList
        //**********************************************************
        static public void DirectAddObject(ItemObj item)
        {
            item.ObjectID = GlobalVariables.UseItemObjectIDCounter();
            m_itemObjList.AddLast(item);
        }

        //**********************************************************
        // Method: DirectAddObject
        //
        // Purpose: Overload of DirectAddObject which adds an item
        // to the m_basicObjList (unlikely to use)
        //**********************************************************
        static public void DirectAddObject(BasicObj basicObj)
        {
            basicObj.ObjectID = GlobalVariables.UseBasicObjectIDCounter();
            m_basicObjList.AddLast(basicObj);
        }

        #endregion
        #endregion

        #region Delete Object Functions
        #region DeleteObject Functions
        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds a
        // player to the m_playerObjDeleteList
        //****************************************************
        static public void DeleteObject(PlayerObj player)
        {
            m_playerObjDeleteList.AddLast(player);
            NetworkingHandler.SendDeleteObjMessage(player);
        }

        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds a
        // hazard to the m_hazardObjDeleteList
        //****************************************************
        static public void DeleteObject(HazardObj hazard)
        {
            m_hazardObjDeleteList.AddLast(hazard);
            NetworkingHandler.SendDeleteObjMessage(hazard);
        }

        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds
        // an environmental object to the
        // m_environmentalObjDeleteList
        //****************************************************
        static public void DeleteObject(EnvironmentalObj environmentalObj)
        {
            m_environmentalObjDeleteList.AddLast(environmentalObj);
            NetworkingHandler.SendDeleteObjMessage(environmentalObj);
        }

        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds a 
        // special environmental object
        // to the m_specialEnvironmentalObjDeleteList
        //****************************************************
        static public void DeleteObject(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            m_specialEnvironmentalObjDeleteList.AddLast(specialEnvironmentalObj);
            NetworkingHandler.SendDeleteObjMessage(specialEnvironmentalObj);
        }

        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds an
        // item to the m_itemObjDeleteList
        //****************************************************
        static public void DeleteObject(ItemObj item)
        {
            m_itemObjDeleteList.AddLast(item);
            NetworkingHandler.SendDeleteObjMessage(item);
        }

        //****************************************************
        // Method: DeleteObject
        //
        // Purpose: Overload of DeleteObject which adds an
        // item to the m_basicObjDeleteList (unlikely to use)
        //****************************************************
        static public void DeleteObject(BasicObj basicObj)
        {
            m_basicObjDeleteList.AddLast(basicObj);
        }

        #endregion

        #region DirectDeleteObject Functions
        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the player from the m_playerObjList
        //********************************************************
        static public void DirectDeleteObject(PlayerObj player)
        {
            m_playerObjList.Remove(player);
        }

        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the hazard from the m_hazardObjList
        //********************************************************
        static public void DirectDeleteObject(HazardObj hazard)
        {
            m_hazardObjList.Remove(hazard);
        }

        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the environmentalObj from
        // the m_environmentalObjList
        //********************************************************
        static public void DirectDeleteObject(EnvironmentalObj environmentalObj)
        {
            m_environmentalObjList.Remove(environmentalObj);
        }

        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the specialEnvironmentalObj from
        // the m_specialEnvironmentalObjList
        //********************************************************
        static public void DirectDeleteObject(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            m_specialEnvironmentalObjList.Remove(specialEnvironmentalObj);
        }

        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the item from the m_itemObjList
        //********************************************************
        static public void DirectDeleteObject(ItemObj item)
        {
            m_itemObjList.Remove(item);
        }

        //********************************************************
        // Method: DirectDeleteObject
        //
        // Purpose: Overload of DirectDeleteObject which directly
        // deletes the basicObj from the m_basicObjList
        // (Unlikely to use)
        //********************************************************
        static public void DirectDeleteObject(BasicObj basicObj)
        {
            m_basicObjList.Remove(basicObj);
        }

        #endregion
        #endregion

        #region Collision Functions
        //****************************************************
        // Method: CollideCheck
        //
        // Purpose: Checks for collision.
        //****************************************************
        static public bool CollideCheck(PhysicsObj obj1, PhysicsObj obj2, ref Vector2 overLap)
        {
            //Checks for collision using N Tutorial A (http://www.metanetsoftware.com/technique/tutorialA.html)
            //Finds how much the rectangles are overlapping, if they are, in the x axis
            overLap.X = obj1.Rect.Width / 2 + obj2.Rect.Width / 2 - Math.Abs(obj2.Rect.Center.X - obj1.Rect.Center.X);
            if (overLap.X >= 0) //if the rects do overlap in the x axis then there's more checking to be done
            {
                //Finds how much the rectangles are overlapping, if they are, in the y axis
                overLap.Y = obj1.Rect.Height / 2 + obj2.Rect.Height / 2 - Math.Abs(obj2.Rect.Center.Y - obj1.Rect.Center.Y);
                if (overLap.Y >= 0)//if the rects overlap in the x and y axis then there is a collision
                {
                    return true;
                }
            }
            return false;
        }

        //****************************************************
        // Method: Collide
        //
        // Purpose: Handles collision between objects.
        //****************************************************
        static public void MomentumTransfer(PhysicsObj obj1, PhysicsObj obj2, Vector2 overLap, GameTime gameTime)
        {
            //REQUIRES WORK
            #region Declarations and Initializations
            PhysicsObj tempObj1 = obj1.DeepClone();
            PhysicsObj tempObj2 = obj2.DeepClone();

            Vector2 obj1OverLap = new Vector2(overLap.X, overLap.Y);
            Vector2 obj2OverLap = new Vector2(overLap.X, overLap.Y);

            Side collisionSide = Side.top; //Used to store the side the collision occured on
            Axis collisionAxis = Axis.x; //Used to store the axis upon which the collision occured
            #endregion

            #region Obtain Total Friction
            //Obtains the total friction between objects
            //1 removes all speed from the object
            //0 doesn't affect the object
            float totalFriction = tempObj1.Friction + tempObj2.Friction;
            if (totalFriction > 1)
            {
                totalFriction = 1F;
            }
            if (tempObj1.Friction * tempObj2.Friction == 0)
            {
                totalFriction = 0F;
            }
            #endregion

            #region Obtain Appliable Over Lap For Object One
            //*********************************************************************
            // overLapAppliable is Used to apply only the amount of the overLap
            // that the object contributed.
            //*********************************************************************
            Vector2 obj1OverLapAppliable = new Vector2(Math.Abs(tempObj1.Vel.X) / (Math.Abs(tempObj1.Vel.X) + Math.Abs(tempObj2.Vel.X)), Math.Abs(tempObj1.Vel.Y) / (Math.Abs(tempObj1.Vel.Y) + Math.Abs(tempObj2.Vel.Y)));
            //Vector2 obj2OverLapAppliable = new Vector2(Math.Abs(tempObj2.Vel.X) / (Math.Abs(tempObj2.Vel.X) + Math.Abs(tempObj1.Vel.X)), Math.Abs(tempObj2.Vel.Y) / (Math.Abs(tempObj2.Vel.Y) + Math.Abs(tempObj1.Vel.Y)));
            #endregion

            #region Determine Side of Collision
            //*******************************************************
            //Used to determine which side the collision occured on
            //*******************************************************
            if (tempObj1.Rect.Center != tempObj2.Rect.Center) //Makes sure the objects don't overlap on their center
            {
                #region "Correct" Collision Side Detection
                if (overLap.X < overLap.Y) //The collision occured on the x axis
                {
                    collisionAxis = Axis.x; //Sets the axis of collision to x

                    if (tempObj1.Rect.Center.X < tempObj2.Rect.Center.X) //obj1 hit the left side of obj2
                    {
                        collisionSide = Side.left;
                    }
                    else //obj1 hit the right side of obj2
                    {
                        collisionSide = Side.right;
                    }

                }
                else //collision occured on y axis
                {
                    collisionAxis = Axis.y; //Sets the axis of collision to y

                    if (tempObj1.Rect.Center.Y < tempObj2.Rect.Center.Y) //obj1 landed on top of obj2
                    {
                        collisionSide = Side.top;
                    }

                    else //obj1 hit the bottom of obj2
                    {
                        collisionSide = Side.bottom;
                    }
                }
                #endregion
            }

            else //If the objects have the same exact center position something different needs to be done
            {
                #region Cludge Collision Side Detection
                //As long as the object's previous position isn't the same as its current position
                //this should work
                if ((int)tempObj1.PrevPos.X != (int)tempObj2.PrevPos.X && (int)tempObj1.PrevPos.Y != (int)tempObj2.PrevPos.Y)
                {
                    if (obj1.ApplyMomentum)
                    {
                        obj1.Pos = obj1.PrevPos;
                    }

                    if (obj2.ApplyMomentum)
                    {
                        obj2.Pos = obj2.PrevPos;
                    }
                }
                //Moves the object in a "random" direction by 1
                //Useful if objects are spawned directly on each other
                else
                {
                    switch (GlobalVariables.Randomizer.Next(0, 4))
                    {
                        case 0:
                            collisionAxis = Axis.y;
                            collisionSide = Side.top;
                            break;
                        case 1:
                            collisionSide = Side.bottom;
                            collisionAxis = Axis.y;
                            break;
                        case 2:
                            collisionSide = Side.right;
                            collisionAxis = Axis.x;
                            break;
                        case 3:
                            collisionSide = Side.left;
                            collisionAxis = Axis.x;
                            break;
                    }
                }
                #endregion

            }
            #endregion

            #region Apply Collisions
            //******************
            // Apply collisions
            //******************
            #region X Axis Collision
            if (collisionAxis == Axis.x)
            {
                #region Apply Overlap
                //************************************************************************************************
                //Applies overLapAppliable to the overLap for the x axis if and only if the calculation succeeded
                //otherwise the overLap is applied in full because of the fact that, for the overLapAppliable
                //to fail its equation, both objects need a 0 speed. Thus, the descrepency in movement would
                //either not exist or be so miniscule that it does not matter.
                //************************************************************************************************
                if (!obj1OverLapAppliable.X.Equals(float.NaN))
                {
                    obj1OverLap.X *= obj1OverLapAppliable.X;
                    obj2OverLap.X *= 1 - obj1OverLapAppliable.X;
                }

                if (collisionSide == Side.left)
                {
                    if (obj1.ApplyMomentum)
                    {
                        obj1.PosX -= obj1OverLap.X; //Moves the object out of the other object by the correct amount
                    }
                    obj1.Sided[Side.right] = true;

                    if (obj2.ApplyMomentum)
                    {
                        obj2.PosX += obj2OverLap.X; //Moves the object out of the other object by the correct amount
                    }
                    obj2.Sided[Side.left] = true;
                }
                else
                {
                    if (obj1.ApplyMomentum)
                    {
                        obj1.PosX += obj1OverLap.X; //Moves the object out of the other object by the correct amount
                    }
                    obj1.Sided[Side.left] = true;

                    if (obj2.ApplyMomentum)
                    {
                        obj2.PosX -= obj2OverLap.X; //Moves the object out of the other object by the correct amount
                    }
                    obj2.Sided[Side.right] = true;
                }
                #endregion

                #region Set Previous X Velocity
                obj1.PrevVelX = obj1.Vel.X; //Stores the velocity of the object before the collision occurs
                obj2.PrevVelX = obj2.Vel.X; //Stores the velocity of the object before the collision occurs
                #endregion

                #region Conserve Momentum
                //Everything in here should on occur during an overlap
                //(somethings should occur when there's no overlap but the objects are next to each other"
                if (overLap.X > 0 && !(collisionSide == Side.left && tempObj1.Vel.X < 0 && tempObj2.Vel.X > tempObj1.Vel.X) && !(collisionSide == Side.right && tempObj1.Vel.X > 0 && tempObj2.Vel.X < tempObj1.Vel.X) && !(collisionSide == Side.left && tempObj2.Vel.X > 0 && tempObj2.Vel.X > tempObj1.Vel.X) && !(collisionSide == Side.right && tempObj2.Vel.X < 0 && tempObj2.Vel.X < tempObj1.Vel.X))
                {
                    //Applies conservation of momentum
                    if (obj1.ApplyMomentum)
                    {
                        obj1.VelX = (((((obj1.Mass - tempObj2.Mass) / (obj1.Mass + tempObj2.Mass)) * obj1.Vel.X) + (((2 * tempObj2.Mass) / (obj1.Mass + tempObj2.Mass)) * tempObj2.Vel.X)));

                        obj1.VelX *= obj1.Restitution + tempObj2.Restitution; //Applies bounciness
                    }

                    if (obj2.ApplyMomentum)
                    {
                        obj2.VelX = (((((obj2.Mass - tempObj1.Mass) / (obj2.Mass + tempObj1.Mass)) * obj2.Vel.X) + (((2 * tempObj1.Mass) / (obj2.Mass + tempObj1.Mass)) * tempObj1.Vel.X)));

                        obj2.VelX *= obj2.Restitution + tempObj1.Restitution; //Applies bounciness
                    }
                    //speed.X = ((speed.X * (mass - obj.Mass) + 2 * (obj.Mass * obj.Speed.X)) / (mass + obj.Mass));

                    //speed.X += (speed.X - prevSpeed.X) * (bounciness + obj.Bounciness); //Applies equal and opposite impulse force
                }
                #endregion

                #region Apply Y Axis Friction
                //Applies friction to the Y axis
                obj1.ContactFrictionY += totalFriction;
                obj2.ContactFrictionY += totalFriction;
                #endregion
            }
            #endregion

            #region Y Axis Collision
            else
            {
                #region Apply Overlap
                //************************************************************************************************
                //Applies overLapAppliable to the overLap for the y axis if and only if the calculation succeeded
                //************************************************************************************************
                if (!obj1OverLapAppliable.Y.Equals(float.NaN))
                {
                    obj1OverLap.Y *= obj1OverLapAppliable.Y;
                    obj2OverLap.Y *= 1 - obj1OverLapAppliable.Y;
                }

                if (collisionSide == Side.top)
                {
                    if (obj1.ApplyMomentum)
                    {
                        obj1.PosY -= obj1OverLap.Y;//Moves the object out of the other object by the correct amount
                    }
                    obj1.Sided[Side.bottom] = true; //Sets the flag stating that the object is on a surface to true

                    if (obj2.ApplyMomentum)
                    {
                        obj2.PosY += obj2OverLap.Y;//Moves the object out of the other object by the correct amount
                    }
                    obj2.Sided[Side.top] = true; //Sets the flag stating that the object is on a surface to true
                }
                else
                {
                    if (obj1.ApplyMomentum)
                    {
                        obj1.PosY += obj1OverLap.Y;//Moves the object out of the other object by the correct amount
                    }
                    obj1.Sided[Side.top] = true; //Sets the flag stating that the object is on a surface to true

                    if (obj2.ApplyMomentum)
                    {
                        obj2.PosY -= obj2OverLap.Y;//Moves the object out of the other object by the correct amount
                    }
                    obj2.Sided[Side.bottom] = true; //Sets the flag stating that the object is on a surface to true
                }
                #endregion

                #region Set Previous Y Velocity
                obj1.PrevVelY = obj1.Vel.Y; //Stores the velocity of the object before the collision occurs
                obj2.PrevVelY = obj2.Vel.Y; //Stores the velocity of the object before the collision occurs
                #endregion

                #region Conserve Momentum
                //Everything in here should on occur during an overlap
                //(somethings should occur when there's no overlap but the objects are next to each other"
                if (overLap.Y > 0 && !(collisionSide == Side.top && tempObj1.Vel.Y < 0 && tempObj2.Vel.Y > tempObj1.Vel.Y) && !(collisionSide == Side.bottom && tempObj1.Vel.Y > 0 && tempObj2.Vel.Y < tempObj1.Vel.Y) && !(collisionSide == Side.top && tempObj2.Vel.Y > 0 && tempObj2.Vel.Y > tempObj1.Vel.Y) && !(collisionSide == Side.bottom && tempObj2.Vel.Y < 0 && tempObj2.Vel.Y < tempObj1.Vel.Y))
                {
                    //Apply conservation of momentum
                    if (obj1.ApplyMomentum)
                    {
                        obj1.VelY = (((obj1.Mass - tempObj2.Mass) / (obj1.Mass + tempObj2.Mass)) * obj1.Vel.Y) + (((2 * tempObj2.Mass) / (obj1.Mass + tempObj2.Mass)) * tempObj2.Vel.Y);

                        obj1.VelY *= obj1.Restitution + tempObj2.Restitution; //Applies bounciness
                    }

                    if (obj2.ApplyMomentum)
                    {
                        obj2.VelY = (((obj2.Mass - tempObj1.Mass) / (obj2.Mass + tempObj1.Mass)) * obj2.Vel.Y) + (((2 * tempObj1.Mass) / (obj2.Mass + tempObj1.Mass)) * tempObj1.Vel.Y);

                        obj2.VelY *= obj2.Restitution + tempObj1.Restitution; //Applies bounciness
                    }
                }
                #endregion

                #region Apply X Axis Friction
                //Applies friction to the X axis
                obj1.ContactFrictionX += totalFriction;
                obj2.ContactFrictionX += totalFriction;
                #endregion
            }
            #endregion

            #endregion
        }

        #endregion

        #region Properties
        static public LinkedList<PlayerObj> PlayerObjList
        {
            get
            {
                return m_playerObjList;
            }
        }

        static public LinkedList<HazardObj> HazardObjList
        {
            get
            {
                return m_hazardObjList;
            }
        }

        static public LinkedList<EnvironmentalObj> EnvironmentalObjList
        {
            get
            {
                return m_environmentalObjList;
            }
        }

        static public LinkedList<SpecialEnvironmentalObj> SpecialEnvironmentalObjList
        {
            get
            {
                return m_specialEnvironmentalObjList;
            }
        }

        static public LinkedList<ItemObj> ItemObjList
        {
            get
            {
                return m_itemObjList;
            }
        }

        static public LinkedList<BasicObj> BasicObjList
        {
            get
            {
                return m_basicObjList;
            }
        }

        static public Camera2D TrackingCamera
        {
            get
            {
                return m_trackingCamera;
            }
        }

        #endregion
    }
}
