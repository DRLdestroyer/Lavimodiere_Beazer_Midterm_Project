using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lavimodiere_Beazer_Midterm_Project
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D spriteSheet;          
        Texture2D titleScreen;            
        SpriteFont defaultFont;            


        enum GameStates { TitleScreen, Playing, WaveComplete, GameOver };
        GameStates gameState = GameStates.TitleScreen;     
        float gameOverTimer = 0.0f;       
        float gameOverDelay = 6.0f;       
        float waveCompleteTimer = 0.0f;     
        float waveCompleteDelay = 6.0f;    



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

            Window.Title = "Player Square:" + (Player.BaseSprite.WorldLocation/32).ToPoint().ToString();//!player location

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
                WeaponManager.Draw(spriteBatch);     
                Player.Draw(spriteBatch);             
                EnemyManager.Draw(spriteBatch);      
                EffectsManager.Draw(spriteBatch);      

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



    }
}
