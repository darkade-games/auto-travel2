using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoTravel2.Commands;

public class CommandManager
{
    private static Dictionary<string, CustomCommandsDelegate> _commands = new();
    internal static Dictionary<string, CustomCommandsDelegate> Commands { get => _commands; }

    public static void Travel(string[] args, bool fromChatBox = false)
    {
        if (args.Length < 1) return;

        switch (args[0])
        {
            case "help":
                string a = "- travel help - Displays this help menu";
                string b = "- travel create {name} - Creates a travel location to your current position, with a name you can use to travel to it later.";
                string c = "- travel remove {name} - Removes a travel location you have created.";
                string d = "- travel list - Lists travel locations you have created";
                string e = "- travel move {name} - Travel to a saved travel location by name. Ignores case and supports partial matches.";
                string f = "- travel {name} - Travel to a saved travel location by name.  Ignores case and supports partial matches.";

                SendMessage($"{a}\n{b}\n{c}\n{d}\n{e}\n{f}", fromChatBox, false);
                break;
            case "create":
                {
                    if (args.Length < 2)
                    {
                        SendMessage("Please provide the location's name", fromChatBox, true);
                        return;
                    }
                    string name = args[1];
                    ModEntry.Instance.AddLocation(name);
                    SendMessage($"Location {name}, added.", fromChatBox, false);
                    break;
                }
            case "remove":
                {
                    if (args.Length < 2)
                    {
                        SendMessage("Please provide the location's name", fromChatBox, true);
                        return;
                    }
                    string name = args[1];
                    TravelLocation? moveLocation = ModEntry.Instance.Locations.Find(x => x.name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    ModEntry.Instance.RemoveLocation(moveLocation);
                    SendMessage($"Location {name}, removed.", fromChatBox, false);
                    break;
                }
            case "list":
                string toPrint = "";
                foreach (TravelLocation loc in ModEntry.Instance.Locations)
                {
                    toPrint += $"- {loc.name}\n";
                }
                SendMessage(toPrint, fromChatBox, false);
                break;
            case "move":
                {
                    if (args.Length < 2)
                    {
                        SendMessage("Please provide the location's name", fromChatBox, true);
                        return;
                    }
                    string name = args[1];
                    TravelLocation? moveLocation = ModEntry.Instance.Locations.Find(x => x.name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    ModEntry.Instance.WarpPlayer(moveLocation);
                    break;
                }
            default:
                {
                    if (args.Length < 1)
                    {
                        SendMessage("Please provide the location's name", fromChatBox, true);
                        return;
                    }
                    string name = args[0];
                    TravelLocation? moveLocation = ModEntry.Instance.Locations.Find(x => x.name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    ModEntry.Instance.WarpPlayer(moveLocation);
                    break;
                }
        }
    }

    private static void SendMessage(string message, bool fromChatBox, bool error)
    {
        if (fromChatBox)
        {
            if (error) Game1.chatBox.addErrorMessage(message);
            else Game1.chatBox.addInfoMessage(message);
        }
        else
        {
            if (error) Log.Info(message);
            else Log.Error(message);
        }
    }

    /// <summary>
    /// Registers the custom commands to the ConsoleCommands to be used in the terminal.
    /// Also adds them to the Commands property which is used in the ChatBox patch to allow commands to be executed via chat messages.
    /// </summary>
    public static void Register(IModHelper modHelper, Harmony harmony)
    {
        try
        {
            MethodInfo method = typeof(CommandManager).GetMethod("Travel")!;
            string commandName = method.Name.ToLower();

            // Store the delegate into the dictionary to be used in the chat box.
            _commands.Add(commandName, (CustomCommandsDelegate)method.CreateDelegate(typeof(CustomCommandsDelegate)));

            // Add the command to ConsoleCommands to be able to use it in terminals as usual.
            modHelper.ConsoleCommands.Add(commandName, "Testing...", (_, args) => method.Invoke(null, new object[] { args, false }));

            harmony.Patch(
               original: AccessTools.DeclaredMethod(typeof(ChatBox), "runCommand"),
               prefix: new HarmonyMethod(typeof(CommandManager), nameof(CommandManager.RunCommandPatch))
            );

            Log.Verbose($"CommandManager: Registered command with name: {commandName}");
        }
        catch (System.Exception)
        {
            Log.Error($"CommandManager: Error occurred while registering command");
        }
    }

    private static bool RunCommandPatch(ChatBox __instance, string command)
    {
        try
        {
            string[] commandWithArgs = ArgUtility.SplitBySpaceQuoteAware(command);
            string commandName = commandWithArgs[0];
            string[] args = commandWithArgs.Skip(1).ToArray();

            if (Commands.TryGetValue(commandName, out var delegateMethod))
            {
                try
                {
                    Log.Verbose($"ChatBoxPatch->RunCommandPatch: Found a custom command named {commandName}, invoking...");
                    delegateMethod(args, true);
                }
                catch (Exception ee)
                {
                    __instance.addErrorMessage($"An error occured while executing {commandName} command");
                    Log.Error($"Error in {commandName}: {ee.StackTrace}");
                }
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in ChatBoxPatch->RunCommandPatch:\n{e.Message}\n{e.StackTrace}");
        }

        return true;
    }
}
