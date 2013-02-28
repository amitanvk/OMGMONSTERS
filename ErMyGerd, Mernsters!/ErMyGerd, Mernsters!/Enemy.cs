using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    public abstract class Enemy : Entity
    {
        protected Texture2D statBarBase;
        protected Texture2D hpBar;
        protected Texture2D mpBar;
        protected bool showHPBar = true;

        public Enemy(Vector2 p) : base()
        {
            Position = p;
            generateHPBarTextures();
        }

        public override void loadContent()
        {
            hpBar = Global.Textures["Health Bar"];
            mpBar = Global.Textures["Mana Bar"];
        }

        public override void onSpawn()
        {
            Global.Enemies.Add(this);
            Faction = Faction.Enemy;
        }

        public override void onDeath()
        {
            int drop = Global.RNG.Next(4);
            switch(drop)
            {
                case 0:
                    new Potion(Position);
                    break;
                case 1:
                    new ManaPickup(Position);
                    break;
            }
            Global.Enemies.Remove(this);
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            base.DrawAfter(sb);
           if (showHPBar)
            {
                sb.Draw(mpBar,
                    Position - new Vector2(-1, SpriteSize.Y / 2 - 1),
                    new Rectangle(0,0,
                        (int)((float)statBarBase.Width * ((float)MP / (float)MaximumMP)) - 2, statBarBase.Height - 2),
                    Color.White,
                    0f,
                    new Vector2(statBarBase.Width / 2, statBarBase.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
                sb.Draw(hpBar,
                    Position - new Vector2(-1, SpriteSize.Y / 2 + statBarBase.Height - 1),
                    new Rectangle(0, 0, 
                        (int)((float)statBarBase.Width * ((float)HP / (float)MaximumHP)) - 2, statBarBase.Height - 2),
                    Color.White,
                    0f,
                    new Vector2(statBarBase.Width / 2, statBarBase.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
               }
        }

        protected override void DrawBefore(SpriteBatch sb)
        {
            base.DrawBefore(sb);
            if (showHPBar)
            {
                sb.Draw(statBarBase,
                    Position - new Vector2(0, SpriteSize.Y / 2),
                    statBarBase.Bounds,
                    Color.White,
                    0f,
                    new Vector2(statBarBase.Width / 2, statBarBase.Height / 2),
                    1f,
                    SpriteEffects.None,
                    1f);
                sb.Draw(statBarBase,
                    Position - new Vector2(0, SpriteSize.Y / 2 + statBarBase.Height),
                    statBarBase.Bounds,
                    Color.White,
                    0f,
                    new Vector2(statBarBase.Width / 2, statBarBase.Height / 2),
                    1f,
                    SpriteEffects.None,
                    1f);
            }
        }

        public void generateHPBarTextures()
        {
            Color[] c = new Color[((int)SpriteSize.X + 2) * 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < (int)SpriteSize.X + 2; j++)
                    switch (i)
                    {
                        case 0:
                        case 3:
                            c[(i * ((int)SpriteSize.X + 2)) + j] = Color.FromNonPremultiplied(new Vector4(0, 0, 0, 1));
                            break;
                        case 1:
                        case 2:
                            if (j == 0 || j == ((int)SpriteSize.X + 1))
                                c[(i * ((int)SpriteSize.X + 2)) + j] = Color.FromNonPremultiplied(new Vector4(0, 0, 0, 1));
                            else
                                c[(i * ((int)SpriteSize.X + 2)) + j] = Color.FromNonPremultiplied(new Vector4(255, 0, 0, 1));
                            break;
                        default:
                            c[(i * ((int)SpriteSize.X + 2)) + j] = Color.FromNonPremultiplied(new Vector4(0, 255, 0, 1));
                            break;
                    }
            statBarBase = new Texture2D(Global.Graphics.GraphicsDevice, (int)SpriteSize.X + 2, 4, false, SurfaceFormat.Color);
            statBarBase.SetData<Color>(c);
        }

        public override void Reset()
        {
            //Leave for Garbage Collection
        }
    }

    public abstract class PathBoundEnemy : Enemy
    {
        public Stack<MapHandler.Tile> path;
        public MapHandler.Tile currentTarget;
        private float baseSpeed = 1;
        protected float BaseSpeed
        {
            get { return baseSpeed; }
            set { baseSpeed = value; }
        }

        public PathBoundEnemy(Vector2 p) : base(p)
        {
            mapBound = true;
            updatePath();
            if(path != null && path.Count > 0)
                currentTarget = path.Pop();
        }

        public abstract void updatePath();

        public override void onUpdate(GameTime gt)
        {
            if (path == null || path.Count <= 0)
            {
                updatePath();
                if (path != null && path.Count > 0)
                     currentTarget = path.Pop();
            }
            if (currentTarget != null)
            {
                moveTowardPoint(Global.MapHandler.findPosition(currentTarget.toPoint()), baseSpeed);
                Point location = Util.vectorToPoint(Global.MapHandler.findTileLoc(Position));
                if (location == currentTarget.toPoint() && path.Count > 0)
                {
                    currentTarget = path.Pop();
                }
            }
        }
    }
}
