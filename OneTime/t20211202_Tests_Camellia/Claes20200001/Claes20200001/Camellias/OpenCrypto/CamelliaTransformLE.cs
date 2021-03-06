// 
// Copyright (c) 2006, Kazuki Oikawa
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Security.Cryptography;

namespace Charlotte.Camellias.OpenCrypto
{
	/// <summary>
	/// <ja>Camelliaの32bit-LittleEndian実装</ja>
	/// </summary>
	internal sealed class CamelliaTransformLE : CamelliaTransform
	{
		public unsafe CamelliaTransformLE(SymmetricAlgorithmPlus algo, byte[] rgbKey, byte[] rgbIV, bool encrypt)
			: base(algo, encrypt, rgbKey.Length == 16 ? 2 : 3, rgbIV, SBOX1_1110, SBOX2_0222, SBOX3_3033, SBOX4_4404)
		{
			fixed (byte* pKey = rgbKey)
			{
				GenerateKeyTable((uint*)pKey, rgbKey.Length, _keyTable);
			}
		}

		static uint LeftShift(uint value, int shift)
		{
			int bit = shift & 7;
			value >>= (shift - bit);

			for (int i = 0; i < bit; i++)
				value = ((value << 1) & 0xfefefefe) | (((value >> 15) | (value << 17)) & 0x10101);

			return (value << bit) >> bit;
		}

		static uint RightShift(uint value, int shift)
		{
			int bit = shift & 7;
			value <<= shift - bit;

			for (int i = 0; i < bit; i++)
				value = ((value >> 1) & 0x7f7f7f7f) | (((value << 15) | (value >> 17)) & 0x80808000);

			return value;
		}

		static unsafe void RotBlock(uint* x, int n, uint* y)
		{
			int r = (n & 31);   /* Must not be 0 */
			int idx = (n >> 5);
			int idx1 = (idx + 1) & 3;
			int idx2 = (idx1 + 1) & 3;

			y[0] = LeftShift(x[idx], r) | RightShift(x[idx1], 32 - r);
			y[1] = LeftShift(x[idx1], r) | RightShift(x[idx2], 32 - r);
		}

		static unsafe void GenerateKeyTable(uint* pKey, int keyLen, uint[] keyTable)
		{
			uint s1, s2, U, D;
			uint* t = stackalloc uint[16];
			fixed (uint* pTable = keyTable)
			{
				switch (keyLen)
				{
					case 16: /* 128bit */
						t[0] = pKey[0]; t[1] = pKey[1];
						t[2] = pKey[2]; t[3] = pKey[3];
						t[4] = t[5] = t[6] = t[7] = 0;
						break;
					case 24: /* 192bit */
						t[0] = pKey[0]; t[1] = pKey[1];
						t[2] = pKey[2]; t[3] = pKey[3];
						t[4] = pKey[4]; t[5] = pKey[5];
						t[6] = ~pKey[4]; t[7] = ~pKey[5];
						break;
					case 32: /* 256bit */
						t[0] = pKey[0]; t[1] = pKey[1];
						t[2] = pKey[2]; t[3] = pKey[3];
						t[4] = pKey[4]; t[5] = pKey[5];
						t[6] = pKey[6]; t[7] = pKey[7];
						break;
					default:
						throw new NotSupportedException();
				}

				t[8] = t[0] ^ t[4]; t[9] = t[1] ^ t[5];
				t[10] = t[2] ^ t[6]; t[11] = t[3] ^ t[7];
				s1 = t[8] ^ 0x7f669ea0; s2 = t[9] ^ 0x8b90cc3b;
				U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
				D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
				t[10] ^= D ^ U; t[11] ^= D ^ U ^ ((U << 8) | (U >> 24));
				s1 = t[10] ^ 0x58e87ab6; s2 = t[11] ^ 0xb273aa4c;
				U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
				D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
				t[8] ^= D ^ U; t[9] ^= D ^ U ^ ((U << 8) | (U >> 24));
				t[8] ^= t[0]; t[9] ^= t[1];
				t[10] ^= t[2]; t[11] ^= t[3];
				s1 = t[8] ^ 0x2f37efc6; s2 = t[9] ^ 0xbe824fe9;
				U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
				D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
				t[10] ^= D ^ U; t[11] ^= D ^ U ^ ((U << 8) | (U >> 24));
				s1 = t[10] ^ 0xa553ff54; s2 = t[11] ^ 0x1c6fd3f1;
				U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
				D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
				t[8] ^= D ^ U; t[9] ^= D ^ U ^ ((U << 8) | (U >> 24));

				/* Fill the keyTable. Requires many block rotations. */
				if (keyLen == 16)
				{
					pTable[0] = t[0]; pTable[1] = t[1];
					pTable[2] = t[2]; pTable[3] = t[3];
					pTable[4] = t[8]; pTable[5] = t[9];
					pTable[6] = t[10]; pTable[7] = t[11];
					for (int i = 4; i < 26; i += 2)
					{
						RotBlock(t + KIDX1[i + 0], KSFT1[i + 0], pTable + i * 2);
						RotBlock(t + KIDX1[i + 1], KSFT1[i + 1], pTable + i * 2 + 2);
					}
				}
				else
				{
					t[12] = t[8] ^ t[4]; t[13] = t[9] ^ t[5];
					t[14] = t[10] ^ t[6]; t[15] = t[11] ^ t[7];
					s1 = t[12] ^ 0xfa27e510; s2 = t[13] ^ 0x1d2d68de;
					U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
					D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
					t[14] ^= D ^ U; t[15] ^= D ^ U ^ ((U << 8) | (U >> 24));
					s1 = t[14] ^ 0xc28856b0; s2 = t[15] ^ 0xfdc1e6b3;
					U = SBOX1_1110[(byte)s1] ^ SBOX2_0222[(byte)(s1 >> 8)] ^ SBOX3_3033[(byte)(s1 >> 16)] ^ SBOX4_4404[(byte)(s1 >> 24)];
					D = SBOX2_0222[(byte)s2] ^ SBOX3_3033[(byte)(s2 >> 8)] ^ SBOX4_4404[(byte)(s2 >> 16)] ^ SBOX1_1110[(byte)(s2 >> 24)];
					t[12] ^= D ^ U; t[13] ^= D ^ U ^ ((U << 8) | (U >> 24));
					pTable[0] = t[0]; pTable[1] = t[1]; pTable[2] = t[2]; pTable[3] = t[3];
					pTable[4] = t[12]; pTable[5] = t[13]; pTable[6] = t[14]; pTable[7] = t[15];
					for (int i = 4; i < 34; i += 2)
					{
						RotBlock(t + KIDX2[i + 0], KSFT2[i + 0], (uint*)(pTable + i * 2));
						RotBlock(t + KIDX2[i + 1], KSFT2[i + 1], (uint*)(pTable + i * 2 + 2));
					}
				}
			}
		}

		protected override unsafe void EncryptBlock(uint* k, uint* sbox1, uint* sbox2, uint* sbox3, uint* sbox4, uint* plaintext, uint* ciphertext)
		{
			uint x0 = plaintext[0] ^ k[0];
			uint x1 = plaintext[1] ^ k[1];
			uint x2 = plaintext[2] ^ k[2];
			uint x3 = plaintext[3] ^ k[3];

			for (int i = 0; ; i++)
			{
				uint s1 = x0 ^ k[4], s2 = x1 ^ k[5];
				uint U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				uint D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[6];
				s2 = x3 ^ k[7];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x0 ^ k[8];
				s2 = x1 ^ k[9];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[10];
				s2 = x3 ^ k[11];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x0 ^ k[12];
				s2 = x1 ^ k[13];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[14];
				s2 = x3 ^ k[15];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				if (i == _flayerLimit) break;

				k += 16;
				U = x0 & k[0];
				x1 ^= ((U << 1) & 0xfefefefe) | (((U >> 15) | (U << 17)) & ~0xfefefefe);
				x0 ^= x1 | k[1];
				x2 ^= x3 | k[3];
				U = x2 & k[2];
				x3 ^= ((U << 1) & 0xfefefefe) | (((U >> 15) | (U << 17)) & ~0xfefefefe);
			}

			ciphertext[0] = k[16] ^ x2;
			ciphertext[1] = k[17] ^ x3;
			ciphertext[2] = k[18] ^ x0;
			ciphertext[3] = k[19] ^ x1;
		}

		protected override unsafe void DecryptBlock(uint* k, uint* sbox1, uint* sbox2, uint* sbox3, uint* sbox4, uint* ciphertext, uint* plaintext)
		{
			k += (_flayerLimit == 2 ? 46 : 62);
			uint x0 = ciphertext[0] ^ k[2];
			uint x1 = ciphertext[1] ^ k[3];
			uint x2 = ciphertext[2] ^ k[4];
			uint x3 = ciphertext[3] ^ k[5];

			for (int i = 0; ; i++)
			{
				uint s1 = x0 ^ k[0], s2 = x1 ^ k[1];
				uint U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				uint D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[-2];
				s2 = x3 ^ k[-1];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x0 ^ k[-4];
				s2 = x1 ^ k[-3];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[-6];
				s2 = x3 ^ k[-5];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x0 ^ k[-8];
				s2 = x1 ^ k[-7];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x2 ^= D ^ U;
				x3 ^= D ^ U ^ ((U << 8) | (U >> 24));

				s1 = x2 ^ k[-10];
				s2 = x3 ^ k[-9];
				U = sbox1[(byte)s1] ^ sbox2[(byte)(s1 >> 8)] ^ sbox3[(byte)(s1 >> 16)] ^ sbox4[(byte)(s1 >> 24)];
				D = sbox2[(byte)s2] ^ sbox3[(byte)(s2 >> 8)] ^ sbox4[(byte)(s2 >> 16)] ^ sbox1[(byte)(s2 >> 24)];
				x0 ^= D ^ U;
				x1 ^= D ^ U ^ ((U << 8) | (U >> 24));

				if (i == _flayerLimit) break;
				k -= 16;
				U = x0 & k[4];
				x1 ^= ((U << 1) & 0xfefefefe) | (((U >> 15) | (U << 17)) & ~0xfefefefe);
				x0 ^= x1 | k[5];
				x2 ^= x3 | k[3];
				U = x2 & k[2];
				x3 ^= ((U << 1) & 0xfefefefe) | (((U >> 15) | (U << 17)) & ~0xfefefefe);
			}

			plaintext[0] = k[-14] ^ x2;
			plaintext[1] = k[-13] ^ x3;
			plaintext[2] = k[-12] ^ x0;
			plaintext[3] = k[-11] ^ x1;
		}

		#region Data
		static uint[] SBOX1_1110 = 
		{
			0x00707070, 0x00828282, 0x002c2c2c, 0x00ececec, 0x00b3b3b3, 0x00272727, 0x00c0c0c0, 0x00e5e5e5,
			0x00e4e4e4, 0x00858585, 0x00575757, 0x00353535, 0x00eaeaea, 0x000c0c0c, 0x00aeaeae, 0x00414141,
			0x00232323, 0x00efefef, 0x006b6b6b, 0x00939393, 0x00454545, 0x00191919, 0x00a5a5a5, 0x00212121,
			0x00ededed, 0x000e0e0e, 0x004f4f4f, 0x004e4e4e, 0x001d1d1d, 0x00656565, 0x00929292, 0x00bdbdbd,
			0x00868686, 0x00b8b8b8, 0x00afafaf, 0x008f8f8f, 0x007c7c7c, 0x00ebebeb, 0x001f1f1f, 0x00cecece,
			0x003e3e3e, 0x00303030, 0x00dcdcdc, 0x005f5f5f, 0x005e5e5e, 0x00c5c5c5, 0x000b0b0b, 0x001a1a1a,
			0x00a6a6a6, 0x00e1e1e1, 0x00393939, 0x00cacaca, 0x00d5d5d5, 0x00474747, 0x005d5d5d, 0x003d3d3d,
			0x00d9d9d9, 0x00010101, 0x005a5a5a, 0x00d6d6d6, 0x00515151, 0x00565656, 0x006c6c6c, 0x004d4d4d,
			0x008b8b8b, 0x000d0d0d, 0x009a9a9a, 0x00666666, 0x00fbfbfb, 0x00cccccc, 0x00b0b0b0, 0x002d2d2d,
			0x00747474, 0x00121212, 0x002b2b2b, 0x00202020, 0x00f0f0f0, 0x00b1b1b1, 0x00848484, 0x00999999,
			0x00dfdfdf, 0x004c4c4c, 0x00cbcbcb, 0x00c2c2c2, 0x00343434, 0x007e7e7e, 0x00767676, 0x00050505,
			0x006d6d6d, 0x00b7b7b7, 0x00a9a9a9, 0x00313131, 0x00d1d1d1, 0x00171717, 0x00040404, 0x00d7d7d7,
			0x00141414, 0x00585858, 0x003a3a3a, 0x00616161, 0x00dedede, 0x001b1b1b, 0x00111111, 0x001c1c1c,
			0x00323232, 0x000f0f0f, 0x009c9c9c, 0x00161616, 0x00535353, 0x00181818, 0x00f2f2f2, 0x00222222,
			0x00fefefe, 0x00444444, 0x00cfcfcf, 0x00b2b2b2, 0x00c3c3c3, 0x00b5b5b5, 0x007a7a7a, 0x00919191,
			0x00242424, 0x00080808, 0x00e8e8e8, 0x00a8a8a8, 0x00606060, 0x00fcfcfc, 0x00696969, 0x00505050,
			0x00aaaaaa, 0x00d0d0d0, 0x00a0a0a0, 0x007d7d7d, 0x00a1a1a1, 0x00898989, 0x00626262, 0x00979797,
			0x00545454, 0x005b5b5b, 0x001e1e1e, 0x00959595, 0x00e0e0e0, 0x00ffffff, 0x00646464, 0x00d2d2d2,
			0x00101010, 0x00c4c4c4, 0x00000000, 0x00484848, 0x00a3a3a3, 0x00f7f7f7, 0x00757575, 0x00dbdbdb,
			0x008a8a8a, 0x00030303, 0x00e6e6e6, 0x00dadada, 0x00090909, 0x003f3f3f, 0x00dddddd, 0x00949494,
			0x00878787, 0x005c5c5c, 0x00838383, 0x00020202, 0x00cdcdcd, 0x004a4a4a, 0x00909090, 0x00333333,
			0x00737373, 0x00676767, 0x00f6f6f6, 0x00f3f3f3, 0x009d9d9d, 0x007f7f7f, 0x00bfbfbf, 0x00e2e2e2,
			0x00525252, 0x009b9b9b, 0x00d8d8d8, 0x00262626, 0x00c8c8c8, 0x00373737, 0x00c6c6c6, 0x003b3b3b,
			0x00818181, 0x00969696, 0x006f6f6f, 0x004b4b4b, 0x00131313, 0x00bebebe, 0x00636363, 0x002e2e2e,
			0x00e9e9e9, 0x00797979, 0x00a7a7a7, 0x008c8c8c, 0x009f9f9f, 0x006e6e6e, 0x00bcbcbc, 0x008e8e8e,
			0x00292929, 0x00f5f5f5, 0x00f9f9f9, 0x00b6b6b6, 0x002f2f2f, 0x00fdfdfd, 0x00b4b4b4, 0x00595959,
			0x00787878, 0x00989898, 0x00060606, 0x006a6a6a, 0x00e7e7e7, 0x00464646, 0x00717171, 0x00bababa,
			0x00d4d4d4, 0x00252525, 0x00ababab, 0x00424242, 0x00888888, 0x00a2a2a2, 0x008d8d8d, 0x00fafafa,
			0x00727272, 0x00070707, 0x00b9b9b9, 0x00555555, 0x00f8f8f8, 0x00eeeeee, 0x00acacac, 0x000a0a0a,
			0x00363636, 0x00494949, 0x002a2a2a, 0x00686868, 0x003c3c3c, 0x00383838, 0x00f1f1f1, 0x00a4a4a4,
			0x00404040, 0x00282828, 0x00d3d3d3, 0x007b7b7b, 0x00bbbbbb, 0x00c9c9c9, 0x00434343, 0x00c1c1c1,
			0x00151515, 0x00e3e3e3, 0x00adadad, 0x00f4f4f4, 0x00777777, 0x00c7c7c7, 0x00808080, 0x009e9e9e,
		};
		static uint[] SBOX4_4404 = 
		{
			0x70007070, 0x2c002c2c, 0xb300b3b3, 0xc000c0c0, 0xe400e4e4, 0x57005757, 0xea00eaea, 0xae00aeae,
			0x23002323, 0x6b006b6b, 0x45004545, 0xa500a5a5, 0xed00eded, 0x4f004f4f, 0x1d001d1d, 0x92009292,
			0x86008686, 0xaf00afaf, 0x7c007c7c, 0x1f001f1f, 0x3e003e3e, 0xdc00dcdc, 0x5e005e5e, 0x0b000b0b,
			0xa600a6a6, 0x39003939, 0xd500d5d5, 0x5d005d5d, 0xd900d9d9, 0x5a005a5a, 0x51005151, 0x6c006c6c,
			0x8b008b8b, 0x9a009a9a, 0xfb00fbfb, 0xb000b0b0, 0x74007474, 0x2b002b2b, 0xf000f0f0, 0x84008484,
			0xdf00dfdf, 0xcb00cbcb, 0x34003434, 0x76007676, 0x6d006d6d, 0xa900a9a9, 0xd100d1d1, 0x04000404,
			0x14001414, 0x3a003a3a, 0xde00dede, 0x11001111, 0x32003232, 0x9c009c9c, 0x53005353, 0xf200f2f2,
			0xfe00fefe, 0xcf00cfcf, 0xc300c3c3, 0x7a007a7a, 0x24002424, 0xe800e8e8, 0x60006060, 0x69006969,
			0xaa00aaaa, 0xa000a0a0, 0xa100a1a1, 0x62006262, 0x54005454, 0x1e001e1e, 0xe000e0e0, 0x64006464,
			0x10001010, 0x00000000, 0xa300a3a3, 0x75007575, 0x8a008a8a, 0xe600e6e6, 0x09000909, 0xdd00dddd,
			0x87008787, 0x83008383, 0xcd00cdcd, 0x90009090, 0x73007373, 0xf600f6f6, 0x9d009d9d, 0xbf00bfbf,
			0x52005252, 0xd800d8d8, 0xc800c8c8, 0xc600c6c6, 0x81008181, 0x6f006f6f, 0x13001313, 0x63006363,
			0xe900e9e9, 0xa700a7a7, 0x9f009f9f, 0xbc00bcbc, 0x29002929, 0xf900f9f9, 0x2f002f2f, 0xb400b4b4,
			0x78007878, 0x06000606, 0xe700e7e7, 0x71007171, 0xd400d4d4, 0xab00abab, 0x88008888, 0x8d008d8d,
			0x72007272, 0xb900b9b9, 0xf800f8f8, 0xac00acac, 0x36003636, 0x2a002a2a, 0x3c003c3c, 0xf100f1f1,
			0x40004040, 0xd300d3d3, 0xbb00bbbb, 0x43004343, 0x15001515, 0xad00adad, 0x77007777, 0x80008080,
			0x82008282, 0xec00ecec, 0x27002727, 0xe500e5e5, 0x85008585, 0x35003535, 0x0c000c0c, 0x41004141,
			0xef00efef, 0x93009393, 0x19001919, 0x21002121, 0x0e000e0e, 0x4e004e4e, 0x65006565, 0xbd00bdbd,
			0xb800b8b8, 0x8f008f8f, 0xeb00ebeb, 0xce00cece, 0x30003030, 0x5f005f5f, 0xc500c5c5, 0x1a001a1a,
			0xe100e1e1, 0xca00caca, 0x47004747, 0x3d003d3d, 0x01000101, 0xd600d6d6, 0x56005656, 0x4d004d4d,
			0x0d000d0d, 0x66006666, 0xcc00cccc, 0x2d002d2d, 0x12001212, 0x20002020, 0xb100b1b1, 0x99009999,
			0x4c004c4c, 0xc200c2c2, 0x7e007e7e, 0x05000505, 0xb700b7b7, 0x31003131, 0x17001717, 0xd700d7d7,
			0x58005858, 0x61006161, 0x1b001b1b, 0x1c001c1c, 0x0f000f0f, 0x16001616, 0x18001818, 0x22002222,
			0x44004444, 0xb200b2b2, 0xb500b5b5, 0x91009191, 0x08000808, 0xa800a8a8, 0xfc00fcfc, 0x50005050,
			0xd000d0d0, 0x7d007d7d, 0x89008989, 0x97009797, 0x5b005b5b, 0x95009595, 0xff00ffff, 0xd200d2d2,
			0xc400c4c4, 0x48004848, 0xf700f7f7, 0xdb00dbdb, 0x03000303, 0xda00dada, 0x3f003f3f, 0x94009494,
			0x5c005c5c, 0x02000202, 0x4a004a4a, 0x33003333, 0x67006767, 0xf300f3f3, 0x7f007f7f, 0xe200e2e2,
			0x9b009b9b, 0x26002626, 0x37003737, 0x3b003b3b, 0x96009696, 0x4b004b4b, 0xbe00bebe, 0x2e002e2e,
			0x79007979, 0x8c008c8c, 0x6e006e6e, 0x8e008e8e, 0xf500f5f5, 0xb600b6b6, 0xfd00fdfd, 0x59005959,
			0x98009898, 0x6a006a6a, 0x46004646, 0xba00baba, 0x25002525, 0x42004242, 0xa200a2a2, 0xfa00fafa,
			0x07000707, 0x55005555, 0xee00eeee, 0x0a000a0a, 0x49004949, 0x68006868, 0x38003838, 0xa400a4a4,
			0x28002828, 0x7b007b7b, 0xc900c9c9, 0xc100c1c1, 0xe300e3e3, 0xf400f4f4, 0xc700c7c7, 0x9e009e9e,
		};
		static uint[] SBOX2_0222 = 
		{
			0xe0e0e000, 0x05050500, 0x58585800, 0xd9d9d900, 0x67676700, 0x4e4e4e00, 0x81818100, 0xcbcbcb00,
			0xc9c9c900, 0x0b0b0b00, 0xaeaeae00, 0x6a6a6a00, 0xd5d5d500, 0x18181800, 0x5d5d5d00, 0x82828200,
			0x46464600, 0xdfdfdf00, 0xd6d6d600, 0x27272700, 0x8a8a8a00, 0x32323200, 0x4b4b4b00, 0x42424200,
			0xdbdbdb00, 0x1c1c1c00, 0x9e9e9e00, 0x9c9c9c00, 0x3a3a3a00, 0xcacaca00, 0x25252500, 0x7b7b7b00,
			0x0d0d0d00, 0x71717100, 0x5f5f5f00, 0x1f1f1f00, 0xf8f8f800, 0xd7d7d700, 0x3e3e3e00, 0x9d9d9d00,
			0x7c7c7c00, 0x60606000, 0xb9b9b900, 0xbebebe00, 0xbcbcbc00, 0x8b8b8b00, 0x16161600, 0x34343400,
			0x4d4d4d00, 0xc3c3c300, 0x72727200, 0x95959500, 0xababab00, 0x8e8e8e00, 0xbababa00, 0x7a7a7a00,
			0xb3b3b300, 0x02020200, 0xb4b4b400, 0xadadad00, 0xa2a2a200, 0xacacac00, 0xd8d8d800, 0x9a9a9a00,
			0x17171700, 0x1a1a1a00, 0x35353500, 0xcccccc00, 0xf7f7f700, 0x99999900, 0x61616100, 0x5a5a5a00,
			0xe8e8e800, 0x24242400, 0x56565600, 0x40404000, 0xe1e1e100, 0x63636300, 0x09090900, 0x33333300,
			0xbfbfbf00, 0x98989800, 0x97979700, 0x85858500, 0x68686800, 0xfcfcfc00, 0xececec00, 0x0a0a0a00,
			0xdadada00, 0x6f6f6f00, 0x53535300, 0x62626200, 0xa3a3a300, 0x2e2e2e00, 0x08080800, 0xafafaf00,
			0x28282800, 0xb0b0b000, 0x74747400, 0xc2c2c200, 0xbdbdbd00, 0x36363600, 0x22222200, 0x38383800,
			0x64646400, 0x1e1e1e00, 0x39393900, 0x2c2c2c00, 0xa6a6a600, 0x30303000, 0xe5e5e500, 0x44444400,
			0xfdfdfd00, 0x88888800, 0x9f9f9f00, 0x65656500, 0x87878700, 0x6b6b6b00, 0xf4f4f400, 0x23232300,
			0x48484800, 0x10101000, 0xd1d1d100, 0x51515100, 0xc0c0c000, 0xf9f9f900, 0xd2d2d200, 0xa0a0a000,
			0x55555500, 0xa1a1a100, 0x41414100, 0xfafafa00, 0x43434300, 0x13131300, 0xc4c4c400, 0x2f2f2f00,
			0xa8a8a800, 0xb6b6b600, 0x3c3c3c00, 0x2b2b2b00, 0xc1c1c100, 0xffffff00, 0xc8c8c800, 0xa5a5a500,
			0x20202000, 0x89898900, 0x00000000, 0x90909000, 0x47474700, 0xefefef00, 0xeaeaea00, 0xb7b7b700,
			0x15151500, 0x06060600, 0xcdcdcd00, 0xb5b5b500, 0x12121200, 0x7e7e7e00, 0xbbbbbb00, 0x29292900,
			0x0f0f0f00, 0xb8b8b800, 0x07070700, 0x04040400, 0x9b9b9b00, 0x94949400, 0x21212100, 0x66666600,
			0xe6e6e600, 0xcecece00, 0xededed00, 0xe7e7e700, 0x3b3b3b00, 0xfefefe00, 0x7f7f7f00, 0xc5c5c500,
			0xa4a4a400, 0x37373700, 0xb1b1b100, 0x4c4c4c00, 0x91919100, 0x6e6e6e00, 0x8d8d8d00, 0x76767600,
			0x03030300, 0x2d2d2d00, 0xdedede00, 0x96969600, 0x26262600, 0x7d7d7d00, 0xc6c6c600, 0x5c5c5c00,
			0xd3d3d300, 0xf2f2f200, 0x4f4f4f00, 0x19191900, 0x3f3f3f00, 0xdcdcdc00, 0x79797900, 0x1d1d1d00,
			0x52525200, 0xebebeb00, 0xf3f3f300, 0x6d6d6d00, 0x5e5e5e00, 0xfbfbfb00, 0x69696900, 0xb2b2b200,
			0xf0f0f000, 0x31313100, 0x0c0c0c00, 0xd4d4d400, 0xcfcfcf00, 0x8c8c8c00, 0xe2e2e200, 0x75757500,
			0xa9a9a900, 0x4a4a4a00, 0x57575700, 0x84848400, 0x11111100, 0x45454500, 0x1b1b1b00, 0xf5f5f500,
			0xe4e4e400, 0x0e0e0e00, 0x73737300, 0xaaaaaa00, 0xf1f1f100, 0xdddddd00, 0x59595900, 0x14141400,
			0x6c6c6c00, 0x92929200, 0x54545400, 0xd0d0d000, 0x78787800, 0x70707000, 0xe3e3e300, 0x49494900,
			0x80808000, 0x50505000, 0xa7a7a700, 0xf6f6f600, 0x77777700, 0x93939300, 0x86868600, 0x83838300,
			0x2a2a2a00, 0xc7c7c700, 0x5b5b5b00, 0xe9e9e900, 0xeeeeee00, 0x8f8f8f00, 0x01010100, 0x3d3d3d00,
		};
		static uint[] SBOX3_3033 = 
		{
			0x38380038, 0x41410041, 0x16160016, 0x76760076, 0xd9d900d9, 0x93930093, 0x60600060, 0xf2f200f2,
			0x72720072, 0xc2c200c2, 0xabab00ab, 0x9a9a009a, 0x75750075, 0x06060006, 0x57570057, 0xa0a000a0,
			0x91910091, 0xf7f700f7, 0xb5b500b5, 0xc9c900c9, 0xa2a200a2, 0x8c8c008c, 0xd2d200d2, 0x90900090,
			0xf6f600f6, 0x07070007, 0xa7a700a7, 0x27270027, 0x8e8e008e, 0xb2b200b2, 0x49490049, 0xdede00de,
			0x43430043, 0x5c5c005c, 0xd7d700d7, 0xc7c700c7, 0x3e3e003e, 0xf5f500f5, 0x8f8f008f, 0x67670067,
			0x1f1f001f, 0x18180018, 0x6e6e006e, 0xafaf00af, 0x2f2f002f, 0xe2e200e2, 0x85850085, 0x0d0d000d,
			0x53530053, 0xf0f000f0, 0x9c9c009c, 0x65650065, 0xeaea00ea, 0xa3a300a3, 0xaeae00ae, 0x9e9e009e,
			0xecec00ec, 0x80800080, 0x2d2d002d, 0x6b6b006b, 0xa8a800a8, 0x2b2b002b, 0x36360036, 0xa6a600a6,
			0xc5c500c5, 0x86860086, 0x4d4d004d, 0x33330033, 0xfdfd00fd, 0x66660066, 0x58580058, 0x96960096,
			0x3a3a003a, 0x09090009, 0x95950095, 0x10100010, 0x78780078, 0xd8d800d8, 0x42420042, 0xcccc00cc,
			0xefef00ef, 0x26260026, 0xe5e500e5, 0x61610061, 0x1a1a001a, 0x3f3f003f, 0x3b3b003b, 0x82820082,
			0xb6b600b6, 0xdbdb00db, 0xd4d400d4, 0x98980098, 0xe8e800e8, 0x8b8b008b, 0x02020002, 0xebeb00eb,
			0x0a0a000a, 0x2c2c002c, 0x1d1d001d, 0xb0b000b0, 0x6f6f006f, 0x8d8d008d, 0x88880088, 0x0e0e000e,
			0x19190019, 0x87870087, 0x4e4e004e, 0x0b0b000b, 0xa9a900a9, 0x0c0c000c, 0x79790079, 0x11110011,
			0x7f7f007f, 0x22220022, 0xe7e700e7, 0x59590059, 0xe1e100e1, 0xdada00da, 0x3d3d003d, 0xc8c800c8,
			0x12120012, 0x04040004, 0x74740074, 0x54540054, 0x30300030, 0x7e7e007e, 0xb4b400b4, 0x28280028,
			0x55550055, 0x68680068, 0x50500050, 0xbebe00be, 0xd0d000d0, 0xc4c400c4, 0x31310031, 0xcbcb00cb,
			0x2a2a002a, 0xadad00ad, 0x0f0f000f, 0xcaca00ca, 0x70700070, 0xffff00ff, 0x32320032, 0x69690069,
			0x08080008, 0x62620062, 0x00000000, 0x24240024, 0xd1d100d1, 0xfbfb00fb, 0xbaba00ba, 0xeded00ed,
			0x45450045, 0x81810081, 0x73730073, 0x6d6d006d, 0x84840084, 0x9f9f009f, 0xeeee00ee, 0x4a4a004a,
			0xc3c300c3, 0x2e2e002e, 0xc1c100c1, 0x01010001, 0xe6e600e6, 0x25250025, 0x48480048, 0x99990099,
			0xb9b900b9, 0xb3b300b3, 0x7b7b007b, 0xf9f900f9, 0xcece00ce, 0xbfbf00bf, 0xdfdf00df, 0x71710071,
			0x29290029, 0xcdcd00cd, 0x6c6c006c, 0x13130013, 0x64640064, 0x9b9b009b, 0x63630063, 0x9d9d009d,
			0xc0c000c0, 0x4b4b004b, 0xb7b700b7, 0xa5a500a5, 0x89890089, 0x5f5f005f, 0xb1b100b1, 0x17170017,
			0xf4f400f4, 0xbcbc00bc, 0xd3d300d3, 0x46460046, 0xcfcf00cf, 0x37370037, 0x5e5e005e, 0x47470047,
			0x94940094, 0xfafa00fa, 0xfcfc00fc, 0x5b5b005b, 0x97970097, 0xfefe00fe, 0x5a5a005a, 0xacac00ac,
			0x3c3c003c, 0x4c4c004c, 0x03030003, 0x35350035, 0xf3f300f3, 0x23230023, 0xb8b800b8, 0x5d5d005d,
			0x6a6a006a, 0x92920092, 0xd5d500d5, 0x21210021, 0x44440044, 0x51510051, 0xc6c600c6, 0x7d7d007d,
			0x39390039, 0x83830083, 0xdcdc00dc, 0xaaaa00aa, 0x7c7c007c, 0x77770077, 0x56560056, 0x05050005,
			0x1b1b001b, 0xa4a400a4, 0x15150015, 0x34340034, 0x1e1e001e, 0x1c1c001c, 0xf8f800f8, 0x52520052,
			0x20200020, 0x14140014, 0xe9e900e9, 0xbdbd00bd, 0xdddd00dd, 0xe4e400e4, 0xa1a100a1, 0xe0e000e0,
			0x8a8a008a, 0xf1f100f1, 0xd6d600d6, 0x7a7a007a, 0xbbbb00bb, 0xe3e300e3, 0x40400040, 0x4f4f004f,
		};

		static int[] KSFT1 =
		{
			0, 64, 0, 64, 15, 79, 15, 79, 30, 94, 45, 109, 45, 124, 60, 124, 77, 13,
			94, 30, 94, 30, 111, 47, 111, 47 
		};

		static int[] KIDX1 = { 0, 0, 8, 8, 0, 0, 8, 8, 8, 8, 0, 0, 8, 0, 8, 8, 0, 0, 0, 0, 8, 8, 0, 0, 8, 8 };

		static int[] KSFT2 = 
		{
			0, 64, 0, 64, 15, 79, 15, 79, 30, 94, 30, 94, 45, 109, 45, 109, 60, 124, 
			60, 124, 60, 124, 77, 13, 77, 13, 94, 30, 94, 30, 111, 47, 111, 47 
		};

		static int[] KIDX2 =
		{
			0, 0, 12, 12, 4, 4, 8, 8, 4, 4, 12, 12, 0, 0, 8, 8, 0, 0, 4, 4, 12, 12, 
			0, 0, 8, 8, 4, 4, 8, 8, 0, 0, 12, 12 
		};
		#endregion
	}
}
