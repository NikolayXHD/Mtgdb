using System;
using Ninject;
using Ninject.Modules;

namespace Mtgdb.Downloader
{
	public class DownloaderModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.BindConfig<ImageSourcesConfig>();
			Kernel.BindConfig<AppSourceConfig>();
			Kernel.BindConfig<MtgjsonSourceConfig>();

			// ReSharper disable once PossibleNullReferenceException

			Kernel.Bind<Installer>().ToSelf()
				.InSingletonScope();

			Kernel.Bind<Megatools>().ToSelf();
			Kernel.Bind<ImageDownloader>().ToSelf();
			Kernel.Bind<ImageDownloadProgressReader>().ToSelf();

			Kernel.Bind<PriceDownloader>().ToSelf()
				.InSingletonScope();

			Kernel.Bind<NewsService>().ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormUpdate>().ToSelf();

			FormUpdate downloaderFormFactory() => Kernel.Get<FormUpdate>();

			Kernel.Bind<Func<FormUpdate>>()
				.ToConstant((Func<FormUpdate>) downloaderFormFactory)
				.InSingletonScope();
		}
	}
}