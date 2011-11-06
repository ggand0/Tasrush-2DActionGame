using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	/// <summary>
	/// 当初Trapとして、障害物全般の基本クラスを予定していたが、変更しPlayer(Character)にDamageを与えるオブジェクトの基本クラスに
	/// することにした.(それでもいまだ必要かどうかは疑問である)DamageBlockを無視して新たに...?とりあえず"触れたらダメージ"にしてtypeで分け、BUlletのようにtextureも指定できるようにすれば
	/// 汎用性は増しそうである。
	/// </summary>
	public class DamageObject : Terrain// 旧class Trap
	{
		public int textureType { get; private set; }

		public DamageObject(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, 0)
		{
		}
		public DamageObject(Stage stage, float x, float y, int width, int height, int type)
			: this(stage, x, y, width, height, 0, 0)
		{
		}
		public DamageObject(Stage stage, float x, float y, int width, int height, int type, int textureType)
			: this(stage, x, y, width, height, 0, 0, null, Vector2.Zero)
		{
		}
		public DamageObject(Stage stage, float x, float y, int width, int height, int type, int textureType, Object user, Vector2 localPosition)// ,mapObjects
			: base(stage, x, y, width, height, type)
		{
			this.user = user;
			this.localPosition = localPosition;
			this.textureType = textureType;

			Load();
		}
		protected override void Load()
		{
			base.Load();
			// Bulletに関してはStageでtexture名を指定してLoadではなく、textureTypeで最初から決めておくのがいいだろう
			//texture = content.Load<Texture2D>("Object\\Terrain\\damageBlock");

			switch (textureType) {
				case 0:// wind S
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\tornade");// Load()呼ばれてない
					break;
				case 1:// wind L
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\tornadeL");
					break;
			}
		}

		public override void Update()
		{
			UpdateAnimation();
		}
		public override void UpdateAnimation()
		{
			switch (textureType) {
				case 0: animation.Update(3, 0, 48, 48, 6, 1);
					break;
				case 1: animation.Update(3, 0, 100, 350, 6, 1);
					break;
			}
		}

		/// <summary>
		/// どこでも触れたらDamageを与えるようにoverride。Weaponとして使われているときは呼ばれない。
		/// </summary>
		/// <param name="targetObject"></param>
		public override void IsHit(Object targetObject)
		{
			/*if (firstTimeInAFrame)// 1人or1匹でも触れていたらisHit=trueのままでいい
				isHit = false;
			targetObject.isHit = false;*/
			ChangeFlags(targetObject);

			if (targetObject.position.X + targetObject.width < position.X) {
			} else if (position.X + width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {// 当たりあり
				ChangeHitFlags(targetObject);
			}
		}
		protected override void ChangeHitFlags(Object targetObject)
		{
			base.ChangeHitFlags(targetObject);
			targetObject.isDamaged = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (isActive && isAlive) {// FuujinのmOのdOが3つともtex null
				if (user != null && isBeingUsed) {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
				} else if (user != null && !isBeingUsed && fadeOut) {
					// アルファ値を計算
					if (counter == 0) dColor = 1f;
					if (counter % 2 == 0) dColor += -.05f;

					spriteBatch.Draw(texture, drawPos, animation.rect, new Color(255, 255, 255, dColor));
					counter++;

					// 終了処理
					if (dColor <= 0 || counter > 120) {
						fadeOut = false;
						counter = 0;
					}
				}
			}
		}
	}
}
