using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErMyGerdMernsters.Menus
{
    public class InstructionMenu : Menu
    {
        public InstructionMenu(Menu p) : base(p)
        {
            background = Global.Textures["Instructions"];
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
