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

namespace ErMyGerdMernsters.Enemies
{
    public class KirinTurret : Enemy
    {
        public Texture2D turretTexture;
        public static Vector2 rotatePoint = new Vector2(14, 7);
        private float number_Per_Burst = 2;
        private float numShot = 0;
        bool bursting;
        float time_last_burst;
        private float time_between_bursts = 2000f;
        float time_last_fire;
        private float time_between_fire = 350f;
        float time_last_teleport;
        float current_Teleport_timer;
        private float max_time_between_teleport = 5000f;
        private bool teleporting = false;
        private bool reappearing = true;

        public KirinTurret(Vector2 p) : base(p) { }

        public override void onSpawn()
        {
            base.onSpawn();
            teleport();
            time_last_burst = (float)Global.RNG.NextDouble() * time_between_bursts;
            current_Teleport_timer = (float)Global.RNG.NextDouble() * max_time_between_teleport;
            MaximumHP = 25;
            MaximumMP = 25;
        }

        public override void loadContent()
        {
            Texture = Global.Textures["Kirin Turret Base"];
            turretTexture = Global.Textures["Kirin Turret"];
            base.loadContent();
        }

        public override void onUpdate(GameTime gt)
        {
            moveTowardEntity(Global.Player, 3f);
            faceEntity(Global.Player);
            time_last_burst += gt.ElapsedGameTime.Milliseconds;
            time_last_teleport += gt.ElapsedGameTime.Milliseconds;
            if (time_last_burst >= time_between_bursts)
            {
                time_last_burst = 0f;
                bursting = true;
            }
            if (bursting)
            {
                time_last_fire += gt.ElapsedGameTime.Milliseconds;
                if (time_last_fire >= time_between_fire)
                {
                    time_last_fire = 0f;
                    Global.SoundEffects["Kirin Fire"].CreateInstance().Play();
                    const float projectileVelocity = 9.0f;
                    new Fireball(
                        Position,
                        projectileVelocity,
                        3,
                        Rotation);
                    numShot++;
                    if (numShot >= number_Per_Burst)
                    {
                        numShot = 0;
                        bursting = false;
                    }
                }
            }
            if (time_last_teleport >= current_Teleport_timer)
            {
                teleport();
            }
        }

        protected override void DrawBefore(SpriteBatch sb)
        {
            base.DrawBefore(sb);
            sb.Draw(
                turretTexture,
                Position,
                new Rectangle(0, 0, turretTexture.Width, turretTexture.Height),
                ColorMask,
                Rotation,
                rotatePoint,
                Scaling,
                SpriteEffects,
                LayerDepth - 1);
        }

        public void teleport()
        {
            teleporting = true;
            reappearing = false;
            time_last_teleport = 0;
            current_Teleport_timer = (float)Global.RNG.NextDouble() * max_time_between_teleport;
            Global.SoundEffects["Kirin Teleport Out"].CreateInstance().Play();
        }

        public override void Move(GameTime gt)
        {
            if (teleporting)
            {
                if (reappearing)
                {
                    Scaling += 0.025f;
                    if (Scaling >= 1)
                    {
                        Scaling = 1;
                        teleporting = false;
                    }
                }
                else
                {
                    Scaling -= 0.025f;
                    if (Scaling <= 0)
                    {
                        reappearing = true;
                        Global.SoundEffects["Kirin Teleport In"].CreateInstance().Play();
                        const int teleportRange = 4;
                        bool teleported = false;
                        Vector2 newPos = new Vector2();
                        Vector2 mapLoc = Global.MapHandler.findTileLoc(Position);
                        int x = 0, y = 0, tries = 0;
                        while (!teleported)
                        {
                            x = (int)mapLoc.X + (int)(Global.RNG.NextDouble() * (2 * teleportRange + 1)) - teleportRange;
                            y = (int)mapLoc.Y + (int)(Global.RNG.NextDouble() * (2 * teleportRange + 1)) - teleportRange;
                            newPos = Global.MapHandler.findPosition(new Vector2(x, y));
                            Vector2 tileLoc = Global.MapHandler.findTileLoc(newPos);
                            try
                            {
                                if (!Global.MapHandler.characterSolid[(int)tileLoc.X, (int)tileLoc.Y] && (mapLoc != tileLoc))
                                {
                                    teleported = true;
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                teleported = false;
                            }
                            tries++;
                            if (tries > 15)
                            {
                                return;
                            }
                        }
                        Position = newPos;
                    }
                }
            }
        }
    }

    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public class Fireball : LinearProjectile
    {
        private int time_last_frame = 0;
        private const int time_between_frames = 200;

        /// <summary>
        /// TO-DO: Comment Constructor
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="moveVector"></param>
        /// <param name="damage"></param>
        /// <param name="rotate"></param>
        public Fireball(
            Vector2 startVector,
            float velocity,
            float damage,
            float rotate) :
            base(startVector, velocity, damage, rotate)
        {
            Visible = Global.Debug.DONT_RENDER_PARTICLES;
            Texture = Global.Textures["Fireball"];
            player = Faction.Enemy;
            hitbox = new Hitbox(Position + hitboxOrigin, 16, player, true);
            hitboxOrigin = new Vector2(32 / 2, 15 / 2);
            SpriteSize = new Vector2(32, 15);
            explosionEffect = Global.ParticleEffects["Fireball Explosion"];
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gt"></param>
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            hitbox.update(Position, 16);
            if (Global.Debug.DONT_RENDER_PARTICLES)
            {
                time_last_frame += gt.ElapsedGameTime.Milliseconds;
                if (time_last_frame >= time_between_frames)
                {
                    time_last_frame = 0;
                    if (++animateFrame > 2)
                    {
                        animateFrame = 0;
                    }
                }
            }
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            base.DrawAfter(sb);
            if(!Global.Debug.DONT_RENDER_PARTICLES)
                Global.ParticleEffects["Fireball Actual"].Trigger(Position);
        }
    }
}
