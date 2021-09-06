using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte
{
	public static class Consts
	{
		/// <summary>
		/// コピー元フォルダパス
		/// </summary>
		public const string SRC_DIR = @"C:\";

		/// <summary>
		/// コピー先フォルダパス
		/// </summary>
		public const string DEST_DIR = @"P:\";

		/// <summary>
		/// コピー元で無視するフォルダのローカル名
		/// </summary>
		public static string[] SRC_IGNORE_NAMES = new string[]
		{
			"$Recycle.Bin",
			"$SysReset",
			"$WinREAgent",
			"Config.Msi",
			"Documents and Settings",
			"MSOCache",
			"PerfLogs",
			"Program Files",
			"Program Files (x86)",
			"ProgramData",
			"Recovery",
			"RECYCLER",
			"System Volume Information",
			"Users",
			"Windows",
			"Windows.old",
			"WINNT",
		};

		/// <summary>
		/// コピー先で無視するフォルダのローカル名
		/// </summary>
		public static string[] DEST_IGNORE_NAMES = new string[]
		{
			"$Recycle.Bin",
			"System Volume Information",
		};

		/// <summary>
		/// ログファイル出力先(1)
		/// </summary>
		public const string LOG_FILE_1 = @"C:\tmp\Backup.log";

		/// <summary>
		/// ログファイル出力先(2)
		/// </summary>
		public const string LOG_FILE_2 = @"P:\Backup.log";
	}
}
