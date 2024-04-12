using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.UI;

internal class CustomNamingMenu : NamingMenu
{
    public CustomNamingMenu(doneNamingBehavior b, string title, string defaultName = null)
        : base(b, title, defaultName)
    { }

    public override void receiveKeyPress(Keys key)
    {
        if (ModEntry.Instance.Config.MenuClose == key)
        {
            if (ModEntry.Instance.Locations.Count > 0)
                base.exitThisMenu();
            else
                Game1.exitActiveMenu();
            return;
        }

        base.receiveKeyPress(key);
    }
}
