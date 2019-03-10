using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Data.Model;
using Mtgdb.Ui;
using Ninject;
using Ninject.Modules;

namespace Mtgdb.Gui
{
	public class GuiModule : NinjectModule
	{
		public override void Load()
		{
			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<DownloaderSubsystem>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormMain>()
				.ToSelf();

			Kernel.BindFunc<FormMain>();

			Kernel.Bind<CardSuggestModel>()
				.ToSelf();

			Kernel.Bind<FormRoot>()
				.ToSelf();

			Kernel.BindFunc<FormRoot>();

			Kernel.Bind<TooltipForm>()
				.ToSelf()
				.InSingletonScope()
				.Named(DefaultTooltipScope)
				.WithConstructorArgument(EnableShadow.Yes);

			Kernel.Bind<TooltipForm>()
				.ToSelf()
				.InSingletonScope()
				.Named(QuickFilterTooltipScope)
				.WithConstructorArgument(EnableShadow.No);

			Kernel.Bind<TooltipForm>()
				.ToMethod(ctx => ctx.Kernel.Get<TooltipForm>(DefaultTooltipScope))
				.WhenParentNamed(DefaultTooltipScope);

			Kernel.Bind<TooltipForm>()
				.ToMethod(ctx => ctx.Kernel.Get<TooltipForm>(QuickFilterTooltipScope))
				.WhenParentNamed(UnspecifiedTooltipScope);

			Kernel.Bind<TooltipController>()
				.ToSelf()
				.Named(UnspecifiedTooltipScope);

			Kernel.Bind<TooltipController>()
				.ToSelf()
				.Named(DefaultTooltipScope);

			Kernel.Bind<TooltipController>()
				.ToMethod(ctx =>
				{
					var controller = ctx.Kernel.Get<TooltipController>(UnspecifiedTooltipScope);
					ctx.Kernel.Get<TooltipConfiguration>().SetupQuickFilterTooltipController(controller);
					return controller;
				})
				.Named(QuickFilterTooltipScope);

			Kernel.Bind<TooltipConfiguration>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<GuiLoader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<App>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<IApplication>()
				.ToMethod(ctx => ctx.Kernel.Get<App>())
				.InSingletonScope();

			Kernel.Bind<HistoryLegacyConverter>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckListLegacyConverter>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckMigrator>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<IconRecognizer>()
				.ToMethod(ctx => IconRecognizerFactory.Create())
				.InSingletonScope();

			Kernel.Bind<ICardCollection>()
				.ToMethod(ctx => ctx.Kernel.Get<CollectionEditorModel>())
				.Named("collection");

			Kernel.Bind<CollectionEditorModel>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckSerializationSubsystem>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<Loader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ColorSchemeController>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ColorSchemeEditor>()
				.ToSelf()
				.InSingletonScope();
		}

		private const string UnspecifiedTooltipScope = "unspecified";
		public const string DefaultTooltipScope = "default-tooltip";
		public const string QuickFilterTooltipScope = "quick-filter-tooltip";
	}
}