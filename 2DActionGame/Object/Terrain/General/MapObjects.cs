using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// 隕石とかの管理.Fuujinの落下物とかの管理.1つ1つを直接管理するよりはよい。「ObstacleっぽいがWeaponじゃないだろう」ってことでTerrainに。
    /// 引数によって生成する障害物を決定し、複数の移動管理関数で動かす.Weaponとして使いたい場合はこのオブジェクトをObstacleのtrapSetに1つ追加すればよい
	/// 
	/// 4/13 ------------つまり複数の地形の集合を扱うクラス------------
    /// </summary>
    public class MapObjects : Terrain
    {
		// userプロパティで自分のuserを返すようにしたい。（単に複数の地形の集合だから）
		private SoundEffect tornadeSound;
        private List<Terrain> mapObjects = new List<Terrain>();
		private List<int> spawnIndex = new List<int>();
        private Random rnd, rnd2;
        private int t;
		/// <summary>
		/// MovePattern()で使用
		/// </summary>
		private int spawnNum;

		/// <summary>
		/// isAliveに統一できるか？
		/// </summary>
		public bool isEnd { get; private set; }
		/// <summary>
		/// mapObjects[index]をそのまま返すインデクサ。
		/// </summary>
		public Terrain this[int index]
		{
			set { this.mapObjects[index] = value; }
			get { return this.mapObjects[index]; }
		}

        public MapObjects(Stage stage, float x, float y, int width, int height, int type)
            : this(stage, x, y, width, height, type, 0, null)
        {
        }
        public MapObjects(Stage stage, float x, float y, int width, int height, int type, int terrainNum, Object user)
            : base(stage, x, y, width, height, type)
        {
            // meteorのuserをnullにすると当たり判定されてしまうので注意. このオブジェクト自身も
			this.user = user;
            hasPlayedSoundEffect = true;
            rnd = new Random();
            rnd2 = new Random();
			switch (type) {
				case 0:
					mapObjects.Add(new Meteor(stage, x, y, width, height, this, Vector2.Zero));
					mapObjects.Add(new Meteor(stage, x, y, width, height, this, new Vector2(-100, -100)));
					mapObjects.Add(new Meteor(stage, x, y, width, height, this, new Vector2(100, 100)));
					break;
				case 1:
					// もっと間隔を広くする予定
					AddIcicles(stage, x, y, width, height);
					break;
				case 2:
					mapObjects.Add(new Block(stage, x, y, width, height, 0, this, Vector2.Zero, false, true, 1));
					mapObjects.Add(new Block(stage, x, y, width, height, 0, this, new Vector2(-50, 100), false, true, 1));
					mapObjects.Add(new Block(stage, x, y, width, height, 0, this, new Vector2(-150, -150), false, true, 1));
					break;
				case 3:
					mapObjects.Add(new DamageObject(stage, x, y, width, height, 0, 0, this, Vector2.Zero));
					mapObjects.Add(new DamageObject(stage, x, y, width, height, 0, 0, this, new Vector2(-50, 100)));
					mapObjects.Add(new DamageObject(stage, x, y, width, height, 0, 0, this, new Vector2(-150, -150)));
					tornadeSound = content.Load<SoundEffect>("Audio\\SE\\tornado");
					break;
				case 4:
					mapObjects.Add(new DamageObject(stage, x, y, width, height, 0, 1, this, Vector2.Zero));						// tornadeL
					tornadeSound = content.Load<SoundEffect>("Audio\\SE\\tornado");
					break;
				case 5:
					//mapObjects.Add(new SnowBall(stage, x, y, width, height, 0, this, Vector2.Zero));
					break;
			}

			
            //foreach (Terrain terrain in mapObjects) stage.dynamicTerrains.Add(terrain);
			//isActive = true;// 本当はCameraで何とかするべき。つまりこのオブジェクトの座標をmapObjectsに合わせて動かすべき
        }
		/// <summary>
		/// デバッグ時に調整しやすいように分化
		/// </summary>
		private void AddIcicles(Stage stage, float x, float y, int width, int height)
		{
			/*mapObjects.Add(new Icicle(stage, x, y, width, height, this, Vector2.Zero));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(64, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-96, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-200, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-150, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-524, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-460, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-340, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-100, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(80, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-400, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-250, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(80, 0)));*/
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, Vector2.Zero));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-64, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-128, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-192, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-256, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-320, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-384, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-448, 0)));
			mapObjects.Add(new Icicle(stage, x, y, width, height, this, new Vector2(-512, 0)));
		}

        public override void Update()
        {
			if (IsActive()) {
				foreach (Terrain terrain in mapObjects) {
					if (terrain is Icicle) { }

					terrain.Update();
					/*if (this.isActive) {// ここの気持ち悪さを考えると、SCorollUpdateでisAcitveを何とかした方がいいような
						terrain.isActive = true;
						terrain.isBeingUsed = true;
					}*/

					/*if (this.isBeingUsed && type != 1) terrain.isBeingUsed = true;// HC
					else if (!this.isBeingUsed) {
						if (terrain is Icicle) { }
						terrain.isBeingUsed = false;
					}*/
				}
				this.position = mapObjects[0].position;

				base.Update();

				if (!hasPlayedSoundEffect && (type == 3 || type == 4)) {
					if (!game.isMuted) tornadeSound.Play(SoundControl.volumeAll, 0f, 0f);	// 3ボスでなぜかnullになる
					hasPlayedSoundEffect = true;
				}
			}
			UpdateG();
        }
		/// <summary>
		/// 管理クラスなので、一括してScrollUpdate()を呼ぶ。
		/// </summary>
		/// <param name="criteriaPosition"></param>
		/// <param name="autoScroll"></param>
		public override void ScrollUpdate(Vector2 criteriaPosition, bool autoScroll)
		{
			foreach (Terrain terrain in mapObjects) {
				terrain.ScrollUpdate(criteriaPosition, autoScroll);

				if (terrain.distanceToCamara < terrain.activeDistance) terrain.isActive = true;
				else terrain.isActive = false;
			}
			base.ScrollUpdate(criteriaPosition, autoScroll);
		}
		private void UpdateG()
		{
			float sumX = 0, sumY = 0;
			foreach (Terrain terrain in mapObjects) {
				//Vector2 sum += terrain.position;
				sumX += terrain.position.X;
				sumY += terrain.position.Y;
			}
			this.position = new Vector2(sumX, sumY) / (float)mapObjects.Count;
		}
		public override void UpdateTimeCoef()
		{
			if (stage.slowmotion.isSlow) {
				foreach (Terrain terrain in mapObjects)	terrain.timeCoef = timeCoefObject;
			} else {
				foreach (Terrain terrain in mapObjects) terrain.timeCoef = 1.0f;
			}
		}
        /// <summary>
        /// mapObejctsの移動パターン0。
        /// </summary>
        /// <param name="startPosition">初期位置</param>
        /// <param name="endTime">何フレーム動かすか</param>
        /// <param name="movementType">このパターン内での動かし方のタイプ</param>
        /// <param name="gravity">個々の地形が落ちるときの重力(落下速度の緩急調節) ←つらら用</param>
        public void MovePattern0(Vector2 startPosition, int endTime, int type, double gravity)
        {
            int spwanInterval = 30;
            bool isEnding = false;
           // int spawnNum = 0;
            
            //Random rnd;
            if (counter > endTime - 240) {
                isEnding = true;
                spwanInterval = 45;
			}

			switch (type) {
				case 0:// 一斉
					if (counter == 0) {
						isEnd = false;
						foreach (Terrain terrain in mapObjects) {
							terrain.position = startPosition + terrain.localPosition;
							terrain.isBeingUsed = true;
							if (terrain is Icicle) {
								(terrain as Icicle).isFallingDown = false;
								terrain.hasPlayedSoundEffect = false;
								terrain.speed = Vector2.Zero;
							}
						}
					}
					break;
				case 1:// ランダムなindexを選択して
					if (counter == 0) {
						isEnd = false;
						t = 0;
						spawnNum = 0;
						//hasPlayedSoundEffect = false;
					}
					// "ランダムっぽい"仕様.　順番に落としたいときはbreakするべし
					/*for (int i = 0; i < mapObjects.Count; i++)
						if (counter % (spwanInterval * (i + 1)) == 0) {
							mapObjects[i].isBeingUsed = true;
							mapObjects[i].position = startPosition + mapObjects[i].localPosition;
							if(mapObjects[i] is Icicle) {
								(mapObjects[i] as Icicle).isFallingDown = false;
								(mapObjects[i] as Icicle).hasPlayedSoundEffect = false;
								mapObjects[i].speed = Vector2.Zero;
							}
						}*/
					if (counter % spwanInterval == 0) {
						spawnNum = rnd.Next(3, mapObjects.Count);// min=3
						//spawnIndex = rnd2.Next(0, spawnNum);
						spawnIndex.Clear();
						for (int i = 0; i <= spawnNum; i++) spawnIndex.Add(rnd2.Next(0, spawnNum));

						for (int i = 0; i <= spawnIndex.Count - 1; i++) {//spawnNum; i++)
							if (mapObjects[spawnIndex[i]] is Icicle && (mapObjects[spawnIndex[i]] as Icicle).hasFalled) {
								(mapObjects[spawnIndex[i]] as Icicle).hasFalled = false;
								(mapObjects[spawnIndex[i]] as Icicle).isFallingDown = false;
								//mapObjects[spawnIndex[i]].hasPlayedSoundEffect = false;
								mapObjects[spawnIndex[i]].speed = Vector2.Zero;
								mapObjects[spawnIndex[i]].position = startPosition + mapObjects[spawnIndex[i]].localPosition;
								mapObjects[spawnIndex[i]].isBeingUsed = true;
							}
						}
					}

					if (isEnding && t == 0) {// coutner = 361
						foreach (Terrain terrain in mapObjects) {
							terrain.gravity = gravity - gravity / 2;
							(terrain as Icicle).hasFalled = true;
						}
						t++;
					}
					break;
				case 2:// 順に
					if (counter == 0) {
						isEnd = false;
						spawnNum = 0;
						foreach (Terrain terrain in mapObjects) {
							terrain.position = startPosition + terrain.localPosition;
							//terrain.isBeingUsed = true;
							if (terrain is Icicle) {
								(terrain as Icicle).isFallingDown = false;
								terrain.hasPlayedSoundEffect = false;
								terrain.speed = Vector2.Zero;
							}
						}
						spawnIndex.Clear();
						for (int i = 0; i <= spawnNum; i++) spawnIndex.Add(i);
					} else if (counter % spwanInterval == 0) {
						//for (int i = 0; i < spawnIndex.Count; i++) {//spawnNum; i++)
						if (mapObjects[spawnNum] is Icicle && (mapObjects[spawnNum] as Icicle).hasFalled) {
							mapObjects[spawnNum].Initialize();
							mapObjects[spawnNum].speed = Vector2.Zero;
							mapObjects[spawnNum].position = startPosition + mapObjects[spawnNum].localPosition;
							mapObjects[spawnNum].isBeingUsed = true;
						}
						spawnNum++;

						// 終了
						if (spawnNum == mapObjects.Count - 1) {//spawnIndex.Count - 1/* && !mapObjects[spawnIndex.isAlive*/) {
							spawnNum = counter = 0;
							isEnd = true;
							foreach (Terrain terrain in mapObjects) terrain.gravity = gravity;
						}
					}
					break;
			}
            counter++;

			// 終了処理
			if (counter > endTime) {// 4/14　ここは到達している counter == 601
				counter = 0;
				isEnd = true;
				foreach (Terrain terrain in mapObjects) terrain.gravity = gravity;// 元の重力に戻す
			}
        }
		private bool hasIniedAll;
		/// <summary>
		/// mapObejctsの移動パターン1。startPositionからendPositionまで移動させる
		/// </summary>
		/// <param name="startPosition">初期位置</param>
		/// <param name="endPosition">終了位置</param>
		/// <param name="speed">移動速度[pixel/frame]</param>
		/// <param name="movementType">0:全部、1:順番</param>
        public void MovePattern1(Vector2 startPosition, Vector2 endPosition, float speed, int movementType)
        {
            Vector2 speedVector = Vector2.Zero;
            speedVector = Vector2.Normalize(endPosition - startPosition);// *new Vector2(5, 5);
            speedVector *= new Vector2(speed, speed);
            int spwanInterval = 30;

			switch (movementType) {
				case 0:// 一斉に
					if (counter == 0) {
						isEnd = false;
						hasPlayedSoundEffect = false;
						hasIniedAll = false;
						foreach (Terrain terrain in mapObjects) {
							terrain.position = startPosition + terrain.localPosition;
							terrain.isBeingUsed = true;
						}
					}

					
					break;
				case 1:// 順に
					if (counter == 0) {
						isEnd = false;
						hasPlayedSoundEffect = false;
						hasIniedAll = false;
						spawnNum = 0;
						foreach (Terrain terrain in mapObjects) {
							terrain.position = startPosition + terrain.localPosition;
							terrain.isBeingUsed = false;
						}
					}
					/*for (int i = 0; i < mapObjects.Count; i++) {// spawnNum
						if (counter % (spwanInterval * (i + 1)) == 0) {
							mapObjects[i].isBeingUsed = true;
							mapObjects[i].position = startPosition + mapObjects[i].localPosition;
						}
					}*/
					if (counter % spwanInterval == 0 && !hasIniedAll) {
						mapObjects[spawnNum].isBeingUsed = true;
						mapObjects[spawnNum].position = startPosition + mapObjects[spawnNum].localPosition;
						if (spawnNum < mapObjects.Count - 1) spawnNum++;
						else hasIniedAll = true;
					}

					
					break;
			}
            counter++;// 位置に注意：最後におくとisEndフラグを立てると共にcounter=0にしても++される

			foreach (Terrain terrain in mapObjects) {
				if (terrain.isBeingUsed) {
					terrain.speed = speedVector;
					terrain.position += terrain.speed;
				}
			}

			// 終了処理
			switch (movementType) {
				case 0:// 一斉に
					if ((Vector2.Distance(mapObjects[0].position, endPosition) < speed)/* || (mapObjects[0].position.X < endPosition.X && mapObjects[0].position.Y > endPosition.Y)*/) { // どれも同じ動きをするから[0]だけでいいだろう?
						counter = 0;
						isEnd = true;
						foreach (Terrain terrain in mapObjects) {
							terrain.isBeingUsed = false;
							terrain.fadeOut = true;
						}
					}
					break;
				case 1:// 順に
					if ((Vector2.Distance(mapObjects[mapObjects.Count - 1].position, endPosition + mapObjects[mapObjects.Count - 1].localPosition) < speed)/* || (mapObjects[0].position.X < endPosition.X && mapObjects[0].position.Y > endPosition.Y)*/) { // どれも同じ動きをするから[0]だけでいいだろう?
						counter = 0;
						isEnd = true;
						foreach (Terrain terrain in mapObjects) {
							terrain.isBeingUsed = false;
							terrain.fadeOut = true;
						}
					}
					break;
			}
            //if(mapObjects[mapObjects.Count - 1].position.X < endPosition.X + mapObjects[mapObjects.Count - 1].localPosition.X) {
            //if(Vector2.Distance(mapObjects[mapObjects.Count - 1].position, endPosition + mapObjects[mapObjects.Count - 1].localPosition) < 5) {
			
        }

		protected override void Move(float moveSpeed, params Vector2[] wayPoints)
		{
			foreach (Terrain terrain in mapObjects) {
				base.Move(moveSpeed, wayPoints);
			}
		}
		public override void IsHit(Object targetObject)
		{
			if (mapObjects[0] is DamageObject) { }
			foreach (Terrain terrain in mapObjects) {
				terrain.IsHit(targetObject);
				if (targetObject.isHit || targetObject.isDamaged) {
					if (mapObjects[0] is DamageObject && (mapObjects[0] as DamageObject).textureType == 0) { }
					break;// １つでも当たってないものがあればfalseにされるので必要
				}
			}
		}

        /// <summary>
        /// ×管理クラスなので基本的に何も描画しない。
		/// ○管理クラスなので、管理してるObject全てを一括描画する。
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			foreach (Terrain terrain in mapObjects) terrain.Draw(spriteBatch);
        }
    }
}
