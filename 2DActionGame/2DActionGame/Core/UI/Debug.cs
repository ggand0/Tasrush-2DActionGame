using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace _2DActionGame
{
    public class Debug
    {
		private Game1 game;
		private Stage stage;
        private Color[] fontColor = new Color[6];
		private float maxValue;

        public Debug(Game1 game, Stage stage)
        {
            this.game = game;
            this.stage = stage;

            fontColor[0] = Color.Blue;
            fontColor[1] = Color.Orange;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // デバッグ情報
			if (game.inDebugMode) {
				switch (game.stageNum) {
					#region Stage0
					case 0:
						// General:
						spriteBatch.DrawString(game.Arial2, "stage.isScrolled:" + stage.isScrolled.ToString(), new Vector2(0, 0), fontColor[0]);
						spriteBatch.DrawString(game.Arial2, "ReversePower:" + stage.reverse.ReversePower.ToString(), new Vector2(0, 10), fontColor[0]);
						// Player
						if (stage.characters.Count > 1) {
							spriteBatch.DrawString(game.Arial2, "player.vector" + stage.player.position.X.ToString(), new Vector2(0, 50), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.speed.X" + stage.player.speed.X.ToString(), new Vector2(0, 60), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.speed.Y" + stage.player.speed.Y.ToString(), new Vector2(0, 70), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isHit:" + stage.player.isHit.ToString(), new Vector2(0, 80), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.normalComboCount:" + stage.player.normalComboCount.ToString(), new Vector2(0, 90), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isDamaged:" + stage.player.isDamaged.ToString(), new Vector2(0, 100), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.jumpCount:" + stage.player.jumpCount.ToString(), new Vector2(0, 110), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.jumpTime:" + stage.player.jumpTime.ToString(), new Vector2(0, 120), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isToChargingMotion:" + stage.player.isToChargingMotion.ToString(), new Vector2(0, 130), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isChargingPower:" + stage.player.isChargingPower.ToString(), new Vector2(0, 140), fontColor[0]);
							if (stage.player.locus.Count > 1)
								spriteBatch.DrawString(game.Arial2, "player.prevVector[1]:" + stage.player.locus[1].ToString(), new Vector2(0, 150), fontColor[0]);
						}
						// else Character
						if (stage.characters.Count > 2) {
							//spriteBatch.DrawString(game.Arial2, "characters[1].totalHits:"  + stage.characters[4].totalHits.ToString(), new Vector2(0, 40), Color.Blue);
							//spriteBatch.DrawString(game.Arial2, "characters[1].comboCount:" + stage.characters[1].comboCount.ToString(), new Vector2(0, 50), Color.Blue);
						}
						for (int i = 0; i < stage.characters.Count; i++) {
							if (stage.characters.Any((x) => x is ShootingEnemy))
								if (stage.characters[i] is ShootingEnemy) {//.Any((x) => x is ShootingEnemy)) {//(x) => x == i))
									ShootingEnemy shootingEnemy = stage.characters[i] as ShootingEnemy;
									//spriteBatch.DrawString(game.Arial2, "shootingEnemy.bullet1.isShot:" + shootingEnemy.bullet1.isShot.ToString(), new Vector2(0, 200), Color.Blue);
								}
						}
						// CD
						spriteBatch.DrawString(game.Arial2, "CD.CrossPoint:" + CollisionDetection.crossPoint.ToString(), new Vector2(0, 200), fontColor[0]);
						spriteBatch.DrawString(game.Arial2, "CD.crossPoints[0]:" + CollisionDetection.crossPoints[0].ToString(), new Vector2(0, 210), fontColor[0]);
						spriteBatch.DrawString(game.Arial2, "CD.crossPoints[1]:" + CollisionDetection.crossPoints[1].ToString(), new Vector2(0, 220), fontColor[0]);
						spriteBatch.DrawString(game.Arial2, "CD.crossPoints[2]:" + CollisionDetection.crossPoints[2].ToString(), new Vector2(0, 230), fontColor[0]);
						//spriteBatch.DrawString(game.Arial2, "devided2.length:" + stage..ToString(), new Vector2(0, 240), fontColor[0]);
						break;
					#endregion
					#region Stage1, 2, 3
					case 1:
						// Player
						if (stage.characters.Count > 1) {
							spriteBatch.DrawString(game.Arial2, "player.vector" + stage.player.position.ToString(), new Vector2(0, 50), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.speed" + stage.player.speed.ToString(), new Vector2(0, 60), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isDamaged" + stage.player.isDamaged.ToString(), new Vector2(0, 70), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isHit:" + stage.player.isHit.ToString(), new Vector2(0, 80), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.normalComboCount:" + stage.player.normalComboCount.ToString(), new Vector2(0, 90), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isJumping:" + stage.player.isJumping.ToString(), new Vector2(0, 100), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpCount:" + stage.player.jumpCount.ToString(), new Vector2(0, 110), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpTime:" + stage.player.jumpTime.ToString(), new Vector2(0, 120), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isToChargingMotion:" + stage.player.isToChargingMotion.ToString(), new Vector2(0, 130), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isChargingPower:" + stage.player.isChargingPower.ToString(), new Vector2(0, 140), fontColor[1]);
							if (stage.player.locus.Count > 1)
								spriteBatch.DrawString(game.Arial2, "player.prevVector[1]:" + stage.player.locus[1].ToString(), new Vector2(0, 150), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.hasAttacked:" + stage.player.hasAttacked.ToString(), new Vector2(0, 160), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "game.score:" + game.score.ToString(), new Vector2(0, 170), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isOnSomething:" + stage.player.isOnSomething.ToString(), new Vector2(0, 180), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isHitLeftSide:" + stage.player.isHitLeftSide.ToString(), new Vector2(0, 190), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "boss.position:" + stage.boss.position.ToString(), new Vector2(0, 200), fontColor[1]);
						}
						break;
					case 2:
						// Player
						if (stage.characters.Count > 1) {
							spriteBatch.DrawString(game.Arial2, "player.vector" + stage.player.position.X.ToString(), new Vector2(0, 50), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.speed.X" + stage.player.speed.X.ToString(), new Vector2(0, 60), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.speed.Y" + stage.player.speed.Y.ToString(), new Vector2(0, 70), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isHit:" + stage.player.isHit.ToString(), new Vector2(0, 80), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.normalComboCount:" + stage.player.normalComboCount.ToString(), new Vector2(0, 90), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isDamaged:" + stage.player.isDamaged.ToString(), new Vector2(0, 100), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpCount:" + stage.player.jumpCount.ToString(), new Vector2(0, 110), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpTime:" + stage.player.jumpTime.ToString(), new Vector2(0, 120), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isToChargingMotion:" + stage.player.isToChargingMotion.ToString(), new Vector2(0, 130), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isChargingPower:" + stage.player.isChargingPower.ToString(), new Vector2(0, 140), fontColor[1]);
							if (stage.player.locus.Count > 1)
								spriteBatch.DrawString(game.Arial2, "player.prevVector[1]:" + stage.player.locus[1].ToString(), new Vector2(0, 150), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.hasAttacked:" + stage.player.hasAttacked.ToString(), new Vector2(0, 160), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "sword.isHit:" + stage.player.sword.isHit.ToString(), new Vector2(0, 170), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "game.score:" + game.score.ToString(), new Vector2(0, 180), fontColor[1]);
							//spriteBatch.DrawString(game.Arial2, "boss.Turret:" + (stage.boss as Fuujin).cutterTurret5Way.defaultPosition.ToString(), new Vector2(0, 180), fontColor[1]);
							if (stage.boss is Fuujin) {
								spriteBatch.DrawString(game.Arial2, "boss.Turret:" + (stage.boss as Fuujin).cutterTurret5Way.position.ToString(), new Vector2(0, 190), fontColor[1]);
								spriteBatch.DrawString(game.Arial2, "boss.dFA:" + (stage.boss as Fuujin).damageFromAttacking.ToString(), new Vector2(0, 200), fontColor[1]);
							}
						}
						break;
					case 3:
						// Player
						if (stage.characters.Count > 1) {
							spriteBatch.DrawString(game.Arial2, "player.vector" + stage.player.position.X.ToString(), new Vector2(0, 50), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.speed.X" + stage.player.speed.X.ToString(), new Vector2(0, 60), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.speed.Y" + stage.player.speed.Y.ToString(), new Vector2(0, 70), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isHit:" + stage.player.isHit.ToString(), new Vector2(0, 80), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.normalComboCount:" + stage.player.normalComboCount.ToString(), new Vector2(0, 90), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isDamaged:" + stage.player.isDamaged.ToString(), new Vector2(0, 100), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpCount:" + stage.player.jumpCount.ToString(), new Vector2(0, 110), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.jumpTime:" + stage.player.jumpTime.ToString(), new Vector2(0, 120), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isToChargingMotion:" + stage.player.isToChargingMotion.ToString(), new Vector2(0, 130), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.isChargingPower:" + stage.player.isChargingPower.ToString(), new Vector2(0, 140), fontColor[1]);
							if (stage.player.locus.Count > 1)
								spriteBatch.DrawString(game.Arial2, "player.prevVector[1]:" + stage.player.locus[1].ToString(), new Vector2(0, 150), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "player.hasAttacked:" + stage.player.hasAttacked.ToString(), new Vector2(0, 160), fontColor[1]);
							spriteBatch.DrawString(game.Arial2, "game.score:" + game.score.ToString(), new Vector2(0, 170), fontColor[1]);
							if (stage.boss is Rival) {
								spriteBatch.DrawString(game.Arial2, "boss.syurikenTurret:" + (stage.boss as Rival).syuriken.isBeingUsed.ToString(), new Vector2(0, 180), fontColor[1]);
								spriteBatch.DrawString(game.Arial2, "boss.syurikenTurret.bullets[0]:" + (stage.boss as Rival).syuriken.bullets[0].position.ToString(), new Vector2(0, 190), fontColor[1]);
                                spriteBatch.DrawString(game.Arial2, "boss.position" + stage.boss.drawPos.ToString(), new Vector2(0, 200), fontColor[1]);
                                spriteBatch.DrawString(game.Arial2, "boss.speed" + stage.boss.speed.ToString(), new Vector2(0, 210), fontColor[1]);
								spriteBatch.DrawString(game.Arial2, "boss.max_speed" + maxValue.ToString(), new Vector2(0, 220), fontColor[1]);
							}
							for (int i = 0; i < stage.boss.attackList.Count; i++) {
								spriteBatch.DrawString(game.Arial2, "boss.attackList:" + stage.boss.attackList[i].ToString(), new Vector2(0, 230 + i * 10), fontColor[1]);
							}
							if (stage.boss.speed.Y > maxValue) maxValue = stage.boss.speed.Y;
						}
						break;
					#endregion
					#region Stage5(BossTest)
					case 5:
						// General:
						spriteBatch.DrawString(game.Arial2, "stage.isScrolled:" + stage.isScrolled.ToString(), new Vector2(0, 0), fontColor[0]);
						spriteBatch.DrawString(game.Arial2, "ReversePower:" + stage.reverse.ReversePower.ToString(), new Vector2(0, 10), fontColor[0]);
						// Player
						if (stage.characters.Count > 1) {
							spriteBatch.DrawString(game.Arial2, "player.vector" + stage.player.position.X.ToString(), new Vector2(0, 50), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.speed.X" + stage.player.speed.X.ToString(), new Vector2(0, 60), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.speed.Y" + stage.player.speed.Y.ToString(), new Vector2(0, 70), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isHit:" + stage.player.isHit.ToString(), new Vector2(0, 80), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.normalComboCount:" + stage.player.normalComboCount.ToString(), new Vector2(0, 90), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isDamaged:" + stage.player.isDamaged.ToString(), new Vector2(0, 100), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.jumpCount:" + stage.player.jumpCount.ToString(), new Vector2(0, 110), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.jumpTime:" + stage.player.jumpTime.ToString(), new Vector2(0, 120), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isToChargingMotion:" + stage.player.isToChargingMotion.ToString(), new Vector2(0, 130), fontColor[0]);
							spriteBatch.DrawString(game.Arial2, "player.isChargingPower:" + stage.player.isChargingPower.ToString(), new Vector2(0, 140), fontColor[0]);
							if (stage.player.locus.Count > 1)
								spriteBatch.DrawString(game.Arial2, "player.prevVector[1]:" + stage.player.locus[1].ToString(), new Vector2(0, 150), fontColor[0]);
						}
						// Boss
						foreach (Character character in stage.characters) {
							if (stage.characters.Any((x) => x is Raijin))
								if (character is Raijin) {
									Raijin raijin = character as Raijin;
									spriteBatch.DrawString(game.Arial2, "raijin.vector:" + raijin.position.ToString(), new Vector2(0, 180), fontColor[0]);
									spriteBatch.DrawString(game.Arial2, "raijin.isStaritingAttack:" + raijin.isStartingAttack.ToString(), new Vector2(0, 190), fontColor[0]);
									spriteBatch.DrawString(game.Arial2, "raijin.isAttacking:" + raijin.isAttacking.ToString(), new Vector2(0, 200), fontColor[0]);
									spriteBatch.DrawString(game.Arial2, "raijin.isEndingAttack:" + raijin.isEndingAttack.ToString(), new Vector2(0, 210), fontColor[0]);
									//spriteBatch.DrawString(game.Arial2, "distanceToPlayer:"               + raijin.distanceD.ToString(), new Vector2(0, 220), fontColor[0]);
								}
							if (character is Rival) {
								Rival rival = character as Rival;
								spriteBatch.DrawString(game.Arial2, "boss.vector:" + rival.position.ToString(), new Vector2(0, 180), fontColor[0]);
								spriteBatch.DrawString(game.Arial2, "boss.isStaritingAttack:" + rival.isStartingAttack.ToString(), new Vector2(0, 190), fontColor[0]);
								spriteBatch.DrawString(game.Arial2, "boss.isAttacking:" + rival.isAttacking.ToString(), new Vector2(0, 200), fontColor[0]);
								spriteBatch.DrawString(game.Arial2, "boss.isEndingAttack:" + rival.isEndingAttack.ToString(), new Vector2(0, 210), fontColor[0]);
								spriteBatch.DrawString(game.Arial2, "boss.isEndingAttack:" + rival.Left.ToString(), new Vector2(0, 220), fontColor[0]);
								spriteBatch.DrawString(game.Arial2, "boss.turnsRight:" + rival.turnsRight.ToString(), new Vector2(0, 230), fontColor[0]);
							}
						}

						spriteBatch.DrawString(game.Arial2, "boss.HP:" + stage.boss.HP.ToString(), new Vector2(0, 260), Color.Red);
						break;
					#endregion
				}
			}
        }
    }
}
