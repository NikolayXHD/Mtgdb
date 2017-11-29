using Ninject;

namespace Mtgdb.Dal
{
	public class UiModel
	{
		public UiModel(ImageCache imageCache, ImageRepository imageRepo, [Optional] CollectionModel collection)
		{
			ImageCache = imageCache;
			ImageRepo = imageRepo;
			Collection = collection;
		}

		public string Language
		{
			get { return Form?.Language ?? "en"; }
			set
			{
				if (Form != null)
					Form.Language = value;
			}
		}

		public bool HasLanguage => Form?.Language != null;

		public ICardCollection Deck { get; set; }
		public IUiForm Form { get; set; }

		public ICardCollection Collection { get; }
		public ImageCache ImageCache { get; }
		public CardRepository CardRepo { get; set; }
		public ImageRepository ImageRepo { get; }
	}
}