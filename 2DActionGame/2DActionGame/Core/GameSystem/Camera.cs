using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	public class Camera : Object
	{
		public float distanceToPlayer { get; private set; }
		public float distanceToBoss { get; private set; }
		public bool isScrollingToPlayer { get; set; }

		public Camera()
		{
		}
		public Camera(Stage stage, float x, float y, int width, int height)
			: base(stage, x, y, width, height)
		{
			this.stage = stage;
			this.width = width;
			this.height = height;
			this.position.X = x;
			this.position.Y = y;

			speed.X = stage.ScrollSpeed;
		}

		public void ScrollUpdate(Object targetObject)
		{
			this.speed.X = stage.ScrollSpeed;
			distanceToPlayer = (stage.player.position.X - this.position.X);
			if (stage.boss != null) distanceToBoss = stage.boss.position.X - position.X;

			if (!stage.isScrolled && !stage.inBossBattle) {
				NormalScrollUpdate(targetObject);
			} else if (!stage.isScrolled && stage.inBossBattle) {// 限定画面スクロール（端ではスクロールの限界あり）
				if (stage.player.position.X < stage.boss.defaultPosition.X - 640 || stage.player.position.X > stage.boss.defaultPosition.X + 100) {
					EdgeScrollUpdate(targetObject);
				} else {
					//NormalScrollUpdate(targetObject);
					CenterScrollUpdate(targetObject);
				}
			} else {
				AutoScrollUpdate(targetObject);
			}

			// 強制スクロール終了時にPlayer中心のスクロールへ戻す
			if (isScrollingToPlayer) {
				RemoveDistance(stage.player);

                if (distanceToPlayer < 2) {
				//if (distanceToPlayer < 0 && distanceToPlayer > 0) {
					stage.isScrolled = false;
					isScrollingToPlayer = false;
				}
			}
		}
		/// <summary>
		/// 通常スクロール：Playerを中心とした相対座標に変換
		/// </summary>
		private void NormalScrollUpdate(Object targetObject)
		{
			this.position.X = stage.player.position.X;

			if (targetObject is ScrollingBackground) {
				(targetObject as ScrollingBackground).ScrollUpdateBoss(stage.player.position);
			} else {
				targetObject.ScrollUpdate(this.position, false);
			}

			UpdateStageList(targetObject);

			if (stage.boss != null && distanceToBoss < 300) {
				stage.toBossScene = true;										// ボス戦フラグ
			}
		}
		/// <summary>
		/// 強制スクロール：Cameraを中心とした相対座標に変換
		/// </summary>
		/// <param name="targetObject"></param>
		private void AutoScrollUpdate(Object targetObject)
		{
			// 強制スクロール：カメラを中心とした相対座標に変換
			targetObject.ScrollUpdate(this.position, true);

			UpdateStageList(targetObject);
			// ボス戦フラグ
			if (stage.boss != null && distanceToBoss < 300 + 1000) {
				stage.toBossScene = true;
				//stage.isScrolled = false;
			}
			if (stage.toBossScene && distanceToBoss < 300) {
				stage.isScrolled = false;
				stage.inBossBattle = true;
				stage.toBossScene = false;
			}
		}
		/// <summary>
		/// ボス戦時、Playerが端にいる時のスクロール
		/// </summary>
		private void EdgeScrollUpdate(Object targetObject)
		{
			if (targetObject is ScrollingBackground) {
				(targetObject as ScrollingBackground).ScrollUpdateBoss(stage.player.position);
			} else {
				targetObject.ScrollUpdate(this.position, true);//stage.player.position
			}

			UpdateStageList(targetObject);
		}
		/// <summary>
		/// ボス戦時、Playerが画面端にいる時以外のスクロール処理
		/// </summary>
		/// <param name="targetObject"></param>
		private void CenterScrollUpdate(Object targetObject)
		{
			this.position.X = stage.player.position.X - Player.screenPosition.X;

			if (targetObject is ScrollingBackground) {
				(targetObject as ScrollingBackground).ScrollUpdateBoss(this.position);
			} else {
				targetObject.ScrollUpdate(this.position, true);
			}

			UpdateStageList(targetObject);
		}

		// 補助メソッド
		/// <summary>
		/// アクティブでないかによってStageのリストに振り分けるメソッド。
		/// </summary>
		/// <param name="targetObject"></param>
		private void UpdateStageList(Object targetObject)
		{
			if (targetObject is Player) { }
			if (targetObject.distanceToCamara < targetObject.activeDistance) {	// ==画面内に表示されているオブジェクト
				targetObject.isActive = true;
				stage.activeObjects.Add(targetObject);							// まず大元のListに追加

				if (targetObject is Player) { }
				if (targetObject is Character) {								// 以下、それぞれ派生クラスのListに追加していく
					stage.activeCharacters.Add(targetObject as Character);
				} else if (targetObject is Terrain) {
					stage.activeTerrains.Add(targetObject as Terrain);
					if (IsDynamicTetrrain(targetObject)) {
						stage.activeDynamicTerrains.Add(targetObject as Terrain);

						if (targetObject is SnowBall
							|| (targetObject is Block && (targetObject as Block).user != null))
							stage.activeDynamicTerrains1.Add(targetObject as Terrain);
						else
							stage.activeDynamicTerrains2.Add(targetObject as Terrain);
					} else
						stage.activeStaticTerrains.Add(targetObject as Terrain);
				} else if (targetObject is Bullet && ((((targetObject as Bullet).turret != null && (targetObject as Bullet).turret.isBeingUsed)
					  || (targetObject as Bullet).turret == null)))
					stage.activeBullets.Add(targetObject as Bullet);
			} else {
				targetObject.isActive = false;
			}
		}
		private bool IsDynamicTetrrain(Object terrain)
		{
			return terrain is SnowBall
						|| (terrain is Block && (terrain as Block).user != null) //)|| (terrain is DamageBlock && (terrain as DamageBlock).user != null))
						|| terrain is CollapsingBlock
						|| terrain is CollapsingFloor
						|| terrain is Water
						|| terrain is Icicle
						|| terrain is Thread
						|| terrain is Meteor
						|| terrain is DamageObject
						|| terrain is MapObjects
						|| terrain is ConveyorBelt
						|| terrain is TracingFoothold
						|| terrain is PointS
						|| terrain is MapObjects;
		}
		/// <summary>
		/// 強制スクロール解除時に呼ばれる
		/// </summary>
		/// <param name="targetObject"></param>
		public void RemoveDistance(Object targetObject)
		{
			if (distanceToPlayer > 0) {
				this.position.X += 0.05f;		// 0にするとなるとかなり難しい 0.1fでもバグる
			} else if (distanceToPlayer < 0) {
				this.position.X -= 0.05f;
			}
		}
	}
}