using JetBrains.Annotations;
using Ninject;

namespace Mtgdb.Dal
{
	public class UiModel
	{
		[UsedImplicitly]
		public UiModel(
			ImageLoader imageLoader,
			ImageRepository imageRepo,
			CardRepository cardRepo,
			[Optional, Named("collection")] ICardCollection collection)
		{
			CardRepo = cardRepo;
			Collection = collection;
			ImageLoader = imageLoader;
			ImageRepo = imageRepo;

			LanguageController = new LanguageController(CardLocalization.DefaultLanguage);
		}

		public UiModel(CardRepository repo, ICardCollection collection, ICardCollection deck)
		{
			CardRepo = repo;
			Collection = collection;
			Deck = deck;
		}

		public UiModel(CardRepository repo, ICardCollection collection)
		{
			CardRepo = repo;
			Collection = collection;
		}

		public LanguageController LanguageController { get; }
		public CardRepository CardRepo { get; }

		public ICardCollection Collection { get; }
		public ImageLoader ImageLoader { get; }
		public ImageRepository ImageRepo { get; }

		public ICardCollection Deck { get; set; }
	}
}