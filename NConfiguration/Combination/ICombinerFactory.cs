using System;

namespace NConfiguration.Combination
{
	public interface ICombinerFactory
	{
		object CreateInstance(Type targetType);
	}
}
