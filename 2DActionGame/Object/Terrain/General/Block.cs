using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class Block : Terrain
    {
		/// <summary>
		/// 天井か地面か。天井用のBLockは反転させて描画する
		/// </summary>
        public bool onCeiling { get; set; }

        /// <summary>
		/// PlayerがBlockのどこに接しているか(デバッグ用)
        /// </summary>
        public bool ontop, onleft, onright, onBottom; 
        private int textureType;

        public Vector2[] sideVector = new Vector2[4];
        public Vector2[] sideVectorStart = new Vector2[4];
        public Vector2[] sideVectorEnd = new Vector2[4];
		
		private Vector2[] locusVectors = new Vector2[10];
		private Vector2 locusVec;
        #region Constructors
        public Block()
        { 
        }
        public Block(Stage stage,  float x, float y, int width, int height)
            : this( stage, x, y, width, height, 0)
        {
        }
        public Block(Stage stage, float x, float y, int width, int height, int type)
            : this(stage, x, y, width, height, type, new Vector2())
        {
        }
		/// <summary>
		/// 静的地形に特化.コンストラクタの連鎖外 使ってない
		/// </summary>
        public Block(Stage stage, float x, float y, int width, int height, int type, bool onCeiling)
            : base(stage, x, y, width, height, type)
        {
            this.onCeiling = onCeiling;
			Load();
        }
		/// <summary>
		/// 静的地形に特化.連鎖外
		/// </summary>
        public Block(Stage stage, float x, float y, int width, int height, int type, int onCeiling)
            : base(stage, x, y, width, height, type)
        {
            if (onCeiling == 0) this.onCeiling = false;
            else this.onCeiling = true;
			Load();
        }
		/// <summary>
		/// 静的地形に特化.連鎖外　使ってない
		/// </summary>
		public Block(Stage stage, float x, float y, int width, int height, int type, int onCeiling, int textureType)
            : base(stage, x, y, width, height, type)
        {
            if (onCeiling == 0) this.onCeiling = false;
            else this.onCeiling = true;
			Load();
        }


        public Block(Stage stage, float x, float y, int width, int height, int type, Vector2 localPosition)
            : this(stage, x, y, width, height, type, null, localPosition)
        {
        }
        public Block(Stage stage, float x, float y, int width, int height, int type, Object user, Vector2 localPosition)
            : this(stage, x, y, width, height, type, user, localPosition, false)
        {
        }
        /// <summary>
		/// 天井用
        /// </summary>
        public Block(Stage stage, float x, float y, int width, int height, int type, Object user, Vector2 localPosition, bool onCeiling)
            : this(stage, x, y, width, height, type, user, localPosition, onCeiling, false, 0)
        {
        }
        /// <summary>
		/// texture別用
        /// </summary>
        public Block(Stage stage, float x, float y, int width, int height, int type, Object user, Vector2 localPosition, bool onCeiling, bool loadManually, int textureType)
            : base(stage, x, y, width, height, user, localPosition)
        {
            this.onCeiling = onCeiling;
            this.loadManually = loadManually;
            this.textureType = textureType;
			switch (type) {
				case 0:
					this.friction = .40f; break;
				case 1:
					this.friction = .18f; break;

				case 2: // hardcording注意
					this.friction = .18f;
					//loadManually = true;
					//Load();
					break;
			}

			if (loadManually) {
				switch (textureType) {
					case 1:
						Load(game.Content, "Object\\Turret&Bullet\\windTurret1");
						animation = new Animation(64, 48);//(width, height);
						break;
					/*case 2:
						Load(game.content, "Stage02\\IcicleD");
						break;*/
				}
			}

			Load();
        }
        #endregion
		protected override void Load()
		{
			base.Load();
			/*if (terrain.movementType == 0)
				if (terrain.isOn && !terrain.isUnder) terrain.Load(content, );
				else terrain.Load(content, );
			else if (terrain.movementType == 1) terrain.Load(content, "Object\\Terrain\\FrozenBlock");
			else if (terrain.movementType == 2) terrain.Load(content, );*/

			/*switch (movementType) {
				case 0:
					if (isOn && !isUnder)
						texture = content.Load<Texture2D>("Object\\Terrain\\Block" + (game.stageNum - 1).ToString() + "1");
					else
						texture = content.Load<Texture2D>("Object\\Terrain\\Block" + (game.stageNum - 1).ToString() + "2");
					break;
				case 1:
					texture = content.Load<Texture2D>("Object\\Terrain\\FrozenBlock");
					break;
				case 2:
					texture = content.Load<Texture2D>("Object\\Terrain\\BlockV");
					break;
			}*/
			switch (type) {
				case 0:
					texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Block01");
					break;
				case 1:
					texture = content.Load<Texture2D>("Object\\Terrain\\FrozenBlock");
					break;
				case 2:
					texture = content.Load<Texture2D>("Object\\Terrain\\BlockV");
					break;
			}
		}

        public override void Update()
        {
            if (loadManually) UpdateAnimation();
            if (user is SnowBall) UpdateNumbers();		
        }
        public override void UpdateAnimation()
        {
            switch(textureType) {
                case 1 :
                    animation.Update(2, 0, 64, 48, 6, 1);
                    break;
            }
        }
		protected override void ChangeHitFlags(Object targetObject)
		{
			base.ChangeHitFlags(targetObject);
			ontop = false; onleft = false; onright = false;// 初期化(デバッグ用)
		}

		//当たり判定と処理
        public override void IsHit(Object targetObject)
        {
			ChangeFlags(targetObject);
            
            if (targetObject.position.X + targetObject.width < position.X) {
            } else if (position.X + width < targetObject.position.X) {
            } else if (targetObject.position.Y + targetObject.height < position.Y) {
            } else if (position.Y + height < targetObject.position.Y) {
            } else
            {
                Vector2 criterionVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);

                // 当たりあり
				ChangeHitFlags(targetObject);

				if (targetObject.speed.Y > 0 && !isUnder) { // targetObjectが下に移動中
                    //if(isLeftSlope && criterionVector.X > position.X &&  criterionVector.X < position.X + width) {
                    //}
                    if ((!isRightSlope && !isLeftSlope && targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)
                        || (!isLeftSlope && isRightSlope && criterionVector.X > position.X && targetObject.position.X < position.X + width)
                        || (!isRightSlope && isLeftSlope && targetObject.position.X + targetObject.width > position.X && criterionVector.X < position.X + width)
                        || (isLeftSlope && isRightSlope && criterionVector.X > position.X && criterionVector.X < position.X + width))
                    {
                        //座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
                        // ブロックの上端
                        if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y - targetObject.height;  // 上に補正

                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
                            targetObject.isJumping = false;　// 着地したらJumpできるように
                            targetObject.position.X += this.speed.X;  // これで慣性を再現できるか！？

                            if (type == 0) targetObject.friction = .40f;
                            else if (type == 1) targetObject.friction = .05f;
                            targetObject.isOnSomething = true;
                            ontop = true;
                            isUnderSomeone = true;
                        }
                    }
				} else if (targetObject.speed.Y < 0 && !isOn) {// 上に移動中
					if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)
						// ブロックの下端
						if (position.Y + height - targetObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y + height;   // 下に補正
						}
				}
                // 右に移動中
                if (targetObject.speed.X - speed.X > 0 && !isRight && !isRightSlope) {
                    if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height)
                        // ブロックの左端
                        if ((targetObject.position.X + targetObject.width) - position.X < maxLength) {
                            targetObject.position.X = position.X - targetObject.width;  // 左に補正
                            onleft = true;
							if (targetObject is Player) (targetObject as Player).isHitLeftSide = true;
                        }
                } else if (targetObject.speed.X - speed.X < 0 && !isLeft && !isLeftSlope) {// 左に移動中
                    if (targetObject.position.Y + targetObject.height> position.Y  && targetObject.position.Y < position.Y + height)
                        // ブロックの右端
                        if ((position.X + width) - targetObject.position.X < maxLength) {
                            targetObject.position.X = position.X + width;   // 右に補正
                            onright = true;
                        }
                }
            }
        }
        public void IsHit(Object targetObject, Vector2 vec)
        {
            if(targetObject.isFirstTimeInAFrame) {
                isHit = false;
                targetObject.isFirstTimeInAFrame = false;
                isHitCB = false;
                targetObject.isOnSomething = false;
            }
            if (vec.X + targetObject.width < position.X) {
			} else if (position.X + width < vec.X) {
			} else if (vec.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < vec.Y) {
			} else {
				Vector2 criterionVector = vec + new Vector2(targetObject.width / 2, targetObject.height);
				isHit = true;
				targetObject.isHit = true;
				isFirstTimeInAFrame = false;
				ontop = false; onleft = false; onright = false;
				if (targetObject.speed.Y > 0 && !isUnder) {
					if ((!isRightSlope && !isLeftSlope && vec.X + targetObject.width > position.X && vec.X < position.X + width)
						|| (!isLeftSlope && isRightSlope && criterionVector.X > position.X && vec.X < position.X + width)
						|| (!isRightSlope && isLeftSlope && vec.X + targetObject.width > position.X && criterionVector.X < position.X + width)
						|| (isLeftSlope && isRightSlope && criterionVector.X > position.X && criterionVector.X < position.X + width))
						if (vec.Y + targetObject.height - position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y - targetObject.height;
							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;
							targetObject.isJumping = false;
							targetObject.position.X += this.speed.X;
							if (type == 0) targetObject.friction = .40f;
							else if (type == 1) targetObject.friction = .05f;
							targetObject.isOnSomething = true;
							ontop = true;
							isUnderSomeone = true;
						}
				} else if (targetObject.speed.Y < 0 && !isOn) {
					if (vec.X + targetObject.width > position.X && vec.X < position.X + width)
						if (position.Y + height - vec.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y + height;
						}
				}
				if (targetObject.speed.X - speed.X > 0 && !isRight && !isRightSlope) {
					if (vec.Y > position.Y - targetObject.height && vec.Y < position.Y + height)
						if ((vec.X + targetObject.width) - position.X < maxLength) {
							targetObject.position.X = position.X - targetObject.width;
							onleft = true;
						}
				} else if (targetObject.speed.X - speed.X < 0 && !isLeft && !isLeftSlope) {
					if (vec.Y + targetObject.height > position.Y && vec.Y < position.Y + height)
						if ((position.X + width) - vec.X < maxLength) {
							targetObject.position.X = position.X + width;
							onright = true;
						}
				}
			}
        }
        public void IsHitTwice(Object targetObject, int division)
        {
            if (targetObject.locus.Count >= 2) {
                locusVec = targetObject.locus[1] - targetObject.locus[0];
                for (int i = 0; i <= division; i++) locusVectors[i] = targetObject.locus[0] +  i * (locusVec / division);
            
                foreach(Vector2 lV in locusVectors) {
                    IsHit(targetObject, lV);
                    if (targetObject.isHit) break;
                }
            }
            else IsHit(targetObject);
        }
        public void IsHitVec(Object targetObject)
        {
        }
		public bool IsInterrupt(Player player)
		{
			return !isRightSlope && !isLeftSlope && isLeft && !isRight
					   && position.X < player.position.X + player.width && player.position.Y + player.height > position.Y && player.position.Y < position.Y + height;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
			if (!loadManually) {
				if (!onCeiling) {
					if (user != null && isBeingUsed) spriteBatch.Draw(texture, drawPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
					else if (user != null) { }// 何も描画しない
					else spriteBatch.Draw(texture, /*drawPos*/position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
				} else {// 天井を考慮(Stage2)して上下反転で描画。
					if (user != null && user.isBeingUsed) spriteBatch.Draw(texture, drawPos, null, Color.White, MathHelper.ToRadians(180), Vector2.Zero, 1, SpriteEffects.None, .5f);
					else spriteBatch.Draw(texture, /*drawPos*/position, null, Color.White, /*MathHelper.ToRadians(180)*/0, Vector2.Zero, 1, SpriteEffects.FlipVertically, .5f);
				}
			} else {// ==動的 //3/19 ここにも上と同様に書いておかないと使ってない時にも描画されてしまう
				//spriteBatch.Draw(textures, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
				if (!onCeiling) {
					if (user != null && isBeingUsed) spriteBatch.Draw(texture, drawPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
					else if (user != null) { }// 何も描画しない
					else spriteBatch.Draw(texture, /*drawPos*/position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
				} else {// 天井を考慮(Stage2)して上下反転で描画。
					if (user != null && user.isBeingUsed) spriteBatch.Draw(texture, drawPos, null, Color.White, MathHelper.ToRadians(180), Vector2.Zero, 1, SpriteEffects.None, .5f);
					else spriteBatch.Draw(texture, /*drawPos*/position, null, Color.White, /*MathHelper.ToRadians(180)*/0, Vector2.Zero, 1, SpriteEffects.FlipVertically, .5f);
				}
			}

			
        }
    }
}
