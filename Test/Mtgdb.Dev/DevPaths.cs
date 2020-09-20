using System;
using System.Collections.Generic;

namespace Mtgdb.Dev
{
	public static class DevPaths
	{
		private static readonly Dictionary<bool, FsPath> _windowsDrive = new Dictionary<bool, FsPath>
		{
			[false] = new FsPath("C:\\"),
			[true] = new FsPath("/", "home", "kolia", "win10")
		};

		private static readonly Dictionary<bool, FsPath> _dataDrive = new Dictionary<bool, FsPath>
		{
			[false] = new FsPath("D:\\"),
			[true] = new FsPath("/", "home", "kolia", "data")
		};

		private static readonly Dictionary<bool, FsPath> _repoDrive = new Dictionary<bool, FsPath>
		{
			[false] = new FsPath("C:\\"),
			[true] = new FsPath("/", "home", "kolia")
		};

		public static readonly FsPath WindowsDrive = _windowsDrive[Runtime.IsLinux];
		public static readonly FsPath DataDrive = _dataDrive[Runtime.IsLinux];
		public static readonly FsPath RepoDrive = _repoDrive[Runtime.IsLinux];

		private static readonly Dictionary<bool, FsPath>[] _knownDrives =
		{
			_windowsDrive,
			_dataDrive,
			_repoDrive
		};

		public static readonly FsPath ReleaseNotesFile =
			RepoDrive.Join("git", "mtgdb.wiki", "Release-notes.rest");

		public static readonly FsPath NotificationsRepo =
			RepoDrive.Join("git", "mtgdb.notifications");

		public static readonly Func<FsPath, FsPath> DriveSubstitution = path =>
		{
			if (!Runtime.IsLinux)
				return path;

			foreach (var alternatives in _knownDrives)
			{
				var relative = path.RelativeTo(alternatives[false], StringComparison.OrdinalIgnoreCase);
				if (relative != FsPath.None)
					return alternatives[true].Join(relative);
			}

			return path;
		};

		public static readonly FsPath MtgContentDir = DataDrive.Join("distrib", "games", "mtg");

		public static readonly FsPath XlhqDir = MtgContentDir.Join("Mega", "XLHQ");
		public static readonly FsPath TorrentsDir = MtgContentDir.Join("XLHQ-Sets-Torrent.Unpacked");

		public static readonly FsPath GathererOriginalDir = MtgContentDir.Join("Gatherer.Original");
		public static readonly FsPath GathererPreprocessedDir = MtgContentDir.Join("Gatherer.PreProcessed");

		public static readonly FsPath GathererOriginalCardsDir = GathererOriginalDir.Join("cards");
		public static readonly FsPath GathererPreprocessedCardsDir = GathererPreprocessedDir.Join("cards");
	}
}
