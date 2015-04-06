// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Connect to the photon turnBased by region, port, appID and game(client) version")]
	public class PhotonTurnBasedConnectToRegionMaster : FsmStateAction
	{
		[Tooltip("Your application ID (Photon Cloud provides you with a GUID for your game).")]
		public FsmString applicationID;
		
		[Tooltip("This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).")]
		public FsmString clientGameVersion;

		[Tooltip("The master server's region.")]
		public FsmString regionCode;
		
		[Tooltip("The master server's port to connect to.")]
		public FsmString playerName;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmBool connectionPending;

		public FsmEvent connectionPendingEvent;
		public FsmEvent errorEvent;

		public override void Reset()
		{
			regionCode			= "EU";
			playerName			= "Player x";
			applicationID		= "YOUR APP ID";
			clientGameVersion	= "1.0";
		}
		
		public override void OnEnter()
		{

			PlayMakerPhotonLoadBalancingClient _lbc = PlayMakerPhotonLoadBalancingClientProxy.instance.StartLoadBalancingClient();

			_lbc.AppId		= applicationID.Value;
			_lbc.AppVersion	= clientGameVersion.Value;
			_lbc.PlayerName	= playerName.Value;

			bool _connectionPending = _lbc.ConnectToRegionMaster(regionCode.Value);

			connectionPending.Value = _connectionPending;

			if (_connectionPending)
			{
				Fsm.Event(connectionPendingEvent);
			}else{
				Fsm.Event(errorEvent);
			}


			Finish();
		}
		
	}
}