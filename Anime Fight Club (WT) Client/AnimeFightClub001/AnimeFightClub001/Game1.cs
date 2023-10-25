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
using EventInput;

namespace AnimeFightClub001
{
    public enum GameState { InitializeSplashScreen, SplashScreen, InitializeLoginMenu, LoginMenu, InitializeRegistrationMenu, RegistrationMenu, InitializeMainMenu, MainMenu, InitializeCustomizationMenu, CustomizationMenu, InitializeServerListMenu, ServerListMenu, InitializeGame, Game, InitalizeBetweenMatches, BetweenMatches };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public GameState currGameState;
        Dictionary<string, Menu> menus;
        HUD hud;

        Controller playerOneControllerGamepad;
        Controller playerOneControllerKeyboard;
        Controller playerOneController;

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
            currGameState = GameState.InitializeSplashScreen;

            this.Window.Title = "AnimeFightClub-Client";

            EventInput.EventInput.Initialize(this.Window);
            #region Full Screen Initialization
            //Full Screen Initialization
            //graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            //graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            //graphics.IsFullScreen = true;
            //graphics.ApplyChanges();
            #endregion

            menus = new Dictionary<string, Menu>();

            playerOneControllerGamepad = new Controller(PlayerIndex.One, ControllerType.Gamepad);
            playerOneControllerKeyboard = new Controller(PlayerIndex.One, ControllerType.Keyboard);

            playerOneController = new Controller(PlayerIndex.One, ControllerType.Gamepad); //Doesn't matter, it merges everything
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            List<Point[]> tempLoopList = new List<Point[]>(); //Used as the temporary list of animation loops for different states
            List<int> tempFrameDurationList = new List<int>(); //Used as the temporary list of frame durations for different states

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Add Sounds
            GlobalVariables.SoundDict.Add("Male Grunt", Content.Load<SoundEffect>(@"Sounds/Male Grunt"));
            GlobalVariables.SoundDict.Add("Female Grunt", Content.Load<SoundEffect>(@"Sounds/Female Grunt"));
            GlobalVariables.SoundDict.Add("Hit Marker", Content.Load<SoundEffect>(@"Sounds/BO HitMarker"));
            GlobalVariables.SoundDict.Add("Sword Swing", Content.Load<SoundEffect>(@"Sounds/Sword Swing"));
            GlobalVariables.SoundDict.Add("Thunder", Content.Load<SoundEffect>(@"Sounds/Thunder"));
            GlobalVariables.SoundDict.Add("Snikt", Content.Load<SoundEffect>(@"Sounds/Snikt"));
            GlobalVariables.SoundDict.Add("Gunshot", Content.Load<SoundEffect>(@"Sounds/Gunshot"));
            GlobalVariables.SoundDict.Add("LongReload", Content.Load<SoundEffect>(@"Sounds/LongReload"));
            GlobalVariables.SoundDict.Add("Throw", Content.Load<SoundEffect>(@"Sounds/Throw"));
            GlobalVariables.SoundDict.Add("EnergyBlade", Content.Load<SoundEffect>(@"Sounds/EnergyBlade"));
            GlobalVariables.SoundDict.Add("FireBall", Content.Load<SoundEffect>(@"Sounds/FireBall"));
            GlobalVariables.SoundDict.Add("FireThrow", Content.Load<SoundEffect>(@"Sounds/FireThrow"));
            GlobalVariables.SoundDict.Add("EpicSound", Content.Load<SoundEffect>(@"Sounds/EpicSound"));
            GlobalVariables.SoundDict.Add("Fus Ro Dah", Content.Load<SoundEffect>(@"Sounds/Fus Ro Dah"));

            GlobalVariables.SoundDict.Add("Explosion Sound 001", Content.Load<SoundEffect>(@"Sounds/Explosion Sound 001"));
            GlobalVariables.SoundDict.Add("FuseBurning", Content.Load<SoundEffect>(@"Sounds/FuseBurning"));
            #endregion

            #region Add Music
            GlobalVariables.SongList.Add(Content.Load<SoundEffect>(@"Music/Danse of Questionable Tuning"));
            GlobalVariables.SongList.Add(Content.Load<SoundEffect>(@"Music/One-eyed Maestro"));
            GlobalVariables.SongList.Add(Content.Load<SoundEffect>(@"Music/Presenterator"));
            #endregion

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

            #region Add Images

            #region GUI Assets
            GlobalVariables.ImageDict.Add("BasicBackground001", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background")));

            GlobalVariables.ImageDict.Add("Title", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/title")));

            #region Load Icons
            GlobalVariables.ImageDict.Add("Caltrop_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Caltrop")));
            GlobalVariables.ImageDict.Add("Fireball_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Fireball")));

            GlobalVariables.ImageDict.Add("Sword_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Sword_Icon")));
            GlobalVariables.ImageDict.Add("Magic Sword_Icon", new ImageInfo(Content.Load<Texture2D>(@"Magic Sword_Icon")));
            GlobalVariables.ImageDict.Add("Shield_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Shield")));
            GlobalVariables.ImageDict.Add("Bomb Bag_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/BombBag")));
            GlobalVariables.ImageDict.Add("Pistol_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Pistol_icon")));

            GlobalVariables.ImageDict.Add("Lightning Bolt_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Lightning Bolt_Icon")));
            GlobalVariables.ImageDict.Add("Javelin_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Javelin_Icon")));

            GlobalVariables.ImageDict.Add("One Hit Shield_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Shield")));
            GlobalVariables.ImageDict.Add("Shrink_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/peter1scaled")));
            GlobalVariables.ImageDict.Add("Grow_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/peter1scaled")));

            GlobalVariables.ImageDict.Add("Energy Blade_Icon", new ImageInfo(Content.Load<Texture2D>(@"ForcePush")));
            GlobalVariables.ImageDict.Add("Force Push_Icon", new ImageInfo(Content.Load<Texture2D>(@"ForcePush")));

            GlobalVariables.ImageDict.Add("Spike Trap_Icon", new ImageInfo(Content.Load<Texture2D>(@"SpikeTrap_Icon")));
            GlobalVariables.ImageDict.Add("Poison Spike Trap_Icon", new ImageInfo(Content.Load<Texture2D>(@"PoisonSpikeTrap_Icon")));


            GlobalVariables.ImageDict.Add("None_Icon", new ImageInfo(Content.Load<Texture2D>(@"None")));
            #endregion

            #region Load Outline001 Assets
            GlobalVariables.ImageDict.Add("Outline001_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_L")));
            GlobalVariables.ImageDict.Add("Outline001_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_R")));
            GlobalVariables.ImageDict.Add("Outline001_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_T")));
            GlobalVariables.ImageDict.Add("Outline001_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_B")));
            GlobalVariables.ImageDict.Add("Outline001_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_TL")));
            GlobalVariables.ImageDict.Add("Outline001_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_TR")));
            GlobalVariables.ImageDict.Add("Outline001_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_BL")));
            GlobalVariables.ImageDict.Add("Outline001_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/focusBorder_BR")));
            #endregion

            #region Load Border001 Assets
            GlobalVariables.ImageDict.Add("Border001_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_L")));
            GlobalVariables.ImageDict.Add("Border001_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_R")));
            GlobalVariables.ImageDict.Add("Border001_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_T")));
            GlobalVariables.ImageDict.Add("Border001_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_B")));
            GlobalVariables.ImageDict.Add("Border001_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_TL")));
            GlobalVariables.ImageDict.Add("Border001_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_TR")));
            GlobalVariables.ImageDict.Add("Border001_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_BL")));
            GlobalVariables.ImageDict.Add("Border001_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_BR")));

            GlobalVariables.ImageDict.Add("Border001_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_L")));
            GlobalVariables.ImageDict.Add("Border001_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_R")));
            GlobalVariables.ImageDict.Add("Border001_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_T")));
            GlobalVariables.ImageDict.Add("Border001_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_B")));
            GlobalVariables.ImageDict.Add("Border001_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_TL")));
            GlobalVariables.ImageDict.Add("Border001_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_TR")));
            GlobalVariables.ImageDict.Add("Border001_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_BL")));
            GlobalVariables.ImageDict.Add("Border001_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/border001_BR")));
            #endregion

            #region Load IconFrame001 Assets
            GlobalVariables.ImageDict.Add("IconFrame001", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_Body")));

            GlobalVariables.ImageDict.Add("IconFrame001_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_L")));
            GlobalVariables.ImageDict.Add("IconFrame001_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_R")));
            GlobalVariables.ImageDict.Add("IconFrame001_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_T")));
            GlobalVariables.ImageDict.Add("IconFrame001_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_B")));
            GlobalVariables.ImageDict.Add("IconFrame001_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_TL")));
            GlobalVariables.ImageDict.Add("IconFrame001_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_TR")));
            GlobalVariables.ImageDict.Add("IconFrame001_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_BL")));
            GlobalVariables.ImageDict.Add("IconFrame001_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_BR")));

            GlobalVariables.ImageDict.Add("IconFrame001_Focus", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_Body")));

            GlobalVariables.ImageDict.Add("IconFrame001_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_L")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_R")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_T")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_B")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_TL")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_TR")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_BL")));
            GlobalVariables.ImageDict.Add("IconFrame001_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/IconFrame001_BR")));
            #endregion

            #region Load TextBox Assets
            GlobalVariables.ImageDict.Add("TextBox", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_Body")));
            GlobalVariables.ImageDict.Add("TextBox_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_L")));
            GlobalVariables.ImageDict.Add("TextBox_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_R")));
            GlobalVariables.ImageDict.Add("TextBox_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_T")));
            GlobalVariables.ImageDict.Add("TextBox_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_B")));
            GlobalVariables.ImageDict.Add("TextBox_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_TL")));
            GlobalVariables.ImageDict.Add("TextBox_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_TR")));
            GlobalVariables.ImageDict.Add("TextBox_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_BL")));
            GlobalVariables.ImageDict.Add("TextBox_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_BR")));

            GlobalVariables.ImageDict.Add("TextBox_Focus", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_Body")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_L")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_R")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_T")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_B")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_TL")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_TR")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_BL")));
            GlobalVariables.ImageDict.Add("TextBox_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/textbox_BR")));
            #endregion

            #region Load Background001 Assets
            GlobalVariables.ImageDict.Add("Background001", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_Body")));
            GlobalVariables.ImageDict.Add("Background001_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_L")));
            GlobalVariables.ImageDict.Add("Background001_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_R")));
            GlobalVariables.ImageDict.Add("Background001_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_T")));
            GlobalVariables.ImageDict.Add("Background001_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_B")));
            GlobalVariables.ImageDict.Add("Background001_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_TL")));
            GlobalVariables.ImageDict.Add("Background001_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_TR")));
            GlobalVariables.ImageDict.Add("Background001_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_BL")));
            GlobalVariables.ImageDict.Add("Background001_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_BR")));

            GlobalVariables.ImageDict.Add("Background001_Focus", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_Body")));
            GlobalVariables.ImageDict.Add("Background001_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_L")));
            GlobalVariables.ImageDict.Add("Background001_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_R")));
            GlobalVariables.ImageDict.Add("Background001_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_T")));
            GlobalVariables.ImageDict.Add("Background001_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_B")));
            GlobalVariables.ImageDict.Add("Background001_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_TL")));
            GlobalVariables.ImageDict.Add("Background001_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_TR")));
            GlobalVariables.ImageDict.Add("Background001_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_BL")));
            GlobalVariables.ImageDict.Add("Background001_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/background001_BR")));
            #endregion

            #region Load RoundButton Assets
            GlobalVariables.ImageDict.Add("RoundButton", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_Body")));
            GlobalVariables.ImageDict.Add("RoundButton_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_L")));
            GlobalVariables.ImageDict.Add("RoundButton_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_R")));
            GlobalVariables.ImageDict.Add("RoundButton_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_T")));
            GlobalVariables.ImageDict.Add("RoundButton_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_B")));
            GlobalVariables.ImageDict.Add("RoundButton_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_TL")));
            GlobalVariables.ImageDict.Add("RoundButton_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_TR")));
            GlobalVariables.ImageDict.Add("RoundButton_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_BL")));
            GlobalVariables.ImageDict.Add("RoundButton_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_BR")));

            GlobalVariables.ImageDict.Add("RoundButton_Focus", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_Body")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_L")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_R")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_T")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_B")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_TL")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_TR")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_BL")));
            GlobalVariables.ImageDict.Add("RoundButton_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/button_BR")));
            #endregion

            #region None

            GlobalVariables.ImageDict.Add("None_L", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_R", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_T", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_B", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_TL", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_TR", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_BL", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_BR", new ImageInfo(Content.Load<Texture2D>(@"None")));

            GlobalVariables.ImageDict.Add("None_Focus_L", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_R", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_T", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_B", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_TL", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_TR", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_BL", new ImageInfo(Content.Load<Texture2D>(@"None")));
            GlobalVariables.ImageDict.Add("None_Focus_BR", new ImageInfo(Content.Load<Texture2D>(@"None")));
            #endregion

            #endregion

            #region Misc

            GlobalVariables.ImageDict.Add("None", new ImageInfo(Content.Load<Texture2D>(@"None"), Point.Zero, 0));
            GlobalVariables.ImageDict.Add("", new ImageInfo(Content.Load<Texture2D>(@"None"), Point.Zero, 0));

            GlobalVariables.ImageDict.Add("None_Focus", new ImageInfo(Content.Load<Texture2D>(@"None"), Point.Zero, 0));

            GlobalVariables.ImageDict.Add("Default", new ImageInfo(Content.Load<Texture2D>(@"Default"), new Point(10, 10), 0));
            #endregion

            #region Status Assets
            GlobalVariables.ImageDict.Add("StatusStun", new ImageInfo(Content.Load<Texture2D>(@"StunSpiral"), new Point(25, 25), 100));
            GlobalVariables.ImageDict.Add("DazedBubble", new ImageInfo(Content.Load<Texture2D>(@"DazedBubble"), new Point(100, 110), 100));
            GlobalVariables.ImageDict.Add("PoisonBubble", new ImageInfo(Content.Load<Texture2D>(@"PoisonBubble"), new Point(100, 110), 100));

            GlobalVariables.ImageDict.Add("Shield", new ImageInfo(Content.Load<Texture2D>(@"Shield"), new Point(60, 130), 100));
            #endregion

            #region Ability Assets

            #region AbilityLightningBolt
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Lightning Bolt Stuff
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Holstered
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Pulling Back
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(7, 0) }); //Hold Back
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(8, 0), new Point(8, 0) }); //Hold Back Charged
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(1, 0) }); //Release
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Hold Out
            tempFrameDurationList.Add(120);

            #endregion

            GlobalVariables.ImageDict.Add("AbilityLightningBolt", new ImageInfo(Content.Load<Texture2D>(@"AbilityLightningBolt"), new Point(100, 110), tempLoopList, tempFrameDurationList));
            #endregion

            #region AbilitySword
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Slash/Stab Stuff
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Holstered
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(0, 1), new Point(2, 1) }); //Slashing
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(1, 0) }); //Slashing Charge
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(1, 0) }); //Slashing Hold Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 1), new Point(1, 1) }); //Slashing Hold Up Charged
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(4, 0) }); //Slashing Hold Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(4, 0) }); //Slashing Release
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(1, 1), new Point(4, 1) }); //Slashing Release Charged
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing Charge
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(0, 6) }); //Stabbing Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(2, 6) }); //Stabbing Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(0, 6) }); //Stabbing Release
            tempFrameDurationList.Add(120);
            #endregion

            GlobalVariables.ImageDict.Add("AbilitySword", new ImageInfo(Content.Load<Texture2D>(@"AbilitySword"), new Point(180, 180), tempLoopList, tempFrameDurationList));
            #endregion

            #region AbilityMagicSword
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Slash/Stab Stuff
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Holstered
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(0, 1), new Point(2, 1) }); //Slashing
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(1, 0) }); //Slashing Charge
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(1, 0) }); //Slashing Hold Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 1), new Point(1, 1) }); //Slashing Hold Up Charged
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(4, 0) }); //Slashing Hold Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(4, 0) }); //Slashing Release
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(1, 1), new Point(4, 1) }); //Slashing Release Charged
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing Charge
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(0, 6) }); //Stabbing Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(2, 6) }); //Stabbing Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(0, 6) }); //Stabbing Release
            tempFrameDurationList.Add(120);
            #endregion

            GlobalVariables.ImageDict.Add("AbilityMagicSword", new ImageInfo(Content.Load<Texture2D>(@"MagicSword"), new Point(180, 180), tempLoopList, tempFrameDurationList));
            #endregion

            #region AbilityPistol
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Shoot Stuff
            tempLoopList.Add(new Point[2] { new Point(-1, 0), new Point(-1, 0) }); //Holstered
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(2, 0) }); //Shooting Forward
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(1, 0) }); //Shooting Forward Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 0), new Point(2, 0) }); //Shooting Forward Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Shooting Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(6, 0), new Point(6, 0) }); //Shooting Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(5, 0) }); //Shooting Forward Up
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(4, 0), new Point(4, 0) }); //Shooting Forward Up Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 0), new Point(5, 0) }); //Shooting Forward Up Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(6, 0), new Point(6, 0) }); //Shooting Forward Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(3, 0) }); //Shooting Forward Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(3, 0) }); //Shooting Forward Running
            tempFrameDurationList.Add(80);
            #endregion

            GlobalVariables.ImageDict.Add("AbilityPistol", new ImageInfo(Content.Load<Texture2D>(@"Pistol"), new Point(110, 140), tempLoopList, tempFrameDurationList));
            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion
            
            #endregion

            #region Environmental Assets
            GlobalVariables.ImageDict.Add("TestPlatform", new ImageInfo(Content.Load<Texture2D>(@"TestPlatform")));
            GlobalVariables.ImageDict.Add("TestPlatform2", new ImageInfo(Content.Load<Texture2D>(@"TestPlatform2")));

            #region Bars And Such
            GlobalVariables.ImageDict.Add("LongHorizontalBar", new ImageInfo(Content.Load<Texture2D>(@"LongHorizontalBar")));
            GlobalVariables.ImageDict.Add("MediumHorizontalBar", new ImageInfo(Content.Load<Texture2D>(@"MediumHorizontalBar")));
            GlobalVariables.ImageDict.Add("ShortHorizontalBar", new ImageInfo(Content.Load<Texture2D>(@"ShortHorizontalBar")));

            GlobalVariables.ImageDict.Add("LongVerticleBar", new ImageInfo(Content.Load<Texture2D>(@"LongVerticleBar")));
            GlobalVariables.ImageDict.Add("ShortVerticleBar", new ImageInfo(Content.Load<Texture2D>(@"ShortVerticleBar")));

            GlobalVariables.ImageDict.Add("SquareBar", new ImageInfo(Content.Load<Texture2D>(@"SquareBar")));

            GlobalVariables.ImageDict.Add("Rope001", new ImageInfo(Content.Load<Texture2D>(@"Rope001")));
            #endregion

            #endregion

            #region Character Assets

            #region Nani

            #region Action: None
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Normal States
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Standing
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(3, 1), new Point(2, 2) }); //Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(3, 0) }); //Jumping
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(6, 0), new Point(1, 1) }); //Double Jumping
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(3, 0) }); //Jumped
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(4, 5), new Point(5, 5) }); //Sliding
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(1, 7), new Point(4, 7) }); //Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 7), new Point(5, 7) }); //Ledge Grabbing
            tempFrameDurationList.Add(120);
            #endregion

            #region Slash/Stab Stuff
            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(0, 7) }); //Slashing
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(5, 6), new Point(3, 6) }); //Slashing Charge
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(3, 6) }); //Slashing Hold Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 7), new Point(0, 7) }); //Slashing Hold Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(0, 7) }); //Slashing Release
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(2, 6) }); //Stabbing Charge
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(0, 6) }); //Stabbing Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(2, 6) }); //Stabbing Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 6), new Point(0, 6) }); //Stabbing Release
            tempFrameDurationList.Add(120);
            #endregion

            #region Directional Shooting
            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(5, 4) }); //Shooting Forward
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(4, 4) }); //Shooting Forward Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 4), new Point(5, 4) }); //Shooting Forward Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(3, 5) }); //Shooting Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 5), new Point(0, 5) }); //Shooting Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 5), new Point(2, 5) }); //Shooting Forward Up
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(1, 5), new Point(1, 5) }); //Shooting Forward Up Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 5), new Point(2, 5) }); //Shooting Forward Up Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 5), new Point(0, 5) }); //Shooting Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Open Palm Stuff
            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(5, 2) }); //Open Palm Forward
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(5, 2), new Point(3, 2) }); //Open Palm Forward Charge
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(3, 2) }); //Open Palm Forward In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 2), new Point(5, 2) }); //Open Palm Forward Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(2, 2) }); //Open Palm Forward Release
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 3), new Point(3, 3) }); //Open Palm Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 3), new Point(1, 3) }); //Open Palm Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 3), new Point(2, 3) }); //Open Palm Forward Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 3), new Point(0, 3) }); //Open Palm Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Walking
            tempLoopList.Add(new Point[2] { new Point(4, 8), new Point(1, 9) }); //Shooting Forward Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 8), new Point(1, 9) }); //Shooting Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 8), new Point(1, 9) }); //Shooting Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 8), new Point(1, 9) }); //Shooting Forward Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 8), new Point(1, 9) }); //Shooting Forward Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 8), new Point(3, 8) }); //Open Palm Forward Walking
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Running
            tempLoopList.Add(new Point[2] { new Point(2, 9), new Point(1, 10) }); //Shooting Forward Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 9), new Point(1, 10) }); //Shooting Up Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 9), new Point(1, 10) }); //Shooting Down Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 9), new Point(1, 10) }); //Shooting Forward Up Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 9), new Point(1, 10) }); //Shooting Forward Down Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(4, 3), new Point(3, 4) }); //Open Palm Forward Running
            tempFrameDurationList.Add(80);
            #endregion

            GlobalVariables.ImageDict.Add("NaniNone", new ImageInfo(Content.Load<Texture2D>(@"NaniNone"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            #region Action: Death
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(4, 0) }); //Falling
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(5, 0), new Point(5, 0) }); //Laying
            tempFrameDurationList.Add(80);

            GlobalVariables.ImageDict.Add("NaniDeath", new ImageInfo(Content.Load<Texture2D>(@"NaniDeath"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            GlobalVariables.AvatarList.Add("Nani");
            GlobalVariables.ImageDict.Add("Nani_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Naniscaled")));
            #endregion

            #region Ninja

            #region Action: None
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Normal States
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Standing
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(2, 1), new Point(0, 2) }); //Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(3, 0) }); //Jumping
            tempFrameDurationList.Add(150);

            tempLoopList.Add(new Point[2] { new Point(2, 0), new Point(3, 0) }); //Double Jumping
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(3, 0) }); //Jumped
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(1, 5), new Point(2, 5) }); //Sliding
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(4, 6), new Point(1, 7) }); //Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 7), new Point(2, 7) }); //Ledge Grabbing
            tempFrameDurationList.Add(120);
            #endregion

            #region Slash/Stab Stuff
            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(3, 6) }); //Slashing
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(0, 6) }); //Slashing Charge
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(0, 6) }); //Slashing Hold Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(3, 6) }); //Slashing Hold Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(3, 6) }); //Slashing Release
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(5, 5) }); //Stabbing
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 5), new Point(3, 5) }); //Stabbing Charge
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(3, 5) }); //Stabbing Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 5), new Point(5, 5) }); //Stabbing Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(5, 5) }); //Stabbing Release
            tempFrameDurationList.Add(120);
            #endregion

            #region Directional Shooting
            tempLoopList.Add(new Point[2] { new Point(1, 4), new Point(2, 4) }); //Shooting Forward
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(1, 4), new Point(1, 4) }); //Shooting Forward Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 4), new Point(2, 4) }); //Shooting Forward Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 5), new Point(0, 5) }); //Shooting Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 4), new Point(3, 4) }); //Shooting Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(5, 4) }); //Shooting Forward Up
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(4, 4) }); //Shooting Forward Up Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 4), new Point(5, 4) }); //Shooting Forward Up Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 4), new Point(3, 4) }); //Shooting Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Open Palm Stuff
            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(3, 2) }); //Open Palm Forward
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(1, 2) }); //Open Palm Forward Charge
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(1, 2) }); //Open Palm Forward In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(3, 2) }); //Open Palm Forward Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(3, 2) }); //Open Palm Forward Release
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 3), new Point(1, 3) }); //Open Palm Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 2), new Point(4, 2) }); //Open Palm Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 3), new Point(0, 3) }); //Open Palm Forward Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 2), new Point(5, 2) }); //Open Palm Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Walking
            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 7), new Point(0, 8) }); //Open Palm Forward Walking
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Running
            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Up Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Down Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Up Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Down Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 3), new Point(0, 4) }); //Open Palm Forward Running
            tempFrameDurationList.Add(80);
            #endregion

            GlobalVariables.ImageDict.Add("NinjaNone", new ImageInfo(Content.Load<Texture2D>(@"NinjaNone"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            #region Action: Death
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(4, 0) }); //Falling
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(5, 0), new Point(5, 0) }); //Laying
            tempFrameDurationList.Add(80);

            GlobalVariables.ImageDict.Add("NinjaDeath", new ImageInfo(Content.Load<Texture2D>(@"NinjaDeath"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            GlobalVariables.AvatarList.Add("Ninja");
            GlobalVariables.ImageDict.Add("Ninja_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/Ninjastandscaled")));
            #endregion

            #region Peter

            #region Action: None
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Normal States
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Standing
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(2, 1), new Point(0, 2) }); //Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(3, 0) }); //Jumping
            tempFrameDurationList.Add(150);

            tempLoopList.Add(new Point[2] { new Point(2, 0), new Point(3, 0) }); //Double Jumping
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(3, 0), new Point(3, 0) }); //Jumped
            tempFrameDurationList.Add(100);

            tempLoopList.Add(new Point[2] { new Point(1, 5), new Point(2, 5) }); //Sliding
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(4, 6), new Point(1, 7) }); //Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 7), new Point(2, 7) }); //Ledge Grabbing
            tempFrameDurationList.Add(120);
            #endregion

            #region Slash/Stab Stuff
            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(3, 6) }); //Slashing
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(0, 6) }); //Slashing Charge
            tempFrameDurationList.Add(70);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(0, 6) }); //Slashing Hold Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 6), new Point(3, 6) }); //Slashing Hold Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 6), new Point(3, 6) }); //Slashing Release
            tempFrameDurationList.Add(40);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(5, 5) }); //Stabbing
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 5), new Point(3, 5) }); //Stabbing Charge
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(3, 5) }); //Stabbing Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 5), new Point(5, 5) }); //Stabbing Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 5), new Point(5, 5) }); //Stabbing Release
            tempFrameDurationList.Add(120);
            #endregion

            #region Directional Shooting
            tempLoopList.Add(new Point[2] { new Point(1, 4), new Point(2, 4) }); //Shooting Forward
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(1, 4), new Point(1, 4) }); //Shooting Forward Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(2, 4), new Point(2, 4) }); //Shooting Forward Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 5), new Point(0, 5) }); //Shooting Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 4), new Point(3, 4) }); //Shooting Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(5, 4) }); //Shooting Forward Up
            tempFrameDurationList.Add(50);

            tempLoopList.Add(new Point[2] { new Point(4, 4), new Point(4, 4) }); //Shooting Forward Up Hold Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 4), new Point(5, 4) }); //Shooting Forward Up Hold In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 4), new Point(3, 4) }); //Shooting Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Open Palm Stuff
            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(3, 2) }); //Open Palm Forward
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(1, 2) }); //Open Palm Forward Charge
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(1, 2) }); //Open Palm Forward In
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 2), new Point(3, 2) }); //Open Palm Forward Out
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 2), new Point(3, 2) }); //Open Palm Forward Release
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 3), new Point(1, 3) }); //Open Palm Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(4, 2), new Point(4, 2) }); //Open Palm Down
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(0, 3), new Point(0, 3) }); //Open Palm Forward Up
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(5, 2), new Point(5, 2) }); //Open Palm Forward Down
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Walking
            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Up Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(1, 8), new Point(4, 8) }); //Shooting Forward Down Walking
            tempFrameDurationList.Add(120);

            tempLoopList.Add(new Point[2] { new Point(3, 7), new Point(0, 8) }); //Open Palm Forward Walking
            tempFrameDurationList.Add(120);
            #endregion

            #region Shoot/Palm While Running
            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Up Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Down Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Up Running
            tempFrameDurationList.Add(90);

            tempLoopList.Add(new Point[2] { new Point(5, 8), new Point(3, 9) }); //Shooting Forward Down Running
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(2, 3), new Point(0, 4) }); //Open Palm Forward Running
            tempFrameDurationList.Add(80);
            #endregion

            GlobalVariables.ImageDict.Add("PeterNone", new ImageInfo(Content.Load<Texture2D>(@"PeterNone"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            #region Action: Death
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(4, 0) }); //Falling
            tempFrameDurationList.Add(200);

            tempLoopList.Add(new Point[2] { new Point(5, 0), new Point(5, 0) }); //Laying
            tempFrameDurationList.Add(80);

            GlobalVariables.ImageDict.Add("PeterDeath", new ImageInfo(Content.Load<Texture2D>(@"PeterDeath"), new Point(100, 110), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            GlobalVariables.AvatarList.Add("Peter");
            GlobalVariables.ImageDict.Add("Peter_Icon", new ImageInfo(Content.Load<Texture2D>(@"GUI Assets/peter1scaled")));
            #endregion

            #endregion

            #region Hazard Assets
            GlobalVariables.ImageDict.Add("HealthOrb", new ImageInfo(Content.Load<Texture2D>(@"HealthOrb"), new Point(32, 32), 100));
            GlobalVariables.ImageDict.Add("BombFire", new ImageInfo(Content.Load<Texture2D>(@"BombFire"), new Point(32, 31), 100));
            GlobalVariables.ImageDict.Add("LightningBolt", new ImageInfo(Content.Load<Texture2D>(@"LightningBolt"), new Point(140, 14), 100));
            GlobalVariables.ImageDict.Add("ForcePush", new ImageInfo(Content.Load<Texture2D>(@"ForcePush"), new Point(57, 102), 100));
            GlobalVariables.ImageDict.Add("PistolBullet", new ImageInfo(Content.Load<Texture2D>(@"PistolBullet"), new Point(40, 40), 100));
            GlobalVariables.ImageDict.Add("DABOMB", new ImageInfo(Content.Load<Texture2D>(@"DABOMB"), new Point(53, 67), 400));
            GlobalVariables.ImageDict.Add("NukeExplosion", new ImageInfo(Content.Load<Texture2D>(@"NukeExplosion"), new Point(40, 40), 40));

            #region HazardSpikeTrap
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Spike Trap States
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Spikes In
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(1, 0) }); //Spikes Out
            tempFrameDurationList.Add(80);

            #endregion

            GlobalVariables.ImageDict.Add("SpikeTrap", new ImageInfo(Content.Load<Texture2D>(@"SpikeTrap"), new Point(100, 110), tempLoopList, tempFrameDurationList));
            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            #region HazardPoisonSpikeTrap
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            #region Poison Spike Trap States
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(0, 0) }); //Spikes In
            tempFrameDurationList.Add(80);

            tempLoopList.Add(new Point[2] { new Point(1, 0), new Point(1, 0) }); //Spikes Out
            tempFrameDurationList.Add(80);

            #endregion

            GlobalVariables.ImageDict.Add("PoisonSpikeTrap", new ImageInfo(Content.Load<Texture2D>(@"PoisonSpikeTrap"), new Point(100, 110), tempLoopList, tempFrameDurationList));
            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion

            /*
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(1, 0) });
            tempFrameDurationList.Add(50);
            GlobalVariables.ImageDict.Add("LightningBolt", new ImageInfo(Content.Load<Texture2D>(@"LightningBolt"), new Point(140, 14), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            */
            #endregion

            #region Item Assets
            #endregion

            #region AnimationLoopTest
            tempLoopList = new List<Point[]>();
            tempFrameDurationList = new List<int>();

            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(2, 1) });
            tempLoopList.Add(new Point[2] { new Point(3, 1), new Point(3, 2) });
            tempLoopList.Add(new Point[2] { new Point(1, 3), new Point(3, 3) });
            tempLoopList.Add(new Point[2] { new Point(0, 0), new Point(3, 3) });

            tempFrameDurationList.Add(100);
            tempFrameDurationList.Add(2000);
            tempFrameDurationList.Add(0);
            tempFrameDurationList.Add(1000);

            GlobalVariables.ImageDict.Add("AnimationLoopTest", new ImageInfo(Content.Load<Texture2D>(@"AnimationLoopTest"), new Point(52, 52), tempLoopList, tempFrameDurationList));

            tempLoopList = null;
            tempFrameDurationList = null;
            #endregion


            #endregion

            #region Menus
            menus.Add("LoginMenu", new LoginMenu(null, this));
            menus.Add("MainMenu", new MainMenu(menus["LoginMenu"], this));
            menus.Add("ServerMenu", new ServerList(menus["MainMenu"], this));
            menus.Add("CustomizationMenu", new CustomizationMenu(menus["MainMenu"], this));
            menus.Add("RegistrationMenu", new RegistrationMenu(menus["LoginMenu"], this));
            menus.Add("MatchResultsMenu", new MatchResults(null, this));
            menus.Add("SplashScreenMenu", new SplashScreen(null, this));
            #endregion

            hud = new HUD(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //Disconnect from master server and server
            NetworkingHandler.ServerDisconnect("Client Closing");
            NetworkingHandler.MasterServerDisconnect("Client Closing");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Updates the Game1 controller, used for menus and stuff (not the best solution)
            for (int i = 0; i < playerOneController.ControllerState.ControlArray.Length; ++i)
            {
                playerOneController.ControllerState.ControlArray[i] = false;
            }

            playerOneControllerGamepad.Update(gameTime);
            playerOneControllerKeyboard.Update(gameTime);
            for (int i = 0; i < playerOneControllerGamepad.ControllerState.ControlArray.Length; ++i)
            {
                if (playerOneControllerGamepad.ControllerState.ControlArray[i])
                {
                    playerOneController.ControllerState.ControlArray[i] = true;
                }
            }

            for (int i = 0; i < playerOneControllerKeyboard.ControllerState.ControlArray.Length; ++i)
            {
                if (playerOneControllerKeyboard.ControllerState.ControlArray[i])
                {
                    playerOneController.ControllerState.ControlArray[i] = true;
                }
            }

            switch (currGameState)
            {
                #region Splash Screen
                case GameState.InitializeSplashScreen:
                    menus["SplashScreenMenu"].Initialize();
                    currGameState = GameState.SplashScreen;
                    break;
                case GameState.SplashScreen:
                    menus["SplashScreenMenu"].HandleInput(playerOneController);
                    menus["SplashScreenMenu"].Update(gameTime);
                    //currGameState = GameState.InitializeLoginMenu;
                    break;
                #endregion

                #region Login Menu
                case GameState.InitializeLoginMenu:
                    NetworkingHandler.StartClient();
                    menus["LoginMenu"].Initialize();

                    currGameState = GameState.LoginMenu;

                    break;
                case GameState.LoginMenu:
                    NetworkingHandler.HandleMessages(this);
                    menus["LoginMenu"].HandleInput(playerOneController);
                    menus["LoginMenu"].Update(gameTime);
                    break;
                #endregion

                #region Registration Menu
                case GameState.InitializeRegistrationMenu:
                    NetworkingHandler.StartClient();
                    menus["RegistrationMenu"].Initialize();

                    currGameState = GameState.RegistrationMenu;

                    break;
                case GameState.RegistrationMenu:
                    //NetworkingHandler.HandleMessages();
                    menus["RegistrationMenu"].HandleInput(playerOneController);
                    menus["RegistrationMenu"].Update(gameTime);
                    break;
                #endregion

                #region Main Menu
                case GameState.InitializeMainMenu:
                    menus["MainMenu"].Initialize();
                    #region Windowed
                    //Full Screen Initialization
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 480;
                    graphics.IsFullScreen = false;
                    graphics.ApplyChanges();
                    #endregion


                    currGameState = GameState.MainMenu;

                    break;
                case GameState.MainMenu:
                    NetworkingHandler.HandleMessages(this);
                    menus["MainMenu"].HandleInput(playerOneController);
                    menus["MainMenu"].Update(gameTime);
                    break;
                #endregion

                #region Customization Menu
                case GameState.InitializeCustomizationMenu:
                    menus["CustomizationMenu"].Initialize();
                    NetworkingHandler.HandleMessages(this);
                    if (GlobalVariables.CharLoadout.Exp != -1)
                        currGameState = GameState.CustomizationMenu;
                    break;
                case GameState.CustomizationMenu:
                    menus["CustomizationMenu"].HandleInput(playerOneController);
                    menus["CustomizationMenu"].Update(gameTime);
                    NetworkingHandler.HandleMessages(this);
                    break;
                #endregion

                #region Server List Menu
                case GameState.InitializeServerListMenu:
                    menus["ServerMenu"].Initialize();
                    currGameState = GameState.ServerListMenu;
                    break;
                case GameState.ServerListMenu:
                    menus["ServerMenu"].HandleInput(playerOneController);
                    menus["ServerMenu"].Update(gameTime);
                    NetworkingHandler.HandleMessages(this);
                    break;
                #endregion

                #region Game
                case GameState.InitializeGame:
                    #region Full Screen
                    //Full Screen Initialization
                    if (!graphics.IsFullScreen)
                    {
                        graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
                        graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
                        graphics.IsFullScreen = true;
                        graphics.ApplyChanges();
                    }
                    #endregion

                    //NetworkingHandler.ConnectToServer("127.0.0.1", 37337);
                    GameObjectHandler.TrackingCamera.TrackingObj = GameObjectHandler.LocalPlayer.PhysicsObj;
                    hud.OwnerObj = GameObjectHandler.LocalPlayer;
                    currGameState = GameState.Game;
                    break;
                case GameState.Game:
                    #region Temporary Back Button
                    if (playerOneControllerGamepad.ControllerState.isControlPressed(Control.Back) || playerOneControllerKeyboard.ControllerState.isControlPressed(Control.Back))
                    {
                        //This has to be tested and it should only disconnect from the server, not the master server...
                        NetworkingHandler.ServerDisconnect();//"User Request");
                        currGameState = GameState.InitializeMainMenu;
                    }
                    #endregion
                    GameObjectHandler.LocalPlayer.Controller.Update(gameTime);
                    NetworkingHandler.HandleMessages(this);
                    GameObjectHandler.Update(gameTime, graphics.GraphicsDevice.Viewport);
                    GameObjectHandler.TrackingCamera.Update();
                    GlobalVariables.IncrementFrame();
                    hud.Update(gameTime, graphics.GraphicsDevice.Viewport);
                    break;
                #endregion

                #region Between Matches
                case GameState.InitalizeBetweenMatches:
                    GameObjectHandler.Reset();
                    menus["MatchResultsMenu"].Initialize();
                    currGameState = GameState.BetweenMatches;
                    break;
                case GameState.BetweenMatches:
                    bool connected = false;
                    string failedToConnect = "";
                    NetworkingHandler.WorldInit(ref connected, ref failedToConnect);
                    if (connected)
                    {
                        currGameState = GameState.InitializeGame;
                    }

                    menus["MatchResultsMenu"].Update(gameTime);
                    break;
                #endregion
            }

            playerOneControllerGamepad.PrevControllerState.Copy(playerOneControllerGamepad.ControllerState);
            playerOneControllerKeyboard.PrevControllerState.Copy(playerOneControllerKeyboard.ControllerState);
            playerOneController.PrevControllerState.Copy(playerOneController.ControllerState);
            GlobalVariables.SongManagerUpdate(gameTime);
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
                #region Splash Screen
                case GameState.InitializeSplashScreen:
                    break;
                case GameState.SplashScreen:
                    menus["SplashScreenMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Login Menu
                case GameState.InitializeLoginMenu:
                    break;
                case GameState.LoginMenu:
                    menus["LoginMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Login Menu
                case GameState.InitializeRegistrationMenu:
                    break;
                case GameState.RegistrationMenu:
                    menus["RegistrationMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Main Menu
                case GameState.InitializeMainMenu:
                    break;
                case GameState.MainMenu:
                    menus["MainMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Customization Menu
                case GameState.InitializeCustomizationMenu:
                    break;
                case GameState.CustomizationMenu:
                    menus["CustomizationMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Server List Menu
                case GameState.InitializeServerListMenu:
                    break;
                case GameState.ServerListMenu:
                    menus["ServerMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion

                #region Game
                case GameState.InitializeGame:
                    break;
                case GameState.Game:

                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, GameObjectHandler.TrackingCamera.GetTransformation(graphics.GraphicsDevice));
                    GameObjectHandler.Draw(spriteBatch);
                    spriteBatch.End();

                    spriteBatch.Begin();
                    hud.Draw(spriteBatch);
                    spriteBatch.End();

                    break;
                #endregion

                #region Between Matches
                case GameState.InitalizeBetweenMatches:
                    break;
                case GameState.BetweenMatches:
                    menus["MatchResultsMenu"].Draw(spriteBatch, GraphicsDevice, Point.Zero);
                    break;
                #endregion
            }

            base.Draw(gameTime);
        }
    }
}
