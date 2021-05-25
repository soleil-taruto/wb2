using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public struct DDCrash
	{
		public DDCrashUtils.Kind_e Kind;
		public D2Point Pt;
		public double R;
		public D4Rect Rect;
		public DDCrash[] Crashes;

		public bool IsCrashed(DDCrash other)
		{
			return DDCrashUtils.IsCrashed(this, other);
		}
	}
}
