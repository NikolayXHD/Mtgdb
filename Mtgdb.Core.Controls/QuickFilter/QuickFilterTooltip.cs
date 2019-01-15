using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class QuickFilterTooltip : ICustomTooltip
	{
		public QuickFilterTooltip(QuickFilterControl control, Control owner)
		{
			_control = control;
			Owner = owner;
		}

		public bool PreferTooltipFromTop { get; set; }
		public bool PreferTooltipFromLeft { get; set; }
		public bool PreferHorizontalShift { get; set; }

		public void SubscribeEvents()
		{
			_control.MouseMove += mouseMove;
			_control.MouseLeave += mouseLeave;
		}

		public void UnsubscribeEvents()
		{
			_control.MouseMove -= mouseMove;
			_control.MouseLeave -= mouseLeave;
		}

		private void mouseLeave(object sender, EventArgs e) =>
			Hide?.Invoke();

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var command = _control.GetCommand(e.Location, MouseButtons.None);

			if (!command.HasValue)
			{
				Hide?.Invoke();
				return;
			}

			var property = _control.Properties[command.Value.ButtonIndex];

			if (string.IsNullOrEmpty(property))
			{
				Hide?.Invoke();
				return;
			}

			var bounds = _control.GetPaintingRectangle(command.Value.ButtonIndex, command.Value.ClickedState);

			var model = new TooltipModel
			{
				Id = "qf." + property,
				Control = _control,
				ObjectBounds = bounds,
				Cursor = bounds.Center(),
				Text = property,
				PositionPreference = getPositionPreference
			};

			Show?.Invoke(model);

			Func<ExtremumFinder<TooltipPosition>, ExtremumFinder<TooltipPosition>> getPositionPreference(Rectangle target)
			{
				Func<TooltipPosition, int> verticalDistanceFrom(Rectangle t) =>
					c =>
					{
						int dy = c.Bounds.CenterY() - t.CenterY();

						return PreferTooltipFromTop
							? Math.Abs((-dy).MultiplyIfNegative(1.5f))
							: Math.Abs(dy.MultiplyIfNegative(1.5f));
					};

				Func<TooltipPosition, int> horizontalDistanceFrom(Rectangle t) =>
					c =>
					{
						int dx = c.Bounds.CenterX() - t.CenterX();

						return PreferTooltipFromLeft
							? Math.Abs((-dx).MultiplyIfNegative(1.5f))
							: Math.Abs(dx.MultiplyIfNegative(1.5f));
					};

				if (PreferHorizontalShift)
					return _ => _
						.ThenAtMin(verticalDistanceFrom(target))
						.ThenAtMin(c => PreferTooltipFromLeft ? c.Bounds.X : -c.Bounds.X);
				else
					return _ => _
						.ThenAtMin(horizontalDistanceFrom(target))
						.ThenAtMin(c => PreferTooltipFromTop ? c.Bounds.Y : -c.Bounds.Y);
			}
		}

		public object Owner { get; }
		public event Action<TooltipModel> Show;
		public event Action Hide;

		private readonly QuickFilterControl _control;
	}
}