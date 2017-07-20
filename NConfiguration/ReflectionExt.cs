using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NConfiguration
{
	public static class ReflectionExt
	{
		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute
		{
			var attributes = info.GetCustomAttributes(typeof (TAttribute), false)
				.OfType<TAttribute>();
				
			return attributes;
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute
		{
			var attribute = info.GetCustomAttributes<TAttribute>().FirstOrDefault();
			return attribute;
		}
	}
}