using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace DedicatedEssentials
{
    public class ServerDataDialog : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5020;
        }

        public override void HandleCommand( byte[ ] data )
        {
            string text = "";
            for ( int r = 0; r < data.Length; r++ )
                text += (char)data[r];

            ServerDialogItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerDialogItem>( text );
            if ( item != null )
                Communication.Dialog( item.title, item.header, "", item.content.Replace( "|", "\r\n" ), item.buttonText );
        }
    }

    public class ServerDialogItem
        {
            public string title { get; set; }
            public string header { get; set; }
            public string content { get; set; }
            public string buttonText { get; set; }
        }
}
