using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
	public class EffectControl
	{
		public static Game1 game;
		Stage stage;

		//public int[] damageTime = new int[100];      // 攻撃が当たってから何フレーム目でダメージを与えるか 10体まで同時攻撃
		//public int[] damagedObjectNum = new int[100];// Listに直さねば...
		//public List<Object> damagedObjects { get; set; }// = new List<Object>();
		//private int counter;
		public List<Effect> effects { get; set; }//effects = new List<Effect>();

		public EffectControl(Stage stage)
		{
			this.stage = stage;
			effects = new List<Effect>();
		}

		// ここではじかれる。直前のdamageEffectのせいか？
		private void NeedEffect(Object targetObj, int i)
		{
			if (effects.Count > 0 && effects.Any((x) => x.targetObject == (targetObj))) { }//x.characterNumber == i)) {} // ==
					else {
				if (stage.activeObjects[i] is Enemy && (targetObj as Enemy).deathEffected) { }
				effects.Add(new Effect(stage, targetObj, i));
			}
		}
		/// <summary>
		/// 各Effectの管理。class Effectで済ませてしまいたかったが、ダメージ調整と違って何フレームかに渡って描画するため別にせざるを得なかった。
		/// </summary>
		public void Update()
		{
			// 2重に描画されるのはactiveObjectsのせいだろうか←activeObjectsの要素は毎フレーム更新される(画面上に表示されているObjectsのListだから)
			// ので、当然同じObjectでもListにおけるindexは変わる. しかしstage.objectsだと数が多すぎて重くなる原因になる.かといってTerrainとかWeaponごとにコンストラクタとDrawメソッドを用意するのも面倒.
			// どうしようか...

			// EffectはObjectについてくる必要はないならactiveObjectsで十分なはず. しかしできれば(追従するような)汎用的な仕様にしたい
			// indexじゃなく"=="で同じObjectsと判定できるならそれを使えないか?←ぱっとみ上手くいっているようだ....多分大丈夫
			for (int i = 0; i < stage.activeObjects.Count; i++) {
				if (stage.activeObjects[i] is JumpingEnemy && stage.activeObjects[i].HP <= 0) { }
				if (stage.activeObjects[i] is JumpingEnemy && stage.activeObjects[i].HP <= 0 && stage.activeObjects[i].deathEffected) { }

				if (stage.activeObjects[i].isEffected && !stage.activeObjects[i].hasDeathEffected) {
					if (stage.activeObjects[i].deathEffected) { }
					//NeedEffect(stage.activeObjects[i], i);
					if (effects.Count > 0 && effects.Any((x) => x.targetObject.Equals(stage.activeObjects[i]))) { }
						//x.characterNumber == i)) {} // ==
					else {
						if (stage.activeObjects[i] is Enemy && (stage.activeObjects[i] as Enemy).deathEffected) { }
						if (stage.activeObjects[i] is Fuujin) { }
						effects.Add(new Effect(stage, stage.activeObjects[i], i));
					}/**/
				}
			}
			if (effects.Count > 0) {
				for (int j = 0; j < effects.Count; j++) {
					effects[j].DrawObjectEffects(game.spriteBatch);
					if (effects[j].hasEffected) {
						// 参照で.
						//stage.activeObjects[effects[i].characterNumber].isEffected = false;// index error
						//stage.activeObjects[effects[i].characterNumber].damageEffected = false;

						effects[j].targetObject.isEffected = false;
						effects[j].targetObject.damageEffected = false;

						effects.RemoveAt(j);
					}
				}
			}

		}
	}
}
