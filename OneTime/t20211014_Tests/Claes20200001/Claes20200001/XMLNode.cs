using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Charlotte
{
	public class XMLNode
	{
		public string Name;
		public string Value;
		public List<XMLNode> Children = new List<XMLNode>();

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
									node.Children.Add(new XMLNode() { Name = reader.Name, Value = reader.Value });

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

				// node 正規化
				{
					node.Name = node.Name ?? "";
					node.Value = node.Value ?? "";

					//node.Name = DecodeXML(node.Name); // 不要
					//node.Value = DecodeXML(node.Value); // 不要

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

			// 正規化
			{
				name = name ?? "";
				value = value ?? "";

				name = EncodeXML(name);
				value = EncodeXML(value);
			}

			Indent(writer, depth);

			if (this.Children.Count != 0)
			{
				writer.Write('<');
				writer.Write(this.Name);
				writer.Write('>');
				writer.WriteLine(this.Value);

				foreach (XMLNode child in this.Children)
					child.WriteTo(writer, depth + 1);

				Indent(writer, depth);
				writer.Write('<');
				writer.Write('/');
				writer.Write(this.Name);
				writer.WriteLine('>');
			}
			else if (this.Value != "")
			{
				writer.Write('<');
				writer.Write(this.Name);
				writer.Write('>');
				writer.Write(this.Value);
				writer.Write('<');
				writer.Write('/');
				writer.Write(this.Name);
				writer.WriteLine('>');
			}
			else
			{
				writer.Write('<');
				writer.Write(this.Name);
				writer.Write('/');
				writer.WriteLine('>');
			}
		}

		private static void Indent(StreamWriter writer, int depth)
		{
			for (int index = 0; index < depth; index++)
				writer.Write('\t');
		}

#if false // 不要
		private static string DecodeXML(string str)
		{
			return str
				.Replace("&quot;", "\"")
				.Replace("&apos;", "'")
				.Replace("&lt;", "<")
				.Replace("&gt;", ">")
				.Replace("&amp;", "&");
		}
#endif

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
