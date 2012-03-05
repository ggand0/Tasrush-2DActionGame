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
	/// 1Bossの技などに使用。直線的な稲妻(矩形で表現)で、矩形自体を伸ばしていく方針。
	/// </summary>
	public class Thunder : Bullet
	{
		private Animation[] animations;
		private SoundEffect thunderSoundSmall, thunderSoundBig;
		private Rectangle rect;
		private int maxWidth, defaultWidth;

		public Thunder(Stage stage, Turret turret, int width, int height)
			: this(stage, turret, width, height, 0)
		{
		}
		public Thunder(Stage stage, Turret turret, int width, int height, int degree)
			: base(stage, turret, width, height)
		{
			this.degree = degree;
			maxWidth = width + 300;
			defaultWidth = width;
			rect = new Rectangle(0, 0, width, height);

			Load();
		}
		public Thunder(Stage stage, Turret turret, int width, int height, int degree, int dissapearType)
			: base(stage, turret, width, height)
		{
			this.degree = degree;
			this.disappearPattern = dissapearType;
			maxWidth = width + 300;
			defaultWidth = width;
			rect = new Rectangle(0, 0, width, height);

			Load();
		}

		protected override void Load()
		{
			base.Load();

			thunderSoundBig = content.Load<SoundEffect>("Audio\\SE\\thunder_big");
			thunderSoundSmall = content.Load<SoundEffect>("Audio\\SE\\thunder_small");
			texture = content.Load<Texture2D>("Object\\Turret&Bullet\\discharge");
			texture2 = content.Load<Texture2D>("Object\\Weapon\\Beam");

			// animation1は矩形、2はエフェクト
			animation = new Animation(width, height);
			animations = new Animation[10];
			for (int i = 0; i < animations.Length; i++) {
				animations[i] = new Animation(texture.Width / 3, texture.Height);//32, 64
			}
		}

		/*public override void Load(ContentManager content, string texture_name, string texture_name2)
		{
			base.Load(content, texture_name, texture_name2);

			animation = new Animation(width, height);
			animations = new Animation[10];
			for (int i = 0; i < animations.Length; i++)
				animations[i] = new Animation(texture.Width / 3, texture.Height);//32, 64
		}*/

		private void PlaySound()
		{
			if (isActive && isShot && (turret == null || (turret != null && turret.isBeingUsed)) && !game.isMuted) {
				if (counter == 5 || counter == 35) thunderSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
				if (counter == 55) thunderSoundBig.Play(SoundControl.volumeAll, 0f, 0f);
			}
		}
		public override void Update()
		{
			base.Update();

			position = turret.position + new Vector2(turret.width / 2, turret.height);
			PlaySound();
		}
		/// <summary>
		/// turretとかが呼ぶ
		/// </summary>
		public void Inicialize()
		{
			isEnd = false;
			width = defaultWidth;

			// 角度 射撃前に１回だけ呼ばせる
			// Random rnd = new Random();
			int randomNum = game.random.Next(3);
		}

		public override void UpdateAnimation()
		{
			animation.Update(3, 0, 32, 64, 6, 1);

			foreach (Animation animation1 in animations) {
				if (animation1.hasStarted)
					animation1.Update(3, 0, texture.Width / 3, texture.Height, 6, 1);
			}
		}
		public override void UpdateFlying(int flyingTime)
		{
			if (isShot) {
				position = turret.position;

				// 幅自体を増やせば描画時に伸びて見えるはず
				if (width < maxWidth) {
					width += 15;
					rect.Width = width;
					for (int i = 0; i < animations.Length; i++)
						if (width > animations[i].rect.Width * i)
							animations[i].hasStarted = true;
				}

				// 終了処理
				if (/*movedDistance >= 600 ||*/ counter > 120 || width >= maxWidth) {
					isShot = false;
					counter = 0;
					movedDistance = 0;
					for (int i = 0; i < animations.Length; i++) animations[i].hasStarted = false;
					isEnd = true;// フラグを立てる
				}

				counter++;
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsBeingUsed()) {
				if (game.visibleSword) spriteBatch.Draw(texture2, drawPos, /*animation.*/rect, Color.White, MathHelper.ToRadians(-degree), Vector2.Zero, 1, SpriteEffects.None, .0f);
				DrawThunders(spriteBatch);
			}
		}
		private void DrawThunders(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < animations.Length; i++) {
				animations[i].vector = drawPos/*+new Vector2(-32,-32)*/ + new Vector2((float)Math.Cos(MathHelper.ToRadians(degree)) * i * 64, -(float)Math.Sin(MathHelper.ToRadians(degree)) * i * 64);// 64*64でいいだと...!? 32*64じゃないのか...!
				animations[i].vector -= new Vector2(16, 16);//(turret.width, turret.height);
			}
			for (int i = 0; i < animations.Length; i++)
				if (animations[i].hasStarted)
					spriteBatch.Draw(texture, animations[i].vector, animations[i].rect, Color.White, MathHelper.ToRadians(-degree + 270), Vector2.Zero, 1, SpriteEffects.None, .0f);
		}

	}
}
