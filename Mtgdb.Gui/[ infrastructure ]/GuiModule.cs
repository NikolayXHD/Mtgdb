using System;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Ninject;
using Ninject.Activation;
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

			Kernel.Bind<SuggestModel>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormRoot>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<IUiForm>()
				.ToMethod(ctx=>ctx.Kernel.Get<FormRoot>())
				.InSingletonScope();

			Kernel.BindLazy<IUiForm>();

			Kernel.Bind<TooltipForm>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<TooltipController>()
				.ToSelf()
				.InSingletonScope();
		}

		private static Func<FormMain> formMainFactory(IContext ctx)
		{
			return () => ctx.Kernel.Get<FormMain>();
		}

		private static Func<FormRoot> formRootFactory(IContext ctx)
		{
			return () => ctx.Kernel.Get<FormRoot>();
		}
	}
}