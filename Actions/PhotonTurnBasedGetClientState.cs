// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Current state this client is in. Careful: several states are 'transitions' that lead to other states.")]
	public class PhotonTurnBasedGetClientState : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(ClientState))]
		public FsmEnum clientState;

		public bool everyFrame;

		public override void Reset()
		{
			clientState = ClientState.Uninitialized;
			everyFrame=false;

		}
		
		public override void OnEnter()
		{
			getProperty();

			if(!everyFrame)
			{
				Finish();
			}
		}
		public override void OnUpdate()
		{
			getProperty();
	
		}

		void getProperty()
		{
			clientState.Value = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.State;
		}
	}
}