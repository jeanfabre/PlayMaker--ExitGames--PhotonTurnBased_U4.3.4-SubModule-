// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Statistic value available on master server: Rooms count (Currently created).")]
	public class PhotonTurnBasedGetRoomsCount : FsmStateAction
	{
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt roomsCount;
		
		public bool everyFrame;
		
		public override void Reset()
		{
			roomsCount = null;
			everyFrame = false;
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
			// TOWATCH: the latency of this variable is way more than the roomsList itself, maybe I should only get the count of the roomlist
			//roomsCount.Value =	PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.RoomsCount;
			roomsCount.Value =	PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.RoomInfoList.Count;
			
		}	
	}
}