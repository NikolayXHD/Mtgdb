using System;
using System.ComponentModel;

namespace Mtgdb.Controls
{
	public class DpiScaler<TComponent, TState>
		where TComponent: IComponent
	{
		public DpiScaler(
			Func<TComponent, TState> getter,
			Action<TComponent, TState> setter,
			Func<TState, TState> scaler)
		{
			Getter = getter;
			Setter = setter;
			Scaler = scaler;
		}

		public void Setup(TComponent control)
		{
			var originalState = Getter(control);

			updateState();

			Dpi.Changed += updateState;
			control.Disposed += unbind;

			void updateState()
			{
				var modifiedState = Scaler(originalState);
				Setter(control, modifiedState);
			}

			void unbind(object s, EventArgs e)
			{
				Dpi.Changed -= updateState;
				control.Disposed -= unbind;
			}
		}

		public Func<TComponent, TState> Getter { get; }
		public Action<TComponent, TState> Setter { get; }
		public Func<TState, TState> Scaler { get; }
	}

	public class DpiScaler<TComponent>
		where TComponent: IComponent
	{
		public DpiScaler(Action<TComponent> handler)
		{
			_handler = handler;
		}

		public void Setup(TComponent control)
		{
			updateState();

			Dpi.Changed += updateState;
			control.Disposed += unbind;

			void updateState() =>
				_handler(control);

			void unbind(object s, EventArgs e)
			{
				Dpi.Changed -= updateState;
				control.Disposed -= unbind;
			}
		}

		private readonly Action<TComponent> _handler;
	}
}