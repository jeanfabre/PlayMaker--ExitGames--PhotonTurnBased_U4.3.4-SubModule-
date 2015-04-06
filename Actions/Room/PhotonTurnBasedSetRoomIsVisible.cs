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
	[Tooltip(" Defines if the current room is listed in its lobby. It will update the server and all clients. Rooms can be created invisible, or changed to invisible.\n" +
		"To change if a room can be joined, use property: isOpen")]
	public class PhotonTurnBasedSetRoomIsVisible : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The current room visibility")]
		public FsmBool isVisible;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		[Tooltip("Update defined values every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			isVisible = null;
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
			_room.IsVisible = isVisible.Value;

		}	
	}
}