﻿using System;
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
    public class EnemyList : DrawableGameElementCollection<Enemy>
    {
        public override void Reset()
        {
            Clear();
        }
    }
}
