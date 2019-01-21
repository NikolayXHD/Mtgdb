using Ninject.Modules;

namespace Mtgdb.Util
{
	public class UtilModule : NinjectModule
	{
		public override void Load()
		{
			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<ImageExport>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}
