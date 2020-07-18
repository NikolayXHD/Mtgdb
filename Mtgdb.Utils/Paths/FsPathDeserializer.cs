using System;
using NConfiguration;
using NConfiguration.Serialization;

namespace Mtgdb
{
	public class FsPathDeserializer<TFake>: IDeserializer<FsPath>
	{
		public FsPathDeserializer()
		{
			if (typeof(TFake) != typeof(FsPath))
				throw new NotSupportedException();
		}

		public FsPath Deserialize(IDeserializer context, ICfgNode cfgNode)
		{
			string path = context.Deserialize<string>(cfgNode);
			return FsPathPersistence.Deserialize(path);
		}
	}
}
