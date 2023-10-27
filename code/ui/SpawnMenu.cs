﻿using Sandbox;
using Sandbox.Tools;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

[Library]
public partial class SpawnMenu : Panel
{
	public static SpawnMenu Instance;
	readonly Panel toollist;
	public Panel ToolPanel { get; private set; }
	public ButtonGroup SpawnMenuLeftTabs;
	public Panel SpawnMenuLeftBody;

	public bool IgnoreMenuButton = false;
	private bool IsOpen = false;

	private static ModelList modelList;
	private bool isSearching;

	public SpawnMenu()
	{
		Instance = this;

		var left = Add.Panel( "left" );
		{
			var tabs = left.AddChild<ButtonGroup>();
			tabs.AddClass( "tabs" );
			SpawnMenuLeftTabs = tabs;

			var body = left.Add.Panel( "body" );
			SpawnMenuLeftBody = body;
			{
				var props = body.AddChild<SpawnList>();
				tabs.SelectedButton = tabs.AddButtonActive( "#spawnmenu.props", ( b ) => props.SetClass( "active", b ) );

				modelList = body.AddChild<ModelList>();
				tabs.AddButtonActive( "#spawnmenu.modellist", ( b ) => modelList.SetClass( "active", b ) );

				var ents = body.AddChild<EntityList>();
				tabs.AddButtonActive( "#spawnmenu.entities", ( b ) => ents.SetClass( "active", b ) );

				var npclist = body.AddChild<NpcList>();
				tabs.AddButtonActive( "#spawnmenu.npclist", ( b ) => npclist.SetClass( "active", b ) );
			}
		}

		var right = Add.Panel( "right" );
		{
			var tabs = right.Add.Panel( "tabs" );
			{
				tabs.Add.Button( "#spawnmenu.tools" ).AddClass( "active" );
			}
			var body = right.Add.Panel( "body" );
			{
				toollist = body.Add.Panel( "toollist" );
				{
					RebuildToolList();
				}
				ToolPanel = body.Add.Panel( "inspector" );
			}
		}

	}

	void RebuildToolList()
	{
		toollist.DeleteChildren( true );

		foreach ( var entry in TypeLibrary.GetTypes<BaseTool>().OrderBy( ( x ) => x.Title ) )
		{
			if ( entry.Name.StartsWith( "Base" ) )
				continue;

			var button = toollist.Add.Button( entry.Title );
			button.SetClass( "active", entry.ClassName == ConsoleSystem.GetValue( "tool_current" ) );

			button.AddEventListener( "onclick", () =>
			{
				Tool.SetActiveTool( entry.ClassName );

				foreach ( var child in toollist.Children )
					child.SetClass( "active", child == button );
				ToolPanel.DeleteChildren( true );
			} );
		}
	}

	private bool menuWasPressed = false;

	public override void Tick()
	{
		base.Tick();
		if ( !IgnoreMenuButton )
		{
			if ( Input.Pressed( "menu" ) )
			{
				IsOpen = true;
			}
			if ( menuWasPressed && !Input.Down( "menu" ) )
			{
				IsOpen = false;
			}
		}
		menuWasPressed = Input.Down( "menu" );

		if ( (bool)(modelList?.SearchInput?.HasFocus) )
		{
			isSearching = true;
		}
		else if ( isSearching && Input.Pressed( "menu" ) )
		{
			isSearching = false;
		}

		UpdateActiveTool();

		if ( isSearching )
			return;

		Parent.SetClass( "spawnmenuopen", IsOpen );

	}

	void UpdateActiveTool()
	{
		var toolCurrent = ConsoleSystem.GetValue( "tool_current" );
		var tool = string.IsNullOrWhiteSpace( toolCurrent ) ? null : TypeLibrary.GetType<BaseTool>( toolCurrent );

		foreach ( var child in toollist.Children )
		{
			if ( child is Button button )
			{
				child.SetClass( "active", tool != null && button.Text == tool.Title );
			}
		}
		Parent.SetClass( "spawnmenuopen", Input.Down( "menu" ) );
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		RebuildToolList();
	}
}
