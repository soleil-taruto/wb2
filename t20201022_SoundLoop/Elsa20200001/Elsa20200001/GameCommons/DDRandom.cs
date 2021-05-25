using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// 擬似乱数列
	/// </summary>
	public class DDRandom
	{
		private ulong X;

		public DDRandom()
			: this(SCommon.CRandom.GetUInt())
		{ }

		public DDRandom(uint seed)
		{
			this.X = (ulong)seed;
		}

		/// <summary>
		/// 0以上2^32未満の乱数を返す。
		/// </summary>
		/// <returns>乱数</returns>
		public uint Next()
		{
			ulong uu1 = this.Next2();

			uint u1 = (uint)(uu1 % 4294967311ul); // 2^32 より大きい最小の素数

			return u1;
		}

		private ulong Next2()
		{
			return this.X = 1103515245 * (ulong)(uint)this.X + 12345;
		}

		/// <summary>
		/// 0以上1以下の乱数を返す。
		/// </summary>
		/// <returns>乱数</returns>
		public double Real()
		{
			return this.Next() / (double)uint.MaxValue;
		}

		public uint GetUInt(uint modulo)
		{
			if (modulo < 1u)
				throw new ArgumentException("Bad modulo: " + modulo);

			return this.Next() % modulo;
		}

		public int GetInt(int modulo)
		{
			return (int)this.GetUInt((uint)modulo);
		}

		public void Shuffle<T>(T[] arr)
		{
			for (int index = arr.Length; 2 <= index; index--)
				SCommon.Swap(arr, this.GetInt(index), index - 1);
		}
	}
}
