using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// ゲームを終了する。
	/// その場でゲームを終了したい場合にこれを投げること。
	/// なので、これを DDMain2 以外で catch してはならない。
	/// 右上の[X]ボタン、エスケープ押下時も DDEngine からこれを投げる。
	/// </summary>
	public class DDCoffeeBreak : Exception
	{ }
}
