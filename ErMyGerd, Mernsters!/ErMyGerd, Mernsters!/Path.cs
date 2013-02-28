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
    public class MapPath
    {
        private List<Point> points;
        private int lastPoint;

        public MapPath(PathElement end)
        {
            points = new List<Point>();
            PathElement currentElement = end;
            while (currentElement != null)
            {
                points.Add(currentElement.Loc);
                currentElement = currentElement.Parent;
            }
            points.Reverse();
            lastPoint = 0;
        }

        public Point getNextPoint()
        {
            return points[lastPoint++];
        }

        public bool IsDone
        {
            get { return lastPoint > points.Count; }
        }
    }

    public class PathElement
    {
        private int _g;
        private int _h;
        private PathElement _parent;
        private Point _loc;
        private Point target;

        public static int Compare(PathElement x, PathElement y)
        {
            if (x.F < y.F)
                return 1;
            else if (x.F == y.F)
                return 0;
            else
                return -1;
        }

        public PathElement(PathElement p, Point loc, Point t)
        {
            _h = Math.Abs(_loc.X - target.X) + Math.Abs(_loc.Y - target.Y);
            _parent = p;
            if (_parent == null)
                _g = 1;
            else
                _g = _parent.G + 1;
            _loc = loc;
            target = t;
        }

        public PathElement Parent
        {
            get { return _parent; }
        }

        public Point Loc
        {
            get { return _loc; }
        }

        public bool AtTarget
        {
            get { return _loc == target; }
        }

        public int G
        {
            get { return _g; }
        }

        public int H
        {
            get { return _h; }
        }

        public int F
        {
            get { return G + H; }
        }
    }
}
