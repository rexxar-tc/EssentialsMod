using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRageMath;
using VRage.ObjectBuilders;
using VRage;
using Sandbox.Common;

namespace DedicatedEssentials
{
    static class Communication
    {
        static public void Message(String text)
        {
            MyAPIGateway.Utilities.ShowMessage("[Essentials]", text);
        }

		static public void Message(string from, string text, bool brackets = true)
		{
			MyAPIGateway.Utilities.ShowMessage(string.Format("{0}", from), text);
		}

        static public void Notification(String text, int disappearTimeMS = 2000, Sandbox.Common.MyFontEnum fontEnum = Sandbox.Common.MyFontEnum.White)
        {
            MyAPIGateway.Utilities.ShowNotification(text, disappearTimeMS, fontEnum);
        }

		static public void Dialog(string title, string prefix, string current, string description, string buttonText)
		{
			MyAPIGateway.Utilities.ShowMissionScreen(title, prefix, current, description, null, buttonText);
		}
        
        /// <summary>
        /// This is kind of shitty.  I should just bitshift and copy the lengths, but whatever.  We're not sending
        /// this enough to really care
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="text"></param>
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

            string messageString = MyAPIGateway.Utilities.SerializeToXML( item );
            byte[ ] data = new byte[messageString.Length];

            for ( int r = 0; r < messageString.Length; r++ )
            {
                data[r] = (byte)messageString[r];
            }

            MyAPIGateway.Multiplayer.SendMessageToServer(9001, data);
		}

        private class MessageRecieveItem
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
