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
        static public int bouncesLeft = 0;//!


        static public WeaponType CurrentWeaponType = WeaponType.Normal;
        static public float WeaponTimeRemaining = 30.0f;
        static private float weaponTimeDefault = 30.0f;


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

        private static void createLargeExplosion(Vector2 location)
        {
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

            shotTimer = 0.0f;     
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

            if (TileMap.IsWallTile(TileMap.GetSquareAtPixel(shot.WorldCenter)))
            {
                shot.Expired = true;      
                if (shot.Frame == 0)           
                {
                    EffectsManager.AddSparksEffect(shot.WorldCenter, shot.Velocity);
                }
                else
                {
                    createLargeExplosion(shot.WorldCenter);     
                    
                    checkRocketSplashDamage(shot.WorldCenter);
                }
            }

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
                        
                        if (shot.Frame == 0)
                        {
                            EffectsManager.AddExplosion(
                                enemy.EnemyBase.WorldCenter,
                                enemy.EnemyBase.Velocity / 30);
                        }
                        else
                        {
                            if (shot.Frame == 1)
                            {
                                createLargeExplosion(shot.WorldCenter);
                                checkRocketSplashDamage(shot.WorldCenter);
                            }
                        }
                    }
                }
            }
        }

        
        private static void checkRocketSplashDamage(Vector2 location)
        {
            int rocketSplashRadius = 40;

            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (!enemy.Destroyed)
                {
                    if (enemy.EnemyBase.IsCircleColliding(location, rocketSplashRadius))
                    {
                        enemy.Destroyed = true;

                        GameManager.Score += 10;         

                        EffectsManager.AddExplosion(enemy.EnemyBase.WorldCenter, Vector2.Zero);
                    }
                }
            }
        }

        static public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shotTimer += elapsed;

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
        }


        static public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle sprite in Shots)
            {
                sprite.Draw(spriteBatch);
            }
        }

    }
}
