using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public sealed partial class FormZoom : Form
	{
		[UsedImplicitly] // by WinForms designer
		public FormZoom()
		{
			InitializeComponent();
			DoubleBuffered = true;
			_contextMenu.ForeColor = SystemColors.ControlText;
			_contextMenu.BackColor = SystemColors.Control;
		}

		public FormZoom(
			CardRepository cardRepository,
			ImageRepository imageRepository,
			ImageLoader imageLoader)
			: this()
		{
			_cardRepository = cardRepository;
			_imageRepository = imageRepository;
			_imageLoader = imageLoader;

			BackgroundImageLayout = ImageLayout.Zoom;
			TransparencyKey = BackColor = _defaultBgColor;

			_ctsLifetime = new CancellationTokenSource();

			_pictureBox.MouseClick += click;
			MouseWheel += mouseWheel;

			scale();

			_showArtButton.CheckedChanged += showArtChanged;
			_showDuplicatesButton.CheckedChanged += (x, y) => onSettingsChanged();
			_showOtherSetsButton.CheckedChanged += (x, y) => onSettingsChanged();

			updateShowArt();

			Disposed += handleDisposed;
		}

		private void scale()
		{
			new DpiScaler<FormZoom>(form =>
			{
				var hotSpot = new Point(14, 8).ByDpi();
				var cursorImage = Runtime.IsMono
					? Dpi.ScalePercent > 100
						? Resources.rightclick_48_bw.HalfResizeDpi()
						: Resources.rightclick_24_bw.ResizeDpi()
					: Resources.rightclick_48.HalfResizeDpi();

				form.Cursor = CursorHelper.CreateCursor(cursorImage, hotSpot);

				bool useLargeIcon = Dpi.ScalePercent > 100;

				form._openFileButton.Image = useLargeIcon
					? Resources.image_file_32.HalfResizeDpi()
					: Resources.image_file_16.ResizeDpi();

				form._showInExplorerButton.Image = useLargeIcon
					? Resources.open_32.HalfResizeDpi()
					: Resources.open_16.ResizeDpi();

				form._showArtButton.Image = useLargeIcon
					? Resources.art_64.HalfResizeDpi()
					: Resources.art_32.ResizeDpi();

				var cloneImg = Resources.clone_48.HalfResizeDpi();

				form._showDuplicatesButton.Image = cloneImg;
				form._showOtherSetsButton.Image = cloneImg;
			}).Setup(this);
		}

		public async Task LoadImages(Card card, UiModel ui)
		{
			_location = Cursor.Position;
			await runLoadImagesTask(card, ui, _ctsLifetime.Token);
		}

		private async Task runLoadImagesTask(Card card, UiModel ui, CancellationToken token)
		{
			_card = card;
			_ui = ui;

			_ctsLoadingImages?.Cancel();

			_cardForms = card.Faces
				.OrderByDescending(face => face.NameNormalized == card.NameNormalized)
				.Select(face => _cardRepository.MapByName(card.IsToken)[face.NameNormalized]
					.AtMax(c => c.HasImage(ui))
					.ThenAtMax(c => c.Set == card.Set)
					.ThenAtMax(c => c == card)
					.Find())
				.Where(c => c.HasImage(ui))
				.ToList();

			foreach (var oldImg in _images)
				oldImg.Dispose();

			_images.Clear();
			_models.Clear();
			_imageIndex = 0;

			var loadingCancellation = new CancellationTokenSource();
			var loadingCancellationLinked = CancellationTokenSource.CreateLinkedTokenSource(loadingCancellation.Token, token);

			var waitingCancellation = new CancellationTokenSource();
			var waitingCancellationLinked = CancellationTokenSource.CreateLinkedTokenSource(waitingCancellation.Token, token);

#pragma warning disable 4014
			loadingCancellationLinked.Token.Run(async tkn =>
#pragma warning restore 4014
			{
				await loadImages(tkn);
				waitingCancellation.Cancel();
			});

			await anyImageLoaded(waitingCancellationLinked.Token).CatchCanceled();

			_ctsLoadingImages = loadingCancellation;
		}

		private async Task anyImageLoaded(CancellationToken cancellation)
		{
			while (_images.Count == 0 && !cancellation.IsCancellationRequested)
				await Task.Delay(50, cancellation);
		}

		private void showArtChanged(object sender, EventArgs e)
		{
			updateShowArt();

			if (_card != null)
			{
				_ctsLifetime.Token.Run(async token =>
				{
					await runLoadImagesTask(_card, _ui, token);

					this.Invoke(delegate
					{
						updateImage();
						applyZoom();
					});
				});
			}

			onSettingsChanged();
		}

		private void updateShowArt() =>
			_showArt = _showArtButton.Checked;

		public void ShowImages()
		{
			updateImage();
			applyZoom();
			Show();
			BringToFront();
		}


		private async Task loadImages(CancellationToken token)
		{
			bool repoLoadingComplete =
				_imageRepository.IsLoadingArtComplete.Signaled &&
				_imageRepository.IsLoadingZoomComplete.Signaled;

			await load(token);

			if (repoLoadingComplete)
				return;

			await Task.WhenAll(
				_imageRepository.IsLoadingArtComplete.Wait(_ctsLifetime.Token),
				_imageRepository.IsLoadingZoomComplete.Wait(_ctsLifetime.Token));

			await load(token);
		}

		private async Task load(CancellationToken token)
		{
			int index = 0;
			for (int j = 0; j < _cardForms.Count; j++)
			{
				if (token.IsCancellationRequested)
					return;

				if (_showArt)
					foreach (var model in _ui.GetImagesArt(_cardForms[j]) ?? Empty<ImageModel>.Sequence)
					{
						while (index > _imageIndex + 10 && !token.IsCancellationRequested)
							await Task.Delay(100, token);

						if (token.IsCancellationRequested)
							return;

						var size = model.ImageFile.IsArt
							? getSizeArt()
							: _imageLoader.ZoomedCardSize;

						var image = _imageLoader.LoadImage(model, size);

						if (image == null)
							continue;

						add(index, model, image);
						index++;
					}

				foreach (var model in _ui.GetZoomImages(_cardForms[j]) ?? Empty<ImageModel>.Sequence)
				{
					while (index > _imageIndex + 10 && !token.IsCancellationRequested)
						await Task.Delay(100, token);

					if (token.IsCancellationRequested)
						return;

					var size = model.ImageFile.IsArt
						? getSizeArt()
						: model.Rotation == RotateFlipType.Rotate270FlipNone || model.Rotation == RotateFlipType.Rotate90FlipNone
							? _imageLoader.ZoomedCardSize.Transpose()
							: _imageLoader.ZoomedCardSize;

					var image = _imageLoader.LoadImage(model, size);

					if (image == null)
						continue;

					add(index, model, image);
					index++;
				}
			}
		}

		private void add(int index, ImageModel model, Bitmap image)
		{
			if (index < _images.Count)
			{
				_images[index] = image;
				_models[index] = model;
			}
			else
			{
				_images.Add(image);
				_models.Add(model);
			}
		}

		private static Size getSizeArt()
		{
			var screenArea = getScreenArea();

			if (screenArea.Width > screenArea.Height)
				return new SizeF(screenArea.Height + (screenArea.Width - screenArea.Height) * 0.75f, screenArea.Height).Round();
			else
				return new SizeF(screenArea.Width, screenArea.Width + (screenArea.Height - screenArea.Width) * 0.75f).Round();
		}

		private void updateImage()
		{
			_image = _images[_imageIndex];
			_pictureBox.Image = _image;
		}

		private void mouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta == 0)
				return;

			bool changed;
			if (e.Delta < 0)
				changed = previousImage();
			else
				changed = nextImage();

			if (changed)
			{
				updateImage();
				applyZoom();
				Application.DoEvents();
			}
		}

		private bool filter(ImageModel imageModel)
		{
			if (!_showOtherSetsButton.Checked && imageModel.ImageFile.SetCode != _card.SetCode)
				return false;

			if (_showDuplicatesButton.Checked)
				return true;

			var currentSetCode = _models[_imageIndex].ImageFile.SetCode;

			var setRepresentative = _models.Where(_ => _.ImageFile.SetCode == currentSetCode)
				.AtMin(_ => _.ImageFile.VariantNumber).Find();

			return imageModel.ImageFile.SetCode != currentSetCode || imageModel == setRepresentative;
		}

		private bool nextImage()
		{
			for (int i = _imageIndex + 1; i < _images.Count; i++)
				if (filter(_models[i]))
				{
					_imageIndex = i;
					return true;
				}

			return false;
		}

		private bool previousImage()
		{
			for (int i = _imageIndex - 1; i >= 0; i--)
				if (filter(_models[i]))
				{
					_imageIndex = i;
					return true;
				}

			return false;
		}

		private void applyZoom()
		{
			var formLocation = new Point(
				(int)(_location.X - _image.Size.Width * 0.5f),
				(int)(_location.Y - _image.Size.Height * 0.5f));

			var formArea = new Rectangle(formLocation, _image.Size);

			var screenArea = getScreenArea();
			var workingArea = new Rectangle(
				screenArea.Left,
				screenArea.Top,
				screenArea.Width,
				screenArea.Height);

			if (formArea.Bottom > workingArea.Bottom)
				formArea.Offset(0, workingArea.Bottom -formArea.Bottom);

			if (formArea.Right > workingArea.Right)
				formArea.Offset(workingArea.Right - formArea.Right, 0);

			if (formArea.Left < workingArea.Left)
				formArea.Offset(workingArea.Left - formArea.Left, 0);

			if (formArea.Top < workingArea.Top)
				formArea.Offset(0, workingArea.Top - formArea.Top);

			_pictureBox.Size = _image.Size;
			_pictureBox.Location = Point.Empty;

			ClientSize = formArea.Size;
			Location = formArea.Location;
		}

		private static Rectangle getScreenArea()
		{
			var screen = Screen.FromPoint(Cursor.Position);
			var workingArea = screen.WorkingArea;
			return workingArea;
		}

		private void click(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			hideImage();
		}

		private void hideImage()
		{
			_ctsLoadingImages?.Cancel();

			_pictureBox.Image = null;
			Application.DoEvents();
			Hide();
		}

		private void openInExplorerClick(object sender, EventArgs e)
		{
			FsPath fullPath = _models[_imageIndex].ImageFile.FullPath;

			if (!fullPath.IsFile())
				return;

			if (Runtime.IsLinux)
			{
				var explorerApp = detectFileExplorerApp();
				if (!string.IsNullOrEmpty(explorerApp))
					Process.Start(explorerApp, $"--select \"{fullPath}\"");
				else
					Process.Start("xdg-open", $"\"{fullPath.Parent()}\"");
			}
			else
				Process.Start("explorer.exe", $"/select, \"{fullPath}\"");
		}

		private static string detectFileExplorerApp()
		{
			if (_nativeExplorerApp != null)
				return _nativeExplorerApp;

			_nativeExplorerApp = detect();
			return _nativeExplorerApp;

			static string detect()
			{
				var queryProcess = Process.Start(
					new ProcessStartInfo("xdg-mime", "query default inode/directory")
					{
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true
					});

				if (queryProcess == null)
					return string.Empty;

				queryProcess.WaitForExit();
				string output = queryProcess.StandardOutput.ReadToEnd();
				var appDesktopName = output.TrimEnd();
				var appDesktopFile = new FsPath($"/usr/share/applications/{appDesktopName}");
				if (!appDesktopFile.IsFile())
					return string.Empty;

				string[] lines;
				try
				{
					lines = appDesktopFile.ReadAllLines();
				}
				catch
				{
					return string.Empty;
				}

				const string prefix = "Exec=";
				string line = lines.FirstOrDefault(_ => _.StartsWith(prefix));
				if (line == null)
					return string.Empty;

				string command = line.Substring(prefix.Length);
				string name = new Regex(@"( %\w)+$").Replace(command, "");
				return name;
			}
		}

		private void openFileClick(object sender, EventArgs e)
		{
			FsPath fullPath = _models[_imageIndex].ImageFile.FullPath;
			if (Runtime.IsLinux)
				Process.Start("xdg-open", $"\"{fullPath}\"");
			else
				Process.Start(fullPath.Value);
		}


		public GuiSettings.ZoomSettings Settings
		{
			get => new GuiSettings.ZoomSettings
			{
				ShowArt = _showArtButton.Checked,
				ShowVariants = _showDuplicatesButton.Checked,
				ShowOtherSet = _showOtherSetsButton.Checked
			};

			set
			{
				if (value == null)
					return;

				_showArtButton.Checked = value.ShowArt;
				_showDuplicatesButton.Checked = value.ShowVariants;
				_showOtherSetsButton.Checked = value.ShowOtherSet;
			}
		}

		private void onSettingsChanged() =>
			SettingsChanged?.Invoke();

		private void handleDisposed(object s, EventArgs e) =>
			_ctsLifetime.Cancel();

		public event Action SettingsChanged;

		private readonly CardRepository _cardRepository;
		private readonly ImageRepository _imageRepository;
		private readonly ImageLoader _imageLoader;
		private int _imageIndex;
		private Bitmap _image;

		private readonly List<Bitmap> _images = new List<Bitmap>();
		private readonly List<ImageModel> _models = new List<ImageModel>();

		private static readonly Color _defaultBgColor = Color.FromArgb(253, 247, 254);
		private Card _card;

		private List<Card> _cardForms;
		private Point _location;
		private bool _showArt;
		private UiModel _ui;

		private static string _nativeExplorerApp;

		private CancellationTokenSource _ctsLoadingImages;
		private readonly CancellationTokenSource _ctsLifetime;
	}
}
