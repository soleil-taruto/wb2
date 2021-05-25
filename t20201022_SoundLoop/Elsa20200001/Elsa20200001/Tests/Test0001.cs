using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Tests
{
	public class Test0001
	{
		// 注意：ここで調べたサンプル位置は「開始位置・終了位置」なので SetLoopByStEnd を使用すること。

		public void Test01()
		{
			// -- choose one --

			// ちょっと詰まっている感じはするけど、まあいいや @ 2020.10.22

			//Test01a(Ground.I.Music.MUS_BOSS_01, 24000, 5234000);
			//Test01a(Ground.I.Music.MUS_BOSS_01, 26000, 5236000);
			//Test01a(Ground.I.Music.MUS_BOSS_01, 28000, 5238000);
			//Test01a(Ground.I.Music.MUS_BOSS_01, 30000, 5240000); // これを採用

			//Test01a(Ground.I.Music.MUS_BOSS_01_v300, 30000, 5240000); // これで問題無くね？

			// - - -

			//Test01a(Ground.I.Music.MUS_BOSS_02, 625000, 7365000);

			// --

			//Test01a(Ground.I.Music.地鳴り, 44100 + 12345, 44100 * 18 + 12345);

			// --

			//Test01a(Ground.I.Music.Floor_07, 793000, 3930000);
			//Test01a(Ground.I.Music.Floor_08, 654000, 4560000);
			//Test01a(Ground.I.Music.Floor_06, 44100 * 1 + 12345 * 5, 44100 * 97 + 12345 * 5);
			//Test01a(Ground.I.Music.Floor_01, 44100 * 36 + 12345 * 1, 44100 * 84 + 12345 * 1);
			Test01a(Ground.I.Music.Floor_02, 44100 * 10 + 1234 * 4, 44100 * 143 + 1234 * 0);

			// --
		}

		//private const int PLAY_START_POS_FROM_END = 50000;
		//private const int PLAY_START_POS_FROM_END = 100000;
		//private const int PLAY_START_POS_FROM_END = 150000;
		private const int PLAY_START_POS_FROM_END = 300000;

		private void Test01a(DDMusic music, int startPos, int endPos)
		{
			DX.SetLoopSamplePosSoundMem(startPos, music.Sound.GetHandle(0)); // ループ開始位置

			DX.SetLoopStartSamplePosSoundMem(endPos, music.Sound.GetHandle(0)); // ループ終了位置

			DX.SetCurrentPositionSoundMem(endPos - PLAY_START_POS_FROM_END, music.Sound.GetHandle(0));
			DX.ChangeVolumeSoundMem(
				128, // 音量：0 ～ 255
				music.Sound.GetHandle(0)
				);
			DX.PlaySoundMem(music.Sound.GetHandle(0), DX.DX_PLAYTYPE_LOOP, 0);

			for (; ; )
			{
				if (DDInput.A.GetInput() == 1) // リスタート
				{
					DX.StopSoundMem(music.Sound.GetHandle(0));
					DX.SetCurrentPositionSoundMem(endPos - PLAY_START_POS_FROM_END, music.Sound.GetHandle(0));
					DX.PlaySoundMem(music.Sound.GetHandle(0), DX.DX_PLAYTYPE_LOOP, 0);
				}

				DDCurtain.DrawCurtain();

				DDPrint.SetPrint();
				DDPrint.PrintLine("現在の再生位置：" + DX.GetCurrentPositionSoundMem(music.Sound.GetHandle(0)));
				DDPrint.PrintLine("曲の長さ：" + DX.GetSoundTotalSample(music.Sound.GetHandle(0)));
				DDPrint.PrintLine("");
				DDPrint.PrintLine("Z or A-Button == リスタート");

				DDEngine.EachFrame();
			}
		}
	}
}
