using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.UI;

// TODO Fix location name not narrating after closing this sub-menu.
internal class CustomNamingMenu : NamingMenu
{
    public CustomNamingMenu()
        : base(NamingMenuCallback, ModEntry.Instance.Config.PhraseCreateLocationPrompt, "")
    {
        // Unselect the text box on menu open if stardew access is installed
        this.textBox.Selected = !ModEntry.Instance.Helper.ModRegistry.IsLoaded("shoaib.stardewaccess");
    }

    public static void NamingMenuCallback(string name)
    {
        ModEntry.Instance.AddLocation(name);
        Game1.exitActiveMenu();
    }

    public override void receiveKeyPress(Keys key)
    {
        if (key == Keys.Escape)
        {
            if (this.textBox.Selected)
            {
                // Unselect the text box if selected instead of closing the sub-menu
                this.textBox.Selected = false;
                return;
            }

            if (ModEntry.Instance.Locations.Count > 0)
                base.exitThisMenu();
            else
                Game1.exitActiveMenu();
            return;
        }

        base.receiveKeyPress(key);
    }
}
