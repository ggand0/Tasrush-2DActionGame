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
	/// フラグを立てると分裂する武器(マルクのアレとか)
	/// </summary>
    public class DivisionWeapon : Weapon
    {
		private static readonly float perDistance = 300;

		private SoundEffect thunderSound;
		private Vector2 defaultPosition;
        private Weapon derived1, derived2;

		public Rectangle rect;
        public bool isDivided;

        //public bool isBeingUsed;// 使用開始フラグ:手動 Autoは挙動を書いた関数を呼ぶようにする
        //private bool isInAuto;

        public DivisionWeapon(Stage stage, float x, float y, int width, int height, Character user)
            : this(stage, x, y, width, height, user, .5f)
        {
        }
        public DivisionWeapon(Stage stage, float x, float y, int width, int height, Character user, float ratio)
            : base(stage, x, y, width, height,user)
        {
            derived1 = new Weapon(stage, x, y, width * (int)ratio, height * (int)ratio, user);
            derived2 = new Weapon(stage, x, y, width * (int)ratio, height * (int)ratio, user);
            rect = new Rectangle(0, 0, width, height);

            stage.weapons.Add(derived1);
            stage.weapons.Add(derived2);
			derived1.isBeingUsed = derived2.isBeingUsed = true;

			Load();
        }

		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Turret&Bullet\\plasma");
			thunderSound = content.Load<SoundEffect>("Audio\\SE\\thunder_big");
			derived1.Load(content, "Object\\Turret&Bullet\\plasma");
			derived2.Load(content, "Object\\Turret&Bullet\\plasma");
		}

        public override void Update()
        {
            defaultPosition = user.position + new Vector2(user.width / 2, user.height / 2);
            /*if(isBeingUsed) {// 発射時に分裂後のオブジェクトのフラグも立てておく。
                derived1.isBeingUsed = true;
                derived2.isBeingUsed = true;
            }
            else{// それ以外の時
                derived1.isBeingUsed = false;
                derived2.isBeingUsed = false;
            }*/

            if (!isDivided && !isBeingUsed) position = defaultPosition;
            if(!isDivided) {
                derived1.position = position;
                derived2.position = position;
            }
            if(!isBeingUsed) {
                counter = 0;
                derived1.isBeingUsed = false;
                derived2.isBeingUsed = false;
            }

            //base.Update();
            UpdateAnimation();
            UpdateNumbers();
        }
        public override void UpdateAnimation()
        {
            animation.Update(3,0,48,48,6,1);
            derived1.animation.Update(3, 0, 48, 48, 6, 1);
            derived2.animation.Update(3, 0, 48, 48, 6, 1);
        }

        private void Division()
        {
            derived1.speed.X = -5;
            derived2.speed.X = 5;

            derived1.position += derived1.speed;
            derived2.position += derived2.speed;

            if (derived1.position.X < position.X - perDistance) {
                isEnd = true;
                isDivided = false;
            }
            //derived1.position.X += 
            //derived1.Update(); // updateはstageでやっていいか
        }

        public void MovePattern1()
        {
            if(!isDivided && isBeingUsed) {
                speed = new Vector2(0, 5);
                position += speed;
            }
            // 分裂位置がハードコーディングだとstackするので地面の位置を取得させよう
            if (!isDivided && ( position.Y > defaultPosition.Y + Math.Abs(defaultPosition.Y - stage.CheckGroundHeight(defaultPosition.X)) - height // 両端の位置で調べてどちらかがtrueなら。 斜面の先端は例外だが
                || position.Y > defaultPosition.Y + Math.Abs(defaultPosition.Y - stage.CheckGroundHeight(defaultPosition.X + width)) - height )) {//defaultPosition.Y + 200) {

                if (!game.isMuted) thunderSound.Play(SoundControl.volumeAll, 0f, 0f);    
                isDivided = true;
                derived1.isBeingUsed = true;
                derived2.isBeingUsed = true;
                isEnd = false;
            }
            if(isDivided) {
                Division();
            }
            counter++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // userに複数攻撃パターンがある場合が考えられるので、isAttackingだけだとこのオブジェクトを使わない攻撃でも描画されてしまう可能性
            // したがってisBeingUsedで管理する必要あり
            //base.DrawEffect(spriteBatch);
            if(isActive && (user as Character).isAttacking) {// attackingは保険、別にいらない
                if(isBeingUsed) {
                    if (!isDivided) spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
                    else {
                        spriteBatch.Draw(derived1.texture, derived1.drawPos, derived1.animation.rect, Color.White);
                        spriteBatch.Draw(derived2.texture, derived2.drawPos, derived2.animation.rect, Color.White);
                    }
                }
            }
        }
    }
}
