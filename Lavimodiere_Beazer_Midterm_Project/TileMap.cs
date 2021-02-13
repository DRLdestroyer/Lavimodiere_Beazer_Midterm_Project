using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lavimodiere_Beazer_Midterm_Project
{
    static class TileMap
    {
        public const int TileWidth = 32;//Width of tile
        public const int TileHeight = 32;//Height of tile
        public const int MapWidth = 50;//Width of Map in tiles
        public const int MapHeight = 50;//Height of Map in tiles

        //floor sprite range (in list)
        public const int FloorTileStart = 0;
        public const int FloorTileEnd = 2;

        //wall sprite range (in list)
        public const int WallTileStart = FloorTileEnd;
        public const int WallTileEnd = 3;

        //wall sprite range (in list)
        public const int WeakWallTileStart = FloorTileEnd;
        public const int WeakWallTileEnd = 3;

        //door sprite range (in list)
        public const int DoorTileStart = WeakWallTileEnd;
        public const int DoorTileEnd = 5;

        static private Texture2D texture;

        static private List<Rectangle> tiles = new List<Rectangle>();

        static private int[,] mapSquares = new int[MapWidth, MapHeight];
        
        static private Random rand = new Random();

        static public void Initialize(Texture2D tileTexture)
        {
            texture = tileTexture;

            tiles.Clear();
            //floor sprite(s)
            tiles.Add(new Rectangle(0, 0, TileWidth, TileHeight));//0
            tiles.Add(new Rectangle(32, 0, TileWidth, TileHeight));//1
            //wall sprite(s)
            tiles.Add(new Rectangle(64, 0, TileWidth, TileHeight));//2
            //weak wall sprite(s)
            tiles.Add(new Rectangle(96, 0, TileWidth, TileHeight));//3
            //door sprite(s)
            tiles.Add(new Rectangle(128, 0, TileWidth, TileHeight));//4

            GenerateRandomMap();

        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetSquareByPixelX((int)Camera.Position.X);
            int startY = GetSquareByPixelY((int)Camera.Position.Y);

            int endX = GetSquareByPixelX((int)Camera.Position.X +
            Camera.ViewPortWidth);
            int endY = GetSquareByPixelY((int)Camera.Position.Y +
            Camera.ViewPortHeight);


            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                    {
                        spriteBatch.Draw(
                        texture,
                        SquareScreenRectangle(x, y),
                        tiles[GetTileAtSquare(x, y)],
                        Color.White);
                    }
                }
        }

        #region Map Squares
        static public int GetSquareByPixelX(int pixelX)
        {
            return pixelX / TileWidth;  
        }

    
        static public int GetSquareByPixelY(int pixelY)
        {
            return pixelY / TileHeight;
        }
        

        static public Vector2 GetSquareAtPixel(Vector2 pixelLocation)
        {
            return new Vector2(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }


        static public Vector2 GetSquareCenter(int squareX, int squareY)
        {
            return new Vector2(
                (squareX * TileWidth) + (TileWidth / 2),             
                (squareY * TileHeight) + (TileHeight / 2));          
        }
        
        
        static public Vector2 GetSquareCenter(Vector2 square)
        {
            return GetSquareCenter((int)square.X, (int)square.Y);
        }
        

        static public Rectangle SquareWorldRectangle(int x, int y)
        {
            return new Rectangle(
                x * TileWidth,
                y * TileHeight,
                TileWidth,
                TileHeight);
        }


        static public Rectangle SquareWorldRectangle(Vector2 square)
        {
            return SquareWorldRectangle((int)square.X, (int)square.Y);
        }


        static public Rectangle SquareScreenRectangle(int x, int y)
        {
            return Camera.Transform(SquareWorldRectangle(x, y));
        }

        static public Rectangle SquareScreenRectangle(Vector2 square)
        {
            return SquareScreenRectangle((int)square.X, (int)square.Y);
        }

        #endregion

        #region Map Tiles
        static public int GetTileAtSquare(int tileX, int tileY)
        {
            if ((tileX >= 0) && (tileX < MapWidth) && (tileY >= 0) && (tileY < MapHeight))
            {
                return mapSquares[tileX, tileY];
            }
            else
            {
                return -1;            
            }
        }


        static public void SetTileAtSquare(int tileX, int tileY, int tile)
        {
            if ((tileX >= 0) && (tileX < MapWidth) && (tileY >= 0) && (tileY < MapHeight))
            {
                mapSquares[tileX, tileY] = tile;           
            }
        }


        static public int GetTileAtPixel(int pixelX, int pixelY)
        {
            return GetTileAtSquare(GetSquareByPixelX(pixelX), GetSquareByPixelY(pixelY));
        }


        static public int GetTileAtPixel(Vector2 pixelLocation)
        {
            return GetTileAtPixel((int)pixelLocation.X, (int)pixelLocation.Y);
        }


        static public bool IsWallTile(int tileX, int tileY)
        {
            int tileIndex = GetTileAtSquare(tileX, tileY);

            if (tileIndex == -1)
            {
                return false;    
            }

            return tileIndex >= WallTileStart;           
        }
        

        static public bool IsWallTile(Vector2 square)
        {
            return IsWallTile((int)square.X, (int)square.Y);
        }
        
        
        static public bool IsWallTileByPixel(Vector2 pixelLocation)
        {
            return IsWallTile(
            GetSquareByPixelX((int)pixelLocation.X),
            GetSquareByPixelY((int)pixelLocation.Y));
        }
        
        #endregion

        static public void GenerateRandomMap()
        {
            int wallChancePerSquare = 10;
            int floorTile = rand.Next(FloorTileStart, FloorTileEnd + 1);
            int wallTile = rand.Next(WallTileStart, WallTileEnd + 1);
            
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    mapSquares[x, y] = floorTile;

                    if ((x == 0) || (y == 0) || (x == MapWidth - 1) || (y == MapHeight - 1))
                    {
                        mapSquares[x, y] = wallTile;
                        continue;        
                    }

                    if ((x == 1) || (y == 1) || (x == MapWidth - 2) || (y == MapHeight - 2))
                    {
                        continue;        
                    }

                    if (rand.Next(0, 100) <= wallChancePerSquare)
                    {
                        mapSquares[x, y] = wallTile;
                    }
                }
            }
        }

    }
}
