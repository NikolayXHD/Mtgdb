using System.ComponentModel;

namespace Mtgdb.Controls
{
	public static class Scalers
	{
		public static DpiScaler<TComponent, (TState1, TState2)> Combine<TComponent, TState1, TState2>(
			DpiScaler<TComponent, TState1> scaler1,
			DpiScaler<TComponent, TState2> scaler2)
			where TComponent: IComponent
		{
			return new DpiScaler<TComponent, (TState1, TState2)>(
				F.Combine(scaler1.Getter, scaler2.Getter),
				F.Combine(scaler1.Setter, scaler2.Setter),
				F.Combine(scaler1.Scaler, scaler2.Scaler));
		}

		public static DpiScaler<TComponent, (TState1, TState2, TState3)> Combine<TComponent, TState1, TState2, TState3>(
			DpiScaler<TComponent, TState1> scaler1,
			DpiScaler<TComponent, TState2> scaler2,
			DpiScaler<TComponent, TState3> scaler3)
			where TComponent: IComponent
		{
			return new DpiScaler<TComponent, (TState1, TState2, TState3)>(
				F.Combine(scaler1.Getter, scaler2.Getter, scaler3.Getter),
				F.Combine(scaler1.Setter, scaler2.Setter, scaler3.Setter),
				F.Combine(scaler1.Scaler, scaler2.Scaler, scaler3.Scaler));
		}

		public static DpiScaler<TComponent, (TState1, TState2, TState3, TState4)> Combine<TComponent, TState1, TState2, TState3, TState4>(
			DpiScaler<TComponent, TState1> scaler1,
			DpiScaler<TComponent, TState2> scaler2,
			DpiScaler<TComponent, TState3> scaler3,
			DpiScaler<TComponent, TState4> scaler4)
			where TComponent: IComponent
		{
			return new DpiScaler<TComponent, (TState1, TState2, TState3, TState4)>(
				F.Combine(scaler1.Getter, scaler2.Getter, scaler3.Getter, scaler4.Getter),
				F.Combine(scaler1.Setter, scaler2.Setter, scaler3.Setter, scaler4.Setter),
				F.Combine(scaler1.Scaler, scaler2.Scaler, scaler3.Scaler, scaler4.Scaler));
		}

		public static DpiScaler<TComponent, (TState1, TState2, TState3, TState4, TState5)> Combine<TComponent, TState1, TState2, TState3, TState4, TState5>(
			DpiScaler<TComponent, TState1> scaler1,
			DpiScaler<TComponent, TState2> scaler2,
			DpiScaler<TComponent, TState3> scaler3,
			DpiScaler<TComponent, TState4> scaler4,
			DpiScaler<TComponent, TState5> scaler5)
			where TComponent: IComponent
		{
			return new DpiScaler<TComponent, (TState1, TState2, TState3, TState4, TState5)>(
				F.Combine(scaler1.Getter, scaler2.Getter, scaler3.Getter, scaler4.Getter, scaler5.Getter),
				F.Combine(scaler1.Setter, scaler2.Setter, scaler3.Setter, scaler4.Setter, scaler5.Setter),
				F.Combine(scaler1.Scaler, scaler2.Scaler, scaler3.Scaler, scaler4.Scaler, scaler5.Scaler));
		}
	}
}