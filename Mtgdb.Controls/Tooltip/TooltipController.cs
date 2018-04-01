using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class TooltipController
	{
		public event Action<TooltipModel> TooltipShown;
		public event Action<TooltipModel> TooltipHidden;

		public TooltipController(TooltipForm form)
		{
			HideCounter = 0;
			_thread = new Thread(updateTooltipLoop);
			_tooltipForm = form;
		}

		public void SetTooltip(string title, string tooltip, params Control[] controls)
		{
			SetTooltip(() => title, () => tooltip, controls);
		}

		public void SetTooltip(Func<string> title, Func<string> tooltip, params Control[] controls)
		{
			var settings = new StaticTooltipSettings
			{
				Text = tooltip,
				Title = title,
				Controls = controls
			};

			foreach (var control in controls)
				_staticTooltips.Add(control, settings);
		}

		public void SetCustomTooltip(ICustomTooltip client)
		{
			_customTooltips.Add(client);
		}



		public void SubscribeToEvents()
		{
			foreach (var control in _staticTooltips.Keys)
			{
				if (!_subscribed.Add(control))
					continue;

				control.MouseEnter += mouseEnter;
				control.MouseLeave += mouseLeave;
				control.GotFocus += gotFocus;
				control.MouseDown += gotFocus;
				control.KeyDown += gotFocus;
			}

			foreach (var customClient in _customTooltips)
			{
				if (!_subscribed.Add(customClient))
					continue;

				customClient.Show += customTooltipShow;
				customClient.Hide += customTooltipHide;
			}
		}

		public void StartThread()
		{
			_thread.Start();
		}

		public void AbortThread()
		{
			_thread.Abort();
		}

		public bool Active
		{
			get => _active;
			set
			{
				_active = value;

				if (!value)
					Tooltip = new TooltipModel();
			}
		}



		private void updateTooltipLoop()
		{
			try
			{
				_tooltip = Tooltip;

				while (true)
				{
					if (isTooltipUpdateSuspended())
					{
						Thread.Sleep(IntervalMs);
						continue;
					}

					var prev = _tooltip;
					if (prev.Id != null)
					{
						if (prev.Id.Equals(Tooltip.Id))
						{
							prev.Abandoned = null;
							Thread.Sleep(DelayMs + IntervalMs);
							continue;
						}

						prev.Abandoned = prev.Abandoned ?? DateTime.Now;

						int elapsedMs = (int) (DateTime.Now - prev.Abandoned.Value).TotalMilliseconds;
						if (elapsedMs < DelayMs)
						{
							Thread.Sleep(DelayMs - elapsedMs + IntervalMs);
							continue;
						}

						hide(prev);

						_tooltip = new TooltipModel();
					}

					var curr = Tooltip;

					if (curr.Id != null && !curr.Id.Equals(prev.Id))
					{
						int elapsedMs = (int) (DateTime.Now - curr.Created).TotalMilliseconds;
						if (elapsedMs < DelayMs)
						{
							Thread.Sleep(DelayMs - elapsedMs + IntervalMs);
							continue;
						}

						show(curr);

						_tooltip = curr;
					}

					Thread.Sleep(IntervalMs);
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private void show(TooltipModel curr)
		{
			curr.Control.Invoke(delegate
			{
				_tooltipForm.ShowTooltip(curr);
				ShowCounter++;
				TooltipShown?.Invoke(curr);
			});
		}

		private void hide(TooltipModel prev)
		{
			prev.Control.Invoke(delegate
			{
				_tooltipForm.HideTooltip();
				HideCounter++;
				TooltipHidden?.Invoke(prev);
			});
		}

		private bool isTooltipUpdateSuspended()
		{
			if (_tooltip == Tooltip)
				return true;

			if (!_tooltipForm.Clickable)
				return false;

			return _tooltipForm.Bounds.Contains(Cursor.Position) || _tooltipForm.UserInteracted;
		}



		private void mouseEnter(object sender, EventArgs e)
		{
			if (Active && Alt || !Active && !Alt)
				return;

			var control = (Control) sender;

			var settgins = _staticTooltips[control];

			var locationControl = settgins.Controls[0];
			if (locationControl == Tooltip?.Control)
				return;

			if (settgins.IsEmpty)
				Tooltip = new TooltipModel();
			else
				Tooltip = new TooltipModel
				{
					Id = locationControl,
					Control = locationControl,
					ObjectBounds = locationControl.ClientRectangle,

					Title = settgins.Title(),
					Text = settgins.Text()
				};
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			Tooltip = new TooltipModel();
		}

		private void gotFocus(object sender, EventArgs e)
		{
			Tooltip = new TooltipModel();
		}



		private void customTooltipShow(TooltipModel tooltip)
		{
			if (Active && Alt || !Active && !Alt)
				return;
			
			Tooltip = tooltip;
		}

		private void customTooltipHide()
		{
			Tooltip = new TooltipModel();
		}


		public int ShowCounter { get; private set; }
		public int HideCounter { get; private set; }

		private static bool Alt => Control.ModifierKeys == Keys.Alt;

		private TooltipModel Tooltip { get; set; } = new TooltipModel();


		private const int DelayMs = 150;
		private const int IntervalMs = 50;

		private readonly Dictionary<Control, StaticTooltipSettings> _staticTooltips = new Dictionary<Control, StaticTooltipSettings>();
		private readonly HashSet<ICustomTooltip> _customTooltips = new HashSet<ICustomTooltip>();

		private readonly HashSet<object> _subscribed = new HashSet<object>();

		private TooltipModel _tooltip;



		private readonly TooltipForm _tooltipForm;
		private bool _active = true;
		private readonly Thread _thread;
	}
}