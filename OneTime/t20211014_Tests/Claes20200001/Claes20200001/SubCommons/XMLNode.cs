using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Charlotte.SubCommons
{
	public class XMLNode
	{
		public string Name;
		public string Value;
		public List<XMLNode> Children = new List<XMLNode>();
		public bool AttributeFlag;

		public static XMLNode LoadFromFile(string xmlFile)
		{
			XMLNode node = new XMLNode();
			Stack<XMLNode> parents = new Stack<XMLNode>();

			using (XmlReader reader = XmlReader.Create(xmlFile))
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							{
								XMLNode child = new XMLNode() { Name = reader.LocalName };

								node.Children.Add(child);
								parents.Push(node);
								node = child;

								bool singleTag = reader.IsEmptyElement;

								while (reader.MoveToNextAttribute())
									node.Children.Add(new XMLNode() { Name = reader.Name, Value = reader.Value, AttributeFlag = true });

								if (singleTag)
									node = parents.Pop();
							}
							break;

						case XmlNodeType.Text:
							node.Value = reader.Value;
							break;

						case XmlNodeType.EndElement:
							node = parents.Pop();
							break;

						default:
							break;
					}
				}
			}
			node = node.Children[0];
			Normalize(node);
			return node;
		}

		private static void Normalize(XMLNode node)
		{
			Queue<XMLNode> q = new Queue<XMLNode>();

			q.Enqueue(node);

			while (1 <= q.Count)
			{
				node = q.Dequeue();

				// 正規化
				{
					node.Name = node.Name ?? "";
					node.Value = node.Value ?? "";

					// XmlReader が &xxx; を変換(復元)してくれる。

					{
						int colon = node.Name.IndexOf(':');

						if (colon != -1)
							node.Name = node.Name.Substring(colon + 1);
					}

					node.Name = node.Name.Trim();
					node.Value = node.Value.Trim();
				}

				foreach (XMLNode child in node.Children)
					q.Enqueue(child);
			}
		}

		public void WriteToFile(string xmlFile)
		{
			using (StreamWriter writer = new StreamWriter(xmlFile, false, Encoding.UTF8))
			{
				writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				this.WriteTo(writer, 0);
			}
		}

		private void WriteTo(StreamWriter writer, int depth)
		{
			string name = this.Name;
			string value = this.Value;

			// node 正規化
			{
				name = name ?? "";
				value = value ?? "";

				name = EncodeXML(name);
				value = EncodeXML(value);
			}

			writer.Write(Indent(depth) + "<" + name);

			foreach (XMLNode node in this.Children)
				if (node.AttributeFlag)
					node.WriteAttributeTo(writer);

			if (this.Children.Any(node => !node.AttributeFlag))
			{
				writer.WriteLine(">" + this.Value);

				foreach (XMLNode node in this.Children)
					if (!node.AttributeFlag)
						node.WriteTo(writer, depth + 1);

				writer.WriteLine(Indent(depth) + "</" + name + ">");
			}
			else if (value != "")
			{
				writer.WriteLine(">" + this.Value + "</" + name + ">");
			}
			else
			{
				writer.WriteLine("/>");
			}
		}

		private void WriteAttributeTo(StreamWriter writer)
		{
			string name = this.Name;
			string value = this.Value;

			// attribute 正規化
			{
				name = name ?? "";
				value = value ?? "";

				name = EncodeXML(name);
				value = EncodeXML(value);
			}

			writer.Write(" " + name + "=\"" + value + "\"");
		}

		private static string Indent(int depth)
		{
			return new string(Enumerable.Repeat('\t', depth).ToArray());
		}

		private static string EncodeXML(string str)
		{
			StringBuilder buff = new StringBuilder();

			foreach (char chr in str)
			{
				switch (chr)
				{
					case '"':
						buff.Append("&quot;");
						break;

					case '\'':
						buff.Append("&apos;");
						break;

					case '<':
						buff.Append("&lt;");
						break;

					case '>':
						buff.Append("&gt;");
						break;

					case '&':
						buff.Append("&amp;");
						break;

					default:
						buff.Append(chr);
						break;
				}
			}
			return buff.ToString();
		}
	}
}
