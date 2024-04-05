using Steamworks;
using swag;
using System;
using System.Text.RegularExpressions;


var steamManager = new SteamManager();
steamManager.SetUp();
while ( true ) 
{
	var input = Console.ReadLine();
	MatchCollection matches = Regex.Matches( input, @"(\S+)" );
	string command = matches[0].Groups[0].Value;
	string parameter = "";
	if ( matches.Count > 1 ) 
	{
		parameter = matches[1].Groups[0].Value;
	}

	switch ( command ) 
	{
		case "createLobby":		
			steamManager.CreateLobby();
			break;
		case "getLobbies":
			steamManager.GetMultplayerLobbies();
			break;
		case "join":
			steamManager.JoinLobby( parameter );
			break;
		case "sendMessage":
			steamManager.SteamConnectionManager.Connection.SendMessage( parameter );
			break;
		default: break;
	}
}

