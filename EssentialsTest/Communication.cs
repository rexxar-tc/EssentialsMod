using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game;

namespace DedicatedEssentials
{
    public static class Communication
    {
        private static Random _random = new Random();
        public static void Message( string text )
        {
            MyAPIGateway.Utilities.ShowMessage( "[Essentials]", text );
        }

        public static void Message( string from, string text, bool brackets = true )
        {
            MyAPIGateway.Utilities.ShowMessage( from, text );
        }

        public static void Notification( string text, int disappearTimeMS = 2000, MyFontEnum fontEnum = MyFontEnum.White )
        {
            if ( disappearTimeMS > 0 )
                MyAPIGateway.Utilities.ShowNotification( text, disappearTimeMS, fontEnum );
        }

        public static void Dialog( string title, string prefix, string current, string description, string buttonText )
        {
            MyAPIGateway.Utilities.ShowMissionScreen( title, prefix, current, description, null, buttonText );
        }

        public static void SendDataToServer( long dataId, string text )
        {
            var item = new MessageRecieveItem
                       {
                           fromID = MyAPIGateway.Session.Player.SteamUserId,
                           msgID = dataId,
                           message = text
                       };
            //hash a random long with the current time and steamID to make a decent quality guid for each message
            byte[] randLong = new byte[sizeof(long)];
            _random.NextBytes(randLong);
            long uniqueId = 23;
            uniqueId = uniqueId * 37 + BitConverter.ToInt64(randLong, 0);
            uniqueId = uniqueId * 37 + DateTime.Now.GetHashCode();
            uniqueId = uniqueId * 37 + MyAPIGateway.Session.Player.SteamUserId.GetHashCode();

            string messageString = MyAPIGateway.Utilities.SerializeToXML( item );
            byte[] messageBytes = Encoding.UTF8.GetBytes( messageString );
            byte[] data = new byte[messageBytes.Length + sizeof(long)];
            BitConverter.GetBytes( uniqueId ).CopyTo( data, 0 );
            messageBytes.CopyTo( data, sizeof(long) );

            MyAPIGateway.Multiplayer.SendMessageToServer( 9001, data );
            //MyAPIGateway.Multiplayer.SendMessageToServer( 9003, data );
        }

        public static void SendMessageParts( long dataId, byte[] data )
        {
        }

        public static void ReceiveMessageParts( byte[] data )
        {
            byte[] message = Desegment( data );
            
            if(message == null)
                return;

            EssentialsCore.HandleServerData( message );
        }
        
        public class MessageRecieveItem
        {
            public ulong fromID { get; set; }
            public long msgID { get; set; }
            public string message { get; set; }
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
        private static Dictionary<int, PartialMessage> messages = new Dictionary<int, PartialMessage>();
    private const int PACKET_SIZE = 4096;
    private const int META_SIZE = sizeof(int) * 2;
    private const int DATA_LENGTH = PACKET_SIZE - META_SIZE;
 
    /// <summary>
    /// Segments a byte array.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static List<byte[]> Segment(byte[] message)
    {
        var hash = BitConverter.GetBytes(message.GetHashCode());
        var packets = new List<byte[]>();
        int msgIndex = 0;
 
        int packetId = message.Length / DATA_LENGTH;
 
        while (packetId >= 0)
        {
            var id = BitConverter.GetBytes(packetId);
            byte[] segment;
 
            if (message.Length - msgIndex > DATA_LENGTH)
            {
                segment = new byte[PACKET_SIZE];
            }
            else
            {
                segment = new byte[META_SIZE + message.Length - msgIndex];
            }
 
            //Copy packet "header" data.
            Array.Copy(hash, segment, hash.Length);
            Array.Copy(id, 0, segment, hash.Length, id.Length);
 
            //Copy segment of original message.
            Array.Copy(message, msgIndex, segment, META_SIZE, segment.Length - META_SIZE);
 
            packets.Add(segment);
            msgIndex += DATA_LENGTH;
            packetId--;
        }
 
        return packets;
    }
 
    /// <summary>
    /// Reassembles a segmented byte array.
    /// </summary>
    /// <param name="packet">Array segment.</param>
    /// <param name="message">Full array, null if incomplete.</param>
    /// <returns>Message fully desegmented, "message" is assigned.</returns>
    public static byte[] Desegment(byte[] packet)
    {
        int hash = BitConverter.ToInt32(packet, 0);
        int packetId = BitConverter.ToInt32(packet, sizeof(int));
        byte[] dataBytes = new byte[packet.Length - META_SIZE];
        Array.Copy(packet, META_SIZE, dataBytes, 0, packet.Length - META_SIZE);
 
        if (!messages.ContainsKey(hash))
        {
            if (packetId == 0)
            {
                return dataBytes;
            }
            else
            {
                messages.Add(hash, new PartialMessage(packetId));
            }
        }
 
        var message = messages[hash];
        message.WritePart(packetId, dataBytes);
 
        if (message.IsComplete)
        {
            messages.Remove(hash);
            return message.Data;
        }
 
        return null;
    }
 
    private class PartialMessage
    {
        public byte[] Data;
        private HashSet<int> receivedPackets = new HashSet<int>();
        private readonly int MaxId;
        public bool IsComplete { get { return receivedPackets.Count == MaxId + 1; } }
 
 
        public PartialMessage(int startId)
        {
            MaxId = startId;
            Data = new byte[0];
        }
 
        public void WritePart(int id, byte[] data)
        {
            int index = MaxId - id;
            int requiredLength = (index * DATA_LENGTH) + data.Length;
 
            if (Data.Length < requiredLength)
            {
                Array.Resize(ref Data, requiredLength);
            }
 
            Array.Copy(data, 0, Data, index * DATA_LENGTH, data.Length);
            receivedPackets.Add(id);
        }
    }
    }
}
