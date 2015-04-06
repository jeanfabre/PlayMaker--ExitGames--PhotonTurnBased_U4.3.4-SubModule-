// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Call the TurnBased Cloud Server to join a room by name or creates new room if room with given name not exists." +
	         "Join will try to enter a room by roomName. Unlike OpJoinRoom, this will create the room if it doesn't exist." +
		"This is an async request which will triggers 'PHOTON TURNBASED / XXX' events ")]
	public class PhotonTurnBasedJoinRoomOrCreate: FsmStateAction
	{
		
		[Tooltip("The name of the room to join. Must be existing already, open and non-full or can't be joined.")]
		public FsmString roomName;
		
		[Tooltip("An actorNumber to claim in room in case the client re-joins a room. Use 0 to not claim an actorNumber.")]
		public FsmInt actorNumber;

		[ActionSection("Room Properties if created")]
		[Tooltip("Defines if this room is listed in the lobby. If not, it also is not joined randomly.")]
		public FsmBool isVisible;
		
		[Tooltip("Defines if this room can be joined at all.")]
		public FsmBool isOpen;
		
		[Tooltip("Max number of players that can be in the room at any time. 0 means 'no limit'.")]
		public FsmInt maxNumberOfPLayers;
		
		[Tooltip("Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds. Set to none for for no timeout")]
		public FsmInt playerTtl;
		
		[Tooltip("Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds. Set to none for max value")]
		public FsmInt emptyRoomTtl;
		
		[Tooltip("Activates UserId checks on joining - allowing a users to be only once in the room.")]
		public FsmBool checkUserOnJoin;
		
		[Tooltip("Removes a user's events and properties from the room when a user leaves.")]
		public FsmBool cleanupCacheOnLeave;
		
		[ActionSection("Custom Properties")]
		
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("The Custom Property to set")]
		public FsmString[] customPropertyKey;
		[RequiredField]
		[Tooltip("Value of the property")]
		public FsmVar[] customPropertyValue;
		
		[ActionSection("Lobby custom Properties")]
		[Tooltip("Properties listed in the lobby.")]
		public FsmString[] lobbyCustomProperties;
		
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True If the operation could be sent (has to be connected).")]
		public FsmBool operationSent;
		
		[Tooltip("Event fired If the operation could NOT be sent (has to be connected).")]
		public FsmEvent operationFailedEvent;

		
		public override void Reset()
		{
			roomName = new FsmString() {UseVariable=true};
			actorNumber = new FsmInt() {UseVariable=true};

			maxNumberOfPLayers = null;
			isOpen = true;
			isVisible = true;
			
			playerTtl = new FsmInt() {UseVariable=true};
			emptyRoomTtl = 0;
			
			checkUserOnJoin = false;
			cleanupCacheOnLeave = true;
			
			customPropertyKey = null;
			customPropertyValue = null;
			lobbyCustomProperties = null;
			
			operationSent = null;
			operationFailedEvent = null;
			
		}
		
		public override void OnEnter()
		{
			
			string _roomName = null;
			if ( ! string.IsNullOrEmpty(roomName.Value) )
			{
				_roomName = roomName.Value;
			}
			
			int pttl = playerTtl.IsNone ? int.MaxValue:playerTtl.Value;
			
			ExitGames.Client.Photon.Hashtable _props = new ExitGames.Client.Photon.Hashtable();
			
			int i = 0;
			foreach(FsmString _prop in customPropertyKey)
			{
				_props[_prop.Value] =  PlayMakerUtils.GetValueFromFsmVar(this.Fsm,customPropertyValue[i]);
				i++;
			}
			
			
			string[] lobbyProps = new string[lobbyCustomProperties.Length];
			
			int j = 0;
			foreach(FsmString _visibleProp in lobbyCustomProperties)
			{
				lobbyProps[j] = _visibleProp.Value;
				j++;
			}
			
			
			RoomOptions roomOptions = new RoomOptions()
			{
				IsVisible = isVisible.Value,
				IsOpen = isOpen.Value,
				MaxPlayers = (byte)maxNumberOfPLayers.Value,
				CustomRoomProperties = _props,
				CustomRoomPropertiesForLobby = lobbyProps,
				EmptyRoomTtl = emptyRoomTtl.IsNone ? int.MaxValue/2:emptyRoomTtl.Value,
				PlayerTtl = playerTtl.IsNone ? int.MaxValue:playerTtl.Value,
				CheckUserOnJoin = checkUserOnJoin.Value,
				CleanupCacheOnLeave = cleanupCacheOnLeave.Value
			};
			
			
			bool _couldBeSent = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpJoinOrCreateRoom(_roomName, actorNumber.Value,roomOptions);
			operationSent.Value = _couldBeSent;
			if (_couldBeSent)
			{
				Fsm.Event(operationFailedEvent);
			}
			
			Finish();
			
		}
		
	}
}