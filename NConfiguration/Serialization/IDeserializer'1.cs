namespace NConfiguration.Serialization
{
	public interface IDeserializer<T>
	{
		T Deserialize(IDeserializer context, ICfgNode cfgNode);
	}
}
