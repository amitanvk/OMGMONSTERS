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
    public class LevelSelectMenu : Menu
    {
        SelectionMenuItem selectItem;
        public LevelSelectMenu(Menu p) : base(p)
        {
            background = Global.Textures["Level Select"];
            selectItem = new SelectionMenuItem(
                this,
                new Vector2(0.5f, 0.15f),
                "Select Level: ",
                Global.Fonts["Pause Font"],
                Color.Black,
                Color.Red,
                new Vector2(0.0f, 0.35f),
                0.45f,
                new string[]{
                    "Easy",
                    "Medium",
                    "Impossible"
                });
            menuItems.Add(selectItem);
        }

        protected override void UPPressed()
        {
            //Do nothing
        }

        protected override void DOWNPressed()
        {
            //Do nothing
        }

        protected override void LEFTPressed()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i] == selectItem.lai)
                {
                    selectionIndex = i;
                    break;
                }
            }
            selectItem.change(-1);
        }

        protected override void RIGHTPressed()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i] == selectItem.rai)
                {
                    selectionIndex = i;
                    break;
                }
            }
            selectItem.change(1);
        }

        protected override void MENU_SELECTPressed()
        {
            switch (selectItem.SelectedOption)
            {
                case "Easy":
                    Global.GM.startNewGame(1);
                    break;
                case "Medium":
                    Global.GM.startNewGame(2);
                    break;
                case "Impossible":
                    Global.GM.startNewGame(3);
                    break;
            }
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
