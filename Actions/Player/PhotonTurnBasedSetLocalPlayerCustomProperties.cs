﻿// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Updates and synchronizes the named properties of the local player with the defined values.")]
	public class PhotonTurnBasedSetLocalPlayerCustomProperties : FsmStateAction
	{
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for the properties")]
		public FsmString[] keys;
		
		[Tooltip("The variable to set.")]
		public FsmVar[] variables;

		[Tooltip("Event fired if we local player is invalid (id -1), likely because we are not in room.")]
		public FsmEvent invalidLocalPlayerEvent;

		public override void Reset()
		{
			keys = null;
			variables = null;
			invalidLocalPlayerEvent = null;
		}
		
		
		public override void OnEnter()
		{
			if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.LocalPlayer.ID==-1)
			{
				Fsm.Event(invalidLocalPlayerEvent);
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
			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.LocalPlayer.SetCustomProperties(props);
		}
		
		
	}
}