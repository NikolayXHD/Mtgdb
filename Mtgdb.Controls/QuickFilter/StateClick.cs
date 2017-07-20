using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public struct StateClick : IEquatable<StateClick>
	{
		public readonly int ButtonIndex;
		public readonly FilterValueState ClickedState;
		public readonly MouseButtons MouseButton;

		public StateClick(int buttonIndex, FilterValueState clickedState, MouseButtons mouseButton)
			: this()
		{
			ButtonIndex = buttonIndex;
			ClickedState = clickedState;
			MouseButton = mouseButton;
		}
		
		public override string ToString()
		{
			return $"{ButtonIndex} {ClickedState}";
		}

		
		public bool Equals(StateClick other)
		{
			return ButtonIndex == other.ButtonIndex && ClickedState == other.ClickedState;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is StateClick && Equals((StateClick) obj);
		}

		public override int GetHashCode()
		{
			return ButtonIndex ^ (int) ClickedState;
		}

		public static bool operator ==(StateClick left, StateClick right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(StateClick left, StateClick right)
		{
			return !left.Equals(right);
		}
	}
}