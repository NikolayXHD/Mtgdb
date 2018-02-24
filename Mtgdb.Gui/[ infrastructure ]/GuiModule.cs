using Mtgdb.Controls;
using Mtgdb.Dal;
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
				.ToSelf();

			Kernel.Bind<FormRoot>()
				.ToSelf();

			Kernel.BindFunc<FormRoot>();

			Kernel.Bind<TooltipForm>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<TooltipController>()
				.ToSelf();

			Kernel.Bind<GuiLoader>()
				.ToSelf()
				.InSingletonScope();

			Kernel.Bind<FormManager>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}