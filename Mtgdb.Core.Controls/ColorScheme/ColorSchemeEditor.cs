using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DrawingEx.ColorManagement;
using DrawingEx.ColorManagement.ColorModels;
using JetBrains.Annotations;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class ColorSchemeEditor : CustomBorderForm
	{
		public ColorSchemeEditor()
		{
			_panelClient.Controls.Add(_layoutPanel);
			ShowIcon = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Color scheme editor";
		}

		[UsedImplicitly]
		public ColorSchemeEditor(ColorSchemeController controller)
			: this()
		{
			_controller = controller;

			foreach (KnownColor colorName in _controller.KnownColors)
				_layoutPanel.Controls.Add(createColorCell(colorName));

			addButtons();

			Closing += (s, e) =>
			{
				Hide();
				_colorPicker.Hide();
				e.Cancel = true;
			};

			int columnsCount = (int) Math.Ceiling(Math.Sqrt(_layoutPanel.Controls.Count));
			int rowsCount = (int) Math.Ceiling((float) _layoutPanel.Controls.Count / columnsCount);

			var cellCount = new Size(columnsCount, rowsCount);

			Size = Border.MultiplyBy(new Size(2, 1))
				.Plus(new Size(0, CaptionHeight))
				.Plus(_layoutPanel.Margin.Size)
				.Plus(_cellMargin.Size.MultiplyBy(cellCount))
				.Plus(_cellSize.MultiplyBy(cellCount));

			_colorPicker = createColorPicker();

			LocationChanged += moved;
			SizeChanged += moved;
		}

		private Control createColorCell(KnownColor color)
		{
			var control = new Label
			{
				AutoSize = false,
				Size = _cellSize,
				Margin = _cellMargin,
				BackColor = Color.FromKnownColor(color),
				TextAlign = ContentAlignment.MiddleCenter,
				BorderStyle = BorderStyle.Fixed3D
			};

			updateText();
			ColorSchemeController.SystemColorsChanging += updateText;

			void updateText()
			{
				var result = new StringBuilder();
				result.Append(color);

				if (_controller.GetOriginalColor(color) != _controller.GetColor(color))
				{
					result.AppendLine("*");
					result.Append("middle-click to reset");
				}

				control.Text = result.ToString();
			}

			bindForeColor(control);

			control.SetTag(color);

			control.MouseUp += (t, te) =>
			{
				switch (te.Button)
				{
					case MouseButtons.Left:
						_colorPicker.DialogResult = DialogResult.None;
						_colorPicker.Color = XYZ.FromRGB(new RGB(control.BackColor));
						_colorPicker.Text = color.ToString();
						_colorPicker.SetTag(color);
						moved(null, null);

						if (!_colorPicker.Visible)
							_colorPicker.Show();

						if (!_colorPicker.Focused)
							_colorPicker.Focus();

						break;

					case MouseButtons.Middle:
						_controller.Reset(color);
						saveCurrentColorScheme();
						break;
				}
			};

			return control;
		}

		private ColorPicker createColorPicker()
		{
			var picker = new ColorPicker
			{
				StartPosition = FormStartPosition.Manual
			};

			picker.FormClosing += closing;

			void closing(object s, FormClosingEventArgs e)
			{
				if (picker.DialogResult != DialogResult.OK)
					return;

				var knownColor = picker.GetTag<KnownColor>();
				var backColor = picker.Color.ToRGB().ToArgb();
				_controller.SetColor(knownColor, backColor.ToArgb());
				saveCurrentColorScheme();
			}

			return picker;
		}

		private void addButtons()
		{
			createButton("Save", (s, e) =>
			{
				if (saveDialog())
					saveCurrentColorScheme();
			});

			createButton("Load", (s, e) =>
			{
				if (trySelectFile(out string selectedFile) && load(selectedFile))
					saveCurrentColorScheme();

				bool trySelectFile(out string result)
				{
					result = null;

					var dlg = new OpenFileDialog
					{
						DefaultExt = Ext,
						InitialDirectory = SaveDirectory,
						AddExtension = true,
						Filter = _filter,
						Title = "Load color scheme",
						CheckFileExists = true
					};

					if (dlg.ShowDialog() != DialogResult.OK)
						return false;

					result = dlg.FileName;
					return true;
				}
			});

			bool resetting = false;
			createButton("Reset all", (s, e) =>
			{
				if (resetting)
					return;

				resetting = true;
				_controller.ResetAll();
				saveCurrentColorScheme();
				resetting = false;
			});

			bool saveDialog()
			{
				var dlg = new SaveFileDialog
				{
					DefaultExt = Ext,
					InitialDirectory = SaveDirectory,
					AddExtension = true,
					Filter = _filter,
					Title = "Save color scheme",
					CheckPathExists = true
				};

				if (dlg.ShowDialog() != DialogResult.OK)
					return false;

				return save(_controller.Save(), dlg.FileName);
			}
		}

		private void createButton(string text, EventHandler clickHandler)
		{
			var deltaSize = _cellSize.MultiplyBy(0.1125f).Round();

			var control = new Label
			{
				AutoSize = false,
				Size = _cellSize.Minus(deltaSize),
				Margin = _cellMargin.Add(deltaSize),
				BackColor = SystemColors.Control,
				ForeColor = SystemColors.ControlText,
				TextAlign = ContentAlignment.MiddleCenter,
				BorderStyle = BorderStyle.Fixed3D,
				Text = text,
				Font = new Font(Font.FontFamily, Font.Size * 9f / 8f, FontStyle.Bold, Font.Unit)
			};

			bindForeColor(control);
			control.Click += clickHandler;
			_layoutPanel.Controls.Add(control);
		}

		private static void bindForeColor(Control c)
		{
			selectForeColor();
			ColorSchemeController.SystemColorsChanging += selectForeColor;

			void selectForeColor()
			{
				c.ForeColor = c.BackColor.TransformHsv(v: _ => (1f - (float) Math.Round(_)).WithinRange(0.3f, 0.7f));
				c.Invalidate();
			}
		}



		private void saveCurrentColorScheme()
		{
			string file = currentColorsFile();
			var values = _controller.Save();

			if (values.Count == 0)
			{
				if (File.Exists(file))
					File.Delete(file);
			}
			else
				save(values, file, quiet: true);
		}

		public void LoadCurrentColorScheme() =>
			load(currentColorsFile(), quiet: true);

		private static bool save(IReadOnlyDictionary<int, int> values, string file, bool quiet = false)
		{
			string serialized = string.Join(Str.Endl, values.Select(_ => string.Format(Format, (KnownColor) _.Key, _.Value)));
			try
			{
				File.WriteAllText(file, serialized);
				return true;
			}
			catch (Exception ex)
			{
				if (!quiet)
					MessageBox.Show("Failed to write file: " + file + Str.Endl + ex.Message);

				return false;
			}
		}

		private bool load(string file, bool quiet = false)
		{
			if (tryReadFile(file, out string serialized) && tryDeserialize(serialized, out var deserialized))
			{
				_controller.Load(deserialized);
				return true;
			}

			return false;

			bool tryReadFile(string fileName, out string result)
			{
				result = null;
				var fileInfo = new FileInfo(fileName);

				if (!fileInfo.Exists)
				{
					warnOnInvalidContent("File not found");
					return false;
				}

				if (fileInfo.Length > 1024 * 1024)
				{
					warnOnInvalidContent("File too large");
					return false;
				}

				try
				{
					result = File.ReadAllText(fileName);
					return true;
				}
				catch (Exception ex)
				{
					warnOnInvalidContent(ex.Message);
					return false;
				}
			}

			bool tryDeserialize(string input, out IReadOnlyDictionary<int, int> result)
			{
				result = null;
				var lines = input.Split(Array.From(Str.Endl), StringSplitOptions.None);
				var map = new Dictionary<int, int>();

				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					var match = _pattern.Match(line);
					if (!match.Success)
					{
						warnOnInvalidContent($"Invalid format of line {i + 1}: {line}");
						return false;
					}

					string name = match.Groups["name"].Value;
					string rgbaStr = match.Groups["argb"].Value;

					if (!Enum.TryParse(name, out KnownColor colorName) || !_controller.KnownColors.Contains(colorName))
					{
						warnOnInvalidContent($"Invalid color name at line {i + 1}: {name}");
						return false;
					}

					if (!int.TryParse(rgbaStr, NumberStyles.HexNumber, Str.Culture, out int rgba))
					{
						warnOnInvalidContent($"Invalid color value at line {i + 1}: {rgbaStr}");
						return false;
					}

					map.Add((int) colorName, rgba);
				}

				result = map.AsReadOnlyDictionary();
				return true;
			}

			void warnOnInvalidContent(string message)
			{
				if (!quiet)
					MessageBox.Show("Failed to read color scheme from file: " + file + Str.Endl + message);
			}
		}

		private string currentColorsFile() =>
			SaveDirectory.AddPath("current" + Ext);



		private void moved(object s, EventArgs e) =>
			_colorPicker.Location =
				new Point(Right, Top).Plus(
					new Point(
						SystemInformation.SizingBorderWidth,
						SystemInformation.SizingBorderWidth));

		private readonly FlowLayoutPanel _layoutPanel = new FlowLayoutPanel
		{
			Dock = DockStyle.Fill,
			Margin = new Padding(0)
		};

		public string SaveDirectory { get; set; }

		private const string Ext = ".colors";
		private const string Format = "{0}: {1:x8}";
		private static readonly string _filter = $"Mtgdb.Gui color scheme (*{Ext})|*{Ext}";
		private static readonly Regex _pattern = new Regex(@"^(?<name>\w+): (?<argb>[\da-f]{8})$", RegexOptions.IgnoreCase);

		private readonly Size _cellSize = new Size(128, 36).ByDpi();
		private readonly Padding _cellMargin = new Padding(3.ByDpiWidth());
		private readonly ColorSchemeController _controller;
		private readonly ColorPicker _colorPicker;
	}
}