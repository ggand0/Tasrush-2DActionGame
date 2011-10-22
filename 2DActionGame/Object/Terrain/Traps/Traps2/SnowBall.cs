using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
	/// <summary>
	/// 回転させる仕様にしたいが実装がだるい
	/// もう面倒なのでこのクラスは後回しにしよう. どうせ間に合わんがね...
	/// 判定はvectorでやってるので判定の矩形2つ分を1つのオブジェクトでは表現不可.よってメンバ変数にblockを持たせることに.(=textureとvectorは他のオブジェクトのように合わせる)
	/// 12/29:DamageBlockを使うのはどうか？(交差判定は諦めてテクスチャだけ回転させる)
	/// 1/15:Blockの位置がおかしいと気づく...userがBallのときはcharacterと判定しないようにする？いや待てよ、すり抜けるほうがおかしいか...ダメージ時に仰け反らせればおｋかな
	/// 判定用の矩形は外接でおかしかったら外接と内接の中間くらい。
	/// 
	/// 11/9/19 baseをEnemyに変更する・・・？
	/// </summary>
	public class SnowBall : Terrain
	{
		private float defaultSpeed;
		public DamageBlock block { get; private set; }   // staticTerrainとの当たり判定用
		private int totalHits, delayTime, comboTime, comboCount;

		public SnowBall(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, 0, null, new Vector2())
		{
		}
		public SnowBall(Stage stage, float x, float y, int width, int height, int type, Object user, Vector2 localPosition)
			: base(stage, x, y, width, height, type)
		{
			this.width = 96;
			this.height = 96;
			this.user = user;
			defaultSpeed = -8;
			HP = 2;

			//block = new DamageBlock (stage, x + 8, y + 32, 48, 48);     // 下端は合わせる    // DamageBlock
			block = new DamageBlock(stage, x, y, 96, 96, this, Vector2.Zero);
			stage.dynamicTerrains.Add(block);

			Load();
		}
		protected override void Load()
		{
			base.Load();
			texture = game.Content.Load<Texture2D>("Object\\Terrain\\SnowBall");
		}　

		public override void Update()
		{
			position = block.position;// +new Vector2(-24, -36);      // 判定用矩形の位置のUpdate
			defaultSpeed = -1;
			//if (position.X > stage.player.position.X + 50) 
			block.speed.X = defaultSpeed;
			//block.UpdateNumbers();    // 落ちる
			//block.position += block.speed;
			d++;    // 実際に矩形を回転させないで、画像だけ回転するために使う角度の変数. 2/20：形骸化
			// degreeだと剣との判定にえいきょうするので別の変数
			//block.UpdateNumbers();

			if (HP <= 0 && time > comboTime) { isAlive = false; block.isAlive = false; }//死んだ後も引っかかってた }// HP0でフルボッコが終わったら志望
			if (!isAlive && counter == 0) { isEffected = true; deathEffected = true; counter++; }// 1回だけフラグを立てる
			if (time > comboTime) comboCount = 0;

			if (stage.player.isThrusting && isDamaged) MotionUpdate();
			time++;
			delayTime++;
		}
		public override void MotionUpdate()
		{
			if (isDamaged && isAlive) {
				HP--;
				totalHits += 1;
				time = delayTime = 0;
				isEffected = damageEffected = true;
				if (time < comboTime) {
					comboCount++;
				}
				time++;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//base.Draw(spriteBatch);
			if (IsBeingUsed()) {
				//spriteBatch.Draw(texture2, drawPos, Color.White);

				// これでほぼ合ったはず(96*96 + updateNumbers()しない時)
				//spriteBatch.Draw(textures, drawPos + new Vector2(width / 2, height / 4 - height / 8), null, Color.White, -d, new Vector2(width / 2, height / 2), new Vector2(1, 1), SpriteEffects.None, 0);// 回転中心は矩形中心
				spriteBatch.Draw(texture, block.drawPos /*+ new Vector2(width/2, height/2)*/, null, Color.White, -d, new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
			}
			//spriteBatch.Draw(textures, drawPos + new Vector2(48, 48), null, Color.White, -d, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
			//block.Draw(spriteBatch);
		}


		//x = for(i←x;j←y) {
		// yield(i,j)
		//}
	}
}
