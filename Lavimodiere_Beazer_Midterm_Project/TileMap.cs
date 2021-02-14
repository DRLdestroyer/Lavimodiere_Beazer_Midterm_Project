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
        public const int FloorTileEnd = 1;

        //wall sprite range (in list)
        public const int WallTileStart = FloorTileEnd+1;
        public const int WallTileEnd = 2;

        //wall sprite range (in list)
        public const int WeakWallTileStart = WallTileEnd+1;
        public const int WeakWallTileEnd = 3;
        static public List<Sprite> weakWall = new List<Sprite>();

        //door sprite range (in list)
        public const int DoorTileStart = WeakWallTileEnd+1;
        public const int DoorTileEnd = 4;
        static public List<Point> doorTileLoc = new List<Point>();

        static private Texture2D texture;

        static private List<Rectangle> tiles = new List<Rectangle>();
        static public List<Color> tileColors = new List<Color>();//! when all enemies killed, change level color

        static public int[,] mapSquares = new int[MapWidth, MapHeight];

        private static Random rand = Game1.rand;

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
            int wallChancePerSquare = 15;
            int floorTile = rand.Next(FloorTileStart, FloorTileEnd + 1);
            int wallTile = rand.Next(WallTileStart, WallTileEnd + 1);
            int weakWallTile = rand.Next(WeakWallTileStart, WeakWallTileEnd + 1);
            int doorTile = rand.Next(DoorTileStart, DoorTileEnd + 1);

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    mapSquares[x, y] = floorTile;

                    if ((x == 0) || (y == 0) || (x == MapWidth - 1) || (y == MapHeight - 1))//world border
                    {
                        mapSquares[x, y] = wallTile;
                        continue;//move onto the next loop iteration
                    }

                    //when not on border and inside wall (0-100)
                    if (rand.Next(0, 100) <= wallChancePerSquare)
                    {
                        if (Game1.rand.Next(1, 3) == 1)
                        {
                            mapSquares[x, y] = weakWallTile;
                        }
                        else
                            mapSquares[x, y] = wallTile;
                        
                    }

                    //set door locations
                    if ((x == (MapWidth / 2 - MapWidth / 4)) && (y == (MapHeight / 2)))//lower door location
                    {
                        mapSquares[x, y] = doorTile;
                        doorTileLoc.Add(new Point(x, y));
                        continue;
                    }
                    if ((x == (MapWidth / 2)) && (y == (MapHeight / 2 - MapWidth / 4)))//right door location
                    {
                        mapSquares[x, y] = doorTile;
                        doorTileLoc.Add(new Point(x, y));
                        continue;
                    }
                    if ((x == (MapWidth / 2 + MapWidth / 4)) && (y == (MapHeight / 2)))//right lower door location
                    {
                        mapSquares[x, y] = doorTile;
                        doorTileLoc.Add(new Point(x, y));
                        continue;
                    }

                    if ((x == (MapWidth / 2) / (MapWidth / 2)) && (y == (MapHeight / 2) / (MapHeight / 2)))//player spawn location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }

                    //clear front of doors
                    if ((x == (MapWidth / 2 - MapWidth / 4)) && (y == (MapHeight / 2)-1))//lower door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }
                    if ((x == (MapWidth / 2 - MapWidth / 4)) && (y == (MapHeight / 2) + 1))//lower door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }
                    if ((x == (MapWidth / 2)-1) && (y == (MapHeight / 2 - MapWidth / 4)))//right door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }
                    if ((x == (MapWidth / 2) + 1) && (y == (MapHeight / 2 - MapWidth / 4)))//right door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }
                    if ((x == (MapWidth / 2 + MapWidth / 4)-1) && (y == (MapHeight / 2)))//right lower door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }
                    if ((x == (MapWidth / 2 + MapWidth / 4) + 1) && (y == (MapHeight / 2)))//right lower door location
                    {
                        mapSquares[x, y] = floorTile;
                        continue;
                    }


                    //set quadrant borders
                    if ((x == (MapWidth / 2)) || (y == (MapHeight / 2)))
                    {
                        mapSquares[x, y] = wallTile;
                        continue;
                    }

                    
                }
            }

        }

        static public void Initialize(Texture2D tileTexture)
        {
            texture = tileTexture;

            tiles.Clear();
            tileColors.Clear();
            //floor sprite(s)
            tiles.Add(new Rectangle(0, 0, TileWidth, TileHeight));//0
            tileColors.Add(Game1.RandomColor());
            tiles.Add(new Rectangle(32, 0, TileWidth, TileHeight));//1
            tileColors.Add(Game1.RandomColor());
            //wall sprite(s)
            tiles.Add(new Rectangle(64, 0, TileWidth, TileHeight));//2
            tileColors.Add(Game1.RandomColor());
            //weak wall sprite(s)
            tiles.Add(new Rectangle(96, 0, TileWidth, TileHeight));//3
            tileColors.Add(Game1.RandomColor());
            //door sprite(s)
            tiles.Add(new Rectangle(128, 0, TileWidth, TileHeight));//4
            tileColors.Add(Game1.RandomColor());

            GenerateRandomMap();
            //Sprite e = new Sprite(new Vector2(x, y), texture,
            //    new Rectangle(
            //        new Point(96, 0),
            //        new Point(32, 32)
            //    ), Vector2.Zero);
            //e.Animate = false;
            //e.CollisionRadius = 14;

            //weakWall.Add(e);
        }

        static public void Update(GameTime gameTime)
        {
            for(int x = weakWall.Count - 1; x >= 0; x--)
            {
                weakWall[x].Update(gameTime);
                if (weakWall[x].Expired)
                {
                    weakWall.RemoveAt(x);
                }
            }
            
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetSquareByPixelX((int)Camera.Position.X);
            int startY = GetSquareByPixelY((int)Camera.Position.Y);

            int endX = GetSquareByPixelX((int)Camera.Position.X +
            Camera.ViewPortWidth);
            int endY = GetSquareByPixelY((int)Camera.Position.Y +
            Camera.ViewPortHeight);

            foreach(Sprite wall in weakWall)
            {
                wall.Draw(spriteBatch, Color.White);
            }

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                    {
                        int a = mapSquares[x, y];//!
                        if (a > tileColors.Count)//if over limit(error case)
                        {
                            a = 0;
                        }
                        
                        spriteBatch.Draw(
                        texture,
                        SquareScreenRectangle(x, y),
                        tiles[GetTileAtSquare(x, y)],
                        tileColors[a]);
                    }
                }
        }

        static public void RandomizeTileColors()
        {
            for (int i = 0; i < tileColors.Count; i++)
            {
                tileColors[i] = Game1.RandomColor();//set random color on level clear
            }
        }
    }
}
