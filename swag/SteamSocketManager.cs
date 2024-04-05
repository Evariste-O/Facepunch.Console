using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace swag
{
	public class SteamSocketManager : SocketManager
	{
		public override void OnConnecting( Connection connection, ConnectionInfo data )
		{
			base.OnConnecting( connection, data );
			Console.WriteLine( $"{data.Identity} is connecting" );
		}

		public override void OnConnected( Connection connection, ConnectionInfo data )
		{
			base.OnConnected( connection, data );
			Console.WriteLine( $"{data.Identity} has joined the game" );
		}

		public override void OnDisconnected( Connection connection, ConnectionInfo data )
		{
			base.OnDisconnected( connection, data );
			Console.WriteLine( $"{data.Identity} is out of here" );
		}

		public override void OnMessage( Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel )
		{
			base.OnMessage( connection, identity, data, size, messageNum, recvTime, channel );
			Console.WriteLine( $"We got a message from {identity}!" );
			byte[] managedArray = new byte[size];
			Marshal.Copy(data, managedArray, 0, size);
			var str = System.Text.Encoding.Default.GetString(managedArray);
			Console.WriteLine("Received message: "+ str);
		}
	}
}
