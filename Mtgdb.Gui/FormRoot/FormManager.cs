using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Mtgdb.Gui
{
	public class FormManager : ApplicationContext
	{
		public FormManager(Func<FormRoot> formFactory)
		{
			_formFactory = formFactory;
		}

		public void CreateForm()
		{
			var form = _formFactory();

			form.Show();
			form.NewTab(onCreated: null);
		}

		public int GetId(FormRoot form)
		{
			return _instances.IndexOf(form);
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

		private readonly List<FormRoot> _instances = new List<FormRoot>();
		private readonly Func<FormRoot> _formFactory;
	}
}