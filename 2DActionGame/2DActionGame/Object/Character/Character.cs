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
    public class Character : Object
    {
        #region Member variable
		public int deathComboTime = 30;

		/// <summary>
		/// このオブジェクトを使用しているオブジェクト
		/// </summary>
		//public Object user { get; internal set; }                  // BossなどがEnemtを使ったりとかする際に。nullの場合は通常の処理
		/// <summary>
		/// 現在乗っている地形のリスト
		/// </summary>
        public List<Terrain> ridingTerrains = new List<Terrain>();
		/// <summary>
		/// 所有武器のリスト
		/// </summary>
        public List<Weapon> weapons = new List<Weapon>();           // 当たり判定の計算用に使うので、派生クラスで別の変数を使ってUpdateの最後に代入という形でもいいかも

		public bool turnsLeft { get; protected set; }
        public bool isDashing { get; set; }
		public bool isBlownAway { get; protected set; }
        public bool isInDamageMotion { get; set; }
        public bool onConveyor { get; set; }

        public int totalHits { get; set; }                          // デバッグ用
        public int comboCount { get; protected set; }
        #endregion

        public Character()
            : this(null, 0, 0, 32, 32)
        {
        }
        public Character(Stage stage,  float x, float y, int width, int height)
            : this(stage, x, y, width, height, null)
        {
        }
        public Character(Stage stage, float x, float y, int width, int height, Character user)
            : base(stage, x, y, width, height)
        {
            this.user = user;
        }

        public override void Update()
        {
            if (isActive) {
                UpdateNumbers();
                UpdateAnimation();
            }
			if (user != null && !isAlive) {
				isBeingUsed = false;
			}
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive) {
                if (user != null && isBeingUsed)// userが指定されているときはuserのフラグで管理させる
                    spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
                else
                    spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
            }
        }

    }
}
