using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WCPortFwd
{
	public class Ground
	{
		private Ground()
		{ }
		public static Ground I = new Ground();

		public readonly string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public readonly string alpha = "abcdefghijklmnopqrstuvwxyz";
		public readonly string DIGIT = "0123456789";
		public readonly string HEX_DIGIT = "0123456789ABCDEF";
		public readonly string hex_digit = "0123456789abcdef";

		public readonly string DEFAULT_RAWKEY = "weakest";
		public readonly string STATUSLABEL_BLANK = " ";
		public readonly int MS_ROW_MAX = 10000;

		public readonly string INFO_FILE = "WCPortFwd.dat";

		public MainWin MainWin;
		public List<ForwardInfo> ForwardInfoList = new List<ForwardInfo>();
		public int 表示されないエラー停止の回数;
		public int シャットダウン_Count;

		public void LoadForwardInfoList()
		{
			try
			{
				this.ForwardInfoList.Clear();

				foreach (string line in File.ReadAllLines(INFO_FILE, Encoding.GetEncoding(932)))
				{
					string[] tokens = line.Split(',');

					ForwardInfo fi = new ForwardInfo()
					{
						Started = int.Parse(tokens[0]) != 0,
						RecvPortNo = Tools.Range(int.Parse(tokens[1]), 1, 65535),
						ForwardPortNo = Tools.Range(int.Parse(tokens[2]), 1, 65535),
						ForwardDomain = Tools.DomainFltr(tokens[3]),
						ConnectMax = Tools.Range(int.Parse(tokens[4]), 1, 500),
						DecMode = int.Parse(tokens[5]) != 0,
						RawKey = Tools.PassphraseFltr(tokens[6]),
					};

					this.ForwardInfoList.Add(fi);
				}
			}
			catch
			{ }
		}

		public void SaveForwardInfoList()
		{
			try
			{
				List<string> lines = new List<string>();

				foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
				{
					lines.Add(string.Join(
						",",
						new string[]
						{
							"" + (fi.Started ? 1 : 0),
							"" + fi.RecvPortNo,
							"" + fi.ForwardPortNo,
							"" + fi.ForwardDomain,
							"" + fi.ConnectMax,
							"" + (fi.DecMode ? 1 : 0),
							"" + fi.RawKey,
						}
						));
				}
				File.WriteAllLines(INFO_FILE, lines, Encoding.GetEncoding(932));
			}
			catch
			{ }
		}
	}

	public class ForwardInfo
	{
		// デフォルト値で初期化
		public bool Started;
		public int RecvPortNo = 8080;
		public int ForwardPortNo = 80;
		public string ForwardDomain = "localhost";
		public int ConnectMax = 10;
		public bool DecMode;
		public string RawKey = ""; // "" == 暗号化ナシ

		public Process Proc;
		public int StartTimeRecvPortNo;
		public bool Modified;

		public bool 停止チェック()
		{
			using (Ground.I.MainWin.TimerOff.LocalIncrement())
			{
				if (this.Started)
				{
					this.Started = false;
				}
				if (this.Proc != null)
				{
					this.停止してね();

					for (int c = 0; c < 3; c++)
					{
						using (Form f = new 何かを待つWin(this))
						{
							f.ShowDialog();
						}
						this.停止();

						if (this.Proc == null)
							break;
					}
				}
				if (this.Proc != null)
				{
					this.Proc.Kill(); // めんどくせえ、死ね。
					this.Proc = null;
				}
				return true;
			}
		}

		public bool ポートの重複有り()
		{
			foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
				if (fi != this && this.RecvPortNo == fi.RecvPortNo && fi.Started)
					return true;

			return false;
		}

		public void 巡回Proc()
		{
			this.停止();

			if (this.Started)
			{
				if (this.Proc == null && this.IsExist停止中() == false)
					this.開始();
			}
			else
			{
				if (this.Proc != null)
					this.停止してね();
			}
		}

		public bool Is停止中()
		{
			return this.Started == false && this.Proc != null;
		}

		public bool IsExist停止中()
		{
			for (int index = 0; index < Ground.I.ForwardInfoList.Count; index++)
			{
				if (Ground.I.ForwardInfoList[index].Is停止中())
				{
					return true;
				}
			}
			return false;
		}

		public void 停止()
		{
			if (this.Proc == null)
				return;

			if (this.Proc.HasExited == false)
				return;

			// シャットダウンした場合 CUI は -1073741510 (CTRL+Cと同じエラーコード)でプロセスが死ぬ...

			//const int NTSTATUS_CTRL_C = -1073741510;
			const int ERRLV_HANDLED_ERROR = 108;
			int errLv = this.Proc.ExitCode;

			// ? エラー終了した。
			//if (errLv == 1)
			if (errLv != 0)
			//if (errLv != 0 && errLv != NTSTATUS_CTRL_C)
			{
				if (errLv == ERRLV_HANDLED_ERROR || this.RawKey != "" && errLv == 1) // ? ハンドルされたエラー
				{
					this.Started = false;
					this.Modified = true;
				}
				else
				{
					Ground.I.表示されないエラー停止の回数++;
					Ground.I.MainWin.SetSubStatusLabel("(未知のエラーコードを " + Ground.I.表示されないエラー停止の回数 + " 回検知しました, 最後のエラーコードは " + errLv + " です)");
				}
			}
			this.Proc = null;
		}

		public void 開始()
		{
			if (this.RawKey != "")
			{
				try
				{
					string key;

					if (Tools.IsHex128(this.RawKey))
					{
						string file = this.GetCrypTunnelKeyFile(this.RecvPortNo);
						File.WriteAllText(file, this.RawKey, Encoding.ASCII);
						key = file;
					}
					else
						key = "*" + this.RawKey;

					ProcessStartInfo psi = new ProcessStartInfo();

					psi.FileName = this.GetCrypTunnelExecFile();
					psi.Arguments =
						this.RecvPortNo +
						" " + this.ForwardDomain +
						" " + this.ForwardPortNo +
						" /C " + this.ConnectMax +
						(this.DecMode ? " /R" : "") +
						" /BSL 50000 " + key;
					psi.CreateNoWindow = true;
					psi.UseShellExecute = false;

					this.Proc = Process.Start(psi);
					this.StartTimeRecvPortNo = this.RecvPortNo;
				}
				catch
				{
					this.Proc = null;
				}

				return;
			}

			try
			{
				ProcessStartInfo psi = new ProcessStartInfo();

				psi.FileName = this.GetExecFile();
				psi.Arguments =
					"/P " + this.RecvPortNo +
					" /FP " + this.ForwardPortNo +
					" /FD " + this.ForwardDomain +
					" /C " + this.ConnectMax +
					(this.RawKey != "" ? " /K " + this.RawKey : "") +
					(this.DecMode ? " /D" : "") +
					" /S";
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;

				this.Proc = Process.Start(psi);
				this.StartTimeRecvPortNo = this.RecvPortNo;
			}
			catch
			{
				this.Proc = null;
			}
		}

		public void 停止してね()
		{
			try
			{
				ProcessStartInfo psi = new ProcessStartInfo();

				psi.FileName = this.GetCrypTunnelExecFile();
				psi.Arguments = this.StartTimeRecvPortNo + " a 1 /S";
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;

				Process.Start(psi);
			}
			catch
			{ }

			try
			{
				File.Delete(this.GetCrypTunnelKeyFile(this.StartTimeRecvPortNo));
			}
			catch
			{ }

			try
			{
#if true
				EventSet.Perform("cerulean.charlotte CryptPortForward server termination PORT " + this.StartTimeRecvPortNo);
#else // プロセス起動版 -> シャットダウン非対応 -- SessionEnding 追加 @ 2021.6
				ProcessStartInfo psi = new ProcessStartInfo();

				psi.FileName = this.GetExecFile();
				psi.Arguments = "/P " + this.StartTimeRecvPortNo + " /T";
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;

				Process.Start(psi);
#endif
			}
			catch
			{ }
		}

		public static void ゴミ掃除()
		{
			try
			{
				foreach (string file in Directory.GetFiles(".", GetCrypTunnelKeyFileWildCard()))
					File.Delete(file);
			}
			catch
			{ }
		}

		private string GetCrypTunnelExecFile()
		{
			return "crypTunnel.exe";
		}

		private string GetCrypTunnelKeyFile(int portNo)
		{
			return "crypTunnel.exe_" + portNo.ToString("D5") + ".tmp";
		}

		private static string GetCrypTunnelKeyFileWildCard()
		{
			return "crypTunnel.exe_*.tmp";
		}

		private string ExecFile;

		private string GetExecFile()
		{
			if (this.ExecFile == null)
			{
				this.ExecFile = "CPortFwd.exe";

				string cwd = Directory.GetCurrentDirectory();

				if (File.Exists(this.ExecFile) == false)
					this.ExecFile = Path.Combine("..\\..\\..\\..\\CPortFwd\\Release", this.ExecFile);
			}
			return this.ExecFile;
		}
	}
}
