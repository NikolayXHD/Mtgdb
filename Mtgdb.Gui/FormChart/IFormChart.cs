namespace Mtgdb.Gui
{
	public interface IFormChart
	{
		void BuildCustomChart(ReportSettings settings);
		ReportSettings ReadSettings();
		string Title { get; set; }
	}
}