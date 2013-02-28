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
    public class UI : DrawableGameElement, GameElement
    {
        private static Texture2D HPBar;
        private static Texture2D MPBar;
        Rectangle HPBarRect, AmmoBarRect;
        public void Initialize() { }
        public UI()
        {
            Texture = Global.Textures["Ui Frame"];
            Position = Vector2.Zero;
            HPBar = Global.Textures["Hp Bar"];
            MPBar = Global.Textures["Mp Bar"];
            HPBarRect = new Rectangle(141, 4, HPBar.Width, HPBar.Height);
            AmmoBarRect = new Rectangle(141, 25, MPBar.Width, MPBar.Height);
            RenderLimitBound = false;
            CameraDependent = false;
        }

        public override void Update(GameTime gt)
        {
            HPBarRect.Width = (int)((float)HPBar.Width * (float)((float)Global.Player.HP / 100));
            AmmoBarRect.Width = (int)((float)MPBar.Width * (float)((float)Global.Player.MP / 100));
        }

        protected override void DrawAfter(SpriteBatch sb)
        {
            sb.Draw(HPBar, HPBarRect, Color.White);
            sb.Draw(MPBar, AmmoBarRect, Color.White);
        }
    }
}
