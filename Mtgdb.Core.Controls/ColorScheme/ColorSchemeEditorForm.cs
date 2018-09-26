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
	public class ColorSchemeEditorForm : CustomBorderForm
	{
		public ColorSchemeEditorForm()
		{
			_panelClient.Controls.Add(_layoutPanel);
			ShowIcon = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Color scheme";
		}

		[UsedImplicitly]
		public ColorSchemeEditorForm(ColorSchemeController controller)
			: this()
		{
			_controller = controller;

			foreach (KnownColor colorName in _controller.KnownColors)
				_layoutPanel.Controls.Add(createColorCell(colorName));

			addButtons();

			Closing += (s, e) =>
			{
				Hide();
				ColorPicker?.Close();
				e.Cancel = true;
			};

			int approxRowsCount = (int) Math.Ceiling(Math.Sqrt(_layoutPanel.Controls.Count));

			Size = Border.MultiplyBy(new Size(2, 1))
				.Plus(new Size(0, CaptionHeight))
				.Plus(_layoutPanel.Margin.Size)
				.Plus(_cellMargin.Size.MultiplyBy(approxRowsCount))
				.Plus(_cellSize.MultiplyBy(approxRowsCount));

			LocationChanged += moved;
			SizeChanged += moved;
		}

		public void LoadCurrentColorScheme() =>
			load(currentColorsFile(), quiet: true);

		private void addButtons()
		{
			addButton("Save", (s, e) =>
			{
				if (save())
					saveTo(currentColorsFile());
			});

			addButton("Load", (s, e) =>
			{
				if (trySelectFile(out string selectedFile) && load(selectedFile, quiet: false))
					saveTo(currentColorsFile());

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
			addButton("Reset all", (s, e) =>
			{
				if (resetting)
					return;

				resetting = true;
				_controller.ResetAll();

				if (File.Exists(currentColorsFile()))
					File.Delete(currentColorsFile());

				resetting = false;
			});

			bool save()
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

				return saveTo(dlg.FileName);
			}

			bool saveTo(string file)
			{
				var saved = _controller.Save();
				string serialized = string.Join(Str.Endl, saved.Select(_ => string.Format(Format, (KnownColor) _.Key, _.Value)));
				try
				{
					File.WriteAllText(file, serialized);
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to write file: " + file + Str.Endl + ex.Message);
					return false;
				}
			}
		}

		private bool load(string file, bool quiet)
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

		private void moved(object s, EventArgs e)
		{
			if (ColorPicker != null)
				ColorPicker.Location = new Point(Right, Top).Plus(new Point(SystemInformation.SizingBorderWidth, SystemInformation.SizingBorderWidth));
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
						var picker = ColorPicker;

						picker.Color = XYZ.FromRGB(new RGB(control.BackColor));
						picker.Text = color.ToString();
						picker.SetTag(color);

						if (!picker.Visible)
							picker.Show();

						if (!picker.Focused)
							picker.Focus();

						break;

					case MouseButtons.Middle:
						_controller.Reset(color);
						break;
				}
			};

			return control;
		}

		private void addButton(string text, EventHandler clickHandler)
		{
			var control = new Label
			{
				AutoSize = false,
				Size = _cellSize,
				Margin = _cellMargin,
				BackColor = SystemColors.Control,
				TextAlign = ContentAlignment.MiddleCenter,
				BorderStyle = BorderStyle.Fixed3D,
				Text = text
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

		private ColorPicker createColorPicker()
		{
			var picker = new ColorPicker { StartPosition = FormStartPosition.Manual };

			picker.FormClosing += closing;

			void closing(object s, FormClosingEventArgs e)
			{
				_colorPicker = null;

				picker.FormClosing -= closing;

				if (picker.DialogResult != DialogResult.OK)
					return;

				var knownColor = picker.GetTag<KnownColor>();
				var backColor = picker.Color.ToRGB().ToArgb();
				_controller.SetColor(knownColor, backColor.ToArgb());
			}

			return picker;
		}



		private readonly FlowLayoutPanel _layoutPanel = new FlowLayoutPanel
		{
			Dock = DockStyle.Fill,
			Margin = new Padding(0)
		};

		private ColorPicker _colorPicker;
		private ColorPicker ColorPicker
		{
			get
			{
				if (_colorPicker == null)
				{
					_colorPicker = createColorPicker();
					moved(null, null);
				}

				return _colorPicker;
			}
		}

		public string SaveDirectory { get; set; }

		private const string Ext = ".colors";
		private const string Format = "{0}: 0x{1:x8}";
		private static readonly string _filter = $"Mtgdb.Gui color scheme (*{Ext})|*{Ext}";
		private static readonly Regex _pattern = new Regex(@"^(?<name>\w+): 0x(?<argb>[\da-f]{8})$", RegexOptions.IgnoreCase);

		private readonly Size _cellSize = new Size(128, 36).ByDpi();
		private readonly Padding _cellMargin = new Padding(3.ByDpiWidth());
		private readonly ColorSchemeController _controller;
	}
}