using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{
	/// <summary>
	/// Bossの攻撃ルーチンをクラス化したもの。
	/// まず、Weaponを使うパターンだけで実験
	/// </summary>
	public class Attack
	{
		private Character character;
		private Weapon weapon;
		private int count;
		private bool isAlive, commencingAttack, onAttacking;

		private Delegate attackMethod;
		private object[] augs;

		// Bossがコンストラクタでweaponを生成しておいて（攻撃typeなどの細かい設定は全て予め行っておく）、
		// Weapon使用時のパラメータをいくつか渡す、という形ならできそうな気がするが....
		// 途中で動的にパラメータを変更する時に困る
		// イベントとかを上手く使うべきなんだろうか...
		// そうだデリゲートを使おう

		// 攻撃パターンが終わったかどうかの判定をどうするか：現状のisEndsまで含めないとクラス化する意味が無い
		// 1. 全ての攻撃メソッドに戻り値boolを設定する（攻撃が終わったらfalse）
		// 2. 全てのメソッドの引数にこのクラスのインスタンスの変数を参照させる
		// 3. メソッドへの参照とは別にweaponを持っておいて、それのisBeingUsedで判断する（ただし汎用性が無い）
		// 4. 現状のisEnds[i]をそのまま引き継ぐ
		public Attack(Character character, Weapon weapon, Delegate attackMethod, params object[] augs)
		{
			this.character = character;
			this.weapon = weapon;
			this.attackMethod = attackMethod;
			this.augs = augs;

			isAlive = commencingAttack = true;
		}

		public void Update()
		{
			if (augs == null) {
				attackMethod.DynamicInvoke();
			} else {
				attackMethod.DynamicInvoke(augs);
			}
		}
	}
}
