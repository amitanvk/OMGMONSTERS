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
    class Sword : SemiAutoWeapon
    {
        bool swinging = false;
        bool rage = false;
        float angleOffset = (float)Math.PI / 2;
        float additionalScaling = 0;
        const float maxAdditionalScaling = 1f;
        const int damage = 20;
        const int rageDamage = 100;
        Hitbox swordHitbox;
        List<Enemy> enemiesHit;

        public Sword()
        {
            enemiesHit = new List<Enemy>();
            Texture = Global.Textures["Sword"];
            Origin = new Vector2(1, 4);
            swordHitbox = new Hitbox(new Point(0, 0), (int)(Texture.Width * 0.75), Faction.Player, true);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (rage)
                ColorMask = Color.Red;
            else
                ColorMask = Color.White;
            float baseRotation = Global.Player.Rotation;
            Rotation = baseRotation + 2 * angleOffset;
            float swordOffsetAngle = (float) Rotation + angleOffset;
            Vector2 spriteSize = Global.Player.SpriteSize;
            Position = Global.Player.Position +
                new Vector2((float)(Math.Cos(swordOffsetAngle) * Global.Player.SpriteSize.X), (float)(Math.Sin(swordOffsetAngle) * Global.Player.SpriteSize.Y));
            if (swinging)
            {
                if(angleOffset > -(float)(Math.PI / 2))
                   angleOffset -= 0.15f;
                else 
                {
                    angleOffset = (float)(Math.PI / 2);
                    swinging = false;
                    enemiesHit.Clear();
                }
            }
            additionalScaling = (1 - (float)Math.Abs((swordOffsetAngle - Rotation) / (Math.PI / 2))) * maxAdditionalScaling;
            Scaling = 1.5f + additionalScaling;
            swordHitbox.update(Position+ 
                new Vector2(
                    (float)(Math.Cos(Rotation + 2 * angleOffset)) * (float)(Texture.Width * (1.5 + additionalScaling)) / 2,
                    (float)(Math.Sin(Rotation + 2 * angleOffset)) * (float)(Texture.Width * (1.5 + additionalScaling)) / 2), 
                    (int)(Texture.Width * (1.5f + additionalScaling))/2);
            if (rage)
            {
                const float step = 10f;
                Vector2 start = Position;
                Vector2 end = Position - new Vector2(
                    (float)(Math.Cos(Rotation + 2 * angleOffset)) * (float)(Texture.Width * (1.5 + additionalScaling)),
                    (float)(Math.Sin(Rotation + 2 * angleOffset)) * (float)(Texture.Width * (1.5 + additionalScaling)));
                double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
                Vector2 changeVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * step;
                int distance = (int)(Util.distance(start, end) / step);
                for (int i = 0; i < distance; i++)
                    Global.ParticleEffects["Blood Splatter"].Trigger(start + i * changeVector);
            }
            if(swinging)
            {
                for(int i = 0; i < Global.Projectiles.Count; i++)
                    if (Hitbox.collisionCheck(swordHitbox, Global.Projectiles[i].hitbox))
                        if (Global.Projectiles[i].player == Faction.Enemy)
                            Global.Projectiles[i].exploding = true;

                for (int i = 0; i < Global.Enemies.Count; i++)
                {
                    Enemy enemyCheck = Global.Enemies[i];
                    if (Hitbox.collisionCheck(swordHitbox, enemyCheck.Hitbox) && !enemiesHit.Contains(enemyCheck))
                    {
                        if (rage)
                            enemyCheck.damage(rageDamage, true);
                        else
                            enemyCheck.damage(damage, true);
                        enemiesHit.Add(enemyCheck);
                    }
                }
            }
        }

        public override void normalAttack()
        {
            if (!swinging)
            {
                swinging = true;
                if (rage)
                {
                    if (!Global.Player.damage(5, false))
                        rage = false;
                }
            }
        }

        public override void specialAttack()
        {
            rage = !rage;
            if (Global.Player.HP - 5 <= 0)
                rage = false;
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            swordHitbox.draw(sb);
        }
    }
}
