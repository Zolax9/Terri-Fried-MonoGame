using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
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
        IsolatedStorageFile saveData; // Stores all saved data (just the highscore)
        int displayWidth;
        int displayHeight;

        // Terri-Fried variables
        const double pi = 3.1415926535897;
        const int gravity = 1;
        int screenWidth = 800;
        int screenHeight = 450;

        Platform[] platforms;
        Player player;

        int scoreInt = 0;
        int highscoreInt = 0;
        string score;  // Changed from 32-element char array to string (easier to deal with in C#)
        string highscore; // Changed from 32-element char array to string (easier to deal with in C#)

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

        Texture2D playerSprite;
        Texture2D lavaSprite;
        Texture2D platformSprite;
        Texture2D coinSprite;
        Texture2D scoreBoxSprite;
        Texture2D logo;
        Texture2D splashEggSprite;

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
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); // Represents SetTargetFPS(60);
        }

        protected override void Initialize()
        {
            displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            ex = new Extra(GraphicsDevice, displayWidth, displayHeight, screenWidth, ref screenHeight); // Changes screen height as well (see Extra class)
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

            playerSprite = Content.Load<Texture2D>("resources/egg");
            lavaSprite = Content.Load<Texture2D>("resources/lava");
            platformSprite = Content.Load<Texture2D>("resources/platform");
            coinSprite = Content.Load<Texture2D>("resources/coin");
            scoreBoxSprite = Content.Load<Texture2D>("resources/scorebox");
            logo = Content.Load<Texture2D>("resources/logo");
            splashEggSprite = Content.Load<Texture2D>("resources/splash_egg");

            fxLaunch = Content.Load<SoundEffect>("resources/launch").CreateInstance();
            fxClick = Content.Load<SoundEffect>("resources/click").CreateInstance();
            fxDeath = Content.Load<SoundEffect>("resources/die").CreateInstance();
            fxCoin = Content.Load<SoundEffect>("resources/fxCoin").CreateInstance(); // coin.wav renamed to fxCoin to prevent overwrite by coin.png (file extensions are removed)
            fxSplash = Content.Load<SoundEffect>("resources/splash").CreateInstance();
            fxSelect = Content.Load<SoundEffect>("resources/select").CreateInstance();
            font = Content.Load<SpriteFont>("resources/font");

            // SetMasterVolume(0.3f); All sound effects are set individually
            fxLaunch.Volume = 0.3f;
            fxClick.Volume = 0.3f;
            fxDeath.Volume = 0.3f;
            fxCoin.Volume = 0.3f;
            fxSplash.Volume = 0.3f;
            fxSelect.Volume = 0.3f;

            // Unloading content is already done by C# (managed)
        }

        protected override void Update(GameTime gameTime)
        {
            ex.Update(); // Updates extra functions

            if (titleScreen)
            {
                if (splashTimer > 120)
                {
                    if (!playedSelect)
                    {
                        fxSelect.Play();
                        playedSelect = true;
                    }
                    if (ex.raylibMouseState.Down)
                    {
                        fxSelect.Play();
                        titleScreen = false;
                        mouseDownX = ex.raylibMouseState.X;
                        mouseDownY = ex.raylibMouseState.Y;
                    }
                }
                else
                {
                    if (!playedSplash)
                    {
                        fxSplash.Play();
                        playedSplash = true;
                    }
                    splashTimer += 1;
                }
            }
            else
            {


                if (playCoinFX)
                {
                    fxCoin.Play();
                    playCoinFX = false;
                }
                if (ex.raylibMouseState.Pressed && player.isOnGround())
                {
                    fxClick.Play();
                    mouseDownX = ex.raylibMouseState.X;
                    mouseDownY = ex.raylibMouseState.Y;
                }
                if (ex.raylibMouseState.Released && player.isOnGround())
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
                        int velocityX = ex.raylibMouseState.X - mouseDownX;

                        int velocityY = ex.raylibMouseState.Y - mouseDownY;

                        player.setVelocity((double)velocityX * .08, (double)velocityY * .08);
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            rt = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            GraphicsDevice.SetRenderTarget(rt); // Renders all sprites to rt rather than screen

            if (titleScreen)
            {
                if (splashTimer > 120)
                {
                    _spriteBatch.Begin();
                    GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));

                    _spriteBatch.Draw(logo, new Vector2(screenWidth / 2 - 200, screenHeight / 2 - 45 - 30), Color.White);
                    _spriteBatch.DrawString(font, highscore, new Vector2(screenWidth / 2 - 37, screenHeight / 2 + 10), Color.Black, 0f, Vector2.Zero, (float)(0.5 * 2) / 3, SpriteEffects.None, 0f);  // (0.5 * 2) / 3 = 32 font size
                    _spriteBatch.DrawString(font, "CLICK ANYWHERE TO BEGIN", new Vector2(screenWidth / 2 - 134, screenHeight / 2 + 50), new Color(213, 196, 184, 255), 0f, Vector2.Zero, (float)(0.5 * 2) / 3, SpriteEffects.None, 0f);  // (0.5 * 2) / 3 = 32 font size & rgba values changed to 0-255 rgb values because rgba values in main.cpp differed
                    _spriteBatch.End();
                }
                else
                {
                    _spriteBatch.Begin();
                    GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));
                    _spriteBatch.DrawString(font, "POLYMARS", new Vector2(screenWidth / 2 - 54, screenHeight / 2 + 3), new Color(new Vector4(.835f, .502f, .353f, 1.0f)), 0f, Vector2.Zero, (float)(0.5 * 2) / 3, SpriteEffects.None, 0f);  // (0.5 * 2) / 3 = 32 font size
                    _spriteBatch.Draw(splashEggSprite, new Vector2(screenWidth / 2 - 16, screenHeight / 2 - 16 - 23), Color.White);
                    _spriteBatch.End();
                }
            }
            else
            {
                _spriteBatch.Begin();

                GraphicsDevice.Clear(new Color(new Vector4(0.933f, 0.894f, 0.882f, 1.0f)));
                if (ex.raylibMouseState.Down && player.isOnGround())
                {
                    ex.DrawLineEx(_spriteBatch, new Vector2((float)(mouseDownX + (player.getX() - mouseDownX) + (player.getWidth() / 2)), (float)(mouseDownY + (player.getY() - mouseDownY) + (player.getHeight() / 2))), new Vector2((float)(ex.raylibMouseState.X + (player.getX() - mouseDownX) + (player.getWidth() / 2)), (float)(ex.raylibMouseState.Y + (player.getY() - mouseDownY) + (player.getHeight() / 2))), 3, new Color(new Vector4(.906f, .847f, .788f, 1.0f)));
                }

                for (int i = 0; i < 4; ++i)
                {
                    _spriteBatch.Draw(platformSprite, new Vector2((float)platforms[i].getX(), (float)platforms[i].getY()), new Color(new Vector4(.698f, .588f, .49f, 1.0f)));
                    if (platforms[i].getHasCoin())
                    {
                        _spriteBatch.Draw(coinSprite, new Vector2(platforms[i].getCoinX(), platforms[i].getCoinY()), Color.White);
                    }
                }
                _spriteBatch.Draw(playerSprite, new Vector2((float)player.getX(), (float)player.getY()), Color.White);
                _spriteBatch.Draw(lavaSprite, new Vector2(0, (float)lavaY), Color.White);
                _spriteBatch.Draw(scoreBoxSprite, new Vector2(17, 17), Color.White);
                _spriteBatch.DrawString(font, score, new Vector2(28, 20), Color.Black, 0f, Vector2.Zero, (float)(1 * 2) / 3, SpriteEffects.None, 0f); // (1 * 2) / 3 = 64 font size   
                _spriteBatch.DrawString(font, highscore, new Vector2(17, 90), Color.Black, 0f, Vector2.Zero, (float)(0.5 * 2) / 3, SpriteEffects.None, 0f); // (0.5 * 2) / 3 = 32 font size

                _spriteBatch.End();
            }

            ex.Draw(GraphicsDevice, _spriteBatch, rt); // Draws the RenderTarget (and therefore, all sprites) to screen
            rt.Dispose(); // Disposes of the RenderTarget (not automatically managed)

            base.Draw(gameTime);
        }

        void addScore(int amount)
        {
            scoreInt += amount;
            if (scoreInt < 10)
            {
                score = string.Format("00{0}", scoreInt);
            }
            else if (scoreInt < 100)
            {
                score = string.Format("0{0}", scoreInt);
            }
            else
            {
                score = string.Format("{0}", scoreInt);
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
        void resetGame()
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
    }
}