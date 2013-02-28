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
    public class Beowolf : PathBoundEnemy
    {
        private int time_last_frame = 0;
        private const int time_between_frames = 500;
        private bool nomming = false;
        private int nomDamage = 3;
        private int time_last_nom = 0;
        private const int time_between_noms = 1000;

        public Beowolf(Vector2 p) : base(p) { }

        public override void onSpawn()
        {
            base.onSpawn();
            SpriteSize = new Vector2(28, 23);
            Origin = SpriteSize / 2;
            MaximumHP = 40;
            MaximumMP = 40;
            BaseSpeed = 3.5f;
        }

        public override void onUpdate(GameTime gt)
        {
            base.onUpdate(gt);
            time_last_nom += gt.ElapsedGameTime.Milliseconds;
            if (nomming)
            {
                Velocity = Vector2.Zero;
                if (time_last_nom >= time_between_noms)
                {
                    nomming = false;
                    time_last_nom = 0;
                }
                animateFrameY = 0;
                time_last_frame += gt.ElapsedGameTime.Milliseconds;
                if (time_last_frame >= time_between_frames)
                {
                    time_last_frame = 0;
                    if (++animateFrameX > 1)
                    {
                        animateFrameX = 0;
                    }
                }
            }
            else
            {
                if (Hitbox.collisionCheck(Hitbox, Global.Player.Hitbox))
                {
                    Velocity = Vector2.Zero;
                    Global.ParticleEffects["Blood Splatter"].Trigger(Global.Player.Position);
                    Global.Player.damage(nomDamage, true);
                    nomming = true;
                    time_last_nom = 0;
                    Global.SoundEffects["Beowulfattackwav"].CreateInstance().Play();
                }
                if (Velocity.X == 0 && Velocity.Y == 0)
                    animateFrameY = 0;
                else
                {
                    animateFrameY = 1;
                    time_last_frame += gt.ElapsedGameTime.Milliseconds;
                    if (time_last_frame >= time_between_frames)
                    {
                        time_last_frame = 0;
                        if (++animateFrameX > 1)
                        {
                            animateFrameX = 0;
                        }
                    }
                }
            }
        }

        public override void loadContent()
        {
            Texture = Global.Textures["Beowulf"];
            base.loadContent();
        }

        public override void updatePath()
        {
            Point start = Util.vectorToPoint(Global.MapHandler.findTileLoc(Position));
            Point finish = Util.vectorToPoint(Global.MapHandler.findTileLoc(Global.Player.Position));
            path = Global.MapHandler.FindPath(start, finish);
        }

        public override void onDeath()
        {
            base.onDeath();
            Global.SoundEffects["Beowulfdeath"].CreateInstance().Play();
        }
    }
}
