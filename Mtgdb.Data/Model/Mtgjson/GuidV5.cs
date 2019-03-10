using System;
using System.Security.Cryptography;
using System.Text;

namespace Mtgdb.Data
{
	public static class GuidV5
	{
		public static string Base64(string name)
		{
			var newGuid = createBytes(DnsNamespace, name);
			// convert the resulting UUID to local byte order (step 13)
			swapByteOrder(newGuid);
			return Convert.ToBase64String(newGuid);
		}

		private static byte[] createBytes(Guid namespaceId, string name)
		{
			const int version = 5;

			// convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
			// ASSUME: UTF-8 encoding is always appropriate
			byte[] nameBytes = Encoding.UTF8.GetBytes(name);

			// convert the namespace UUID to network order (step 3)
			byte[] namespaceBytes = namespaceId.ToByteArray();
			swapByteOrder(namespaceBytes);

			// compute the hash of the name space ID concatenated with the name (step 4)
			byte[] hash;
			using (HashAlgorithm algorithm = SHA1.Create())
			{
				algorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
				algorithm.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
				hash = algorithm.Hash;
			}

			// most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
			byte[] newGuid = new byte[16];
			System.Array.Copy(hash, 0, newGuid, 0, 16);

			// set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
			newGuid[6] = (byte) ((newGuid[6] & 0x0F) | (version << 4));

			// set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
			newGuid[8] = (byte) ((newGuid[8] & 0x3F) | 0x80);
			return newGuid;
		}

		/// <summary>
		/// The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

		/// <summary>
		/// The namespace for URLs (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid UrlNamespace = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

		/// <summary>
		/// The namespace for ISO OIDs (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid IsoOidNamespace = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

		// Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
		private static void swapByteOrder(byte[] guid)
		{
			swapBytes(guid, 0, 3);
			swapBytes(guid, 1, 2);
			swapBytes(guid, 4, 5);
			swapBytes(guid, 6, 7);
		}

		private static void swapBytes(byte[] guid, int left, int right)
		{
			byte temp = guid[left];
			guid[left] = guid[right];
			guid[right] = temp;
		}
	}
}