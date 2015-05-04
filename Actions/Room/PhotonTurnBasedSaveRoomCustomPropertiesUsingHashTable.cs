// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using HutongGames.PlayMaker.Photon.TurnBased;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon TurnBased")]
	[Tooltip("Save custom properties of this Room taking keys and values from ArrayMaker Hashtable.")]
	public class PhotonTurnBasedSaveRoomCustomPropertiesUsingHashTable : HashTableActions
	{
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Event fired if we are not in a room.")]
		public FsmEvent notInRoomEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			notInRoomEvent = null;
		}
		
		
		public override void OnEnter()
		{
			Room _room = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom;
			
			if (_room==null)
			{
				Fsm.Event(notInRoomEvent);
			}else{
				if (SetUpHashTableProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value))
				{
					SaveCustomProperties();
				}
			}

			Finish();
		}

		public void SaveCustomProperties()
		{
			ExitGames.Client.Photon.Hashtable _customProperties = new Hashtable();

			foreach(DictionaryEntry entry in proxy._hashTable)
			{
				_customProperties[entry.Key.ToString()] = entry.Value;
			}

			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.SetCustomProperties(_customProperties);
		}
		
		
	}
}