﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lavimodiere_Beazer_Midterm_Project
{   
    class ComputerTerminal
    {
        private Sprite activeSprite;
        private Sprite inactiveSprite;
        public Vector2 MapLocation;
        public bool Active = true;
        public float LastSpawnCounter = 0;
        public float minSpawnTime = 6.0f;


        public ComputerTerminal(Sprite activeSprite, Sprite inactiveSprite, Vector2 mapLocation)
        {
            MapLocation = mapLocation;                   
            this.activeSprite = activeSprite;            
            this.inactiveSprite = inactiveSprite;        
        }



        #region Public Methods
        public bool IsCircleColliding(Vector2 otherCenter, float radius)
        {
            if (!Active)
            {
                return false;          
            }

            return activeSprite.IsCircleColliding(otherCenter, radius);
        }


        public void Deactivate()
        {
            Active = false;   
        }
        
        
        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                LastSpawnCounter += elapsed;

                if (LastSpawnCounter > minSpawnTime)
                {
                    if (Vector2.Distance(activeSprite.WorldCenter, Player.BaseSprite.WorldCenter) > 128)
                    {
                        if (EnemyManager.Enemies.Count < EnemyManager.MaxActiveEnemies)
                        {
                            LastSpawnCounter = 0;
                        }
                    }
                }

                activeSprite.Update(gameTime);
            }
            else
            {
                inactiveSprite.Update(gameTime);          
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                activeSprite.Draw(spriteBatch);
            }
            else
            {
                inactiveSprite.Draw(spriteBatch);
            }
        }
        
        #endregion

    }
}
