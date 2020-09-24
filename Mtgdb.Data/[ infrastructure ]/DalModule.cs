using Mtgdb.Data.Index;
using Ninject;
using Ninject.Modules;

namespace Mtgdb.Data
{
	public class DalModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.BindConfig<ImageLocationsConfig>();

			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<CardRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<PriceRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<KeywordSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ImageRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ImageLoader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<CardSearcher>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<ForgeSetRepository>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<UiModel>()
				.ToSelf();

			Kernel.Bind<CardDocumentAdapter>()
				.ToSelf()
				.InSingletonScope();

			Kernel.BindConfig<MtgaIntegrationConfig>();
			Kernel.Bind<MtgArenaIntegration>().ToSelf()
				.InSingletonScope();

			Kernel.Bind<UiConfigRepository>()
				.ToSelf()
				.InSingletonScope();

			if (!Kernel.CanResolve<IApplication>())
				Kernel.Bind<IApplication>()
					.To<ApplicationStub>()
					.InSingletonScope();

			Kernel.Bind<CardFormatter>()
				.ToSelf()
				.InSingletonScope();

			Kernel.BindFunc<IDataDownloader>();
		}
	}
}
