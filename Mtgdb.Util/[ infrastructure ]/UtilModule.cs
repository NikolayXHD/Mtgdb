using Mtgdb.Dev;
using Ninject.Modules;

namespace Mtgdb.Util
{
	public class UtilModule : NinjectModule
	{
		public override void Load()
		{
			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<ImageDirectorySigner>()
				.ToSelf()
				.InSingletonScope();

			FsPathPersistence.RegisterPathSubstitution(DevPaths.DriveSubstitution);
		}
	}
}
