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
	[Tooltip("Gets a Player Inactive state.")]
	public class PhotonTurnBasedGetPlayerIsInactive : FsmStateAction
	{

		[Tooltip("The actorNumber of the Player. Leave to none to target local player")]
		public FsmInt actorNumber;

		[Tooltip("The inactive state of the player")]
		public FsmBool isInactive;
		
		[Tooltip("Event fired if player target is active.")]
		public FsmEvent isActiveEvent;

		[Tooltip("Event fired if player target is inactive.")]
		public FsmEvent isInactiveEvent;

		[Tooltip("Event fired if we are not in a room. Accessing Player properties is only available when in a room")]
		public FsmEvent notInRoomEvent;

		[Tooltip("Every frame, useful to watch for changes")]
		public bool everyFrame;

		Player _target;

		public override void Reset()
		{
			actorNumber = new FsmInt() {UseVariable=true};
			isInactive = null;
			isActiveEvent = null;
			isInactiveEvent = null;
			notInRoomEvent = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			GetProperty();
			
			if (!everyFrame)
			{
				Finish();
			}
		}	
		
		public override void OnUpdate()
		{
			GetProperty();
		}	
		
		void GetProperty()
		{
			if (actorNumber.IsNone)
			{
				_target = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.LocalPlayer;
			}else{
				_target = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.GetPlayer(actorNumber.Value);
			}

			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;
			
			if (_room==null)
			{
				if (notInRoomEvent!=null)
				{
					Fsm.Event(notInRoomEvent);
				}
				return;
			}

			bool _state = _target.IsInactive;

			isInactive.Value = _state;

			if (_state)
			{
				if (isInactiveEvent!=null)
				{
					Fsm.Event(isInactiveEvent);
				}
			}else{
				if (isActiveEvent!=null)
				{
					Fsm.Event(isActiveEvent);
				}
			}
			
		}	
	}
}