using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using NConfiguration.Combination;

namespace Mtgdb.Downloader
{
	[DataContract(Name = "ImageSources")]
	public class ImageSourcesConfig : ICombinable<ImageSourcesConfig>
	{
		[DataMember(Name = "QualityGroup")]
		public QualityGroupConfig[] QualityGroups { get; set; }

		public string MegaPrefix { get; [UsedImplicitly] set; }

		public string GdrivePrefix { get; [UsedImplicitly] set; }

		public void Combine(ICombiner combiner, ImageSourcesConfig other)
		{
			var resultGroups = new List<QualityGroupConfig>();

			var groupsByType = QualityGroups.ToDictionary(_ => _.Name ?? _.Quality);
			var otherGroupsByType = other.QualityGroups.ToDictionary(_ => _.Name ?? _.Quality);

			foreach ((string name, QualityGroupConfig grp) in otherGroupsByType)
			{
				if (groupsByType.TryGetValue(name, out var group))
				{
					var combined = combiner.Combine(group, grp);
					resultGroups.Add(combined);
				}
				else
					resultGroups.Add(grp);
			}

			QualityGroups = resultGroups.ToArray();
		}
	}
}
