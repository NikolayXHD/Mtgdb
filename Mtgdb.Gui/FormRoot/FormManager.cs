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

			form.Show();
			form.AddTab();
		}

		public int GetId(FormRoot form)
		{
			int result = _instances.IndexOf(form);

			if (result == -1)
				return _instances.Count;

			return result;
		}

		public void Add(FormRoot form)
		{
			_instances.Add(form);
		}

		public void Remove(FormRoot form)
		{
			_instances.Remove(form);

			if (_instances.Count == 0)
				ExitThread();
		}

		public void MoveToEnd(FormRoot form)
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

		public void SwapTabs(int formId1, int tabId1, int formId2, int tabId2)
		{
			var tempDir = AppDir.History.AddPath("temp");

			var file1 = AppDir.History.AddPath(formId1.ToString()).AddPath(tabId1 + ".json");
			var file2 = AppDir.History.AddPath(formId2.ToString()).AddPath(tabId2 + ".json");

			var tempFile = Path.Combine(tempDir, Path.GetFileName(file1));

			if (Directory.Exists(tempDir))
				Directory.Delete(tempDir, recursive: true);

			Directory.CreateDirectory(tempDir);

			if (File.Exists(file1))
				File.Move(file1, tempFile);

			if (File.Exists(file2))
				File.Move(file2, file1);

			if (File.Exists(tempFile))
				File.Move(tempFile, file2);
		}

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