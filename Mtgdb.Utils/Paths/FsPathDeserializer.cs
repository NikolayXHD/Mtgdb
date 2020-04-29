using NConfiguration;
using NConfiguration.Serialization;

namespace Mtgdb
{
	public class FsPathDeserializer: IDeserializer<FsPath>
	{
		public FsPath Deserialize(IDeserializer context, ICfgNode cfgNode)
		{
			string path = context.Deserialize<string>(cfgNode);
			return FsPathPersistence.Deserialize(path);
		}
	}
}
