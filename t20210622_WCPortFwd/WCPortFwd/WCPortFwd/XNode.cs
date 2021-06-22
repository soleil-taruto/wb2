using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WCPortFwd
{
	public class XNode
	{
		public XNode Parent;
		public string Name;
		public string Text;
		public List<XNode> Children = new List<XNode>();

		public static XNode Load(string xmlFile)
		{
			using (XmlReader reader = XmlReader.Create(xmlFile))
			{
				XNode node = new XNode();

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							{
								XNode child = new XNode();

								child.Parent = node;
								child.Name = reader.LocalName;

								node.Children.Add(child);
								node = child;

								bool singleTag = reader.IsEmptyElement;

								while (reader.MoveToNextAttribute())
								{
									XNode attr = new XNode();

									attr.Parent = node;
									attr.Name = reader.Name;
									attr.Text = reader.Value;

									node.Children.Add(attr);
								}
								if (singleTag)
								{
									node = node.Parent;
								}
							}
							break;

						case XmlNodeType.Text:
							node.Text = reader.Value;
							break;

						case XmlNodeType.EndElement:
							node = node.Parent;
							break;

						default:
							break;
					}
				}
				if (node.Parent != null)
				{
					throw new Exception("XMLフォーマットエラー_1");
				}
				if (node.Children.Count != 1)
				{
					throw new Exception("XMLフォーマットエラー_2");
				}
				node = node.Children[0];
				node.Parent = null;

				PostLoad(node);

				return node;
			}
		}

		private static void PostLoad(XNode node)
		{
			node.Name = PLTrim(node.Name);
			node.Text = PLTrim(node.Text);

			foreach (XNode child in node.Children)
			{
				PostLoad(child);
			}
		}

		private static string PLTrim(string str)
		{
			if (str == null)
			{
				str = "";
			}
			return str.Trim();
		}

		// ----

		public XNode GetChild(string name)
		{
			foreach (XNode child in this.Children)
			{
				if (string.Compare(child.Name, name, true) == 0)
				{
					return child;
				}
			}
			throw new Exception("ノード <" + name + "> が見つかりません。");
		}
	}
}
