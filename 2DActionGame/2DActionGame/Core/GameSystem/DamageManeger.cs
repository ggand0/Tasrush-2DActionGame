using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
	public class DamageManeger
	{
		// WeaponとPlayerでそれぞれやるよりは統一したほうがいいだろう bulletも剣で跳ね返すのに
		public static Game1 game;
		private Stage stage;

		/// <summary>
		/// 攻撃が当たってから何フレーム目でダメージを与えるか 10体まで同時攻撃
		/// </summary>
		public int[] damageTime = new int[100];
		public int[] damagedObjectNum = new int[100];   // Listに直さねば...
		/// <summary>
		/// ダメージを受けたObject.ステージを参照する
		/// </summary>
		public List<Object> damagedObjects { get; set; }
		public List<Object> attackedObjects { get; set; }
		public List<Damage> damages { get; set; }
		public List<Object2> adObjects { get; set; }
		/// <summary>
		/// コンボ中のオブジェクトをしばらく滞在させておくリスト
		/// </summary>
		public List<Object> inComboObjectsTmp { get; set; }
		/// <summary>
		/// 滞在時間
		/// </summary>
		private int remainTime;
		/// <summary>
		/// カウンタ
		/// </summary>
		private int counter;

		public DamageManeger(Stage stage)
		{
			this.stage = stage;

			damagedObjects = new List<Object>();
			damages = new List<Damage>();
		}
		public DamageManeger(Stage stage, List<Object> attackedObjects, List<Object> damagedObjects)
		{

			this.stage = stage;

			//damagedObjects = new List<Object>();
			this.attackedObjects = attackedObjects;
			this.damagedObjects = damagedObjects;
			damages = new List<Damage>();
		}

		/// <summary>
		/// Sword.DamageUpdateを移植
		/// </summary>
		public void Update()
		{
			// Playerと敵の判定に限定→より汎用的にしたい
			if (!stage.player.hasAttacked) {
				for (int i = 0; i < stage.activeObjects.Count; i++) {
					if (stage.activeObjects[i].isDamaged) {											// 最初にtrueになったときのみに限定
						if (stage.activeObjects[i] is Player)
							stage.player.MotionUpdate();											// Raijinの件はここが原因か.HP制にする場合はここを消すだけで行けるような気が...

						if (damagedObjectNum.Any((x) => x == i)) {									// 既にList中にある場合は何もしない
						} else {
							damagedObjects.Add(stage.activeObjects[i]);								// 無い場合（新しくダメージをもらったObjectのとき）Listに追加
							damagedObjectNum[0 + (damagedObjects.Count - 1)/*counter*/] = i;		// 判定済みの敵を記憶
						}
						for (int l = 0; l < damagedObjects.Count; l++) {                            // その敵の判定開始 2回(damageC.Count分)damageTimeが判定されてしまう　面倒だ
							damageTime[l]++;
						}
						for (int m = 0; m < stage.damagedObjects.Count; m++) {
							if (damageTime[0 + m] == 1) {
								stage.activeObjects[damagedObjectNum[m]].MotionUpdate();
							}
						}
						counter++;
						stage.activeObjects[i].isDamaged = false;
					}
				}
			}

		}

		/// <summary>
		/// より汎用的な処理に改良。
		/// </summary>
		public void Update2()
		{
			// Listを別々に作るのではなく、STageで攻撃側、ダメージ側の組み合わせごとのListを作ってそれをもって来るべきだ.
			foreach (Object2 Obj in stage.adObjects) {
				//if (damages.Count > 0 && damages.Any((x) => x.adObject == Obj)) { }
				if (damages.Count > 0 && damages.Any((x) => x.adObject.object1 == Obj.object1 && x.adObject.object2 == Obj.object2)) {
				} else { damages.Add(new Damage(stage, Obj)); }// ダメージを受けた敵の参照を渡す.参照なのでその後敵が画面外に出ても大丈夫. 
				// 12/13:無駄にnewされてた.Obj==x.adObjectだと値で比較されてるだけのようだ？
			}

			// ダメージを受けてるCharacterをstageのListに追加しておく
			//stage.inComboObjects.Clear();	// 毎フレーム初期化ではなくダメージを受け終わったら自らRemoveするようにする
			/*foreach (Damage dmg in damages) {
				if (dmg.adObject.object1 is Player && dmg.adObject.object2 is Enemy) {
					stage.inComboObjects.Add(dmg.adObject.object2);
				}
			}*/

			/*//stage.maxComboCount = 0;
			for (int i = 0; i < stage.inComboObjects.Count; i++)    // 列要素を比較
				if (stage.maxComboCount < (stage.inComboObjects[i] as Enemy).comboCount)
					stage.maxComboCount = (stage.inComboObjects[i] as Enemy).comboCount;*/

			if (damages.Count > 0) {  //null,nullのDamageが追加されてる.←adObjectsを見よ
				for (int i = 0; i < damages.Count; i++) {//zenbu update saretenai youna? atteru!
					damages[i].Update();
					/*if (damages[i].hasDamaged) {													// ダメージを与え終えた(終了フラグ)
						//damages[i].damagedObject.isDamaged = false;
						//damages[i].damagedObject.damageFromAttacking = false;						// とりあえずはisDamagedのみで管理する仕様で試す

						damages[i].adObject.object2.isDamaged = false;								// 一応
						damages.RemoveAt(i);
					}*/
					/*else if(!damages[i].adObject.object2.damageFromAttacking || !damages[i].adObject.object2.damageFromTouching) {// 12/27:原因不明だが意味もなくdamageが残ってたら消す→だめだった
						damages[i].adObject.object2.isDamaged = false;								// 一応
						damages.RemoveAt(i);
					}*/
				}

				// UpdateとRemoveは分ける. 
				/// <summary>
				/// 参考：http://www.atmarkit.co.jp/fdotnet/dotnettips/815listremove/listremove.html
				/// ループ中にコレクションの要素を削除するときは末尾から先頭に向かってループをかければおｋ
				/// </summary>
				for (int i = damages.Count - 1; i >= 0; i--) {
					if (damages[i].hasDamaged) {													// ダメージを与え終えた(終了フラグ)
						//damages[i].damagedObject.isDamaged = false;
						//damages[i].damagedObject.damageFromAttacking = false;						// とりあえずはisDamagedのみで管理する仕様で試す

						damages[i].adObject.object2.isDamaged = false;								// 一応
						damages.RemoveAt(i);
					}
				}
			}

		}
	}
}