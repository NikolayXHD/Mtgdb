using System;

namespace NConfiguration.Joining
{
	public class LoadedEventArgs : EventArgs
	{
		public IIdentifiedSource Settings { get; private set; }

		public LoadedEventArgs(IIdentifiedSource settings)
		{
			Settings = settings;
		}
	}
}

