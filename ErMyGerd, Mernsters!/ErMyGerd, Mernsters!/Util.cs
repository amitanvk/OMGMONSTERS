using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    public static class Util
    {
        public static Dictionary<String, T> LoadContent<T>(this ContentManager contentManager, String contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<String, T> result = new Dictionary<String, T>();
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(key.ToLower())] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }

        public static void CreateCircle(Vector2 position, int r, Color circleColor, SpriteBatch sb)
        {
            int outerRadius = r * 2 + 2; // So circle doesn't go out of bounds
            Texture2D circle = new Texture2D(Global.Graphics.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / r;
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                int x = (int)Math.Round(r + r * Math.Cos(angle));
                int y = (int)Math.Round(r + r * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            circle.SetData(data);
            sb.Draw(circle, new Vector2(position.X, position.Y), new Rectangle(0, 0, circle.Width, circle.Height), circleColor, 0.0f,
                    new Vector2(circle.Width / 2, circle.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
        }

        public static void CreateLine(Vector2 start, int length, float rotation, Color color, int thickness, SpriteBatch sb)
        {
            if (length <= 0)
                return;
            Texture2D line = new Texture2D(Global.Graphics.GraphicsDevice, length, thickness);
            Vector2 origin = new Vector2(0, line.Height / 2);
            Color[] data = new Color[length * thickness];
            for (int i = 0; i < length * thickness; i++)
                data[i] = Color.White;
            line.SetData(data);
            sb.Draw(
                line,
                start,
                new Rectangle(0, 0, line.Width, line.Height),
                color,
                rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                1.0f);
        }

        public static void CreateExplosion(Vector2 position, int radius, Color outerColor, Color innerColor, int changeThickness, int iterations, SpriteBatch sb)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            int numberOfChanges = radius / changeThickness;
            Texture2D circle = new Texture2D(Global.Graphics.GraphicsDevice, outerRadius, outerRadius);
            Color[] data = new Color[outerRadius * outerRadius];
            Vector4 interval = (innerColor.ToVector4() - outerColor.ToVector4()) / iterations;
            int intervalNum;
            // Colour the entire texture transparent first.
            for(int y = 0; y < outerRadius; y++)
                for (int x = 0; x < outerRadius; x++)
                {
                    int distance = (int)Util.distance(new Vector2(x, y), new Vector2(outerRadius / 2, outerRadius / 2));
                    if (distance < radius)
                    {
                        intervalNum = Math.Abs(radius - distance) / changeThickness;
                        data[y * outerRadius + x] = Color.FromNonPremultiplied(outerColor.ToVector4() + interval * 
                            ((intervalNum > numberOfChanges) ? numberOfChanges : intervalNum));
                    }
                    else
                    {
                        data[y * outerRadius + x] = Color.Transparent;
                    }
                }

            circle.SetData(data);
            sb.Draw(circle, new Vector2(position.X, position.Y), new Rectangle(0, 0, circle.Width, circle.Height), Color.White, 0.0f,
                    new Vector2(circle.Width / 2, circle.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
        
        }

        public static void CreateBeam(Vector2 start, int length, float rotation, Color outerColor, Color innerColor, int changeThickness, int iterations, int thickness, SpriteBatch sb)
        {
            if (length <= 0)
                return;
            int numberOfChanges = (thickness / 2) / changeThickness;
            Texture2D line = new Texture2D(Global.Graphics.GraphicsDevice, length, thickness);
            Vector4 interval = (innerColor.ToVector4() - outerColor.ToVector4()) / iterations;
            Vector2 origin = new Vector2(0, line.Height / 2);
            Color[] data = new Color[length * thickness];
            int intervalNum;
            for (int i = 0; i < thickness; i++)
            {
                intervalNum = Math.Abs(i - (thickness / 2)) / changeThickness;
                Color color = Color.FromNonPremultiplied(outerColor.ToVector4() + interval * (numberOfChanges -intervalNum));
                for (int j = 0; j < length; j++)
                {
                    data[i * length + j] = color;
                }
            }
            line.SetData(data);
            sb.Draw(
                line,
                start,
                new Rectangle(0, 0, line.Width, line.Height),
                Color.White,
                rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                1.0f);
        }

        public static bool inRenderLimit(Vector2 screenPos, int renderLimit)
        {
            return screenPos.X > 
                    Global.Camera.Position.X - Global.Graphics.PreferredBackBufferWidth / 2 - renderLimit &&
                    screenPos.X < 
                    Global.Camera.Position.X + Global.Graphics.PreferredBackBufferWidth / 2 + renderLimit &&
                    screenPos.Y > 
                    Global.Camera.Position.Y - Global.Graphics.PreferredBackBufferHeight / 2- renderLimit &&
                    screenPos.Y <
                    Global.Camera.Position.Y + Global.Graphics.PreferredBackBufferHeight / 2 + renderLimit;
        }

        public static void CreateCircle( Point position, int r, SpriteBatch sb)
        {
            CreateCircle( position, r, Color.White, sb);
        }

        public static void CreateCircle( Vector2 position, int r, SpriteBatch sb)
        {
            CreateCircle( position, r, Color.White, sb);
        }

        public static void CreateCircle( Point position, int r, Color color, SpriteBatch sb)
        {
            CreateCircle( pointToVector(position), r, color, sb);
        }

        public static void DrawLine(Point start, Point end, Color color, int thickness, SpriteBatch sb)
        {
            DrawLine(pointToVector(start), pointToVector(end), color, thickness, sb);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, int thickness, SpriteBatch sb)
        {
            int length = (int)distance(start, end);
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            CreateLine(start, length, rotation, color, thickness, sb);
        }

        public static void DrawLine(Point start, int length, float rotation, Color color, int thickness, SpriteBatch sb)
        {
            CreateLine(pointToVector(start), length, rotation, color, thickness, sb);
        }

        public static void DrawBeam(Point start, Point end, Color outerColor, Color innerColor, int changeThickness, int iterations, int thickness, SpriteBatch sb)
        {
            DrawBeam(pointToVector(start), pointToVector(end), outerColor, innerColor, changeThickness, iterations, thickness, sb);
        }

        public static void DrawBeam(Vector2 start, Vector2 end, Color outerColor, Color innerColor, int changeThickness, int iterations, int thickness, SpriteBatch sb)
        {
            int length = (int)distance(start, end);
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            CreateBeam(start, length, rotation, outerColor, innerColor, changeThickness, iterations, thickness, sb);
        }

        public static void DrawBeam(Point start, int length, float rotation, Color outerColor, Color innerColor, int changeThickness, int iterations, int thickness, SpriteBatch sb)
        {
            CreateBeam(pointToVector(start), length, rotation, outerColor, innerColor, changeThickness, iterations, thickness, sb);
        }

        public static float distance(Vector2 start, Vector2 end)
        {
            return (float)Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));
        }

        public static float distance(Point start, Point end)
        {
            return (float)Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));
        }

        public static Point vectorToPoint(Vector2 input)
        {
            return new Point((int)input.X, (int)input.Y);
        }

        public static Vector2 pointToVector(Point input)
        {
            return new Vector2(input.X, input.Y);
        }
    }
}
