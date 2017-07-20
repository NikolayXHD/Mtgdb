using System;

namespace NConfiguration.Joining
{
	public class FindingSettingsArgs : EventArgs
	{
		public IConfigNodeProvider Source { get; private set; }
		public IncludeFileConfig IncludeFile { get; private set; }
		public string SearchPath { get; private set; }

		public FindingSettingsArgs(IConfigNodeProvider source, IncludeFileConfig cfg, string searchPath)
		{
			Source = source;
			IncludeFile = cfg;
			SearchPath = searchPath;
		}
	}
}

