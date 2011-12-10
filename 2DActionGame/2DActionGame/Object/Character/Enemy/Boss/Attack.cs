using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{
	public class Attack
	{
		// Member
		/// <summary>
		/// 動的にInvokeしたいメソッド本体のデリゲート
		/// </summary>
		private Delegate attackMethod;
		/// <summary>
		/// attackMethodに渡す”引数の配列”（配列の引数ではない）
		/// </summary>
		private object[] augs;
		/// <summary>
		/// attackMethodが必要とする引数の数。
		/// TargetParameterCountException回避のために使う
		/// </summary>
		private int augNum;

		/// <summary>
		/// 参照元のメソッドを呼び出す
		/// </summary>
		public void DynamicInvoke()
		{

		}

		// Constructor
		public Attack(Delegate attackMethod, int augNum, object[] augs)
		{
			this.attackMethod = attackMethod;
			this.augNum = augNum;
			this.augs = augs;
		}
	}
}
