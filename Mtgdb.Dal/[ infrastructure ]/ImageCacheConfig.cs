using System.Runtime.Serialization;

namespace Mtgdb.Dal
{
	[DataContract(Name = "ImageCache")]
	public class ImageCacheConfig
	{
		[DataMember(Name = "CacheCapacity")]
		public int? CacheCapacity { get; set; }

		public int GetCacheCapacity()
		{
			return CacheCapacity ?? 500;
		}
	}
}