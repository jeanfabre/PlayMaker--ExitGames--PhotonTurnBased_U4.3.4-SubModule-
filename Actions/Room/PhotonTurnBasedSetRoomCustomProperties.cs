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
	[Tooltip("Updates and synchronizes the named properties of this Room with the defined values.")]
	public class PhotonTurnBasedSetRoomCustomProperties : FsmStateAction
	{
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for the properties")]
		public FsmString[] keys;
		
		[Tooltip("The variable to set.")]
		public FsmVar[] variables;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		public override void Reset()
		{
			keys = null;
			variables = null;
			notInRoomEvent = null;
		}
		
		
		public override void OnEnter()
		{
			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;
			
			if (_room==null)
			{
				Fsm.Event(notInRoomEvent);
			}else{
				SetCustomProperties();
			}


			Finish();
		}
		
		public void SetCustomProperties()
		{
			Hashtable props = new Hashtable();
			for(int i = 0;i<keys.Length;i++)
			{
				props[keys[i].Value] = PlayMakerUtils.GetValueFromFsmVar(Fsm,variables[i]);
			}

			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.SetCustomProperties(props);
		}
		
		
	}
}