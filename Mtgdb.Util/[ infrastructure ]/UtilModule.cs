using Ninject.Modules;

namespace Mtgdb.Util
{
	public class UtilModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.BindConfig<ForgeIntegrationConfig>();

			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<ForgeIntegration>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}
