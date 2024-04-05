using Steamworks;
using Steamworks.Data;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;

namespace swag
{
	public class SteamManager
	{
		private Lobby hostedLobby { get; set; }
		private List<Lobby> AvailabeLobbies { get; set; } = new List<Lobby>();
		private SteamId PlayerSteamId { get; set; }
		public bool isHost { get; set; }
		public SteamSocketManager SteamSocketManager { get; set; }
		public SteamConnectionManager SteamConnectionManager { get; set; }

		public SteamManager() {
			try
			{
				SteamClient.Init( 480 );
				if ( !SteamClient.IsValid )
				{
					Console.WriteLine( "SteamClient is Invalid" );
					throw new Exception();
				}
				Console.WriteLine($"SteamClient Connected: {SteamClient.Name}");
				PlayerSteamId = SteamClient.SteamId;
			}
			catch (Exception )
			{
				// Couldn't init for some reason (dll errors, blocked ports)
			}
		}

		public void SetUp() 
		{
			SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
			SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
			SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
			SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyDisconnectedCallback;
			SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
			SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
		}

		private void OnLobbyEnteredCallback( Lobby lobby )
		{
			if(lobby.MemberCount > 1 )
			{
				Console.WriteLine( $"you entered {lobby.Owner.Name}'s Lobby: " );
			}
			else
			{
				Console.WriteLine("you have joined your own lobby: " + lobby.Id);
			}
			JoinSteamSocketServer( lobby.Owner.Id );
		}

		private void OnLobbyDisconnectedCallback( Lobby lobby, Friend friend) 
		{
			Console.WriteLine("User disconncted from lobby" + friend.Name);
		}

		private void OnLobbyMemberLeaveCallback( Lobby lobby, Friend friend )
		{
			Console.WriteLine( "User has left lobby" + friend.Name);
		}

		private void OnLobbyMemberJoinedCallback( Lobby lobby, Friend friend )
		{
			Console.WriteLine( "User has Joined lobby" );
		}

		private void OnLobbyCreatedCallback( Result result, Lobby lobby )
		{
			if(result != Result.OK )
			{
				Console.WriteLine( "Lobby creation has failed" );
			}
			else 
			{
				Console.WriteLine("lobby created!");
			}

			CreateSteamSocketServer();
		}

		private void OnLobbyGameCreatedCallback( Lobby lobby, uint id, ushort port, SteamId steamId )
		{
			Console.WriteLine( "lobby game created" );
		}

		public async Task<bool> CreateLobby() 
		{
			try
			{
				Console.WriteLine("creating Lobby");
				var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync( 8 );

				if ( !createLobbyOutput.HasValue ) 
				{
					Console.WriteLine( "lobby created but didnt instance correctly!" );
					throw new Exception();
				}

				hostedLobby = createLobbyOutput.Value;
				hostedLobby.SetPublic();
				hostedLobby.SetJoinable( true );
				hostedLobby.SetData( "ownerName", "swag" );
				Console.WriteLine( "Lobby Created! id =" + hostedLobby.Id );
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine( "Failed to create Lobby" + e.Message );
				return false;
			}
		}

		public async Task<bool> GetMultplayerLobbies()
		{
			try 
			{
				Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithMaxResults( 1000 ).RequestAsync();
				if ( lobbies != null ) 
				{
					foreach ( var lobby in lobbies ) 
					{
						Console.WriteLine(lobby.Id);
						AvailabeLobbies.Add( lobby );
					}
				}
				return true;
			}
			catch (Exception e) 
			{
				Console.WriteLine( "Error fetching Lobbies" + e.Message );
				return false;
			}
		}

		public async Task<bool> JoinLobby(string lobbyId) 
		{
			Console.WriteLine("joining Lobby:" + lobbyId);
			ulong id = ulong.Parse(lobbyId);
			try
			{
				await AvailabeLobbies.First( (lobby) => lobby.Id.Value == id ).Join();
				return true;
			}
			catch(Exception e) 
			{
				Console.WriteLine( "Joining lobby didnt work: " + e.Message );
				return false;
			}
		}

		public  async void run()
		{
			while ( true )
			{
				SteamClient.RunCallbacks();
				try
				{
					if( SteamSocketManager != null )
					{
						SteamSocketManager.Receive();
					}
					if(SteamConnectionManager != null && SteamConnectionManager.Connected )
					{
						SteamConnectionManager.Receive();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine ("an error occured: " +  e.Message );
				}
			}
		}

		public void CreateSteamSocketServer() 
		{
			SteamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>( 0 );
			SteamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>( PlayerSteamId );
			Console.WriteLine( "SocketServer created" );
		}

		public void JoinSteamSocketServer(SteamId host )
		{
			if ( !isHost )
			{
				Console.WriteLine("Joining SocketServer");
				SteamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>( host, 0 );
			}
		}
	}
}
