namespace NConfiguration.Serialization.Enums
{
	internal interface IEnumParser<T> where T: struct
	{
		T Parse(string text);
	}
}
