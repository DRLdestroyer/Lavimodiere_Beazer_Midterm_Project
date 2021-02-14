using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Lavimodiere_Beazer_Midterm_Project
{
    class WeaponManager
    {
        static public List<Particle> Shots = new List<Particle>();//List of shots in the world
        static public Texture2D Texture;
        
        //Projectile vars
        public enum WeaponType { Normal, Triple, Rocket };
        static private float[] minTimer = {
            0.15f,//shot//0
            0.5f//,//rocket//1
        };
        static public Rectangle shotRectangle = new Rectangle(0, 128, 32, 32);
        static public float WeaponSpeed = 600f;
        static private float shotTimer = 0f;
        static public int bouncesForWeapon = 1;//!
        static public int bouncesLeft = 1;//!


        static public WeaponType CurrentWeaponType = WeaponType.Normal;
        static public float WeaponTimeRemaining = 30.0f;
        static private float weaponTimeDefault = 30.0f;

        //B O M B S
        static public List<Sprite> bombList = new List<Sprite>();
        static private float bombTimer = 0f;
        static private float bombMinTimer = 3.0f;
        static public float detonateTimer = 0.0f;
        static private float detonateMinTimer = 1.0f;


        private static void AddShot(Vector2 location, Vector2 velocity, int projectileTextureID)
        {
            Particle shot = new Particle(
            location,
            Texture,
            shotRectangle,
            velocity,
            Vector2.Zero,
            400f,
            120,
            Color.White,
            Color.White);

            shot.AddFrame(new Rectangle(
            shotRectangle.X + shotRectangle.Width,
            shotRectangle.Y,
            shotRectangle.Width,
            shotRectangle.Height));
            shot.Animate = false;
            shot.Frame = projectileTextureID;
            shot.RotateTo(velocity);

            Shots.Add(shot);
        }
        private static void AddBomb(Vector2 location)
        {
            Sprite newBomb = new Sprite(
                   new Vector2(location.X, location.Y),
                   Texture,
                   new Rectangle(64, 160, 32, 32),
                   Vector2.Zero);

            newBomb.Animate = false;         //set animate to false
            newBomb.CollisionRadius = 14;    //Set a circular collision area
            newBomb.AddFrame(new Rectangle(128, 128, 32, 32)); //add a frame
            newBomb.Frame = 1;

            bombList.Add(newBomb);       //Add the powerup sprite to the list

        }

        private static void createLargeExplosion(Vector2 location)
        {
            Game1.soundEffectBank["boom"].Play();
            EffectsManager.AddLargeExplosion(location);
            EffectsManager.AddLargeExplosion(location + new Vector2(-10, -10));
            EffectsManager.AddLargeExplosion(location + new Vector2(-10, 10));
            EffectsManager.AddLargeExplosion(location + new Vector2(10, 10));
            EffectsManager.AddLargeExplosion(location + new Vector2(10, -10));            
        }


        static public float WeaponFireDelay
        {
            get
            {
                switch (CurrentWeaponType)
                {
                    case WeaponType.Normal:
                        return minTimer[0];
                    case WeaponType.Triple:
                        return minTimer[0];
                    case WeaponType.Rocket:
                        return minTimer[1];
                    default:
                        return 0;
                }
            }
        }

        static public bool CanFireWeapon
        {
            get
            {
                return (shotTimer >= WeaponFireDelay);
            }
        }
        static public bool CanDropBomb
        {
            get
            {
                return (bombTimer >= bombMinTimer);
            }
        }
        private static bool CanDetonateBomb
        {
            get
            {
                return (detonateTimer >= detonateMinTimer);
            }
        }

        public static void FireWeapon(Vector2 location, Vector2 velocity)//!creates bullets
        {
            switch (CurrentWeaponType)
            {
            case WeaponType.Normal:
                AddShot(location, velocity, 0);
                break;
            case WeaponType.Triple:
                AddShot(location, velocity, 0);
                float baseAngle = (float)Math.Atan2(velocity.Y, velocity.X);
                float offset = MathHelper.ToRadians(15);//15 is split angle
                AddShot(location, 
                    new Vector2((float)Math.Cos(baseAngle - offset),
                        (float)Math.Sin(baseAngle - offset)) * velocity.Length(), 0);
                AddShot(location,
                    new Vector2((float)Math.Cos(baseAngle + offset),
                        (float)Math.Sin(baseAngle + offset)) * velocity.Length(), 0);
                break;
            case WeaponType.Rocket:
                AddShot(location, velocity, 1);
                break;
            }
            Game1.soundEffectBank["bang"].Play();
            bouncesLeft += bouncesForWeapon;
            shotTimer = 0.0f;
        }

        static public void DropBomb(Vector2 location)
        {
            AddBomb(location);
            bombTimer = 0.0f;
        }

        private static void DetonateBomb(float elapsed, Sprite bomb)
        {
            detonateTimer += elapsed;
            if (detonateTimer >= detonateMinTimer)
            {
                bomb.Expired = true;
                createLargeExplosion(bomb.WorldCenter);
                checkExplosionSplashDamage(bomb.WorldCenter);
                //CheckBreakingWall(bomb.WorldCenter);//!
                detonateTimer = 0.0f;
            }

        }


        private static void checkWeaponUpgradeExpire(float elapsed)
        {
            if (CurrentWeaponType != WeaponType.Normal)
            {
                WeaponTimeRemaining -= elapsed;
                if (WeaponTimeRemaining <= 0)
                {
                    CurrentWeaponType = WeaponType.Normal;
                }
            }
        }




        private static void checkShotWallImpacts(Sprite shot)
        {
            if (shot.Expired)
            {
                return;
            }

            if (TileMap.IsWallTile(TileMap.GetSquareAtPixel(shot.WorldCenter)))//!
            {
                if (bouncesLeft > 0)
                {
                    //Ricochet shots
                    if (shot.Velocity.Y < 0 && shot.Velocity.X > 0)
                    {
                        shot.Velocity *= new Vector2(-1, -1);
                    }
                    if (shot.Velocity.Y < 0 && shot.Velocity.X < 0)
                    {
                        shot.Velocity *= new Vector2(-1, 1);
                    }
                    if (shot.Velocity.Y > 0 && shot.Velocity.X > 0)
                    {
                        shot.Velocity *= new Vector2(1, -1);
                    }
                    if (shot.Velocity.Y > 0 && shot.Velocity.X < 0)
                    {
                        shot.Velocity *= new Vector2(-1, 1);
                    }
                    else
                    {
                        shot.Velocity *= new Vector2(-1, -1);
                    }
                    bouncesLeft--;
                }
                else if(bouncesLeft <= 0)
                {
                    shot.Expired = true;    //Expire the shot
                    //Add a new Spark effect at the location of the shot
                    EffectsManager.AddSparksEffect(shot.WorldCenter, shot.Velocity);
                }
            }

            
        }



        private static void checkShotEnemyImpacts(Sprite shot)
        {
            if (shot.Expired)
            {
                return;
            }

            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (!enemy.Destroyed)
                {
                    if (shot.IsCircleColliding(
                        enemy.EnemyBase.WorldCenter, 
                        enemy.EnemyBase.CollisionRadius))
                    {
                        shot.Expired = true;          
                        enemy.Destroyed = true;         

                        GameManager.Score += 10;        

                        EffectsManager.AddExplosion(
                            enemy.EnemyBase.WorldCenter,
                            enemy.EnemyBase.Velocity / 30);
                    }
                }
            }
        }

        
        private static void checkExplosionSplashDamage(Vector2 location)
        {
            int explosionSplashRadius = 40;

            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (!enemy.Destroyed)
                {
                    if (enemy.EnemyBase.IsCircleColliding(location, explosionSplashRadius))
                    {
                        enemy.Destroyed = true;

                        GameManager.Score += 10;         

                        EffectsManager.AddExplosion(enemy.EnemyBase.WorldCenter, Vector2.Zero);
                    }
                }
            }

            Point a = Game1.ConvertToGrid(new Point((int)location.X, (int)location.Y));

            if (TileMap.mapSquares[a.X, a.Y] == TileMap.WeakWallTileEnd)
            {

            }
        }

        static public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shotTimer += elapsed;
            bombTimer += elapsed;

            checkWeaponUpgradeExpire(elapsed);


            for (int x = Shots.Count - 1; x >= 0; x--)
            {
                Shots[x].Update(gameTime);            
                checkShotWallImpacts(Shots[x]);            
                
                checkShotEnemyImpacts(Shots[x]);
                
                if (Shots[x].Expired)                    
                {
                    Shots.RemoveAt(x);
                }
            }
            for (int x = bombList.Count - 1; x >= 0; x--)
            {
                bombList[x].Update(gameTime);
                DetonateBomb(elapsed, bombList[x]);

                if (bombList[x].Expired)
                {
                    bombList.RemoveAt(x);
                }

            }
        }


        static public void Draw(SpriteBatch spriteBatch, Color colorTint)
        {
            foreach (Particle sprite in Shots)
            {
                sprite.Draw(spriteBatch, colorTint);
            }
            foreach (Sprite sprite in bombList)
            {
                sprite.Draw(spriteBatch, Color.Yellow);
            }
        }

    }
}
