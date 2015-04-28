using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;

using HutongGames.PlayMaker;

using HutongGames.PlayMaker.Photon;

namespace HutongGames.PlayMaker.Photon.TurnBased
{
	public class PlayMakerPhotonLoadBalancingClientProxy : MonoBehaviour
	{
		private static bool debug = false;

		public static PlayMakerPhotonLoadBalancingClientProxy instance;
		public static PlayMakerFSM Fsm;

		public PlayMakerPhotonLoadBalancingClient LbcInstance;

		/// <summary>
		/// Register to this action to start working with the LoadBalancing system
		/// </summary>>
		public Action OnServiceStarted { get; set; }


		public string appId ="Your AppID here";
		public string appVersion = "1.0";

		public string state ="Uninitialized";

		public StatusCode statusCode;

		/// <summary>
		/// The last event data received. Use with "PhotonTurnBasedGetLastEventData" action
		/// </summary>
		public static EventData lastEventData;

		#region MONO BEHAVIOR CALLS
		void Awake()
		{
			instance = this;
			Fsm = GetComponent<PlayMakerFSM>();
		}

		void Start()
		{
			Application.runInBackground = true;
			CustomTypes.Register();
		}

		public void Update ()
		{
			if (LbcInstance!=null)
			{
				LbcInstance.Service();
			}
			
		}
		
		public void OnApplicationQuit()
		{
			if (LbcInstance != null && LbcInstance.loadBalancingPeer != null)
			{
				LbcInstance.Disconnect();
				LbcInstance.loadBalancingPeer.StopThread();
			}
			LbcInstance = null;
		}
		#endregion MONO BEHAVIOR CALLS

		public PlayMakerPhotonLoadBalancingClient StartLoadBalancingClient()
		{

			LbcInstance = new PlayMakerPhotonLoadBalancingClient ();
			LbcInstance.OnStateChangeAction += this.OnStateChanged;
			LbcInstance.OnStatusChangedAction += this.OnStatusChanged;

			LbcInstance.OnEventAction += this.OnEventCallback;
			if (OnServiceStarted!=null)
			{
				OnServiceStarted();
			}
			return LbcInstance;
		}

		private void OnEventCallback(EventData photonEvent)
		{	
			if (EventCodetoPlayMakerEventsLUT.ContainsKey(photonEvent.Code))
			{
				string _e = EventCodetoPlayMakerEventsLUT[photonEvent.Code];
				if (debug) Debug.Log("OnEventCallback ->"+photonEvent.Code+" -> "+_e);
				PlayMakerFSM.BroadcastEvent(_e);
			}
		}

		private void OnStateChanged (ClientState clientState)
		{

			state = clientState.ToString ();

			string _e = clientStatetoPlayMakerEventsLUT[clientState];
			if (debug) Debug.Log("OnStateChanged ->"+clientState+" -> "+_e);
			PlayMakerFSM.BroadcastEvent(_e);
		}

		private void OnStatusChanged(StatusCode _statusCode)
		{
			statusCode = _statusCode;

			string _e = statusCodetoPlayMakerEventsLUT[_statusCode];
			if (debug) Debug.Log("OnStatusChanged ->"+_statusCode+" -> "+_e);
			PlayMakerFSM.BroadcastEvent(_e);

			switch (_statusCode)
			{
			case StatusCode.Exception:
			case StatusCode.ExceptionOnConnect:
				Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + LbcInstance.MasterServerAddress+ ") reachable?");
				break;
			case StatusCode.Disconnect:
				//HideBoard();
				break;
			}
		}


		static Dictionary<byte, string> EventCodetoPlayMakerEventsLUT = new Dictionary<byte, string>()
		{
			{ EventCode.AppStats,			 				"PHOTON TURNBASED / EVENT / APP STATS"},
			{ EventCode.CacheSliceChanged,			 		"PHOTON TURNBASED / EVENT / CACHE SLICE CHANGED"},
			{ EventCode.Disconnect,			 				"PHOTON TURNBASED / EVENT / DISCONNECT"},
			{ EventCode.ErrorInfo,			 				"PHOTON TURNBASED / EVENT / ERROR INFO"},
			{ EventCode.GameList,			 				"PHOTON TURNBASED / EVENT / GAME LIST"},
			{ EventCode.GameListUpdate,			 			"PHOTON TURNBASED / EVENT / GAME LIST UPDATE"},
			{ EventCode.Join,			 					"PHOTON TURNBASED / EVENT / JOIN"},
			{ EventCode.Leave,			 					"PHOTON TURNBASED / EVENT / LEAVE"},
			{ EventCode.Match,			 					"PHOTON TURNBASED / EVENT / MATCH"},
			{ EventCode.PropertiesChanged,			 		"PHOTON TURNBASED / EVENT / PROPERTIES CHANGED"},
			{ EventCode.QueueState,			 				"PHOTON TURNBASED / EVENT / QUEUE STATE"}
		};

		static Dictionary<ClientState, string> clientStatetoPlayMakerEventsLUT = new Dictionary<ClientState, string>()
		{
			{ ClientState.Uninitialized,			 		"PHOTON TURNBASED / UNINITIALIZED"},
			{ ClientState.ConnectingToMasterserver,			"PHOTON TURNBASED / CONNECTING TO MASTER SERVER"},
			{ ClientState.ConnectedToMaster,				"PHOTON TURNBASED / CONNECTED TO MASTER"},
			{ ClientState.Queued,							"PHOTON TURNBASED / QUEUED"},
			{ ClientState.Authenticating,					"PHOTON TURNBASED / AUTHENTICATING"},
			{ ClientState.Authenticated,					"PHOTON TURNBASED / AUTHENTICATED"},
			{ ClientState.JoinedLobby,						"PHOTON TURNBASED / JOINED LOBBY"},
			{ ClientState.DisconnectingFromMasterserver,	"PHOTON TURNBASED / DISCONNECTING FROM MASTER SERVER"},
			{ ClientState.ConnectingToGameserver, 			"PHOTON TURNBASED / CONNECTING TO GAME SERVER"},
			{ ClientState.ConnectedToGameserver, 			"PHOTON TURNBASED / CONNECTED TO GAME SERVER"},
			{ ClientState.Joining, 							"PHOTON TURNBASED / JOINING"},
			{ ClientState.Joined, 							"PHOTON TURNBASED / JOINED"},
			{ ClientState.Leaving, 							"PHOTON TURNBASED / LEAVING"},
			{ ClientState.Left, 							"PHOTON TURNBASED / LEFT"},
			{ ClientState.DisconnectingFromGameserver, 		"PHOTON TURNBASED / DISCONNECTING FROM GAME SERVER"},
			{ ClientState.QueuedComingFromGameserver ,		"PHOTON TURNBASED / QUEUED COMING FROM GAME SERVER" },
			{ ClientState.Disconnecting , 					"PHOTON TURNBASED / DISCONNECTING"},
			{ ClientState.Disconnected , 					"PHOTON TURNBASED / DISCONNECTED" },
			{ ClientState.ConnectingToNameServer , 			"PHOTON TURNBASED / CONNECTING TO NAME SERVER"},
			{ ClientState.ConnectedToNameServer , 			"PHOTON TURNBASED / CONNECTED TO NAME SERVER"},
			{ ClientState.DisconnectingFromNameServer , 	"PHOTON TURNBASED / DISCONNECTING FROM NAME SERVER"},

		};

		static Dictionary<StatusCode, string> statusCodetoPlayMakerEventsLUT = new Dictionary<StatusCode, string>()
		{
			{ StatusCode.Connect,			 				"PHOTON TURNBASED / STATUS / CONNECT"},
			{ StatusCode.Disconnect,			 			"PHOTON TURNBASED / STATUS / DISCONNECT"},
			{ StatusCode.Exception,			 				"PHOTON TURNBASED / STATUS / EXCEPTION"},
			{ StatusCode.ExceptionOnConnect,				"PHOTON TURNBASED / STATUS / EXCEPTION ON CONNECT"},
			{ StatusCode.SecurityExceptionOnConnect,		"PHOTON TURNBASED / STATUS / SECURITY EXCEPTION ON CONNECT"},
			{ StatusCode.QueueOutgoingReliableWarning,		"PHOTON TURNBASED / STATUS / QUEUE OUTGOING RELIABLE WARNING"},
			{ StatusCode.QueueOutgoingUnreliableWarning,	"PHOTON TURNBASED / STATUS / QUEUE OUTGOING UNRELIABLE WARNING"},
			{ StatusCode.SendError,			 				"PHOTON TURNBASED / STATUS / SEND ERROR"},
			{ StatusCode.QueueOutgoingAcksWarning,			"PHOTON TURNBASED / STATUS / QUEUE OUTGOING ACKS WARNING"},
			{ StatusCode.QueueIncomingReliableWarning,		"PHOTON TURNBASED / STATUS / QUEUE INCOMING RELIABLE WARNING"},
			{ StatusCode.QueueIncomingUnreliableWarning,	"PHOTON TURNBASED / STATUS / QUEUE INCOMING UNRELIABLE WARNING"},
			{ StatusCode.QueueSentWarning,			 		"PHOTON TURNBASED / STATUS / QUEUE SENT WARNING"},
			{ StatusCode.ExceptionOnReceive,			 	"PHOTON TURNBASED / STATUS / EXCEPTION ON RECEIVE"},
			{ StatusCode.TimeoutDisconnect,			 		"PHOTON TURNBASED / STATUS / TIMEOUT DISCONNECT"},
			{ StatusCode.DisconnectByServer,			 	"PHOTON TURNBASED / STATUS / DISCONNECT BY SERVER"},
			{ StatusCode.DisconnectByServerUserLimit,		"PHOTON TURNBASED / STATUS / DISCONNECT BY SERVER USER LIMIT"},
			{ StatusCode.DisconnectByServerLogic,			"PHOTON TURNBASED / STATUS / DISCONNECT BY SERVER LOGIC"},
			{ StatusCode.EncryptionEstablished,				"PHOTON TURNBASED / STATUS / ENCRYPTION ESTABLISHED"},
			{ StatusCode.EncryptionFailedToEstablish,		"PHOTON TURNBASED / STATUS / ENCRYPTION FAILED TO ESTABLISH"}
		};	
	}
}