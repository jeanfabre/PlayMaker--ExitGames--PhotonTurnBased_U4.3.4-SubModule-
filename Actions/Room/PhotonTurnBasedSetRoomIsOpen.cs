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
	[Tooltip("Defines if the current room can be joined. It will update the server and all clients\n" +
		"This does not affect listing in a lobby but joining the room will fail if not open." +
		"If not open, the room is excluded from random matchmaking. " +
		"Due to racing conditions, found matches might become closed while users are trying to join." +
		"Simply re-connect to master and find another.\n" +
		"Use property 'IsVisible' to not list the room.")]
	public class PhotonTurnBasedSetRoomIsOpen : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The current room open state")]
		public FsmBool isOpen;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		[Tooltip("Update defined values every frame")]
		public bool everyFrame;


		public override void Reset()
		{
			isOpen = null;
			notInRoomEvent = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			SetProperty();
			
			if (!everyFrame)
			{
				Finish();
			}
		}	
		
		public override void OnUpdate()
		{
			SetProperty();
		}	
		
		void SetProperty()
		{
			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;

			if (_room==null)
			{
				Fsm.Event(notInRoomEvent);
				return;
			}
			_room.IsOpen = isOpen.Value;

		}	
	}
}