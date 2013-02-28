using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VGDevLib
{
    public struct Spawn
    {
        public int ID;
        public int number;
        public int delay;
        public int spawnPoint;

        private Spawn(int i, int n, int d, int s)
        {
            ID = i;
            number = n;
            delay = d;
            spawnPoint = s;
        }

        public Spawn generateCopy()
        {
            return new Spawn(ID, number, delay, spawnPoint);
        }
    }
}
