using System;

namespace NConfiguration
{
	/// <summary>
	/// Subscriber may cancel the event the cause.
	/// </summary>
	public class CancelableEventArgs : EventArgs
	{
		/// <summary>
		/// The reason the event is canceled
		/// </summary>
		public bool Canceled { get; set; }
	}
}

