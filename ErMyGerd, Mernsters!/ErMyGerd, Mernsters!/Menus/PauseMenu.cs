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

namespace ErMyGerdMernsters.Menus
{
    public class PauseMenu : Menu
    {
        public PauseMenu(Menu p) : base(p)
        {
            const float buttonSpacing = 0.1f;
            title = new TextMenuItem(this, new Vector2(0.5f, 0.3f), "Game Paused", Global.Fonts["Pause Font"], Color.White);
            background = null;
            float x = 0.5f;
            float y = 0.4f;
            menuItems.Add(new MainMenuButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new OptionsButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new ExitButton(this, new Vector2(x, y)));
        }

        private abstract class PauseMenuItem : TextureMenuItem
        {
            private Texture2D selectedTexture;

            public PauseMenuItem(Menu p, Vector2 pos, Texture2D tex, float s, Color c)
                : base(p, pos, tex, s, c)
            {
                Selectable = true;
                selectedTexture = Global.MenuButtons["Menu Selected"];
            }

            public override void Draw(GameTime gt, SpriteBatch sb)
            {
                base.Draw(gt, sb);
                if (selected)
                {
                    sb.Draw(
                        Global.MenuButtons["Menu Selected"],
                        new Vector2(selectionBox.X + (4f * scaling), selectionBox.Y + (4f * scaling)),
                        new Rectangle(0, 0, Global.MenuButtons["Menu Selected"].Width, Global.MenuButtons["Menu Selected"].Height + 2),
                        Color.White,
                        0.0f,
                        new Vector2(0, 0),
                        scaling,
                        SpriteEffects.None,
                        1.0f);
                }
            }
        }

        private class MainMenuButton : PauseMenuItem
        {
            public MainMenuButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Main Menu"], 0.5f, Color.White) { }

            public override void Action()
            {
                Global.CurrentMenu = new MainMenu(parent);
                Global.GameState = GameState.MAIN_MENU;
            }
        }

        private class OptionsButton : PauseMenuItem
        {
            public OptionsButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Options"], 0.5f, Color.White) { }

            public override void Action()
            {
                //need programming for options screen first
            }
        }

        private class ExitButton : PauseMenuItem
        {
            public ExitButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Exit Game"], 0.5f, Color.White) { }

            public override void Action()
            {
                Global.GM.Exit();
            }
        }

        protected override void DOWNPressed()
        {
            if (selectionIndex + 1 < menuItems.Count)
            {
                selectionIndex++;
            }
        }

        protected override void UPPressed()
        {
            if (selectionIndex - 1 >= 0)
            {
                selectionIndex--;
            }
        }

        protected override void LEFTPressed()
        {
        }

        protected override void RIGHTPressed()
        {
        }

        public override void Draw(GameTime gt, SpriteBatch sb)
        {
            Global.GM.fade(sb, Color.Black, 75, new Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight));
            base.Draw(gt, sb);
        }

        public override void Return()
        {
            if (parent == null)
                Global.GM.Exit();
            else
                Global.CurrentMenu = parent;
        }
    }
}
