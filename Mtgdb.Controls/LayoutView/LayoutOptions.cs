using System;
using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class LayoutOptions
	{
		public event Action Changed;

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

		private Size _cardInterval;
		private bool _allowPartialCards;
		private Size _partialCardsThreshold;
	}
}