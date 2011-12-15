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
	public struct MoveInfo
	{
	}

	/// <summary>
	/// 隕石とかBossが使うTrapとかの障害物：Terrainにするか迷うが多分Weaponのほうが使いやすいだろう
	/// </summary>
	public class Obstacle : Weapon
	{
		private SoundEffect blastSound;
		/// <summary>
		/// trapSetのプリセットのindex
		/// </summary>
		private int type;
		/// <summary>
		/// MapObjects生成時の詳細タイプ
		/// </summary>
		private int typeDetailed;
		private int movePattern;
		/// <summary>
		/// 地形の外に出たか：Effectに使う？
		/// </summary>
		private bool hasBeenOut;
		/// <summary>
		/// まとめて扱いたい地形を追加するList
		/// </summary>
		public List<Terrain> trapSet { get; private set; }
		public int attackCounter { get; internal set; }
		//public bool isEnd { get; set; }                     // （移動処理などが）終了したか. 大抵userが何かの判断に使う

		public Obstacle(Stage stage, Object user, float x, float y, int width, int height, int type)
			: this(stage, user, x, y, width, height, type, 0)
		{
		}
		public Obstacle(Stage stage, Object user, float x, float y, int width, int height, int type, int typeDetailed)
			: this(stage, user, x, y, width, height, type, typeDetailed, 0)
		{
		}
		public Obstacle(Stage stage, Object user, float x, float y, int width, int height, int type, int typeDetailed, int movePettern)
			: base(stage, x, y, width, height, user)
		{
			this.type = type;
			this.typeDetailed = typeDetailed;
			trapSet = new List<Terrain>();

			switch (type) {
				case 0:
					trapSet.Add(new Block(stage, x, y, 32, 32, 0, this, Vector2.Zero));
					trapSet.Add(new Block(stage, x, y, 32, 32, 0, this, new Vector2(0, 32)));
					trapSet[0].isOn = true;
					trapSet[1].isUnder = true;
					foreach (Terrain terrain in trapSet) stage.dynamicTerrains.Add(terrain);
					this.width = 32;
					this.height = 64;
					break;
				case 1:
					trapSet.Add(new DamageBlock(stage, x, y, 32, 32, this, Vector2.Zero));
					trapSet.Add(new DamageBlock(stage, x, y, 32, 32, this, new Vector2(0, 32)));
					trapSet.Add(new Block(stage, x, y, 32, 32, 0, this, new Vector2(32, 0)));
					trapSet.Add(new Block(stage, x, y, 32, 32, 0, this, new Vector2(32, 32)));
					trapSet[0].isOn = true;
					trapSet[1].isOn = true;
					trapSet[2].isUnder = true;
					trapSet[3].isUnder = true;
					foreach (Terrain terrain in trapSet) stage.dynamicTerrains.Add(terrain);
					this.width = 64;
					this.height = 64;
					break;
				case 2:
					switch (typeDetailed) {
						case 0:
							trapSet.Add(new MapObjects(stage, x, y, 32, 32, 0, 3, this));	// Meteor
							break;
						case 1:
							trapSet.Add(new MapObjects(stage, x, y, 32, 32, 1, 3, this));	// Icicle
							break;
						case 2:
							trapSet.Add(new MapObjects(stage, x, y, 32, 32, 2, 3, this));	// Wind
							break;
						case 3:
							trapSet.Add(new MapObjects(stage, x, y, 32, 32, 3, 3, this));	// TornadeS
							break;
						case 4:
							trapSet.Add(new MapObjects(stage, x, y, 100, 350, 4, 1, this));	// TornadeL
							break;
						case 5:
							trapSet.Add(new MapObjects(stage, x, y, 64, 64, 5, 1, this));	// Yukidama
							break;
						case 6:
							break;
					}
					
					foreach (Terrain terrain in trapSet) stage.dynamicTerrains.Add(terrain);
					break;
			}
		}

		protected override void Load()
		{
			base.Load();
			blastSound = content.Load<SoundEffect>("Audio\\SE\\blast_small");
		}

		public override void Update()
		{
			// obstacleのvectorに地形群を追従させる
			if (isBeingUsed) {
				foreach (Terrain terrain in trapSet) {
					terrain.position = position + terrain.localPosition;
					terrain.isBeingUsed = true;				// Icicle以外
				}
				UpdateNumbers();
				foreach (Terrain terrain in trapSet) terrain.speed = this.speed;
			} else {
				foreach (Terrain terrain in trapSet) terrain.isBeingUsed = false;
			}
		}

		/// <summary>
		/// 使ってない
		/// </summary>
		public void MovePattern0(List<Terrain> trapSet, Vector2 startPosition, Vector2 endPosition)
		{
			if (isBeingUsed) {
				if (attackCounter == 0) {
					position = new Vector2(user.position.X, stage.surfaceHeightAtBoss);
					startPosition = position;
					endPosition = new Vector2(startPosition.X - 400, startPosition.Y);
				}
				if (position.X > endPosition.X) {
					speed.X += -2.5f;
					position += speed;
				} else if (position.X < endPosition.X)
					isEnd = true;
			}
			attackCounter++;
		}

		/// <summary>
		/// SPからspeed0で飛び出した後speed1でEPに移動させるパターン.Stageの地形の高さに沿って移動。
		/// </summary>
		public void MovePattern1(List<Terrain> trapSet, Vector2 startPosition, Vector2 wayPoint, Vector2 EndPosition, Vector2 speed0, Vector2 speed1)
		{
			if (isBeingUsed) {
				if (attackCounter == 0) {
					speed = speed0;//Vector2.Zero;
					position = startPosition;
					hasBeenOut = false;
				}

				/*if (!hasBeenOut && position.Y > wayPoint.Y - height) speed = speed0;//追加：-height
				if (!hasBeenOut && position.Y <= wayPoint.Y - height) hasBeenOut = true;
				if (hasBeenOut && position.X > EndPosition.X) speed = speed1;
				if (Vector2.Distance(position, EndPosition) < speed1.Length()) isEnd = true;*/
				if (!hasBeenOut) speed = speed0;
				if (!hasBeenOut && Vector2.Distance(position, wayPoint) < speed0.Length()) {
					hasBeenOut = true;
					blastSound.Play(SoundControl.volumeAll, 0f, 0f);
				}
				if (hasBeenOut) speed.X = speed1.X;
				if (hasBeenOut && isOnSomething) position.Y = stage.CheckGroundHeight(position.X) - height;
				if (hasBeenOut && Vector2.Distance(position, EndPosition) < speed1.Length()) isEnd = true;
			}
			attackCounter++;
			float test = speed1.Length();
		}
		/// <summary>
		/// MapObjects用の移動パターン：wayPointsとspeedの情報だけあればおｋ：コンストラクタだけで設定することが可能
		/// </summary>
		public void MovePattern2(List<Terrain> trapSet, Vector2 startPosition, Vector2 wayPoint, Vector2 endPosition, int type, float speed)// typeは パ ラ メ ー タだからおｋ
		{
			// MapObjectsに任せる
			if (isBeingUsed) {
				switch (this.type) {
					case 2:
						switch (this.typeDetailed) {
							case 1:// Icicle
								(trapSet[0] as MapObjects).MovePattern0(startPosition, 600, type, .20);
								break;
							default:
								// 可能ならばMove(float moveSpeed, int type, paramsVector2[] wayPoints)に統一した方がいいかも。
								(trapSet[0] as MapObjects).MovePattern1(startPosition, endPosition, speed, type);//0
								break;
						}
						break;
				}
				/*for (int i = 0; i < trapSet.Count; i++) {
					if (movementType == i && trapSet[i] is MapObjects && (trapSet[i] as MapObjects).isEnd)
						isEnd = true;// mapObjectsのisEndを受けてobstacleもisEndを立てる
				}*/
				if ((trapSet[0] as MapObjects).isEnd) isEnd = true;
			}
		}

		/// <summary>
		/// trapSetを全て同様に移動させる。引数はbaseと同様。
		/// </summary>
		public void MovePattern2(float moveSpeed, int type, params Vector2[] wayPoints)
		{
			if (isBeingUsed) {
				foreach (Terrain terrain in trapSet) {
					base.Move(moveSpeed, wayPoints);

					if ((terrain as MapObjects).isEnd) isEnd = true;
				}
			}
		}

		/// <summary>
		/// 管理クラスなので何も描画しない。
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch)
		{
		}
	}
}
