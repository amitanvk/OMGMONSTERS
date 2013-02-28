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

namespace ErMyGerdMernsters
{
    public class Hitbox
    {
        public Point center;
        private Faction fac;
        public int radius;
        private bool attack;
        private Color factionColor;

        public Hitbox(int x, int y, int r, Faction f, bool atk)
        {
            center = new Point(x, y);
            radius = r;
            fac = f;
            attack = atk;
            switch (fac)
            {
                case Faction.Enemy:
                    factionColor = Color.Red;
                    break;
                case Faction.Neutral:
                    factionColor = Color.Yellow;
                    break;
                case Faction.Player:
                    factionColor = Color.Green;
                    break;
            }
        }

        public Hitbox(Vector2 vector, int r, Faction play, bool atk) : this((int)vector.X, (int)vector.Y, r, play, atk) { }

        public Hitbox(Point point, int r, Faction play, bool atk) : this(point.X, point.Y, r, play, atk) { }
        
        public void update(int x, int y, int r)
        {
            center.X = x;
            center.Y = y;
            radius = r;
        }

        public void update(Vector2 vector, int r)
        {
            update((int)vector.X, (int)vector.Y, r);
        }

        public void update(Point point, int r)
        {
            update(point.X, point.Y, r);
        }

        public static bool collisionCheck(Hitbox h1, Hitbox h2)
        {
            return ((int)Util.distance(h1.center, h2.center) < h1.radius + h2.radius);
        }

        public void draw(SpriteBatch sb)
        {
            if (Global.Debug.HITBOX_SHOW)
                Util.CreateCircle(center, radius, factionColor, sb);
        }
    }
}
