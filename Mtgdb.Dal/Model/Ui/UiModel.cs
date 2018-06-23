using Ninject;

namespace Mtgdb.Dal
{
	public class UiModel
	{
		public UiModel(
			ImageLoader imageLoader,
			ImageRepository imageRepo,
			CardRepository cardRepo,
			[Optional] CollectionModel collection)
		{
			CardRepo = cardRepo;
			Collection = collection;
			ImageLoader = imageLoader;
			ImageRepo = imageRepo;

			LanguageController = new LanguageController(CardLocalization.DefaultLanguage);
		}

		public LanguageController LanguageController { get; }
		public CardRepository CardRepo { get; }

		public ICardCollection Collection { get; }
		public ImageLoader ImageLoader { get; }
		public ImageRepository ImageRepo { get; }

		public ICardCollection Deck { get; set; }
	}
}