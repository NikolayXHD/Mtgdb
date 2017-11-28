namespace Mtgdb.Controls
{
	public class RichTextToken
	{
		public int Index { get; set; }
		public int Length { get; set; }
		public RichTextTokenType Type { get; set; }
		public bool IsHighlighted { get; set; }
		public bool IsContext { get; set; }
		public string IconName { get; set; }
		public bool IconNeedsShadow { get; set; }
	}
}