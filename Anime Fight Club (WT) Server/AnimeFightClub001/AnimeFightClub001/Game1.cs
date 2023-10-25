//******************************************************
// File: Game1.cs
//
// Purpose: Contains the definition of the Game1 class.
// This class is simply a main class. It will hold
// the main structure of the game and will call
// all necessary functions and initialize everything.
//
// or something like that.... lol
//
// Written By: Salvatore Hanusiewicz
//******************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AnimeFightClub001
{
    public enum GameState { InitializeGame, Game, InitalizeBetweenMatches, BetweenMatches};
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public GameState currGameState;
        int betweenMatchesTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //currGameState = GameState.SplashScreen;
            currGameState = GameState.InitializeGame;
            this.Window.Title = "AnimeFightClub-Server";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalVariables.ImageDict.Add("Default", Content.Load<Texture2D>(@"Default"));
            GlobalVariables.ImageDict.Add("TestPlatform", Content.Load<Texture2D>(@"TestPlatform"));
            GlobalVariables.ImageDict.Add("TestPlatform2", Content.Load<Texture2D>(@"TestPlatform2"));
            GlobalVariables.ImageDict.Add("RunSheet1", Content.Load<Texture2D>(@"runningsheet"));
            GlobalVariables.ImageDict.Add("ForcePush", Content.Load<Texture2D>(@"ForcePush"));

            #region Add Fonts
            GlobalVariables.FontDict.Add("Arial20", Content.Load<SpriteFont>(@"Arial20"));
            GlobalVariables.FontDict.Add("Arial18", Content.Load<SpriteFont>(@"Arial18"));
            GlobalVariables.FontDict.Add("Arial16", Content.Load<SpriteFont>(@"Arial16"));
            GlobalVariables.FontDict.Add("Arial12", Content.Load<SpriteFont>(@"Arial12"));
            GlobalVariables.FontDict.Add("Arial10", Content.Load<SpriteFont>(@"Arial10"));
            GlobalVariables.FontDict.Add("Anvil20", Content.Load<SpriteFont>(@"Anvil20"));
            GlobalVariables.FontDict.Add("Anvil40", Content.Load<SpriteFont>(@"Anvil40"));
            GlobalVariables.FontDict.Add("Penshurst20", Content.Load<SpriteFont>(@"Penshurst20"));
            GlobalVariables.FontDict.Add("Penshurst40", Content.Load<SpriteFont>(@"Penshurst40"));

            //GlobalVariables.FontDict.Add("CommonBullets30", Content.Load<SpriteFont>(@"CommonBullets30"));
            #endregion
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            NetworkingHandler.MasterServerDisconnect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();
            switch (currGameState)
            {

                #region Game
                case GameState.InitializeGame:
                    NetworkingHandler.StartServer();
                    GameObjectHandler.LoadMap("One");

                    currGameState = GameState.Game;
                    break;
                case GameState.Game:
                    NetworkingHandler.HandleMessages(this);
                    GameObjectHandler.Update(gameTime, graphics.GraphicsDevice.Viewport);
                    GameObjectHandler.TrackingCamera.Update();

                    NetworkingHandler.SendAccelUpdateToAll(gameTime);
                    NetworkingHandler.SendVelUpdateToAll(gameTime);
                    NetworkingHandler.SendPosUpdateToAll(gameTime);

                    GlobalVariables.IncrementFrame();

                    GameObjectHandler.CheckForWinCondition(this);
                    break;
                #endregion

                #region Between Matches
                case GameState.InitalizeBetweenMatches:
                    betweenMatchesTimer = 0;

                    NetworkingHandler.SendMatchEndToPlayers();
                    NetworkingHandler.SendMatchEndToMasterServer();
                    GameObjectHandler.BetweenMatchReset();

                    currGameState = GameState.BetweenMatches;
                    break;
                case GameState.BetweenMatches:
                    NetworkingHandler.HandleMessages(this);
                    betweenMatchesTimer += gameTime.ElapsedGameTime.Milliseconds;

                    if (betweenMatchesTimer >= 10000)
                    {
                        GameObjectHandler.NextMap();
                        NetworkingHandler.NextMap();

                        //Technically it should be InitalizeGame but that also starts the server right now so blah
                        currGameState = GameState.Game;
                    }
                    break;
                #endregion

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (currGameState)
            {

                #region Game
                case GameState.InitializeGame:
                    break;
                case GameState.Game:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, GameObjectHandler.TrackingCamera.GetTransformation(graphics.GraphicsDevice));
                    //spriteBatch.Begin();
                    GameObjectHandler.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                #endregion

                #region Between Matches
                case GameState.InitalizeBetweenMatches:
                    break;
                case GameState.BetweenMatches:
                    break;
                #endregion
            }

            base.Draw(gameTime);
        }
    }
}
