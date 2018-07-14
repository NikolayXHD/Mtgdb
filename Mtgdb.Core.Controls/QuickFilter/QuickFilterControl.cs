using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Bitmaps;

namespace Mtgdb.Controls
{
	public class QuickFilterControl : UserControl
	{
		public QuickFilterControl()
		{
			init();

			ImageSize = new Size(20, 20);
			PropertiesCount = DefaultPropertiesCount;

			_selectionColor = Color.Transparent;
			_selectionBorderColor = Color.Transparent;
			_prohibitedColor = Color.Transparent;

			_opacityEnabled = OpacityEnabled;
			_opacityToEnable = OpacityToEnable;
			_opacityToDisable = OpacityToDisable;
			_opacityDisabled = OpacityDisabled;

			CostNeutralValues = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

			Paint += controlPaint;
			MouseClick += controlClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
			MouseMove += mouseMove;
		}


		private void makeProhibitedValuesIgnored()
		{
			if (_states == null)
				return;

			bool stateChanged = false;

			for (int i = 0; i < _states.Length; i++)
			{
				stateChanged = _states[i] == FilterValueState.Prohibited;
				if (stateChanged)
					_states[i] = FilterValueState.Ignored;
			}

			if (stateChanged)
				StateChanged?.Invoke(this, null);
		}

		private void createDerivedImages()
		{
			if (PropertyImages == null)
				return;

			_images =
				transformImages(Opacity1Enabled, PropertyImages);

			_imagesToEnable =
				transformImages(Opacity2ToEnable, PropertyImages);

			_imagesToDisable =
				transformImages(Opacity3ToDisable, PropertyImages);

			_imagesDisabled =
				transformImages(Opacity4Disabled, PropertyImages);
		}

		private IList<Bitmap> transformImages(
			float opacity,
			IList<Bitmap> imageCollection)
		{
			if (imageCollection == null || imageCollection.Count == 0)
				return null;

			var imagesTransformed = new Bitmap[imageCollection.Count];

			for (int i = 0; i < imagesTransformed.Length; i++)
			{
				imagesTransformed[i] = imageCollection[i].FitIn(ImageSize);
				new GrayscaleBmpProcessor(imagesTransformed[i], opacity)
					.Execute();
			}

			return imagesTransformed;
		}

		private void setButtonState(StateClick click, FilterValueState[] states)
		{
			FilterValueState[] filterValueStates = states;
			if ((int) filterValueStates[click.ButtonIndex] % 2 != (int) click.ClickedState % 2)
				applyCheckLogic(click, states);
			else if (EnableRequiringSome && filterValueStates[click.ButtonIndex] == FilterValueState.Ignored && click.ClickedState == FilterValueState.Ignored)
				applyCheckLogic(click, states);
			else
				applyUncheckLogic(click, states);
		}

		private void applyUncheckLogic(StateClick click, FilterValueState[] states)
		{
			switch (click.ClickedState)
			{
				case FilterValueState.RequiredSome:
				case FilterValueState.Required:

					states[click.ButtonIndex] = FilterValueState.Ignored;
					break;

				case FilterValueState.Ignored:

					if (EnableCostBehavior && click.MouseButton == MouseButtons.Left)
						applyUncheckCostLogic(click, states);
					else if (EnableRequiringSome)
						states[click.ButtonIndex] = FilterValueState.Ignored;
					else
						states[click.ButtonIndex] = FilterValueState.Prohibited;

					break;

				case FilterValueState.Prohibited:

					states[click.ButtonIndex] = FilterValueState.Ignored;
					break;
			}
		}

		private bool isCostNeutral(int buttonIndex)
		{
			string value = Properties[buttonIndex];
			return CostNeutralValues.Contains(value);
		}

		private void applyUncheckCostLogic(StateClick click, FilterValueState[] states)
		{
			bool allAllowed = true;
			bool allProhibited = true;

			var i = click.ButtonIndex;
			for (int j = 0; j < states.Length; j++)
			{
				if (i == j)
					continue;

				allProhibited &= states[j] == FilterValueState.Prohibited;

				if (isCostNeutral(j))
					continue;

				allAllowed &= states[j] == FilterValueState.Ignored;
			}

			if (allAllowed && !isCostNeutral(i))
			{
				// Оставить разрешёнными только кликнутое требование и его более слабую версию
				// Например, {W} и {W/P}
				for (int j = 0; j < states.Length; j++)
				{
					if (i == j || isCostNeutral(j) || isWeakerRequirement(i, j))
						continue;

					states[j] = FilterValueState.Prohibited;
				}
			}
			else if (allProhibited)
			{
				for (int j = 0; j < states.Length; j++)
				{
					if (i == j || isCostNeutral(j) && !isCostNeutral(i))
						continue;

					states[j] = FilterValueState.Ignored;
				}
			}
			else
				states[i] = FilterValueState.Prohibited;
		}

		private bool isWeakerRequirement(int i, int j)
		{
			if (Properties?[i] == null || Properties[j] == null)
				return false;

			return Properties[j].Trim('{', '}').Contains(Properties[i].Trim('{', '}'));
		}

		private void applyCheckLogic(StateClick click, FilterValueState[] states)
		{
			var i = click.ButtonIndex;

			if (EnableMutuallyExclusive && click.ClickedState == FilterValueState.Required)
				for (int j = 0; j < states.Length; j++)
					if (j != i && states[j] == FilterValueState.Required)
						states[j] = FilterValueState.Ignored;

			if (EnableCostBehavior && click.MouseButton == MouseButtons.Left &&
				states[i] == FilterValueState.Prohibited && click.ClickedState == FilterValueState.Ignored)
			{
				for (int j = 0; j < states.Length; j++)
					if ((i == j || isWeakerRequirement(i, j)) && states[j] == FilterValueState.Prohibited)
						states[j] = FilterValueState.Ignored;
			}

			if (EnableRequiringSome && states[i] != FilterValueState.RequiredSome && click.ClickedState == FilterValueState.Ignored)
				states[i] = FilterValueState.RequiredSome;
			else
				states[i] = click.ClickedState;
		}



		private void paintRequireButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			var value = FilterValueState.Required;

			if (EnableMutuallyExclusive || state != value && statePreview != value && _lastClickPreview?.ButtonIndex != i)
				return;

			paintButton(e, state, statePreview, value, ImageRequired ?? _imagesDisabled[i], i);
		}

		private void paintAllowButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			FilterValueState value =
				EnableRequiringSome
					? FilterValueState.RequiredSome
					: FilterValueState.Ignored;

			if ((state == FilterValueState.Required || state == FilterValueState.Prohibited && !HideProhibit) && statePreview != value && _lastClickPreview?.ButtonIndex != i)
				return;

			paintButton(e, state, statePreview, value, _imagesDisabled[i], i);
		}

		private void paintProhibitButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			if (HideProhibit)
				return;

			var value = FilterValueState.Prohibited;

			if (state != value && statePreview != value && _lastClickPreview?.ButtonIndex != i)
				return;

			paintButton(e, state, statePreview, value, _imagesDisabled[i], i);
		}

		private void paintButton(
			PaintEventArgs e,
			FilterValueState state,
			FilterValueState statePreview,
			FilterValueState filterValueState,
			Image imageDisabled,
			int i)
		{
			if (imageDisabled == null)
				return;

			var rectangle = getPaintingRectangle(i, filterValueState);

			bool enabled = state == filterValueState;
			bool enabledPreview = statePreview == filterValueState;

			if (enabled)
			{
				if (enabledPreview || !_showPreview)
					e.Graphics.DrawImage(_images[i], rectangle);
				else
					e.Graphics.DrawImage(_imagesToDisable[i], rectangle);
			}
			else /* !enabled */
			{
				if (!enabledPreview || !_showPreview)
					e.Graphics.DrawImage(imageDisabled, rectangle);
				else
					e.Graphics.DrawImage(_imagesToEnable[i], rectangle);
			}
		}



		private void updateSize()
		{
			Size = MinimumSize = getSize();
		}

		private Size getSize()
		{
			int statesCount = StatesCount;

			int border = Border;

			int propertiesCount = PropertiesCount;

			int width =
				propertiesCount * _imageSize.Width +
				(propertiesCount - 1) * Spacing.Width +
				2 * border;

			int height =
				statesCount * _imageSize.Height +
				(statesCount - 1) * _spacing.Height +
				2 * border;

			if (IsVertical)
				return new Size(height, width);

			return new Size(width, height);
		}

		private Rectangle getPaintingRectangle(int i, FilterValueState state)
		{
			int imageWidth = ImageSize.Width;
			var imageHeight = ImageSize.Height;

			// чтобы Allowed был 0 Prohibited сделан -1
			const int maxStateIndex = 1;
			// таким образом RequireSome == Allowed для вычисления координат
			int stateIndex = (int) state % 2;

			if (EnableMutuallyExclusive)
				stateIndex++;

			int shift = maxStateIndex - stateIndex;

			int border = Border;

			int x = border + i * (imageWidth + Spacing.Width);
			var y = border + shift * (imageHeight + Spacing.Height);

			if (IsVertical)
			{
				swap(ref x, ref y);
				swap(ref imageWidth, ref imageHeight);

				if (IsFlipped)
					x = Width - x - imageWidth;
			}

			var result = new Rectangle(x, y, imageWidth, imageHeight);
			return result;
		}

		private StateClick? getCommand(int x, int y, MouseButtons button)
		{
			var border = Border;

			if (IsVertical)
			{
				if (IsFlipped)
					x = Width - x;

				swap(ref x, ref y);
			}

			int index = (x - border + Spacing.Width / 2) / (ImageSize.Width + Spacing.Width);

			if (index >= PropertiesCount)
				index = (x - border + Spacing.Width) / (ImageSize.Width + Spacing.Width);

			if (index < 0 || index >= PropertiesCount)
				return null;

			int stateIndex = (y - border + Spacing.Height / 2) / (ImageSize.Height + Spacing.Height);

			if (stateIndex == StatesCount)
				stateIndex = (y - border + Spacing.Height) / (ImageSize.Height + Spacing.Height);

			if (EnableMutuallyExclusive)
				stateIndex++;

			switch (stateIndex)
			{
				case 0:
					return new StateClick(index, FilterValueState.Required, button);
				case 1:
					return new StateClick(index, FilterValueState.Ignored, button);
				case 2:
					if (HideProhibit)
						return null;

					return new StateClick(index, FilterValueState.Prohibited, button);
				default:
					return null;
			}
		}

		private static void swap<T>(ref T x, ref T y)
		{
			var tmp = x;
			x = y;
			y = tmp;
		}



		private void controlClick(object sender, MouseEventArgs e)
		{
			var command = getCommand(e.X, e.Y, e.Button);

			if (!command.HasValue)
				return;

			_lastClick = command;
			_showPreview = false;

			if (e.Button == MouseButtons.Middle)
			{
				States = null;
				return;
			}

			setButtonState(command.Value, _states);
			Refresh();

			StateChanged?.Invoke(this, null);
		}



		private void controlPaint(object sender, PaintEventArgs e)
		{
			if (_images == null)
				createDerivedImages();

			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

			paintProhibitZone(e);
			paintSelection(e);

			for (int i = 0; i < PropertiesCount; i++)
			{
				var state = _states?[i] ?? FilterValueState.Ignored;
				var statePreview = _showPreview
					? _statesPreview?[i] ?? FilterValueState.Ignored
					: state;

				if (_images == null || i >= _images.Count)
					return;

				if (_lastClickPreview?.ClickedState == FilterValueState.Required)
				{
					paintProhibitButton(e, state, statePreview, i);
					paintAllowButton(e, state, statePreview, i);
					paintRequireButton(e, state, statePreview, i);
				}
				else if (_lastClickPreview?.ClickedState == FilterValueState.Prohibited)
				{
					paintRequireButton(e, state, statePreview, i);
					paintAllowButton(e, state, statePreview, i);
					paintProhibitButton(e, state, statePreview, i);
				}
				else
				{
					paintRequireButton(e, state, statePreview, i);
					paintProhibitButton(e, state, statePreview, i);
					paintAllowButton(e, state, statePreview, i);
				}
			}

			if (_mouseInside && ShowValueHint)
				paintValueHint(e);
		}

		private void paintProhibitZone(PaintEventArgs e)
		{
			if (HideProhibit)
				return;

			for (int i = 0; i < PropertiesCount; i++)
			{
				var rectangle = (RectangleF) getPaintingRectangle(i, FilterValueState.Prohibited);

				if (IsVertical)
					rectangle.Offset(Spacing.Height, 0);
				else
					rectangle.Offset(0, -Spacing.Height);

				var color = Color.FromArgb(30, ProhibitedColor);
				var color2 = Color.FromArgb(0, ProhibitedColor);

				Brush fillBrush;

				float widthPart = 0.5f;

				if (IsVertical)
				{
					rectangle.Inflate(new SizeF(0, Spacing.Width / 2f));

					if (IsFlipped)
					{
						rectangle = new RectangleF(
							new PointF(rectangle.Right - rectangle.Width * widthPart, rectangle.Top),
							new SizeF(rectangle.Width * widthPart, rectangle.Height));

						fillBrush = new LinearGradientBrush(
							new PointF(rectangle.Right + 1, rectangle.Top),
							new PointF(rectangle.Left, rectangle.Top),
							color,
							color2);
					}
					else
					{
						rectangle = new RectangleF(
							rectangle.Location,
							new SizeF(rectangle.Width * widthPart, rectangle.Height));

						fillBrush = new LinearGradientBrush(
							new PointF(rectangle.Left, rectangle.Top),
							new PointF(rectangle.Right + 1, rectangle.Top),
							color,
							color2);
					}
				}
				else
				{
					rectangle.Inflate(new SizeF(Spacing.Width / 2f, 0));

					rectangle = new RectangleF(
						rectangle.Location,
						new SizeF(rectangle.Width, rectangle.Height * widthPart));

					fillBrush = new LinearGradientBrush(
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Left, rectangle.Bottom + 1),
						color,
						color2);
				}

				e.Graphics.FillRectangle(fillBrush, rectangle);
			}
		}

		private void paintValueHint(PaintEventArgs e)
		{
			if (Properties == null)
				return;

			var command = getCommand(_mouseLocation.X, _mouseLocation.Y, MouseButtons.None);

			if (!command.HasValue)
				return;

			var property = Properties[command.Value.ButtonIndex];

			if (string.IsNullOrEmpty(property))
				return;

			var font = Font;
			var border = Border;

			var location = getPaintingRectangle(command.Value.ButtonIndex, command.Value.ClickedState)
				.Location;

			var textSize = e.Graphics.MeasureText(property, font);
			bool rotateText = IsVertical && textSize.Width > Width;

			int x;
			int y;

			if (IsVertical)
			{
				if (rotateText)
				{
					x = HintTextShift.Height + location.Y + ImageSize.Height + border;
					y = -(HintTextShift.Width + location.X + ImageSize.Width - (ImageSize.Width - font.Height) / 2);
				}
				else
				{
					x = HintTextShift.Width + location.X;
					y = HintTextShift.Height + location.Y + ImageSize.Height + border;
				}
			}
			else
			{
				x = HintTextShift.Width + location.X + ImageSize.Width + border;
				y = HintTextShift.Height + location.Y + (ImageSize.Height - font.Height) / 2;
			}

			if (rotateText)
			{
				if (x < 0)
					x = 0;
				else if (x + textSize.Width > Height)
					x = Height - textSize.Width;

				if (-y < 0)
					y = 0;
				else if (-(y + textSize.Height) > Width)
					y = -(Width - textSize.Height);
			}
			else
			{
				if (x < 0)
					x = 0;
				else if (x + textSize.Width > Width)
					x = Width - textSize.Width;

				if (y < 0)
					y = 0;
				else if (y + textSize.Height > Height)
					y = Height - textSize.Height;
			}

			if (rotateText)
				e.Graphics.RotateTransform(90f);

			var textLocation = new Point(x, y);

			if (HintIcon != null)
				e.Graphics.DrawImage(HintIcon, textLocation.Plus(HintIconShift).Minus(HintTextShift));

			e.Graphics.DrawText(property, font, Color.Black, Color.White, 1f, 3f, new Rectangle(textLocation, textSize));

			if (rotateText)
				e.Graphics.RotateTransform(-90f);
		}

		private void paintSelection(PaintEventArgs e)
		{
			for (int i = 0; i < _states.Length; i++)
			{
				var state = _states[i];

				if (EnableRequiringSome && state == FilterValueState.Ignored)
					continue;

				if (state == FilterValueState.Prohibited && HideProhibit)
					continue;

				var rectBorder = (RectangleF) getPaintingRectangle(i, state);

				rectBorder.Offset(-0.5f, -0.5f);
				var rectContent = rectBorder;
				rectBorder.Inflate(SelectionBorder, SelectionBorder);

				if (!rectBorder.IntersectsWith(e.ClipRectangle))
					continue;

				Action<Color, RectangleF> fill;
				Action<Color, RectangleF, float> fillBorder;

				if (BorderShape == BorderShape.Ellipse)
					fill = (c, r) => e.Graphics.FillEllipse(new SolidBrush(c), r);
				else if (BorderShape == BorderShape.Rectangle)
					fill = (c, r) => e.Graphics.FillRectangle(new SolidBrush(c), r);
				else
					throw new NotSupportedException();

				if (BorderShape == BorderShape.Ellipse)
					fillBorder = (c, r, w) =>
					{
						r.Inflate(-w / 2, -w / 2);
						e.Graphics.DrawEllipse(new Pen(c, w), r);
					};
				else if (BorderShape == BorderShape.Rectangle)
					fillBorder = (c, r, w) => { e.Graphics.DrawRectangles(new Pen(c, w), Array.From(r)); };
				else
					throw new NotSupportedException();

				Color selectionColor;

				if (state == FilterValueState.Prohibited)
					selectionColor = Color.FromArgb((int) (255 * 0.2f), ProhibitedColor);
				else
					selectionColor = SelectionColor;

				if (SelectionBorder > 0)
				{
					fillBorder(SelectionBorderColor, rectBorder, SelectionBorder);
					fill(selectionColor, rectContent);
				}
				else if (SelectionBorder < 0)
				{
					fillBorder(SelectionBorderColor, rectContent, -SelectionBorder);
					fill(selectionColor, rectBorder);
				}
				else
				{
					fill(selectionColor, rectBorder);
				}
			}
		}



		private void mouseEnter(object sender, EventArgs e)
		{
			_lastClick = null;
			_lastClickPreview = null;
			_mouseInside = true;
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			_mouseInside = false;
			_showPreview = false;
			_lastClickPreview = null;
			Invalidate();
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (!_mouseInside)
				return;

			if (!ClientRectangle.Contains(e.Location))
				return;

			_mouseLocation = e.Location;

			var commandPreview = getCommand(e.X, e.Y, MouseButtons.Left);
			if (commandPreview == null)
			{
				if (_lastClickPreview != null)
				{
					_statesPreview = _states.ToArray();
					_lastClickPreview = null;
					Refresh();
				}

				return;
			}

			if (commandPreview != _lastClick)
				_lastClick = null;

			// _lastClick != null => состояние после клика уже прорисовано
			if (_lastClick == null)
			{
				if (commandPreview != _lastClickPreview)
				{
					_lastClickPreview = commandPreview;

					_showPreview = true;
					_statesPreview = _states.ToArray();
					setButtonState(commandPreview.Value, _statesPreview);

					Invalidate();
				}
			}
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
				components?.Dispose();

			base.Dispose(disposing);
		}

		private void init()
		{
			SuspendLayout();

			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			Name = "QuickFilterControl";
			Size = new Size(100, 60);
			DoubleBuffered = true;

			ResumeLayout(false);
		}



		[Category("Settings")]
		public event EventHandler StateChanged;

		[Category("Settings")]
		[DefaultValue(typeof(Size), "20, 20")]
		public Size ImageSize
		{
			get => _imageSize;
			set
			{
				_imageSize = value;
				updateSize();
				createDerivedImages();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size Spacing
		{
			get => _spacing;
			set
			{
				_spacing = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(DefaultPropertiesCount)]
		public int PropertiesCount
		{
			get => _propertiesCount;
			set
			{
				_propertiesCount = value;
				_states = new FilterValueState[value];
				StatesDefault = new FilterValueState[value];
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Image ImageRequired
		{
			get => _imageRequired;
			set
			{
				_imageRequired = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap HintIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size HintTextShift { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size HintIconShift { get; set; }

		[Category("Settings")]
		public IList<Bitmap> PropertyImages
		{
			get => _propertyImages;
			set
			{
				_propertyImages = value;
				createDerivedImages();

				if (value != null)
					PropertiesCount = value.Count;

				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color SelectionColor
		{
			get => _selectionColor;
			set
			{
				_selectionColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color SelectionBorderColor
		{
			get => _selectionBorderColor;
			set
			{
				_selectionBorderColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color ProhibitedColor
		{
			get => _prohibitedColor;
			set
			{
				_prohibitedColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(BorderShape), "Rectangle")]
		public BorderShape BorderShape
		{
			get => _borderShape;
			set
			{
				_borderShape = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(0f)]
		public float SelectionBorder
		{
			get => _selectionBorder;
			set
			{
				_selectionBorder = value;
				updateSize();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool EnableCostBehavior { get; set; }

		[Category("Settings")]
		[DefaultValue(false)]
		public bool EnableMutuallyExclusive
		{
			get => _enableMutuallyExclusive;
			set
			{
				_enableMutuallyExclusive = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool HideProhibit
		{
			get => _hideProhibit;
			set
			{
				_hideProhibit = value;

				updateSize();

				if (value && EnableRequiringSome)
					// В режиме EnableRequiringSome невозможно визуально отличить запрещённые значения от игнорируемых
					makeProhibitedValuesIgnored();

				Invalidate();
			}
		}

		/// <summary>
		/// <para>Позволяет указывать желаемые занчения через или, то есть включает возможность выбора состояния RequiredSome.</para>
		/// <para>Состояние RequiredSome отображается на месте Ignored</para>
		/// <para>Состояние Ignored не отображается</para>
		/// <para>При отключении кнопки в состоянии RequiredSome она переходит в Ignored</para>
		/// <para>Включить состояние Prohibited при скрытых кнопках данного состояния становится невозможно</para>
		/// </summary>
		[Category("Settings"), DefaultValue(false)]
		public bool EnableRequiringSome
		{
			get => _enableRequiringSome;
			set
			{
				if (value != _enableRequiringSome)
				{
					_enableRequiringSome = value;

					if (_states != null && !value)
						for (int i = 0; i < _states.Length; i++)
							if (_states[i] == FilterValueState.RequiredSome)
								_states[i] = FilterValueState.Ignored;

					Invalidate();
					StateChanged?.Invoke(this, null);
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(OpacityEnabled)]
		public float Opacity1Enabled
		{
			get => _opacityEnabled;
			set
			{
				_opacityEnabled = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(OpacityDisabled)]
		public float Opacity4Disabled
		{
			get => _opacityDisabled;
			set
			{
				_opacityDisabled = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(OpacityToDisable)]
		public float Opacity3ToDisable
		{
			get => _opacityToDisable;
			set
			{
				_opacityToDisable = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[DefaultValue(OpacityToEnable)]
		[Category("Settings")]
		public float Opacity2ToEnable
		{
			get => _opacityToEnable;
			set
			{
				_opacityToEnable = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool ShowValueHint { get; set; }

		[Category("Settings")]
		[DefaultValue(false)]
		public bool IsVertical
		{
			get => _isVertical;
			set
			{
				_isVertical = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool IsFlipped
		{
			get => _isFlipped;
			set
			{
				_isFlipped = value;
				Invalidate();
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FilterValueState[] StatesDefault { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FilterValueState[] States
		{
			get => _states.ToArray();
			set
			{
				if (value == null || value.Length != PropertiesCount)
				{
					if (StatesDefault != null && StatesDefault.Length == PropertiesCount)
						_states = StatesDefault.ToArray();
					else
						_states = new FilterValueState[PropertiesCount];
				}
				else
					_states = value.ToArray();

				_statesPreview = _states.ToArray();

				Invalidate();
				StateChanged?.Invoke(this, null);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HashSet<string> CostNeutralValues { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<string> Properties { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private int StatesCount
		{
			get
			{
				int statesCount = 1;

				if (!EnableMutuallyExclusive)
					statesCount++;

				if (!HideProhibit)
					statesCount++;

				return statesCount;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Border => (int) Math.Ceiling(SelectionBorder);


		private const float OpacityEnabled = 1.00f;
		private const float OpacityToEnable = 0.90f;
		private const float OpacityToDisable = 0.30f;
		private const float OpacityDisabled = 0.24f;
		private const int DefaultPropertiesCount = 5;

		private IList<Bitmap> _images;
		private IList<Bitmap> _imagesToDisable;
		private IList<Bitmap> _imagesToEnable;
		private IList<Bitmap> _imagesDisabled;

		private FilterValueState[] _statesPreview;
		private StateClick? _lastClick;
		private StateClick? _lastClickPreview;
		private bool _showPreview;
		private bool _mouseInside;
		private Point _mouseLocation;

		private FilterValueState[] _states;
		private int _propertiesCount;
		private Size _imageSize;
		private Size _spacing;
		private IList<Bitmap> _propertyImages;
		private Color _selectionColor;
		private Color _selectionBorderColor;
		private Color _prohibitedColor;
		private Image _imageRequired;
		private bool _enableRequiringSome;
		private float _selectionBorder;
		private BorderShape _borderShape;
		private bool _enableMutuallyExclusive;
		private bool _hideProhibit;
		private float _opacityEnabled;
		private float _opacityDisabled;
		private float _opacityToDisable;
		private float _opacityToEnable;
		private bool _isVertical;
		private bool _isFlipped;

		private readonly IContainer components = null;
	}
}