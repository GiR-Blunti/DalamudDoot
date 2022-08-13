﻿using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.IO;
using XivCommon;

namespace HypnotoadPlugin;

public class TestPlugin : IDalamudPlugin
{
    public static XivCommonBase CBase;
    public string Name => "Hypnotoad";

    private const string commandName = "/hypnotoad";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private Configuration Configuration { get; init; }
    private PluginUI PluginUi { get; init; }

    public unsafe TestPlugin(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chatGui)
    {
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;

        this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(this.PluginInterface);

        try
        {
            CBase = new XivCommonBase();
        }
        catch (Exception ex)
        {
            PluginLog.LogError($"exception: {ex}");
        }

        // you might normally want to embed resources and load them from the manifest stream
        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "toad.png");
        var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
        this.PluginUi = new PluginUI(this.Configuration, goatImage);

        this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        this.PluginUi.Dispose();
        CBase.Dispose();
        this.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        this.PluginUi.Visible = true;
    }

    private void DrawUI()
    {
        this.PluginUi.Draw();
    }

    private void DrawConfigUI()
    {
        this.PluginUi.SettingsVisible = true;
    }
}
