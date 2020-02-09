using Mtgdb.Data;
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

			Kernel.BindFunc<IDataDownloader, Installer>();

			Kernel.Bind<Megatools>().ToSelf();
			Kernel.Bind<ImageDownloader>().ToSelf();
			Kernel.Bind<ImageDownloadProgressReader>().ToSelf();

			Kernel.Bind<NewsService>().ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormUpdate>().ToSelf()
				.InSingletonScope();
			Kernel.BindFunc<FormUpdate>();
		}
	}
}
