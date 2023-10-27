﻿using System;
using Sandbox;
using Sandbox.UI;

[Library]
public partial class SandboxHud : HudEntity<RootPanel>
{
	public static SandboxHud Instance;
	public SandboxHud()
	{
		Instance = this;
		if ( !Game.IsClient )
			return;

		PopulateHud();
	}

	private void PopulateHud()
	{

		RootPanel.StyleSheet.Load( "/Styles/sandbox.scss" );

		RootPanel.AddChild<Chat>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<VoiceSpeaker>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
		RootPanel.AddChild<SpawnMenu>();
		RootPanel.AddChild<Crosshair>();
		RootPanel.AddChild<HintFeed>();
		Event.Run( "sandbox.hud.loaded" );
	}

	[ClientRpc]
	public static void HotReloadTool()
	{
		CurrentTool.GetCurrentTool()?.Activate();
	}

	[Event( "package.mounted" )]
	[Event.Hotload]
	private void OnReloaded()
	{
		Log.Info( "SandboxHud.OnReloaded" );
		if ( !Game.IsClient )
			return;

		RootPanel.DeleteChildren();
		PopulateHud();
		HotReloadTool();
	}

	[ConCmd.Client( "reload_hud" )]
	public static void ReloadHud()
	{
		Instance?.OnReloaded();
	}
}
