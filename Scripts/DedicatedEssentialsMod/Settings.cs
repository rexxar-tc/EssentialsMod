using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI;
using System.IO;


namespace DedicatedEssentials
{
	public class Settings
	{
		private static Settings m_instance = null;

		public static Settings Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new Settings();

				return m_instance;
			}
		}

		public void Load()
		{
			if (MyAPIGateway.Utilities == null)
				return;

			try
			{
				using (TextReader reader = MyAPIGateway.Utilities.ReadFileInLocalStorage("Settings.txt", typeof(Settings)))
				{
					// Do nothing right now
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(String.Format("Load(): {0}", ex.ToString()));
			}
		}

		public void Save()
		{
			if (MyAPIGateway.Utilities == null)
				return;

			try
			{
				using (TextWriter writer = MyAPIGateway.Utilities.WriteFileInLocalStorage("Settings.txt", typeof(Settings)))
				{
					// Nope
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(String.Format("Save(): {0}", ex.ToString()));
			}
		}
	}
}
