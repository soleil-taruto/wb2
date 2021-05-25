using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Charlotte.Commons;
using Charlotte.Tests;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			// -- choose one --

			//TestMain(); // テスト
			ProductMain(); // 本番

			// --

			Console.WriteLine("Press ENTER key.");
			Console.ReadLine();
		}

		private void TestMain()
		{
			// -- choose one --

			new Test0001().Test01();
			//new Test0001().Test02();
			//new Test0001().Test03();

			// --
		}

		private class UUIDPositionInfo
		{
			public string FilePath;
			public Encoding Encoding;
			public int LineIndex;
			public int CharIndex;
			public string UUID;
			public bool UUIDChanged = false;
		}

		private List<UUIDPositionInfo> UUIDPositions = new List<UUIDPositionInfo>();

		private void ProductMain()
		{
			// 何回か CollectUUIDPosition を実行して全ての UUID (UUIDPosition) を回収してから、最後に SolveUUIDCollision を実行する。

			this.CollectUUIDPosition("C:\\Dev", "ProcMain.cs", Encoding.UTF8, "\t\tpublic const string APP_IDENT = \"", "\"; // アプリ毎に変更する。");
			//this.CollectUUIDPosition();
			//this.CollectUUIDPosition();
			//this.CollectUUIDPosition();
			// ...

			this.SolveUUIDCollision();
		}

		private void CollectUUIDPosition(string rootDir, string localFile, Encoding encoding, string uuidPrefix, string uuidSuffix)
		{
			if (string.IsNullOrEmpty(rootDir)) throw null;
			if (!Directory.Exists(rootDir)) throw null;
			if (string.IsNullOrEmpty(localFile)) throw null;
			if (uuidPrefix == null) throw null;
			if (uuidSuffix == null) throw null;

			foreach (string file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
			{
				if (SCommon.EqualsIgnoreCase(Path.GetFileName(file), localFile))
				{
					Console.WriteLine("file: " + file); // cout

					string[] lines = File.ReadAllLines(file, encoding);

					for (int index = 0; index < lines.Length; index++)
					{
						string line = lines[index];
						string uuid = Common.EraseStartEnd(line, uuidPrefix, uuidSuffix);

						if (uuid != null)
						//if (uuid != null && IsUUID(uuid))
						{
							Console.WriteLine("行番号：" + (index + 1)); // cout

							if (!IsUUID(uuid))
								throw new Exception();

							this.UUIDPositions.Add(new UUIDPositionInfo()
							{
								FilePath = file,
								Encoding = encoding,
								LineIndex = index,
								CharIndex = uuidPrefix.Length,
								UUID = uuid,
							});
						}
					}
				}
			}
		}

		private const int UUID_LEN = 38;

		private static bool IsUUID(string str)
		{
			return Regex.IsMatch(str, "^\\{[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\\}$"); // 小文字のみを想定
		}

		private void SolveUUIDCollision()
		{
			KnownFilePathList knwonFilePaths = new KnownFilePathList();
			knwonFilePaths.Load();

			this.UUIDPositions.Sort((a, b) =>
			{
				if (a == b) // 同じ要素を比較することがある。
					return 0;

				int ret;

				// 既知のファイルをソート順で先に -- 新しいファイルを更新対象にするため
				{
					int aVal = knwonFilePaths.Contains(a.FilePath) ? 0 : 1;
					int bVal = knwonFilePaths.Contains(b.FilePath) ? 0 : 1;

					ret = aVal - bVal;
				}

				if (ret != 0)
					return ret;

				ret = Common.CompPath(a.FilePath, b.FilePath);
				//ret = SCommon.CompIgnoreCase(a.FilePath, b.FilePath); // old
				if (ret != 0)
					return ret;

				ret = a.LineIndex - b.LineIndex;
				if (ret != 0)
					return ret;

				ret = a.CharIndex - b.CharIndex;
				if (ret != 0)
					return ret;

				throw null; // never -- 同じ UUID を複数回検出した可能性あり
			});

			for (int lead = 0; lead + 1 < this.UUIDPositions.Count; lead++)
			{
				for (int far = lead + 1; far < this.UUIDPositions.Count; far++)
				{
					if (this.UUIDPositions[lead].UUID == this.UUIDPositions[far].UUID) // ? コリジョン発見 -> far側(後の方)を更新する。
					{
						this.UUIDPositions[far].UUID = Guid.NewGuid().ToString("B");
						this.UUIDPositions[far].UUIDChanged = true;
					}
				}
			}

			foreach (UUIDPositionInfo up in this.UUIDPositions)
			{
				if (up.UUIDChanged)
				{
					string[] lines = File.ReadAllLines(up.FilePath, up.Encoding);

					// lines[up.LineIndex] <-- 要る
					//
					if (lines.Length <= up.LineIndex)
						throw null;

					string line = lines[up.LineIndex];

					// line[up.CharIndex + UUID_LEN - 1] <-- 要る
					// line[up.CharIndex + UUID_LEN - 0] <-- 要らない
					//
					if (line.Length < up.CharIndex + UUID_LEN)
						throw null;

					if (!IsUUID(line.Substring(up.CharIndex, UUID_LEN)))
						throw null;

					line = line.Substring(0, up.CharIndex) + up.UUID + line.Substring(up.CharIndex + UUID_LEN);

					Console.WriteLine("< " + lines[up.LineIndex]); // cout
					Console.WriteLine("> " + line); // cout

					lines[up.LineIndex] = line;

					File.WriteAllLines(up.FilePath, lines, up.Encoding);
				}
			}

			knwonFilePaths.Clear();
			knwonFilePaths.AddRange(this.UUIDPositions.Select(up => up.FilePath));
			knwonFilePaths.Save();
		}
	}
}
