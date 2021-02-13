﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lavimodiere_Beazer_Midterm_Project
{
    class Enemy
    {
        public Sprite EnemyBase;
        public Sprite EnemyClaws;
        public float EnemySpeed = 60f;
        public Vector2 currentTargetSquare;
        public bool Destroyed = false;
        private int collisionRadius = 14;
        public enum MovementType
        {
            Stationary, Moving
        };
        public enum WeaponType
        {
            Basic,//bounces once
            Fast,//doesnt bounce
            Bouncy//,//bounces twice
        };


        //Constructor(s)
        public Enemy(Vector2 worldLocation, Texture2D texture, Rectangle initialFrame)
        {
            EnemyBase = new Sprite(worldLocation, texture, initialFrame, Vector2.Zero);
            EnemyBase.CollisionRadius = collisionRadius;
            
            Rectangle turretFrame = initialFrame;
            turretFrame.Offset(0, initialFrame.Height);

            EnemyClaws = new Sprite(worldLocation, texture, turretFrame, Vector2.Zero);
        }


        public void Update(GameTime gameTime)
        {
            if (!Destroyed)
            {
               Vector2 direction = determineMoveDirection();
               direction.Normalize();
               EnemyBase.Velocity = direction * EnemySpeed;
               EnemyBase.RotateTo(direction);
   
               EnemyBase.Update(gameTime);

               Vector2 directionToPlayer = 
                   Player.BaseSprite.WorldCenter - EnemyBase.WorldCenter;
               directionToPlayer.Normalize();

               EnemyClaws.WorldLocation = EnemyBase.WorldLocation;
               EnemyClaws.RotateTo(directionToPlayer);
            }
        }
        
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Destroyed)
            {
               EnemyBase.Draw(spriteBatch);
               EnemyClaws.Draw(spriteBatch);
            }
        }

        #region AI
        private Vector2 determineMoveDirection()
        {
            if (reachedTargetSquare())
            {
                currentTargetSquare = getNewTargetSquare();
            }

            Vector2 squareCenter = TileMap.GetSquareCenter(currentTargetSquare);

            return squareCenter - EnemyBase.WorldCenter;
        }


        private bool reachedTargetSquare()
        {
            return (
            Vector2.Distance(EnemyBase.WorldCenter,TileMap.GetSquareCenter(currentTargetSquare)) <= 2);
        }

        private Vector2 getNewTargetSquare()
        {
            List<Vector2> path = PathFinder.FindPath(
            TileMap.GetSquareAtPixel(EnemyBase.WorldCenter),
            TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter));

            if (path.Count > 1)
            {
                return new Vector2(path[1].X, path[1].Y);
            }
            else
            {
                return TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter);
            }
        }
        #endregion
    }
}
