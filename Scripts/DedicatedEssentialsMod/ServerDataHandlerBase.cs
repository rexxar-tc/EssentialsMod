using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedEssentials
{
	public abstract class ServerDataHandlerBase
	{
		public virtual Boolean CanHandle(byte[] data)
		{
			long dataId = DecodeData(data);
			if(GetDataId() == dataId)
				return true;
			
			return false;
		}

		public byte[] ProcessCommand(byte[] data)
		{
			byte length = data[0];
			byte[] newData = new byte[data.Length - 1 - length];
			Array.Copy(data, length + 1, newData, 0, newData.Length);
			return newData;
		}

		protected byte[] EncodeData(long data)
		{
			string convert = data.ToString();
			byte[] result = new byte[convert.Length + 1];
			result[0] = (byte)convert.Length;
			for (int r = 1; r < convert.Length; r++)
				result[r] = (byte)convert[r - 1];
			return result;
		}

		protected long DecodeData(byte[] data)
		{
			byte length = data[0];
			string convert = "";

			for (int r = 1; r < length + 1; r++)
				convert += (char)data[r];

			return long.Parse(convert);
		}

		public virtual long GetDataId()
		{
			return 0;
		}

		public virtual void HandleCommand(byte[] data)
		{

		}
	}
}
