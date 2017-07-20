using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	/// <summary>
	/// Information about the properties of the readed file
	/// </summary>
	public class ReadedFileInfo
	{
		public static ReadedFileInfo Create(string fileName, Action<Stream> streamReader)
		{
			var fileInfo = new FileInfo(fileName);

			using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var md5 = MD5.Create())
			{
				using (var crStream = new CryptoStream(fileStream, md5, CryptoStreamMode.Read))
					streamReader(crStream);
				return new ReadedFileInfo(fileInfo, md5.Hash);
			}
		}

		private ReadedFileInfo(FileInfo fileInfo, byte[] hash)
		{
			_fileInfo = fileInfo;
			_length = _fileInfo.Length;
			_creation = _fileInfo.CreationTimeUtc;
			_lastWrite = _fileInfo.LastWriteTimeUtc;
			_fileAttributes = _fileInfo.Attributes;
			_hash = hash;
		}

		public string FullName
		{
			get
			{
				return _fileInfo.FullName;
			}
		}

		public bool WasChanged(bool checkAttributes)
		{
			try
			{
				_fileInfo.Refresh();
				if (!_fileInfo.Exists || _length != _fileInfo.Length)
					return true;

				return checkAttributes &&
					(_fileAttributes != _fileInfo.Attributes || _creation != _fileInfo.CreationTimeUtc || _lastWrite != _fileInfo.LastWriteTimeUtc);
			}
			catch (Exception)
			{
				return true;
			}
		}

		public bool WasHashChanged()
		{
			try
			{
				using (var fileStream = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var md5 = MD5.Create())
				{
					using (var crStream = new CryptoStream(fileStream, md5, CryptoStreamMode.Read))
						crStream.CopyTo(Stream.Null);

					return !_hash.SequenceEqual(md5.Hash);
				}
			}
			catch (Exception)
			{
				return true;
			}
		}

		private readonly FileInfo _fileInfo;
		private readonly DateTime _lastWrite;
		private readonly DateTime _creation;
		private readonly long _length;
		private readonly FileAttributes _fileAttributes;
		private readonly byte[] _hash;
	}
}
