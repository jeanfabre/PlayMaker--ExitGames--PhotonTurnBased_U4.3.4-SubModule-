// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Photon.TurnBased.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Call the TurnBased Cloud Server to join a room by name, and sets this player's properties " +
	         "Join will try to enter a room by roomName. If the room is full or closed, this will fail. " +
		"This is an async request which will triggers 'PHOTON TURNBASED / XXX' events ")]
	public class PhotonTurnBasedJoinRoom: FsmStateAction
	{
		
		[Tooltip("The name of the room to join. Must be existing already, open and non-full or can't be joined.")]
		public FsmString roomName;
		
		[Tooltip("An actorNumber to claim in room in case the client re-joins a room. Use 0 to not claim an actorNumber. Leave to none for no effect")]
		public FsmInt actorNumber;
		
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True If the operation could be sent (has to be connected).")]
		public FsmBool operationSent;
		
		[Tooltip("Event fired If the operation could NOT be sent (has to be connected) or failed")]
		public FsmEvent operationFailedEvent;

		[Tooltip("Return code of the operation")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(ErrorCode))]
		public FsmEnum operationReturnCode;

		[Tooltip("Event fired If the operation response was ok")]
		public FsmEvent operationsuccededEvent;
		
		public override void Reset()
		{
			roomName = null;
			actorNumber = new FsmInt() {UseVariable=true};
			
			operationSent = null;
			operationFailedEvent = null;
			operationsuccededEvent = null;
		}
		
		public override void OnEnter()
		{
			
			string _roomName = null;
			if ( ! string.IsNullOrEmpty(roomName.Value) )
			{
				_roomName = roomName.Value;
			}

			bool _couldBeSent = false;

			if (actorNumber.IsNone)
			{
				_couldBeSent = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpJoinRoom(_roomName);
			}else{
				_couldBeSent = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpJoinRoom(_roomName, actorNumber.Value);
			} 

			operationSent.Value = _couldBeSent;
			if (_couldBeSent)
			{
				Fsm.Event(operationFailedEvent);
			}
			
			Finish();
			
		}
		
	}
}