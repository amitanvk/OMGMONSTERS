using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using ErMyGerdMernsters.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    /// <summary>
    /// TO-DO: Comment Class
    /// </summary>
    public class Player : Entity
    {
        public List<Weapon> weapons;
        private int currentWeapon;
        private bool dashing = false;
        private int currentDashTime;
        private const int dashTime = 100;
        private const int dashDelay = 5000;
        private int timeLastDash = 0;

        public override void loadContent()
        {
            Texture = Global.Textures["Pss"];
            SpriteSize = new Vector2(18, 18);
            Scaling = 2;
            Origin = SpriteSize / 2;
        }

        public override void onSpawn()
        {
            Faction = Faction.Player;
            Position = new Vector2(Global.Graphics.PreferredBackBufferWidth / 2, Global.Graphics.PreferredBackBufferHeight / 2);
            HPRegenDecreaseResetTime = 15000f;
            MPRegenDecreaseResetTime = 5000f;
            decreasedHPRegen = 0.0f;
            decreasedMPRegen = 0.001f;
            normalHPRegen = 0.25f;
            normalMPRegen = 1f;
            HitboxRadius = 16;
            weapons = new List<Weapon>();
            weapons.Add(new ManaPulse());
            weapons.Add(new Sword());
            weapons.Add(new MagniCrossCannon());
            currentWeapon = 0;
            MaximumHP = 100;
            MaximumMP = 100;
        }

        public override void onDeath()
        {
            Global.GameState = GameState.LOSE;
        }

        public override void onUpdate(GameTime gt)
        {
            timeLastDash += gt.ElapsedGameTime.Milliseconds;
            if (timeLastDash > dashDelay)
            {
                if (ControlManager.RIGHT.DoubleTap || ControlManager.LEFT.DoubleTap || ControlManager.UP.DoubleTap || ControlManager.DOWN.DoubleTap)
                {
                    dashing = true;
                    currentDashTime = 0;
                }
            }
            if (dashing)
            {
                currentDashTime += gt.ElapsedGameTime.Milliseconds;
                if (currentDashTime > dashTime)
                {
                    dashing = false;
                    timeLastDash = 0;
                }
            }
            float speed = (dashing) ? 9.0f : 3.0f;
            Velocity = new Vector2();
            Vector2 center = new Vector2(Global.Graphics.PreferredBackBufferWidth / 2, Global.Graphics.PreferredBackBufferHeight / 2);
            if (ControlManager.UP.Pressed)
                Velocity -= Vector2.UnitY * speed;
            if (ControlManager.DOWN.Pressed)
                Velocity += Vector2.UnitY * speed;
            if (ControlManager.LEFT.Pressed)
                Velocity -= Vector2.UnitX * speed;
            if (ControlManager.RIGHT.Pressed)
                Velocity += Vector2.UnitX * speed;
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            facePoint(Global.Cursor.Position);
            weapons[currentWeapon].Update(gt);
            if (ControlManager.DELTA_SCROLL != 0)
            {
                currentWeapon += ControlManager.DELTA_SCROLL;
                do
                {
                    if (currentWeapon < 0)
                        currentWeapon = weapons.Count + currentWeapon;
                    if (currentWeapon >= weapons.Count)
                        currentWeapon = currentWeapon % weapons.Count;
                }
                while (currentWeapon < 0 || currentWeapon >= weapons.Count);
            }
            if (ControlManager.NUM_1.Pressed)
            {
                currentWeapon = 0;
            }
            else if (ControlManager.NUM_2.Pressed)
            {
                currentWeapon = 1;
            }
            else if (ControlManager.NUM_3.Pressed)
            {
                currentWeapon = 2;
            }
        }

        public override bool damage(float value, bool external)
        {
            if (Global.Debug.INVINCIBLE)
                return true;
            return base.damage(value, external);
        }

        public override bool drain(float value, bool external)
        {
            if (Global.Debug.INFINITE_MANA)
                return true;
            return base.drain(value, external);
        }

        /// <summary>
        /// TO-DO: Comment Method
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="sb"></param>
        protected override void DrawAfter(SpriteBatch sb)
        {
            weapons[currentWeapon].Draw(sb);
        }

        public override void Reset()
        {
            currentWeapon = 0;
            heal(MaximumHP);
            rejuvenate(MaximumMP);
        }

        public override void Move(GameTime gt)
        {
            xTile.Dimensions.Rectangle viewPort = Global.MapHandler.viewPort;
            Map map = MapHandler.map;
            Vector2 v = Velocity;
            if (Position.X - SpriteSize.X * 0.5f + Velocity.X < Global.Camera.Position.X - Global.Graphics.PreferredBackBufferWidth / 2)
                v.X = Global.Camera.Position.X - Global.Graphics.PreferredBackBufferWidth / 2 - (Position.X - SpriteSize.X * 0.5f);
            if(Position.X + SpriteSize.X * 0.5f + Velocity.X > Global.Camera.Position.X +  Global.Graphics.PreferredBackBufferWidth / 2)
                v.X = Global.Camera.Position.X + Global.Graphics.PreferredBackBufferWidth / 2 - (Position.X + SpriteSize.X * 0.5f);
            if (Position.Y - SpriteSize.Y * 0.5f + Velocity.Y < Global.Camera.Position.Y - Global.Graphics.PreferredBackBufferHeight / 2)
                v.Y = Global.Camera.Position.Y - Global.Graphics.PreferredBackBufferHeight / 2 - (Position.Y - SpriteSize.Y * 0.5f);
            if (Position.Y + SpriteSize.Y * 0.5f + Velocity.Y > Global.Camera.Position.Y + Global.Graphics.PreferredBackBufferHeight / 2)
                v.Y = Global.Camera.Position.Y + Global.Graphics.PreferredBackBufferHeight / 2 - (Position.Y + SpriteSize.Y * 0.5f);
            Velocity = v;
            base.Move(gt);
        }
    }
}
