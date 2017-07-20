namespace NConfiguration.Serialization
{
	public interface IDeserializer
	{
		T Deserialize<T>(IDeserializer context, ICfgNode cfgNode);
	}
}
