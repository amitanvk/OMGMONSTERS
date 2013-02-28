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
    public abstract class Menu : GameElement
    {
        protected MenuItem title;
        protected Menu parent;
        protected Texture2D background;
        public List<MenuItem> menuItems = new List<MenuItem>();
        private bool mouse = true;
        public int selectionIndex = 0;
        public Rectangle backgroundConstraints;

        public Menu(Menu p)
        {
            parent = p;
            backgroundConstraints = new Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight);
        }

        public virtual void Update(GameTime gt)
        {
            foreach (MenuItem m in menuItems)
                m.Update(gt);
            if(title != null)
                title.Update(gt);
            backgroundConstraints = new Rectangle(backgroundConstraints.X, backgroundConstraints.Y, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight);
            if (ControlManager.ControlType == ControlManager.ControlMethod.KeyboardMouse && mouse)
            {
                Vector2 mouseVector = ControlManager.mouseVector;
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (menuItems[i].Selectable)
                    {
                        if (menuItems[i].mouseSelectionCheck(mouseVector))
                        {
                            selectionIndex = i;
                            if (ControlManager.FIRE.Bumped)
                            {
                                menuItems[selectionIndex].Action();
                            }
                        }
                    }
                }
                if (ControlManager.UP.Pressed || ControlManager.DOWN.Pressed || ControlManager.LEFT.Pressed || ControlManager.RIGHT.Pressed)
                {
                    mouse = false;
                }
            }
            else
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (i == selectionIndex)
                        menuItems[i].selected = true;
                    else
                        menuItems[i].selected = false;
                }
                if (ControlManager.UP.Bumped)
                {
                    UPPressed();
                }
                else if (ControlManager.DOWN.Bumped)
                {
                    DOWNPressed();
                }
                else if (ControlManager.RIGHT.Bumped)
                {
                    RIGHTPressed();
                }
                else if (ControlManager.LEFT.Bumped)
                {
                    LEFTPressed();
                }
                if (ControlManager.FIRE.Pressed || ControlManager.ALT_FIRE.Pressed)
                {
                    mouse = true;
                }
            }
            if (ControlManager.MENU_SELECT.Bumped)
            {
                MENU_SELECTPressed();
            }
            if (ControlManager.MENU_RETURN.Bumped)
            {
                MENU_RETURNPressed();
            }
        }

        protected abstract void UPPressed();

        protected abstract void DOWNPressed();

        protected abstract void RIGHTPressed();

        protected abstract void LEFTPressed();

        protected virtual void MENU_SELECTPressed()
        {
            try
            {
                menuItems[selectionIndex].Action();
            }
            catch (ArgumentOutOfRangeException problem)
            {
                //Problem ABORT
            }
        }

        protected virtual void MENU_RETURNPressed()
        {
            Return();
        }

        public virtual void Return()
        {
            if (parent == null)
                Global.GM.Exit();
            else
                Global.CurrentMenu = parent;
        }

        public virtual void Draw(GameTime gt, SpriteBatch sb)
        {
            if(background != null)
                sb.Draw(background, backgroundConstraints, Color.White);
            foreach (MenuItem m in menuItems)
                m.Draw(gt, sb);
            if(title != null)
                title.Draw(gt, sb);
        }

        public void Reset()
        {
        }
    }

    public abstract class MenuItem : GameElement
    {
        protected Rectangle selectionBox;
        protected Menu parent;
        public bool Selectable;
        public bool selected;
        public Vector2 relativeLoc;

        public MenuItem(Menu p, Vector2 rl)
        {
            parent = p;
            relativeLoc = rl;
        }

        public bool mouseSelectionCheck(Vector2 mouseVector)
        {
            if(Selectable)
            {
                selected = selectionBox.Contains(Util.vectorToPoint(mouseVector));
            }
            return selected;
        }

        public virtual void Update(GameTime gt)
        {
            selectionBox = new Rectangle(
                (int)(Global.Graphics.PreferredBackBufferWidth * relativeLoc.X) - (int)(0.5 * selectionBox.Width), 
                (int)(Global.Graphics.PreferredBackBufferHeight * relativeLoc.Y) - (int)(0.5 * selectionBox.Height),
                selectionBox.Width, 
                selectionBox.Height);
        }

        public abstract void Draw(GameTime gt, SpriteBatch sb);

        public abstract void Action();

        public void Reset()
        {
        }
    }

    public class TextureMenuItem : MenuItem
    {
        private Texture2D texture;
        protected float baseScaling;
        protected float scaling;
        protected Color colorMask;

        public TextureMenuItem(Menu p, Vector2 rl, Texture2D tex, float s, Color color) : base(p, rl)
        {
            texture = tex;
            baseScaling = s;
            colorMask = color;
            selectionBox = new Rectangle(0, 0, (int)(tex.Width * baseScaling), (int)(tex.Height * baseScaling));
        }

        public override void Update(GameTime gt)
        {
            scaling = baseScaling * ((float)Global.Graphics.PreferredBackBufferWidth / 800f);
            selectionBox.Width = (int)(scaling * texture.Width);
            selectionBox.Height = (int)(scaling * texture.Height);
            base.Update(gt);
        }

        public override void Draw(GameTime gt, SpriteBatch sb)
        {
            sb.Draw(texture, selectionBox, colorMask);
        }

        public override void Action()
        {
            throw new NotImplementedException();
        }
    }

    public class TextMenuItem : MenuItem
    {
        public string text;
        public SpriteFont spriteFont;
        public Color color;

        public TextMenuItem(Menu p, Vector2 rl, string t, SpriteFont sf, Color col)
            : base(p, rl)
        {
            text = t;
            spriteFont = sf;
            color = col;
            Vector2 stringSize = sf.MeasureString(new StringBuilder(t));
            selectionBox = new Rectangle(0, 0, (int)stringSize.X, (int)stringSize.Y);
        }

        public override void Update(GameTime gt)
        {
            Vector2 stringSize = spriteFont.MeasureString(new StringBuilder(text));
            selectionBox.Width = (int)stringSize.X;
            selectionBox.Height = (int)stringSize.Y;
            base.Update(gt);
        }

        public override void Draw(GameTime gt, SpriteBatch sb)
        {
            sb.DrawString(spriteFont, text, new Vector2(selectionBox.X, selectionBox.Y), color);
        }

        public override void Action()
        {
            throw new NotImplementedException();
        }
    }

    public class SelectionMenuItem : TextMenuItem
    {
        public LeftArrowItem lai;
        public RightArrowItem rai;
        public OptionsMenuItem isi;
        private Color selectionColor;

        public SelectionMenuItem(Menu p, Vector2 rl, string t, SpriteFont sf, Color col, Color selectCol, Vector2 offset, float spacing, string[] opt)
            : base(p, rl, t, sf, col)
        {
            lai = new LeftArrowItem(this, offset, spacing, selectCol);
            rai = new RightArrowItem(this, offset, spacing, selectCol);
            isi = new OptionsMenuItem(this, offset, selectCol, opt);
            parent.menuItems.Add(lai);
            parent.menuItems.Add(rai);
            parent.menuItems.Add(isi);
            selectionColor = selectCol;
            Selectable = true;
        }

        public override void Action()
        {
            selected = true;
        }

        public void change(int change)
        {
            isi.change(change);
        }

        public bool contains(TextMenuItem t)
        {
            return (t == lai) || (t == rai) || (t == isi) || (t == this);
        }

        public string SelectedOption
        {
            get { return isi.getSelectedOption(); }
        }

        public class LeftArrowItem : TextMenuItem
        {
            private SelectionMenuItem parentMenuItem;
            private Color normalColor;
            private Color selectionColor;

            public LeftArrowItem(SelectionMenuItem p, Vector2 offset, float spacing, Color selCol)
                : base(p.parent, p.relativeLoc + new Vector2(offset.X - spacing, offset.Y), "<", p.spriteFont, p.color)
            {
                this.parentMenuItem = p;
                selectionColor = selCol;
                normalColor = p.color;
                Selectable = true;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if (selected)
                    color = selectionColor;
                else
                    color = normalColor;
            }

            public override void Action()
            {
                parentMenuItem.change(-1);
            }
        }

        public class RightArrowItem : TextMenuItem
        {
            private SelectionMenuItem parentMenuItem;
            private Color normalColor;
            private Color selectionColor;

            public RightArrowItem(SelectionMenuItem p, Vector2 offset, float spacing, Color selCol)
                : base(p.parent, p.relativeLoc + new Vector2(offset.X + spacing, offset.Y), ">", p.spriteFont, p.color)
            {
                this.parentMenuItem = p;
                selectionColor = selCol;
                normalColor = p.color;
                Selectable = true;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if (selected)
                    color = selectionColor;
                else
                    color = normalColor;
            }

            public override void Action()
            {
                parentMenuItem.change(1);
            }
        }

        public class OptionsMenuItem : TextMenuItem
        {            
            private SelectionMenuItem parentMenuItem;
            private Color normalColor;
            private Color selectionColor;
            private string[] options;
            private int index;

            public OptionsMenuItem(SelectionMenuItem p, Vector2 offset, Color selCol, string[] opt)
                : base(p.parent, p.relativeLoc + offset, opt[0], p.spriteFont, p.color)
            {
                this.parentMenuItem = p;
                selectionColor = selCol;
                normalColor = p.color;
                Selectable = true;
                options = opt;
                index = 0;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                text = options[index];
                if (selected)
                    color = selectionColor;
                else
                    color = normalColor;
            }

            public void change(int change)
            {
                index += change;
                if (index >= options.Length)
                    index = index % options.Length;
                else if (index < 0)
                    index = options.Length + index;
            }

            public string getSelectedOption()
            {
                return options[index];
            }

            public void setOption(string option)
            {
                for (int i = 0; i < options.Length; i++)
                    if (options[i] == option)
                        index = i;
            }

            public override void Action()
            {
                //Nothing
            }
        }
    }
}
