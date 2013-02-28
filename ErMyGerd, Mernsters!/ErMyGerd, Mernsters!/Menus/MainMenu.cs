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
    public class MainMenu : Menu
    {
        public MainMenu(Menu p) : base(p)
        {
            const float buttonSpacing = 0.1f;
            title = new TextureMenuItem(this, new Vector2(0.5f, 0.15f), Global.Textures["Omg Monsters Logo"], 0.75f, Color.Black);
            background = Global.Textures["Title Background"];
            float x = 0.135f;
            float y = 0.4f;
            if (parent is PauseMenu)
                menuItems.Add(new ContinueButton(this, new Vector2(x, y)));
            else
                menuItems.Add(new NewGameButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new InstructionButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new CreditsButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new OptionsButton(this, new Vector2(x, y)));
            y += buttonSpacing;
            menuItems.Add(new ExitButton(this, new Vector2(x, y)));
        }

        private abstract class MainMenuItem : TextureMenuItem
        {
            private Texture2D selectedTexture;

            public MainMenuItem(Menu p, Vector2 pos, Texture2D tex, float s, Color c)
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

        private class NewGameButton : MainMenuItem
        {
            public NewGameButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["New Game"], 0.5f, Color.Black){}

            public override void Action()
            {
                Global.CurrentMenu = new LevelSelectMenu(parent);
            }
        }

        private class ContinueButton : MainMenuItem
        {
            public ContinueButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Continue"], 0.5f, Color.Black) { }

            public override void Action()
            {
                Global.GameState = GameState.PAUSED;
                parent.Return();
            }
        }

        private class InstructionButton : MainMenuItem
        {
            public InstructionButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Instructions"], 0.5f, Color.Black) { }

            public override void Action()
            {
                Global.CurrentMenu = new InstructionMenu(parent);
            }
        }

        private class CreditsButton : MainMenuItem
        {
            public CreditsButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Credits"], 0.5f, Color.Black) { }

            public override void Action()
            {
                Global.CurrentMenu = new CreditsMenu(parent);
            }
        }

        private class OptionsButton : MainMenuItem
        {
            public OptionsButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Options"], 0.5f, Color.Black) { }

            public override void Action()
            {
                Global.CurrentMenu = new OptionsMenu(parent);
            }
        }

        private class ExitButton : MainMenuItem
        {
            public ExitButton(Menu p, Vector2 pos) : base(p, pos, Global.MenuButtons["Exit Game"], 0.5f, Color.Black) { }

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

        public override void Return()
        {
            if (parent == null)
                Global.GM.Exit();
            else
                Global.CurrentMenu = parent;
        }
    }
}
