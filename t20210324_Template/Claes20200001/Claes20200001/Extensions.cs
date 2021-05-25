using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte
{
	public static class Extensions
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T> action)
		{
			foreach (T element in src)
				action(element);

			return src;
		}

		public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> src, Comparison<T> comp)
		{
#if true
			{
				List<T> list = src.ToList();
				list.Sort(comp);
				src = list;
			}
#else // src自体をソートしても良い場合
			if (src is T[])
			{
				Array.Sort((T[])src, comp);
			}
			else if (src is List<T>)
			{
				((List<T>)src).Sort(comp);
			}
			else
			{
				List<T> list = src.ToList();
				list.Sort(comp);
				src = list;
			}
#endif
			return src;
		}

		public static IEnumerable<T> OrderedDistinct<T>(this IEnumerable<T> src, Func<T, T, bool> match)
		{
			IEnumerator<T> reader = src.GetEnumerator();

			if (reader.MoveNext())
			{
				T last = reader.Current;

				yield return last;

				while (reader.MoveNext())
				{
					if (!match(reader.Current, last))
					{
						last = reader.Current;
						yield return last;
					}
				}
			}
		}
	}
}
