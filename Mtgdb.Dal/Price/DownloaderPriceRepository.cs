using System.IO;

namespace Mtgdb.Dal
{
	public class DownloaderPriceRepository : PriceRepository
	{
		public override void Load()
		{
			Load(IdFile, PriceFileInProgress);
		}

		public void ResetPendingProgress()
		{
			if (File.Exists(PriceFileInProgress))
				File.Delete(PriceFileInProgress);
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
	}
}