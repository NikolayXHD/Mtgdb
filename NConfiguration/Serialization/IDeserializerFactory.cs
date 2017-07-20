using System;

namespace NConfiguration.Serialization
{
	public interface IDeserializerFactory
	{
		object CreateInstance(Type targetType);
	}
}
