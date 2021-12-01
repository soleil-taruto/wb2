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

		/// <summary>
		/// 鍵の分割：
		/// --  16 --> 16
		/// --  24 --> 24
		/// --  32 --> 32
		/// --  40 --> 24, 16
		/// --  48 --> 32, 16
		/// --  56 --> 32, 24
		/// --  64 --> 32, 32
		/// --  72 --> 32, 24, 16
		/// --  80 --> 32, 32, 16
		/// --  88 --> 32, 32, 24
		/// --  96 --> 32, 32, 32
		/// -- 104 --> 32, 32, 24, 16
		/// -- 112 --> 32, 32, 32, 16
		/// -- 120 --> 32, 32, 32, 24
		/// -- 128 --> 32, 32, 32, 32
		/// -- ...
		/// </summary>
		/// <param name="rawKey">鍵</param>
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

		public void Dispose()
		{
			if (this.Transformers != null)
			{
				foreach (Camellia transformer in this.Transformers)
					transformer.Dispose();

				this.Transformers = null;
			}
		}

		/// <summary>
		/// 暗号化を行う。
		/// </summary>
		/// <param name="data">入力データ</param>
		/// <returns>出力データ</returns>
		public byte[] Encrypt(byte[] data)
		{
			if (data == null)
				throw new ArgumentException();

			data = AddPadding(data);
			data = AddCRandPart(data, 64);
			data = AddHash(data);
			data = AddCRandPart(data, 16);

			foreach (Camellia transformer in this.Transformers)
				EncryptRingCBC(data, transformer);

			return data;
		}

		/// <summary>
		/// 復号を行う。
		/// データの破損や鍵の不一致も含め復号に失敗すると例外を投げる。
		/// </summary>
		/// <param name="data">入力データ</param>
		/// <returns>出力データ</returns>
		public byte[] Decrypt(byte[] data)
		{
			if (
				data == null ||
				data.Length < 32 ||
				data.Length % 16 != 0
				)
				throw new ArgumentException();

			foreach (Camellia transformer in this.Transformers.Reverse())
				DecryptRingCBC(data, transformer);

			data = RemoveCRandPart(data, 16);
			data = RemoveHash(data);
			data = RemoveCRandPart(data, 64);
			data = RemovePadding(data);
			return data;
		}

		private static byte[] AddPadding(byte[] data)
		{
			int size = 16 - data.Length % 16;
			byte[] padding = SCommon.CRandom.GetBytes(size);
			size--;
			padding[size] &= 0xf0;
			padding[size] |= (byte)size;
			data = SCommon.Join(new byte[][] { data, padding });
			return data;
		}

		private static byte[] RemovePadding(byte[] data)
		{
			CheckLength(data, 1);
			int size = data[data.Length - 1] & 0x0f;
			size++;
			CheckLength(data, size);
			data = SCommon.GetSubBytes(data, 0, data.Length - size);
			return data;
		}

		private static byte[] AddCRandPart(byte[] data, int size)
		{
			byte[] padding = SCommon.CRandom.GetBytes(size);
			data = SCommon.Join(new byte[][] { data, padding });
			return data;
		}

		private static byte[] RemoveCRandPart(byte[] data, int size)
		{
			CheckLength(data, size);
			data = SCommon.GetSubBytes(data, 0, data.Length - size);
			return data;
		}

		private const int HASH_SIZE = 64;

		private static byte[] AddHash(byte[] data)
		{
			byte[] hash = SCommon.GetSHA512(data);
			if (hash.Length != HASH_SIZE) throw null; // 2bs
			data = SCommon.Join(new byte[][] { data, hash });
			return data;
		}

		private static byte[] RemoveHash(byte[] data)
		{
			CheckLength(data, HASH_SIZE);
			data = SCommon.GetSubBytes(data, 0, data.Length - HASH_SIZE);
			return data;
		}

		private static void EncryptRingCBC(byte[] data, Camellia transformer)
		{
			byte[] input = new byte[16];
			byte[] output = new byte[16];

			Array.Copy(data, data.Length - 16, output, 0, 16);

			for (int offset = 0; offset < data.Length; offset += 16)
			{
				Array.Copy(data, offset, input, 0, 16);
				XorBlock(input, output);
				transformer.EncryptBlock(input, output);
				Array.Copy(output, 0, data, offset, 16);
			}
		}

		private static void DecryptRingCBC(byte[] data, Camellia transformer)
		{
			byte[] input = new byte[16];
			byte[] output = new byte[16];

			Array.Copy(data, data.Length - 16, input, 0, 16);

			for (int offset = data.Length - 16; 0 <= offset; offset -= 16)
			{
				transformer.DecryptBlock(input, output);
				Array.Copy(data, (offset + data.Length - 16) % data.Length, input, 0, 16);
				XorBlock(output, input);
				Array.Copy(output, 0, data, offset, 16);
			}
		}

		private static void CheckLength(byte[] data, int minlen)
		{
			if (data.Length < minlen)
				throw new Exception("データ長不足");
		}

		private static void XorBlock(byte[] data, byte[] maskData)
		{
			for (int index = 0; index < 16; index++)
				data[index] ^= maskData[index];
		}
	}
}
