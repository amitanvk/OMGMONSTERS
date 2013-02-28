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
    public interface DrawableElement
    {
        void Draw(SpriteBatch sb);
        bool CameraDependent
        {
            get;
            set;
        }
    }

    public abstract class DrawableGameElement : GameElement, DrawableElement
    {
        private Texture2D _texture;
        public virtual Texture2D Texture
        {
            get { return _texture; }
            set 
            { 
                _texture = value;
                if (value != null)
                {
                    SourceRectangle = new Rectangle(0, 0, value.Width, value.Height);
                }
            }
        }

        private Vector2 _pos;
        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        private Vector2 _textureOrigin;
        public Vector2 Origin
        {
            get { return _textureOrigin; }
            set { _textureOrigin = value; }
        }

        private Rectangle _sourceRectangle;
        public virtual Rectangle SourceRectangle
        {
            get { return _sourceRectangle; }
            set { _sourceRectangle = value; }
        }

        private Color _colorMask;
        public Color ColorMask
        {
            get { return _colorMask; }
            set { _colorMask = value; }
        }

        private float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        private float _scaling;
        public float Scaling
        {
            get { return _scaling; }
            set { _scaling = value; }
        }

        private SpriteEffects _spriteEffects;
        public SpriteEffects SpriteEffects
        {
            get { return _spriteEffects; }
            set { _spriteEffects = value; }
        }

        private float _layerDepth;
        public float LayerDepth
        {
            get { return _layerDepth; }
            set { _layerDepth = value; }
        }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value;}
        }

        private bool _renderLimitBound = true;
        public bool RenderLimitBound
        {
            get { return _renderLimitBound; }
            set { _renderLimitBound = value; }
        }

        private bool _cameraDependent = true;
        public bool CameraDependent
        {
            get { return _cameraDependent; }
            set { _cameraDependent = value; }
        }

        public virtual void Reset()
        {
        }

        public DrawableGameElement()
        {
            Texture = null;
            Position = Vector2.Zero;
            Origin = Vector2.Zero;
            SourceRectangle = Rectangle.Empty;
            ColorMask = Color.White;
            Rotation = 0f;
            Scaling = 1f;
            SpriteEffects = SpriteEffects.None;
            LayerDepth = 1f;
        }

        public DrawableGameElement(string textureString, Vector2 pos) 
            : this(Global.Textures[textureString], pos) { }

        public DrawableGameElement(Texture2D texture, Vector2 pos)
            : this(texture,
                pos,
                new Vector2(texture.Width / 2, texture.Height / 2))
                { }

        public DrawableGameElement(Texture2D texture, Vector2 pos, Vector2 origin) 
            : this(texture,
                pos,
                origin,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                0f,
                1f,
                SpriteEffects.None,
                1f) { }

        public DrawableGameElement(
            Texture2D texture, 
            Vector2 pos, 
            Vector2 origin, 
            Rectangle source, 
            Color mask, 
            float rotation, 
            float scaling, 
            SpriteEffects effects, 
            float layerDepth)
        {
            Texture = texture;
            Position = pos;
            Origin = origin;
            SourceRectangle = source;
            ColorMask = mask;
            Rotation = rotation;
            Scaling = scaling;
            SpriteEffects = effects;
            LayerDepth = layerDepth;
        }

        public virtual void Update(GameTime gt)
        {
        }

        public void Draw(SpriteBatch sb)
        {
            DrawBefore(sb);
            if(Visible && 
                (Util.inRenderLimit(Position, Global.RenderLimit) || !RenderLimitBound))
            {
                if(Texture != null)
                    sb.Draw(
                        Texture, 
                        Position, 
                        SourceRectangle, 
                        ColorMask, 
                        Rotation, 
                        Origin, 
                        Scaling, 
                        SpriteEffects, 
                        LayerDepth);
            }
            DrawAfter(sb);
        }

        protected virtual void DrawBefore(SpriteBatch sb)
        {
        }

        protected virtual void DrawAfter(SpriteBatch sb)
        {
        }
    }

    public abstract class AnimatedGameElement : DrawableGameElement
    {
        public override Texture2D Texture
        {
            get { return base.Texture; }
            set
            {
                base.Texture = value;
                if (Texture != null)
                {
                    SpriteSize = new Vector2(value.Width, value.Height);
                    maxDisplayX = (int)(value.Width / SpriteSize.X);
                    maxDisplayY = (int)(value.Height / SpriteSize.Y);
                    AnimationFrameX = 0;
                    AnimationFrameY = 0;
                }
            }
        }

        private Vector2 _spriteSize;
        public virtual Vector2 SpriteSize
        {
            get { return _spriteSize; }
            set 
            {
                _spriteSize = value;
                if (Texture != null)
                {
                    maxDisplayX = (int)(Texture.Width / value.X);
                    maxDisplayY = (int)(Texture.Height / value.Y);
                    AnimationFrameX = 0;
                    AnimationFrameY = 0;
                }
            }
        }

        private int _animationFrameX;
        public int AnimationFrameX
        {
            get { return _animationFrameX; }
            set
            {
                if (value > maxDisplayX)
                    throw new ArgumentOutOfRangeException();
                else
                    _animationFrameX = value;
            }
        }

        private int _animationFrameY;
        public int AnimationFrameY
        {
            get { return _animationFrameY; }
            set
            {
                if (value > maxDisplayY)
                    throw new ArgumentOutOfRangeException();
                else
                    _animationFrameY = value;
            }
        }

        public override Rectangle SourceRectangle
        {
            get
            {
                return new Rectangle(
                  (int)(AnimationFrameX * SpriteSize.X),
                  (int)(AnimationFrameY * SpriteSize.Y),
                  (int)SpriteSize.X,
                  (int)SpriteSize.Y);
            }
        }

        private int maxDisplayX;
        private int maxDisplayY;

        public AnimatedGameElement() : base()
        {
            SpriteSize = new Vector2(1, 1);
        }

        public AnimatedGameElement(
            string texture,
            Vector2 pos,
            Vector2 origin,
            Vector2 spriteSize)
            : this(Global.Textures[texture], pos, origin, spriteSize)
        {
        }

        public AnimatedGameElement(
            Texture2D texture,
            Vector2 pos,
            Vector2 origin,
            Vector2 spriteSize)
            : this(texture, pos, origin, Color.White, 0f, 1f, SpriteEffects.None, 1f, spriteSize)
        {
        }

        public AnimatedGameElement(
            Texture2D texture, 
            Vector2 pos, 
            Vector2 origin, 
            Color mask, 
            float rotation, 
            float scaling, 
            SpriteEffects effects, 
            float layerDepth,
            Vector2 spriteSize)
            : base(texture, pos, origin, new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y), mask, rotation, scaling, effects, layerDepth)
        {
            maxDisplayX = (int)(texture.Width / spriteSize.X);
            maxDisplayY = (int)(texture.Height / spriteSize.Y);
        }
    }

    public abstract class DrawableGameElementCollection<T> 
        : GameElementCollection<T>, DrawableElement
        where T : DrawableGameElement
    {
        private bool _cameraDependent = true;
        public bool CameraDependent
        {
            get { return _cameraDependent; }
            set { _cameraDependent = value; }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < Count; i++)
                this[i].Draw(sb);
        }
    }
}
