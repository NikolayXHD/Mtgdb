using System;
using Mtgdb.Dal;
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

			Kernel.Bind<ImageDownloader>().ToSelf();
			Kernel.Bind<ImageDownloadProgressReader>().ToSelf();

			Kernel.Bind<PriceDownloader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<UpdateForm>().ToSelf();

			Func<UpdateForm> downloaderFormFactory = () => Kernel.Get<UpdateForm>();

			Kernel.Bind<Func<UpdateForm>>()
				.ToConstant(downloaderFormFactory)
				.InSingletonScope();
		}
	}
}