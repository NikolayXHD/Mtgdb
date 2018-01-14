using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class LayoutOptions
	{
		private const Direction DefaultDirection = Direction.TopLeft;
		public event Action Changed;

		public Bitmap GetAlignIcon(Direction direction, bool hovered)
		{
			var iconsByDirection = hovered
				? _alignmentHoveredIconsByDirection ?? _alignmentIconsByDirection
				: _alignmentIconsByDirection;

			return iconsByDirection.TryGet(direction);
		}

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size CardInterval
		{
			get { return _cardInterval; }
			set
			{
				if (value != _cardInterval)
				{
					_cardInterval = value;
					Changed?.Invoke();
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool AllowPartialCards
		{
			get { return _allowPartialCards; }
			set
			{
				if (value != _allowPartialCards)
				{
					_allowPartialCards = value;
					Changed?.Invoke();
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size PartialCardsThreshold
		{
			get { return _partialCardsThreshold; }
			set
			{
				if (value != _partialCardsThreshold)
				{
					_partialCardsThreshold = value;
					Changed?.Invoke();
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Direction), "TopLeft")]
		public Direction Alignment
		{
			get { return _alignment; }
			set
			{
				if (value != _alignment)
				{
					_alignment = value;
					Changed?.Invoke();
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap AlignTopLeftIcon
		{
			get { return _alignTopLeftIcon; }
			set
			{
				_alignTopLeftIcon = value;
				setAlignIcons(_alignmentIconsByDirection, value);
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap AlignTopLeftHoveredIcon
		{
			get { return _alignTopLeftHoveredIcon; }
			set
			{
				_alignTopLeftHoveredIcon = value;
				setAlignIcons(_alignmentHoveredIconsByDirection, value);
			}
		}

		private static void setAlignIcons(Dictionary<Direction, Bitmap> iconsByDirection, Bitmap value)
		{
			iconsByDirection[Direction.TopLeft] = value;

			value = value.Transform(RotateFlipType.RotateNoneFlipX);
			iconsByDirection[Direction.TopRight] = value;

			value = value.Transform(RotateFlipType.RotateNoneFlipY);
			iconsByDirection[Direction.BottomRight] = value;

			value = value.Transform(RotateFlipType.RotateNoneFlipX);
			iconsByDirection[Direction.BottomLeft] = value;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasAlignIcons => _alignmentIconsByDirection.Count > 0;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size AlignIconSize => _alignmentIconsByDirection.TryGet(DefaultDirection)?.Size ?? default(Size);

		private Bitmap _alignTopLeftHoveredIcon;
		private Bitmap _alignTopLeftIcon;
		private bool _allowPartialCards;
		private Size _cardInterval;
		private Size _partialCardsThreshold;
		private Direction _alignment = DefaultDirection;

		private readonly Dictionary<Direction, Bitmap> _alignmentIconsByDirection = new Dictionary<Direction, Bitmap>();
		private readonly Dictionary<Direction, Bitmap> _alignmentHoveredIconsByDirection = new Dictionary<Direction, Bitmap>();
	}
}