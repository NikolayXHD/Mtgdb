using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Data;
using NLog;

namespace Mtgdb.Gui
{
	public class App : ApplicationContext, IApplication
	{
		[UsedImplicitly]
		public App(Func<FormRoot> formFactory)
		{
			_formFactory = formFactory;
		}

		public void MigrateHistoryFiles()
		{
			FsPath firstFormDirectory = AppDir.History.Join(0.ToString());

			if (firstFormDirectory.IsDirectory())
				return;

			firstFormDirectory.CreateDirectory();

			foreach (FsPath file in AppDir.History.EnumerateFiles())
				file.CopyFileTo(firstFormDirectory.Join(file.Basename()));
		}

		public void StartForm()
		{
			_log.Info($"{nameof(StartForm)}");

			var form = _formFactory();

			form.AddTab();
			form.Show();

			_instances.Add(form);
		}

		public int GetId(FormRoot form)
		{
			int result = _instances.IndexOf(form);

			if (result == -1)
				return _instances.Count;

			return result;
		}

		public void StopForm(FormRoot form)
		{
			_log.Info($"{nameof(StopForm)}");

			_instances.Remove(form);

			if (_instances.Count == 0)
				ExitThread();
		}

		public void MoveFormHistoryToEnd(FormRoot form)
		{
			var idBeforeClosing = GetId(form);

			FsPath tempDir = AppDir.History.Join("temp");

			if (tempDir.IsDirectory())
				tempDir.DeleteDirectory(recursive: true);

			AppDir.History.Join(idBeforeClosing.ToString()).MoveDirectoryTo(tempDir);

			int lastId = _instances.Count - 1;

			for (int i = idBeforeClosing + 1; i <= lastId; i++)
				AppDir.History.Join(i.ToString()).MoveDirectoryTo(AppDir.History.Join((i - 1).ToString()));

			tempDir.MoveDirectoryTo(AppDir.History.Join(lastId.ToString()));

			_instances.RemoveAt(idBeforeClosing);
			_instances.Add(form);
		}

		public void MoveTabHistory(int toFormId, int toTabId, int fromFormId, int fromTabId)
		{
			var fromFile = GetHistoryFile(fromFormId, fromTabId);

			var toFile = GetHistoryFile(toFormId, toTabId);

			var toTabIds = getSavedTabIds(toFormId)
				.Where(tabId => tabId >= toTabId)
				.OrderByDescending(tabId => tabId)
				.ToList();

			foreach (int tabId in toTabIds)
			{
				GetHistoryFile(toFormId, tabId).MoveFileTo(
					GetHistoryFile(toFormId, tabId + 1));
			}

			if (fromFile.IsFile())
				fromFile.MoveFileTo(toFile);

			var fromTabIds = getSavedTabIds(fromFormId)
				.Where(tabId => tabId > fromTabId)
				.OrderBy(tabId => tabId)
				.ToList();

			foreach (int tabId in fromTabIds)
			{
				GetHistoryFile(fromFormId, tabId).MoveFileTo(
					GetHistoryFile(fromFormId, tabId - 1));
			}
		}

		private static IEnumerable<int> getSavedTabIds(int formId)
		{
			return getHistoryDirectory(formId).EnumerateFiles()
				.Where(f => Str.Equals(f.Extension(), ".json"))
				.Select(f => f.Basename(extension: false))
				.Where(n => n.All(c => '0' <= c && c <= '9'))
				.Select(int.Parse);
		}

		public static FsPath GetHistoryFile(int formId, int tabId) => AppDir.History.Join(formId.ToString(), $"{tabId}.v4.json");

		private static FsPath getHistoryDirectory(int formId) => AppDir.History.Join(formId.ToString());

		public FormMain FindCardDraggingForm()
		{
			return _instances
				.SelectMany(_ => _.Tabs)
				.FirstOrDefault(_ => _.IsDraggingCard);
		}

		public IEnumerable<FormRoot> Forms => _instances;

		public void CancelAllTasks() => _cts.Cancel();

		public CancellationToken CancellationToken => _cts.Token;

		private readonly List<FormRoot> _instances = new List<FormRoot>();
		private readonly Func<FormRoot> _formFactory;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
