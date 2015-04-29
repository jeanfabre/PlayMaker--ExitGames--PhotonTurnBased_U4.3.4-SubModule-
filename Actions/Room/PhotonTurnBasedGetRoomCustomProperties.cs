// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Get custom properties of this Room with the defined values.")]
	public class PhotonTurnBasedGetRoomCustomProperties : FsmStateAction
	{
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for the properties")]
		public FsmString[] keys;
		
		[Tooltip("The variable to get.")]
		[UIHint(UIHint.FsmString)]
		public FsmVar[] variables;

		[Tooltip("Event fired if a key is not found.")]
		public FsmEvent KeyNotFoundEvent;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		public override void Reset()
		{
			keys = null;
			variables = null;
			KeyNotFoundEvent = null;
			notInRoomEvent = null;
		}
		
		
		public override void OnEnter()
		{
			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;
			
			if (_room==null)
			{
				Fsm.Event(notInRoomEvent);
			}else{
				GetCustomProperties();
			}

			Finish();
		}
		
		public void GetCustomProperties()
		{
			ExitGames.Client.Photon.Hashtable _customProperties = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.CustomProperties;


			string _key_i;
			int _keyInt_i;
			byte _keyByte_i;

			for(int i = 0;i<keys.Length;i++)
			{
				_key_i = keys[i].Value;

				if (_customProperties.ContainsKey(_key_i))
				{
					PlayMakerUtils.ApplyValueToFsmVar(Fsm,variables[i],_customProperties[_key_i]);
				}
				else if ( int.TryParse(_key_i,out _keyInt_i))
				{
					PlayMakerUtils.ApplyValueToFsmVar(Fsm,variables[i],_customProperties[_keyInt_i]);
				}
				else if ( byte.TryParse(_key_i,out _keyByte_i))
				{
					PlayMakerUtils.ApplyValueToFsmVar(Fsm,variables[i],_customProperties[_keyByte_i]);
				}else{
					if (KeyNotFoundEvent!=null)
					{
				 		Fsm.Event(KeyNotFoundEvent);
					}
				}
			}
		}
		
		
	}
}