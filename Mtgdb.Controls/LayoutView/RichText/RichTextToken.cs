namespace Mtgdb.Controls
{
	public class RichTextToken
	{
		public RichTextToken()
		{
		}

		public RichTextToken(RichTextToken token, int index, int length)
		{
			Index = index;
			Length = length;
			Type = token.Type;
			IsHighlighted = token.IsHighlighted;
			IsContext = token.IsContext;
			IconName = token.IconName;
			IconNeedsShadow = token.IconNeedsShadow;
		}

		public int Index { get; set; }
		public int Length { get; set; }

		public int Right => Index + Length;

		public RichTextTokenType Type { get; set; }
		public bool IsHighlighted { get; set; }
		public bool IsContext { get; set; }
		public string IconName { get; set; }
		public bool IconNeedsShadow { get; set; }
	}
}