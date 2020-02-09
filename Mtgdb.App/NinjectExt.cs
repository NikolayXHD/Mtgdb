using System;
using NConfiguration;
using Ninject;
using Ninject.Activation;
using Ninject.Syntax;

namespace Mtgdb
{
	public static class NinjectExt
	{
		public static void BindConfig<TConfig>(this IKernel kernel)
		{
			kernel.Bind<TConfig>()
				.ToMethod(ctx => ctx.Kernel.Get<IAppSettings>().Get<TConfig>())
				.InSingletonScope();
		}

		public static void BindFunc<TVal>(this IKernel kernel, string name = null) =>
			kernel.Bind<Func<TVal>>()
				.ToMethod(ctx => getMethod<TVal>(ctx, name))
				.InSingletonScope();

		public static IBindingNamedWithOrOnSyntax<TImpl> BindFunc<TBase, TImpl>(this IKernel kernel, string name = null)
			where TImpl : TBase =>
			kernel.Bind<TBase>()
				.ToMethod(getMethod<TImpl>(name))
				.InSingletonScope();

		public static IBindingNamedWithOrOnSyntax<TImpl> RebindFunc<TBase, TImpl>(this IKernel kernel, string name = null)
			where TImpl : TBase =>
			kernel.Rebind<TBase>()
				.ToMethod(getMethod<TImpl>(name))
				.InSingletonScope();

		private static Func<IContext, TImpl> getMethod<TImpl>(string name)
		{
			if (name == null)
				return ctx => ctx.Kernel.Get<TImpl>();

			return ctx => ctx.Kernel.Get<TImpl>(name);
		}

		private static Func<TImpl> getMethod<TImpl>(IContext ctx, string name)
		{
			if (name == null)
				return () => ctx.Kernel.Get<TImpl>();

			return () => ctx.Kernel.Get<TImpl>(name);
		}
	}
}
