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


        public static void Initialize(Texture2D texture, Rectangle initialFrame)
        {
            enemyTexture = texture;                  
            enemyInitialFrame = initialFrame;         
        }


        public static void AddEnemy(Vector2 squareLocation, Enemy.MovementType movement, Enemy.WeaponType weapon)
        {
            int startX = (int)squareLocation.X;
            int startY = (int)squareLocation.Y;

            Rectangle squareRect = TileMap.SquareWorldRectangle(startX, startY);

            Enemy newEnemy = 
                new Enemy(new Vector2(squareRect.X, squareRect.Y),enemyTexture,enemyInitialFrame);

            newEnemy.currentTargetSquare = squareLocation;

            Enemies.Add(newEnemy);
        }

        public static void AddEnemyForLevel(int level)
        {
            if(level == 1)
            {
                //add specific enemy locations here//!
                AddEnemy(new Vector2(10, 10), Enemy.MovementType.Stationary, Enemy.WeaponType.Basic);
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
