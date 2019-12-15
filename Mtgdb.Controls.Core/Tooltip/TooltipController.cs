using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			_tooltipForm = form;
			_tooltipForm.Show();
		}

		public void SetTooltip(object owner, string title, string tooltip, params Control[] controls) =>
			SetTooltip(owner, () => title, () => tooltip, controls);

		public void SetTooltip(object owner, Func<string> title, Func<string> tooltip, params Control[] controls)
		{
			var settings = new StaticTooltipSettings
			{
				Text = tooltip,
				Title = title,
				Controls = controls,
				Owner = owner
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

				subscribeStaticTooltipEvents(control);
			}

			foreach (var customClient in _customTooltips)
			{
				if (!_subscribed.Add(customClient))
					continue;

				subscribeCustomTooltipEvents(customClient);
			}
		}

		private void subscribeStaticTooltipEvents(Control control)
		{
			control.MouseEnter += mouseEnter;
			control.MouseLeave += mouseLeave;
			control.GotFocus += gotFocus;
			control.MouseDown += gotFocus;
			control.KeyUp += gotFocus;
		}

		private void subscribeCustomTooltipEvents(ICustomTooltip customClient)
		{
			customClient.Show += customTooltipShow;
			customClient.Hide += customTooltipHide;
			customClient.SubscribeEvents();
		}



		public void UnsetTooltips(object owner)
		{
			var staticTooltipsToRemove = _staticTooltips
				.Where(_ => _.Value.Owner == owner)
				.Select(_=>_.Key)
				.ToArray();

			foreach (var control in staticTooltipsToRemove)
			{
				unsubscribeStaticTooltipEvents(control);
				_staticTooltips.Remove(control);
			}

			var customTooltipsToRemove = _customTooltips
				.Where(_ => _.Owner == owner)
				.ToArray();

			foreach (var customTooltip in customTooltipsToRemove)
			{
				unsubscribeCustomTooltipEvents(customTooltip);
				_customTooltips.Remove(customTooltip);
			}
		}

		private void unsubscribeStaticTooltipEvents(Control control)
		{
			control.MouseEnter -= mouseEnter;
			control.MouseLeave -= mouseLeave;
			control.GotFocus -= gotFocus;
			control.MouseDown -= gotFocus;
			control.KeyUp -= gotFocus;
		}

		private void unsubscribeCustomTooltipEvents(ICustomTooltip customClient)
		{
			customClient.Show -= customTooltipShow;
			customClient.Hide -= customTooltipHide;
			customClient.UnsubscribeEvents();
		}



		public void StartThread()
		{
			if (_cts != null && !_cts.IsCancellationRequested)
				throw new InvalidOperationException("Already started");

			var cts = new CancellationTokenSource();
			Task.Run( () => updateTooltipLoop(cts.Token), cts.Token);

			_cts = cts;
		}

		public void AbortThread() =>
			_cts?.Cancel();

		public bool Active
		{
			get => _active;
			set
			{
				_active = value;

				if (!value)
					Tooltip = _emptyTooltip;
			}
		}



		private async Task updateTooltipLoop(CancellationToken token)
		{
			_tooltip = Tooltip;

			while (!token.IsCancellationRequested)
			{
				if (isTooltipUpdateSuspended())
				{
					await Task.Delay(IntervalMs, token);
					continue;
				}

				var prev = _tooltip;
				if (prev.Id != null)
				{
					if (prev.Id.Equals(Tooltip.Id))
					{
						prev.Abandoned = null;
						await Task.Delay(DelayMs + IntervalMs, token);
						continue;
					}

					prev.Abandoned ??= DateTime.Now;

					int elapsedMs = (int) (DateTime.Now - prev.Abandoned.Value).TotalMilliseconds;
					if (elapsedMs < DelayMs)
					{
						await Task.Delay(DelayMs - elapsedMs + IntervalMs, token);
						continue;
					}

					hide(prev);

					_tooltip = _emptyTooltip;
				}

				var curr = Tooltip;

				if (curr.Id != null && !curr.Id.Equals(prev.Id))
				{
					int elapsedMs = (int) (DateTime.Now - curr.Created).TotalMilliseconds;
					if (elapsedMs < DelayMs)
					{
						await Task.Delay(DelayMs - elapsedMs + IntervalMs, token);
						continue;
					}

					show(curr);

					_tooltip = curr;
				}

				await Task.Delay(IntervalMs, token);
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
			if (!IsActive)
				return;

			var control = (Control) sender;

			var settings = _staticTooltips[control];

			if (control == Tooltip.Control)
				return;

			if (settings.IsEmpty)
				Tooltip = _emptyTooltip;
			else
				Tooltip = new TooltipModel
				{
					Id = settings.Controls[0],
					Control = control,
					ObjectBounds = control.ClientRectangle,

					Title = settings.Title(),
					Text = settings.Text()
				};
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			Tooltip = _emptyTooltip;
		}

		private void gotFocus(object sender, EventArgs e)
		{
			Tooltip = _emptyTooltip;
		}



		private void customTooltipShow(TooltipModel tooltip)
		{
			if (!IsActive)
				return;

			Tooltip = tooltip;
		}

		private void customTooltipHide()
		{
			Tooltip = _emptyTooltip;
		}


		public int ShowCounter { get; private set; }
		public int HideCounter { get; private set; }

		private bool IsActive => Active != Alt;
		private bool Alt => ToggleOnAlt && (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.Control);

		public bool ToggleOnAlt { get; set; } = true;

		private TooltipModel Tooltip { get; set; } = _emptyTooltip;


		public int DelayMs { get; set; } = 150;
		public int IntervalMs { get; set; } = 50;

		private readonly Dictionary<Control, StaticTooltipSettings> _staticTooltips = new Dictionary<Control, StaticTooltipSettings>();
		private readonly HashSet<ICustomTooltip> _customTooltips = new HashSet<ICustomTooltip>();

		private readonly HashSet<object> _subscribed = new HashSet<object>();

		private TooltipModel _tooltip;

		private readonly TooltipForm _tooltipForm;
		private bool _active = true;

		private static readonly TooltipModel _emptyTooltip = new TooltipModel();

		private CancellationTokenSource _cts;
	}
}
