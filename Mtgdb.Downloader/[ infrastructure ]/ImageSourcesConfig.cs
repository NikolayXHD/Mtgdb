using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NConfiguration.Combination;

namespace Mtgdb.Downloader
{
	[DataContract(Name = "ImageSources")]
	public class ImageSourcesConfig : ICombinable<ImageSourcesConfig>
	{
		[DataMember(Name = "QualityGroup")]
		public QualityGroupConfig[] QualityGroups { get; set; }

		public void Combine(ICombiner combiner, ImageSourcesConfig other)
		{
			var resultGroups = new List<QualityGroupConfig>();

			var groupsByType = QualityGroups.ToDictionary(_ => _.Quality);
			var otherGroupsByType = other.QualityGroups.ToDictionary(_ => _.Quality);

			foreach (var entry in otherGroupsByType)
			{
				var otherGroup = entry.Value;

				if (groupsByType.TryGetValue(entry.Key, out var @group))
				{
					var combined = combiner.Combine(group, otherGroup);
					resultGroups.Add(combined);
				}
				else
					resultGroups.Add(otherGroup);
			}

			QualityGroups = resultGroups.ToArray();
		}
	}
}