using System;
using Ninject;

namespace Mtgdb.Dal
{
	public class UiModel
	{
		public UiModel(
			ImageCache imageCache,
			ImageRepository imageRepo,
			Lazy<CardRepository> cardRepoFactory,
			[Optional] Lazy<IUiForm> formFactory,
			[Optional] CollectionModel collection)
		{
			_formFactory = formFactory;
			_cardRepoFactory = cardRepoFactory;

			Collection = collection;
			ImageCache = imageCache;
			ImageRepo = imageRepo;
		}

		public IUiForm Form => _formFactory?.Value;
		public CardRepository CardRepo => _cardRepoFactory.Value;

		public ICardCollection Collection { get; }
		public ImageCache ImageCache { get; }
		public ImageRepository ImageRepo { get; }

		public ICardCollection Deck { get; set; }

		private readonly Lazy<IUiForm> _formFactory;
		private readonly Lazy<CardRepository> _cardRepoFactory;
	}
}