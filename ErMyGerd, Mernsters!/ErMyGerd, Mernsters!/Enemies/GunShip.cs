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
    public class GunShip : Enemy
    {
        private List<Gear> gears;
        private int maxGearCount = 6;
        private float gearReferenceAngle = 0;
        private int surroundingDistance = 50;
        private bool inOut = true;
        private bool shipOrPlayer = false;
        private int gearRespawnTime = 1000;
        private int manicGearRespawnTime = 100;
        private int lastRespawn = 0;
        float time_last_burst;
        private float time_between_bursts = 1500f;
        private SoundEffectInstance activeSound;

        public GunShip(Vector2 p) : base(p) { }

        public override void onSpawn()
        {
            base.onSpawn();
            mapBound = false;
            MaximumHP = 100;
            MaximumMP = 200;
            gearReferenceAngle = (float)(Global.RNG.NextDouble() * (2 * Math.PI));
            gears = new List<Gear>();
            activeSound = Global.SoundEffects["Gunship Amvient"].CreateInstance();
            activeSound.IsLooped = true;
            //activeSound.Play();
        }

        public override void loadContent()
        {
            Texture = Global.Textures["Gunship"];
            base.loadContent();
        }

        public override void onUpdate(GameTime gt)
        {
            if (Global.GameState != GameState.IN_GAME)
                activeSound.Stop();
            if (HP > 0.45 * MaximumHP)
            {
                maxGearCount = 6;
                shipOrPlayer = true;
            }
            else
            {
                maxGearCount = 20;
                gearRespawnTime = manicGearRespawnTime;
                shipOrPlayer = false;
            }
            lastRespawn += gt.ElapsedGameTime.Milliseconds;
            if (lastRespawn > gearRespawnTime && gears.Count < maxGearCount)
            {
                Gear newGear = new Gear(Position, this, Position, 4);
                gears.Add(newGear);
                damage((int)(newGear.HP * 0.05), true);
                lastRespawn = 0;
            }
            const float speed = 0.5f;
            int distance = (int)Util.distance(Global.Player.Position, Position);
            if (distance > 200)
                moveTowardEntity(Global.Player, speed);
            else if (distance < 200)
            {
                moveTowardEntity(Global.Player, speed);
                Velocity *= -2;
            }
            else
                Velocity = Vector2.Zero;
            gearReferenceAngle += 0.05f;
            if (gearReferenceAngle > 2 * Math.PI)
                gearReferenceAngle %= 2 * (float)Math.PI;
            if (surroundingDistance > 50 && !inOut)
            {
                surroundingDistance--;
                if (surroundingDistance <= 50)
                    inOut = !inOut;
            }
            if (surroundingDistance < 100 && inOut)
            {
                surroundingDistance++;
                if (surroundingDistance >= 100)
                    inOut = !inOut;
            }
            for(int i = 0; i < gears.Count; i++)
            {
                float angle = gearReferenceAngle + i * (float)(2 * Math.PI / gears.Count);
                gears[i].Target = ((shipOrPlayer) ? Position : Global.Player.Position) + new Vector2((float)(Math.Sin(angle) * surroundingDistance), (float)(Math.Cos(angle) * surroundingDistance));
            }

            time_last_burst += gt.ElapsedGameTime.Milliseconds;
            if (time_last_burst >= time_between_bursts)
            {
                time_last_burst = 0f;
                for (int i = 0; i < gears.Count; i++)
                {
                    if (shipOrPlayer)
                        gears[i].fire((float)(Math.Atan2(Position.Y - gears[i].Position.Y, Position.X - gears[i].Position.X) + Math.PI / 2));
                    else
                        gears[i].fire(Global.Player);
                }
            }
        }

        public override void onDeath()
        {
            base.onDeath();
            activeSound.Stop();
        }

        private class Gear : Enemy
        {
            private GunShip owner;
            private Vector2 targetPos;
            private bool kamikaze = false;
            private bool exploding = false;
            private int explodingFrame = 0;

            public Vector2 Target
            {
                get { return targetPos; }
                set { targetPos = value; }
            }

            private int speed;

            public Gear(Vector2 p, GunShip g, Vector2 t, int s) : base(p) 
            {
                Position = p;
                owner = g;
                Rotation = (float)(Global.RNG.NextDouble() * (2 * Math.PI));
                targetPos = t;
                speed = s;
            }

            public override void onSpawn()
            {
                base.onSpawn();
                mapBound = false;
                showHPBar = false;
                MaximumHP = 10;
                MaximumMP = 10;
            }

            public override void loadContent()
            {
                Texture = Global.Textures["Gunship Gear"];
                base.loadContent();
            }

            public override void onUpdate(GameTime gt)
            {
                if (owner.HP <= 0)
                {
                    kamikaze = true;
                    speed += 5;
                }
                if (!exploding)
                {
                    Rotation += 1f;
                    if (kamikaze)
                    {
                        moveTowardEntity(Global.Player, speed);
                        if (Hitbox.collisionCheck(Hitbox, Global.Player.Hitbox))
                        {
                            Global.Player.damage((int)(HP * 0.3), true);
                            Texture = null;
                            exploding = true;
                        }
                    }
                    else
                        moveTowardPoint(targetPos, speed);
                }
                else
                {
                    Velocity = Vector2.Zero;
                    explodingFrame++;
                    Scaling += 0.01f;
                    if (explodingFrame > 20)
                    {
                        onDeath();
                    }
                }
            }
            
            protected override void DrawAfter(SpriteBatch sb)
            {
                base.DrawAfter(sb);
                if (exploding)
                {
                    Global.ParticleEffects["Fireball Explosion"].Trigger(Position);
                }
            }

            public override void onDeath()
            {
                base.onDeath();
                owner.gears.Remove(this);
            }

            public override void Move(GameTime gt)
            {
                Position += Velocity;
            }

            public void fire(float angle)
            {
                new GunShipMissile(
                    Position,
                    4,
                    3,
                    angle);
            }

            public void fire(Entity target)
            {
                faceEntity(target);
                new GunShipMissile(
                    Position,
                    4,
                    3,
                    Rotation);
            }
        }
    }

    public class GunShipMissile : LinearProjectile
    {
        private const float rotationSpeed = 0.05f;
        private float targetAngle;
        private Vector2 origin = new Vector2(13, 7);
        private bool returning = false;
        private int returnTime = 0;
        private const int time_to_return = 2500;
        private int time_last_frame = 0;
        private const int time_between_frames = 200;
        private SoundEffectInstance activeSound;

        public GunShipMissile(
            Vector2 startVector,
            float speed,
            float damage,
            float rotate) :
            base(startVector, speed, damage, rotate)
        {
            Texture = Global.Textures["Gunship Missile"];
            explosionEffect = Global.ParticleEffects["Explosion"];
            SpriteSize = new Vector2(32, 13);
            player = Faction.Enemy;
            hitbox = new Hitbox(Position, 16, player, true);
            activeSound = Global.SoundEffects["Missile Active"].CreateInstance();
            activeSound.IsLooped = true;
            //activeSound.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Global.GameState != GameState.IN_GAME)
                activeSound.Stop();
            returnTime += gameTime.ElapsedGameTime.Milliseconds;
            if (returnTime >= time_to_return)
                returning = true;
            time_last_frame += gameTime.ElapsedGameTime.Milliseconds;
            if (time_last_frame >= time_between_frames)
            {
                time_last_frame = 0;
                if (++AnimationFrameY >= 6)
                {
                    AnimationFrameY = 0;
                }
            }
            hitbox.update(Position, 16);
            if (returning)
            {
                targetAngle = (float)Math.Atan2(Global.Player.Position.Y - Position.Y, Global.Player.Position.X - Position.X);
                while (Rotation < -Math.PI || Rotation > Math.PI)
                {
                    if (Rotation > Math.PI)
                        Rotation -= 2 * (float)Math.PI;
                    if (Rotation < -Math.PI)
                        Rotation += 2 * (float)Math.PI;
                }
                if (Math.Abs(Rotation - targetAngle) > Math.PI)
                {
                    float TargetAngleInPI = targetAngle /(float) Math.PI;
                    if(TargetAngleInPI > 0)
                        Rotation -= rotationSpeed;
                    if (TargetAngleInPI < 0)
                        Rotation += rotationSpeed;
                }
                else
                {
                    if (Rotation > targetAngle)
                        if (Rotation - rotationSpeed < targetAngle)
                                Rotation = targetAngle;
                            else
                                Rotation -= rotationSpeed;
                    else if (Rotation < targetAngle)
                        if (Rotation + rotationSpeed > targetAngle)
                            Rotation = targetAngle;
                        else
                            Rotation += rotationSpeed;
                }
            }
            movementVector = new Vector2(velocity * (float)Math.Cos(Rotation), velocity * (float)Math.Sin(Rotation));
        }

        public override void explode()
        {
            base.explode();
            activeSound.Stop();

        }
        
        protected override void DrawAfter(SpriteBatch sb)
        {
            if (exploding)
            {
                int radius = 10 + 3 * exploding_Frame;
                Util.CreateExplosion(Position, radius, Color.Red, Color.Yellow, 1, (int)(radius * (2.0 / 3.0)), sb);
                Util.CreateExplosion(Position, (int)(radius * (2.0 / 3.0)), Color.Yellow, Color.White, 1, (int)(radius * (1.0 / 3.0)), sb);
            }
        }
    }
}
