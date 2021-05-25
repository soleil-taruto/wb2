using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte
{
	public class KnownFilePathList
	{
		private List<string> KnownFilePaths = new List<string>();

		public void Clear()
		{
			this.KnownFilePaths.Clear();
		}

		public void AddRange(IEnumerable<string> filePaths)
		{
			this.KnownFilePaths.AddRange(filePaths);
		}

		public bool Contains(string uuid)
		{
			return this.KnownFilePaths.Any(knownFilePath => SCommon.EqualsIgnoreCase(knownFilePath, uuid));
		}

		private const string KNOWN_FILE_PATH_LIST_FILE = @"C:\appdata\SUC-KnownFilePathList.txt"; // zantei ?

		public void Load()
		{
			if (File.Exists(KNOWN_FILE_PATH_LIST_FILE))
				this.KnownFilePaths = File.ReadAllLines(KNOWN_FILE_PATH_LIST_FILE, Encoding.ASCII).ToList();
			else
				this.KnownFilePaths.Clear();
		}

		public void Save()
		{
			File.WriteAllLines(KNOWN_FILE_PATH_LIST_FILE, this.KnownFilePaths, Encoding.ASCII);
		}
	}
}
