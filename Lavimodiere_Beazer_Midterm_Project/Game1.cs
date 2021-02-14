using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Lavimodiere_Beazer_Midterm_Project
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Dictionary<string, SoundEffectInstance> soundEffectBank = new Dictionary<string, SoundEffectInstance>();

        Texture2D spriteSheet;          
        Texture2D titleScreen;            
        SpriteFont defaultFont;            


        public enum GameStates { TitleScreen, Playing, WaveComplete, GameOver };
        static public GameStates gameState = GameStates.TitleScreen;     
        float gameOverTimer = 0.0f;       
        float gameOverDelay = 6.0f;       
        float waveCompleteTimer = 0.0f;     
        float waveCompleteDelay = 6.0f;

        static public Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();
            this.IsMouseVisible = true;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheetNew");//sprite sheet src
            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            defaultFont = Content.Load<SpriteFont>(@"Fonts\defaultFont");

            //add instances of sounds to SoundInstance list (older at top of list)
            soundEffectBank.Add("bang", Content.Load<SoundEffect>(@"Audio\bang").CreateInstance());
            soundEffectBank.Add("boom", Content.Load<SoundEffect>(@"Audio\boom").CreateInstance());
            soundEffectBank.Add("death", Content.Load<SoundEffect>(@"Audio\death").CreateInstance());

            //to play sounds, utilize:
            //Game1.soundEffectBank["SoundID"].Play();

            TileMap.Initialize(spriteSheet);
            Player.Initialize(spriteSheet, new Rectangle(0, 64, 32, 32), 6,
                new Rectangle(0, 96, 32, 32), 1, new Vector2(32, 32));
            Camera.WorldRectangle = new Rectangle(0, 0, 1600, 1600);
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;

            EffectsManager.Initialize(
                spriteSheet, new Rectangle(0, 288, 2, 2), new Rectangle(0, 256, 32, 32), 3);


            WeaponManager.Texture = spriteSheet;

            EnemyManager.Initialize(spriteSheet, new Rectangle(0, 64, 32, 32));//set enemy sprite
        }

        protected override void UnloadContent()
        {
        }

        private void checkPlayerDeath()
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (enemy.EnemyBase.IsCircleColliding(
                    Player.BaseSprite.WorldCenter,
                    Player.BaseSprite.CollisionRadius))
                {
                    gameState = GameStates.GameOver;
                    
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (gameState)
            {
                case GameStates.TitleScreen:

                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) ||
                        (Keyboard.GetState().IsKeyDown(Keys.Space)))
                    {
                        GameManager.StartNewGame();       
                        gameState = GameStates.Playing;     
                    }
                    break;

                case GameStates.Playing:

                    Player.Update(gameTime);
                    WeaponManager.Update(gameTime);
                    EnemyManager.Update(gameTime);
                    EffectsManager.Update(gameTime);
                    //!when all enemies killed go to next area (unlock door)
                    //if ( == 0) gameState = GameStates.WaveComplete;//! 
                    break;

                case GameStates.WaveComplete:

                    waveCompleteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (waveCompleteTimer > waveCompleteDelay)
                    {
                        GameManager.StartNewWave();       
                        gameState = GameStates.Playing;   
                        waveCompleteTimer = 0.0f;        
                    }
                    break;

                case GameStates.GameOver:
                    
                    gameOverTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (gameOverTimer > gameOverDelay)
                    {
                        gameState = GameStates.TitleScreen;        
                        gameOverTimer = 0.0f;                    
                    }
                    break;
            }

            Window.Title = "Player Square:" + ConvertToGrid(Player.BaseSprite.WorldLocation).ToString();//!player location

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, 800, 600), Color.White);
            }


            if ((gameState == GameStates.Playing) || (gameState == GameStates.WaveComplete) ||
                (gameState == GameStates.GameOver))
            {
                TileMap.Draw(spriteBatch);            
                WeaponManager.Draw(spriteBatch, RandomColor());     
                Player.Draw(spriteBatch);             
                EnemyManager.Draw(spriteBatch);      
                EffectsManager.Draw(spriteBatch, RandomColor());      

                checkPlayerDeath();                    

                spriteBatch.DrawString(defaultFont,
                    "Score: " + GameManager.Score.ToString(), new Vector2(30, 5), Color.White);

                //spriteBatch.DrawString(defaultFont,"Terminals Remaining: " + GoalManager.ActiveTerminals, new Vector2(520, 5), Color.White);
            }

            if (gameState == GameStates.WaveComplete)
            {
                spriteBatch.DrawString(defaultFont, "Beginning Wave " + (GameManager.CurrentWave + 1).ToString(),
                    new Vector2(300, 300), Color.White);
            }

            if (gameState == GameStates.GameOver)
            {
                spriteBatch.DrawString(defaultFont, "G A M E O V E R!", new Vector2(300, 300), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //ultility
        public static Point ConvertToGrid(Point location)
        {
            return new Point(location.X / TileMap.TileHeight, location.Y / TileMap.TileWidth);
        }
        public static Point ConvertToGrid(Vector2 location)
        {
            return new Point((int)location.X / TileMap.TileHeight, (int)location.Y / TileMap.TileWidth);
        }
        public static Vector2 UnconvertFromGrid(Point location)
        {
            return new Vector2(location.X * TileMap.TileHeight, location.Y * TileMap.TileWidth);
        }
        public static Point UnconvertFromGrid(Vector2 location)
        {
            return new Point((int)location.X * TileMap.TileHeight, (int)location.Y * TileMap.TileWidth);
        }
        static public Color RandomColor()
        {
            Color randColor = new Color(rand.Next(256 / 2), rand.Next(256 / 2), rand.Next(256 / 2));
            if(randColor == Color.Blue)
            {
                RandomColor();
            }
            if (randColor == Color.Red)
            {
                RandomColor();
            }
            return randColor;
        }
    }
}
