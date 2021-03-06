﻿// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System.Collections.Generic;

using UnityEngine; 

using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker.Photon.TurnBased;
using ExitGames.Client.Photon.LoadBalancing;

namespace HutongGames.PlayMaker.Photon.TurnBased.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("GUILayout listing of Players.")]
	public class PhotonTurnBasedGuiLayoutPlayerList : GUILayoutAction
	{


		[Tooltip("The selected game ActorNr")]
		[UIHint(UIHint.Variable)]
		public FsmInt lastPlayerId;


		public override void Reset()
		{
			base.Reset();

			lastPlayerId = null;

		}
		
		public override void OnGUI()
		{

			if (PlayMakerPhotonLoadBalancingClientProxy.instance==null)
			{
				return;
			}

			if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance == null)
			{
				return;
			}
			
			foreach (Player player in PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.Players.Values)
			{
				string _playerDescription = player.ToString();
				if (player.IsInactive)
				{
					_playerDescription += " (inactive)";
				}
				if (player.ID == lastPlayerId.Value)
				{
					_playerDescription += " (played last)";
				}

				GUILayout.Label(_playerDescription,this.LayoutOptions);
			}
		}
		
	}
}