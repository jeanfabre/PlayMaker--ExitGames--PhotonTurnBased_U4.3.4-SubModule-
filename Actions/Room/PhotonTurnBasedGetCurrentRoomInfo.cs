// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Current state this client is in. Careful: several states are 'transitions' that lead to other states.")]
	public class PhotonTurnBasedGetCurrentRoomInfo : FsmStateAction
	{

		[UIHint(UIHint.Variable)]
		[Tooltip("The name of a room. Unique identifier (per Loadbalancing group) for a room/match (per AppId + game-Version)\n" +
		         "Note: The name can't be changed once it's set. It's only for references")]
		public FsmString name;

		[UIHint(UIHint.Variable)]
		[Tooltip("Defines if the room can be joined. This does not affect listing in a lobby but joining the room will fail if not open.\n" +
			"If not open, the room is excluded from random matchmaking.\n" +
			"Due to racing conditions, found matches might become closed while users are trying to join.")]
		public FsmBool isOpen;

		[UIHint(UIHint.Variable)]
		[Tooltip("Defines if the room is listed in its lobby.\n" +
			"Rooms can be created invisible, or changed to invisible.\n" +
			"To change if a room can be joined, use action: PhotonTurnBasedOpenRoom.")]
		public FsmBool isVisible;

		[UIHint(UIHint.Variable)]
		[Tooltip("State if the room is full (players count == maxplayers), joining this room will fail.")]
		public FsmBool isFull;

		[UIHint(UIHint.Variable)]
		[Tooltip("State if the local client is already in the game or still going to join it on gameserver (in lobby: false).")]
		public FsmBool isLocalClientInside;

		[UIHint(UIHint.Variable)]
		[Tooltip("The limit of players for this room. This property is shown in lobby, too.\n" +
		         "If the room is full (players count == maxplayers), joining this room will fail.")]
		public FsmInt maxPlayers;

		[UIHint(UIHint.Variable)]
		[Tooltip("Count of players currently in room")]
		public FsmInt playerCount;

		[UIHint(UIHint.Variable)]
		[Tooltip("The List of players id currently in the room")]
		[ArrayEditorAttribute(VariableType.Int)]
		public FsmArray players;

		[UIHint(UIHint.Variable)]
		[Tooltip("List of all custom properties keys. Use GetCustomProperties to get values")]
		[ArrayEditorAttribute(VariableType.String)]
		public FsmArray customPropertyKeys;

		[UIHint(UIHint.Variable)]
		[Tooltip("List of custom properties that are accessible in the lobby")]
		[ArrayEditorAttribute(VariableType.String)]
		public FsmArray propsListedInLobby;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		[Tooltip("Update defined values every frame")]
		public bool everyFrame;
		
		public override void Reset()
		{
			name = null;
			isOpen = null;
			isVisible = null;
			isLocalClientInside = null;

			players = null;
			maxPlayers = null;
			isFull = null;

			playerCount = null;

			customPropertyKeys = null;
			propsListedInLobby = null;

			notInRoomEvent = null;
			everyFrame=false;
			
		}
		
		public override void OnEnter()
		{
			getProperties();
			
			if(!everyFrame)
			{
				Finish();
			}
		}
		public override void OnUpdate()
		{
			getProperties();
			
		}
		
		void getProperties()
		{
			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;

			if (_room==null)
			{
				Fsm.Event(notInRoomEvent);
				return;
			}

			if (!name.IsNone)
			{
				name.Value = _room.Name;
			}

			if (!isOpen.IsNone)
			{
				isOpen.Value = _room.IsOpen;
			}

			if (!isVisible.IsNone)
			{
				isVisible.Value = _room.IsVisible;
			}

			if (!isLocalClientInside.IsNone)
			{
				isLocalClientInside.Value = _room.IsLocalClientInside;
			}

			if (!name.IsNone)
			{
				name.Value = _room.Name;
			}

			if (!isFull.IsNone)
			{
				isFull.Value = Convert.ToInt32(_room.MaxPlayers) == _room.PlayerCount;
			}

			if (!maxPlayers.IsNone)
			{
				maxPlayers.Value = Convert.ToInt32(_room.MaxPlayers);
			}

			if (!playerCount.IsNone)
			{

				//playerCount.Value = _room.PlayerCount;
			}

			if (!players.IsNone)
			{
				playerCount.Value = _room.Players.Count;
				players.intValues = _room.Players.Keys.ToArray();
				players.SaveChanges();
			}

			if (!propsListedInLobby.IsNone)
			{
				propsListedInLobby.Values = _room.PropsListedInLobby;
				propsListedInLobby.SaveChanges();
			}

			if (!customPropertyKeys.IsNone)
			{
				customPropertyKeys.Values = _room.CustomProperties.Keys.ToArray();
				customPropertyKeys.SaveChanges();
			}


		}
	}
}