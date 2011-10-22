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
        public bool isScrollingToPlayer;

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
            distanceToPlayer =  (stage.player.position.X - this.position.X);
            if (stage.boss != null) distanceToBoss = stage.boss.position.X - position.X;

            #region Normal Scroll
            // 通常スクロール：Playerを中心とした相対座標に変換 ：危機管理工学：大宮先生は受けなくてよい
			if (!stage.isScrolled && !stage.inBossBattle) {
				/*if (targetObject is ScrollingBackground) {
					if ((targetObject as ScrollingBackground).isFrontal)
						targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .10f + Player.screenX;
					else
						targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .05f + Player.screenX;
				} else {
					targetObject.drawPos.X = targetObject.position.X - stage.player.position.X + Player.screenX;
				}
                // cameraの位置をPlayerに合わせておく
				targetObject.drawPos.Y = targetObject.position.Y;
                this.position.X = stage.player.position.X;
				targetObject.distanceToCamara = Math.Abs(this.position.X - targetObject.position.X);*/
				this.position.X = stage.player.position.X;
				if (targetObject is ScrollingBackground) {
					(targetObject as ScrollingBackground).ScrollUpdateBoss(stage.player.position);
				} else {
					targetObject.ScrollUpdate(this.position, false);
				}

				if (targetObject.distanceToCamara < targetObject.activeDistance) {	// ==画面内に表示されているオブジェクト
					targetObject.isActive = true;
					stage.activeObjects.Add(targetObject);							// まず大元のListに追加

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
                
				if (stage.boss != null && distanceToBoss < 300) {
					stage.toBossScene = true;										// ボス戦フラグ
				}
            }
            #endregion
            #region Boss Screen
            // 限定画面スクロール（端ではスクロールの限界あり）
            else if (!stage.isScrolled && stage.inBossBattle) {
                #region Edge
                if (stage.player.position.X < stage.boss.defaultPosition.X - 640 || stage.player.position.X > stage.boss.defaultPosition.X + 100) {
					//if (stage.player.position.X < stage.boss.defaultPosition.X - 640) this.position.X = stage.player.position.X - Player.screenPosition.X;

					/*if (targetObject is ScrollingBackground) {
						if ((targetObject as ScrollingBackground).isFrontal)
							targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .10f + Player.screenPosition.X;
						else
							targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .05f + Player.screenPosition.X;
					} else {
						// ここはScrollUpdateをoverrideさせるべきか？
						targetObject.drawPos.X = targetObject.position.X - stage.player.position.X + Player.screenPosition.X;
						targetObject.drawPos.X = targetObject.position.X - this.position.X;
					}
                    targetObject.drawPos.Y = targetObject.position.Y;
                    targetObject.distanceToCamara = Math.Abs(this.position.X - targetObject.position.X);
					*/
					if (targetObject is ScrollingBackground) {
						(targetObject as ScrollingBackground).ScrollUpdateBoss(stage.player.position);
					} else {
						targetObject.ScrollUpdate(this.position, true);//stage.player.position
					}

					if (targetObject.distanceToCamara < 1800) {
						targetObject.isActive = true;
						stage.activeObjects.Add(targetObject);

						if (targetObject is Character) {
							stage.activeCharacters.Add(targetObject as Character);
						} else if (targetObject is Terrain) {
							stage.activeTerrains.Add(targetObject as Terrain);
							if (IsDynamicTetrrain(targetObject)) {
								stage.activeDynamicTerrains.Add(targetObject as Terrain);
								if (!(targetObject is Icicle) && (targetObject is SnowBall
									|| (targetObject is Block && (targetObject as Block).user != null))) {
									stage.activeDynamicTerrains1.Add(targetObject as Terrain);
								} else
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
                #endregion
                #region Center
                else {
					/*if (targetObject is ScrollingBackground) {
						if ((targetObject as ScrollingBackground).isFrontal)
							targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .10f + Player.screenPosition.X;// 変わらない()
						else
							targetObject.drawPos.X = targetObject.position.X - stage.player.position.X * .05f + Player.screenPosition.X;
					} else {
						targetObject.drawPos.X = targetObject.position.X - stage.player.position.X + Player.screenPosition.X;
					}
                    // 画面端以外（真ん中）はPlayer中心のスクロール
					targetObject.drawPos.Y = targetObject.position.Y;
                    this.position.X = stage.player.position.X - Player.screenPosition.X;
                    targetObject.distanceToCamara = Math.Abs(this.position.X - targetObject.position.X);*/

					this.position.X = stage.player.position.X - Player.screenPosition.X;
					if (targetObject is ScrollingBackground) {
						(targetObject as ScrollingBackground).ScrollUpdateBoss(this.position);
					} else {
						targetObject.ScrollUpdate(this.position, true);
					}

					if (targetObject.distanceToCamara < 1800) {
						targetObject.isActive = true;
						stage.activeObjects.Add(targetObject);
						if (targetObject is Bullet && !(targetObject is Thunder)) { }

						if (targetObject is Character) {
							stage.activeCharacters.Add(targetObject as Character);
						} else if (targetObject is Terrain) {
							stage.activeTerrains.Add(targetObject as Terrain);
							if (IsDynamicTetrrain(targetObject)) {
								stage.activeDynamicTerrains.Add(targetObject as Terrain);

								if (!(targetObject is Icicle) && (targetObject is SnowBall
									|| (targetObject is Block && (targetObject as Block).user != null))) {
									stage.activeDynamicTerrains1.Add(targetObject as Terrain);
								} else
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
                #endregion
            }
            #endregion
            #region Auto Scroll
            else {
                // 強制スクロール：カメラを中心とした相対座標に変換
                /*targetObject.drawPos.Y = targetObject.position.Y;
                targetObject.drawPos.X = targetObject.position.X - this.position.X;
                targetObject.distanceToCamara = Math.Abs(this.position.X - targetObject.position.X);*/
				targetObject.ScrollUpdate(this.position, true);

				if (targetObject.distanceToCamara < targetObject.activeDistance) {
					targetObject.isActive = true;
					stage.activeObjects.Add(targetObject);

					if (targetObject is Character) {
						stage.activeCharacters.Add(targetObject as Character);
					} else if (targetObject is Terrain) {
						stage.activeTerrains.Add(targetObject as Terrain);

						if (IsDynamicTetrrain(targetObject)) {
							stage.activeDynamicTerrains.Add(targetObject as Terrain);

							if (targetObject is SnowBall || (targetObject is Block && ((targetObject as Block).user != null && (targetObject as Block).user != this)))
								stage.activeDynamicTerrains1.Add(targetObject as Terrain);
							else
								stage.activeDynamicTerrains2.Add(targetObject as Terrain);
						} else {
							stage.activeStaticTerrains.Add(targetObject as Terrain);
						}
					} else if (targetObject is Bullet && ((((targetObject as Bullet).turret != null && (targetObject as Bullet).turret.isBeingUsed)
						  || (targetObject as Bullet).turret == null))) {
						stage.activeBullets.Add(targetObject as Bullet);
					}
				} else {
					targetObject.isActive = false;
				}

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

            // 強制スクロール終了時にPlayer中心のスクロールへ戻す
			if (isScrollingToPlayer) {
				RemoveDistance(stage.player);

				if (distanceToPlayer < 1 && distanceToPlayer > -1) {
					stage.isScrolled = false;
					isScrollingToPlayer = false;
				}
			}
            #endregion
        }

		// 補助メソッド
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
        public void RemoveDistance(Object targetObject)
        {
            if (distanceToPlayer > 1) {
				this.position.X += 0.05f;		// 0にするとなるとかなり難しい 0.1fでもバグる
			} else if (distanceToPlayer < -1) {
				this.position.X -= 0.05f;
			}
        }
    }
}
