using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public static class Test1001
	{
		private static DateTime? EnteredTime = null;

		public static void Enter()
		{
			if (EnteredTime != null)
				throw null; // never

			EnteredTime = DateTime.Now;
		}

		public static void Leave(string position)
		{
			if (EnteredTime == null)
				throw null; // never

			double millis = (DateTime.Now - EnteredTime.Value).TotalMilliseconds;

			if (10.0 < millis) // ? 時間掛かり過ぎ
				ProcMain.WriteLog(position + " --> " + millis);

			EnteredTime = null;
		}
	}
}
