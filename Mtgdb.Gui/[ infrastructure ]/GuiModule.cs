using Mtgdb.Controls;
using Mtgdb.Dal;
using Ninject;
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
	}
}