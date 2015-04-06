// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Leave the room. This is an asynchronous operation.")]
	public class PhotonTurnBasedLeaveRoom: FsmStateAction
	{

		[Tooltip("If true, player will be back to the Lobby.")]
		public FsmBool willReturnLater;
		
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True If the current room could not be left (impossible while not in a room)")]
		public FsmBool notInRoom;
		
		[Tooltip("Event fired If the current room could not be left (impossible while not in a room)")]
		public FsmEvent notInRoomEvent;
		
		public override void Reset()
		{
			willReturnLater = false;
			notInRoom = null;
			notInRoomEvent = null;
		}
		
		public override void OnEnter()
		{

			bool ok = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpLeaveRoom(willReturnLater.Value);
			
			notInRoom.Value = !ok;
			if (!ok)
			{
				Fsm.Event(notInRoomEvent);
			}
			
			Finish();
			
		}
		
	}
}