using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Charlotte.Commons;

namespace Charlotte.SubCommons
{
	public class FileCipher : IDisposable
	{
		public class AES : IDisposable
		{
			private AesManaged Aes;
			private ICryptoTransform Encryptor = null;
			private ICryptoTransform Decryptor = null;

			public AES(byte[] rawKey)
			{
				if (
					rawKey.Length != 16 &&
					rawKey.Length != 24 &&
					rawKey.Length != 32
					)
					throw new ArgumentException();

				this.Aes = new AesManaged();
				this.Aes.KeySize = rawKey.Length * 8;
				this.Aes.BlockSize = 128;
				this.Aes.Mode = CipherMode.ECB;
				this.Aes.IV = new byte[16]; // dummy
				this.Aes.Key = rawKey;
				this.Aes.Padding = PaddingMode.None;
			}

			public void EncryptBlock(byte[] input, byte[] output)
			{
				if (
					input.Length != 16 ||
					output.Length != 16
					)
					throw new ArgumentException();

				if (this.Encryptor == null)
					this.Encryptor = this.Aes.CreateEncryptor();

				this.Encryptor.TransformBlock(input, 0, 16, output, 0);
			}

			public void DecryptBlock(byte[] input, byte[] output)
			{
				if (
					input.Length != 16 ||
					output.Length != 16
					)
					throw new ArgumentException();

				if (this.Decryptor == null)
					this.Decryptor = this.Aes.CreateDecryptor();

				this.Decryptor.TransformBlock(input, 0, 16, output, 0);
			}

			public void Dispose()
			{
				if (this.Aes != null)
				{
					if (this.Encryptor != null)
						this.Encryptor.Dispose();

					if (this.Decryptor != null)
						this.Decryptor.Dispose();

					this.Aes.Dispose();
					this.Aes = null;
				}
			}
		}

		private AES[] Transformers;

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
		public FileCipher(byte[] rawKey)
		{
			if (
				rawKey == null ||
				rawKey.Length < 16 ||
				rawKey.Length % 8 != 0
				)
				throw new ArgumentException();

			List<AES> dest = new List<AES>();

			for (int offset = 0; offset < rawKey.Length; )
			{
				int size = rawKey.Length - offset;

				if (48 <= size)
					size = 32;
				else if (size == 40)
					size = 24;

				dest.Add(new AES(SCommon.GetSubBytes(rawKey, offset, size)));
				offset += size;
			}
			this.Transformers = dest.ToArray();
		}

		public void Dispose()
		{
			if (this.Transformers != null)
			{
				foreach (AES transformer in this.Transformers)
					transformer.Dispose();

				this.Transformers = null;
			}
		}

		/// <summary>
		/// 暗号化を行う。
		/// </summary>
		/// <param name="data">入出力ファイル</param>
		public void Encrypt(string file)
		{
			if (
				file == null ||
				!File.Exists(file)
				)
				throw new ArgumentException();

			AddPadding(file);
			AddCRandPart(file, 64);
			AddHash(file);
			AddCRandPart(file, 16);

			foreach (AES transformer in this.Transformers)
				EncryptRingCBC(file, transformer);
		}

		/// <summary>
		/// 復号を行う。
		/// データの破損や鍵の不一致も含め復号に失敗すると例外を投げる。
		/// -- このとき入出力ファイルの内容は中途半端な状態であることに注意すること。
		/// </summary>
		/// <param name="data">入出力ファイル</param>
		public void Decrypt(string file)
		{
			if (
				file == null ||
				!File.Exists(file)
				)
				throw new ArgumentException();

			long fileSize = new FileInfo(file).Length;

			if (
				fileSize < 16 + 64 + 64 + 16 || // ? AddPadding-したデータ_(最短)16 + cRandPart_64 + hash_64 + cRandPart_16 より短い
				fileSize % 16 != 0
				)
				throw new Exception("入力データの破損を検出しました。");

			foreach (AES transformer in this.Transformers.Reverse())
				DecryptRingCBC(file, transformer);

			RemoveCRandPart(file, 16);
			RemoveHash(file);
			RemoveCRandPart(file, 64);
			RemovePadding(file);
		}

		private static void AddPadding(string file)
		{
			long fileSize = new FileInfo(file).Length;
			int size = 16 - (int)(fileSize % 16);
			byte[] padding = SCommon.CRandom.GetBytes(size);
			size--;
			padding[size] &= 0xf0;
			padding[size] |= (byte)size;
			AppendBytes(file, padding);
		}

		private static void RemovePadding(string file)
		{
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				long fileSize = stream.Length;

				if (fileSize < 1)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.Seek(fileSize - 1, SeekOrigin.Begin);
				int size = stream.ReadByte() & 0x0f;
				size++;

				if (fileSize < size)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.SetLength(fileSize - size);
			}
		}

		private static void AddCRandPart(string file, int size)
		{
			byte[] padding = SCommon.CRandom.GetBytes(size);
			AppendBytes(file, padding);
		}

		private static void RemoveCRandPart(string file, int size)
		{
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				long fileSize = stream.Length;

				if (fileSize < size)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.SetLength(fileSize - size);
			}
		}

		private const int HASH_SIZE = 64;

		private static void AddHash(string file)
		{
			byte[] hash = GetSHA512ByFile(file);

			if (hash.Length != HASH_SIZE)
				throw null; // never

			AppendBytes(file, hash);
		}

		private static void RemoveHash(string file)
		{
			using (SHA512 sha512 = SHA512.Create())
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				long fileSize = stream.Length;

				if (fileSize < HASH_SIZE)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.Seek(fileSize - HASH_SIZE, SeekOrigin.Begin);
				byte[] hash = SCommon.Read(stream, HASH_SIZE);
				stream.Seek(0L, SeekOrigin.Begin);
				stream.SetLength(fileSize - HASH_SIZE);
				byte[] recalcHash = sha512.ComputeHash(stream);

				if (SCommon.Comp(hash, recalcHash) != 0)
					throw new Exception("入力データの破損または鍵の不一致を検出しました。");
			}
		}

		private static void EncryptRingCBC(string file, AES transformer)
		{
			byte[] input = new byte[16];
			byte[] output = new byte[16];

			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				long fileSize = stream.Length;

				if (
					fileSize < 32 ||
					fileSize % 16 != 0
					)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.Seek(fileSize - 16, SeekOrigin.Begin);
				SCommon.Read(stream, output);
				stream.Seek(0L, SeekOrigin.Begin);

				for (long offset = 0; offset < fileSize; offset += 16)
				{
					SCommon.Read(stream, input);
					XorBlock(input, output);
					transformer.EncryptBlock(input, output);
					stream.Seek(offset, SeekOrigin.Begin);
					SCommon.Write(stream, output);
				}
			}
		}

		private static void DecryptRingCBC(string file, AES transformer)
		{
			byte[] input = new byte[16];
			byte[] output = new byte[16];

			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				long fileSize = stream.Length;

				if (
					fileSize < 32 ||
					fileSize % 16 != 0
					)
					throw new Exception("Bad fileSize: " + fileSize);

				stream.Seek(fileSize - 16, SeekOrigin.Begin);
				SCommon.Read(stream, input);

				for (long offset = fileSize - 16; 0L <= offset; offset -= 16)
				{
					transformer.DecryptBlock(input, output);
					stream.Seek((offset + fileSize - 16) % fileSize, SeekOrigin.Begin);
					SCommon.Read(stream, input);
					XorBlock(output, input);

					if (offset == 0L)
						stream.Seek(0L, SeekOrigin.Begin);

					SCommon.Write(stream, output);
				}
			}
		}

		private static void XorBlock(byte[] data, byte[] maskData)
		{
			for (int index = 0; index < 16; index++)
				data[index] ^= maskData[index];
		}

		private static void AppendBytes(string file, byte[] data)
		{
			using (FileStream writer = new FileStream(file, FileMode.Append, FileAccess.Write))
			{
				SCommon.Write(writer, data);
			}
		}

		private static byte[] GetSHA512ByFile(string file)
		{
			using (SHA512 sha512 = SHA512.Create())
			using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				return sha512.ComputeHash(reader);
			}
		}
	}
}
