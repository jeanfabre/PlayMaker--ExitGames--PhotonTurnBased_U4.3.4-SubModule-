// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Current Status Code this Peer. Careful: several states are 'transitions' that lead to other states.")]
	public class PhotonTurnBasedGetStatusCode : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(StatusCode))]
		public FsmEnum statusCode;
		
		public bool everyFrame;
		
		public override void Reset()
		{
			statusCode = StatusCode.Exception;
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
			statusCode.Value = PlayMakerPhotonLoadBalancingClientProxy.instance.statusCode;
		}
	}
}