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

namespace ErMyGerdMernsters.Weapons
{
    public class ManaPulse : SemiAutoWeapon
    {
        private const int baseMPCost = 1;

        public ManaPulse()
        {
            Texture = Global.Textures["Mana Pulse"];
            Origin = new Vector2(0, 4);
        }

        public override void normalAttack()
        {
            if (Global.Player.drain(baseMPCost, false))
            {
                const float velocity = 10.0f;
                //Global.SoundEffects["Kirin Fire"].CreateInstance().Play();
                ManaPulseShot bullet = new ManaPulseShot(
                    Position,
                    velocity,
                    5,
                    Global.Player.Rotation);
            }
        }

        public override void specialAttack()
        {
            if (Global.Player.drain(baseMPCost * 30, false))
            {
                const float velocity = 10.0f;
                for (int i = 0; i <= 10; i++)
                {
                    float newRotationAngle = (float)(Global.Player.Rotation - (Math.PI / 8) + (Math.PI / 4) * Global.RNG.NextDouble());
                    Projectile bullet = new ManaPulseShot(
                        Position + (new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * (1.5f * Texture.Width)),
                        velocity,
                        5,
                        newRotationAngle);
                }
            }
        }
    }

    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public class ManaPulseShot : LinearProjectile
    {
        /// <summary>
        /// TO-DO: Comment Constructor
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="moveVector"></param>
        /// <param name="damage"></param>
        /// <param name="rotate"></param>
        public ManaPulseShot(
            Vector2 startVector,
            float velocity,
            float damage,
            float rotate) :
            base(startVector, velocity, damage, rotate)
        {
            player = Faction.Player;
            hitbox = new Hitbox(Position + hitboxOrigin, 16, player, true);
            Texture = Global.Textures["Mana Pulse Shot"];
            explosionEffect = Global.ParticleEffects["Mana Pulse Shot Explosion"];
            hitboxOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Visible = false;
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            hitbox.update(Position, 16);
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            Global.ParticleEffects["Mana Pulse Shot"].Trigger(Position);
            //if (exploding)
                //sb.Draw(explosionTexture, Position, null, Color.White, 0.0f, new Vector2(explosionTexture.Width / 2, explosionTexture.Height / 2), 0.5f, SpriteEffects.None, 1.0f);
        }
    }
}
