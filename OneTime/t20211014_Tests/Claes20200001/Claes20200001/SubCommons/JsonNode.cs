using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.SubCommons
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
			private int Index = 0;

			public Reader(string text)
			{
				this.Text = text;
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

					this.Index--;

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

		public void WriteToFile(string file)
		{
			this.WriteToFile(file, Encoding.UTF8);
		}

		public void WriteToFile(string file, Encoding encoding)
		{
			File.WriteAllText(file, this.GetString(), encoding);
		}

		public string GetString()
		{
			StringBuilder buff = new StringBuilder();
			new Writer(buff).WriteRoot(this);
			return buff.ToString();
		}

		public static bool ShortMode = false;

		private class Writer
		{
			private StringBuilder Buff;
			private int Depth = 0;

			public Writer(StringBuilder buff)
			{
				this.Buff = buff;
			}

			public void WriteRoot(JsonNode node)
			{
				this.Write(node);
				this.WriteNewLine();
			}

			public void Write(JsonNode node)
			{
				if (node.Array != null) // ? Array
				{
					this.Write('[');
					this.WriteNewLine();
					this.Depth++;

					for (int index = 0; index < node.Array.Count; index++)
					{
						this.WriteIndent();
						this.Write(node.Array[index]);

						if (index < node.Array.Count - 1)
							this.Write(',');

						this.WriteNewLine();
					}
					this.Depth--;
					this.WriteIndent();
					this.Write(']');
				}
				else if (node.Map != null) // ? Map
				{
					this.Write('{');
					this.WriteNewLine();
					this.Depth++;

					for (int index = 0; index < node.Map.Count; index++)
					{
						this.WriteIndent();
						this.Write(node.Map[index].Name);
						this.Write(':');
						this.WriteSpace();
						this.Write(node.Map[index].Value);

						if (index < node.Map.Count - 1)
							this.Write(',');

						this.WriteNewLine();
					}
					this.Depth--;
					this.WriteIndent();
					this.Write('}');
				}
				else if (node.WordFlag) // ? Word (number || true || false || null)
				{
					this.Write(node.Value);
				}
				else // ? String
				{
					this.Write('"');

					foreach (char chr in node.Value)
					{
						if (chr == '"')
						{
							this.Write("\\\"");
						}
						else if (chr == '\\')
						{
							this.Write("\\\\");
						}
						else if (chr == '\b')
						{
							this.Write("\\b");
						}
						else if (chr == '\f')
						{
							this.Write("\\f");
						}
						else if (chr == '\n')
						{
							this.Write("\\n");
						}
						else if (chr == '\r')
						{
							this.Write("\\r");
						}
						else if (chr == '\t')
						{
							this.Write("\\t");
						}
						else
						{
							this.Write(chr);
						}
					}
					this.Write('"');
				}
			}

			private void WriteIndent()
			{
				if (!ShortMode)
					for (int index = 0; index < this.Depth; index++)
						this.Write('\t');
			}

			private void WriteNewLine()
			{
				if (!ShortMode)
					this.Write("\r\n");
			}

			private void WriteSpace()
			{
				if (!ShortMode)
					this.Write(' ');
			}

			private void Write(string str)
			{
				this.Buff.Append(str);
			}

			private void Write(char chr)
			{
				this.Buff.Append(chr);
			}
		}
	}
}
