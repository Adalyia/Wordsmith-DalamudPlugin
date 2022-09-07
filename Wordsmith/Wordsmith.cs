﻿global using System;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Numerics;
global using System.Collections.Generic;
global using Dalamud.Logging;

global using Wordsmith.Interfaces;

using Dalamud.Game.Command;
using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Plugin;

using Lumina.Excel.GeneratedSheets;

namespace Wordsmith;

public sealed class Wordsmith : IDalamudPlugin
{
    #region Constants
    /// <summary>
    /// Plugin name.
    /// </summary>
    internal const string AppName = "Wordsmith";

    /// <summary>
    /// Command to open a new or specific scratch pad.
    /// </summary>
    private const string SCRATCH_CMD_STRING = "/scratchpad";

    /// <summary>
    /// Command to open the settings window.
    /// </summary>
    private const string SETTINGS_CMD_STRING = "/wordsmith";

    /// <summary>
    /// Command to open the thesaurus.
    /// </summary>
    private const string THES_CMD_STRING = "/thesaurus";
    #endregion

    /// <summary>
    /// Plugin name interface property.
    /// </summary>
    public string Name => AppName;

    #region Plugin Services
    [PluginService]
    internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static CommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static DataManager DataManager { get; private set; } = null!;
    #endregion

    /// <summary>
    /// <see cref="Configuration"/> holding all configurable data for the plugin.
    /// </summary>
    internal static Configuration Configuration { get; private set; } = null!;

    #region Constructor and Disposer
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Wordsmith()
    {
        // Get the configuration.
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // Add commands
        CommandManager.AddHandler(THES_CMD_STRING, new CommandInfo(this.OnMainCommand) { HelpMessage = "Display the thesaurus window." });
        CommandManager.AddHandler(SETTINGS_CMD_STRING, new CommandInfo(this.OnSettingsCommand) { HelpMessage = "Display the configuration window." });
        CommandManager.AddHandler(SCRATCH_CMD_STRING, new CommandInfo(this.OnScratchCommand) { HelpMessage = "Opens a new scratch pad or a specific pad if number given i.e. /scratchpad 5" });

        // Register handlers for draw and openconfig events.
        PluginInterface.UiBuilder.Draw += WordsmithUI.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += WordsmithUI.ShowSettings;

        // Initialize the dictionary.
        Data.Lang.Init();
    }

    /// <summary>
    /// Disposal method for cleaning the plugin.
    /// </summary>
    public void Dispose()
    {
        // Remove events.
        PluginInterface.UiBuilder.Draw -= WordsmithUI.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= WordsmithUI.ShowSettings;

        // Remove command handlers.
        CommandManager.RemoveHandler(THES_CMD_STRING);
        CommandManager.RemoveHandler(SETTINGS_CMD_STRING);
        CommandManager.RemoveHandler(SCRATCH_CMD_STRING);

        // Dispose of the UI
        WordsmithUI.Dispose();
    }
    #endregion
    #region Event Callbacks
    private void OnMainCommand(string command, string args) => WordsmithUI.ShowThesaurus();
    private void OnSettingsCommand(string command, string args) => WordsmithUI.ShowSettings();
    private void OnScratchCommand(string command, string args)
    {
        int x;
        if (int.TryParse(args.Trim(), out x))
            WordsmithUI.ShowScratchPad(x);
        else
            WordsmithUI.ShowScratchPad(-1);
    }
    #endregion
}