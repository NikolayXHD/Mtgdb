using Ninject.Modules;

namespace Mtgdb.Util
{
	public class UtilModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.Bind<ImageDirectorySigner>()
				.ToSelf()
				.InSingletonScope();
		}
	}
}
