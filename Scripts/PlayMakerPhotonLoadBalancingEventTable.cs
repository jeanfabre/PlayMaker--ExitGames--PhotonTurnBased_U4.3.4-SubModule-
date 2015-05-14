using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker.Ecosystem.Utils;

using HutongGames.PlayMaker.Photon;
using HutongGames.PlayMaker.Actions;


namespace HutongGames.PlayMaker.Photon.TurnBased
{
	[Serializable]
	public class PlayMakerByteEventReference
	{
		public byte Key = 0;
		public string EventName = "none";

		[NonSerialized]
		public int Count = 0;

	}

	public class PlayMakerPhotonLoadBalancingEventTable : MonoBehaviour {

		public PlayMakerEventTarget EventTarget = new PlayMakerEventTarget(ProxyEventTarget.BroadCastAll);

		public List<PlayMakerByteEventReference> Events;


		void OnEnable () {

			if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance!=null)
			{
				OnRegisterCallBack();
			}else{
				PlayMakerPhotonLoadBalancingClientProxy.instance.OnServiceStarted += OnRegisterCallBack;
			}


		}

		void OnRegisterCallBack()
		{
			PlayMakerPhotonLoadBalancingClientProxy.instance.OnServiceStarted -= OnRegisterCallBack;
			PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OnEventAction += OnEventActionCallBack;
		}

		void OnDisable()
		{
			if (PlayMakerPhotonLoadBalancingClientProxy.instance!=null)
			{
				PlayMakerPhotonLoadBalancingClientProxy.instance.OnServiceStarted -= OnRegisterCallBack;
				if (PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance!=null)
				{
					PlayMakerPhotonLoadBalancingClientProxy.instance.LbcInstance.OnEventAction -= OnEventActionCallBack;
				}
			}
		}

		void OnEventActionCallBack(EventData data)
		{
			Debug.Log("PlayMakerPhotonLoadBalancingEventTable received OnEventAction "+data.ToStringFull());

			//check if we have something in store
			foreach(var _item in Events)
			{
				//Debug.Log("_item key "+_item.Key +" code"+data.Code);
				if (_item.Key == data.Code)
				{
					Debug.Log ("Broadcasting event: "+_item.EventName);

					_item.Count ++;
					// get the custom data.
					PhotonTurnBasedGetEventProperties.Properties =  data.Parameters[ParameterCode.CustomEventContent] as ExitGames.Client.Photon.Hashtable;

					PlayMakerEvent _event = new PlayMakerEvent(_item.EventName);
					_event.SendEvent(PlayMakerPhotonLoadBalancingClientProxy.Fsm,EventTarget);

					#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
					#endif

				}
			}
		}

	}

}