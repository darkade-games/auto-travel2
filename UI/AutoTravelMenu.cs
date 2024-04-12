using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.UI;

public enum MenuDirection
{
    Up,
    Down
}

public class AutoTravelMenu : IClickableMenu
{
    public static int menuWidth = 600 + borderWidth * 2;
    public static int menuHeight = 100 + borderWidth * 2 + Game1.tileSize;

    public TravelLocation SelectedLocation = null;
    public ModEntry Mod;

    public AutoTravelMenu() : base((int)GetMenuPosition().X, (int)GetMenuPosition().Y, menuWidth, menuHeight, false)
    {
        Mod = ModEntry.Instance;
    }

    public override void receiveKeyPress(Keys key)
    {
        if (GetChildMenu() != null)
        {
            GetChildMenu().receiveKeyPress(key);
            return;
        }

        if (SelectedLocation != null)
        {
            if (Mod.Config.MenuDelete == key)
            {
                if (Mod.Locations.Count > 1)
                {
                    var temp = Mod.GetNextLocation(SelectedLocation);
                    Mod.RemoveLocation(SelectedLocation);
                    SelectedLocation = temp;
                    SaySelectedLocation(interrupt: false);
                }
                else
                {
                    Mod.RemoveLocation(SelectedLocation);
                    Game1.exitActiveMenu();
                }
                return;
            }
            if (Mod.Config.FavoriteToggleKey == key)
            {
                SelectedLocation.favorite = !SelectedLocation.favorite;

                if (SelectedLocation.favorite)
                {
                    Mod.ScreenReader.SayWithMenuChecker(Mod.Config.PhraseLocationFavorited.Replace("{name}", SelectedLocation.name), true);
                }
                else
                {
                    Mod.ScreenReader.SayWithMenuChecker(Mod.Config.PhraseLocationUnfavorited.Replace("{name}", SelectedLocation.name), true);
                }
                return;
            }
            if (Mod.Config.MenuSubmit == key)
            {
                Mod.WarpPlayer(SelectedLocation);
                Game1.exitActiveMenu();
                return;
            }
            if (Mod.Config.MenuUpButtons.Contains(key.ToSButton()))
            {
                ChangeMenu(MenuDirection.Up);
                return;
            }
            if (Mod.Config.MenuDownButtons.Contains(key.ToSButton()))
            {
                ChangeMenu(MenuDirection.Down);
                return;
            }
        }

        if (Mod.Config.CreateDestinationButton.Equals(key.ToSButton()))
        {
            SetChildMenu(new CustomNamingMenu());
            return;
        }

        base.receiveKeyPress(key);
    }

    public void SaySelectedLocation(bool first = false, bool interrupt = true)
    {
        if (SelectedLocation == null) return;

        string say = Mod.Config.PhraseMenuSelectPrompt.Replace("{name}", SelectedLocation.name);
        if (first) say = Mod.Config.PhraseMenuSelectPromptOpenPrefix + say;
        Mod.ScreenReader.SayWithMenuChecker(say, interrupt);
    }

    public override void update(GameTime time)
    {
        if (GetChildMenu() != null)
        {
            GetChildMenu().update(time);
            return;
        }

        base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
        if (GetChildMenu() != null)
        {
            GetChildMenu().draw(b);
            return;
        }

        if (SelectedLocation != null)
        {
            string text = SelectedLocation.name;
            float text_scale = 1;
            float yOffset = 0;
            float xOffset = 0;

            if (text.Length <= 12)
            {
                text_scale = 1.75f;
                yOffset = 15;
            }
            else if (text.Length <= 24)
            {
                text_scale = 1.5f;
                yOffset = -15;
                xOffset = 35;
            }
            else if (text.Length <= 36)
            {
                text_scale = 0.9f;
                yOffset = -8;
                xOffset = 150;
            }

            int base_x = (int)GetMenuPosition().X;
            int base_y = (int)GetMenuPosition().Y;
            Game1.drawDialogueBox(base_x, base_y, width, height, false, true);

            b.DrawString(Game1.dialogueFont, text, new Vector2(Game1.viewport.Width / 2 - menuWidth / 2 + borderWidth + xOffset, Game1.viewport.Height / 2 - 15 + yOffset), Game1.textColor, 0, Vector2.Zero, text_scale, SpriteEffects.None, 0);

            if (SelectedLocation.favorite)
            {
                ClickableTextureComponent c = new ClickableTextureComponent("Favorite", new Rectangle(this.xPositionOnScreen + this.width + 390 - borderWidth - spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height + 250 - borderWidth - spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, new Rectangle(346, 400, 8, 8), 5f);
                c.draw(b);
                //b.DrawString(Game1.dialogueFont, "Favorited", new Vector2(Game1.viewport.Width / 2 - menuWidth / 2 + borderWidth + xOffset, Game1.viewport.Height / 2 - 15 + 125), Game1.textColor, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            }
        }

        if (Mod.Locations.Count == 0) SetChildMenu(new CustomNamingMenu());

        drawMouse(b, false, -1);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (GetChildMenu() != null)
        {
            GetChildMenu().receiveLeftClick(x, y, playSound);
            return;
        }

        base.receiveLeftClick(x, y, playSound);
    }

    public void ChangeMenu(MenuDirection direction)
    {
        TravelLocation NextLocation = direction == MenuDirection.Up ? Mod.GetPreviousLocation(SelectedLocation) : Mod.GetNextLocation(SelectedLocation);
        if (NextLocation != null)
        {
            Game1.playSound("shiny4");
            SelectedLocation = NextLocation;
        }
        SaySelectedLocation();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        xPositionOnScreen = (int)GetMenuPosition().X;
        yPositionOnScreen = (int)GetMenuPosition().Y;
    }

    public static Vector2 GetMenuPosition()
    {
        Vector2 defaultPosition = new Vector2(Game1.viewport.Width / 2 - menuWidth / 2, Game1.viewport.Height / 2 - menuHeight / 2);

        if (defaultPosition.X + menuWidth > Game1.viewport.Width)
        {
            defaultPosition.X = 0;
        }

        if (defaultPosition.Y + menuHeight > Game1.viewport.Height)
        {
            defaultPosition.Y = 0;
        }
        return defaultPosition;
    }
}
