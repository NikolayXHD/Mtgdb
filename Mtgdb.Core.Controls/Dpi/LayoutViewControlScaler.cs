using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public static class LayoutViewControlScaler
	{
		public static void ScaleDpi(
			this LayoutViewControl view,
			Func<Bitmap, Bitmap> transformIcons,
			Func<Bitmap, Bitmap> transformSearchIcon,
			Func<Bitmap, string, int, Bitmap> transformCustomButtonIcon)
		{
			_scaler.Setup(view);

			var fieldScaler = createFieldIconsScaler(
				transformSearchIcon,
				transformCustomButtonIcon);

			view.CardCreating += cardCreating;

			void cardCreating(object v, LayoutControl c)
			{
				foreach (var field in c.Fields)
				{
					field.ScaleDpiFont();
					fieldScaler.Setup(field);
				}

				c.ScaleDpi();
			}

			createViewOptionsScaler(transformIcons).Setup(view);

			_layoutResetter.Setup(view);
		}

		private static
			DpiScaler<FieldControl, ((string fieldName, IReadOnlyList<Bitmap> bitmaps), (string fieldName, IReadOnlyList<Bitmap> bitmaps), Bitmap, Bitmap)> createFieldIconsScaler(
				Func<Bitmap, Bitmap> transformSearchIcon,
				Func<Bitmap, string, int, Bitmap> transformCustomButtonIcon)
		{
			return DpiScalers.Combine(
				new DpiScaler<FieldControl, (string fieldName, IReadOnlyList<Bitmap> bitmaps)>(
					c => (c.FieldName, c.CustomButtons.Select(b => b.Icon).ToReadOnlyList()),
					(c, iconsInfo) => c.CustomButtons
						.Zip(iconsInfo.bitmaps, (btn, bmp) => (btn, bmp))
						.ForEach(_ => _.btn.Icon = _.bmp),
					customIconsScaler),
				new DpiScaler<FieldControl, (string fieldName, IReadOnlyList<Bitmap> bitmaps)>(
					c => (c.FieldName, c.CustomButtons.Select(b => b.IconTransp).ToReadOnlyList()),
					(c, iconsInfo) => c.CustomButtons
						.Zip(iconsInfo.bitmaps, (btn, bmp) => (btn, bmp))
						.ForEach(_ => _.btn.IconTransp = _.bmp),
					customIconsScaler),
				new DpiScaler<FieldControl, Bitmap>(
					f => f.SearchOptions.Button.Icon,
					(f, bmp) => f.SearchOptions.Button.Icon = bmp,
					bmp => bmp?.Invoke0(transformSearchIcon)),
				new DpiScaler<FieldControl, Bitmap>(
					f => f.SearchOptions.Button.IconTransp,
					(f, bmp) => f.SearchOptions.Button.IconTransp = bmp,
					bmp => bmp?.Invoke0(transformSearchIcon))
			);

			(string fieldName, IReadOnlyList<Bitmap> bitmaps) customIconsScaler((string fieldName, IReadOnlyList<Bitmap> bitmaps) _) =>
			(
				_.fieldName,
				_.bitmaps
					.Select((bmp, i) => bmp?.Invoke1(transformCustomButtonIcon, _.fieldName, i))
					.ToReadOnlyList()
			);
		}

		private static
			DpiScaler<LayoutViewControl,
				((Dictionary<Direction, Bitmap>, Dictionary<Direction, Bitmap>),
				(Bitmap, Bitmap, Bitmap),
				(Bitmap, Bitmap, Bitmap),
				(Bitmap, Bitmap))>
			createViewOptionsScaler(Func<Bitmap, Bitmap> transformIcons)
		{
			return DpiScalers.Combine(
				DpiScalers.Combine(
					new DpiScaler<LayoutViewControl, Dictionary<Direction, Bitmap>>(
						c => c.LayoutOptions.AlignmentIconsByDirection.ToDictionary(),
						(c, d) => c.LayoutOptions.AlignmentIconsByDirection = d,
						d => d.ToDictionary(_ => _.Key, _ => _.Value?.Invoke0(transformIcons))
					),
					new DpiScaler<LayoutViewControl, Dictionary<Direction, Bitmap>>(
						c => c.LayoutOptions.AlignmentHoveredIconsByDirection.ToDictionary(),
						(c, d) => c.LayoutOptions.AlignmentHoveredIconsByDirection = d,
						d => d.ToDictionary(_ => _.Key, _ => _.Value?.Invoke0(transformIcons))
					)),
				DpiScalers.Combine(
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.Icon,
						(c, bmp) => c.SortOptions.Icon = bmp,
						bmp => bmp?.Invoke0(transformIcons)),
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.AscIcon,
						(c, bmp) => c.SortOptions.AscIcon = bmp,
						bmp => bmp?.Invoke0(transformIcons)),
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.DescIcon,
						(c, bmp) => c.SortOptions.DescIcon = bmp,
						bmp => bmp?.Invoke0(transformIcons))
				),
				DpiScalers.Combine(
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.IconTransp,
						(c, bmp) => c.SortOptions.IconTransp = bmp,
						bmp => bmp?.Invoke0(transformIcons)),
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.AscIconTransp,
						(c, bmp) => c.SortOptions.AscIconTransp = bmp,
						bmp => bmp?.Invoke0(transformIcons)),
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SortOptions.DescIconTransp,
						(c, bmp) => c.SortOptions.DescIconTransp = bmp,
						bmp => bmp?.Invoke0(transformIcons))),
				DpiScalers.Combine(
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SearchOptions.Button.Icon,
						(c, bmp) => c.SearchOptions.Button.Icon = bmp,
						bmp => bmp?.Invoke0(transformIcons)),
					new DpiScaler<LayoutViewControl, Bitmap>(
						c => c.SearchOptions.Button.IconTransp,
						(c, bmp) => c.SearchOptions.Button.IconTransp = bmp,
						bmp => bmp?.Invoke0(transformIcons))
				)
			);
		}

		private static readonly DpiScaler<LayoutViewControl, Size> _thresholdScaler =
			new DpiScaler<LayoutViewControl, Size>(
				c => c.LayoutOptions.PartialCardsThreshold,
				(c, s) => c.LayoutOptions.PartialCardsThreshold = s,
				s => s.ByDpi());

		private static readonly DpiScaler<LayoutViewControl, Size> _intervalScaler =
			new DpiScaler<LayoutViewControl, Size>(
				c => c.LayoutOptions.CardInterval,
				(c, s) => c.LayoutOptions.CardInterval = s,
				s => s.ByDpi());

		private static readonly DpiScaler<LayoutViewControl, (Size, Size)> _scaler =
			DpiScalers.Combine(_thresholdScaler, _intervalScaler);

		private static readonly DpiScaler<LayoutViewControl> _layoutResetter =
			new DpiScaler<LayoutViewControl>(v => v.ResetLayout());
	}
}