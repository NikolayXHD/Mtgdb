using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization
{
	public interface IDeserializer
	{
		T Deserialize<T>(IDeserializer context, ICfgNode cfgNode);
	}
}
