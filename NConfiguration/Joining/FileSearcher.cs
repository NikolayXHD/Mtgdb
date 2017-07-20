using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NConfiguration.Joining
{
	public sealed class FileSearcher: IIncludeHandler<IncludeFileConfig>
	{
		private readonly Func<string, IIdentifiedSource> _creater;
		private readonly HashSet<string> _validExtensions;

		public FileSearcher(Func<string, IIdentifiedSource> creater, params string[] validExtensions)
		{
			_creater = creater;
			_validExtensions = new HashSet<string>(validExtensions.Select(_ => "." + _), NameComparer.Instance);
		}

		public event EventHandler<FindingSettingsArgs> FindingSettings;

		private void onFindingSettings(IConfigNodeProvider source, IncludeFileConfig cfg, string searchPath)
		{
			var copy = FindingSettings;
			if (copy != null)
				copy(this, new FindingSettingsArgs(source, cfg, searchPath));
		}

		public IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, IncludeFileConfig cfg, string searchPath)
		{
			var filePath = cfg.Path;

			if (_validExtensions.Count != 0 && !_validExtensions.Contains(Path.GetExtension(filePath)))
				yield break;

			if (Path.IsPathRooted(filePath))
			{
				onFindingSettings(owner, cfg, null);

				if (!File.Exists(filePath) && !cfg.Required)
					yield break;

				yield return _creater(filePath);
				yield break;
			}

			var basePath = selectBasePath(owner as IFilePathOwner, searchPath, ref filePath);

			onFindingSettings(owner, cfg, basePath);
			var found = searchSettings(basePath, filePath, cfg.Search);

			if (found.Count == 0)
			{
				if (cfg.Required)
					throw new ApplicationException(string.Format("configuration file '{0}' not found in '{1}'", filePath, basePath));

				yield break;
			}

			if (cfg.Include == IncludeMode.First)
				yield return found.First();
			else if (cfg.Include == IncludeMode.Last)
				yield return found.Last();
			else
				foreach (var item in found)
					yield return item;
		}

		private static string selectBasePath(IFilePathOwner owner, string searchPath, ref string filePath)
		{
			if (filePath[0] == '~')
				filePath = filePath.Substring(2); // remove ~ and path separator
			else if (owner != null)
				return owner.Path;

			if (searchPath == null || !Path.IsPathRooted(searchPath))
				throw new InvalidOperationException(
					string.Format("path '{0}' required rooted search path. But was define '{1}'", filePath, searchPath));

			return searchPath;
		}

		private List<IIdentifiedSource> searchSettings(string basePath, string fileName, SearchMode mode)
		{
			var result = new List<IIdentifiedSource>();

			if(basePath == null)
				return result;

			if (mode == SearchMode.Up)
				basePath = Path.GetDirectoryName(basePath);

			if(basePath == null)
				return result;

			while(true)
			{
				try
				{
					if (!Directory.Exists(basePath))
						break;
					var fullPath = Path.Combine(basePath, fileName);

					if (File.Exists(fullPath))
					{
						var item = _creater(fullPath);
						result.Add(item);
					}

					if (mode == SearchMode.Exact)
						break;

					basePath = Path.GetDirectoryName(basePath);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
			}

			return result;
		}
	}
}

