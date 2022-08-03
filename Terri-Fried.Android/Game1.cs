using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Terri_Fried.Android
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Added variables
        Extra ex; // Contains all extra functions
        RenderTarget2D rt; // Contains everything rendered so it can be scaled to screen
        RenderTarget2D pauseMenu; // Contains the pause menu (paused text and background)
        IsolatedStorageFile saveData; // Stores all saved data (just the highscore)

        int displayWidth;
        int displayHeight;

        bool unpauseNextFrame; // Unpauses the game the frame after it was paused (prevents click being applied to game)
        bool pauseMouseDown; // If the mouse is still pressed after pausing (prevents click being applied to game)
        bool _paused; // If the game is paused
        bool paused
        {
            get { return _paused; }
            set
            {
                switch (value)
                {
                    case true:
                        fxLaunch.Pause();
                        fxClick.Pause();
                        fxDeath.Pause();
                        fxCoin.Pause();
                        fxSplash.Pause();
                        fxSelect.Pause();
                        break;

                    default:
                        // .Resume() function would play sound if not played
                        if (fxLaunch.State == SoundState.Paused) { fxLaunch.Resume(); }
                        if (fxClick.State == SoundState.Paused) { fxClick.Resume(); }
                        if (fxDeath.State == SoundState.Paused) { fxDeath.Resume(); }
                        if (fxCoin.State == SoundState.Paused) { fxCoin.Resume(); }
                        if (fxSplash.State == SoundState.Paused) { fxSplash.Resume(); }
                        if (fxSelect.State == SoundState.Paused) { fxSelect.Resume(); }
                        break;
                }
                _paused = value;
            }
        }

        Texture2D fadeTexture;
        Texture2D pauseTexture;
        Rectangle pauseRect;

        // Terri-Fried variables
        const double pi = 3.1415926535897;
        const int gravity = 1;
        const int screenWidth = 800;
        int screenHeight = 450;

        Platform[] platforms;
        Player player;

        int scoreInt = 0;
        int highscoreInt = 0;
        string score;
        string highscore;

        bool titleScreen = true;
        bool playCoinFX = false;

        int mouseDownX = 0;
        int mouseDownY = 0;
        double lavaY;
        double timer = 0;
        double splashTimer = 0;
        bool firstTime = true;
        bool playedSplash = false;
        bool playedSelect = false;

        Texture2D playerTexture;
        Texture2D lavaTexture;
        Texture2D platformTexture;
        Texture2D coinTexture;
        Texture2D scoreBoxTexture;
        Texture2D logoTexture;
        Texture2D splashEggTexture;

        SoundEffectInstance fxLaunch;
        SoundEffectInstance fxClick;
        SoundEffectInstance fxDeath;
        SoundEffectInstance fxCoin;
        SoundEffectInstance fxSplash;
        SoundEffectInstance fxSelect;
        SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        }

        protected override void Initialize()
        {
            displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            ex = new Extra(GraphicsDevice, displayWidth, displayHeight, screenWidth, ref screenHeight); // Changes screen height as well (see Extra class)
            rt = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            saveData = IsolatedStorageFile.GetUserStoreForDomain(); // Gets all save files (just highscore)

            lavaY = screenHeight - 32;

            platforms = new Platform[4] { new Platform(0, screenWidth, screenHeight), new Platform(1, screenWidth, screenHeight), new Platform(2, screenWidth, screenHeight), new Platform(3, screenWidth, screenHeight) };
            player = new Player(platforms[0].getX() + platforms[0].getWidth() / 2 - 26 / 2, platforms[0].getY() - 32, 26, 32, screenWidth, screenHeight); // player.getHeight() replaced with 32, assuming that they're equal

            // Creates a new highscore file if it doesn't exist
            if (!File.Exists("/data/data/io.github.zolax9.terri_fried/files/.local/share/highscore"))
            {
                FileStream fs = File.Create("/data/data/io.github.zolax9.terri_fried/files/.local/share/highscore");
                fs.Close();
            }

            // Gets the highscore integer
            using (FileStream fs = File.Open("/data/data/io.github.zolax9.terri_fried/files/.local/share/highscore", FileMode.Open)) // Opens the file of the stored highscore with a file stream
            {

                using (StreamReader sr = new StreamReader(fs)) // Reads the stored highscore
                {
                    try { highscoreInt = int.Parse(sr.ReadLine()); } // Represents int highscoreInt = LoadStorageValue(0);
                    catch (ArgumentNullException e) { highscoreInt = 0; } // Sets highscore to 0 if stored highscore isn't a number (corrupted)
                    sr.Close();
                }
            }
            highscore = string.Format("BEST: {0}", highscoreInt);

            resetScore();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("resources/egg");
            lavaTexture = Content.Load<Texture2D>("resources/lava");
            platformTexture = Content.Load<Texture2D>("resources/platform");
            coinTexture = Content.Load<Texture2D>("resources/coin");
            scoreBoxTexture = Content.Load<Texture2D>("resources/scorebox");
            logoTexture = Content.Load<Texture2D>("resources/logo");
            splashEggTexture = Content.Load<Texture2D>("resources/splash_egg");

            fadeTexture = Content.Load<Texture2D>("resources/bg_fade");
            pauseTexture = Content.Load<Texture2D>("resources/pause");
            pauseRect = new Rectangle(screenWidth - pauseTexture.Width, 0, pauseTexture.Width, pauseTexture.Height);

            fxLaunch = Content.Load<SoundEffect>("resources/launch").CreateInstance();
            fxClick = Content.Load<SoundEffect>("resources/click").CreateInstance();
            fxDeath = Content.Load<SoundEffect>("resources/die").CreateInstance();
            fxCoin = Content.Load<SoundEffect>("resources/fxCoin").CreateInstance();
            fxSplash = Content.Load<SoundEffect>("resources/splash").CreateInstance();
            fxSelect = Content.Load<SoundEffect>("resources/select").CreateInstance();

            font = Content.Load<SpriteFont>("resources/font");

            fxLaunch.Volume = 0.3f;
            fxClick.Volume = 0.3f;
            fxDeath.Volume = 0.3f;
            fxCoin.Volume = 0.3f;
            fxSplash.Volume = 0.3f;
            fxSelect.Volume = 0.3f;

            pauseMouseDown = false;
            paused = false;
        }

        protected override void Update(GameTime gameTime)
        {
            ex.Update(paused); // Updates extra functions

            unpauseNextFrame = false;
            if (!titleScreen)
            {
                if (ex.rbMouseState.Pressed && pauseRect.Contains(new Point(ex.rbMouseState.X, ex.rbMouseState.Y))) { pauseGame(false); } // If the screen is touched at the pause button area
            }
            if (pauseMouseDown)
            {
                // Prevents pressing of touch from escaping pause menu to game
                ex.gameRBMouseState.Down = false;
                ex.gameRBMouseState.Pressed = false;

                if (ex.rbMouseState.Released)
                {
                    // Prevents releasing of touch from escaping pause menu to game
                    ex.gameRBMouseState.Up = false;
                    ex.gameRBMouseState.Released = false;
                    pauseMouseDown = false;
                }
            }

            switch (titleScreen)
            {
                case true:
                    switch (splashTimer > 120)
                    {
                        case true:
                            if (!playedSelect)
                            {
                                fxSelect.Play();
                                playedSelect = true;
                            }
                            if (ex.gameRBMouseState.Down)
                            {
                                fxSelect.Play();
                                titleScreen = false;
                                mouseDownX = ex.gameRBMouseState.X;
                                mouseDownY = ex.gameRBMouseState.Y;
                            }
                            break;

                        default:
                            if (!playedSplash)
                            {
                                fxSplash.Play();
                                playedSplash = true;
                            }
                            splashTimer += 1;
                            break;
                    }
                    break;

                default:
                    if (!paused)
                    {
                        if (playCoinFX)
                        {
                            fxCoin.Play();
                            playCoinFX = false;
                        }
                        if (ex.gameRBMouseState.Pressed && player.isOnGround())
                        {
                            fxClick.Play();
                            mouseDownX = ex.gameRBMouseState.X;
                            mouseDownY = ex.gameRBMouseState.Y;
                        }
                        if (ex.gameRBMouseState.Released && player.isOnGround())
                        {
                            if (firstTime)
                            {
                                firstTime = false;
                            }
                            else
                            {
                                fxLaunch.Play();
                                if (player.isOnPlatform())
                                {
                                    player.setY(player.getY() - 1d);
                                }

                                int velocityX = ex.gameRBMouseState.X - mouseDownX;
                                int velocityY = ex.gameRBMouseState.Y - mouseDownY;
                                player.setVelocity(velocityX * .08, velocityY * .08);
                            }
                        }

                        checkPlayerCollision();
                        player.updatePosition();

                        if (player.getY() > screenHeight)
                        {
                            fxDeath.Play();
                            resetGame();
                        }

                        for (int i = 0; i < 4; ++i)
                        {
                            platforms[i].updatePosition();
                        }

                        lavaY = screenHeight - 43 - Math.Sin(timer) * 5;
                        timer += 0.05;
                    }

                    if (unpauseNextFrame) { paused = false; } // Unpauses game
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            pauseMenu = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            if (!paused)
            {
                if (!rt.IsDisposed) { rt.Dispose(); }// Disposes of the RenderTarget (not automatically managed)
                rt = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                GraphicsDevice.SetRenderTarget(rt); // Renders all sprites to rt rather than screen

                _spriteBatch.Begin();
                switch (titleScreen)
                {
                    case true:
                        switch (splashTimer > 120)
                        {
                            case true:
                                GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));
                                _spriteBatch.Draw(logoTexture, new Vector2(screenWidth / 2 - 200, screenHeight / 2 - 45 - 30), Color.White);
                                _spriteBatch.DrawString(font, highscore, new Vector2(screenWidth / 2 - 37, screenHeight / 2 + 10), Color.Black, 0f, Vector2.Zero, 0.5f * (2f / 3), SpriteEffects.None, 0f);  // 0.5f * (2f / 3) = 32 font size
                                _spriteBatch.DrawString(font, "CLICK ANYWHERE TO BEGIN", new Vector2(screenWidth / 2 - 134, screenHeight / 2 + 50), new Color(213, 196, 184, 255), 0f, Vector2.Zero, 0.5f * (2f / 3), SpriteEffects.None, 0f);
                                break;

                            default:
                                GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));
                                _spriteBatch.DrawString(font, "POLYMARS", new Vector2(screenWidth / 2 - 54, screenHeight / 2 + 3), new Color(new Vector4(.835f, .502f, .353f, 1.0f)), 0f, Vector2.Zero, 0.5f * (2f / 3), SpriteEffects.None, 0f);  // 0.5f * (2f / 3) = 32 font size
                                _spriteBatch.Draw(splashEggTexture, new Vector2(screenWidth / 2 - 16, screenHeight / 2 - 16 - 23), Color.White);
                                break;
                        }
                        break;

                    case false:
                        GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));
                        if (ex.gameRBMouseState.Down && player.isOnGround())
                        {
                            ex.DrawLineEx(_spriteBatch, new Vector2((float)(mouseDownX + (player.getX() - mouseDownX) + (player.getWidth() / 2)), (float)(mouseDownY + (player.getY() - mouseDownY) + (player.getHeight() / 2))), new Vector2((float)(ex.gameRBMouseState.X + (player.getX() - mouseDownX) + (player.getWidth() / 2)), (float)(ex.gameRBMouseState.Y + (player.getY() - mouseDownY) + (player.getHeight() / 2))), 3, new Color(new Vector4(.906f, .847f, .788f, 1.0f)));
                        }

                        for (int i = 0; i < 4; ++i)
                        {
                            _spriteBatch.Draw(platformTexture, new Vector2((float)platforms[i].getX(), (float)platforms[i].getY()), new Color(new Vector4(.698f, .588f, .49f, 1.0f)));
                            if (platforms[i].getHasCoin())
                            {
                                _spriteBatch.Draw(coinTexture, new Vector2(platforms[i].getCoinX(), platforms[i].getCoinY()), Color.White);
                            }
                        }

                        _spriteBatch.Draw(playerTexture, new Vector2((float)player.getX(), (float)player.getY()), Color.White);
                        _spriteBatch.Draw(lavaTexture, new Vector2(0, (float)lavaY), Color.White);
                        _spriteBatch.Draw(scoreBoxTexture, new Vector2(17, 17), Color.White);

                        _spriteBatch.DrawString(font, score, new Vector2(28, 20), Color.Black, 0f, Vector2.Zero, (float)(1 * 2) / 3, SpriteEffects.None, 0f); // (1 * 2) / 3 = 64 font size   
                        _spriteBatch.DrawString(font, highscore, new Vector2(17, 90), Color.Black, 0f, Vector2.Zero, (float)(0.5 * 2) / 3, SpriteEffects.None, 0f); // (0.5 * 2) / 3 = 32 font size

                        _spriteBatch.Draw(pauseTexture, pauseRect, Color.White);
                        break;
                }
                _spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(pauseMenu);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            _spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, (int)(screenWidth * ex.aspectMultiplier), (int)(screenHeight * ex.aspectMultiplier)), new Rectangle(0, 0, 1, 1), Color.White); // Adds fade effect to screen
            _spriteBatch.Draw(pauseTexture, pauseRect, Color.White);
            _spriteBatch.DrawString(font, "PAUSED", new Vector2((screenWidth / 2) - (font.MeasureString("PAUSED").X * (1f / 3)), (screenHeight / 2) - (font.MeasureString("PAUSED").Y * (1f / 3))), Color.Black, 0f, Vector2.Zero, 2f / 3, SpriteEffects.None, 0f);
            _spriteBatch.End();

            ex.Draw(GraphicsDevice, _spriteBatch, rt, pauseMenu, paused); // Draws the RenderTarget (and therefore, all sprites) to screen

            pauseMenu.Dispose(); // Disposes pause menu

            base.Draw(gameTime);
        }

        void addScore(int amount)
        {
            scoreInt += amount;
            switch (scoreInt)
            {
                case int n when (n < 10):
                    score = string.Format("00{0}", scoreInt);
                    break;

                case int n when (n < 100):
                    score = string.Format("0{0}", scoreInt);
                    break;

                default:
                    score = string.Format("{0}", scoreInt);
                    break;
            }
            if (scoreInt > highscoreInt)
            {
                highscoreInt = scoreInt;
                highscore = string.Format("BEST: {0}", highscoreInt);
            }
        }

        void resetScore()
        {
            scoreInt = 0;
            score = string.Format("00{0}", scoreInt);

            using (FileStream fs = File.Create("/data/data/io.github.zolax9.terri_fried/files/.local/share/highscore")) // Opens the file of the stored highscore with a file stream
            {
                using (StreamWriter sr = new StreamWriter(fs)) // Overwrites the scored highscore
                {
                    sr.WriteLine(highscoreInt.ToString()); // Represents SaveStorageValue(0, highscoreInt);
                    sr.Flush(); // Saves the changes made by the previous line of code
                    sr.Close();
                }
            }
        }
        public void resetGame()
        {
            resetScore();
            for (int i = 0; i < 4; ++i)
            {
                platforms[i] = new Platform(i, screenWidth, screenHeight);
            }

            player.setVelocity(0, 0);
            player.setX((int)platforms[0].getX() + platforms[0].getWidth() / 2 - 26 / 2);
            player.setY((int)platforms[0].getY() - player.getHeight());
        }

        void checkPlayerCollision()
        {
            bool onPlatform = false;
            for (int i = 0; i < 4; ++i)
            {
                if (platforms[i].getHasCoin() && player.getX() + player.getWidth() - 3 > platforms[i].getCoinX() && player.getX() + 3 < platforms[i].getCoinX() + 24 && player.getY() + player.getHeight() - 3 > platforms[i].getCoinY() && player.getY() + 3 < platforms[i].getCoinY() + 24)
                {
                    addScore(1);
                    platforms[i].setHasCoin(false);
                    playCoinFX = true;
                }
                if (player.getX() + 1 < platforms[i].getX() + platforms[i].getWidth() && player.getX() + player.getWidth() > platforms[i].getX() && player.getY() + player.getHeight() >= platforms[i].getY() && player.getY() < platforms[i].getY() + platforms[i].getHeight())
                {
                    if (player.getY() > platforms[i].getY() + platforms[i].getHeight() / 2)
                    {
                        player.setVelocity(player.getVelocity().X, 5);
                    }
                    else if (player.getY() + player.getHeight() < platforms[i].getY() + platforms[i].getHeight())
                    {
                        onPlatform = true;
                        player.setY((int)platforms[i].getY() - player.getHeight());
                        player.setY((int)player.getY() + 1);
                    }
                }
            }
            player.setOnPlatform(onPlatform);
        }

        public void pauseGame(bool pause) // Whether or not to alternate between paused and unpaused or just pause the game
        {
            switch (pause)
            {
                case true:
                    if (!titleScreen) { paused = true; }
                    break;

                default:
                    switch (paused)
                    {
                        case true:
                            unpauseNextFrame = true; // Unpauses after the rest of Update() is executed
                            pauseMouseDown = true;
                            break;

                        case false:
                            paused = true;
                            break;
                    }
                    break;
            }
        }
    }
}