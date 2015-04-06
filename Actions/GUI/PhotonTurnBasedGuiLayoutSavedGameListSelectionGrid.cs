// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System.Collections.Generic;

using UnityEngine; 

using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Photon.TurnBased.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("GUILayout SelectionGrid listing Saved Games.\n" +
	         "The selection event int data contains the game index, and the event string data contains the selected game id")]
	public class PhotonTurnBasedGuiLayoutSavedGameListSelectionGrid : GUILayoutAction
	{
		
		//[Tooltip("If True, append to the room name the number of users against the maximum ( '--- 1/3' )")]
		//public FsmBool displayRoomDetails;

		[Tooltip("How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.")]
		public FsmInt xCount;

		[Tooltip("The string format. keywords {GameId} and {ActorNr} will be replaced by the actual value")]
		public FsmString labelFormat;

		[Tooltip("The selected game index")]
		[UIHint(UIHint.Variable)]
		public FsmInt selectedGameIndex;

		[Tooltip("The selected game ActorNr")]
		[UIHint(UIHint.Variable)]
		public FsmInt selectedGameActorNr;

		[Tooltip("The selected game id")]
		[UIHint(UIHint.Variable)]
		public FsmString selectedGameId;
		
		[Tooltip("Event sent when user select a Game from the toolbar")]
		public FsmEvent selectionEvent;

		public FsmString style;
		
		 
		Dictionary<string, GameDescription> gameList;
		
		public override void Reset()
		{
			base.Reset();

			labelFormat = "Rejoin {GameId} #{ActorNr}";
			selectedGameIndex = null;
			selectedGameId = null;
			xCount = 1;
			selectionEvent = null;
			
			style = "Button";
		}
		
		public override void OnGUI()
		{
			var guiChanged = GUI.changed;
			GUI.changed = false;

			if (PlayMakerPhotonLoadBalancingClientProxy.instance==null)
			{
				return;
			}
			Dictionary<string, GameDescription> _list = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.SavedGames;
			
			int count = _list.Count;

			if (count==0)
			{
				return;
			}

			string[] _labels = new string[count];
			string[] _keys = new string[count];
			int i = 0;
			foreach(KeyValuePair<string, GameDescription> _item in _list)
			{
				string _label =  labelFormat.Value;
				_label = _label.Replace("{GameId}",_item.Key);
				_label = _label.Replace("{ActorNr}",_item.Value.ActorNr.ToString());
				_labels[i] = _label;
				_keys[i] = _item.Key;
				i++;
			}

			int _selection = GUILayout.SelectionGrid(selectedGameIndex.Value, _labels,xCount.Value, style.Value, LayoutOptions);

			if (GUI.changed)
			{
				selectedGameIndex.Value = _selection;
				GameDescription _selectedGame = _list[_keys[_selection]];
				string _selectedGameId = _selectedGame.GameId;
				selectedGameId.Value = _selectedGameId;
				selectedGameActorNr.Value = _selectedGame.ActorNr;

				Fsm.EventData.IntData = _selection;
				Fsm.EventData.StringData = _selectedGameId;
				Fsm.Event(selectionEvent);
				GUIUtility.ExitGUI();
				
				
			}else{
				GUI.changed = guiChanged;
			}
		}
		
	}
}