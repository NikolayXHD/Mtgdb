using System;
using System.Collections.Generic;
using System.Linq;

namespace NConfiguration.Combination
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class CombinerAttribute : Attribute, ICombinerFactory
	{
		public readonly ICollection<Type> CombinerTypes;

		public CombinerAttribute(params Type[] combinerTypes)
		{
			if (combinerTypes == null)
				throw new ArgumentNullException("combinerType");
			CombinerTypes = combinerTypes;
		}

		public virtual object CreateInstance(Type targetType)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");

			var requiredCombinerType = typeof(ICombiner<>).MakeGenericType(targetType);


			var candidate = CombinerTypes.SelectMany(ct => getVariants(ct, targetType)).FirstOrDefault(_ => requiredCombinerType.IsAssignableFrom(_));

			if(candidate == null)
				throw new InvalidOperationException(string.Format("supported combiner for type '{0}' not found", targetType.FullName));

			return Activator.CreateInstance(candidate);
		}

		private static Type tryMakeCombinerType(Type genericCombinerType, Type targetType)
		{
			try
			{
				return genericCombinerType.MakeGenericType(targetType);
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

		private static IEnumerable<Type> getVariantGenericArgument(Type targetType)
		{
			yield return targetType;

			if(targetType.IsGenericType)
			{
				var genArgs = targetType.GetGenericArguments();
				if (genArgs.Length == 1)
					yield return genArgs[0];
			}

			if (targetType.IsArray)
			{
				yield return targetType.GetElementType();
			}
		}

		private static IEnumerable<Type> getVariants(Type combinerType, Type targetType)
		{
			if(combinerType.IsGenericTypeDefinition)
			{
				foreach(var genArg in getVariantGenericArgument(targetType))
				{
					var candidate = tryMakeCombinerType(combinerType, genArg);
					if (candidate != null)
						yield return candidate;
				}
			}
			else
				yield return combinerType;
		}
	}
}
