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
    public abstract class Pickup : DrawableGameElement
    {
        public Hitbox hitbox;
        public Vector2 hitboxOrigin;
        public Pickup(
            Vector2 position
            )
        {
            Position = position;
            Global.Pickups.Add(this);
        }

        public override void Update(GameTime gt)
        {
            if (Hitbox.collisionCheck(Global.Player.Hitbox, hitbox))
            {
                effect();
                Global.Pickups.Remove(this);
            }
        }

        public abstract void effect();
    }

    public class PickupList : DrawableGameElementCollection<Pickup>
    {
    }

    public class ManaPickup : Pickup
    {
        public ManaPickup(Vector2 position) : base(position) 
        {
            Texture = Global.Textures["Mp Pickup"];
            hitbox = new Hitbox(position + hitboxOrigin, 16, Faction.Neutral, false);
        }
        
        public override void effect()
        {
            Global.Player.rejuvenate(Global.Player.MaximumMP * 0.2f);
        }
    }
    
    public class Potion : Pickup
    {
        public Potion(Vector2 position) : base(position) 
        {
            hitbox = new Hitbox(position + hitboxOrigin, 16, Faction.Neutral, false);
            Texture = Global.Textures["Hp Pickup"];
            hitboxOrigin = new Vector2((int)Texture.Width / 2, (int)Texture.Height / 2);
        }

        public override void effect()
        {
            Global.Player.heal(Global.Player.MaximumHP * 0.2f);
        }
    }
}
