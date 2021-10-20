using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte
{
	public class JsonNode
	{
		public class Pair
		{
			public string Name;
			public JsonNode Value;
		}

		public List<JsonNode> Array;
		public List<Pair> Map;
		public string Value;
		public bool WordFlag;

		public static JsonNode LoadFromFile(string file)
		{
			return LoadFromFile(file, GetFileEncoding(file));
		}

		public static JsonNode LoadFromFile(string file, Encoding encoding)
		{
			return Load(File.ReadAllText(file, encoding));
		}

		public static JsonNode Load(string text)
		{
			return new Reader(text).GetNode();
		}

		private class Reader
		{
			private string Text;
			private int Index;

			public Reader(string text)
			{
				this.Text = text;
				this.Index = text[0] == '\xfeff' ? 1 : 0; // BOM 読み飛ばし
			}

			private char Next()
			{
				return this.Text[this.Index++];
			}

			private char NextNS()
			{
				char chr;

				do
				{
					chr = this.Next();
				}
				while (chr <= ' ');

				return chr;
			}

			public JsonNode GetNode()
			{
				char chr = this.NextNS();
				JsonNode node = new JsonNode();

				if (chr == '[') // ? Array
				{
					node.Array = new List<JsonNode>();

					if ((chr = this.NextNS()) != ']')
					{
						for (; ; )
						{
							this.Index--;
							node.Array.Add(this.GetNode());
							chr = this.NextNS();

							if (chr == ']')
								break;

							if (chr != ',')
								throw new Exception("JSON format error: Array ','");

							chr = this.NextNS();

							if (chr == ']')
							{
								ProcMain.WriteLog("JSON format warning: ',' before ']'");
								break;
							}
						}
					}
				}
				else if (chr == '{') // ? Map
				{
					node.Map = new List<Pair>();

					if ((chr = this.NextNS()) != '}')
					{
						for (; ; )
						{
							this.Index--;
							string name = this.GetNode().Value;

							if (name == null)
								throw new Exception("JSON format error: Map name");

							if (this.NextNS() != ':')
								throw new Exception("JSON format error: Map ':'");

							node.Map.Add(new Pair()
							{
								Name = name,
								Value = this.GetNode(),
							});

							chr = this.NextNS();

							if (chr == '}')
								break;

							if (chr != ',')
								throw new Exception("JSON format error: Map ','");

							chr = this.NextNS();

							if (chr == '}')
							{
								ProcMain.WriteLog("JSON format warning: ',' before '}'");
								break;
							}
						}
					}
				}
				else if (chr == '"') // ? String
				{
					StringBuilder buff = new StringBuilder();

					for (; ; )
					{
						chr = this.Next();

						if (chr == '"')
							break;

						if (chr == '\\')
						{
							chr = this.Next();

							if (chr == 'b')
							{
								chr = '\b';
							}
							else if (chr == 'f')
							{
								chr = '\f';
							}
							else if (chr == 'n')
							{
								chr = '\n';
							}
							else if (chr == 'r')
							{
								chr = '\r';
							}
							else if (chr == 't')
							{
								chr = '\t';
							}
							else if (chr == 'u')
							{
								char c1 = this.Next();
								char c2 = this.Next();
								char c3 = this.Next();
								char c4 = this.Next();

								chr = (char)Convert.ToInt32(new string(new char[] { c1, c2, c3, c4 }), 16);
							}
						}
						buff.Append(chr);
					}
					node.Value = buff.ToString();
				}
				else // ? Word (number || true || false || null)
				{
					StringBuilder buff = new StringBuilder();

					buff.Append(chr);

					while (this.Index < this.Text.Length)
					{
						chr = this.Next();

						if (
							chr == '}' ||
							chr == ']' ||
							chr == ',' ||
							chr == ':'
							)
						{
							this.Index--;
							break;
						}
						buff.Append(chr);
					}
					node.Value = buff.ToString().Trim();
					node.WordFlag = true;
				}
				return node;
			}
		}

		private static Encoding GetFileEncoding(string file)
		{
			using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				byte[] buff = new byte[4];
				int readSize = reader.Read(buff, 0, 4);

				if (4 <= readSize)
				{
					if (
						buff[0] == 0x00 &&
						buff[1] == 0x00 &&
						buff[2] == 0xfe &&
						buff[3] == 0xff
						||
						buff[0] == 0xff &&
						buff[1] == 0xfe &&
						buff[2] == 0x00 &&
						buff[3] == 0x00
						)
						return Encoding.UTF32;
				}
				if (2 <= readSize)
				{
					if (
						buff[0] == 0xfe &&
						buff[1] == 0xff
						||
						buff[0] == 0xff &&
						buff[1] == 0xfe
						)
						return Encoding.Unicode;
				}
				return Encoding.UTF8;
			}
		}

		private const string NEW_LINE = "\r\n";

		public string GetString()
		{
			StringBuilder buff = new StringBuilder();
			this.WriteTo(buff, 0);
			buff.Append(NEW_LINE);
			return buff.ToString();
		}

		private void WriteTo(StringBuilder buff, int depth)
		{
			if (this.Array != null) // ? Array
			{
				buff.Append('[');
				buff.Append(NEW_LINE);

				for (int index = 0; index < this.Array.Count; index++)
				{
					buff.Append(Indent(depth + 1));
					this.Array[index].WriteTo(buff, depth + 1);
					buff.Append(index < this.Array.Count - 1 ? "," : "");
					buff.Append(NEW_LINE);
				}
				buff.Append(Indent(depth));
				buff.Append(']');
			}
			else if (this.Map != null) // ? Map
			{
				buff.Append('{');
				buff.Append(NEW_LINE);

				for (int index = 0; index < this.Map.Count; index++)
				{
					buff.Append(Indent(depth + 1));
					buff.Append(this.Map[index].Name);
					buff.Append(": ");
					this.Map[index].Value.WriteTo(buff, depth + 1);
					buff.Append(index < this.Array.Count - 1 ? "," : "");
					buff.Append(NEW_LINE);
				}
				buff.Append(Indent(depth));
				buff.Append('}');
			}
			else if (this.WordFlag) // ? Word (number || true || false || null)
			{
				buff.Append(this.Value);
			}
			else // ? String
			{
				buff.Append("\"");

				foreach (char chr in this.Value)
				{
					if (chr == '"')
					{
						buff.Append("\\\"");
					}
					else if (chr == '\\')
					{
						buff.Append("\\\\");
					}
					else if (chr == '\b')
					{
						buff.Append("\\b");
					}
					else if (chr == '\f')
					{
						buff.Append("\\f");
					}
					else if (chr == '\n')
					{
						buff.Append("\\n");
					}
					else if (chr == '\r')
					{
						buff.Append("\\r");
					}
					else if (chr == '\t')
					{
						buff.Append("\\t");
					}
					else
					{
						buff.Append(chr);
					}
				}
				buff.Append("\"");
			}
		}

		private static string Indent(int depth)
		{
			return new string(Enumerable.Repeat('\t', depth).ToArray());
		}
	}
}
