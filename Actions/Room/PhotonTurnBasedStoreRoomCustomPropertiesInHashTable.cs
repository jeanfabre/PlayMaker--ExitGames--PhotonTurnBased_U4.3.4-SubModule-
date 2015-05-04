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
	[Tooltip("Store custom properties of this Room in an ArrayMaker Hashtable.")]
	public class PhotonTurnBasedStoreRoomCustomPropertiesInHashTable : HashTableActions
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
					GetCustomProperties();
				}
			}

			Finish();
		}

		public void GetCustomProperties()
		{
			ExitGames.Client.Photon.Hashtable _customProperties = PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.CurrentRoom.CustomProperties;


			proxy._hashTable.Clear();
			foreach(DictionaryEntry entry in _customProperties)
			{
				Debug.Log(entry.Value.GetType());

				proxy._hashTable[entry.Key.ToString()] = entry.Value;
			}

		}
		
		
	}
}