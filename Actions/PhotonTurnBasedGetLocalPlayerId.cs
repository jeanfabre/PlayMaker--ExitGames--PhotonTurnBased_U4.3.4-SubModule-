// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Get the local player Id (actorId).")]
	public class PhotonTurnBasedGetLocalPlayerId : FsmStateAction
	{
		[Tooltip("The Id")]
		public FsmInt id;

		[Tooltip("The Id as string")]
		public FsmString idAsString;

		[Tooltip("Event fired if we local player is invalid (id -1), likely because we are not in room.")]
		public FsmEvent invalidLocalPlayerEvent;
		
		public override void Reset()
		{
			id = new FsmInt(){UseVariable=true};
			idAsString =  new FsmString(){UseVariable=true};
			invalidLocalPlayerEvent = null;
		}
		
		
		public override void OnEnter()
		{
			if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.LocalPlayer.ID==-1)
			{
				Fsm.Event(invalidLocalPlayerEvent);
			}else{
				GetProperty();
			}
			
			Finish();
		}
		
		public void GetProperty()
		{

			int _id = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.LocalPlayer.ID;
			if (!id.IsNone)
			{
				id.Value = _id;
			}
			if (!idAsString.IsNone)
			{
				idAsString.Value = _id.ToString();
			}
		}
		
		
	}
}