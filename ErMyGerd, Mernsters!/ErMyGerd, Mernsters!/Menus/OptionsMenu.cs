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
    public class OptionsMenu : Menu
    {
        SelectionMenuItem controlItem;
        SelectionMenuItem windowTypeItem;
        SelectionMenuItem resolutionItem;
        public OptionsMenu(Menu p) : base(p)
        {
            background = Global.Textures["Level Select"];
            title = new TextMenuItem(this, new Vector2(0.5f, 0.15f), "Options", Global.Fonts["Pause Font"], Color.Black);
            controlItem = new SelectionMenuItem(
                this,
                new Vector2(0.25f, 0.3f),
                "Control Type:",
                Global.Fonts["Pause Font"],
                Color.Black,
                Color.Red,
                new Vector2(0.45f, 0f),
                0.25f,
                new string[]{
                    "Keyboard/Mouse",
                    "Xbox"});
            windowTypeItem = new SelectionMenuItem(
                this,
                new Vector2(0.25f, 0.4f),
                "Fullscreen:",
                Global.Fonts["Pause Font"],
                Color.Black,
                Color.Red,
                new Vector2(0.45f, 0.0f),
                0.25f,
                new string[]{
                    "Windowed",
                    "Fullscreen"});
            resolutionItem = new SelectionMenuItem(
                    this,
                    new Vector2(0.25f, 0.5f),
                    "Resolution:",
                    Global.Fonts["Pause Font"],
                    Color.Black,
                    Color.Red,
                    new Vector2(0.45f, 0.0f),
                    0.25f,
                    new string[]{
                    "320x200",
                    "320x240",
                    "400x300",
                    "512x384",
                    "640x400",
                    "640x480",
                    "800x600",
                    "1024x768",
                    "1280x600",
                    "1280x720",
                    "1280x768",
                    "1360x768",
                    "1366x768"});
            menuItems.Add(controlItem);
            menuItems.Add(windowTypeItem);
            menuItems.Add(resolutionItem);
        }

        protected override void DOWNPressed()
        {
            while (!(menuItems[selectionIndex] is SelectionMenuItem))
            {
                if (selectionIndex + 1 < menuItems.Count)
                {
                    selectionIndex++;
                }
            }
        }

        protected override void UPPressed()
        {
            while (!(menuItems[selectionIndex] is SelectionMenuItem))
            {
                if (selectionIndex - 1 >= 0)
                {
                    selectionIndex--;
                }
            }
        }
        protected override void LEFTPressed()
        {
            if(menuItems[selectionIndex] is TextMenuItem)
                for (int i = 0; i < menuItems.Count; i++)
                    if (menuItems[i] is SelectionMenuItem)
                        if ((menuItems[i] as SelectionMenuItem).contains(menuItems[selectionIndex] as TextMenuItem))
                            (menuItems[i] as SelectionMenuItem).change(-1);
        }

        protected override void RIGHTPressed()
        {
            if (menuItems[selectionIndex] is TextMenuItem)
                for (int i = 0; i < menuItems.Count; i++)
                    if (menuItems[i] is SelectionMenuItem)
                        if ((menuItems[i] as SelectionMenuItem).contains(menuItems[selectionIndex] as TextMenuItem))
                            (menuItems[i] as SelectionMenuItem).change(1);
        }

        protected override void MENU_SELECTPressed()
        {
            Return();
        }

        public override void Return()
        {
            int x = 0, y = 0;
            switch (resolutionItem.SelectedOption)
            {
                case "320x200":
                    x = 320;
                    y = 200;
                    break;
                case "320x240":
                    x = 320;
                    y = 240;
                    break;
                case "400x300":
                    x = 400;
                    y = 300;
                    break;
                case "512x384":
                    x = 512;
                    y = 384;
                    break;
                case "640x400":
                    x = 640;
                    y = 400;
                    break;
                case "640x480":
                    x = 640;
                    y = 480;
                    break;
                case "800x600":
                    x = 800;
                    y = 600;
                    break;
                case "1024x768":
                    x = 1024;
                    y = 768;
                    break;
                case "1280x600":
                    x = 1280;
                    y = 600;
                    break;
                case "1280x720":
                    x = 1280;
                    y = 780;
                    break;
                case "1280x768":
                    x = 1280;
                    y = 768;
                    break;
                case "1360x768":
                    x = 1360;
                    y = 768;
                    break;
                case "1366x768":
                    x = 1366;
                    y = 768;
                    break;
            }
            Global.Graphics.PreferredBackBufferWidth = x;
            Global.Graphics.PreferredBackBufferHeight = y;
            switch (windowTypeItem.SelectedOption)
            {
                case "Fullscreen":
                    Global.Graphics.IsFullScreen = true;
                    break;
                case "Windowed":
                    Global.Graphics.IsFullScreen = false;
                    break;
            }
            switch (controlItem.SelectedOption)
            {
                case "Keyboard/Mouse":
                    ControlManager.ControlType = ControlManager.ControlMethod.KeyboardMouse;
                    break;
                case "Xbox":
                    ControlManager.ControlType = ControlManager.ControlMethod.Xbox;
                    break;
            }
            if (parent == null)
                Global.GM.Exit();
            else
                Global.CurrentMenu = parent;
        }
    }
}
