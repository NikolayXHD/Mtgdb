using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Gui
{
	public class FormManager : ApplicationContext
	{
		public FormManager(Func<FormRoot> formFactory)
		{
			_formFactory = formFactory;
		}

		public void MigrateHistoryFiles()
		{
			string firstFormDirectory = AppDir.History.AddPath(0.ToString());

			if (Directory.Exists(firstFormDirectory))
				return;

			Directory.CreateDirectory(firstFormDirectory);

			foreach (var file in Directory.GetFiles(AppDir.History))
				File.Copy(file, firstFormDirectory.AddPath(Path.GetFileName(file)));
		}

		public void CreateForm()
		{
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

		public void Remove(FormRoot form)
		{
			_instances.Remove(form);

			if (_instances.Count == 0)
				ExitThread();
		}

		public void MoveFormHistoryToEnd(FormRoot form)
		{
			var idBeforeClosing = GetId(form);

			var tempDir = AppDir.History.AddPath("temp");

			if (Directory.Exists(tempDir))
				Directory.Delete(tempDir, recursive: true);

			Directory.Move(AppDir.History.AddPath(idBeforeClosing.ToString()), tempDir);

			int lastId = _instances.Count - 1;

			for (int i = idBeforeClosing + 1; i <= lastId; i++)
				Directory.Move(AppDir.History.AddPath(i.ToString()), AppDir.History.AddPath((i - 1).ToString()));

			Directory.Move(tempDir, AppDir.History.AddPath(lastId.ToString()));

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
				File.Move(
					GetHistoryFile(toFormId, tabId), 
					GetHistoryFile(toFormId, tabId + 1));
			}

			if (File.Exists(fromFile))
				File.Move(fromFile, toFile);

			var fromTabIds = getSavedTabIds(fromFormId)
				.Where(tabId => tabId > fromTabId)
				.OrderBy(tabId => tabId)
				.ToList();

			foreach (int tabId in fromTabIds)
			{
				File.Move(
					GetHistoryFile(fromFormId, tabId), 
					GetHistoryFile(fromFormId, tabId - 1));
			}
		}

		private IEnumerable<int> getSavedTabIds(int formId)
		{
			return Directory.GetFiles(GetHistoryDirectory(formId))
				.Where(f => Str.Equals(Path.GetExtension(f), ".json"))
				.Select(Path.GetFileNameWithoutExtension)
				.Where(n => n.All(c => '0' <= c && c <= '9'))
				.Select(int.Parse);
		}

		public string GetHistoryFile(int formId, int tabId) => AppDir.History.AddPath($"{formId}\\{tabId}.json");
		public string GetHistoryDirectory(int formId) => AppDir.History.AddPath($"{formId}");

		public FormMain FindCardDraggingForm()
		{
			return _instances
				.SelectMany(_ => _.Tabs)
				.FirstOrDefault(_ => _.IsDraggingCard);
		}

		public IEnumerable<FormRoot> Forms => _instances;

		private readonly List<FormRoot> _instances = new List<FormRoot>();
		private readonly Func<FormRoot> _formFactory;
	}
}