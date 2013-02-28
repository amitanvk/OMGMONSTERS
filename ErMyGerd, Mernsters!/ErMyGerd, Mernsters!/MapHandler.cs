using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//xTile engine references
using xTile;
using xTile.Dimensions;
using xTile.Display;

namespace ErMyGerdMernsters
{
    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public class MapHandler : DrawableGameElement
    {
        private IDisplayDevice mapDisplayDevice;
        public xTile.Dimensions.Rectangle viewPort;
        public List<SpawnPoint> spawnPoints;
        public static Map map;
        public bool[,] characterSolid;
        public bool[,] projectileSolid;
        private int mapIDLoaded;
        private int tileHeight, tileWidth;
        private static Tile[,] mapAsTile;
        private int screenWidth;
        private int screenHeight;
        private Point playerSpawnPoint;

        public override void Reset()
        {
            mapIDLoaded = -1;

            spawnPoints = null;
            characterSolid = null;
            projectileSolid = null;
            map = null;
            mapAsTile = null;
        }

        /// <summary>
        /// TO-DO: Comment Constructor
        /// </summary>
        public MapHandler(ContentManager content)
        {             
            mapDisplayDevice = new XnaDisplayDevice(content, Global.Graphics.GraphicsDevice);
            viewPort = new xTile.Dimensions.Rectangle(new Location(200,200), new Size(Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight));
            screenHeight = viewPort.Height;
            screenWidth = viewPort.Width;
            RenderLimitBound = false;
            mapIDLoaded = -1;
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="mapID"></param>
        public void loadMap(int mapID)
        {
            spawnPoints = new List<SpawnPoint>();
            mapIDLoaded = mapID;
            map = Global.Maps["Level " + mapID];
            map.LoadTileSheets(mapDisplayDevice);
            xTile.Layers.Layer collisionLayer = map.GetLayer("Collision Layer");
            tileWidth = collisionLayer.TileWidth;
            tileHeight = collisionLayer.TileHeight;
            characterSolid = new bool[collisionLayer.LayerWidth, collisionLayer.LayerHeight];
            projectileSolid = new bool[collisionLayer.LayerWidth, collisionLayer.LayerHeight];
            for (int x = 0; x < collisionLayer.LayerWidth; x++)
            {
                for (int y = 0; y < collisionLayer.LayerHeight; y++)
                {
                    if (collisionLayer.Tiles[x, y] == null)
                    {
                        characterSolid[x, y] = false;
                        projectileSolid[x, y] = false;
                        continue;
                    }
                    int tileIndex = collisionLayer.Tiles[x, y].TileIndex;
                    switch (tileIndex)
                    {
                        case 0:
                            characterSolid[x, y] = false;
                            projectileSolid[x, y] = false;
                            break;
                        case 1:
                            characterSolid[x, y] = true;
                            projectileSolid[x, y] = false;
                            break;
                        case 2:
                            characterSolid[x, y] = false;
                            projectileSolid[x, y] = false;
                            spawnPoints.Add(new SpawnPoint(x, y));
                            break;
                        case 3:
                            characterSolid[x, y] = false;
                            projectileSolid[x, y] = true;
                            break;
                        case 4:
                            characterSolid[x, y] = true;
                            projectileSolid[x, y] = true;
                            break;
                        case 5:
                            characterSolid[x, y] = false;
                            projectileSolid[x, y] = false;
                            playerSpawnPoint = new Point(x, y);
                            break;
                    }
                }
            }
            Global.Camera.Position = findPosition(playerSpawnPoint);
            Global.Player.Position = findPosition(playerSpawnPoint);

            SetMapAsTiles();
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        private String generateMapFileName(int mapID)
        {
            return "Maps\\Map " + mapID;
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gt"></param>
        public override void Update(GameTime gt)
        {
            map.Update(gt.ElapsedGameTime.Milliseconds);
            viewPort.Size.Height = Global.Graphics.PreferredBackBufferHeight;
            viewPort.Size.Width = Global.Graphics.PreferredBackBufferWidth;
            Vector2 center = new Vector2 { X = viewPort.Size.Width / 2, Y = viewPort.Size.Height / 2};
            Vector2 targetLoc = Global.Player.Position;
            if (targetLoc.X - center.X < 0)
                targetLoc.X = center.X;
            if (targetLoc.Y - center.Y < 0)
                targetLoc.Y = center.Y;
            if (targetLoc.X + center.X > map.DisplayWidth)
                targetLoc.X = map.DisplayWidth - center.X;
            if (targetLoc.Y + center.Y > map.DisplayHeight)
                targetLoc.Y = map.DisplayHeight - center.Y;
            Global.Camera.Position = targetLoc;
            viewPort.Location.X = (int)(Global.Camera.Position.X - center.X);
            viewPort.Location.Y = (int)(Global.Camera.Position.Y - center.Y);
            int i;
            for (i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].Update(gt);
            }
            for (i = 0; i < Global.Projectiles.Count; i++)
            {
                Projectile projectile = Global.Projectiles[i];
                Vector2 mapLoc = findTileLoc(projectile.Position);
                try
                {
                    if (projectileSolid[(int)mapLoc.X, (int)mapLoc.Y])
                    {
                        projectile.exploding = true;
                    }
                }
                catch (IndexOutOfRangeException j)
                {
                    Global.Projectiles.Remove(projectile);
                }
            }
            for (i = 0; i < Global.Enemies.Count; i++)
            {
                Enemy enemy = Global.Enemies[i];
                Vector2 mapLoc = findTileLoc(enemy.Position);
                try
                {
                    if (characterSolid[(int)mapLoc.X, (int)mapLoc.Y] && enemy.mapBound)
                    {
                        enemy.onDeath();
                    }
                }
                catch (IndexOutOfRangeException j)
                {
                    enemy.onDeath();
                }
            }
            for (i = 0; i < Global.Pickups.Count; i++)
            {
                Pickup pickup = Global.Pickups[i];
                Vector2 mapLoc = findTileLoc(pickup.Position);
                if (characterSolid[(int)mapLoc.X, (int)mapLoc.Y])
                {
                    Global.Pickups.Remove(pickup);
                }
                try
                {
                    if (projectileSolid[(int)mapLoc.X, (int)mapLoc.Y])
                    {
                        Global.Pickups.Remove(pickup);
                    }
                }
                catch (IndexOutOfRangeException j)
                {
                    Global.Pickups.Remove(pickup);
                }
            }
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public Vector2 findTileLoc(Vector2 screenPos)
        {
            return new Vector2(screenPos.X / tileWidth, screenPos.Y / tileHeight);
        }

        public Vector2 findTileLoc(Point screenPos)
        {
            return new Vector2(screenPos.X / tileWidth, screenPos.Y / tileHeight);
        }

        public Vector2 findPosition(Vector2 mapLoc)
        {
            return new Vector2((mapLoc.X + 0.5f) * tileWidth, (mapLoc.Y  + 0.5f) * tileHeight);
        }

        public Vector2 findPosition(Point mapLoc)
        {
            return new Vector2((mapLoc.X + 0.5f) * tileWidth, (mapLoc.Y + 0.5f) * tileHeight);
        }

        public void spawnEnemy(int EnemyID, int spawnPoint)
        {
            if (spawnPoint < 0)
                spawnPoint = 0;
            if (spawnPoint >= spawnPoints.Count)
                spawnPoint = spawnPoints.Count - 1;
            spawnPoints[spawnPoint].spawn(EnemyID);
        }

        public void spawnEnemyAtEveryPoint(int EnemyID)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
                spawnEnemy(EnemyID, i);
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="sb"></param>
        protected override void DrawAfter(SpriteBatch sb)
        {
            map.Draw(mapDisplayDevice, viewPort);
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].Draw(sb);
            }
        }

        public Vector2 mapCollisionCheck(Vector2 velocity, Vector2 screenPos, Vector2 size)
        {
            if (velocity.X == 0 && velocity.Y == 0)
                return velocity;
            Vector2 TL = Vector2.Zero, TR = Vector2.Zero, BL = Vector2.Zero, BR = Vector2.Zero;
            Vector2 baseTL, baseTR, baseBL, baseBR;
            Vector2 yComponent = velocity * Vector2.UnitY;
            Vector2 xComponent = velocity * Vector2.UnitX;
            baseTL = screenPos + (size * -0.5f);
            baseTR = screenPos + (size * 0.5f) * new Vector2(1, -1) - new Vector2(1,0);
            baseBL = screenPos + (size * 0.5f) * new Vector2(-1, 1) - new Vector2(0,1);
            baseBR = screenPos + (size * 0.5f) - new Vector2(1,1);
            if (velocity.X != 0)
            {
                TR = findTileLoc(baseTR + xComponent);
                BR = findTileLoc(baseBR + xComponent);
                TL = findTileLoc(baseTL + xComponent);
                BL = findTileLoc(baseBL + xComponent);
                if (velocity.X > 0)
                {
                    if (characterSolid[(int)TR.X, (int)TR.Y])
                        velocity.X = CollisionCheckHelper(baseTR.X, tileWidth, false);
                    else if (characterSolid[(int)BR.X, (int)BR.Y])
                        velocity.X = CollisionCheckHelper(baseBR.X, tileWidth, false);
                }
                else
                {
                    if (characterSolid[(int)TL.X, (int)TL.Y])
                        velocity.X = CollisionCheckHelper(baseTL.X, tileWidth, true);
                    else if (characterSolid[(int)BL.X, (int)BL.Y])
                        velocity.X = CollisionCheckHelper(baseBL.X, tileWidth, true);
                }
            }
            if (velocity.Y != 0)
            {
                BR = findTileLoc(baseBR + yComponent);
                BL = findTileLoc(baseBL + yComponent);
                TR = findTileLoc(baseTR + yComponent);
                TL = findTileLoc(baseTL + yComponent);
                if (velocity.Y > 0)
                {
                    if (characterSolid[(int)BR.X, (int)BR.Y])
                        velocity.Y = CollisionCheckHelper(baseBR.Y, tileHeight, false);
                    else if (characterSolid[(int)BL.X, (int)BL.Y])
                        velocity.Y = CollisionCheckHelper(baseBL.Y, tileHeight, false);
                }
                else
                {
                    if (characterSolid[(int)TR.X, (int)TR.Y])
                        velocity.Y = CollisionCheckHelper(baseTR.Y, tileHeight, true);
                    else if (characterSolid[(int)TL.X, (int)TL.Y])
                        velocity.Y = CollisionCheckHelper(baseTL.Y, tileHeight, true);
                }
            }
            return velocity;
        }

        private float CollisionCheckHelper(float measure, int tileSize, bool negative)
        {
            float difference = measure % tileSize;
            return (difference == 0) ? 0 : ((negative) ? - difference : tileSize - difference - 1);
        }

        public void moveMap(Vector2 movementVector)
        {
            Global.Camera.Move(movementVector);
        }

        private void SetMapAsTiles()
        {
            if (mapAsTile == null)
                mapAsTile = new Tile[map.Layers[0].LayerWidth, map.Layers[0].LayerHeight];

            for (int x = 0; x < map.Layers[0].LayerWidth; x++)
                for (int y = 0; y < map.Layers[0].LayerHeight; y++)
                    mapAsTile[x,y] = new Tile(x, y, !characterSolid[x, y]);

            for (int x = 0; x < map.Layers[0].LayerWidth; x++)
                for (int y = 0; y < map.Layers[0].LayerHeight; y++)
                    mapAsTile[x,y].SetNeighbors(mapAsTile, new Vector2(tileWidth, tileHeight));
        }

        public Tile GetTile(Point loc)
        {
            return mapAsTile[loc.X, loc.Y];
        }

        public Stack<Tile> FindPath(Point start, Point end)
        {
            Tile a = mapAsTile[start.X, start.Y];
            Tile b = mapAsTile[end.X, end.Y];

            Queue<Tile> q = new Queue<Tile>();
            HashSet<Tile> p = new HashSet<Tile>();

            foreach (Tile t in mapAsTile)
            {
                t.Parent = null;
            }

            q.Enqueue(a);
            Tile found = null;
            while (q.Count > 0)
            {
                Tile check = q.Dequeue();

                if (check == b)
                {
                    found = check;
                    break;
                }

                foreach (Tile tile in check.neighbors)
                {
                    if (tile != null && !p.Contains(tile) && tile.passable)
                    {
                        tile.Parent = check;
                        tile.calculateH(b.X, b.Y);
                        q.Enqueue(tile);
                        p.Add(tile);
                    }
                }

                //Find the tile with the smallest F value
                int minF = int.MaxValue;
                int minG = int.MaxValue;
                foreach (Tile t in q)
                {
                    if (t.F < minF)
                    {
                        check = t;
                        minF = t.F;
                        minG = t.G;
                    }
                    else if (t.F == minF && t.G < minG)
                    {
                        check = t;
                        minG = t.G;
                    }
                }
            }

            Stack<Tile> path = new Stack<Tile>();
            Tile currentTile = found;
            if (currentTile == null)
                return path;
            while (currentTile != a)
            {
                path.Push(currentTile);
                if (currentTile.Parent == null)
                    break;
                else
                    currentTile = currentTile.Parent;
            }

            return path;
            
        }

        public class Tile
        {
            private int posX;
            private int posY;
            private int g;
            private int h;
            public Tile[] neighbors;
            private Tile parent;
            public bool passable;

            public Tile(int x, int y, bool p)
            {
                posX = x;
                posY = y;
                passable = p;
            }

            public void calculateG(int previousG)
            {
                g = previousG + 1;
            }

            public void calculateH(int x, int y)
            {
                h = Math.Abs(posX - x) + Math.Abs(posY - y);
            }

            public void SetNeighbors(Tile[,] a, Vector2 tileSize)
            {
                neighbors = new Tile[8];

                if(posX + 1 < map.Layers[0].LayerWidth)
                    neighbors[0] = a[posX + 1, posY];

                if (posY + 1 < map.Layers[0].LayerHeight)
                    neighbors[1] = a[posX, posY + 1];

                if(posX - 1 >= 0)
                    neighbors[2] = a[posX - 1, posY];

                if (posY - 1 >= 0)
                    neighbors[3] = a[posX, posY - 1];


                if (posX + 1 < map.Layers[0].LayerWidth && posY + 1 < map.Layers[0].LayerHeight)
                    neighbors[4] = a[posX + 1, posY + 1];

                if (posX - 1 >= 0 && posY + 1 < map.Layers[0].LayerHeight)
                    neighbors[5] = a[posX - 1, posY + 1];

                if (posX + 1 < map.Layers[0].LayerWidth && posY - 1 >= 0)
                    neighbors[6] = a[posX + 1, posY - 1];

                if (posX - 1 >= 0 && posY - 1 >= 0)
                    neighbors[7] = a[posX - 1, posY - 1];

            }

            public Point toPoint()
            {
                return new Point(posX, posY);
            }

            public int X
            {
                get { return posX; }
            }

            public int Y
            {
                get { return posY; }
            }

            public int G
            {
                get { return g; }
            }

            public int H
            {
                get { return h; }
            }

            public int F
            {
                get { return G + H; }
            }

            public Tile Parent
            {
                get { return parent; }
                set
                {
                    parent = value;
                    if(parent != null)
                        calculateG(parent.G);
                }
            }
        }

        public static Tile MapTile(int x, int y)
        {
            return mapAsTile[x, y];
        }
    }
}
