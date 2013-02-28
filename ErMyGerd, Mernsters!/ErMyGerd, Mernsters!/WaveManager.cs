using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using VGDevLib;
using ErMyGerdMernsters.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    public class WaveManager : DrawableGameElement
    {
        public static List<SpawnPoint> spawnPoints;

        private List<Wave> waves;
        private bool stageDone;
        private int waveNumber;
        private int finalWave;

        public void Initialize() { }
        private float transparency;
        private float scale;

        private SpriteFont waveFont;
        private Vector2 screenCenter;

        public WaveManager()
        {
            transparency = 1;
            scale = 2;
            waves = new List<Wave>();
            waveFont = Global.Fonts["Wave Font"];
            CameraDependent = false;
        }

        public bool IsFinished
        {
            get
            {
                return stageDone;
            }
        }

        public override void Update(GameTime gt)
        {
            if (!stageDone)
            {
                if (transparency > 0f)
                {
                    transparency -= 0.005f;
                    scale -= 0.005f;
                }
                Wave currentWave;
                try
                {
                    currentWave = waves[waveNumber - 1];
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    stageDone = true;
                    return;
                }
                if (!currentWave.IsFinished)
                    currentWave.Update(gt);
                if (currentWave.IsFinished && Global.Enemies.Count <= 0)
                {
                    waveNumber++;
                    if (waveNumber > finalWave)
                        stageDone = true;
                    transparency = 1;
                    scale = 2;
                }
            }
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            if (!stageDone)
            {
                if (transparency > 0)
                {
                    screenCenter = new Vector2(Global.Graphics.PreferredBackBufferWidth / 2, Global.Graphics.PreferredBackBufferHeight * 1 / 4);
                    StringBuilder waveString;
                    if (waveNumber == finalWave)
                    {
                        waveString = new StringBuilder("FINAL WAVE");
                    }
                    else
                    {
                        waveString = new StringBuilder("WAVE  " + waveNumber);
                    }
                    Vector2 origin = waveFont.MeasureString(waveString) / 2;
                    Color drawColor = new Color(new Vector4(Color.Red.ToVector3(), transparency));
                    sb.DrawString(waveFont, waveString, screenCenter, drawColor, 0.0f, origin, scale, SpriteEffects.None, 1.0f);
                }
            }
        }

        public override void Reset()
        {
            stageDone = false;
            waveNumber = 1;
            waves = null;
        }

        public void loadWaves(int mapID)
        {
            Reset();
            waves = new List<Wave>();
            Spawn[][] waveToLoad = Global.Waves["Level " + mapID];
            for (int i = 0; i < waveToLoad.Length; i++)
            {
                waves.Add(new Wave(waveToLoad[i]));
            }
            for (int i = 0; i < waves.Count; i++)
                waves[i].fixDelays();
            finalWave = waves.Count;
        }
    }

    public class Wave
    {
        private const int initialDelay = 5000;
        public Spawn[] spawn;
        private int waveTime;
        private int lastSpawnIndex;
        private bool doneSpawning;

        public Wave(Spawn[] xmlData)
        {
            waveTime = 0;
            spawn = new Spawn[xmlData.Length];
            for (int i = 0; i < spawn.Length; i++)
            {
                spawn[i] = xmlData[i].generateCopy();
            }
            lastSpawnIndex = 0;
            doneSpawning = false;
        }

        public void Update(GameTime gt)
        {
            waveTime += gt.ElapsedGameTime.Milliseconds;
            for (int i = lastSpawnIndex; i < spawn.Length; i++)
            {
                if (waveTime >= spawn[i].delay)
                {
                    for (int j = 0; j < spawn[i].number; j++)
                    {
                        if (spawn[i].spawnPoint <= 0)
                            Global.MapHandler.spawnEnemyAtEveryPoint(spawn[i].ID);
                        else
                            Global.MapHandler.spawnEnemy(spawn[i].ID, spawn[i].spawnPoint);
                    }
                    lastSpawnIndex++;
                }
                if (lastSpawnIndex >= spawn.Length)
                    doneSpawning = true;
            }
        }

        public bool IsFinished
        {
            get
            {
                return doneSpawning;
            }
        }

        public void fixDelays()
        {
            int totalDelay = initialDelay;
            for (int i = 0; i < spawn.Length; i++)
            {
                totalDelay += spawn[i].delay;
                spawn[i].delay = totalDelay;
            }
        }
    }

    public class SpawnPoint : DrawableGameElement, GameElement
    {
        private Point mapLoc;

        public SpawnPoint(int x, int y)
        {
            Rotation = (float)(Global.RNG.NextDouble() * (Math.PI * 2));
            mapLoc = new Point(x, y);
            Position = Global.MapHandler.findPosition(mapLoc);
            Texture = Global.Textures["Spawn"];
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            ColorMask = Color.Purple;
        }

        public override void Update(GameTime gt)
        {
            Rotation += 0.1f;
            if (Rotation > Math.PI * 2)
                Rotation %= (float)(Math.PI * 2);
        }

        public void spawn(int id)
        {
            if (Global.Debug.DONT_SPAWN)
                return;
            switch (id)
            {
                case 1:
                    new KirinTurret(Position);
                    break;
                case 2:
                    new Beowolf(Position);
                    break;
                case 4:
                    new GunShip(Position);
                    break;
                default:
                    break;
            }
        }
    }
}
