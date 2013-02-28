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
    public class MagniCrossCannon : AutomaticWeapon
    {
        private class Beam
        {
            public static List<Enemy> hitEnemies;
            public Beam child;
            public Vector2 start;
            public Vector2 end;
            public Enemy target;
            public int charge;
            private const float chargeToDamageRatio = 0.0125f;
            private const float chargeToSearchRadiusRatio = 0.5f;
            private const float chargeChainDecrease = 0.95f;
            private const int chargePerChildren = 25;

            public Beam(Vector2 s, Vector2 e, Enemy t, int c)
            {
                start = s;
                end = e;
                target = t;
                charge = c;
                child = null;
                if (target != null)
                {
                    hitEnemies.Add(target);
                    target.immobile = true;
                    generateChildren();
                }
            }

            public Beam(Vector2 s, Vector2 e, int c)
            {
                start = s;
                end = e;
                target = null;
                charge = c;
                child = null;
            }

            public void damageEnemy()
            {
                if (target == null)
                    return;
                target.damage(chargeToDamageRatio * charge, true);
                target.immobile = false;
                if(child != null)
                    child.damageEnemy();
            }

            public void Update(int c)
            {
                charge = c;
                if (child != null)
                {
                    child.Update((int)(charge * chargeChainDecrease));
                }
            }

            public void generateChildren()
            {
                int futureCharge = (int)(charge * chargeChainDecrease);
                if (futureCharge < stopAttackingThreshold)
                    return;
                Enemy choice = null;
                int distance = int.MaxValue;
                for (int i = 0; i < Global.Enemies.Count; i++)
                {
                    int currentDistance = (int)Util.distance(end, Global.Enemies[i].Position);
                    if (currentDistance < charge * chargeToSearchRadiusRatio && !hitEnemies.Contains(Global.Enemies[i]) && currentDistance < distance)
                        choice = Global.Enemies[i];
                }
                if (choice == null)
                    child = null;
                else
                    child = new Beam(end, choice.Position, choice, futureCharge);
            }

            public void Draw(SpriteBatch sb)
            {
                const float step = 10f;
                double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
                Vector2 changeVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * step;
                int distance = (int)(Util.distance(start, end) / step) ;
                for (int i = 0; i < distance; i++)
                    Global.ParticleEffects["Magnicross Cannon Beam"].Trigger(start + i * changeVector);
                if(child != null)
                    child.Draw(sb);
            }
        }

        private Beam mainBeam;
        private static float charge;
        private int maxCharge;
        private bool specialCharging;
        private float chargeRate;

        private bool charging = false;
        private bool startAttacking = false;
        private bool stopAttacking = false;
        private bool attacking = false;
        private const float normalChargeRate = 0.3f;
        private const float specialChargeRate = 1f;
        private const int normalMaxCharge = 100;
        private const int specialMaxCharge = 200;
        private const float attackThreshold = 45f;
        private const float stopAttackingThreshold = 20f;
        private const float dechargeRate = 1f;
        private const float chargeToMPDrainRatio = 0.001f;
        private static int beamRadius;
        private Vector2 fireLocus;

        public MagniCrossCannon()
        {
            Beam.hitEnemies = new List<Enemy>();
            Texture = Global.Textures["Laser Cannon"];
            Origin = new Vector2(0, 9);
            SpriteSize = new Vector2(40, 17);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            beamRadius = (int)(charge * (3.0f / 16.0f)) / 2;
            fireLocus = Position + (new Vector2((float)(Math.Cos(Rotation) * (Texture.Width * 1.5 + beamRadius * 2.5)), (float)(Math.Sin(Rotation) * (Texture.Width * 1.5 + beamRadius * 2.5))));
            if (charge <= stopAttackingThreshold)
                AnimationFrameY = 0;
            else if (charge > stopAttackingThreshold && charge <= normalMaxCharge)
                AnimationFrameY = 1;
            else
                AnimationFrameY = 2;
            if(mainBeam != null)
                mainBeam.Update((int)charge);
            if (charging)
            {
                if (specialCharging)
                {
                    Global.Player.drain(charge * chargeToMPDrainRatio * 5, false);
                    chargeRate = specialChargeRate;
                }
                else
                {
                    Global.Player.drain(charge * chargeToMPDrainRatio, false);
                    chargeRate = normalChargeRate;
                }
                if (Global.Player.MP <= 0)
                    charging = false;
                if(charge > maxCharge)
                    charge -= dechargeRate;
                else if (charge + chargeRate > maxCharge)
                    charge = maxCharge;
                else
                    charge += chargeRate;
                startAttacking = charge >= attackThreshold;
                stopAttacking = false;
            }
            else
            {
                if(charge - dechargeRate < 0)
                    charge = 0;
                else
                    charge -= dechargeRate;
                startAttacking = false;
                stopAttacking = charge <= stopAttackingThreshold;
            }
            if (!ControlManager.FIRE.Pressed && !ControlManager.ALT_FIRE.Pressed)
                charging = false;
            attacking = (charging && startAttacking) ||
                (!charging && !stopAttacking && attacking);
            if (attacking)
            {
                Global.Player.immobile = true;
                newAttack();
            }
            else
                Global.Player.immobile = false;
        }

        public void newAttack()
        {
            if (mainBeam != null)
            {
                mainBeam.damageEnemy();
                Beam.hitEnemies.Clear();
            }
            Hitbox checkHitbox = new Hitbox(Position, beamRadius, Faction.Player, true);
            Vector2 checkInterval = new Vector2((float)Math.Cos(Rotation) * beamRadius, (float)Math.Sin(Rotation) * beamRadius);
            Enemy closestTarget = null;
            int range = 500;
            int check = 0;
            int endCheck = (int)(range / checkInterval.Length());
            checkInterval += 2 * checkInterval;
            while((closestTarget == null && Util.distance(Position, Util.pointToVector(checkHitbox.center)) < range) && check < endCheck)
            {
                for (int i = 0; i < Global.Enemies.Count; i++)
                    if (Hitbox.collisionCheck(checkHitbox, Global.Enemies[i].Hitbox))
                        closestTarget = Global.Enemies[i];
                for (int i = 0; i < Global.Projectiles.Count; i++)
                    if (Hitbox.collisionCheck(checkHitbox, Global.Projectiles[i].hitbox) && Global.Projectiles[i].player == Faction.Enemy)
                        Global.Projectiles[i].exploding = true;
                checkHitbox.update(Util.pointToVector(checkHitbox.center) + checkInterval, beamRadius);
                check++;
            }
            if (closestTarget == null)
                mainBeam = new Beam(fireLocus, 
                    Position + new Vector2((float)(Math.Cos(Rotation) * range), (float)(Math.Sin(Rotation) * range)), (int)charge);
            else
                mainBeam = new Beam(fireLocus, closestTarget.Position, closestTarget, (int)charge);
        }

        public override void normalAttack()
        {
            if (Global.Player.MP > 1)
                charging = true;
            else
                charging = false;
            maxCharge = normalMaxCharge;
            specialCharging = false;
        }

        public override void specialAttack()
        {
            if (Global.Player.MP > 1)
                charging = true;
            else
                charging = false;
            maxCharge = specialMaxCharge;
            specialCharging = true;
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            if (attacking && mainBeam != null)
                mainBeam.Draw(sb);
            Util.CreateExplosion(fireLocus, (int)(2.5 * beamRadius), Color.Blue, Color.White, 1, 20, sb);
        }
    }
}
