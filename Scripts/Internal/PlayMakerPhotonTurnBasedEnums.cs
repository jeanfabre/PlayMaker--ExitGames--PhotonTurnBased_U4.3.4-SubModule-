using System;
using System.Collections;

namespace HutongGames.PlayMaker.Photon.TurnBased
{

	/// <summary>
	/// Exact Mirror of the ErrorCode class from Photon TurnBased internal implementation. 
	/// </summary>
	/// <remarks>
	/// Inside PlayMaker, Enum can be used,
	/// but it's impossible for PlayMaker as is to use a class with constants, there isn't a mechanism to expose this kind of scripting visually
	/// </remarks>
	public enum ErrorCode
	{
		Ok = 0,
		OperationNotAllowedInCurrentState = -3,
		InvalidOperationCode = -2,
		InternalServerError = -1,
		InvalidAuthentication = 0x7FFF,
		GameIdAlreadyExists = 0x7FFF - 1,
		GameFull = 0x7FFF - 2,
		GameClosed = 0x7FFF - 3,
		AlreadyMatched = 0x7FFF - 4,
		ServerFull = 0x7FFF - 5,
		UserBlocked = 0x7FFF - 6,
		NoRandomMatchFound = 0x7FFF - 7,
		GameDoesNotExist = 0x7FFF - 9,
		MaxCcuReached = 0x7FFF - 10,
		InvalidRegion = 0x7FFF - 11,
		CustomAuthenticationFailed = 0x7FFF - 12
	}


}
