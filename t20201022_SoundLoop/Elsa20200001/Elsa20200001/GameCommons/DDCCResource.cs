using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDCCResource
	{
		private static Dictionary<string, DDPicture> PictureCache = SCommon.CreateDictionaryIgnoreCase<DDPicture>();

		public static DDPicture GetPicture(string file)
		{
			if (!PictureCache.ContainsKey(file))
				PictureCache.Add(file, DDPictureLoaders.Standard(file));

			return PictureCache[file];
		}

		private static Dictionary<string, DDMusic> MusicCache = SCommon.CreateDictionaryIgnoreCase<DDMusic>();

		public static DDMusic GetMusic(string file)
		{
			if (!MusicCache.ContainsKey(file))
				MusicCache.Add(file, new DDMusic(file));

			return MusicCache[file];
		}

		private static Dictionary<string, DDSE> SECache = SCommon.CreateDictionaryIgnoreCase<DDSE>();

		public static DDSE GetSE(string file)
		{
			if (!SECache.ContainsKey(file))
				SECache.Add(file, new DDSE(file));

			return SECache[file];
		}

		// ====
		// ここから開放
		// ====

		public static void ClearPicture()
		{
			Clear(PictureCache, DDPictureUtils.Pictures, picture => picture.Unload());
		}

		/// <summary>
		/// クリア対象の音楽は停止していること。
		/// -- 再生中に Unload したらマズいのかどうかは不明。多分マズいだろう。
		/// </summary>
		public static void ClearMusic()
		{
			Clear(MusicCache, DDMusicUtils.Musics, music => music.Sound.Unload());
		}

		/// <summary>
		/// クリア対象の効果音は停止していること。
		/// -- 再生中に Unload したらマズいのかどうかは不明。多分マズいだろう。
		/// </summary>
		public static void ClearSE()
		{
			Clear(SECache, DDSEUtils.SEList, se => se.Sound.Unload());
		}

		public static void Clear<K, T>(Dictionary<K, T> cache, List<T> store, Action<T> a_unload)
		{
			HashSet<T> handles = new HashSet<T>(cache
				.Values // KeepComment:@^_ConfuserElsa // NoRename:@^_ConfuserElsa
				);

			foreach (T handle in handles)
				a_unload(handle);

			store.RemoveAll(handle => handles.Contains(handle));
			cache.Clear();
		}
	}
}
