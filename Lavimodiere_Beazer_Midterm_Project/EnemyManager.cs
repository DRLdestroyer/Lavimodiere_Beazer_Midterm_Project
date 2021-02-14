using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lavimodiere_Beazer_Midterm_Project
{
    static class EnemyManager
    {
        
        public static List<Enemy> Enemies = new List<Enemy>();
        public static Texture2D enemyTexture;
        public static Rectangle enemyInitialFrame;
        public static int MaxActiveEnemies = 30;
        private static bool enemiesAlive = true;


        public static void Initialize(Texture2D texture, Rectangle initialFrame)
        {
            enemyTexture = texture;                  
            enemyInitialFrame = initialFrame;         
        }


        public static void AddEnemy(Vector2 squareLocation, Enemy.MovementType movement, int weaponID)
        {
            int startX = (int)squareLocation.X;
            int startY = (int)squareLocation.Y;

            Rectangle squareRect = TileMap.SquareWorldRectangle(startX, startY);

            Enemy newEnemy = 
                new Enemy(new Vector2(squareRect.X, squareRect.Y),enemyTexture,enemyInitialFrame);

            newEnemy.currentTargetSquare = squareLocation;

            Enemies.Add(newEnemy);
        }

        public static void AddEnemiesForLevel(int level)
        {
            switch (level)//add specific enemy locations here//!
            {
                case 1:
                    AddEnemy(new Vector2(10, 10), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(15, 10), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(10, 5), Enemy.MovementType.Stationary, 0);
                    enemiesAlive = true;
                    break;
                case 2:
                    AddEnemy(new Vector2(20, 35 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(15, 40 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(15, 45 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(15, 35 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(10, 45 ), Enemy.MovementType.Stationary, 0);
                    enemiesAlive = true;
                    break;

                case 3:
                    AddEnemy(new Vector2(30, 10 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(35, 20 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(45, 10 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(45, 20 ), Enemy.MovementType.Stationary, 0);
                    AddEnemy(new Vector2(45, 15 ), Enemy.MovementType.Stationary, 0);
                    enemiesAlive = true;
                    break;
                default:
                    enemiesAlive = false;
                    break;
            }
        }

        public static void Update(GameTime gameTime)
        {
            for (int x = Enemies.Count - 1; x >= 0; x--)
            {
                Enemies[x].Update(gameTime);       
                
                if (Enemies[x].Destroyed)             
                {
                    Enemies.RemoveAt(x);            
                }
            }

            

            if (Enemies.Count <= 0)//!if area cleared of enemies
            {
                if (GameManager.CurrentWave == 1)
                {
                    AddEnemiesForLevel(GameManager.CurrentWave);

                }
                if (GameManager.CurrentWave > 1)//prevent on first wave
                {
                    
                    AddEnemiesForLevel(GameManager.CurrentWave);
                    if (GameManager.CurrentWave >= 2)
                    {
                        TileMap.SetTileAtSquare(TileMap.doorTileLoc[0].X, TileMap.doorTileLoc[0].Y, 0);//remove door from list here
                        TileMap.RandomizeTileColors();
                    }
                        
                    if (GameManager.CurrentWave >= 3)
                    {
                        TileMap.SetTileAtSquare(TileMap.doorTileLoc[1].X, TileMap.doorTileLoc[1].Y, 0);//remove door from list here
                        TileMap.RandomizeTileColors();
                    }
                        
                    if (GameManager.CurrentWave >= 4)
                    {
                        TileMap.SetTileAtSquare(TileMap.doorTileLoc[2].X, TileMap.doorTileLoc[2].Y, 0);//remove door from list here
                        TileMap.RandomizeTileColors();
                        Game1.gameState = Game1.GameStates.WaveComplete;
                        GameManager.CurrentWave = 0;
                    }
                }
                GameManager.CurrentWave++;
            }

        }
    
        
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

    }
}
