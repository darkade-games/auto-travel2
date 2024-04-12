using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace AutoTravel2;

public class ModConfig
{
    public SButton OpenMenuButton { get; set; } = SButton.V;
    public Keys MenuSubmit { get; set; } = Keys.Enter;
    public Keys MenuDelete { get; set; } = Keys.Back;

    public Keys FavoriteToggleKey { get; set; } = Keys.F;
    public SButton CreateDestinationButton { get; set; } = SButton.N;
    public List<SButton> MenuUpButtons { get; set; } = new List<SButton>() { SButton.W, SButton.Up };
    public List<SButton> MenuDownButtons { get; set; } = new List<SButton>() { SButton.S, SButton.Down };
    public bool EnableMouseMenuScroll { get; set; } = true;

    public string PhraseLocationExists = "Location {name} already exists.";
    public string PhraseLocationCreated = "Location {name} created.";
    public string PhraseLocationDeleted = "Location {name} deleted.";
    public string PhraseFinishedTravel = "Finished Traveling.";
    public string PhraseCreateLocationPrompt = "Enter location name";
    public string PhraseMenuSelectPrompt = "{name} - Press the MenuSubmit button to travel.";
    public string PhraseMenuSelectPromptOpenPrefix = "Auto Travel Opened - ";
    public string PhraseLocationFavorited = "Favorited location {name}";
    public string PhraseLocationUnfavorited = "Unfavorited location {name}";
}
