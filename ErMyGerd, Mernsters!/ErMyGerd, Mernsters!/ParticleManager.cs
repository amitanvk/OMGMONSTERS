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
using ProjectMercury;
using ProjectMercury.Controllers;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace ErMyGerdMernsters
{
    public class ParticleManager : Renderer
    {

        public ParticleManager()
        {
            Batch = new SpriteBatch(Global.Graphics.GraphicsDevice);
            foreach (KeyValuePair<string, ParticleEffect> pe in Global.ParticleEffects)
            {
                pe.Value.LoadContent(Global.Content);
                pe.Value.Initialise();
            }
            LoadContent(Global.Content);
        }

        public void Update(GameTime gt)
        {
            foreach (KeyValuePair<string, ParticleEffect> pe in Global.ParticleEffects)
            {
                pe.Value.Update((float)gt.ElapsedGameTime.TotalSeconds);
            }
        }

        public int TotalParticles()
        {
            int particleCount = 0;
            foreach (KeyValuePair<string, ParticleEffect> pe in Global.ParticleEffects)
            {
                particleCount += pe.Value.ActiveParticlesCount;
            }
            return particleCount;
        }

        public void Render()
        {
            Matrix camTransformation = Global.Camera.get_transformation();
            foreach (KeyValuePair<string, ParticleEffect> pe in Global.ParticleEffects)
            {
                RenderEffect(pe.Value, ref camTransformation);
            }
        }

        private SpriteBatch Batch;

        /// <summary>
        /// Disposes any unmanaged resources being used by the Renderer.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (this.Batch != null)
                    this.Batch.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads any content required by the renderer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the GraphicsDeviceManager has not been set.</exception>
        public override void LoadContent(ContentManager content)
        {
            if (this.Batch == null)
                this.Batch = new SpriteBatch(base.GraphicsDeviceService.GraphicsDevice);
        }

        /// <summary>
        /// Renders the specified Emitter, applying the specified transformation offset.
        /// </summary>
        public override void RenderEmitter(Emitter emitter, ref Matrix transform)
        {
            BlendState BlendState = BlendState.AlphaBlend;
            switch (emitter.BlendMode)
            {
                case EmitterBlendMode.None:
                    return;
                case EmitterBlendMode.Alpha:
                    BlendState = BlendState.NonPremultiplied;
                    break;
                case EmitterBlendMode.Add:
                    BlendState = BlendState.Additive;
                    break;
            }
            if (emitter.BlendMode == EmitterBlendMode.None)
                return;
            else
            if (emitter.ParticleTexture != null && emitter.ActiveParticlesCount > 0)
            {
                // Calculate the source rectangle and origin offset of the Particle texture...
                Rectangle source = new Rectangle(0, 0, emitter.ParticleTexture.Width, emitter.ParticleTexture.Height);
                Vector2 origin = new Vector2(source.Width / 2f, source.Height / 2f);

                this.Batch.Begin(SpriteSortMode.Immediate, BlendState, null, null, null, null, transform);

                for (int i = 0; i < emitter.ActiveParticlesCount; i++)
                {
                    Particle particle = emitter.Particles[i];
                    if(Util.inRenderLimit(particle.Position, 20))
                    {
                        float scale = particle.Scale / emitter.ParticleTexture.Width;
                        this.Batch.Draw(emitter.ParticleTexture, particle.Position, source, new Color(particle.Colour), particle.Rotation, origin, scale, SpriteEffects.None, 0f);
                    }
                }

                this.Batch.End();
            }
        }
    }
}
