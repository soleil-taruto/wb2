using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Camellias
{
	public class RingCipher : IDisposable
	{
		private Camellia[] Transformers;

		public RingCipher(byte[] rawKey)
		{
			if (
				rawKey == null ||
				rawKey.Length < 16 ||
				rawKey.Length % 8 != 0
				)
				throw new ArgumentException();

			List<Camellia> dest = new List<Camellia>();

			for (int offset = 0; offset < rawKey.Length; )
			{
				int size = rawKey.Length - offset;

				if (48 <= size)
					size = 32;
				else if (size == 40)
					size = 24;

				byte[] subRawKey = new byte[size];
				Array.Copy(rawKey, offset, subRawKey, 0, size);
				Camellia transformer = new Camellia(subRawKey);
				dest.Add(transformer);
				offset += size;
			}
			this.Transformers = dest.ToArray();
		}

		public byte[] Encrypt(byte[] data)
		{
			if (data == null)
				throw new ArgumentException();

			data = AddPadding(data);
			data = AddCRandPart(data, 64);
			data = AddHash(data);
			data = AddCRandPart(data, 16);
			EncryptRingCBC(data, this.Transformers);
			return data;
		}

		public byte[] Decrypt(byte[] data)
		{
			if (
				data == null ||
				data.Length % 16 != 0
				)
				throw new ArgumentException();

			DecryptRingCBC(data, this.Transformers);
			data = RemoveCRandPart(data, 16);
			data = RemoveHash(data);
			data = RemoveCRandPart(data, 64);
			data = RemovePadding(data);
			return data;
		}

		private static byte[] AddPadding(byte[] data)
		{
			throw new NotImplementedException();
		}

		private static byte[] RemovePadding(byte[] data)
		{
			throw new NotImplementedException();
		}

		private static byte[] AddCRandPart(byte[] data, int size)
		{
			throw new NotImplementedException();
		}

		private static byte[] RemoveCRandPart(byte[] data, int size)
		{
			throw new NotImplementedException();
		}

		private static byte[] AddHash(byte[] data)
		{
			throw new NotImplementedException();
		}

		private static byte[] RemoveHash(byte[] data)
		{
			throw new NotImplementedException();
		}

		private static void EncryptRingCBC(byte[] data, Camellia[] transformers)
		{
			throw new NotImplementedException();
		}

		private static void DecryptRingCBC(byte[] data, Camellia[] transformers)
		{
			throw new NotImplementedException();
		}
	}
}
