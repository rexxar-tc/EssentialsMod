using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;

namespace DedicatedEssentials
{
	class Utility
	{
		public static string[] SplitString(string data)
		{
			var result = data.Split('"').Select((element, index) => index % 2 == 0  // If even index
												 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
												 : new string[] { element })  // Keep the entire item					
												 .SelectMany(element => element).ToList();

			return result.ToArray();
		}

	    public static bool CompareBytes(byte[] byteA, byte[] byteB)
	    {
	        if (byteA.Length != byteB.Length)
	            return false;

	        for (int i = 0; i < byteA.Length; ++i)
	        {
	            if (byteA[i] != byteB[i])
	                return true;
	        }

	        return false;
	    }

	    public static IMyGps ParseGps( string message )
	    {
            //copied from SE because MyGpsCollection is private
            foreach (Match match in Regex.Matches(message, @"GPS:([^:]{0,32}):([\d\.-]*):([\d\.-]*):([\d\.-]*):"))
            {
                string name = match.Groups[1].Value;
                double x, y, z;
                try
                {
                    x = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    x = Math.Round(x, 2);
                    y = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    y = Math.Round(y, 2);
                    z = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                    z = Math.Round(z, 2);
                }
                catch (Exception)
                {
                    return null;
                }
                return MyAPIGateway.Session.GPS.Create( name, "", new Vector3D( x, y, z ), false, true );
            }
	        return null;
	    }
	}
}
