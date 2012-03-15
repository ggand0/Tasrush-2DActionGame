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
		/// より汎用的な処理に改良。
		/// </summary>
		public void Update()
		{
			foreach (Object2 Obj in stage.adObjects) {
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

			if (damages.Count > 0) {
				for (int i = 0; i < damages.Count; i++) {
					damages[i].Update();
				}

				/// <summary>
				/// UpdateとRemoveは分ける。
				/// ループ中にコレクションの要素を削除するときは末尾から先頭に向かってループをかければおｋ
				/// </summary>
				/// <see cref="http://www.atmarkit.co.jp/fdotnet/dotnettips/815listremove/listremove.html"/>
				for (int i = damages.Count - 1; i >= 0; i--) {
					if (damages[i].hasDamaged) {													// ダメージを与え終えた(終了フラグ)
						damages[i].adObject.object2.isDamaged = false;								// 一応
						damages.RemoveAt(i);
					}
				}
			}

		}
	}
}