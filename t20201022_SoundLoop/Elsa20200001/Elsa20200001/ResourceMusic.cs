using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte
{
	public class ResourceMusic
	{
		//public DDMusic Dummy = new DDMusic(@"dat\General\muon.wav");

		public DDMusic MUS_BOSS_01 = new DDMusic(@"dat\Mirror of ES\nc213704.mp3");
		public DDMusic MUS_BOSS_01_v300 = new DDMusic(@"dat\Mirror of ES\nc213704_v300.mp3");
		public DDMusic MUS_BOSS_02 = new DDMusic(@"dat\Reda\nc136551.mp3");

		public DDMusic 地鳴り = new DDMusic(@"dat\DovaSyndrome\ゴゴゴ_激しい地鳴り音.mp3");

		public DDMusic Floor_07 = new DDMusic(@"dat\甘茶の音楽工房\moeochirusakura_muon-100-100.mp3");
		public DDMusic Floor_08 = new DDMusic(@"dat\甘茶の音楽工房\kanashiminotexture2_muon-100-100.mp3");
		public DDMusic Floor_01 = new DDMusic(@"dat\甘茶の音楽工房\orb1_muon-100-100.mp3");
		public DDMusic Floor_02 = new DDMusic(@"dat\甘茶の音楽工房\orb2_muon-100-100.mp3");
		public DDMusic Floor_06 = new DDMusic(@"dat\甘茶の音楽工房\kanashiminotexture1_muon-100-100.mp3");

		public ResourceMusic()
		{
			//this.Dummy.Volume = 0.1; // 非推奨
		}
	}
}
