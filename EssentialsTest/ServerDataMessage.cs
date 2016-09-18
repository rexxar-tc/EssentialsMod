using System.Text;
using Sandbox.ModAPI;

namespace DedicatedEssentials
{
	public class ServerDataMessage : ServerDataHandlerBase
	{
		public override long GetDataId()
		{
			return 5003;
		}

		public override void HandleCommand(byte[] data)
		{
            string text = Encoding.UTF8.GetString( data );

            ServerMessageItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerMessageItem>(text);
		    Logging.Instance.WriteLine( text );
		    if ( item != null )
		    {
		        Communication.Message(item.From, item.Message);
		        if ( item.Message.StartsWith( "GPS" ) )
		        {
		            var gps = Utility.ParseGps( item.Message );
		            if ( gps != null )
		                MyAPIGateway.Session.GPS.AddGps( MyAPIGateway.Session.Player.IdentityId, gps );
		        }
		    }
		}
	}

	public class ServerMessageItem
	{
		public string From { get; set; }
		public string Message { get; set; }
	}
}
