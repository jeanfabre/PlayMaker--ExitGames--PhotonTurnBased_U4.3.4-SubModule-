// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets properties on the last Photon turn Based event that caused a state change. \n" +
		"Keys are using enum to remove guess work and potential issues with string based keys.\n" +
		"It however will check the enum string name, int value and byte value in order, so that all common cases are taken in account.\n" +
		"Use Set Event Enum Properties to define these values when sending events")]
	public class PhotonTurnBasedGetEventProperties : FsmStateAction
	{

		public static ExitGames.Client.Photon.Hashtable Properties;

		[CompoundArray("Event Properties", "Key", "Data")]
		[UIHint(UIHint.Variable)]
		public FsmEnum[] keys;
		[UIHint(UIHint.Variable)]
		public FsmVar[] datas;

		[Tooltip("EVent Fire if a key is not found in the event properties")]
		public FsmEvent notFoundEvent;


		public override void Reset()
		{
			keys = new FsmEnum[1];
			datas = new FsmVar[1];
			notFoundEvent = null;
		}

		public override void OnEnter()
		{
		
			try{
				if (Properties == null)
				{
					throw new System.ArgumentException("No Parameters");
				}

				Debug.Log("Properties :"+SupportClass.DictionaryToString(Properties));

				for (int i = 0; i < keys.Length; i++) 
				{	
					Debug.Log("getting key "+i+" ->"+keys[i].Value);

					string _keyval = Enum.GetName(keys[i].EnumType,  keys[i].Value);
					int _keyvalInt =  (int)((object)keys[i].Value);
					if (Properties.ContainsKey(_keyval))
					{
						Debug.Log("found key via string "+i+" ->"+keys[i].Value+" ="+Properties[_keyval]);
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,datas[i],Properties[_keyval]);
					}else if (Properties.ContainsKey(_keyvalInt))
					{
						Debug.Log("found key via int "+i+" ->"+keys[i].Value+" ="+Properties[_keyvalInt]);
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,datas[i],Properties[_keyvalInt]);
					}else if (Properties.ContainsKey((byte)_keyvalInt))
					{
						Debug.Log("found key via byte "+i+" ->"+keys[i].Value+" ="+Properties[(byte)_keyvalInt]);
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,datas[i],Properties[(byte)_keyvalInt]);
					}else
					{
						Fsm.Event(notFoundEvent);
					}
				}
				
			}catch(Exception e)
			{
				Fsm.Event(notFoundEvent);
				Debug.Log("no properties found "+e);
			}
			
			Finish();
		}
	}
}