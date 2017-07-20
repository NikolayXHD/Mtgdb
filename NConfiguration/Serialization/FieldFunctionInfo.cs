using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NConfiguration.Serialization
{
	internal sealed class FieldFunctionInfo
	{
		public Type ResultType { get; private set; }
		public string Name { get; private set; }
		public bool Required { get; private set; }
		public IDeserializerFactory DeserializerFactory { get; private set; }

		public FieldFunctionInfo(FieldInfo fi)
		{
			ResultType = fi.FieldType;
			Name = fi.Name;
			initByAttributes(fi);
		}

		public FieldFunctionInfo(PropertyInfo pi)
		{
			ResultType = pi.PropertyType;
			Name = pi.Name;
			initByAttributes(pi);
		}

		private void initByAttributes(MemberInfo mi)
		{
			DeserializerFactory = mi.GetCustomAttributes(false).OfType<IDeserializerFactory>().SingleOrDefault();

			var dmAttr = mi.GetCustomAttribute<DataMemberAttribute>();
			if (dmAttr == null)
				return;

			Required = dmAttr.IsRequired;
			if (string.IsNullOrWhiteSpace(dmAttr.Name))
				return;

			Name = dmAttr.Name;
		}
	}
}
