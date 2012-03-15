using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
	/// <summary>
	/// Characterにダメージを与えるクラス。DamageManagerで管理される
	/// </summary>
	public class Damage
	{
		private Stage stage;
		/// <summary>
		/// 何フレーム目でダメージを入れるか(HPを減らすか)
		/// </summary>
		private int damageFrame;
		private int damageCounter;

		/// <summary>
		/// 攻撃をヒットさせたObject
		/// </summary>
		public Object attackedObject { get; set; }
		/// <summary>
		/// 攻撃を受けたObject
		/// </summary>
		public Object damagedObject { get; set; }
		public Object2 adObject { get; set; }
		public bool hasDamaged { get; set; }

		public Damage(Stage stage, Object attackedObject, Object damagedObject)
		{
			this.stage = stage;
			this.attackedObject = attackedObject;
			this.damagedObject = damagedObject;

			damageFrame = 0;
		}
		public Damage(Stage stage, Object2 ADObject)
		{
			this.stage = stage;
			this.adObject = ADObject;

			damageFrame = 0;
		}

		/// <summary>
		/// 改良したver
		/// </summary>
		public void Update()
		{
			if (adObject.object2.damageFromAttacking) {

				if (adObject.object1.isAttacking
					|| (adObject.object1 is Weapon && (adObject.object1 as Weapon).user.isAttacking)
					|| (adObject.object1 is Bullet && (adObject.object1 as Bullet).isShot)) {
					if (damageCounter == damageFrame) {
						adObject.object2.isDamaged = true;
						adObject.object2.MotionUpdate();
						if (!(adObject.object2 is Player)) stage.inComboObjects.Add(adObject.object2);
					}

					adObject.object2.isDamaged = false;                                             // 決まったフレームでのみtrueにする。それ以外ではfalse
					if (adObject.object1 is Bullet) (adObject.object1 as Bullet).isShot = false;
					damageCounter++;
				} else {
					hasDamaged = true;
					adObject.object2.isDamaged = false;

					if (stage.damageControl.damages.Any((x) => !x.Equals(this) && x.adObject.object2.Equals(this.adObject.object2))) {// !=, ==
					} else {
						adObject.object2.damageFromAttacking = false;
						adObject.object2.damageFromTouching = false;
						adObject.object2.damageFromThrusting = false;
					}
					damageCounter = 0;

					stage.inComboObjects.Remove(adObject.object2);
				}
			} else if (adObject.object2.damageFromThrusting) {
				if (adObject.object1.isAttacking
					|| (adObject.object1 is Weapon && (adObject.object1 as Weapon).user.isAttacking)
					|| (adObject.object1 is Bullet && (adObject.object1 as Bullet).isShot)) {

					if (damageCounter == damageFrame || damageCounter <= Player.thrustingTime && damageCounter % 5 == 0) {
						adObject.object2.isDamaged = true;
						adObject.object2.MotionUpdate();
						if (!(adObject.object2 is Player)) stage.inComboObjects.Add(adObject.object2);
					}
					adObject.object2.isDamaged = false;
					if (adObject.object1 is Bullet) (adObject.object1 as Bullet).isShot = false;
					damageCounter++;
				} else if (damageCounter > Player.thrustingTime) {
					hasDamaged = true;
					adObject.object2.isDamaged = false;
					if (stage.damageControl.damages.Any((x) => !x.Equals(this) && x.adObject.object2.Equals(this.adObject.object2))) {
					} else {
						adObject.object2.damageFromAttacking = false;
						adObject.object2.damageFromTouching = false;
						adObject.object2.damageFromThrusting = false;
					}
					damageCounter = 0;

					stage.inComboObjects.Remove(adObject.object2);
				}
			} else if (adObject.object2.damageFromTouching) {
				if (damageCounter <= 60) {
					if (damageCounter == damageFrame) {
						adObject.object2.isDamaged = true;
						adObject.object2.MotionUpdate();
					}

					adObject.object2.isDamaged = false;
					damageCounter++;
				} else {
					hasDamaged = true;
					if (stage.damageControl.damages.Any((x) => x != this && x.adObject.object2 == this.adObject.object2)) {
					} else {
						adObject.object2.damageFromAttacking = false;
						adObject.object2.damageFromTouching = false;
						adObject.object2.damageFromThrusting = false;
					}
					damageCounter = 0;

					stage.inComboObjects.Remove(adObject.object2);
				}
			}
		}
	}
}
