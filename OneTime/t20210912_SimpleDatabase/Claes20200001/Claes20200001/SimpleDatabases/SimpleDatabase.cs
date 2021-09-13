﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.SimpleDatabases
{
	public class SimpleDatabase
	{
		private string RootDir;
		private long FileSizeLimit;

		public SimpleDatabase(string rootDir, long fileSizeLimit)
		{
			if (rootDir == null)
				throw new ArgumentNullException();

			if (fileSizeLimit < 1L || SCommon.IMAX_64 < fileSizeLimit)
				throw new ArgumentOutOfRangeException();

			rootDir = SCommon.MakeFullPath(rootDir);

			SCommon.CreateDir(rootDir);

			this.RootDir = rootDir;
			this.FileSizeLimit = fileSizeLimit;
		}

		public void Add(string[] row)
		{
			row = RowFilter(row);

			string[] files = this.GetFiles();
			string file;
			bool append = true;

			if (files.Length == 0)
			{
				file = this.GetFirstFilePath();
				append = false;
			}
			else
			{
				file = files[files.Length - 1];

				FileInfo fileInfo = new FileInfo(file);

				if (this.FileSizeLimit < fileInfo.Length)
				{
					file = this.CreateNextFilePath(file);
					append = false;
				}
			}

			using (CsvFileWriter writer = new CsvFileWriter(file, append))
			{
				writer.WriteRow(row);
			}
		}

		public IEnumerable<string[]> ReadToEnd()
		{
			string[] files = this.GetFiles();

			foreach (string file in files)
			{
				if (File.Exists(file)) // 読み込み中に削除される場合あり -- this.ReadToEnd() 読み込み中に this.Remove();
				{
					string[][] rows = ReadFile(file);

					foreach (string[] row in rows)
						yield return row;
				}
			}
		}

		public void Remove(Predicate<string[]> match)
		{
			string[] files = this.GetFiles();

			foreach (string file in files)
			{
				if (File.Exists(file)) // 読み込み中に削除される場合あり -- this.Remove() 中に this.Remove();
				{
					string[][] rows = ReadFile(file);
					bool removed = false;

					for (int index = 0; index < rows.Length; index++)
					{
						if (match(rows[index]))
						{
							rows[index] = null;
							removed = true;
						}
					}
					if (removed)
					{
						rows = rows.Where(row => row != null).ToArray();

						if (rows.Length == 0)
						{
							SCommon.DeletePath(file);
						}
						else
						{
							WriteFile(file, rows);
						}
					}
				}
			}
		}

		public void Edit(Func<string[], string[]> editor)
		{
			string[] files = this.GetFiles();

			foreach (string file in files)
			{
				if (File.Exists(file)) // 読み込み中に削除される場合あり -- this.Edit() 中に this.Remove();
				{
					string[][] rows = ReadFile(file);
					bool edited = false;

					for (int index = 0; index < rows.Length; index++)
					{
						string[] row = editor(rows[index]);

						if (row != null)
						{
							row = RowFilter(row);
							rows[index] = row;
							edited = true;
						}
					}
					if (edited)
					{
						WriteFile(file, rows);
					}
				}
			}
		}

		private string[] GetFiles()
		{
			return Directory.GetFiles(this.RootDir, "*.csv", SearchOption.TopDirectoryOnly).OrderBy(SCommon.Comp).ToArray();
		}

		private string GetFirstFilePath()
		{
			return Path.Combine(this.RootDir, "20190101000059000.csv");
		}

		private string CreateNextFilePath(string file)
		{
			long counter = long.Parse(Path.GetFileNameWithoutExtension(file));
			counter++;
			return Path.Combine(this.RootDir, counter + ".csv");
		}

		private static string[] RowFilter(string[] row)
		{
			return row.Select(cell => SCommon.ToJString(SCommon.ENCODING_SJIS.GetBytes(cell), true, true, true, true)).ToArray();
		}

		private static string[][] ReadFile(string file)
		{
			using (CsvFileReader reader = new CsvFileReader(file))
			{
				return reader.ReadToEnd();
			}
		}

		private static void WriteFile(string file, string[][] rows)
		{
			string fileNew = CreateTempFile(file + "-new-");
			string oldFile = CreateTempFile(file + "-old-");

			using (CsvFileWriter writer = new CsvFileWriter(fileNew))
			{
				writer.WriteRows(rows);
			}
			File.Move(file, oldFile);
			File.Move(fileNew, file);
			SCommon.DeletePath(oldFile);
		}

		private static string CreateTempFile(string prefix)
		{
			long counter = SCommon.SimpleDateTime.Now().ToTimeStamp() * 1000L;

			for (; ; )
			{
				string file = prefix + counter + ".tmp";

				if (!File.Exists(file))
					return file;

				counter++;
			}
		}
	}
}