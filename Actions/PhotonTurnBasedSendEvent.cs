// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon.Lite;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Send an event with custom code/type and any content to the other players in the same room.")]
	public class PhotonTurnBasedSendEvent : FsmStateAction
	{
		public enum TurnBasedEventKeyFormat {String,Int,Byte};

		[Tooltip("Identifies this type of event (and the content). Your game's event codes can start with 0.")]
		public FsmInt eventId;

		[Tooltip("Define if this event has to arrive reliably (potentially repeated if it's lost).")]
		public FsmBool reliable;

		[ActionSection("Data")]
		[CompoundArray("Event Data", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for the properties")]
		public FsmString[] keys;
		
		[Tooltip("The variable to set.")]
		public FsmVar[] variables;

	

		public TurnBasedEventKeyFormat keyFormat;

		[ActionSection("Options, Leave to none for default")]


		[Tooltip("Defines if the server should simply send the event,put it in the cache or remove events that are like this one. Default ( no caching) \nNote: When using option: SliceSetIndex, SlicePurgeIndex or SlicePurgeUpToIndex, set a CacheSliceIndex.All other options except SequenceChannel get ignored.")]
		[ObjectType(typeof(EventCaching))]
		public FsmEnum cachingOption;

		[Tooltip("The number of the Interest Group to send this to. 0 goes to all users but to get 1 and up, clients must subscribe to the group first.")]
		public FsmInt interestGroup;

		[Tooltip("A list of PhotonPlayer.IDs to send this event to. You can implement events that just go to specific users this way.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditorAttribute(VariableType.Int)]
		public FsmArray targetActors;

		[Tooltip("Sends the event to All, MasterClient or Others (default). \n" +
			"Be careful with MasterClient, as the client might disconnect before it got the event and it gets lost.")]
		[ObjectType(typeof(ReceiverGroup))]
		public FsmEnum receivers;

		[Tooltip("Events are ordered per 'channel'.\n" +
		         "If you have events that are independent of others, they can go into another sequence or channel.")]
		public FsmInt sequenceChannel;

		[Tooltip("Events can be forwarded to Webhooks, which can evaluate and use the events to follow the game's state.")]
		public FsmBool forwardToWebhook;


		public override void Reset()
		{
			eventId = null;
			reliable = true;

			keys = new FsmString[]{};
			variables = new FsmVar[]{};

			keyFormat = TurnBasedEventKeyFormat.String;

			cachingOption = null;
			interestGroup = new FsmInt(){UseVariable=true};
			targetActors  = new FsmArray(){UseVariable=true};
			receivers = ReceiverGroup.Others;
			sequenceChannel = new FsmInt(){UseVariable=true};
			forwardToWebhook = new FsmBool(){UseVariable=true};
		}
		
		public override void OnEnter()
		{

			Hashtable props = new Hashtable();
			for(int i = 0;i<keys.Length;i++)
			{
				var _value = PlayMakerUtils.GetValueFromFsmVar(Fsm,variables[i]);;
				if (keyFormat == TurnBasedEventKeyFormat.Int)
				{
					props[int.Parse(keys[i].Value)] = _value;
				}else if (keyFormat == TurnBasedEventKeyFormat.Byte)
				{
					props[byte.Parse(keys[i].Value)] = _value;
				}
			}

			RaiseEventOptions _options = new RaiseEventOptions();
			if (!cachingOption.IsNone)
			{
				_options.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache;
			}

			if (!interestGroup.IsNone)
			{
				_options.InterestGroup = (byte)interestGroup.Value;
			}


			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.loadBalancingPeer.OpRaiseEvent(
				(byte)eventId.Value,
				props,
				reliable.Value,
				_options
				);

			Finish();
		}

	}
}