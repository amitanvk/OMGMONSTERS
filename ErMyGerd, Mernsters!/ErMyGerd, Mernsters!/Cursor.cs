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
    public class Cursor : DrawableGameElement
    {
        private Vector2 defaultPosition;

        public Cursor()
        {
            Texture = Global.Textures["Cursor"];
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Rotation = -1 * (float)(Math.PI / 2);
            ColorMask = Color.Red;
            RenderLimitBound = false;
        }

        public override void Reset()
        {
        }

        public override void Update(GameTime gt)
        {
            if (ControlManager.ControlType == ControlManager.ControlMethod.KeyboardMouse)
            {
                Position = Global.Camera.Position + (ControlManager.mouseVector - new Vector2(Global.Graphics.PreferredBackBufferWidth / 2, Global.Graphics.PreferredBackBufferHeight / 2));
            }
            else if (ControlManager.ControlType == ControlManager.ControlMethod.Xbox)
            {
                const int distance = 300;
                const float searchRange = (float)Math.PI / 8;
                float relativeAngle = ControlManager.getCursorAngleFrom(Global.Player.Position);
                List<Enemy> candidates = new List<Enemy>();
                defaultPosition = Global.Player.Position + new Vector2((float)Math.Cos(relativeAngle), (float)Math.Sin(relativeAngle)) * distance;
                for (int i = 0; i < Global.Enemies.Count; i++)
                {
                    float enemyAngle = (float)Math.Atan2(Global.Enemies[i].Position.Y - Global.Player.Position.Y, Global.Enemies[i].Position.X - Global.Player.Position.X);
                    if(Util.inRenderLimit(Global.Enemies[i].Position, -Texture.Width))
                    {
                        double difference = Math.Abs(relativeAngle - enemyAngle);
                        if (difference < searchRange
                            || (difference > Math.PI && 2 * Math.PI - difference < searchRange))
                        {
                            candidates.Add(Global.Enemies[i]);
                        }
                    }
                }
                if (candidates.Count != 0)
                {
                    int closestDistance = int.MaxValue;
                    Enemy finalChoice = null;
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        int testDistance = (int)Util.distance(candidates[i].Position, Global.Player.Position);
                        if (closestDistance > testDistance)
                        {
                            finalChoice = candidates[i];
                            closestDistance = testDistance;
                        }
                    }
                    Position = finalChoice.Position;
                }
                else
                {
                    Position = defaultPosition;
                }
            }
            Rotation += 0.05f;
            if (Rotation >= Math.PI)
            {
                Rotation = -(float)Math.PI;
            }
        }
    }
}
