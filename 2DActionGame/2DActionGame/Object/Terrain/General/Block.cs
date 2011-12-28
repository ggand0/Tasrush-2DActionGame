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
        /// デバッグ用
        /// </summary>
        private bool isMarked;
		/// <summary>
		/// 辺の数
		/// </summary>
		public const byte sideNum = 4;
		/// <summary>
		/// 天井か地面か。天井用のBLockは反転させて描画する
		/// </summary>
        public bool onCeiling { get; private set; }
		/// <summary>
		/// Slopeの下か（草ブロックにするかどうかに使う）
		/// </summary>
		public bool isUnderSlope { get; set; }
        /// <summary>
		/// PlayerがBlockのどこに接しているか(デバッグ用)
        /// </summary>
        public bool ontop, onleft, onright, onBottom; 
        private int textureType;

        public Vector2[] sideVector = new Vector2[sideNum];
		public Vector2[] sideVectorStart = new Vector2[sideNum];
		public Vector2[] sideVectorEnd = new Vector2[sideNum];
		
		//private Vector2[] locusVectors = new Vector2[10];
		private List<Vector2> locusVectors = new List<Vector2>();
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
					this.friction = defFriction; break;
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

                            //if (targetObject is Rival) { }
                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
                            targetObject.isJumping = false;　// 着地したらJumpできるように
                            targetObject.position.X += this.speed.X;  // これで慣性を再現できるか！？

                            if (type == 0) targetObject.friction = defFriction;
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
        /*public void IsHit(Object targetObject, Vector2 vec)
        {
			if (targetObject.firstTimeInAFrame) {
				isHit = false;
				targetObject.firstTimeInAFrame = false;
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
				firstTimeInAFrame = false;
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
							if (type == 0) targetObject.friction = defFriction;
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
        }*/
		public void IsHit(Object targetObject, Vector2 targetPos)
		{
			//if (targetObject is FlyingOutEnemy3 && (targetObject as FlyingOutEnemy3).hasFlownOut) { }
			ChangeFlags(targetObject);

			if (targetPos.X + targetObject.width < position.X) {
			} else if (position.X + width < targetPos.X) {
			} else if (targetPos.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetPos.Y) {
			} else {
				Vector2 criterionVector = targetPos + new Vector2(targetObject.width / 2, targetObject.height);// 底辺の中点
                if (targetObject is Player) { }
				// 当たりあり
				ChangeHitFlags(targetObject);
                if (targetObject is Player && (targetObject as Player).inDmgMotion) { }

				if (targetObject.speed.Y > 0 && !isUnder) { // targetObjectが下に移動中
					if ((!isRightSlope && !isLeftSlope && targetPos.X + targetObject.width > position.X && targetPos.X < position.X + width)
						|| (!isLeftSlope && isRightSlope && criterionVector.X > position.X && targetPos.X < position.X + width)
						|| (!isRightSlope && isLeftSlope && targetPos.X + targetObject.width > position.X && criterionVector.X < position.X + width)
						|| (isLeftSlope && isRightSlope && criterionVector.X > position.X && criterionVector.X < position.X + width)) {
						//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
							if (targetPos.Y + targetObject.height - position.Y < maxLength) {//targetObject.position.Y + targetObject.height - position.Y < maxLength
							if (targetObject is Player) { }
							if (targetObject is Player && (targetObject as Player).inDmgMotion) { }

							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;
							targetObject.isJumping = false;　// 着地したらJumpできるように
							targetObject.position.X += this.speed.X;
							if (type == 0) targetObject.friction = defFriction;
							else if (type == 1) targetObject.friction = .05f;

							ontop = true;
							isUnderSomeone = true;
						}
					}
				} else if (targetObject.speed.Y < 0 && !isOn) {// 上に移動中
					if (targetPos.X + targetObject.width > position.X && targetPos.X < position.X + width)//targetObject.position.X + targetObject.width > position.X && targetPos.X < position.X + width
						// ブロックの下端
						if (position.Y + height - targetPos.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y + height;   // 下に補正
							targetObject.isTouchSomeCeiling = true;
							//targetObject.jumpCount = 0;
							//(targetObject as Player).isJumping = false;
						}
				}
				// 右に移動中
				if (targetObject.speed.X - speed.X > 0 && !isRight && !isRightSlope) {
					// (重要) 既に上で補正されている場合はtargetPosじゃないよねっていう...!!
					//targetObject.position.Y > position.Y - targetObject.height && targetPos.Y < position.Y + height
					//targetPos.Y > position.Y - targetObject.height && targetPos.Y < position.Y + height
					if ((!targetObject.isOnSomething && targetPos.Y > position.Y - targetObject.height || targetObject.isOnSomething && targetObject.position.Y > position.Y - targetObject.height)
						&& targetObject.position.Y < position.Y + height/*(!targetObject.isTouchSomeCeiling && targetPos.Y < position.Y + height || targetObject.isTouchSomeCeiling && targetObject.position.Y < position.Y + height)*/)//targetPos.Y < position.Y + height
						// ブロックの左端
						if ((targetPos.X + targetObject.width) - position.X < maxLength) {
							targetObject.position.X = position.X - targetObject.width;  // 左に補正
							onleft = true;
							if (targetObject is Player) (targetObject as Player).isHitLeftSide = true;
						}
				} else if (targetObject.speed.X - speed.X < 0 && !isLeft && !isLeftSlope) {// 左に移動中
					//targetObject.position.Y + targetObject.height > position.Y && targetPos.Y < position.Y + height
					//targetPos.Y + targetObject.height > position.Y && targetPos.Y < position.Y + height
					if ((!targetObject.isOnSomething && targetPos.Y > position.Y - targetObject.height || targetObject.isOnSomething && targetObject.position.Y > position.Y - targetObject.height)
						&& targetObject.position.Y < position.Y + height/*(!targetObject.isTouchSomeCeiling && targetPos.Y < position.Y + height || targetObject.isTouchSomeCeiling && targetObject.position.Y < position.Y + height)*/) {
						// ブロックの右端
						if (targetObject.isTouchSomeCeiling) { }
						if ((position.X + width) - targetPos.X < maxLength) {
							targetObject.position.X = position.X + width;   // 右に補正
							onright = true;
						}
					}
				}
			}
		}
        public void IsHitDetailed(Object targetObject, int division)
        {
			if (targetObject.locus.Count >= 2) {
				if (targetObject is Player && targetObject.speed.Y > 0) { }
				locusVec = targetObject.locus[1] - targetObject.locus[0];
                //for (int i = 0; i < locusVectors.Length; i++) locusVectors[i] = new Vector2();
				locusVectors.Clear();
				for (int i = 0; i <= division; i++) locusVectors.Add(targetObject.locus[0] + i * (locusVec / division));
                if (targetObject is Player && (targetObject as Player).isInDamageMotion && Math.Abs(locusVectors[0].Y - locusVectors[1].Y) > 10) { }
                if (targetObject is Player && (targetObject as Player).inDmgMotion) { }

				foreach (Vector2 lv in locusVectors) {
					IsHit(targetObject, lv);// わかった。当たってるかどうかだけ軌跡を使って、当たってたら”現在の情報を使って”補正すればよい
					if (targetObject.isHit || targetObject.isOnSomething) {
						if (targetObject is Player && (targetObject as Player).inDmgMotion) { }
						//break;
					}
					//if ()
				}
			} else {
				IsHit(targetObject);
			}
        }
		public bool IsInterrupt(Player player)
		{
			return !isRightSlope && !isLeftSlope && isLeft && !isRight
					   && position.X < player.position.X + player.width && player.position.Y + player.height > position.Y && player.position.Y < position.Y + height;
		}
    }
}
