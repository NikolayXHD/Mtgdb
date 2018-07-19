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
			Kernel.BindConfig<UndoConfig>();

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
				.InSingletonScope();

			Kernel.Bind<TooltipController>()
				.ToSelf();

			Kernel.Bind<GuiLoader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormManager>()
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

			Kernel.Bind<DeckListAsnycUpdateSubsystem>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<UiModelSnapshotFactory>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}