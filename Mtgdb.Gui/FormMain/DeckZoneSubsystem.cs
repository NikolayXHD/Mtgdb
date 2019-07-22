using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class DeckZoneSubsystem
	{
		public DeckZoneSubsystem(
			Control panelDeckTabsContainer,
			TabHeaderControl tabHeadersDeck,
			DraggingSubsystem dragging,
			LayoutViewControl viewDeck)
		{
			_panelDeckTabsContainer = panelDeckTabsContainer;
			_tabHeadersDeck = tabHeadersDeck;
			_dragging = dragging;
			_viewDeck = viewDeck;
		}

		public void HideSampleHand()
		{
			if (DeckZone == Zone.SampleHand)
				DeckZone = Zone.Main;
		}

		public void SubscribeEvents()
		{
			_tabHeadersDeck.SizeChanged += deckTabsResized;
			_tabHeadersDeck.DragOver += deckZoneDrag;
			_tabHeadersDeck.MouseMove += deckZoneHover;
			_dragging.DragRemoved += dragRemoved;
			_tabHeadersDeck.SelectedIndexChanging += changing;
		}

		public void UnsubscribeEvents()
		{
			_tabHeadersDeck.SizeChanged -= deckTabsResized;
			_tabHeadersDeck.DragOver -= deckZoneDrag;
			_tabHeadersDeck.MouseMove -= deckZoneHover;
			_dragging.DragRemoved -= dragRemoved;
			_tabHeadersDeck.SelectedIndexChanging -= changing;
		}



		private void deckTabsResized(object sender, EventArgs e)
		{
			_panelDeckTabsContainer.Size = new Size(
				Math.Max(_tabHeadersDeck.Width, _panelDeckTabsContainer.Width),
				_tabHeadersDeck.Height);
		}

		private void deckZoneDrag(object sender, DragEventArgs e)
		{
			var location = _tabHeadersDeck.PointToClient(new Point(e.X, e.Y));

			_tabHeadersDeck.GetTabIndex(location, out int hoveredIndex, out _);

			if (
				hoveredIndex < 0 ||
				hoveredIndex == _tabHeadersDeck.SelectedIndex ||
				hoveredIndex == (int)Zone.SampleHand ||
				hoveredIndex > MaxZoneIndex)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;
		}

		private void deckZoneHover(object sender, EventArgs e)
		{
			if (!_dragging.IsDragging || !_dragging.FromDeck)
				return;

			var hoveredIndex = _tabHeadersDeck.HoveredIndex;

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex ||
				hoveredIndex > MaxZoneIndex)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;
		}

		private void dragRemoved(Card c, Zone zone) =>
			// to undo unwanted deck zone switching by hovering deck zone tab
			// while dragging card from deck to search result
			DeckZone = zone;

		private void changing(TabHeaderControl s, int i)
		{
			var zone = DeckZone;
			if (zone.HasValue)
				_scrollByZone[zone.Value] = _viewDeck.CardIndex;
		}

		public Zone? DeckZone
		{
			get
			{
				int zoneIndex = _tabHeadersDeck.SelectedIndex;

				if (zoneIndex > MaxZoneIndex)
					return null;

				return (Zone) zoneIndex;
			}

			set
			{
				if (!value.HasValue)
					throw new NotSupportedException();
				_tabHeadersDeck.SelectedIndex = (int)value.Value;
			}
		}

		public int? GetLastScroll(Zone? zone)
		{
			if (zone.HasValue)
				return _scrollByZone.TryGet(zone.Value);
			return null;
		}

		public bool IsDeckListSelected =>
			_tabHeadersDeck.SelectedIndex == DeckListTabIndex;


		private const int MaxZoneIndex = (int) Zone.SampleHand;
		public const int DeckListTabIndex = MaxZoneIndex + 1;

		private readonly Control _panelDeckTabsContainer;
		private readonly TabHeaderControl _tabHeadersDeck;
		private readonly DraggingSubsystem _dragging;
		private readonly LayoutViewControl _viewDeck;

		private readonly Dictionary<Zone, int> _scrollByZone =
			new Dictionary<Zone, int>();
	}
}