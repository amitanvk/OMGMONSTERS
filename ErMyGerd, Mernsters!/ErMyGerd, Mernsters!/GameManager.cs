using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using VGDevLib;
using ErMyGerdMernsters.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//xTile engine references
using xTile;
using xTile.Dimensions;
using xTile.Display;

using ProjectMercury;
using ProjectMercury.Controllers;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace ErMyGerdMernsters
{
    /// <summary>
    /// Enumerator for which state the game is in
    /// </summary>
    public enum GameState
    {
        MAIN_MENU, IN_GAME, PAUSED, OPTIONS, WIN, LOSE
    }

    public enum Faction
    {
        Player, Neutral, Enemy
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private List<GameElement> components;
        private GameState oldGameState;
        private float time_last_frame;
        private float time_between_frames = 1000f;
        private bool show = true;
        private bool gameStateChanged = false;
        private bool fadingBack = false;
        private int fadeLevel = 0;
        private int frames;
        private int fps;
        private int fpstimer;

        /// <summary>
        /// TO-DO: Comment Constructor
        /// </summary>
        public GameManager()
        {
            Global.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Global.Content = Content;
            System.Windows.Forms.Form.FromHandle(Window.Handle).MinimumSize = new System.Drawing.Size(640, 480);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            Global.Graphics.PreferredBackBufferHeight = 600;
            Global.Graphics.PreferredBackBufferWidth = 800;
            Global.Graphics.ApplyChanges();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Global.Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            Global.Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            Global.Graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            LoadContent();
            Global.GM = this;
            this.IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            oldGameState = Global.GameState;
            components = new List<GameElement>();
            Global.Cursor = new Cursor();
            Global.MapHandler = new MapHandler(Content);
            Global.Player = new Player();
            Global.Projectiles = new ProjectileList();
            Global.UI = new UI();
            Global.Enemies = new EnemyList();
            Global.Pickups = new PickupList();
            Global.WaveManager = new WaveManager();
            Global.CurrentMenu = new ErMyGerdMernsters.Menus.MainMenu(null);
            Global.Camera = new Camera();
            Global.ParticleManager = new ParticleManager();
            components.Add(Global.MapHandler);
            components.Add(Global.Cursor);
            components.Add(Global.Projectiles);
            components.Add(Global.Pickups);
            components.Add(Global.Player);
            components.Add(Global.Enemies);
            components.Add(Global.WaveManager);
            components.Add(Global.UI);
            base.Initialize();
        }

        public void startNewGame(int level)
        {
            Global.MapHandler.loadMap(level);
            Global.WaveManager.loadWaves(level);
            changeBackgroundMusic("Level " + level);
            Global.GameState = GameState.IN_GAME;
        }

        public void finish()
        {
            foreach (GameElement ge in components)
                ge.Reset();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Global.Textures = Util.LoadContent<Texture2D>(Content, "Textures");
            Global.MenuButtons = Util.LoadContent<Texture2D>(Content, "Textures\\Menu Buttons");
            Global.BackgroundMusic = Util.LoadContent<Song>(Content, "Background Music");
            Global.Fonts = Util.LoadContent<SpriteFont>(Content, "Fonts");
            Global.Maps = Util.LoadContent<Map>(Content, "Maps");
            Global.Waves = Util.LoadContent<Spawn[][]>(Content, "Maps\\Waves");
            Global.SoundEffects = Util.LoadContent<SoundEffect>(Content, "Sound Effects");
            Global.ParticleEffects = Util.LoadContent<ParticleEffect>(Content, "Particle Effects");

            Texture2D hpBar = new Texture2D(Global.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Microsoft.Xna.Framework.Color[] c = new Microsoft.Xna.Framework.Color[1];
            c[0] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(new Vector4(0, 255, 0, 1));
            hpBar.SetData<Microsoft.Xna.Framework.Color>(c);
            Global.Textures.Add("Health Bar", hpBar);

            Texture2D mpBar = new Texture2D(Global.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            c[0] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(new Vector4(0, 0, 2, 1));
            mpBar.SetData<Microsoft.Xna.Framework.Color>(c);
            Global.Textures.Add("Mana Bar", mpBar);

            MediaPlayer.Play(Global.BackgroundMusic["Main Menu"]);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
            // jliu: I don't think we're going to have any
        }

        private void changeBackgroundMusic(String dictionaryEntry)
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(Global.BackgroundMusic[dictionaryEntry]);
        }

        public void fade(SpriteBatch sb, Microsoft.Xna.Framework.Color color, int level, Microsoft.Xna.Framework.Rectangle area)
        {
            Texture2D texture = new Texture2D(Global.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Microsoft.Xna.Framework.Color[] c = new Microsoft.Xna.Framework.Color[1];
            c[0] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(new Vector4(color.ToVector3(), (float)level / 100.0f));
            texture.SetData<Microsoft.Xna.Framework.Color>(c);
            spriteBatch.Draw(texture, area, Microsoft.Xna.Framework.Color.White);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime);
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            //Advances the Global Random number generator anywhere between 0 and 100 seeds, prevents predictability of the random elements of the game
            int seedAdvance = Global.RNG.Next(100);
            for (int i = 0; i < seedAdvance; i++)
                Global.RNG.Next();

            gameStateChanged = oldGameState != Global.GameState;
            oldGameState = Global.GameState;

            switch (Global.GameState)
            {
                case GameState.MAIN_MENU:
                    if (gameStateChanged)
                    {
                        changeBackgroundMusic("Main Menu");
                        this.IsMouseVisible = true;
                        gameStateChanged = false;
                    }
                    Global.CurrentMenu.Update(gameTime);
                    break;
                case GameState.IN_GAME:
                    if (gameStateChanged)
                    {
                        this.IsMouseVisible = false;
                        gameStateChanged = false;
                    }
                    for (int i = 0; i < components.Count; i++)
                        components[i].Update(gameTime);
                    if (ControlManager.PAUSE.Bumped)
                    {
                        Global.CurrentMenu = new PauseMenu(null);
                        Global.GameState = GameState.PAUSED;
                    }
                    if (Global.WaveManager.IsFinished && !Global.Debug.DONT_SPAWN)
                    {
                        Global.GameState = GameState.WIN;
                        gameStateChanged = true;
                    }
                    break;
                case GameState.PAUSED:
                    if (gameStateChanged)
                    {
                        this.IsMouseVisible = true;
                    }
                    if (ControlManager.PAUSE.Bumped)
                    {
                        Global.GameState = GameState.IN_GAME;
                    }
                    Global.MapHandler.Update(gameTime);
                    Global.CurrentMenu.Update(gameTime);
                    break;
                case GameState.WIN:
                    if (gameStateChanged)
                    {
                        this.IsMouseVisible = true;
                        changeBackgroundMusic("Victory");
                        fadingBack = false;
                        Global.CurrentMenu = new ErMyGerdMernsters.Menus.MainMenu(null);
                        fadeLevel = 0;
                        gameStateChanged = false;
                        Global.Projectiles.Clear();
                    }
                    if (!fadingBack)
                    {
                        fadeLevel++;
                        if (fadeLevel >= 100)
                            fadingBack = true;
                    }
                    else
                    {
                        if (fadeLevel > 0)
                            fadeLevel--;
                        else
                        {
                            time_last_frame += gameTime.ElapsedGameTime.Milliseconds;
                            if (time_last_frame >= time_between_frames)
                            {
                                time_last_frame = 0f;
                                show = !show;
                            }
                            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                            {
                                Global.GameState = GameState.MAIN_MENU;
                                gameStateChanged = true;
                                fadingBack = false;
                                finish();
                            }
                        }
                    }

                    break;
                case GameState.LOSE:
                    if (gameStateChanged)
                    {
                        changeBackgroundMusic("Death");
                        this.IsMouseVisible = true;
                        Global.CurrentMenu = new ErMyGerdMernsters.Menus.MainMenu(null);
                        fadingBack = false;
                        fadeLevel = 0;
                    }
                    if (!fadingBack)
                    {
                        fadeLevel++;
                        if (fadeLevel >= 100)
                            fadingBack = true;
                    }
                    else
                    {
                        if (fadeLevel > 0)
                            fadeLevel--;
                        else
                        {
                            time_last_frame += gameTime.ElapsedGameTime.Milliseconds;
                            if (time_last_frame >= time_between_frames)
                            {
                                time_last_frame = 0f;
                                show = !show;
                            }
                            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                            {
                                Global.GameState = GameState.MAIN_MENU;
                                gameStateChanged = true;
                                fadingBack = false;
                                finish();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            Global.ParticleManager.Update(gameTime);
            base.Update(gameTime);
        }

        public void DrawCameraDependentElements(SpriteBatch sb)
        {
            for (int i = 0; i < components.Count; i++)
                if (components[i] is DrawableElement)
                {
                    DrawableElement element = (components[i] as DrawableElement);
                    if(element.CameraDependent)
                        element.Draw(sb);
                }
        }

        public void DrawCameraIndependentElements(SpriteBatch sb)
        {
            for (int i = 0; i < components.Count; i++)
                if (components[i] is DrawableElement)
                {
                    DrawableElement element = (components[i] as DrawableElement);
                    if(!element.CameraDependent)
                        element.Draw(sb);
                }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Draw Camera Independent Elements
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                                    BlendState.AlphaBlend,
                                    null,
                                    null,
                                    null,
                                    null,
                                    Global.Camera.get_transformation());
            switch (Global.GameState)
            {
                case GameState.PAUSED:
                case GameState.IN_GAME:
                case GameState.WIN:
                case GameState.LOSE:
                    DrawCameraDependentElements(spriteBatch);
                    break;
                default:
                    break;
            }
            spriteBatch.End();

            //Render Particles
            if(!Global.Debug.DONT_RENDER_PARTICLES)
                Global.ParticleManager.Render();

            //Draw Camera Independent Elements
            spriteBatch.Begin();
            if (Global.Debug.SHOW_FPS)
            {
                frames++;// += Global.ParticleManager.TotalParticles();
                fpstimer += gameTime.ElapsedGameTime.Milliseconds;
                if (fpstimer > 1000)
                {
                    fps = frames;
                    frames = 0;
                    fpstimer = 0;
                }
                Vector2 measurements = Global.Fonts["Pause Font"].MeasureString("" + fps);
                spriteBatch.DrawString(Global.Fonts["Pause Font"], "" + fps,
                    new Vector2(Global.Graphics.PreferredBackBufferWidth, 0)
                    - measurements * Vector2.UnitX, Microsoft.Xna.Framework.Color.Black);
            }
            switch (Global.GameState)
            {
                case GameState.MAIN_MENU:
                    Global.CurrentMenu.Draw(gameTime, spriteBatch);
                    break;
                case GameState.IN_GAME:
                    DrawCameraIndependentElements(spriteBatch);
                    break;
                case GameState.PAUSED:
                    DrawCameraIndependentElements(spriteBatch);
                    Global.CurrentMenu.Draw(gameTime, spriteBatch);
                    break;
                case GameState.WIN:
                    if (fadingBack)
                    {
                        spriteBatch.Draw(Global.Textures["Stage Victory"], new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight), Microsoft.Xna.Framework.Color.White);
                        if (show)
                        {
                            spriteBatch.Draw(Global.Textures["Title Message"], new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight), Microsoft.Xna.Framework.Color.White);
                        }
                    }
                    else
                    {
                        DrawCameraIndependentElements(spriteBatch);
                    }
                    fade(spriteBatch, Microsoft.Xna.Framework.Color.White, fadeLevel, new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight));
                    break;
                case GameState.LOSE:
                    if (fadingBack)
                    {
                        spriteBatch.Draw(Global.Textures["Game Over Screen"], new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight), Microsoft.Xna.Framework.Color.White);
                        if (show)
                        {
                            spriteBatch.Draw(Global.Textures["Title Message"], new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight), Microsoft.Xna.Framework.Color.White);
                        }
                    }
                    else
                    {
                        DrawCameraIndependentElements(spriteBatch);
                    }
                    fade(spriteBatch, Microsoft.Xna.Framework.Color.Black, fadeLevel, new Microsoft.Xna.Framework.Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight));
                    break;
                default:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public static class Global
    {
        //Content 
        public static Dictionary<string, Texture2D> Textures;
        public static Dictionary<string, Texture2D> MenuButtons;
        public static Dictionary<string, Song> BackgroundMusic;
        public static Dictionary<string, SpriteFont> Fonts;
        public static Dictionary<string, Map> Maps;
        public static Dictionary<string, Spawn[][]> Waves;
        public static Dictionary<string, SoundEffect> SoundEffects;
        public static Dictionary<string, ParticleEffect> ParticleEffects;

        public static GameManager GM;
        public static Cursor Cursor;
        public static Player Player;
        public static ProjectileList Projectiles;
        public static EnemyList Enemies;
        public static PickupList Pickups;
        public static MapHandler MapHandler;
        public static WaveManager WaveManager;
        public static Menu CurrentMenu;
        public static int RenderLimit;
        public static Random RNG = new Random();
        public static ContentManager Content;
        public static ParticleManager ParticleManager;
        public static Camera Camera;

        public static GraphicsDeviceManager Graphics;
        public static UI UI;
        public static GameState GameState = GameState.MAIN_MENU;

        public class Debug
        {
            public static bool HITBOX_SHOW = false;
            public static bool INVINCIBLE = false;
            public static bool INFINITE_MANA = false;
            public static bool SHOW_FPS = false;
            public static bool DONT_SPAWN = false;
            public static bool DONT_RENDER_PARTICLES = false;
        }
    }

#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameManager game = new GameManager())
            {
                game.Run();
            }
        }
    }
#endif
}