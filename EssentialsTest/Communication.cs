using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRageMath;
using VRage.ObjectBuilders;
using VRage;
using Sandbox.Common;
using System.Text;
using VRage.Game;
using VRage.Library.Collections;

namespace DedicatedEssentials
{
    public static class Communication
    {
        static public void Message(string text)
        {
            MyAPIGateway.Utilities.ShowMessage("[Essentials]", text);
        }

		static public void Message(string from, string text, bool brackets = true)
		{
			MyAPIGateway.Utilities.ShowMessage(string.Format("{0}", from), text);
		}

        static public void Notification(string text, int disappearTimeMS = 2000, MyFontEnum fontEnum = MyFontEnum.White)
        {
            if ( disappearTimeMS > 0 )
                MyAPIGateway.Utilities.ShowNotification(text, disappearTimeMS, fontEnum);
        }

		static public void Dialog(string title, string prefix, string current, string description, string buttonText)
		{
			MyAPIGateway.Utilities.ShowMissionScreen(title, prefix, current, description, null, buttonText);
		}
        
		public static void SendDataToServer(long dataId, string text)
		{
            /* Let's try something else instead...
			string msgIdString = dataId.ToString();
            string steamIdString = MyAPIGateway.Session.Player.SteamUserId.ToString();

			byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
			byte[] newData = new byte[text.Length + 1 + msgIdString.Length + steamIdString.Length + 1];

            int pos = 0;
			newData[pos] = (byte)msgIdString.Length;
            for (int r = 0; r < msgIdString.Length; r++)
            {
                pos++;
                newData[pos] = (byte)msgIdString[r];
            }
            pos++;

            newData[pos] = (byte)steamIdString.Length;
            for (int r = 0; r < steamIdString.Length; r++ )
            {
                pos++;
                newData[pos] = (byte)steamIdString[r];
            }
            pos++;

            Array.Copy(data, 0, newData, pos, data.Length);*/

            MessageRecieveItem item = new MessageRecieveItem( );
            item.fromID = MyAPIGateway.Session.Player.SteamUserId;
            item.msgID = dataId;
            item.message = text;

            string messageString = MyAPIGateway.Utilities.SerializeToXML<MessageRecieveItem>( item );
            byte[ ] data = Encoding.UTF8.GetBytes( messageString );

            MyAPIGateway.Multiplayer.SendMessageToServer(9001, data);
            //MyAPIGateway.Multiplayer.SendMessageToServer( 9003, data );
        }

        public static void SendMessageParts( long dataId, byte[] data )
        {
            List<byte> byteList = data.ToList();
            while ( byteList.Count>0 )
            {
                int count = Math.Min( byteList.Count, 3500 );
                byte[] dataPart = byteList.GetRange( 0, count ).ToArray();
                byteList.RemoveRange( 0, count );

                var partItem = new MessagePartItem
                {
                    DataId = dataId,
                    Data = dataPart,
                    LastPart = byteList.Count == 0
                };

                var message = MyAPIGateway.Utilities.SerializeToXML( partItem );
                var outData = Encoding.UTF8.GetBytes( message );

                MyAPIGateway.Multiplayer.SendMessageToServer( 9005, outData );
            }
        }

        private static List<MessagePartItem> receiveParts = new List<MessagePartItem>();

        public static void ReveiveMessageParts( byte[] data )
        {
            var message = Encoding.UTF8.GetString( data );
            var item = MyAPIGateway.Utilities.SerializeFromXML<MessagePartItem>(message);

            receiveParts.Add( item );

            if ( !item.LastPart )
                return;

            List<byte> bytesList = new List<byte>();
            foreach ( var part in receiveParts )
            {
                foreach( var dataByte in part.Data)
                    bytesList.Add( dataByte );
            }

            EssentialsCore.HandleServerData( bytesList.ToArray() );
        }

        public class MessagePartItem
        {
            public long DataId;
            public bool LastPart;
            public byte[] Data;
        }

        public class MessageRecieveItem
        {
            public ulong fromID
            {
                get; set;
            }
            public long msgID
            {
                get; set;
            }
            public string message
            {
                get; set;
            }
        }

        public class ServerDialogItem
        {
            public string title { get; set; }
            public string header { get; set; }
            public string content { get; set; }
            public string buttonText { get; set; }
        }

        public class ServerNotificationItem
        {
            public MyFontEnum color { get; set; }
            public int time { get; set; }
            public string message { get; set; }
        }

        public class ServerMoveItem
        {
            public string moveType { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
        }

        public enum DataMessageType
        {
            Test = 5000,
            VoxelHeader,
            VoxelPart,
            Message,
            RemoveStubs,
            ChangeServer,
            ServerSpeed,
            Credits,

            //these are new and need implemented client side
            Dialog,
            Move,
            Notification,
            MaxSpeed,
            ServerInfo
        }        
    }
}
