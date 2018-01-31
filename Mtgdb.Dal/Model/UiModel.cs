using System;
using Ninject;

namespace Mtgdb.Dal
{
	public class UiModel
	{
		public UiModel(
			ImageLoader imageLoader,
			ImageRepository imageRepo,
			Lazy<CardRepository> cardRepoFactory,
			[Optional] Lazy<IUiForm> formFactory,
			[Optional] CollectionModel collection)
		{
			_formFactory = formFactory;
			_cardRepoFactory = cardRepoFactory;

			Collection = collection;
			ImageLoader = imageLoader;
			ImageRepo = imageRepo;
		}

		public IUiForm Form => _formFactory?.Value;
		public CardRepository CardRepo => _cardRepoFactory.Value;

		public ICardCollection Collection { get; }
		public ImageLoader ImageLoader { get; }
		public ImageRepository ImageRepo { get; }

		public ICardCollection Deck { get; set; }

		private readonly Lazy<IUiForm> _formFactory;
		private readonly Lazy<CardRepository> _cardRepoFactory;

		public bool ShowTextualFields { get; set; }
		public bool ShowDeck { get; set; }
		public bool ShowPartialCards { get; set; }
	}
}