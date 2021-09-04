using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Cryptography;

namespace WCPortFwd
{
	// sync > @ UISuspend

	public class UISuspend : IDisposable
	{
		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		private static extern uint SendMessage(IntPtr hWnd, uint wMsg, uint wParam, uint lParam);

		private const uint WM_SETREDRAW = 0x000B;

		private static void SetWindowRedraw(IWin32Window window, bool fRedraw)
		{
			if ((window != null) && (window.Handle != IntPtr.Zero))
			{
				SendMessage(window.Handle, WM_SETREDRAW, (fRedraw) ? 1u : 0u, 0u);
			}
		}

		private Control _ctrl;

		public UISuspend(Control ctrl)
		{
			SetWindowRedraw(ctrl, false);
			_ctrl = ctrl;
		}

		public void Dispose()
		{
			if (_ctrl != null)
			{
				SetWindowRedraw(_ctrl, true);
				_ctrl.Refresh();
				_ctrl = null;
			}
		}
	}

	// < sync

	public class Counter
	{
		private int Count;

		public Counter(int initCount)
		{
			this.Count = initCount;
		}

		public void Increment()
		{
			this.Count++;
		}

		public void Decrement()
		{
			if (0 < this.Count)
			{
				this.Count--;
			}
		}

		public bool IsZero()
		{
			return this.Count == 0;
		}

		public LocalIncrementer LocalIncrement()
		{
			return new LocalIncrementer(this);
		}

		public class LocalIncrementer : IDisposable
		{
			private Counter C;

			public LocalIncrementer(Counter c)
			{
				this.C = c;
				this.C.Increment();
			}

			public void Dispose()
			{
				if (this.C != null)
				{
					this.C.Decrement();
					this.C = null;
				}
			}
		}
	}

	public static class Tools
	{
		public static void DoubleBufferOn(Control ctrl)
		{
			typeof(Control).GetProperty(
				"DoubleBuffered",
				BindingFlags.Instance | BindingFlags.NonPublic
				)
				.SetValue(ctrl, true, null);
		}

		public static int Parse(string str, int defaultValue = -1)
		{
			try
			{
				return int.Parse(str);
			}
			catch
			{ }

			return defaultValue;
		}

		public static int Range(int value, int minval, int maxval)
		{
			return Math.Min(Math.Max(value, minval), maxval);
		}

		public static void Swap<TYPE>(List<TYPE> list, int index1, int index2)
		{
			TYPE tmp = list[index1];

			list[index1] = list[index2];
			list[index2] = tmp;
		}

		public static string DomainFltr(string domain)
		{
			try
			{
				StringBuilder sBuff = new StringBuilder();

				foreach (char chr in domain)
					if ((Ground.I.ALPHA + Ground.I.alpha + Ground.I.DIGIT + "-.").IndexOf(chr) != -1)
						sBuff.Append(chr);

				domain = "" + sBuff;

				if (domain == "")
					throw new Exception();

				if (1000 < domain.Length) // ? 長すぎる。
					throw new Exception();
			}
			catch
			{
				return "localhost";
			}
			return domain;
		}

		public static string PassphraseFltr(string passphrase)
		{
			const string USABLE_PUNCT = "!#$_.,:;+-*/=[]{}";

			try
			{
				StringBuilder sBuff = new StringBuilder();

				foreach (char chr in passphrase)
					if ((Ground.I.ALPHA + Ground.I.alpha + Ground.I.DIGIT + USABLE_PUNCT).IndexOf(chr) != -1 || (Tools.Is全角(chr) && chr != '　'))
						sBuff.Append(chr);

				passphrase = "" + sBuff;

				if (1000 < passphrase.Length) // ? 長すぎる。
					throw new Exception("tooLong");
			}
			catch
			{
				return "[key-or-passphrase-error]";
			}
			return passphrase;
		}

		public static bool Is全角(char chr)
		{
			try
			{
				return Encoding.GetEncoding(932).GetByteCount(new char[] { chr }) == 2;
			}
			catch
			{ }

			return false;
		}

		public static bool IsHex128(string str)
		{
			if (str.Length != 128)
				return false;

			foreach (char chr in str)
				if ((Ground.I.DIGIT + Ground.I.ALPHA.Substring(0, 6) + Ground.I.alpha.Substring(0, 6)).IndexOf(chr) == -1)
					return false;

			return true;
		}

		public static string GetCompactStamp(DateTime dt)
		{
			return
				Tools.LPad("0", 4, "" + dt.Year) +
				Tools.LPad("0", 2, "" + dt.Month) +
				Tools.LPad("0", 2, "" + dt.Day) +
				Tools.LPad("0", 2, "" + dt.Hour) +
				Tools.LPad("0", 2, "" + dt.Minute) +
				Tools.LPad("0", 2, "" + dt.Second);
		}

		public static string LPad(string lPadPtn, int minlen, string str)
		{
			while (str.Length < minlen)
			{
				if (lPadPtn.Length < 1)
				{
					throw new Exception("lPadPtn is null string");
				}
				str = lPadPtn + str;
			}
			return str;
		}

		public static string GetHexString(byte[] bytes, int startPos, int count)
		{
			StringBuilder buff = new StringBuilder();

			for (int i = 0; i < count; i++)
			{
				byte byteval = bytes[startPos + i];
				int intval = (int)byteval;

				buff.Append(Ground.I.hex_digit[intval / 16]);
				buff.Append(Ground.I.hex_digit[intval % 16]);
			}
			return buff.ToString();
		}

		public static string GetCheckSum(string text)
		{
			using (SHA512 crypto = new SHA512CryptoServiceProvider())
			{
				return Tools.GetHexString(crypto.ComputeHash(Encoding.GetEncoding(932).GetBytes(text)), 0, 4);
			}
		}
	}

	public static class EventSet
	{
		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

		[DllImport("kernel32.dll")]
		private static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		public static void Perform(string eventName)
		{
			try
			{
				IntPtr hdl = CreateEvent((IntPtr)0, false, false, eventName);

				if (hdl == (IntPtr)0)
					throw new Exception();

				SetEvent(hdl);
				CloseHandle(hdl);
			}
			catch
			{ }
		}
	}
}
