using NConfiguration;
using Ninject;
using Ninject.Activation;

namespace Mtgdb
{
	public static class NinjectExt
	{
		public static void BindConfig<TConfig>(this IKernel kernel)
		{
			kernel.Bind<TConfig>()
				.ToMethod(getConfig<TConfig>)
				.InSingletonScope();
		}

		private static TConfig getConfig<TConfig>(IContext ctx)
		{
			var appSettings = ctx.Kernel.Get<IAppSettings>();
			return appSettings.Get<TConfig>();
		}
	}
}