// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using HutongGames.PlayMaker.Photon.TurnBased;
using HutongGames.PlayMaker.Ecosystem.Utils;

using Rotorz.ReorderableList;
using HutongGames.PlayMaker.Photon;

namespace HutongGames.PlayMaker.Photon.TurnBased
{
	[CustomEditor(typeof(PlayMakerPhotonLoadBalancingEventTable))]
	public class PlayMakerPhotonLoadBalancingEventTableInspector : UnityEditor.Editor {

		PlayMakerPhotonLoadBalancingEventTable _target;


		SerializedObject _object;

		string[] _availableEventList;

		PlayMakerByteEventReference itemTarget;

		public override void OnInspectorGUI()
		{
			_availableEventList = PlayMakerInspectorUtils.GetGlobalEvents(true);

			_target = (PlayMakerPhotonLoadBalancingEventTable)target;

			SerializedObject _object = new SerializedObject(_target);

			EditorGUILayout.PropertyField(_object.FindProperty("EventTarget"));


			if (_target.Events==null)
			{
				_target.Events = new List<PlayMakerByteEventReference>();
			}
			ReorderableListGUI.Title("Events:");
			ReorderableListGUI.ListField<PlayMakerByteEventReference>(_target.Events,DrawListItem);

		}

		private PlayMakerByteEventReference DrawListItem(Rect position, PlayMakerByteEventReference value) {

			if (value==null)
			{
				value =  new PlayMakerByteEventReference();
			}

			float width = position.width;

			Rect _keyRect = position;
			_keyRect.width = 30;

			value.Key = (byte)EditorGUI.IntField(_keyRect,(int)value.Key);

			position.x +=35;
			position.width -= Application.isPlaying?60:35;
			if (GUI.Button(
				position,
				string.IsNullOrEmpty(value.EventName)?"none":value.EventName, 
				EditorStyles.popup))
			{
				itemTarget = value;
				GenericMenu menu = GenerateEventMenu(_availableEventList,value.EventName);
				menu.DropDown(position);
				
			}

			if (Application.isPlaying)
			{
				position.x = _keyRect.x + width -15;
				position.width = 30;

				GUI.Label(position,value.Count.ToString());
			}
			return value;
		}


		void EventMenuSelectionCallBack(object userdata)
		{
			
			if (userdata==null) // none
			{
				//nothing:
			}else{
				itemTarget.EventName = (string)userdata;
			}

			itemTarget = null;

		}
		
		GenericMenu GenerateEventMenu(string[] _eventList,string currentSelection)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("none"), currentSelection.Equals("none"), EventMenuSelectionCallBack, null);
			
			foreach(string _event in _eventList)
			{
				menu.AddItem(new GUIContent(_event), currentSelection.Equals(_event), EventMenuSelectionCallBack,_event);
			}
			
			return menu;
		}

	}
}
