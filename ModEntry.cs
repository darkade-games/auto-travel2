using System.Threading.Tasks;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using AutoTravel2.UI;
using System.Collections.Generic;
using AutoTravel2.Integration;
using Microsoft.Xna.Framework.Input;

namespace AutoTravel2;

public sealed class ModEntry : Mod
{
    public ModConfig Config;
    public static ModEntry Instance;
    public IScreenReader ScreenReader;
    private IModHelper helper;
    public List<TravelLocation> Locations = new List<TravelLocation>();

    public override void Entry(IModHelper helper)
    {
        this.helper = helper;
        Instance = this;

        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Input.MouseWheelScrolled += OnMouseScroll;

        Config = helper.ReadConfig<ModConfig>();
    }

    private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        if (this.Helper.Data.ReadGlobalData<List<TravelLocation>>("Character_" + Game1.player.Name) is List<TravelLocation> previousLocations)
        {
            Locations = previousLocations;
        }
    }

    public void SaveLocations()
    {
        this.Helper.Data.WriteGlobalData("Character_" + Game1.player.Name, Locations);
    }

    public void AddLocation(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        if (Locations.Any(location => location.name == name))
        {
            ScreenReader.SayWithMenuChecker(Config.PhraseLocationExists.Replace("{name}", name), true);
            return;
        }

        Locations.Add(new(name, Game1.player.Position, Game1.currentLocation.NameOrUniqueName, Game1.player.FacingDirection));
        ScreenReader.SayWithMenuChecker(Config.PhraseLocationCreated.Replace("{name}", name), true);
        SaveLocations();
    }

    public void RemoveLocation(TravelLocation location)
    {
        Locations.Remove(location);
        ScreenReader.SayWithMenuChecker(Config.PhraseLocationDeleted.Replace("{name}", location.name), true);
        Game1.playSound("shwip");
        SaveLocations();
    }

    public void WarpPlayer(TravelLocation location)
    {
        Warp warp = new Warp(0, 0, location.region, (int)(location.position.X + 16f) / 64, (int)location.position.Y / 64, false, false);
        Game1.player.warpFarmer(warp, location.facingDirection);

        Task.Run(async delegate
        {
            while (Game1.isWarping)
            {
                await Task.Delay(500);
            }
            if (location.region == "FarmHouse")
            {
                Game1.player.warpFarmer(warp, location.facingDirection);
                while (Game1.isWarping)
                {
                    await Task.Delay(500);
                }
            }
            ScreenReader.SayWithMenuChecker(Config.PhraseFinishedTravel, true);
            Game1.player.faceDirection(location.facingDirection);
        });
    }

    private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        if (helper.ModRegistry.IsLoaded("shoaib.stardewaccess"))
        {
            ScreenReader = Helper.ModRegistry.GetApi<IScreenReader>("shoaib.stardewaccess");
        }
    }

    private void OnMouseScroll(object sender, MouseWheelScrolledEventArgs e)
    {
        if (!Config.EnableMouseMenuScroll || e.Delta == 0) return;

        if (Game1.activeClickableMenu is AutoTravelMenu autoTravelMenu)
        {
            (e.Delta > 0 ? Config.MenuUpButtons[0] : Config.MenuDownButtons[0]).TryGetKeyboard(out Keys key);
            autoTravelMenu.receiveKeyPress(key);
        }
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (Game1.activeClickableMenu != null) return;
        if (!Context.IsPlayerFree) return;

        if (e.Button == Config.OpenMenuButton)
        {
            AutoTravelMenu autoTravelMenu = new AutoTravelMenu();
            Game1.activeClickableMenu = autoTravelMenu;

            if (Locations.Count > 0)
            {
                TravelLocation[] locations = Locations.OrderByDescending(l => l.favorite).ThenBy(l => l.name).ToArray();

                if (locations.Length == 0)
                {
                    autoTravelMenu.SetChildMenu(new CustomNamingMenu());
                }
                else
                {
                    autoTravelMenu.SelectedLocation = locations[0];
                    autoTravelMenu.SaySelectedLocation(true);
                }
            }
        }
    }

    public TravelLocation GetPreviousLocation(TravelLocation currentLocation)
    {
        TravelLocation[] locations = Locations.OrderByDescending(l => l.favorite).ThenBy(l => l.name).ToArray();

        for (int i = 0; i < locations.Length; i++)
        {
            int last_index = i == 0 ? locations.Length - 1 : i - 1;
            TravelLocation thisLocation = locations[i];

            if (thisLocation.Equals(currentLocation))
            {
                return locations[last_index];
            }
        }
        return null;
    }

    public TravelLocation GetNextLocation(TravelLocation currentLocation)
    {
        TravelLocation[] locations = Locations.OrderByDescending(l => l.favorite).ThenBy(l => l.name).ToArray();

        for (int i = 0; i < locations.Length; i++)
        {
            int next_index = i == locations.Length - 1 ? 0 : i + 1;
            TravelLocation thisLocation = locations[i];

            if (thisLocation.Equals(currentLocation))
            {
                return locations[next_index];
            }
        }
        return null;
    }
}
