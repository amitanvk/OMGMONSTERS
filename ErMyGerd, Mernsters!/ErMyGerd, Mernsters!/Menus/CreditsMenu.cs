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
    public class CreditsMenu : Menu
    {
        public CreditsMenu(Menu p) : base(p)
        {
            background = Global.Textures["Credits"];
            backgroundConstraints = new Rectangle(0, 0, Global.Graphics.PreferredBackBufferWidth, background.Height);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            backgroundConstraints = new Rectangle(0, backgroundConstraints.Y, Global.Graphics.PreferredBackBufferWidth, background.Height);
            if (-backgroundConstraints.Y + Global.Graphics.PreferredBackBufferHeight < background.Height)
            {
                backgroundConstraints.Y -= 1;
            }
            else if (-backgroundConstraints.Y + Global.Graphics.PreferredBackBufferHeight > background.Height)
            {
                backgroundConstraints.Y = -(background.Height - Global.Graphics.PreferredBackBufferHeight);
            }
        }

        protected override void DOWNPressed()
        {
        }

        protected override void UPPressed()
        {
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
