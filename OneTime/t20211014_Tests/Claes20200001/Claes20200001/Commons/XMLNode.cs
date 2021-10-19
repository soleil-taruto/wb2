using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Charlotte.Commons
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
			// TODO
		}
	}
}
