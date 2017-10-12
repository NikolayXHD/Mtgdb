using System.Linq;
using Mtgdb.Dal.Index;
using Ninject.Modules;

namespace Mtgdb.Dal
{
	public class DalModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.BindConfig<ImageLocationsConfig>();
			Kernel.BindConfig<ImageCacheConfig>();
			Kernel.BindConfig<SuggestImageDownloaderConfig>();

			// ReSharper disable once PossibleNullReferenceException

			Kernel.Bind<Loader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<UiModel>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<CardRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<KeywordSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<LocalizationRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ImageRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ImageCache>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<LuceneSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<PriceRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<DownloaderPriceRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ForgeSetRepository>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}
