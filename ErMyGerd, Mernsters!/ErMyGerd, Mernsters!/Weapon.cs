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
    public abstract class Weapon : AnimatedGameElement
    {
        public Weapon()
        {
            Scaling = 1.5f;
        }

        public override void Update(GameTime gt)
        {
            Rotation = Global.Player.Rotation;
            Vector2 spriteSize = Global.Player.SpriteSize;
            Position = Global.Player.Velocity + Global.Player.Position + new Vector2((float)(Math.Cos(Rotation) * spriteSize.X * 0.5f), (float)(Math.Sin(Rotation) * spriteSize.Y * 0.5));
        }

        public abstract void normalAttack();
        public abstract void specialAttack();
    }

    public abstract class SemiAutoWeapon : Weapon
    {
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (ControlManager.FIRE.Bumped)
            {
                normalAttack();
            }
            if (ControlManager.ALT_FIRE.Bumped)
            {
               specialAttack();
            }
        }
    }

    public abstract class AutomaticWeapon : Weapon
    {
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (ControlManager.FIRE.Pressed)
            {
                normalAttack();
            }
            if (ControlManager.ALT_FIRE.Pressed)
            {
                specialAttack();
            }
        }
    }
}
