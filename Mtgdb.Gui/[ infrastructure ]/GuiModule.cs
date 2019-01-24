using Mtgdb.Controls;
using Mtgdb.Dal;
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

			Kernel.Bind<DeckSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckSuggestModel>()
				.ToSelf();

			Kernel.Bind<DeckDocumentAdapter>()
				.ToSelf()
				.InSingletonScope();

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
				.WhenParentNamed(QuickFilterTooltipScope);

			Kernel.Bind<TooltipController>()
				.ToSelf()
				.Named(DefaultTooltipScope);

			Kernel.Bind<TooltipController>()
				.ToSelf()
				.InSingletonScope()
				.Named(QuickFilterTooltipScope);

			Kernel.Bind<TooltipConfiguration>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<GuiLoader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<Application>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<IApplication>()
				.ToMethod(ctx => ctx.Kernel.Get<Application>())
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

			Kernel.Bind<DeckListModel>()
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

			Kernel.Bind<CollectedCardsDeckTransformation>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckIndexUpdateSubsystem>()
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

		public const string DefaultTooltipScope = "default-tooltip";
		public const string QuickFilterTooltipScope = "quick-filter-tooltip";
	}
}