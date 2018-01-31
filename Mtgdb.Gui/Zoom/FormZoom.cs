using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public sealed partial class FormZoom : Form
	{
		private readonly CardRepository _cardRepository;
		private readonly ImageRepository _imageRepository;
		private readonly ImageLoader _imageLoader;
		private int _imageIndex;
		private Bitmap _image;

		private readonly List<Bitmap> _images = new List<Bitmap>();
		private readonly List<ImageModel> _models = new List<ImageModel>();

		private static readonly Color _defaultBgColor = Color.FromArgb(254, 247, 253);
		private Card _card;

		private Thread _imageLoadingThread;
		private List<Card> _cardForms;
		private Point _location;

		public FormZoom()
		{
			InitializeComponent();
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

			_pictureBox.MouseClick += click;
			MouseWheel += mouseWheel;
			DoubleBuffered = true;

			var hotSpot = new Size(14, 8).ByDpi();
			var cursorImage = Resources.rightclick_48.HalfResizeDpi();
			Cursor = CursorHelper.CreateCursor(cursorImage, hotSpot);

			_openFileButton.Image = Dpi.ScalePercent > 100
				? Resources.image_file_32.HalfResizeDpi()
				: Resources.image_file_16.ResizeDpi();

			_showInExplorerButton.Image = Dpi.ScalePercent > 100
				? Resources.open_32.HalfResizeDpi()
				: Resources.open_16.ResizeDpi();

			var cloneImg = Resources.clone_48.HalfResizeDpi();

			_showDuplicatesButton.Image = cloneImg;
			_showOtherSetsButton.Image = cloneImg;
		}

		public void LoadImages(Card card)
		{
			_location = Cursor.Position;
			_imageLoadingThread?.Abort();

			_cardForms = _cardRepository.GetForms(card);
			_card = card;

			foreach (var oldImg in _images)
				oldImg.Dispose();

			_images.Clear();
			_models.Clear();
			_imageIndex = 0;

			_imageLoadingThread = new Thread(loadImages);
			_imageLoadingThread.Start();

			while (_images.Count == 0)
				Thread.Sleep(50);
		}

		public void ShowImages()
		{
			updateImage();
			applyZoom();

			Application.DoEvents();

			Show();
			Focus();
		}


		private void loadImages()
		{
			try
			{
				bool repoLoadingComplete = isRepoLoadingComplete();
				load();

				if (repoLoadingComplete)
					return;

				while (!isRepoLoadingComplete())
					Thread.Sleep(100);

				load();
			}
			catch (ThreadAbortException)
			{
			}
		}

		private bool isRepoLoadingComplete()
		{
			return _imageRepository.IsLoadingArtComplete && _imageRepository.IsLoadingZoomComplete;
		}

		private void load()
		{
			int index = 0;
			for (int j = 0; j < _cardForms.Count; j++)
			{
				foreach (var model in _cardRepository.GetImagesArt(_cardForms[j], _imageRepository))
				{
					while (index > _imageIndex + 10)
						Thread.Sleep(100);

					var size = model.ImageFile.IsArt
						? getSizeArt()
						: _imageLoader.ZoomedCardSize;

					var image = _imageLoader.LoadImage(model, size);

					if (image == null)
						continue;

					add(index, model, image);
					index++;
				}

				foreach (var model in _cardRepository.GetZoomImages(_cardForms[j], _imageRepository))
				{
					while (index > _imageIndex + 10)
						Thread.Sleep(100);

					var size = model.ImageFile.IsArt
						? getSizeArt()
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

			Size = formArea.Size;
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
			_imageLoadingThread?.Abort();
			_pictureBox.Image = null;
			Application.DoEvents();
			Hide();
		}

		private void openInExplorerClick(object sender, EventArgs e)
		{
			string fullPath = _models[_imageIndex].ImageFile.FullPath;

			if (!File.Exists(fullPath))
				return;

			string workingDirectory = Path.GetDirectoryName(fullPath);
			if (workingDirectory == null)
				return;

			Process.Start(
				new ProcessStartInfo("explorer.exe", $@"/select,""{fullPath}"""));
		}

		private void openFileClick(object sender, EventArgs e)
		{
			string fullPath = _models[_imageIndex].ImageFile.FullPath;
			Process.Start(new ProcessStartInfo(fullPath));
		}
	}
}