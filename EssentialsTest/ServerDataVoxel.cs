using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Common.ObjectBuilders.VRageData;
using VRageMath;
using VRage;

namespace DedicatedEssentials
{
	public class ServerDataVoxelHeader : ServerDataHandlerBase
	{
		private static Dictionary<long, VoxelHeaderData> m_voxelHeaders = new Dictionary<long, VoxelHeaderData>();
		public static Dictionary<long, VoxelHeaderData> VoxelHeaders
		{
			get { return m_voxelHeaders; }
		}

		public override long GetDataId()
		{
			return 5001;
		}

		public override void HandleCommand(byte[] data)
		{
			ushort headerLength = (ushort)(data[0] | data[1] << 8);
			string headerString = "";
			for (int r = 0; r < headerLength; r++)
			{
				headerString += (char)data[r + 2];
			}

			VoxelHeaderData header = MyAPIGateway.Utilities.SerializeFromXML<VoxelHeaderData>(headerString);
			//byte[] asteroidData = new byte[data.Length - (headerLength + 2)];
			//Array.Copy(data, headerLength + 2, asteroidData, 0, data.Length - (headerLength + 2));

			//Communication.Message(string.Format("Loading Asteroid - {0} - {1} - {2} - {3} - data: ({4},{5} - {6})", header.EntityId, header.Name, (Vector3I)header.HalfExtent, (Vector3D)header.Position, asteroidData[0], asteroidData[1], asteroidData.Length));
			//MyAPIGateway.Session.VoxelMaps.CreateVoxelMap(header.Name, MyAPIGateway.Session.VoxelMaps.CreateStorage(asteroidData), (Vector3D)header.Position - (Vector3D)((Vector3I)header.HalfExtent), header.EntityId); 
			//Communication.Message("Asteroid Loaded!");
			if (m_voxelHeaders.ContainsKey(header.EntityId))
				m_voxelHeaders.Remove(header.EntityId);

			m_voxelHeaders.Add(header.EntityId, header);

			Logging.Instance.WriteLine(string.Format("Received header: {0}", header.EntityId));
		}
	}

	public class VoxelHeaderData
	{
		public long EntityId { get; set; }
		public string Name { get; set; }
		public SerializableVector3I HalfExtent { get; set; }
		public SerializableVector3D Position { get; set; }
		public int DataLength { get; set; }
	}
}
