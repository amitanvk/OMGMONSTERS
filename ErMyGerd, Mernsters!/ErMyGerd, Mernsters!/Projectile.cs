using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;

namespace ErMyGerdMernsters
{
    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public abstract class Projectile : AnimatedGameElement
    {
        public ParticleEffect explosionEffect;
        public Vector2 hitboxOrigin;
        public Hitbox hitbox;
        public Faction player;
        public float dmg;
        public bool exploding;
        protected int animateFrame = 0, exploding_Frame = 0;

        public Projectile(
            Vector2 pos,
            float damage)
        {
            Position = pos;
            dmg = damage;
            Global.Projectiles.Add(this);
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (!exploding)
                move();
            else
            {
                Texture = null;
                explode();
            }
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="sb"></param>
        protected override void DrawAfter(SpriteBatch sb)
        {
            base.DrawAfter(sb);
            if (!Global.Debug.HITBOX_SHOW)
                hitbox.draw(sb);
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        public abstract void move();

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        public virtual void explode()
        {
            if(!Global.Debug.DONT_RENDER_PARTICLES && explosionEffect != null)
                explosionEffect.Trigger(Position);
            Global.Projectiles.Remove(this);
            Global.SoundEffects["Missile Explosion"].CreateInstance().Play();
        }
    }
    
    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public abstract class LinearProjectile : Projectile
    {
        protected float velocity;
        protected Vector2 movementVector;

        /// <summary>
        /// TO-DO: Comment Constructor
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="moveVector"></param>
        /// <param name="damage"></param>
        /// <param name="rotate"></param>
        public LinearProjectile(
            Vector2 startVector,
            float speed,
            float damage,
            float rotate) :
            base(startVector, damage)
        {
            velocity = speed;
            Rotation = rotate;
            movementVector = new Vector2(velocity * (float)Math.Cos(rotate), velocity * (float)Math.Sin(rotate));
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        public sealed override void move()
        {
            if (!exploding)
            {
                Position += movementVector;
            }
        }
    }

    public class ProjectileList : DrawableGameElementCollection<Projectile>
    {
        public override void Update(GameTime gt)
        {
            for (int i = 0; i < Count; i++)
                this[i].Update(gt);
            for (int i = 0; i < this.Count; i++)
            {
                Projectile projectileCheck = this[i];
                if (projectileCheck.player == Faction.Player && !projectileCheck.exploding)
                {
                    for (int j = 0; j < Count; j++)
                    {
                        Projectile projectileCheck2 = this[j];
                        if (projectileCheck2.player == Faction.Enemy && !projectileCheck2.exploding)
                        {
                            if (Hitbox.collisionCheck(projectileCheck.hitbox, projectileCheck2.hitbox))
                            {
                                projectileCheck.exploding = true;
                                projectileCheck2.exploding = true;
                            }
                        }
                    }

                    for (int j = 0; j < Global.Enemies.Count; j++)
                    {
                        if(Hitbox.collisionCheck(projectileCheck.hitbox, Global.Enemies[j].Hitbox))
                        {
                            Global.Enemies[j].damage(projectileCheck.dmg, true);
                            projectileCheck.exploding = true;
                        }
                    }
                }
                else if(projectileCheck.player == Faction.Enemy && !projectileCheck.exploding)
                {
                    if(Hitbox.collisionCheck(projectileCheck.hitbox, Global.Player.Hitbox))
                    {
                        Global.Player.damage(projectileCheck.dmg, true);
                        projectileCheck.exploding = true;
                    }
                }
            }
        }

        public override void Reset()
        {
            Clear();
        }
    }
}
