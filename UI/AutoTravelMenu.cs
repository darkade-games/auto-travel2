using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.UI
{
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
        public bool IsInputtingNewDestination = false;
        public TextInputBox createDestinationInput;
        public ModEntry Mod;

        public AutoTravelMenu() : base((int)GetMenuPosition().X, (int)GetMenuPosition().Y, menuWidth, menuHeight, false)
        {
            Mod = ModEntry.Instance;
            createDestinationInput = new TextInputBox(null, null, Game1.dialogueFont, Game1.textColor);
            Game1.keyboardDispatcher.Subscriber = createDestinationInput;
            createDestinationInput.Selected = false;
            createDestinationInput.OnBackspacePressed += this.SearchBox_OnBackspacePressed;
        }

        public void ReceiveInput(SButton button)
        {
            if (createDestinationInput.Selected) return;
            if (Mod.Config.MenuUpButtons.Contains(button)) ChangeMenu(MenuDirection.Up);
            else if (Mod.Config.MenuDownButtons.Contains(button)) ChangeMenu(MenuDirection.Down);
            else if (button == Mod.Config.CreateDestinationButton) ShowPlayerInput();
        }

        public void ShowPlayerInput()
        {
            createDestinationInput.Update();
            IsInputtingNewDestination = true;
            if (!createDestinationInput.Selected) createDestinationInput.SelectMe();
            Mod.ScreenReader.SayWithMenuChecker(Mod.Config.PhraseCreateLocationPrompt, true);
        }

        public void ClosePlayerInput()
        {
            IsInputtingNewDestination = false;
            createDestinationInput.Selected = false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (createDestinationInput.Selected)
            {
                if (key == Mod.Config.MenuClose)
                {
                    CloseMenu();
                    Game1.exitActiveMenu();
                }
                if (key == Mod.Config.MenuSubmit)
                {
                    string destination_name = createDestinationInput.Text;
                    Mod.AddLocation(destination_name);
                    CloseMenu();
                    Game1.exitActiveMenu();
                }
                return;
            }
            else
            {
                if (SelectedLocation != null)
                {
                    if (key == Mod.Config.MenuSubmit)
                    {
                        Mod.WarpPlayer(SelectedLocation);
                        CloseMenu();
                        Game1.exitActiveMenu();
                    } else if (key == Mod.Config.MenuDelete) {
                        Mod.RemoveLocation(SelectedLocation);
                        CloseMenu();
                        Game1.exitActiveMenu();
                    } else if (key == Mod.Config.FavoriteToggleKey)
                    {
                        SelectedLocation.favorite = !SelectedLocation.favorite;

                        if(SelectedLocation.favorite)
                        {
                            Mod.ScreenReader.SayWithMenuChecker(Mod.Config.PhraseLocationFavorited.Replace("{name}", SelectedLocation.name), true);
                        } else
                        {
                            Mod.ScreenReader.SayWithMenuChecker(Mod.Config.PhraseLocationUnfavorited.Replace("{name}", SelectedLocation.name), true);
                        }
                    }
                }
            }
            base.receiveKeyPress(key);
        }

        public void CloseMenu()
        {
            createDestinationInput.Text = "";
            createDestinationInput.Selected = false;
            IsInputtingNewDestination = false;
            SelectedLocation = null;
        }

        private void SearchBox_OnBackspacePressed(TextBox sender)
        {
            createDestinationInput.backSpacePressed();
        }

        public void SaySelectedLocation(bool first = false)
        {
            if (SelectedLocation != null)
            {
                string say = Mod.Config.PhraseMenuSelectPrompt.Replace("{name}", SelectedLocation.name);
                if (first) say = Mod.Config.PhraseMenuSelectPromptOpenPrefix + say;
                Mod.ScreenReader.SayWithMenuChecker(say, true);
            }
        }

        public override void draw(SpriteBatch b)
        {
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

            if (!IsInputtingNewDestination && Mod.Locations.Count() == 0) ShowPlayerInput();

            createDestinationInput.X = (int)(Game1.viewport.Width / 2 - menuWidth / 2 + borderWidth - 5);
            createDestinationInput.Y = (int)(Game1.viewport.Height / 2 - 15 + -110);
            createDestinationInput.Width = this.width - 85;
            createDestinationInput.Height = 192;

            if (IsInputtingNewDestination)
            {
                createDestinationInput.Draw(b, true);
                b.DrawString(Game1.dialogueFont, "New Destination", new Vector2(Game1.viewport.Width / 2 - menuWidth / 2 + borderWidth, Game1.viewport.Height / 2 - 15 + -116), Game1.textColor, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }

            drawMouse(b, false, -1);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (IsInputtingNewDestination)
            {
                Rectangle r = new Rectangle(createDestinationInput.X, createDestinationInput.Y, createDestinationInput.Width, createDestinationInput.Height / 2);
                if (r.Contains(x, y))
                {
                    ShowPlayerInput();
                }
                else
                {
                    createDestinationInput.Selected = false;
                    ClosePlayerInput();
                }
            }
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
}