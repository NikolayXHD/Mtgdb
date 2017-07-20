using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NConfiguration.Serialization;

namespace NConfiguration.Monitoring
{
	public abstract class FileChecker: IChangeable, IDisposable
	{
		internal static readonly string ConfigSectionName = "WatchFile";

		public static FileChecker TryCreate(ReadedFileInfo fileInfo, WatchMode watch, TimeSpan? delay, CheckMode check)
		{
			switch (watch)
			{
				case WatchMode.None:
					return null;

				case WatchMode.Time:
					if (delay == null)
						throw new ArgumentNullException("delay");
					return new PeriodicFileChecker(fileInfo, delay.Value, check);

				case WatchMode.Auto:
					try
					{
						return new WatchedFileChecker(fileInfo, delay, check);
					}
					catch (Exception)
					{
						if (delay == null)
							throw;
					}
					return new PeriodicFileChecker(fileInfo, delay.Value, check);

				case WatchMode.System:
					return new WatchedFileChecker(fileInfo, delay, check);

				default:
					throw new ArgumentOutOfRangeException("unexpected mode");
			}
		}

		public static IEnumerable<FileChecker> TryCreate(IEnumerable<IConfigNodeProvider> providers)
		{
			return TryCreate(providers.OfType<ILoadedFromFile>());
		}

		public static IEnumerable<FileChecker> TryCreate(IEnumerable<ILoadedFromFile> fileInfoOwners)
		{
			return fileInfoOwners.Select(TryCreate).Where(_ => _ != null);
		}

		public static FileChecker TryCreate(ILoadedFromFile loadedFromFile)
		{
			return TryCreate(loadedFromFile, loadedFromFile.FileInfo);
		}

		public static FileChecker TryCreate(IConfigNodeProvider nodeProvider, ReadedFileInfo fileInfo)
		{
			var node = nodeProvider.ByName(ConfigSectionName).FirstOrDefault();
			if (node == null)
				return null;

			var cfg = DefaultDeserializer.Instance.Deserialize<WatchFileConfig>(node);
			return TryCreate(fileInfo, cfg.Mode, cfg.Delay, cfg.Check.GetValueOrDefault(CheckMode.All));
		}

		private readonly ReadedFileInfo _fileInfo;

		protected FileChecker(ReadedFileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public string WatchedFile {
			get
			{
				return _fileInfo.FullName;
			}
		}

		protected bool checkFile(CheckMode check)
		{
			lock(_sync)
				if (_disposed)
					return false;

			if (_fileInfo.WasChanged(check.HasFlag(CheckMode.Attr)))
				return true;

			if (check.HasFlag(CheckMode.Hash))
			{
				if (_fileInfo.WasHashChanged())
					return true;
			}

			return false;
		}

		protected virtual void onChanged()
		{
			EventHandler copy;
			lock (_sync)
			{
				if (_changed)
					return;

				if (_disposed)
					return;

				_changed = true;
				copy = _changedHandler;
				_changedHandler = null;
			}

			if (copy != null)
				copy(this, EventArgs.Empty);
		}

		private EventHandler _changedHandler = null;

		public event EventHandler Changed
		{
			add
			{
				lock (_sync)
				{
					if (_disposed)
						return;

					if (_changed)
						ThreadPool.QueueUserWorkItem(asyncChangedWork, value);
					else
						_changedHandler += value;
				}
			}
			remove
			{
				lock (_sync)
				{
					if (_disposed)
						return;

					if (!_changed)
						_changedHandler -= value;
				}
			}
		}

		private void asyncChangedWork(object arg)
		{
			((EventHandler)arg)(this, EventArgs.Empty);
		}

		private readonly object _sync = new object();
		private bool _changed = false;
		private bool _disposed = false;

		public virtual void Dispose()
		{
			lock (_sync)
			{
				if (_disposed)
					return;

				_changedHandler = null;
				_disposed = true;
			}
		}
	}
}
