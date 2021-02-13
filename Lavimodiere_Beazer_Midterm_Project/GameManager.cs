using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lavimodiere_Beazer_Midterm_Project
{
    static class GameManager
    {
        public static int Score = 0;
        public static int CurrentWave = 0;
        public static Vector2 PlayerStartLoc = new Vector2(32, 32);

        public static void StartNewWave()
        {
            CurrentWave++;

            Player.BaseSprite.WorldLocation = PlayerStartLoc;
            Camera.Position = Vector2.Zero;
            WeaponManager.CurrentWeaponType = WeaponManager.WeaponType.Normal;
            
            WeaponManager.Shots.Clear();       
            EffectsManager.Effects.Clear();    
            EnemyManager.Enemies.Clear();      

            TileMap.GenerateRandomMap();
            EnemyManager.AddEnemiesForLevel(CurrentWave);
        }


        public static void StartNewGame()
        {
            CurrentWave = 0;
            Score = 0;
            StartNewWave();
            //EnemyManager.AddEnemiesForLevel(1);
        }

    }
}
