using System;

namespace DedicatedEssentials
{
	public abstract class ServerDataHandlerBase
	{
		public virtual bool CanHandle(byte[] data)
		{
		    return GetDataId() == BitConverter.ToInt64( data, 0 );
		}

		public byte[] ProcessCommand(byte[] data)
		{
            long dataId = BitConverter.ToInt64(data, 0);
            byte[] newData = new byte[data.Length - sizeof(long)];
		    Array.Copy( data, sizeof (long), newData, 0, newData.Length );
			return newData;
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
