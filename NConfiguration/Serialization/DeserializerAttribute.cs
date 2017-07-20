using System;
using System.Collections.Generic;

namespace NConfiguration.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DeserializerAttribute : Attribute, IDeserializerFactory
	{
		public readonly ICollection<Type> DeserializerTypes;

		public DeserializerAttribute(params Type[] deserializerTypes)
		{
			if (deserializerTypes == null)
				throw new ArgumentNullException("deserializerTypes");
			DeserializerTypes = deserializerTypes;
		}

		public virtual object CreateInstance(Type targetType)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");

			foreach (var candidate in DeserializerTypes)
			{
				Type deserializerType;
				try
				{
					deserializerType = candidate.MakeGenericType(targetType);
				}
				catch (InvalidOperationException)
				{
					deserializerType = candidate;
				}

				if (!typeof(IDeserializer<>).MakeGenericType(targetType).IsAssignableFrom(deserializerType))
					continue;

				return Activator.CreateInstance(deserializerType);
			}

			throw new InvalidOperationException("supported deserializer not found");
		}
	}
}
