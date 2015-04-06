// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System;
using System.Collections.Generic;

using UnityEngine;

using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Photon.TurnBased.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Call the TurnBased Cloud Server to join a random, available room." +
	         "If all rooms are closed or full, the OperationResponse will have a returnCode of ErrorCode.NoRandomMatchFound." +
	         "If successful, the OperationResponse contains a gameserver address and the name of some room. " +
	         "This is an async request which will triggers 'PHOTON TURNBASED / XXX' events ")]
	public class PhotonTurnBasedJoinRandomRoom: FsmStateAction
	{

		[Tooltip("Type of matchMaking, Leave to none for no effect")]
		[ObjectType(typeof(MatchmakingMode))]
		public FsmEnum matchMakingMode;
		
		[Tooltip("Max number of players that can be in the room at any time. 0 means 'no limit'.")]
		public FsmInt maxNumberOfPLayers;
	
		[ActionSection("Expected Properties")]
		
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("The expected Property to set")]
		public FsmString[] expectedPropertyKey;
		[RequiredField]
		[Tooltip("Value of the property")]
		public FsmVar[] expectedPropertyValue;

		
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True If the operation could be sent (has to be connected).")]
		public FsmBool operationSent;
		
		[Tooltip("Event fired If the operation could NOT be sent (has to be connected) or if all rooms are closed or full.")]
		public FsmEvent operationNotSentEvent;

		[Tooltip("Return code of the operation")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(ErrorCode))]
		public FsmEnum operationReturnCode;

		[Tooltip("Event fired when operation is done")]
		public FsmEvent successEvent;

		[Tooltip("Event fired when operation is terminated but failed. Use the operationReturnCode to know what happened")]
		public FsmEvent failureEvent;

		[Tooltip("Event fired if no match was found. This is for convenience, you can also catch the failureEvent and check the ReturnCode to cover all cases failure.")]
		public FsmEvent noMatchFoundEvent;

		public override void Reset()
		{

			matchMakingMode = new FsmEnum() {UseVariable=true};
			maxNumberOfPLayers = null;

			expectedPropertyKey = null;
			expectedPropertyValue = null;

			
			operationSent = null;
			operationNotSentEvent = null;

			operationReturnCode = null;
			successEvent = null;
			failureEvent = null;

			noMatchFoundEvent = null;
		}
		
		public override void OnEnter()
		{
			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OnJoinRandomGameResponseAction += OnJoinRandomGameResponseAction;

			// we keep it null for no rules.
			ExitGames.Client.Photon.Hashtable _expectedProps = null;

			if (expectedPropertyKey.Length>0)
			{
				_expectedProps = new ExitGames.Client.Photon.Hashtable();
				
				int i = 0;
				foreach(FsmString _prop in expectedPropertyKey)
				{
					_expectedProps[_prop.Value] =  PlayMakerUtils.GetValueFromFsmVar(this.Fsm,expectedPropertyValue[i]);
					i++;
				}
			}

			byte _maxNumberOfPLayers = (byte)maxNumberOfPLayers.Value;

			MatchmakingMode _mod = (MatchmakingMode)Enum.ToObject(typeof(MatchmakingMode),matchMakingMode.Value);


			bool _couldBeSent = false;

			if (matchMakingMode.IsNone)
			{
				_couldBeSent = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpJoinRandomRoom(_expectedProps,0);
			}else{
				_couldBeSent = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OpJoinRandomRoom(_expectedProps,_maxNumberOfPLayers,_mod);
			}

			operationSent.Value = _couldBeSent;

			if (_couldBeSent)
			{

				Fsm.Event(operationNotSentEvent);

				Fsm.Event(failureEvent);

				Finish();
			}


		}

		void OnJoinRandomGameResponseAction(short returnCode)
		{
			Debug.Log("OnJoinRandomGameResponseAction "+returnCode);

			ErrorCode _errorCode = (ErrorCode) Enum.ToObject(typeof(ErrorCode), returnCode);

			operationReturnCode.Value = _errorCode;

			if (_errorCode == ErrorCode.Ok)
			{
				Fsm.Event(successEvent);
				Finish ();
				return;
			}else if (_errorCode == ErrorCode.NoRandomMatchFound)
			{
				Fsm.Event(noMatchFoundEvent);
				Finish ();
				return;
			}

			Fsm.Event(failureEvent);

			Finish();
		}


		
	}
}