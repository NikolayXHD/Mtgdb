using System.IO;

namespace Mtgdb.Dal
{
	public class DownloaderPriceRepository : PriceRepository
	{
		public DownloaderPriceRepository()
		{
			PriceFileInProgress = AppDir.Data.AddPath("price.inprogress.json");
		}

		public override void Load()
		{
			Load(IdFile, PriceFileInProgress);
		}

		public void CommitProgress()
		{
			if (File.Exists(PriceFile))
				File.Delete(PriceFile);

			File.Move(PriceFileInProgress, PriceFile);
		}

		public FileStream AppendPriceInProgressStream()
		{
			var stream = new FileInfo(PriceFileInProgress).Open(FileMode.Append, FileAccess.Write, FileShare.None);
			return stream;
		}

		public FileStream AppendPriceIdStream()
		{
			var stream = new FileInfo(IdFile).Open(FileMode.Append, FileAccess.Write, FileShare.None);
			return stream;
		}

		protected readonly string PriceFileInProgress;
	}
}