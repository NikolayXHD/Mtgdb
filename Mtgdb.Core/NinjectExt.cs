using System;
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

		public static void BindFunc<TVal>(this IKernel kernel)
		{
			kernel.Bind<Func<TVal>>()
				.ToMethod(ctx => () => ctx.Kernel.Get<TVal>())
				.InSingletonScope();
		}

		public static void BindLazy<TVal>(this IKernel kernel)
		{
			kernel.Bind<Lazy<TVal>>()
				.ToMethod(ctx => new Lazy<TVal>(() => ctx.Kernel.Get<TVal>()))
				.InSingletonScope();
		}

		private static TConfig getConfig<TConfig>(IContext ctx)
		{
			var appSettings = ctx.Kernel.Get<IAppSettings>();
			return appSettings.Get<TConfig>();
		}
	}
}