// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System;
using System.Text;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using HutongGames.PlayMaker.Actions;

using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Photon.TurnBased.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("GUILayout SelectionGrid listing Rooms In Lobby.\n" +
	         "This list is populated while being in the lobby of the Master. It contains RoomInfo per roomName (keys)." +
	         "The selection event int data contains the room index, and the event string data contains the selected game id")]
	public class PhotonTurnBasedGuiLayoutRoomsInLobbyListSelectionGrid : GUILayoutAction
	{

		[Tooltip("How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.")]
		public FsmInt xCount;

		[Tooltip("The string format. keywords {RoomName} and {xxx} will be replaced by the actual value, xxx being the key of any custom properties of that room")]
		public FsmString labelFormat;

		[Tooltip("The selected room index")]
		[UIHint(UIHint.Variable)]
		public FsmInt selectedRoomIndex;

		[Tooltip("The selected room name")]
		[UIHint(UIHint.Variable)]
		public FsmString selectedRoomName;
		
		[Tooltip("Event sent when user select a Game from the toolbar")]
		public FsmEvent selectionEvent;

		public FsmString style;
		
		 
		Dictionary<string, GameDescription> gameList;
		
		public override void Reset()
		{
			base.Reset();

			labelFormat = "{RoomName} turn: {t#}";
			selectedRoomIndex = null;
			selectedRoomName = null;
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
			Dictionary<string,RoomInfo> _list = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.RoomInfoList;
			
			int count = _list.Count;

			if (count==0)
			{
				return;
			}

			string[] _labels = new string[count];
			string[] _keys = new string[count];
			int i = 0;
			foreach(KeyValuePair<string, RoomInfo> _item in _list)
			{
				RoomInfo _info = _item.Value;

				string _label =  labelFormat.Value;

				_label = _label.Replace("{RoomName}",_item.Key);

				foreach(DictionaryEntry _prop in _info.CustomProperties)
				{
					_label = _label.Replace("{"+_prop.Key+"}",_prop.Value.ToString());
				}


				_labels[i] = _label;
				_keys[i] = _item.Key;
				i++;
			}

			int _selection = GUILayout.SelectionGrid(selectedRoomIndex.Value, _labels,xCount.Value, style.Value, LayoutOptions);

			if (GUI.changed)
			{
				selectedRoomIndex.Value = _selection;
				string _name = _keys[_selection];
				selectedRoomName.Value = _name;

				Fsm.EventData.IntData = _selection;
				Fsm.EventData.StringData = _name;
				Fsm.Event(selectionEvent);
				GUIUtility.ExitGUI();
				
				
			}else{
				GUI.changed = guiChanged;
			}
	
		}
		
	}
}