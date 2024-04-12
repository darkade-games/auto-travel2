using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.UI;

internal class CustomNamingMenu : NamingMenu
{
    public CustomNamingMenu()
        : base(NamingMenuCallback, ModEntry.Instance.Config.PhraseCreateLocationPrompt, null)
    {
        this.textBox.Selected = false; // Unselect the text box on menu open
    }

    public static void NamingMenuCallback(string name)
    {
        ModEntry.Instance.AddLocation(name);
        // CloseMenu();
        Game1.exitActiveMenu();
    }

    public override void receiveKeyPress(Keys key)
    {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
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
