// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Statistic value available on master server: Players in rooms (playing).")]
	public class PhotonTurnBasedGetPlayersInRoomsCount : FsmStateAction
	{

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt playersInRoomsCount;

		public bool everyFrame;

		public override void Reset()
		{
			playersInRoomsCount = null;
			everyFrame = true;
		}
		
		public override void OnEnter()
		{
			getProperty();

			if (!everyFrame)
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
			playersInRoomsCount.Value =	PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.PlayersInRoomsCount;

		}	
	}
}