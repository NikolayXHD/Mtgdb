using System.IO;

namespace Mtgdb.Dal
{
	public class PriceDownloaderRepository : PriceRepository
	{
		public PriceDownloaderRepository()
		{
			PriceFileInProgress = AppDir.Data.AddPath("price.inprogress.json");
		}

		public override void Load()
		{
			if (!File.Exists(PriceFileInProgress))
			{
				if (!File.Exists(PriceFile))
					File.WriteAllBytes(PriceFileInProgress, new byte[0]);
				else
					File.Copy(PriceFile, PriceFileInProgress);
			}

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