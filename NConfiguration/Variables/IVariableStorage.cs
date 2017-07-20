namespace NConfiguration.Variables
{
	public interface IVariableStorage
	{
		string this[string name] { get; set; }
		ICfgNode CfgNodeConverter(string name, ICfgNode candidate);
	}
}