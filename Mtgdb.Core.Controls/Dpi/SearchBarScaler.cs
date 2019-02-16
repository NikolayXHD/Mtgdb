namespace Mtgdb.Controls
{
	public static class SearchBarScaler
	{
		public static void ScaleDpi(this SearchBar searchBar)
		{
			DropDownBaseScaler.ScaleDpi(searchBar);
			searchBar.Input.ScaleDpiFont();
		}
	}
}