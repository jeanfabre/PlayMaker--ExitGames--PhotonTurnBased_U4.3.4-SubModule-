
using System;
using System.Collections.Generic;

using UnityEngine;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

using HutongGames.PlayMaker.Photon;

namespace HutongGames.PlayMaker.Photon.TurnBased
{

	public class GameDescription
	{
		public string GameId;
		public int ActorNr;
		public Dictionary<string, object> Properties;
	}

	public class PlayMakerPhotonLoadBalancingClient : LoadBalancingClient
	{
		#region Action Delegates
		/// <summary>
		/// Use this Action to be informed when the GameList is received
		/// </summary>
		public Action 						OnGameListReceivedAction		{ get; set; }

		/// <summary>
		/// Use this Action to be informed when joining a game Operation got a response 
		/// </summary>
		public Action<short> 			OnJoinGameResponseAction		{ get; set; }

		/// <summary>
		/// Use this Action to be informed when joining a random game Operation got a response 
		/// </summary>
		public Action<short> 			OnJoinRandomGameResponseAction		{ get; set; }

		public Action<StatusCode> 			OnStatusChangedAction			{ get; set; }
		//public Action<DebugLevel, string> 	OnDebugReturnAction				{ get; set; }
		//public Action<OperationResponse> 	OnOperationResponseAction		{ get; set; }
		public Action<EventData> 			OnEventAction					{ get; set; }
		#endregion

		public string ErrorMessageToShow { get; set; }


		public Dictionary<string,GameDescription> SavedGames = new Dictionary<string, GameDescription>();


		public override void OnOperationResponse(OperationResponse operationResponse)
		{
			base.OnOperationResponse(operationResponse);
			this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());
			
			switch (operationResponse.OperationCode)
			{
			case (byte)OperationCode.WebRpc:
				if (operationResponse.ReturnCode == 0)
				{
					this.OnWebRpcResponse(new WebRpcResponse(operationResponse));
				}
				break;

			case (byte)OperationCode.JoinGame:
				if(OnJoinGameResponseAction!=null)
				{
					DebugReturn(DebugLevel.INFO,"OnOperationResponse code:JoinGame calling Action:OnJoinGameResponseAction");
					OnJoinGameResponseAction(operationResponse.ReturnCode);
				}
				break;

			case (byte)OperationCode.CreateGame:
				if (this.Server == ServerConnection.GameServer)
				{
					if (operationResponse.ReturnCode == 0)
					{
						this.UpdateBoard();
					}
				}
				break;

			case (byte)OperationCode.JoinRandomGame:

				if(OnJoinRandomGameResponseAction!=null)
				{
					DebugReturn(DebugLevel.INFO,"OnOperationResponse code:JoinRandomGame calling Action:OnJoinRandomGameResponseAction");
					OnJoinRandomGameResponseAction(operationResponse.ReturnCode);
				}
				break;
			}
		}
		
		
		public override void OnEvent(EventData photonEvent)
		{
			base.OnEvent(photonEvent);
			
			switch (photonEvent.Code)
			{
			case EventCode.PropertiesChanged:
				Debug.Log("Got Properties via Event. Update board by room props.");
				this.UpdateBoard();
				break;
			case EventCode.Join:
				if (this.CurrentRoom.Players.Count == 2 && this.CurrentRoom.IsOpen)
				{
					this.CurrentRoom.IsOpen = false;
					this.CurrentRoom.IsVisible = false;
				}
				break;
			}

			if(OnEventAction!=null)
			{
				DebugReturn(DebugLevel.INFO,"Calling Action:OnEventAction EventCode: "+photonEvent.Code);

				OnEventAction(photonEvent);
			}
		}
		
		public override void DebugReturn(DebugLevel level, string message)
		{
			base.DebugReturn(level, message);

			Debug.Log(message);
		}


		public override void OnStatusChanged(StatusCode statusCode)
		{
			base.OnStatusChanged(statusCode);

			if (OnStatusChangedAction!=null)
			{
				OnStatusChangedAction(statusCode);
			}
		}

		private void OnWebRpcResponse(WebRpcResponse response)
		{

			DebugReturn(DebugLevel.INFO,"OnWebRpcResponse "+response.ReturnCode+ " " +response.Name);

			if (response.ReturnCode != 0)
			{
				Debug.Log(response.ToStringFull());     // in an error case, it's often helpful to see the full response
				return;
			}

			if (response.Name.Equals("GetGameList"))
			{
				this.SavedGames.Clear();

				if (response.ReturnCode == 0)
				{
					if (response.Parameters == null)
					{
						DebugReturn(DebugLevel.INFO,"WebRpcResponse for GetGameList contains no rooms: " + response.ToStringFull());
					}else{

						// the response for GetGameList contains a Room's name as Key and another Dictionary<string,object> with the values the web service sends
						foreach (KeyValuePair<string, object> pair in response.Parameters)
						{
							// per key (room name), we send 
							// "ActorNr" which is the PlayerId/ActorNumber this user had in the room
							// "Properties" which is another Dictionary<string,object> with the properties that the lobby sees
							Dictionary<string, object> roomValues = pair.Value as Dictionary<string, object>;


							GameDescription _game = new GameDescription();
							_game.GameId = pair.Key;
							_game.ActorNr = (int)roomValues["ActorNr"];
							_game.Properties = roomValues["Properties"] as Dictionary<string, object>;

							this.SavedGames.Add(pair.Key, _game);

							Debug.Log(_game.GameId + " actorNr: " + _game.ActorNr + " props: " + SupportClass.DictionaryToString(_game.Properties));
						}
					}
				}

				if(OnGameListReceivedAction!=null)
				{
					DebugReturn(DebugLevel.INFO,"OnWebRpcResponse OnGameListReceivedAction");
					OnGameListReceivedAction();
				}
			}




		}



		/*
		public void SaveBoardAsProperty()
		{
			MemoryBoard board = GameObject.FindObjectOfType<MemoryBoard>();
			
			Hashtable boardProps = board.GetBoardAsCustomProperties();
			boardProps.Add("pt", this.PlayerIdToMakeThisTurn);  // "pt" is for "player turn" and contains the ID/actorNumber of the player who's turn it is
			boardProps.Add("t#", this.TurnNumber);
			boardProps.Add(GetPlayerPointsPropKey(this.LocalPlayer.ID), this.MyPoints); // we always only save "our" points. this will not affect the opponent's score.
			this.OpSetCustomPropertiesOfRoom(boardProps);
			
			Debug.Log("saved board to room-props " + SupportClass.DictionaryToString(boardProps));
		}
		*/

		string GetPlayerPointsPropKey(int id)
		{
			return "pt" + id;
		}
		
		byte GetPlayerPointsFromProps(int id)
		{
			string pointsKey = GetPlayerPointsPropKey(id);
			if (this.CurrentRoom.CustomProperties.ContainsKey(pointsKey))
			{
				return (byte)this.CurrentRoom.CustomProperties[pointsKey];
			}
			
			return 0;
		}
		
		public byte MyPoints = 0;
		public byte OthersPoints = 0;
		
		
		public void UpdateBoard()
		{
			// we set properties "pt" (player turn) and "t#" (turn number). those props might have changed
			// it's easier to use a variable in gui, so read the latter property now
			if (this.CurrentRoom.CustomProperties.ContainsKey("t#"))
			{
				this.TurnNumber = (int) this.CurrentRoom.CustomProperties["t#"];
			}
			else
			{
				this.TurnNumber = 1;
			}
			
			if (this.CurrentRoom.CustomProperties.ContainsKey("pt"))
			{
				this.PlayerIdToMakeThisTurn = (int) this.CurrentRoom.CustomProperties["pt"];
			}
			else
			{
				// if the game didn't save a player's turn yet: use master
				this.PlayerIdToMakeThisTurn = this.CurrentRoom.MasterClientId;
			}
			
			this.MyPoints = GetPlayerPointsFromProps(this.LocalPlayer.ID);
			this.OthersPoints = GetPlayerPointsFromProps(this.GetOpponentsPlayerId());
			
			/*
			Hashtable roomProps = this.CurrentRoom.CustomProperties;

			MemoryBoard board = GameObject.FindObjectOfType<MemoryBoard>();
			bool success = board.SetBoardByCustomProperties(roomProps);
			Debug.Log("loaded board from room props: " + success);

			
			if (!success && this.LocalPlayer.IsMasterClient)
			{
				//board.GenerateNewBoard();
				this.SaveBoardAsProperty();
			}
			
			if (board.AreTwoTilesFlipped())
			{
				board.EndTurnDelayed();
			}
			
			if (!board.enabled)
			{
				board.enabled = true;
			}
			board.UpdateVisuals();
			*/
		}

		public void HandoverTurnToNextPlayer()
		{
			int idOfPlayerWhoDidntPlay = this.GetOtherPlayerId(this.PlayerIdToMakeThisTurn);
			this.PlayerIdToMakeThisTurn = idOfPlayerWhoDidntPlay;
		}
		
		/// <summary>Returns the player's actorNumber for the remote player (not LocalPlayer.ID) in a two-player game.</summary>
		/// <returns>The other player's actorNumber (same as player.ID) or -1</returns>
		public int GetOpponentsPlayerId()
		{
			return GetOtherPlayerId(this.LocalPlayer.ID);
		}
		
		/// <summary>Returns the actorNumber of the other player in a two-player game.</summary>
		/// <returns>The other player's actorNumber (same as player.ID) or -1</returns>
		public int GetOtherPlayerId(int currentPlayer)
		{
			if (this.CurrentRoom == null || this.CurrentRoom.Players == null || this.CurrentRoom.Players.Count != 2)
			{
				return -1;
			}
			
			foreach (int playerId in this.CurrentRoom.Players.Keys)
			{
				if (currentPlayer != playerId)
				{
					return playerId;
				}
			}
			
			return -1;
		}

		private const byte MaxPlayers = 2;
		
		public int TurnNumber = 1;
		
		public int PlayerIdToMakeThisTurn;  // who's turn this is. when "done", set the other player's actorNumber and save
		
		public bool IsMyTurn { get { return this.PlayerIdToMakeThisTurn == this.LocalPlayer.ID; } }
		
		
		
		public bool GameCanStart 
		{
			get { return this.CurrentRoom != null && this.CurrentRoom.Players.Count == 2; }
		}
		
		public bool GameWasAbandoned
		{
			get { return this.CurrentRoom != null && this.CurrentRoom.Players.Count < 2 && this.CurrentRoom.CustomProperties.ContainsKey("flips"); }
		}
		
		public bool IsMyScoreHigher
		{
			get { return this.MyPoints > this.OthersPoints; }
		}
		
		public bool IsScoreTheSame
		{
			get { return this.MyPoints == this.OthersPoints; }
		}
	}
}
