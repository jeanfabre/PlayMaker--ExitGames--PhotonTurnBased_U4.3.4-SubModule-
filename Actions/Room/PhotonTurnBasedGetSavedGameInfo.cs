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
	[Tooltip("Get info on a saveGames using the room name.")]
	public class PhotonTurnBasedGetSavedGameInfo : FsmStateAction
	{
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The name of a room.")]
		public FsmString roomName;

		[Tooltip("The ActorNumber for that room. Use this to rejoin the room")]
		public FsmInt  actorNr;

		[Tooltip("Event fired if this room was not found in the savedGamed.")]
		public FsmEvent notInSavedGameEvent;
		
	
		public override void Reset()
		{
			roomName = null;

			notInSavedGameEvent = null;
		
		}
		
		public override void OnEnter()
		{
			getProperties();

			Finish();
		}
		
		void getProperties()
		{

			if (! PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.SavedGames.ContainsKey(roomName.Value))
			{
				// we want to set it properly even if it's not in room, typically, you call this action before joinOrCreate a room and so the actorNumber is properly set in one step.
				if (!actorNr.IsNone)
				{
					actorNr.Value = 0;
				}

				Fsm.Event(notInSavedGameEvent);
				return;
			}

			GameDescription _game = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.SavedGames[roomName.Value];


			if (!actorNr.IsNone)
			{
				actorNr.Value = _game.ActorNr;
			}

		}
	}
}