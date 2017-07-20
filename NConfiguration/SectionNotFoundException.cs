using System;
namespace NConfiguration
{
	/// <summary>
	/// Section not found exception.
	/// </summary>
	public class SectionNotFoundException: ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NConfiguration.SectionNotFoundException"/> class.
		/// </summary>
		/// <param name='sectionName'>section name</param>
		/// <param name='configType'>configuration type</param>
		public SectionNotFoundException(string sectionName, Type configType)
			: base(string.Format("configuration section `{0}' (type {1}) not found", sectionName, configType.FullName))
		{
		}
	}
}

