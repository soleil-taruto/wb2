using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Camellias.OpenCrypto;

namespace Charlotte.Camellias
{
	public class Camellia : IDisposable
	{
		private CamelliaTransformLE Transform;

		public Camellia(byte[] rawKey)
		{
			if (
				rawKey.Length != 16 &&
				rawKey.Length != 24 &&
				rawKey.Length != 32
				)
				throw new ArgumentException();

			this.Transform = new CamelliaTransformLE(new CamelliaManaged(), rawKey, new byte[0], true);
		}

		public void EncryptBlock(byte[] input, byte[] output)
		{
			if (
				input.Length != 16 ||
				output.Length != 16
				)
				throw new ArgumentException();

			this.Transform.EncryptECB(input, 0, output, 0);
		}

		public void DecryptBlock(byte[] input, byte[] output)
		{
			if (
				input.Length != 16 ||
				output.Length != 16
				)
				throw new ArgumentException();

			this.Transform.DecryptECB(input, 0, output, 0);
		}

		public void Dispose()
		{
			if (this.Transform != null)
			{
				this.Transform.Dispose();
				this.Transform = null;
			}
		}
	}
}
