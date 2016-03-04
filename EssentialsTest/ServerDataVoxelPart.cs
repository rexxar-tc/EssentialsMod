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
	public class ServerDataVoxelData : ServerDataHandlerBase
	{
		private static Dictionary<long, byte[]> m_voxelData = new Dictionary<long, byte[]>();
		public Dictionary<long, byte[]> VoxelData
		{
			get { return m_voxelData; }
		}

		public override long GetDataId()
		{
			return 5002;
		}

		public override void HandleCommand(byte[] data)
		{
			long entityId = (long)(((long)data[7] << 56) + ((long)data[6] << 48) + ((long)data[5] << 40) + ((long)data[4] << 32) + ((long)data[3] << 24) + ((long)data[2] << 16) + (long)(data[1] << 8) + (long)data[0]);
			ushort partLength = (ushort)(((ushort)data[9] << 8) + (ushort)data[8]);
			ushort part = (ushort)(((ushort)data[11] << 8) + (ushort)data[10]);

			Logging.Instance.WriteLine(string.Format("Received Part: {0} {1} {2}", entityId, partLength, part));

			if (!ServerDataVoxelHeader.VoxelHeaders.ContainsKey(entityId))
				return;

			VoxelHeaderData header = ServerDataVoxelHeader.VoxelHeaders[entityId];
			if(!m_voxelData.ContainsKey(entityId))
				m_voxelData.Add(entityId, new byte[header.DataLength]);

			byte[] voxelData = m_voxelData[entityId];
			Array.Copy(data, 12, voxelData, part * 4096, partLength);

			if (part >= (header.DataLength / 4096))
			{				
				MyAPIGateway.Session.VoxelMaps.CreateVoxelMap(header.Name, MyAPIGateway.Session.VoxelMaps.CreateStorage(voxelData), (Vector3D)header.Position - (Vector3D)((Vector3I)header.HalfExtent), header.EntityId);
				Logging.Instance.WriteLine(string.Format("Adding Voxel To World: {0}", header.Name));
			}
		}
	}
}
