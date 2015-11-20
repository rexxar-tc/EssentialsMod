using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
