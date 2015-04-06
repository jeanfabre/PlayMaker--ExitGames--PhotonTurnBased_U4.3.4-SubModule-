// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Call the TurnBased Cloud Server webhook 'GetGameList' to return the current list of saved games. This is an asynchronous operation.")]
	public class PhotonTurnBasedCallGetGameList : FsmStateAction
	{
		[Tooltip("The list of game id")]
		[UIHint(UIHint.Variable)]
		[ArrayEditorAttribute(VariableType.String)]
		public FsmArray gameIdList;

		[UIHint(UIHint.Variable)]
		[ArrayEditorAttribute(VariableType.Int)]
		public FsmArray ActorNrList;

		[Tooltip("The number of games returned")]
		[UIHint(UIHint.Variable)]
		public FsmInt gameCount;

		[Tooltip("Event sent when gameList is received but list is empty. This event will be sent first, and then gameListReceivedEvent will be sent")]
		public FsmEvent gameListEmptyEvent;

		[Tooltip("Event sent when gameList is received")]
		public FsmEvent gameListReceivedEvent;

		public override void Reset()
		{
			gameIdList = null;
			ActorNrList = null;
			gameCount = null;
			gameListReceivedEvent = null;
		}

		public override void OnEnter()
		{

			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpWebRpc("GetGameList", new Dictionary<string, object>());

			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OnGameListReceivedAction += OnGameListReceived;
		}

		public override void OnExit()
		{
			if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance!=null)
			{
				PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OnGameListReceivedAction -= OnGameListReceived;
			}

		}

		void OnGameListReceived ()
		{
			Debug.Log("OnGameListReceived");

			Dictionary<string, GameDescription> _list = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.SavedGames;

			int count = _list.Count;

			string[] _keys = new string[count];
			object[] ActorNrs = new object[count];

			int i = 0;
			foreach(KeyValuePair<string, GameDescription> _item in _list)
			{
				_keys[i] = _item.Key;
				ActorNrs[i] = (object)_item.Value.ActorNr;
				i++;
			}

			if (!gameIdList.IsNone)
			{
				gameIdList.Values = _keys;
			}

			if (!ActorNrList.IsNone)
			{
				ActorNrList.Values = ActorNrs;
			}

			gameCount.Value = count;

			if (count==0)
			{
				Fsm.Event(gameListEmptyEvent);
			}

			Fsm.Event(gameListReceivedEvent);

			Finish();
		}
		
	}
}