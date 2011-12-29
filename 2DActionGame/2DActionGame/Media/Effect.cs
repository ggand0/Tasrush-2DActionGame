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
	/// <summary>
	/// エフェクト描画のほぼ全般を担当するクラス。
	/// </summary>
	public class Effect : Animation
	{
		#region Member variable
		private static SoundEffect siren = game.Content.Load<SoundEffect>("Audio\\SE\\siren");
		private static SoundEffect lastExplosion = game.Content.Load<SoundEffect>("Audio\\SE\\last_explosion");
		private static SoundEffect playerDeath = game.Content.Load<SoundEffect>("Audio\\SE\\magic");
		/// <summary>
		/// ティウン時に出す弾エフェクトの数。
		/// </summary>
		//public const int deathEffectNum = 4;
		//private const int deathEffectNum = 4;
		/// <summary>
		/// プレイヤーのdeathEffect時のパラメータ。素数にする
		/// </summary>
		private readonly int deathEffectDelayTime = 47;
		private readonly int deathEffectMaxSize = 71;
		private readonly int bossExplosionEffectTime = 180;//360;//180;
		private readonly int dashEffectTime = 25;
		private readonly int stageScreenEffectTime = 41;//41;

		private Stage stage;
		private Vector2 originVector;
		private int repeatTime;
		/// <summary>
		/// プレイヤーのティウン時に撒き散らす弾エフェクト
		/// </summary>
		//private Object[] deathEffects;// = new Object[deathEffectNum];
		private List<Object[]> deathEffects = new List<Object[]>();
		//private Object[] deathEffectsOutSide = new Object[deathEffectNum * 2];

		private float dColor;
		private int counter;
		private int time;
		private bool fadeOut;
		private List<int> count = new List<int>();
		private bool hasPlayedSoundEffect;

		private Texture2D[] textures = new Texture2D[10];
		public Object targetObject { get; private set; }
		public int characterNumber { get; private set; }
		/// <summary>
		/// DeathEffectなど複数パターンがあるときに区別するために設定
		/// </summary>
		public int textureType { get; private set; }
		public int degree { get; private set; }
		/// <summary>
		/// エフェクトを描画し終わったか
		/// </summary>
		public bool hasEffected { get; private set; }

		#endregion
		public Effect(Stage stage)
			: this(stage, null, 0)
		{
		}
		public Effect(Stage stage, Object targetObject, int characterNumber)
		{
			this.stage = stage;
			this.targetObject = targetObject;
			this.characterNumber = characterNumber;
			hasEffected = false;

			Random rnd = new Random();
			int randomNumber = rnd.Next(3);

			switch (randomNumber) {
				case 1: { degree = -135; originVector += new Vector2(48, 32); } break;
				case 2: { degree = -215; originVector += new Vector2(16, 32); } break;
				case 3: { degree = -45; originVector += new Vector2(32, -8); } break;
			}
			counter = -1;
		}

		public virtual void Load(ContentManager content, string texture_name, int textureNum)
		{
			if (textureNum <= 1) {
				texture = content.Load<Texture2D>(texture_name);
			} else {
				for (int i = 0; i < textureNum; i++) textures[i] = content.Load<Texture2D>(texture_name);
			}
		}
		public virtual void Load(ContentManager content, string texture_name, ref Texture2D texture)
		{
			texture = content.Load<Texture2D>(texture_name);
		}

		/// <summary>
		/// Update兼Draw
		/// </summary>
		public void DrawObjectEffects(SpriteBatch spriteBatch)
		{
			if (targetObject.damageEffected) {
				Load(game.Content, "Effect\\5koma_red", 1);
				Update(5, 0, 32, 96, 3, 5);

				spriteBatch.Draw(texture, targetObject.drawPos + targetObject.effectPos/* + originVector*/, rect, Color.White, MathHelper.ToRadians(-degree),
					Vector2.Zero, 1, SpriteEffects.None, .1f);

				if (poseCount >= poseNum) {
					targetObject.damageEffected = false;
					targetObject.isEffected = false;
					hasEffected = true;
				}
			}

			if (targetObject.deathEffected) {// このときにdamageEffectedもtrue!! すでにposeCount == 2はいいのか?
				if (counter == -1) {
					switch (game.random.Next(2)) {
						case 0:
							Load(game.Content, "Effect\\DeathEffect", 1);
							textureType = 0;
							break;
						case 1:
							Load(game.Content, "Effect\\DeathEffect2", 1);
							textureType = 1;
							break;
					}
				}

				switch (textureType) {
					case 0:
						Update(4, 0, 48, 48, 3, 5);
						break;
					case 1:
						Update(3, 0, 64, 64, 3, 5);
						break;
				}

				spriteBatch.Draw(texture, targetObject.drawPos
					+ new Vector2(targetObject.width / 2, targetObject.height / 2), rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

				counter++;

				// 終了処理
				if (poseCount >= poseNum) {
					counter = 0;
					targetObject.deathEffected = false;
					targetObject.hasDeathEffected = true;
					targetObject.isEffected = false;
					hasEffected = true;
				}
			}

			if (targetObject.dashEffected && !targetObject.isJumping) {
				Load(game.Content, "Effect\\dash_ef_32", 1);
				Update(5, 0, 32, 32, 5, 5);//2
				if (targetObject is Character) {
					if ((targetObject as Character).turnsRight)
						spriteBatch.Draw(texture, targetObject.drawPos + new Vector2(-8, targetObject.height / 2 - 8), rect, Color.White);
					else
						spriteBatch.Draw(texture, targetObject.drawPos + new Vector2(24, targetObject.height / 2 - 8), rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0f);
				} else if (targetObject is Bullet)
					spriteBatch.Draw(texture, targetObject.drawPos + new Vector2(24, targetObject.height / 2 - 8), rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0f);

				counter++;
				if (counter > dashEffectTime) {// counter > 5が原因
					counter = 0;
					targetObject.dashEffected = false;
					targetObject.isEffected = false;
					targetObject.hasDashed = true;
					hasEffected = true;
				}
			}
		}
		/// <summary>
		/// Player死亡時のエフェクトを描画するメソッド。ティウンティウン描画
		/// 二重以上に描画したいときは再帰させる。今のところ二重のみ
		/// </summary>
		/// <param name="deathEffectNum">描画する弾の数</param>
		/// <param name="direction">角度の間隔</param>
		/// <param name="defPos">エフェクトを描画する中心の座標</param>
		/// <param name="maxTime">何重にするか</param>
		/// <param name="speed">弾の速度</param>
		/// <param name="time">何重目の描画か。再帰で使う</param>
		public void DrawPlayerDeathEffect(SpriteBatch spriteBatch, int deathEffectNum
			, Vector2 defPos , float direction, float speed, int maxTime, int time)
		{
			// VS2008EE(恐らくC#4.0未対応)だとデフォルト引数が使えない！
			//Vector2 defPos = new Vector2(stage.player.position.X + stage.player.width / 2, stage.player.position.Y + stage.player.height / 2);
			//float direction = 360 / (float)deathEffectNum;
			//float speed = 2;
			int index = maxTime - time;
			if (count.Count == index) count.Add(-1);
			//DrawPlayerDeathEffectOutSide(spriteBatch);
			if (count[index] == -1)	deathEffects.Add(new Object[deathEffectNum]);
			if (time == 1) DrawPlayerDeathEffect(spriteBatch, deathEffectNum * 2, defPos, 360 / (float)(deathEffectNum * 2), speed + 1, maxTime, time - 1);

			// 等幅で飛ぶようにspeedを与える。
			float size = count[index] != 0 ? deathEffectMaxSize % count[index] / (float)deathEffectDelayTime : 0;
			if (count[index] % deathEffectMaxSize == 0) {
				if (index == 0)	repeatTime++;
				count[index] = 0;
				deathEffects.Add(new Object[deathEffectNum]);

				if (!game.isMuted) playerDeath.Play(SoundControl.volumeAll, 0f, 0f);
			}

			for (int i = 0; i < deathEffectNum; i++) {
				if (count[index] == -1) {
					deathEffects[index][i] = new Object(stage, defPos.X, defPos.Y, 25, 25);
					deathEffects[index][i].Load(game.Content, "Effect\\playerDeathEffect0");
				} else if (count[index] == 0) {
					deathEffects[index][i].isActive = true;
					stage.unitToAdd.Add(deathEffects[index][i]);

					deathEffects[index][i].speed.X = (float)Math.Cos(MathHelper.ToRadians(direction * i)) * speed;
					deathEffects[index][i].speed.Y = (float)Math.Sin(MathHelper.ToRadians(direction * i)) * speed;
				} else if (deathEffectMaxSize % count[index] == 0) {// counter % deathEffectDelayTime == 0
					foreach (Object obj in deathEffects[index]) obj.position = defPos;
				} else {
					deathEffects[index][i].position += deathEffects[index][i].speed;
					deathEffects[index][i].drawPos.X = deathEffects[index][i].position.X - stage.camera.position.X;// スクロールもさせちゃう
					deathEffects[index][i].drawPos.Y = deathEffects[index][i].position.Y;
					deathEffects[index][i].animation.Update(4, 0, 25, 25, 12, 1);

					spriteBatch.Draw(deathEffects[index][i].texture, deathEffects[index][i].drawPos, deathEffects[index][i].animation.rect, Color.White, 0, Vector2.Zero, new Vector2(size), SpriteEffects.None, 0);
					// debug : //spriteBatch.DrawString(game.Arial, (deathEffectMaxSize % counter).ToString(), new Vector2(0, 250), Color.Orange);
				}
			}

			count[index]++;
			//if (counter > deathEffectMaxSize * 2) stage.hasEffectedPlayerDeath = true;	// 120
			if (repeatTime > 2) stage.hasEffectedPlayerDeath = true;
		}
		/*public void DrawPlayerDeathEffectOutSide(SpriteBatch spriteBatch)
		{
			Vector2 defPos = new Vector2(stage.player.position.X + stage.player.width / 2, stage.player.position.Y + stage.player.height / 2);
			float direction = 360 / (float)(deathEffectNum * 2);
			float speed = 3;

			// 等幅で飛ぶようにspeedを与える。
			float size = counter != 0 ? deathEffectMaxSize % counter / (float)deathEffectDelayTime : 0;

			for (int i = 0; i < deathEffectNum * 2; i++) {
				if (counter == -1) {
					deathEffectsOutSide[i] = new Object(stage, defPos.X, defPos.Y, 25, 25);
					deathEffectsOutSide[i].Load(game.Content, "Effect\\playerDeathEffect0");
				} else if (counter == 0) {
					deathEffectsOutSide[i].isActive = true;
					stage.unitToAdd.Add(deathEffectsOutSide[i]);

					deathEffectsOutSide[i].speed.X = (float)Math.Cos(MathHelper.ToRadians(direction * i)) * speed;
					deathEffectsOutSide[i].speed.Y = (float)Math.Sin(MathHelper.ToRadians(direction * i)) * speed;
				} else if (deathEffectMaxSize % counter == 0) {
					foreach (Object obj in deathEffectsOutSide) obj.position = defPos;
				} else {
					deathEffectsOutSide[i].position += deathEffectsOutSide[i].speed;
					deathEffectsOutSide[i].drawPos.X = deathEffectsOutSide[i].position.X - stage.camera.position.X;
					deathEffectsOutSide[i].drawPos.Y = deathEffectsOutSide[i].position.Y;
					deathEffectsOutSide[i].animation.Update(4, 0, 25, 25, 6, 1);

					spriteBatch.Draw(deathEffectsOutSide[i].texture, deathEffectsOutSide[i].drawPos, deathEffectsOutSide[i].animation.rect, Color.White, 0, Vector2.Zero, new Vector2(size), SpriteEffects.None, 0);
				}
			}
		}*/

		/// <summary>
		/// screen関係
		/// </summary>
		public void DrawBossScreenEffect(SpriteBatch spriteBatch, int effectTime)
		{
			Load(game.Content, "Effect\\scrEffect_Boss", ref textures[0]);// 値渡し?←関数の引数は基本値渡し！
			Load(game.Content, "Effect\\scrEffect_Boss1", ref textures[1]);
			Load(game.Content, "Effect\\scrEffect_Boss2", ref textures[2]);

			if (time == 0) {
				if (!game.isMuted) siren.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (time < effectTime) {
				spriteBatch.Draw(textures[0], Vector2.Zero, Color.White);// ぬるぽ new Vector2(100, 100)
				//if (game.stageNum == 1) spriteBatch.Draw(textures[1], new Vector2(100, 100), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
				//else if (game.stageNum == 2 || game.stageNum == 5) spriteBatch.Draw(textures[2], new Vector2(100, 100), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
			} else {
				stage.hasEffectedWarning = true;
				time = 0;
			}

			time++;
		}
		Vector2 adj;
		/// <summary>
		/// ボス死亡時の爆発エフェクト。適当なので後で直したい
		/// </summary>
		/// <param name="targetBoss">エフェクトを描画する対象のボス</param>
		public void DrawBossDeathEffect(SpriteBatch spriteBatch, Boss targetBoss)
		{
			Load(game.Content, "Effect\\DeathEffect2", ref textures[0]);// 値渡し?←関数の引数は基本値渡し！
			Update(3, 0, 64, 64, 4, 1);//912
			//bossExplosionEffectTime = 360;

			if (!hasPlayedSoundEffect) {
				if (!game.isMuted) lastExplosion.Play(SoundControl.volumeAll, 0f, 0f);
				hasPlayedSoundEffect = true;
			} else {
				// = new Vector2();
				if (stage.gameTimeNormal % 4 == 0) adj = new Vector2(game.random.Next(-32, 32), game.random.Next(-32, 32));
				
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 2, targetBoss.height / 2) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 2 - 50, targetBoss.height / 2 + 50) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 2, targetBoss.height / 2 - 30) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width, targetBoss.height + 50) + adj, rect, Color.White);/**/

				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 2 - 50, targetBoss.height + 50) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 3 + 100, targetBoss.height / 2 + 50) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width , targetBoss.height / 2) + adj, rect, Color.White);
				spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width, targetBoss.height / 2 - 50) + adj, rect, Color.White);
				/*for (int i = 0; i < 10; i++) {
					if (time > i * 10)
					spriteBatch.Draw(textures[0], targetBoss.drawPos + new Vector2(targetBoss.width / 2, targetBoss.height / 2) + new Vector2(game.random.Next(targetBoss.width / 2) * 3f, game.random.Next(targetBoss.height / 2) * 3f), rect, Color.White);
				}*/
			}
			if (time > bossExplosionEffectTime) {
				stage.hasEffectedBossExplosion = true;
				time = 0;
				hasPlayedSoundEffect = false;
			}
			time++;
		}
		/// <summary>
		/// Stage開始時のエフェクトを描画するメソッド。
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="stageNum">game.stageNum</param>
		/// <param name="effectTime">描画時間</param>
		public void DrawStageScreenEffect(SpriteBatch spriteBatch, int stageNum, int effectTime)
		{
			Load(game.Content, "Effect\\scrEffect_Stage1", ref textures[0]);
			Load(game.Content, "Effect\\scrEffect_Stage2", ref textures[1]);

			switch (stageNum) {
				case 1:
					Load(game.Content, "Effect\\scrEffect_st01", ref textures[2]);
					Load(game.Content, "Effect\\scrEffect_st02", ref textures[3]);
					break;
				case 2:
					Load(game.Content, "Effect\\scrEffect_st11", ref textures[2]);
					Load(game.Content, "Effect\\scrEffect_st12", ref textures[3]);
					break;
				case 3:
					Load(game.Content, "Effect\\scrEffect_st21", ref textures[2]);
					Load(game.Content, "Effect\\scrEffect_st22", ref textures[3]);
					break;
			}

			if (stageNum != 0 && stageNum != 5) {// debug stage以外なら
				if (!fadeOut && time < effectTime) {
					spriteBatch.Draw(textures[2], Vector2.Zero, Color.White);//new Vector2(0, 200)
				} else if (time >= effectTime && !fadeOut) {
					fadeOut = true;
					time = 0;
					dColor = 1.0f;
				}

				if (fadeOut && time < stageScreenEffectTime) {
					if (!stage.isPausing) {
						if (/*dColor > .5f || */time % 2 == 0) dColor -= .1f;
						if (dColor <= 0) dColor = 0;
					}

					spriteBatch.Draw(textures[2], Vector2.Zero, Color.White * dColor);//new Color(255, 255, 255, dColor));
				} else if (fadeOut && time >= stageScreenEffectTime) {
					time = 0;
					fadeOut = false;
					stage.hasEffectedBeginning = true;
				}
			}
			if (!stage.isPausing) time++;
		}
	}
}