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
    public abstract class Entity : AnimatedGameElement, GameElement
    {
        private Faction _faction;
        public Faction Faction
        {
            get { return _faction; }

            set { _faction = value; }
        }

        public override Texture2D Texture
        {
            get
            {
                return base.Texture;
            }

            set 
            {
                base.Texture = value;
                if (base.Texture != null)
                {
                    Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
                    SpriteSize = new Vector2(Texture.Width, Texture.Height);
                }
            }
        }

        public override Vector2 SpriteSize
        {
            get
            {
                return base.SpriteSize;
            }
            set
            {
                base.SpriteSize = value;
                HitboxRadius = (int)(SpriteSize.X + SpriteSize.Y) / 2;
            }
        }

        public Hitbox Hitbox;
        public Vector2 HitboxOffset;
        public int HitboxRadius;

        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private float _hp;
        public float HP
        {
            get { return _hp; }
        }

        private float _mp;
        public float MP
        {
            get { return _mp; }
        }
        public float MaximumHP;
        public float MaximumMP;
        private float oldHP;
        private float oldMP;
        protected int animateFrameX = 0;
        protected int animateFrameY = 0;
        protected bool HPRegenDecrease = false;
        protected bool MPRegenDecrease = false;
        private float HPRengenDecreaseTimer = 0.0f;
        private float MPRengenDecreaseTimer = 0.0f;
        protected float HPRegenDecreaseResetTime = -1.0f;
        protected float MPRegenDecreaseResetTime = -1.0f;
        protected float normalHPRegen = 0.0f;  
        protected float normalMPRegen = 0.0f;
        protected float decreasedHPRegen = 0.0f;
        protected float decreasedMPRegen = 0.0f;
        public bool mapBound = true;
        public bool incapacitated = false;
        public bool immobile = false;

        public Entity()
        {
            loadContent();
            onSpawn();
            Hitbox = new Hitbox(Position, HitboxRadius, Faction, false);
            _hp = MaximumHP;
            _mp = MaximumMP;
            oldHP = _hp;
            oldMP = _mp;
        }

        public void Initialize() { }

        public void rengen(GameTime gt)
        {
            int timeChange = gt.ElapsedGameTime.Milliseconds;
            if (MPRegenDecreaseResetTime > 0)
            {
                if (MPRegenDecrease)
                {
                    MPRengenDecreaseTimer += timeChange;
                    if (oldMP > _mp)
                        MPRengenDecreaseTimer = 0.0f;
                    if (MPRengenDecreaseTimer >= MPRegenDecreaseResetTime)
                        MPRegenDecrease = false;
                }
                else
                {
                    MPRegenDecrease = oldMP > _mp;
                    MPRengenDecreaseTimer = 0.0f;
                }
                if (MPRegenDecrease)
                    rejuvenate(decreasedMPRegen);
                else
                    rejuvenate(normalMPRegen);
                oldMP = _mp;
            }
            if (HPRegenDecreaseResetTime > 0)
            {
                if (HPRegenDecrease)
                {
                    HPRengenDecreaseTimer += timeChange;
                    if (oldHP > _hp)
                        HPRengenDecreaseTimer = 0.0f;
                    if (HPRengenDecreaseTimer >= HPRegenDecreaseResetTime)
                        HPRegenDecrease = false;
                }
                else
                {
                    HPRegenDecrease = oldHP > _hp;
                    HPRengenDecreaseTimer = 0.0f;
                }
                if (HPRegenDecrease)
                    heal(decreasedHPRegen);
                else
                    heal(normalHPRegen);
                oldHP = _hp;
            }
        }

        public virtual void Move(GameTime gt)
        {
            if (mapBound)
                Velocity = Global.MapHandler.mapCollisionCheck(Velocity, Position, SpriteSize * Scaling);
            Position += Velocity;
        }

        public abstract void loadContent();

        public abstract void onSpawn();

        public abstract void onUpdate(GameTime gt);

        public abstract void onDeath();

        public override void Update(GameTime gt)
        {
            if (_hp <= 0)
                onDeath();
            Hitbox.update(Position + HitboxOffset, (int)((float)HitboxRadius * Scaling));
            rengen(gt);
            if (!incapacitated)
            {
                onUpdate(gt);
                if(!immobile)
                    Move(gt);
            }
        }

        public override void Reset()
        {
            _hp = MaximumHP;
            _mp = MaximumMP;
            incapacitated = false;
            immobile = false;
        }

        public void heal(float value)
        {
            if (value < 0)
                return;
            if (_hp + value > MaximumHP)
                _hp = MaximumHP;
            else
                _hp += value;
        }

        public void rejuvenate(float value)
        {
            if (value < 0)
                return;
            if (_mp + value > MaximumMP)
                _mp = MaximumMP;
            else
                _mp += value;
        }

        public virtual bool damage(float value, bool external)
        {
            if (value < 0)
                return false;
            if (_hp - value <= 0)
            {
                if (external)
                {
                    _hp -= value;
                    return true;
                }
                else
                    return false;
            }
            else
                _hp -= value;
            return true;
        }

        public virtual bool drain(float value, bool external)
        {
            if (value < 0)
                return false;
            if (_mp - value < 0)
            {
                if (external)
                {
                    _mp = 0;
                    return true;
                }
                else
                    return false;
            }
            else
                _mp -= value;
            return true;
        }

        public void facePoint(Vector2 targetPoint)
        {
            Rotation = (float)Math.Atan2(targetPoint.Y - Position.Y, targetPoint.X - Position.X);
        }

        public void faceEntity(Entity target)
        {
            facePoint(target.Position);
        }

        public void moveTowardPoint(Vector2 targetPoint, float speed)
        {
            facePoint(targetPoint);
            _velocity = new Vector2(speed * (float)Math.Cos(Rotation), speed * (float)Math.Sin(Rotation));
        }

        public void moveTowardEntity(Entity target, float speed)
        {
            moveTowardPoint(target.Position, speed);
        }
    }
}
