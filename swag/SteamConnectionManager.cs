using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swag
{
	public class SteamConnectionManager : ConnectionManager
	{
		public override void OnConnected( ConnectionInfo info )
		{
			base.OnConnected( info );
			Console.WriteLine("on connection");
		}

		public override void OnConnecting( ConnectionInfo info )
		{
			base.OnConnecting( info );
			Console.WriteLine("on connecting");
		}

		public override void OnDisconnected( ConnectionInfo info ) 
		{  
			base.OnDisconnected( info );
			Console.WriteLine("on disconnection");
		}

		public override void OnMessage( nint data, int size, long messageNum, long recvTime, int channel )
		{
			base.OnMessage( data, size, messageNum, recvTime, channel );
			Console.WriteLine( "got a Message!" );
		}
	}
}
