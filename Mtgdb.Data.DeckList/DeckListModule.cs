using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Ninject.Modules;

namespace Mtgdb.Data
{
	public class DeckListModule : NinjectModule
	{
		public override void Load()
		{
			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<DeckSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckSuggestModel>()
				.ToSelf();

			Kernel.Bind<DeckDocumentAdapter>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<CollectedCardsDeckTransformation>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckIndexUpdateSubsystem>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DeckListModel>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}