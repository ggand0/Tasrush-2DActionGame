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
	/// Stage1の飛ぶ敵
	/// </summary>
	public class FlyingEnemy : Enemy
	{
		private SoundEffect flappingSound;
		private SoundEffectInstance flappingSoundInstance;

		private float jumpNum;
		/// <summary>
		/// 動きのタイプ（弾を出すか、など）
		/// </summary>
		private int motionType;
		protected Turret turret;
		protected Vector2 shootPosition;

		public FlyingEnemy()
		{
		}
		public FlyingEnemy(Stage stage, float x, float y, int width, int height, int HP)
			: this(stage, x, y, width, height, HP, 1)
		{
		}
		public FlyingEnemy(Stage stage, float x, float y, int width, int height, int HP, int motionType)
			: base(stage, x, y, width, height, HP)
		{
			this.motionType = motionType;

			// 動き回る仕様
			isMovingAround = true;
			delayTime = motionDelayTime + 1;

			// 弾を射撃するか否か
			if (motionType == 1) {
				shootPosition = new Vector2(5, 5);
				turret = new Turret(stage, this, shootPosition, 32, 32, 0, 1, 1, false, true, false, 3, 0, 1, 6);
				stage.weapons.Add(turret);
			}
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\FlyingEnemy1");
			flappingSound = content.Load<SoundEffect>("Audio\\SE\\bird");
			flappingSoundInstance = flappingSound.CreateInstance();
		}

		public override void Update()
		{
			float distance;

			if (IsActive()) {
				if (isMovingAround) MovementUpdate();
				if (isWinced) turret.isBeingUsed = false;
				else turret.isBeingUsed = true;

				distance = Vector2.Distance(stage.player.position, position);
				if (distance < 100) turret.isBeingUsed = false;
				distance = stage.player.position.X - position.X;
				if (distance > 200) turret.isBeingUsed = false;
				if (distance > 0) turnsRight = true;
				else turnsRight = false;
			} else {
				turret.isBeingUsed = false;
			}

			base.Update();
		}
		public override void UpdateAnimation()
		{
			animation.Update(3, 0, 64, 48, 6, 1);
		}

		public virtual void MovementUpdate()
		{
			if (!isOnSomething && jumpNum >= 15) {
				speed.Y = -4.5f;
				jumpNum = 1;
			}
			if (position.Y > 300) speed.Y = -9;

			RoundTripMotion(defPos, moveDistance, 2);

			if (flappingSoundInstance.State == SoundState.Stopped) {
				if (!game.isMuted) { flappingSoundInstance.Volume = SoundControl.volumeAll; flappingSoundInstance.Play(); }
			}

			jumpNum += timeCoef;
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsActive()) {
				if (!turnsRight) {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
				} else {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
				}
				DrawComboCount(spriteBatch);
			}
		}
	}
}
