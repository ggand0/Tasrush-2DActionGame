using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	public class GameStatus
	{
		//public int second, minute;
		/// <summary>
		/// プレイ開始時からの秒数を格納する。
		/// </summary>
		public double time;

		public int maxComboCount;
		public int comboCountVisibleTime;
		public readonly int maxComboCountVisibleTime = 240;
	}

	/// <summary>
	/// ゲーム内Objectの管理クラスであるStageの基本クラス。
	/// </summary>
	public class Stage : Scene
	{
		#region Member variable
		// GameSystem
		public Reverse reverse { get; protected set; }
		public Camera camera { get; private set; }

		public DamageControl damageControl { get; private set; }
		public EffectControl effectControl { get; private set; }
		public UserInterface userInterface { get; protected set; }
		public SlowMotion slowmotion { get; protected set; }
		private Debug debug;
		private Effect effectBoss, effectStage, effectPlayerDeath;

		// Objects

		/// <summary>
		/// 扱いやすいように持たせておく...？
		/// </summary>
		public Player player { get; protected set; }
		public Sword sword { get; protected set; }
		public Boss boss { get; protected set; }
		/// <summary>
		/// 背景。UIに入れてもいいかも
		/// </summary>
		public ScrollingBackground scrollingBackGround { get; protected set; }
		public ScrollingBackground frontalScrollingBackGround { get; protected set; }
		public ScrollingTASEffect scrollingTASEffect { get; protected set; }

		// Lists
		/// <summary>
		/// 静的な地形を描画したbackGroundのリスト
		/// </summary>
		protected List<BackGround> backGrounds;
		/// <summary>
		/// ゲーム中に新たにリストに追加すべきオブジェクトを生成する場合、一旦このリストを経由する。
		/// （Load等の関係で。後で訂正するかも）
		/// </summary>
		public List<Object> unitToAdd { get; set; }
		/// <summary>
		/// 画面内に存在する動的な地形１
		/// </summary>
		public List<Terrain> activeDynamicTerrains1 { get; set; }
		/// <summary>
		/// 画面内に存在する動的な地形２
		/// </summary>
		public List<Terrain> activeDynamicTerrains2 { get; set; }
		/// <summary>
		/// カメラの左端にブロックの壁を作ろうと試みた名残
		/// </summary>
		public List<Block> cameraWall { get; set; }
		/// <summary>
		/// わすれた
		/// </summary>
		public List<ScrollingBackground> bgs { get; set; }
		/// <summary>
		/// コンボ中の（プレイヤーに攻撃されている）オブジェクト
		/// </summary>
		public List<Object> inComboObjects { get; set; }
		/// <summary>
		/// 画面内に存在するキャラクターのリスト
		/// </summary>
		public List<Character> activeCharacters;
		/// <summary>
		/// 画面内に存在するすべての地形のリスト
		/// </summary>
		public List<Terrain> activeTerrains;
		/// <summary>
		/// 画面内に存在する動的な地形のリスト
		/// </summary>
		public List<Terrain> activeDynamicTerrains;
		/// <summary>
		/// わすれた
		/// </summary>
		public List<Terrain> activeDynamicTerrainsD;
		/// <summary>
		/// 画面内に存在する静的な地形のリスト
		/// </summary>
		public List<Terrain> activeStaticTerrains;
		/// <summary>
		/// 画面内を飛んでいる弾のリスト
		/// </summary>
		public List<Bullet> activeBullets;
		/// <summary>
		/// 画面内に存在するすべてのオブジェクトのリスト
		/// </summary>
		public List<Object> activeObjects;
		/// <summary>
		/// もう使ってないような..?
		/// </summary>
		public List<Bullet> damagedBullets;
		/// <summary>
		/// プレイヤーに攻撃されてダメージを受けた（ダメージ処理をされるべき）オブジェクトのリスト。確かもう使ってない
		/// </summary>
		public List<Object> damagedObjects;
		public List<Object> attackedObjects;
		/// <summary>
		/// １フレーム内でダメージを与えたObjectとダメージを受けたObjectの組を格納するリスト。
		/// damageControlに渡す
		/// </summary>
		public List<Object2> adObjects;
		/// <summary>
		/// Stage上の全ての（Characterが所持しているものも含めた）武器のリスト。
		/// </summary>
		public List<Weapon> weapons;
		/// <summary>
		/// 今現在誰かに使われていてアクティブな武器のリスト（画面内にいるかどうかは無関係）
		/// </summary>
		public List<Weapon> activeWeapons;

		// Status
		/// <summary>
		/// Stage中の敵のｋ本的なHP
		/// </summary>
		public int defHP { get; set; }
		public string contentDirectory { get; set; }
		protected bool BGMchanged;
		public float endOfTheStage { get; set; }
		/// <summary>
		/// 強制スクロール中かどうか
		/// </summary>
		public bool isScrolled { get; set; }
		/// <summary>
		/// 強制スクロールのスクロール速度
		/// </summary>
		public float scrollSpeed { get; private set; }
		public float ScrollSpeed
		{
			get { return this.scrollSpeed; }
			set { this.scrollSpeed = value; }
		}
		public Vector2 bossLocation { get; set; }
		//private Vector2 playerDeathDrawPos { get; private set; }
		public float surfaceHeightAtBoss { get; set; }
		private bool hasPlayedSE { get; set; }
		/// <summary>
		/// TAS中かどうか
		/// </summary>
		public bool isAccelerated { get; set; }
		public bool hasEffectedBeginning { get; set; }
		public bool hasEffectedWarning { get; set; }
		public bool hasEffectedBossExplosion { get; set; }

		public bool hasEffectedPlayerDeath { get; set; }
		public bool toGameOver { get; internal set; }
		/// <summary>
		/// ボス戦移行シーンにいるか
		/// </summary>
		public bool toBossScene { get; set; }
		/// <summary>
		/// ボス戦中か
		/// </summary>
		public bool inBossBattle { get; set; }
		/// <summary>
		/// 通常のゲーム時間カウンタ
		/// </summary>
		public int gameTimeNormal { get; set; }
		/// <summary>
		/// TASを使った時間のカウンタ
		/// </summary>
		public int gameTimeTAS { get; set; }
		public bool isPausing { get; set; }

		// GameInfo
		public GameStatus gameStatus { get; private set; }

		// Lists, Objects：以下はインスタンス生成されたのちは不変の（ある意味静的な）リスト群
		protected List<MapData> mapDatas = new List<MapData>();
		/// <summary>
		/// 動的な地形のリスト
		/// </summary>
		public List<Terrain> dynamicTerrains;
		/// <summary>
		/// 静的な地形：Blockなどを入れるリスト
		/// </summary>
		public List<Terrain> staticTerrains;
		public List<Character> characters;
		public List<Bullet> bullets;
		public List<Character> damagedCharacters;
		public List<Character> effectedCharacters;
		public List<Object> objects2;
		public List<Object> objects;
		public List<Effect> effects;

		// I/O
		protected class MapData// structだと配列の初期化が面倒なのでクラスにした
		{
			public string fileName;
			public string original;
			public string[] devided1;
			public string[,] devided2;
			public MapData(string fileName)
			{
				this.fileName = fileName;
				devided1 = new string[50000];
				devided2 = new string[50000, 7];
			}
		}
		public string input_original;
		public string[] input_second;
		public string[,] input_last;
		#endregion
		protected bool isHighLvl;
		public float bossScreenEdgeLeft, bossScreenEdgeRight;
		/// <summary>
		/// 最小化復帰時にBackGroundを再ロードするためのフラグ
		/// </summary>
		private bool reloadScreen;
		public Stage()
		{
		}
		public Stage(Scene privousScene, bool isHighLvl)
			: base(privousScene)
		{
			this.isHighLvl = isHighLvl;
			// System
			reverse = new Reverse(this, game);
			slowmotion = new SlowMotion();
			damageControl = new DamageControl(this, attackedObjects, damagedObjects);
			effectControl = new EffectControl(this);
			camera = new Camera(this, 320, 240, 640, 480);
			effectBoss = new Effect(this);
			effectStage = new Effect(this);
			effectPlayerDeath = new Effect(this);
			debug = new Debug(game, this);

			// Lists
			characters = new List<Character>();
			dynamicTerrains = new List<Terrain>();
			staticTerrains = new List<Terrain>();
			bullets = new List<Bullet>();
			objects = new List<Object>();
			effects = new List<Effect>();
			damagedCharacters = new List<Character>();
			effectedCharacters = new List<Character>();
			activeDynamicTerrains1 = new List<Terrain>();
			activeDynamicTerrains2 = new List<Terrain>();
			inComboObjects = new List<Object>();
			backGrounds = new List<BackGround>();
			activeCharacters = new List<Character>();
			activeTerrains = new List<Terrain>();
			activeDynamicTerrains = new List<Terrain>();
			activeDynamicTerrainsD = new List<Terrain>();
			activeStaticTerrains = new List<Terrain>();
			activeBullets = new List<Bullet>();
			activeObjects = new List<Object>();
			damagedBullets = new List<Bullet>();
			damagedObjects = new List<Object>();
			attackedObjects = new List<Object>();
			adObjects = new List<Object2>();
			weapons = new List<Weapon>();
			activeWeapons = new List<Weapon>();

			unitToAdd = new List<Object>();
			cameraWall = new List<Block>();
			bgs = new List<ScrollingBackground>();

			// UI
			userInterface = new UserInterface(game, this);
			userInterface.Initialize();

			// Else
			scrollingBackGround = new ScrollingBackground(this, Vector2.Zero);
			frontalScrollingBackGround = new ScrollingBackground(this, Vector2.Zero);
			frontalScrollingBackGround.isFrontal = true;
			scrollingTASEffect = new ScrollingTASEffect(this, Vector2.Zero);
			defHP = 3;
			scrollSpeed = 2;
			
			Load();
		}

		/// <summary>
		/// 指定のテキストファイルからステージのマップデータを読み込む。
		/// </summary>
		/// <param name="stageNumber"></param>
		/// <param name="fileName"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected void LoadMapData(int stageNumber, string fileName, float x, float y)
		{
			objects.Clear();                                                                                                    // Stage内の全てのListを初期化：
			backGrounds.Clear();
			staticTerrains.Clear();
			dynamicTerrains.Clear();
			characters.Clear();
			weapons.Clear();
			bullets.Clear();

			mapDatas.Add(new MapData(fileName));                                                                                // inicialize
			StreamReader sr = new StreamReader(fileName);                                                                       // テキストファイルの読み込み
			mapDatas[mapDatas.Count - 1].original = sr.ReadToEnd();                                                             // とりあえず最後まで文字列に放り込む
			mapDatas[mapDatas.Count - 1].devided1 = mapDatas[mapDatas.Count - 1].original.Replace("\r\n", "\n").Split('\n');    // "\r\n"を'\n'に置き換えた後(Spritはchar型しか受け取らない)に、まず１行ずつに分割　

			if (mapDatas[0].original != "")                                                                                     // 空ファイル対策
				for (int i = 0; i < mapDatas[mapDatas.Count - 1].devided1.Length; i++) {                                        // ファイルの末尾に改行コードがあるとエラーを吐くので注意
					for (int j = 0; j < 7; j++) {
						string[] input_tmp = new string[7];
						if (mapDatas[mapDatas.Count - 1].devided1[i].Contains("Segment")) {
							input_tmp = mapDatas[mapDatas.Count - 1].devided1[i].Split(new char[] { ',' }, 7);                  // countは区切り文字の数ではなく分けるブロックの数らしい
						} else { 
							input_tmp = mapDatas[mapDatas.Count - 1].devided1[i].Split(new char[] { ',' });
						}
						mapDatas[mapDatas.Count - 1].devided2[i, j] = input_tmp[j];                                             // 次にカンマで分割：一時的な配列に入れた後に二次元配列の列へ代入
					}
				}
			sr.Close();
			AddMapData(mapDatas[mapDatas.Count - 1], x, y);
			//sr.Close();
		}
		/// <summary>
		/// LoadMapDataで読み取ったマップの情報を元にStageのListにObjectを追加していく。
		/// </summary>
		/// <param name="mapData"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected void AddMapData(MapData mapData, float x, float y)
		{
			for (int i = 0; i < mapData.devided1.Length; i++) {							// 引数がnullだとエラーになるので注意(配列の長さ的な意味で) 12/5 -1は要らない...?"mapData.devided1.Length-1"
				switch (mapData.devided2[i, 0]) {										// Objectの種類ごとに指定 (できれば new "読み取った文字列" (constructor...)としたい)
					#region Terrain
					case ("Block"):
						/*terrains.Add(new Block(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]), 
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]),-4));*/
						// また"0Block"でエラーである
						staticTerrains.Add(new Block(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), int.Parse(mapData.devided2[i, 5]), int.Parse(mapData.devided2[i, 6]))); // こうじゃなくてTerrain内のBlockを参照するように
						break;
					#region Slope
					// onGround
					case ("Slope"):// "Slope"のみならデフォルトで生成
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("Slope0"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, 0));
						break;
					case ("Slope1"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, 1));
						break;
					case ("Slope2"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, 2));
						break;
					case ("Slope-0"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), false, 0));
						break;
					case ("Slope-1"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), false, 1));
						break;
					case ("Slope-2"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), false, 2));
						break;
					// onCeiling
					case ("-Slope"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, true, 0));
						break;
					case ("-Slope0"):// Stage2:objects1にもstaticTerrainsにも入ってないようだ...?呼ばれて入っていたはずなのだが...
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, true, 0));
						break;
					case ("-Slope1"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, true, 1));
						break;
					case ("-Slope2"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, true, 2));
						break;
					case ("-Slope-0"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, false, 0));
						break;
					case ("-Slope-1"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, false, 1));
						break;
					case ("-Slope-2"):
						staticTerrains.Add(new Slope(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), true, false, 2));
						break;
					#endregion

					case ("CollapsingBlock"):
						dynamicTerrains.Add(new CollapsingBlock(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("CollapsingFloor"):
						dynamicTerrains.Add(new CollapsingFloor(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("DamageBlock"):
						staticTerrains.Add(new DamageBlock(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("Foothold"):
						staticTerrains.Add(new Foothold(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("Water"):
						dynamicTerrains.Add(new Water(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("Icicle"):
						dynamicTerrains.Add(new Icicle(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					/*case ("SnowBall"):
						dynamicTerrains.Add(new SnowBall(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;*/
					case ("ConveyorBelt"):
						dynamicTerrains.Add(new ConveyorBelt(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;
					case ("Segment"):
						// 点座標の情報を１つのstringに押し込んでるので解凍が面倒

						//string[] tmp = new string[10];  // とりあえず10点まで対応.Listに(ry
						//string[] tmp2 = new string[20]; // 座標ひとまとまりずつ
						// List<string> tmp = new List<string>();
						List<string> tmp2 = new List<string>();
						string[] t = new string[2];

						//string[] tmpX = new string[10]; // 二次元配列ではなく1次元×1にしよう
						//string[] tmpY = new string[10];
						List<string> tmpX = new List<string>();
						List<string> tmpY = new List<string>();
						List<Vector2> tmpV = new List<Vector2>();
						tmpV.Clear();

						string[] tmp = mapData.devided2[i, 5].Split(new char[] { '/' });  // Splitで配列に分けて区切るからtmp(string[])にそのまま代入でいい.  input_lastがnull...?何故.. ←mapData-を入れ忘れてた
						for (int j = 0; j < tmp.Length/*10*/; j = j + 2) {
							// 最後のindexが""になってindexOutOfRangeException 単に10に満たないから？

							t = tmp[j].Split(':');   // 失敗：最後の一組しか入ってない.    暗黙的型変換乙！
							//tmp2[j] = t[0];
							//tmp2[j + 1] = t[1];
							tmpX.Add(t[0]);
							tmpY.Add(t[1]);
							if (tmp[j + 1] == "" || tmp[j + 2] == "") break;  // よくない処理だが時間がないっす
						}

						/*for(int i=0;i<20;i++) {
							if(tmp2[i]!=null) {
								if (i % 2 == 0) tmpX[i-1] = tmp2[i];  // 2次元配列の片方の側に[]を(丸ごと)入れたい←無理、で変えたがこれだと全部片方に入っちゃうな...
								else tmpY[i-1] = tmp2[i];
						}
						for (int i = 0; i < 10; i++)
							tmpV.Add(new Vector2(float.Parse(tmpX[i]), float.Parse(tmpY[i])));
						*/

						// 直接生成しようか
						/*for (int j = 0; j < 10; j = j + 2) {
							tmpV.Add(new Vector2(float.Parse(tmp2[j]), float.Parse(tmp2[j+1])));
							if (tmp2[j + 2] == null) break;
						}*/
						//foreach(string str in 
						for (int j = 0; j < tmpX.Count;/*tmp.Length - 1*/ j++) {//j = j + 2) {
							//tmpV.Add(new Vector2(float.Parse(tmp2[j]), float.Parse(tmp2[j+1])));
							tmpV.Add(new Vector2(float.Parse(tmpX[j]), float.Parse(tmpY[j])));
						}

						dynamicTerrains.Add(new TracingFoothold(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							96, int.Parse(mapData.devided2[i, 4]), tmpV));
						break;
					#endregion
					#region Charactter
					// Stage1:
					case ("StationalEnemy"):
						characters.Add(new StationalEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							48, 48, defHP));
						break;
					case ("JumpingEnemy"):
						characters.Add(new JumpingEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							40, 40, defHP));
						break;
					case ("FlyingEnemy"):
						characters.Add(new FlyingEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							64, 48, defHP));
						break;
					case ("ShootingEnemy"):
						characters.Add(new ShootingEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							48, 48, defHP));
						break;
					case ("FlyingOutEnemy"):
						characters.Add(new FlyingOutEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) - 8 + y,//-8は飛び出てる部分の補正
							40, 40, defHP));

						break;

					// Stage2:
					case ("FlyingEnemy2"):
						characters.Add(new FlyingEnemy2(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), defHP));
						break;
					case ("FlyingOutEnemy2"):
						characters.Add(new FlyingOutEnemy2(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), defHP));
						break;
					case ("SkatingEnemy"):
						characters.Add(new SkatingEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							40, 40, defHP));// hardcording...
						break;
					case ("RushingOutEnemy"):
						characters.Add(new RushingOutEnemy(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4]), 0, defHP));
						break;
					case ("SnowBall") :
						characters.Add(new SnowBall(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							int.Parse(mapData.devided2[i, 3]), int.Parse(mapData.devided2[i, 4])));
						break;

					// Stage3:
					case ("StationalEnemy3"):
						characters.Add(new StationalEnemy3(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							48, 48, defHP));
						break;
					case ("JumpingEnemy3"):
						characters.Add(new JumpingEnemy3(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							40, 40, defHP - 1));
						break;
					case ("FlyingEnemy3"):
						characters.Add(new FlyingEnemy3(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							60, 48, defHP));
						break;
					case ("FlyingOutEnemy3"):
						characters.Add(new FlyingOutEnemy3(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) - 13 + y,   // 13は補正
							40, 40, defHP));
						break;
					case ("SkatingEnemy2"):
						characters.Add(new SkatingEnemy2(this, float.Parse(mapData.devided2[i, 1]) + x, float.Parse(mapData.devided2[i, 2]) + y,
							40, 40, defHP));
						break;
					#endregion
					default: break;
				}
			}

			// 背景バッファを必要分追加: 
			for (int i = 0; i < ((int)CheckMapEnd(mapData.devided2)) / 2048 + 1; i++) {	// stage0は45までしかinputastに入ってない xna側でいくつか読んでつなげてるから
				backGrounds.Add(new BackGround(this, 2048, 480));
				if (x == 0)
					backGrounds[backGrounds.Count - 1].position.X += i * 2048;
				else if (x > 0)
					backGrounds[backGrounds.Count - 1].position.X = x + i * 2048;
			}
		}
		/// <summary>
		/// AddMapDataと分離.各Listをobjectsに全部放り込む
		/// </summary>
		protected void AddObjects()
		{
			objects.Clear();
			foreach (Terrain terrain in dynamicTerrains) objects.Add(terrain);
			foreach (Terrain terrain in staticTerrains) objects.Add(terrain);
			foreach (Character character in characters) objects.Add(character);
			foreach (Bullet bullet in bullets) objects.Add(bullet);
			foreach (Weapon weapon in weapons) objects.Add(weapon);
			objects.Add(scrollingBackGround);
			objects.Add(frontalScrollingBackGround);
			foreach (BackGround bg in backGrounds) objects.Add(bg);
		}
		/// <summary>
		/// マップの地形の最大X座標を返す
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		protected float CheckMapEnd(string[,] input)
		{
			float max; int i;
			max = 0;
			for (i = 0; i < input.GetLength(0); i++)        // 列要素を比較
				if (input[i, 1] == null) break;
				else if (max < float.Parse(input[i, 1]))
					max = float.Parse(input[i, 1]);

			this.endOfTheStage = max;
			return max;
		}
		protected string[] LoadParameters(string fileName)
		{
			StreamReader sr = new StreamReader(fileName);
			string tmpString = sr.ReadToEnd();

			return tmpString.Split(new char[] { ',' }); 

		}
		public override void Load()
		{
			#region Terrain
			/*foreach(Terrain terrain in dynamicTerrains) {
                if (terrain is Block && !terrain.loadManually) {
                    if (terrain.movementType == 0) 
                        if (game.stageNum != 0)                      terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Block02");
                        else                                        terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Block01");
                    else if (terrain.movementType == 1)                     terrain.Load(content, "Object\\Terrain\\FrozenBlock");
                    else if (terrain.movementType == 2)                     terrain.Load(content, "Object\\Terrain\\BlockV"); 
                }
                    
                if (terrain is SnowBall)                            terrain.Load(content, "Object\\Terrain\\SnowBall", "Object\\Weapon\\Weapon");
                if (terrain is Meteor) {
                                                                    (terrain as Meteor).Load(content, "Object\\Terrain\\Meteor01", ref terrain.texture);
                                                                    (terrain as Meteor).Load(content, "Object\\Terrain\\Meteor02", ref terrain.texture2);
                }
                if (terrain is DamageObject)                        terrain.Load(content);
            }*/


			foreach (Terrain terrain in staticTerrains) {
				if (terrain is Block && !(terrain is DamageBlock) && !terrain.loadManually) {
					if (terrain.type == 0) {
						if (terrain.isOn && !terrain.isUnder && !(terrain as Block).isUnderSlope) {// 草ブロックにする条件
							terrain.Load(content, "Object\\Terrain\\Block" + (game.stageNum - 1).ToString() + "1");
						} else terrain.Load(content, "Object\\Terrain\\Block" + (game.stageNum - 1).ToString() + "2");
					} else if (terrain.type == 1) terrain.Load(content, "Object\\Terrain\\FrozenBlock");
					else if (terrain.type == 2) terrain.Load(content, "Object\\Terrain\\BlockV");
				} /**/ // 静的な地形に関しては今のところここでもう1回読み込む必要あり：GetDirectionの位置の関係
			}
			#endregion
			#region Character
			// 移行済み
			#endregion
			#region Bullets & Weapon
			// 移行済み
			if (weapons.Count == 0) sword.Load(content, "Object\\Weapon\\Melee");
			/*else foreach (Weapon weapon in weapons) {
				if (weapon is Sword) {
				}
				else                                        weapon.Load(content, "Object\\Weapon\\Melee");
			}*/
			#endregion
			#region Etc
			// UI
			userInterface.Initialize();
			userInterface.Load();

			// BackGround
			scrollingBackGround = new ScrollingBackground(this, Vector2.Zero);
			frontalScrollingBackGround = new ScrollingBackground(this, Vector2.Zero);
			frontalScrollingBackGround.isFrontal = true;

			if (!game.isHighLvl) {
				scrollingBackGround.Load(game.GraphicsDevice, content, "Object\\BackGround\\BackGround" + (game.stageNum - 1).ToString() + "2");
				frontalScrollingBackGround.Load(game.GraphicsDevice, content, "Object\\BackGround\\BackGround" + (game.stageNum - 1).ToString() + "1");
			} else {
				scrollingBackGround.Load(game.GraphicsDevice, content, "Object\\BackGround\\BackGround" + (game.stageNum - 1).ToString() + "3");
				frontalScrollingBackGround.Load(game.GraphicsDevice, content, "Object\\BackGround\\BackGround" + (game.stageNum - 1).ToString() + "1");
			}
			scrollingTASEffect = new ScrollingTASEffect(this, Vector2.Zero);
			scrollingTASEffect.Load(game.GraphicsDevice, content, "Effect\\TAS1");

			// BGM
			//SoundControl.IniMusic("Audio\\BGM\\forest");
			//SoundControl.Play();
			gameStatus = new GameStatus();
			DrawBackGround();// 背景バッファの作成
			player.position = new Vector2(game.hasReachedCheckPoint ? 13400 : 100, 100);
			gameTimeNormal = 0;
			gameTimeTAS = 0;

			bossScreenEdgeLeft = boss.defaultPosition.X - 640;// -Player.screenPosition.X;
			bossScreenEdgeRight = boss.defaultPosition.X + 640 + 100;// 100
			camera.position = game.hasReachedCheckPoint ? new Vector2(player.position.X, 0) : Vector2.Zero;
			isScrolled = true;
			hasEffectedWarning = false;
			hasEffectedBeginning = false;
			hasEffectedBossExplosion = false;
			inBossBattle = false;
			BGMchanged = false;
			#endregion
		}

        public void ResetDeathEffect()
        {
            this.effectPlayerDeath = new Effect(this);
        }

		/// <summary>
		/// 1フレーム中の計算処理。通常/TAS中/スロー で分岐
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(double dt)
		{
			gameStatus.time += dt;
			/*if (game.Deactivated) {
				reloadScreen = true;
			} else if (game.Activated && reloadScreen) {

			}*/

			if (isAccelerated) {
				player.TASpower += -1;
				scrollingTASEffect.Update(5);
			}
			if (player.TASpower < 1) {
				isAccelerated = false;
			}

			//timerForFinishReverse.Update();
			if (reverse.isReversed) {
				UpdateReverse();
			} else if (!reverse.isReversed && slowmotion.isSlow) {
				UpdateSlow();
			} else {
				UpdateNormal();
			}
			userInterface.Update();

			this.gameTimeNormal++;
		}
		private void UpdateNormal()
		{
			int jumpingEnemyCount = 0;

			if (player.position.X > 13000 && !game.hasReachedCheckPoint) {
				game.hasReachedCheckPoint = true;
				game.tmpScore = game.stageScores[game.stageNum - 1];
			}
			if (isPausing) PushScene(new PauseMenu(this));
			if (unitToAdd.Count > 0) AddUnits();				// 新たにObjectが発生したらリストに追加する

			// BackGround
			double elapsed = game.dt;//(float)gameTime.ElapsedGameTime.TotalSeconds;
			scrollingBackGround.Update(1/*elapsed * 100*/);
			frontalScrollingBackGround.Update(2);
			// TimeCoef
			UpdateTimeCoef();
			// ObjectのUpdate
			/*foreach (Object obj in activeObjects) {
				if (!(obj is Weapon)) obj.Update();//&& !(obj is Bullet)
			}
			*/
			foreach (Object obj in activeObjects) {
				if (obj is JumpingEnemy) {
					jumpingEnemyCount++;
					(obj as JumpingEnemy).canPlayse = jumpingEnemyCount <= JumpingEnemy.maxSoundEffectNum;
				}
				obj.Update();
			}

			// Reverse用のLogをとる←色々Update終わったあとにLog取らせることに
			// 注意！極力UpdateReverseでのreverse.Updateと同タイミングに置くこと！！
            reverse.RegenerateTAS();
			reverse.UpdateLog();

			//後処理
			//objects.RemoveAll((Object obj) => !obj.isAlive);// ここを入れると死亡エフェクトが入る前に(ry

			Collide();											// CollisionDetection(Terrain、Weapon等)
			ScrollUpdate();										// スクロール時の座標変換(画面上のすべてのオブジェクトについて)
			PlayBGM();											// BGM管理
			damageControl.Update2();							// WeaponとEnemyのダメージ判定の調整
			UpdateUICalculate();
		}
		private void UpdateReverse()
		{
			// Reverse実行 注意:置く場所間違えるとバグる
			reverse.Update();
			scrollingTASEffect.Update(5);

			// 画面スクロール
			if (!reverse.isFinishing) {
				reverse.ScrollUpdate();
			} else {
				ScrollUpdate();
				reverse.isFinishing = false;
			}
			gameTimeTAS++;
		}
		private void UpdateSlow()
		{
			slowmotion.Update();

			// unitsAdd:分裂する敵などが死んだら、その場(U@date()を実行中)でcharacters二追加するとforeachが使えないのでqueに追加しておいて次回Updateの始めに追加、という形にする。
			if (unitToAdd.Count > 0)
				for (int i = unitToAdd.Count - 1; i >= 0; i--) {
					//while (unitToAdd.Count > 0) {
					if (unitToAdd[i] is Character) {
						objects.Add(unitToAdd[i]);
						characters.Add(unitToAdd[i] as Character);
						unitToAdd.Remove(unitToAdd[i]);
					} else if (unitToAdd[i] is Bullet) {
						objects.Add(unitToAdd[i]);
						bullets.Add(unitToAdd[i] as Bullet);
						unitToAdd.Remove(unitToAdd[i]);
					}
				}

			// BackGround
			double elapsed = game.dt;
			scrollingBackGround.Update(1/*elapsed * 100*/);
			frontalScrollingBackGround.Update(2);

			//TimeCoef
			UpdateTimeCoef();

			// Update
			foreach (Object obj in activeObjects) obj.Update();
			//foreach (Weapon weapon in activeWeapons) weapon.Update();
			//Reverse用のLogをとる←色々Update終わったあとにLog取らせることに
			//*注意！極力UpdateReverseでのreverse.Updateと同タイミングに置くこと！！
			reverse.UpdateLog();

			// CollisionDetection(Terrain、Weapon等)
			Collide();

			damageControl.Update2();                //sword.DamageUpdate();　 // WeaponとEnemyのダメージ判定の調整
			// スクロール時の座標変換(画面上のすべてのオブジェクトについて)
			ScrollUpdate();

			// BGM
			PlayBGM();

			// maxCombo
			
			//TASpower
			player.TASpower += -1;

			gameTimeTAS++;
			
		}
		private void UpdateUICalculate()
		{
			for (int i = 0; i < inComboObjects.Count; i++)    // 列要素を比較
				if (gameStatus.maxComboCount < (inComboObjects[i] as Enemy).comboCount) {
					gameStatus.maxComboCount = (inComboObjects[i] as Enemy).comboCount;
					gameStatus.comboCountVisibleTime = 0;
				}
			//if (inComboObjects.Count == 0) gameStatus.maxComboCount = 0;

			if (gameStatus.comboCountVisibleTime > gameStatus.maxComboCountVisibleTime) {
				gameStatus.maxComboCount = 0;
			}

			gameStatus.comboCountVisibleTime++;
		}
		private void UpdateTimeCoef()
		{
			// Update
			foreach (Terrain terrain in activeDynamicTerrains) terrain.UpdateTimeCoef();
			for (int i = 0; i < characters.Count; i++)//foreach(Character character in characters) {
				characters[i].UpdateTimeCoef();
			foreach (Bullet bullet in bullets)
				bullet.UpdateTimeCoef();
			foreach (Weapon weapon in weapons)
				if (weapon.isAlive)
					weapon.UpdateTimeCoef();
		}
		/// <summary>
		/// 当たり判定処理
		/// </summary>
		private void Collide()
		{
			#region Initialize
			activeWeapons.Clear();
			adObjects.Clear();
			damagedCharacters.Clear();
			damagedObjects.Clear();
			attackedObjects.Clear();
			foreach (Weapon weapon in weapons) {
				if (weapon.isBeingUsed || (weapon is Turret && (weapon as Turret).isVisible))
					activeWeapons.Add(weapon);
			}
			foreach (Object obj in objects) {
				obj.firstTimeInAFrame = true;
				obj.isFirstTimevsCB = true;
			}
			/*foreach (Bullet bul in bullets)
				bul.isHit = false;*/
			#endregion
			#region staticTerrain/Character, dynamicTerrain
			foreach (Terrain terrain in activeStaticTerrains) {
				if (terrain.isOn && terrain.isUnder && terrain.isRight && terrain.isLeft || !terrain.isAlive) continue;// 完全に埋もれてるのは除外
				//if (terrain.user != null && terrain.isBeingUsed) continue;
				if (terrain.user != null && !terrain.isBeingUsed) continue;
				if (terrain is MapObjects) continue;

				foreach (Terrain terrainD in activeDynamicTerrains1)                           // blockでもuserがいればactiveDynamicTsに入れるようにしよう.
					//if (!(terrainD is SnowBall)) 
						terrain.IsHit(terrainD);
					//else terrain.IsHit((terrainD as SnowBall).block);

				foreach (Character character in activeCharacters)
					if (!(character is FlyingOutEnemy3)) {
						if (terrain is Slope) (terrain as Slope).IsHitLite(character);
						//else if (terrain is SnowBall) CollisionDetection.RectangleCross(terrain, character, terrain.degree, character.degree);
						else {
							if (terrain is Block && !(terrain is DamageBlock)) (terrain as Block).IsHitDetailed(character, 3);//3);
							else terrain.IsHit(character);
							//terrain.IsHit(character);
						}
					}
				foreach (Weapon obstacle in activeWeapons)
					if (obstacle.isBeingUsed)
						terrain.IsHit2(obstacle);
				/*foreach (Bullet bul in activeBullets)
					if (bul.isShot)
						//CollisionDetection.RectangleCross(terrain, bul, terrain.degree, bul.degree);
						terrain.IsHit(bul);*/
			}
			#endregion
			#region dynamicTerrain/Character
			foreach (Terrain terrain in activeDynamicTerrains) {
				if (!terrain.isAlive) continue;
				if (terrain.user != null && !terrain.isBeingUsed) continue;
				//if (terrain is MapObjects) continue;

				foreach (Character character in activeCharacters) {
					if (!character.isAlive) continue;

					//if (terrain.user != null && terrain.user == character || terrain.user.user != null && terrain.user.user == chareacter) continue; 
					// 何というハードコーディング（mapObejects/Fuujin）
					if (terrain.user != null && terrain.user == character || terrain.user != null && terrain.user.user != null && terrain.user.user == character) continue;
					if (!(character is Player && game.inDebugMode)) {
						terrain.IsHit(character);
					}

					// 11/10/1 Player.isDamagedだがdamageFromAtackingがtrueで到達バグ
					if (character.isDamaged && !character.damageFromAttacking && !character.damageFromTouching) {
						// Weaponから攻撃を受けてisDamaged=trueのときもここに到達してしまうのが問題→dFAと共にdObjectをObjectに入れる？ // 条件追加したら、雪玉と接触してる敵がどこでも剣の攻撃を受けるようになって改悪
						character.damageFromTouching = true;
						adObjects.Add(new Object2(terrain.user != null ? terrain.user as Object : terrain, character));
					}
				}
			}
			#endregion
			#region Weapon/Character, Bullet, Weapon
			foreach (Weapon weapon in activeWeapons) {
				if (!weapon.isAlive) continue;
				foreach (Character character in activeCharacters) {
					if (!character.isAlive) continue;
					if ((weapon.user != character))
						if (weapon is Sword) {
							CollisionDetection.RectangleCrossDetailed(weapon, character, weapon.degree, character.degree, 16);
						} else {
							CollisionDetection.RectangleCross(weapon, character, weapon.degree, character.degree);
						}
					// Fuujin.damageFromAttacking == trueで剣での攻撃が"damageControlに感知されないまま"isDamaged状態が続いて毎フレHPが減るｗ
					if (character.isDamaged && !character.damageFromAttacking && !character.damageFromTouching && !character.damageFromThrusting) {
						// character==weapon.userなら無視 10/12/27:別の場所でisDamaged=trueになったため当たり判定せずにListに追加しちゃってる状況らしい.(雪玉に当たった)
						if (character == weapon.user) continue;
						else {
							if (weapon is Sword && weapon.user is Player) player.AirSlashReflection(Player.defReflectSpeed);//8, -8
							
							attackedObjects.Add(weapon.user != null ? weapon.user as Object : weapon);
							damagedObjects.Add(character);
							if (!player.isThrusting) {
								character.damageFromAttacking = true;
								character.damageFromThrusting = false;
							} else {
								character.damageFromAttacking = false;
								character.damageFromThrusting = true;
							}
							adObjects.Add(new Object2(weapon.user != null ? weapon.user as Object : weapon, character));
						}
					}
				}
				// activeDynamicTerrainとも.とりあえずBallだけ 2/20
				/*foreach (Terrain terrain in activeDynamicTerrains)
					if (terrain is SnowBall) {
						if (!terrain.isAlive || terrain.user == weapon || weapon is Obstacle) continue;
						CollisionDetection.RectangleCross(weapon, (terrain as SnowBall).block, weapon.degree, terrain.degree);
						if ((terrain as SnowBall).block.isDamaged) terrain.isDamaged = true;
						if (terrain.isDamaged) adObjects.Add(new Object2(weapon, terrain));
						terrain.damageFromAttacking = true;
					}*/

				foreach (Weapon weaponD in activeWeapons) {
					if ((weaponD.canBeDestroyed && weapon != weaponD)
						|| (weapon != weaponD && (weapon is Sword && weaponD is Sword))) {
						if (weapon is Sword && weaponD is Sword) { }
						CollisionDetection.RectangleCross(weapon, weaponD, weapon.degree, weaponD.degree);
					}
					//else if (weapon != weaponD && (weapon is Sword && weaponD is Sword)) {
					if (weaponD.isDamaged && !(weaponD is Sword)) {
						damagedObjects.Add(weaponD);
						weaponD.damageFromTouching = true;
						if (weapon is Sword && weaponD is Turret) { }
						adObjects.Add(new Object2(weapon, weaponD));
					}
					if ((weaponD.isDamaged || weapon.isDamaged) && (weaponD is Sword && weapon is Sword) && weapon != weaponD) {
						//Cue cue = game.soundBank.GetCue("katana");
						//if (gameTime % 5 == 0) cue.Play(SoundControl.volumeAll, 0f, 0f);
						/*if(!hasPlayedSoundEffect) { 
							cue.Play(SoundControl.volumeAll, 0f, 0f);
							hasPlayedSoundEffect = true;
						}*/
						if (weapon.user is Player) {
							(weapon.user as Player).SwordReflection(weaponD.user);
							(weaponD.user as Rival).SwordReflection(weapon.user);
						} else {
							(weapon.user as Rival).SwordReflection(weaponD.user);
							(weaponD.user as Player).SwordReflection(weapon.user);
						}
					}
				}

				// Bulletを跳ね返せるように。
				if (weapon is Sword) {
					foreach (Bullet bullet in activeBullets)// activeBulletsだった！これが原因だな。道理で全く判定されなかったわけだ
						if (!(bullet is Beam) && !(bullet is Thunder) && bullet.isShot && weapon.isBeingUsed) {
							CollisionDetection.RectangleCrossDetailed(weapon, bullet, weapon.degree, bullet.degree, 16);
							if (bullet.isDamaged) {
								adObjects.Add(new Object2(weapon.user != null ? weapon.user as Object : weapon, bullet));
								bullet.damageFromTouching = true;
								bullet.isCollideWith(weapon);
							}
						}
				}
			}
			#endregion
			#region Bullet/Chracter
			foreach (Bullet bullet in activeBullets) {
				if (bullet.isShot) {//CollisionDetection.RectangleDetectionLite(bullet ,player, 0);   // 引数の順番に注意！:targetObject.isDamaged = trueの仕様 のせたくないならLiteを用いる
					foreach (Character character in activeCharacters) {
						//if (bullet.isHostile &&bullet.turret.user != character || !bullet.isHostile) {

						// 11/12/15 左の条件が要らなさそうだったのでアウトしてみた
						if (/*(bullet.turret.user == character && !bullet.isHostile || bullet.isHostile) && */(bullet.isHostile && character is Player && !game.inDebugMode || !bullet.isHostile && character is Enemy)) {
							CollisionDetection.RectangleCross(bullet, character, bullet.degree, character.degree);// メソッド内でtargetObject.isDmaged=trueにされる
						} /*else if (!bullet.isHostile && character is Enemy) {
								CollisionDetection.RectangleCross(bullet, character, bullet.degree, character.degree);
							}*/
						if (!bullet.isHostile && character is Fuujin && character.isDamaged) { }
						if (character.isDamaged && !character.damageFromAttacking) {// 条件でdFAを入れてなかったからdamagesに追加されまくってたorz
							if (character is Fuujin) { }// わかった、dFAは攻撃Object側のisAttacking側に依存している。Bulletにはそれの管理機構がないせいかも←Bulletの場合はisShotで管理してた
							//bullet.isCollideWith(character);
							adObjects.Add(new Object2(bullet, character));
							character.damageFromAttacking = true;
						}
					}
				}
				/*if (player.isDamaged) {
					adObjects.Add(new Object2(bullet, player));
					player.damageFromTouching = true;           // FromAttackingだとFuujinの１つの攻撃パターン中１回しか当たらないから...
					break;  // しないと当たってるのにfalseになる
				}*/

			}
			if (adObjects.Count > 0) { }
			if (!game.inDebugMode) {
				foreach (Character character in activeCharacters) {
					if (!(character is Player) && character is Enemy && character.isAlive && !(character as Enemy).isWinced) {
						if (!(character is Boss)) {
							CollisionDetection.RectangleDetection(character, player, 0);
						} else {// Bossは判定範囲をbindで指定する
							CollisionDetection.RectangleDetection((character as Boss).bind, player, 0);//1
						}
						if (player.isDamaged) {
							adObjects.Add(new Object2(character, player));
							player.damageFromTouching = true;
							break;
						}
					}
				}
			}
			#endregion
			#region Etc
			// 上端
			if (player.position.Y < 32) player.position.Y = 32;
			// スクロール挟まり判定
			/*if (isScrolled && (player.position.X + 2 < camera.position.X && 
					activeStaticTerrains.Any((x) => !(x is Slope) && x.position.X < player.position.X + player.width - 5)
						&& activeStaticTerrains.Any((y) => !(y is Slope) && y.position.X < player.position.X + player.width - 5 && player.position.Y + player.height > y.position.Y + adj && player.position.Y + adj < y.position.Y + y.height))) {

				player.position = new Vector2(camera.position.X + 320, 100);
				adObjects.Add(new Object2(camera, player));
				player.isDamaged = true;
				player.damageFromTouching = true;
				player.HP += -1;
			}*/
			// 左側が露出している地形だけ見るように改良
			if ((isScrolled || inBossBattle) && player.position.X <= camera.position.X) {
				player.position.X = camera.position.X;									// 位置を前に変更
				player.speed.X = scrollSpeed;											// 押す
				player.scrollPush = true;
			} else player.scrollPush = false;

			if (isScrolled && (player.position.X <= camera.position.X && player.isHitLeftSide/*&&
				   activeStaticTerrains.Any((x) => !(x is Slope) && !x.isRightSlope && !x.isLeftSlope && x.isLeft && !x.isRight
					   && x.position.X < player.position.X + player.width && player.position.Y + player.height > x.position.Y + adj && player.position.Y + adj < x.position.Y + x.height))*/
				)) {
				player.position = new Vector2(camera.position.X + 320, 100);
				adObjects.Add(new Object2(camera, player));
				player.isDamaged = true;
				player.damageFromTouching = true;
				player.HP += -1;
			}
			//if (isScrolled && boss.position.X < camera.position.X) boss.position.X = camera.position.X;
			//if (inBossBattle && boss.position.X < camera.position.X) boss.position.X = camera.position.X;

			/// Game Over処理
			if (!toGameOver && (player.HP < 1 || player.position.Y > 540)) {// 600 546
				//camera.isScrollingToPlayer = true;// スクロールモードの差を取り除く
				//playerDeathDrawPos = player.drawPos;
				//player.position += Player.screenPosition;

				//camera.position.X -= Player.screenPosition.X; //player.drawPos == (-8, 199.8); 2430

				player.isAlive = false;
				toGameOver = true;
				hasEffectedPlayerDeath = false;
			}
			if (toGameOver && hasEffectedPlayerDeath) {
				SoundControl.Stop();
				PushScene(new GameOver(this));
			}
			if (boss.position.Y > 600) boss.isAlive = false;// characterが自分で殺すようにする...?
			if (hasEffectedBossExplosion && game.stageNum != 0 && !boss.isAlive) {
				SoundControl.Stop();
				PushScene(new ClearScene(this));
			}
			#endregion
		}

		// Sub Methods
		/// <summary>
		/// 静的な地形で、隣接する地形がある場合、その方向の当たり判定をしないフラグを立てる
		/// わかりづらい＼(^o^)／
		/// </summary>
		protected void SetTerrainDirection()
		{
			// 重いのでLoad時1回だけ実行　
			// 処理をする方向を決定、動かない地形が敷き詰められていても外郭だけの判定に狭められる(重なっているときは無理)
			for (int i = 0; i < staticTerrains.Count; i++) {
				if (staticTerrains[i] is Slope) {
					for (int j = 0; j < staticTerrains.Count; j++) {// 自分自身の場合はスキップ 読み込み段階で地形の位置は被らない筈
						// 向き合わさった斜面対策
						/*if (staticTerrains[i] is Slope && !(staticTerrains[i] as Slope).isUp &&
							staticTerrains[i].position.X + staticTerrains[i].width == staticTerrains[i].position.X
							&& staticTerrains[i].position.Y == staticTerrains[i].position.Y)
							(staticTerrains[i] as Slope).isLeftSlope = true;*/
						if (staticTerrains[i].position == staticTerrains[j].position) { // trueとかになっててもどのみち同じいちにいるんだから気にしない
							continue;
						} else {
							// 上下左右にいても全く問題ない　そこは判定してくれないと困る、SLopeの場合は斜め

							// ブロックとの継ぎ目対策
							/*if(staticTerrains[i] is Block && staticTerrains[i] is Slope && !(staticTerrains[i] as Slope).isUp
								&& staticTerrains[i].position.X + staticTerrains[i].width == staticTerrains[i].position.X 
								&& staticTerrains[i].position.Y == staticTerrains[i].position.Y) {
								(staticTerrains[i] as Slope).isRightBlock = true;
							}*/

							/// <summary>
							/// ↓Slopeの判定をIsHitで統一しようとしてたときの処理IsHitLiteでやるのならば必要ない
							/// </summary>
							if ((staticTerrains[i] as Slope).isUp) {
								if (staticTerrains[i].position.Y - staticTerrains[i].height == staticTerrains[j].position.Y && staticTerrains[i].position.X + staticTerrains[j].width == staticTerrains[j].position.X)
									(staticTerrains[i] as Slope).isLowerLeft = true; //右方向の当たり判定をしない
							} else
								if (staticTerrains[i].position.Y - staticTerrains[i].height == staticTerrains[j].position.Y && staticTerrains[i].position.X - staticTerrains[j].width == staticTerrains[j].position.X)
									(staticTerrains[i] as Slope).isLowerRight = true; // 左方向の判定をしない
						}
					}
				} else {// sT[i]がSlope以外
					for (int j = 0; j < staticTerrains.Count; j++) {// 自分自身の場合はスキップ 読み込み段階で地形の位置は被らない筈
						if (staticTerrains[i].position == staticTerrains[j].position) { // ここがmiss.下でtrueにしても全部falseにしてしまう
							continue;// ただとばすだけ
						} else {
							if (staticTerrains[i].position.X == staticTerrains[j].position.X && staticTerrains[i].position.Y - staticTerrains[j].height == staticTerrains[j].position.Y)
								staticTerrains[i].isUnder = true;	// 上方向の当たり判定をしない(Playerが下に動いているときの判定をしない)
							if (staticTerrains[i].position.X == staticTerrains[j].position.X && staticTerrains[i].position.Y + staticTerrains[j].height == staticTerrains[j].position.Y)
								staticTerrains[i].isOn = true;		// 下方向の当たり判定をしない
							if (staticTerrains[i].position.Y == staticTerrains[j].position.Y && staticTerrains[i].position.X + staticTerrains[i].width == staticTerrains[j].position.X)
								staticTerrains[i].isLeft = true;	// 右方向の当たり判定をしない
							if (staticTerrains[i].position.Y == staticTerrains[j].position.Y && staticTerrains[i].position.X - staticTerrains[j].width == staticTerrains[j].position.X) {
								staticTerrains[i].isRight = true;	// 左方向の当たり判定をしない
								//if (staticTerrains[i] is Water && staticTerrains[j] is Water) (staticTerrains[i] as Water).neighborWater = staticTerrains[j] as Water; waterはsTじゃなかった....
							}
							// 草
							if (staticTerrains[j] is Slope && staticTerrains[i].position.Y == staticTerrains[j].position.Y + staticTerrains[j].height
								&& staticTerrains[i].position.X >= staticTerrains[j].position.X && staticTerrains[i].position.X + staticTerrains[i].width <= staticTerrains[j].position.X + staticTerrains[j].width) {
								(staticTerrains[i] as Block).isUnderSlope = true;
							}

							// Block等がSlopeとの継ぎ目に位置する場合、端の判定を変えるためにフラグを立てる
							// 天井に位置していても全く同じ処理でおｋ.
							if (staticTerrains[j] is Slope && (staticTerrains[j] as Slope).isUp) {// 上り坂：左にあれば
								if (staticTerrains[i].position.X - staticTerrains[j].width == staticTerrains[j].position.X && staticTerrains[i].position.Y == staticTerrains[j].position.Y)
									staticTerrains[i].isRightSlope = true;// 斜面の→
							}
							if (staticTerrains[j] is Slope && !(staticTerrains[j] as Slope).isUp) {// 下り坂：右にあれば
								if (staticTerrains[i].position.X + staticTerrains[i].width == staticTerrains[j].position.X && staticTerrains[i].position.Y == staticTerrains[j].position.Y)
									staticTerrains[i].isLeftSlope = true;// 斜面の左
							}
						}
					}
				}
			}
			/*foreach(Terrain terrain in staticTerrains) {
				if(terrain.isUnder && terrain.isOn && terrain.isLeft && terrain.isRight) staticTerrains.Remove(terrain);// まだこれをやるほど重くはない
			}*/
		}
		protected void SetWaterDirection()
		{
			foreach (Terrain t0 in dynamicTerrains) {
				foreach (Terrain t1 in dynamicTerrains) {
					if (t0 is Water && t1 is Water) {
						if (t0.position.Y == t0.position.Y) {
							if (t0.position.X - t0.width == t1.position.X) {
								(t0 as Water).neighborWater = t1 as Water;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 分裂する敵などが死んだら、その場(Update()を実行中)でcharacters二追加するとforeachが使えないので
		/// queに追加しておいて次回Updateの始めに追加、という形にする。
		/// </summary>
		private void AddUnits()
		{
			if (unitToAdd.Count > 0) {
				for (int i = unitToAdd.Count - 1; i >= 0; i--) {
					//while (unitToAdd.Count > 0) {
					if (unitToAdd[i] is Character) {
						objects.Add(unitToAdd[i]);
						characters.Add(unitToAdd[i] as Character);
						unitToAdd.Remove(unitToAdd[i]);
					} else if (unitToAdd[i] is Bullet) {
						objects.Add(unitToAdd[i]);
						bullets.Add(unitToAdd[i] as Bullet);
						unitToAdd.Remove(unitToAdd[i]);
					}
				}
			}
		}
		/// <summary>
		/// objectsの通常/強制スクロールのスクリーン座標をcameraに求めさせる
		/// </summary>
		protected virtual void ScrollUpdate()
		{
			activeCharacters.Clear();
			activeTerrains.Clear();
			activeDynamicTerrains.Clear();
			activeDynamicTerrains1.Clear();
			activeDynamicTerrains2.Clear();
			activeStaticTerrains.Clear();
			activeObjects.Clear();
			activeBullets.Clear();

			if (isScrolled) {
				if (isAccelerated)
					camera.position.X += scrollSpeed * 4.0f;
				else
					camera.position.X += scrollSpeed;//.60f
			}
			/*foreach (Block b in cameraWall) {
				b.position.X = camera.position.X - 32;
				b.speed = camera.speed;
			}*/

			foreach (Object object1 in objects) {
				if (object1 is Player) { }
				camera.ScrollUpdate(object1);
			}
			camera.ScrollUpdate(scrollingBackGround);
			camera.ScrollUpdate(frontalScrollingBackGround);
		}
		public virtual void ScrollUpdateReverse(float x)
		{
			activeCharacters.Clear();
			activeTerrains.Clear();
			activeDynamicTerrains.Clear();
			activeDynamicTerrains1.Clear();
			activeDynamicTerrains2.Clear();
			activeStaticTerrains.Clear();
			activeObjects.Clear();
			activeBullets.Clear();

			foreach (Object object1 in objects)
				camera.ScrollUpdate(object1);
			if (isScrolled)
				camera.position.X += scrollSpeed;//.60f
		}
		/// <summary>
		/// 与えられたX座標からStageの地形表面の高さ(position.Y)を返す。
		/// </summary>
		/// <param name="positionX">X座標(スクリーン座標ではなく実際の座標)</param>
		/// <returns></returns>
		public float CheckGroundHeight(float positionX)
		{
			float vectorY = 0;
			/*foreach (Terrain terrain in activeStaticTerrains)// 動かない地形から探す.場所によってdynamicが明らかに地形表面ということもあるが知ったことではない
				if (terrain.isOn && !terrain.isUnder && positionX > terrain.position.X && positionX < terrain.position.X + terrain.width) {// 一番上で、かつ範囲内にあれば:
					vectorY = terrain.position.Y;
					break;
				}*/
			foreach (Object obj in activeObjects) {
				if (obj is Terrain) {
					if (((obj as Terrain).isOn && !(obj as Terrain).isUnder && positionX > obj.position.X && positionX < obj.position.X + obj.width)
						|| (!(obj as Terrain).isOn && !(obj as Terrain).isUnder && positionX > obj.position.X && positionX < obj.position.X + obj.width)) {// 一番上で、かつ範囲内にあれば:
						vectorY = obj.position.Y;
						break;
					}
				}
			}

			return vectorY;
		}

		// Sound, Drawing
		/// <summary>
		/// BGMの再生管理
		/// </summary>
		protected virtual void PlayBGM()
		{
			if (inBossBattle && !BGMchanged) {
				SoundControl.Stop();
				if (game.stageNum == 3) {
					//BGM = game.soundBank.GetCue("boss");
					SoundControl.IniMusic("Audio\\BGM\\hard_last");
				} else {
					//BGM = game.soundBank.GetCue("boss_nomal");
					SoundControl.IniMusic("Audio\\BGM\\boss_nomal");
				} SoundControl.musicInstance.IsLooped = true;

				if (!game.isMuted) SoundControl.Play();
				BGMchanged = true;
			}
			/*else if(BGM != null && (BGM.IsStopped || BGM.IsPaused) && inBossBattle && !game.isClear && !game.isOvered) {
				if (!game.isMuted) {
					if (BGM.IsStopped) {
					}
					else if ( BGM.IsPrepared) BGM.Play(SoundControl.volumeAll, 0f, 0f);//BGM.IsPaused ||
					else if (BGM.IsPaused) BGM.Resume();
				}
			}*/
		}
		/// <summary>
		/// 静的な地形をBackGroundクラスの大きなテクスチャにまとめて描画する。
		/// </summary>
		/// <see cref="http://memeplex.blog.shinobi.jp/Entry/54/"/>
		/// <seealso cref="http://msdn.microsoft.com/ja-jp/library/bb975265.aspx"/>
		/// <param name="gameTime"></param>
		protected virtual void DrawBackGround()
		{
			// GraphicDeviceのセット
			/*foreach (BackGround bg in backGrounds) {
				game.GraphicsDevice.SetRenderTarget(0, bg.renderTarget);
				// Cache the current depth buffer
				DepthStencilBuffer old = game.GraphicsDevice.DepthStencilBuffer;
				// Set our custom depth buffer
				game.GraphicsDevice.DepthStencilBuffer = bg.depthStencilBuffer;
				game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.TransparentWhite);					// 透明

				foreach (Terrain terrain in staticTerrains) {
					terrain.drawPos.Y = terrain.position.Y;
					terrain.drawPos.X = terrain.position.X - player.position.X + Player.screenPosition.X;

					if (terrain.position.X >= bg.position.X && terrain.position.X < bg.position.X + bg.width + 32) {
						spriteBatch.Begin();
						terrain.position -= bg.position;
						terrain.Draw(spriteBatch);
						terrain.position += bg.position;
						spriteBatch.End();
					}
				}

				game.GraphicsDevice.SetRenderTarget(0, null); // nullをセットして、元に戻す。
				game.GraphicsDevice.DepthStencilBuffer = old;
				bg.texture = bg.renderTarget.GetTexture();*/

			// xna4.0版
			foreach (BackGround bg in backGrounds) {
				game.GraphicsDevice.SetRenderTarget(bg.renderTarget);
				/*// Cache the current depth buffer
				DepthStencilBuffer old = game.GraphicsDevice.DepthStencilBuffer;
				// Set our custom depth buffer
				game.GraphicsDevice.DepthStencilBuffer = bg.depthStencilBuffer;*/
				game.GraphicsDevice.Clear(Color.Transparent);				// 透明

				foreach (Terrain terrain in staticTerrains) {
					terrain.drawPos.Y = terrain.position.Y;
					terrain.drawPos.X = terrain.position.X - player.position.X + Player.screenPosition.X;

					if (terrain.position.X >= bg.position.X && terrain.position.X < bg.position.X + bg.width + 32) {
						spriteBatch.Begin();
						terrain.position -= bg.position;
						terrain.Draw(spriteBatch);
						terrain.position += bg.position;
						spriteBatch.End();
					}
				}

				/*game.GraphicsDevice.SetRenderTarget(0, null); // nullをセットして、元に戻す。
				game.GraphicsDevice.DepthStencilBuffer = old;*/
				bg.texture = bg.renderTarget;//GetTexture();	.GraphicsDevice.GetRenderTargets();
			}

			game.GraphicsDevice.SetRenderTarget(null);
			/*using (FileStream savedstream = new FileStream("bgtest0.png", FileMode.Create)) {// 描画されてはいた。描画のところか？
				backGrounds[0].texture.SaveAsPng(savedstream, backGrounds[0].texture.Width, backGrounds[0].texture.Height);
			}*/
			//game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.White);
			// 最後にウィンドゥに描画
			//game.GraphicsDevice.re
			//"一度レンダー ターゲットへの描画処理が終了すれば、ResolveRenderTarget を呼ぶ必要はなくなります。"
			//http://msdn.microsoft.com/ja-jp/xna/cc676829.aspx
		}
		/// <summary>
		/// クリア時にゲーム内情報を描画。
		/// </summary>
		/// <param name="spriteBatch"></param>
		protected virtual void DrawGameStatus(SpriteBatch spriteBatch)
		{
			//gameStatus.second = (int)(gameTimeNormal % 60);
			//gameStatus.minute = (int)(gameStatus.second / 60.0);

			if (game.visibleScore) {
				spriteBatch.DrawString(game.pumpDemi, "GameTime(frame):" + this.gameTimeNormal.ToString(), new Vector2(0, 32), Color.Orange);
				//spriteBatch.DrawString(game.pumpDemi, "GameTime(frame):" + minute.ToString() + ":" + sec.ToString(), new Vector2(0, 64), Color.Orange);
				spriteBatch.DrawString(game.pumpDemi, "TAS Time(frame):" + this.gameTimeTAS.ToString(), new Vector2(0, 96), Color.Orange);
				spriteBatch.DrawString(game.pumpDemi, "Score:" + game.score.ToString(), new Vector2(0, 128), Color.Orange);
			} else {
				spriteBatch.DrawString(game.pumpDemi, "Time:"
				+ ((int)(gameStatus.time / 60.0)).ToString() + ":" + ((int)(gameStatus.time % 60)).ToString(), new Vector2(0, 32), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

				//if (gameStatus.comboCountVisibleTime < gameStatus.maxComboCountVisibleTime) 
					spriteBatch.DrawString(game.pumpDemi, "MAX:" + gameStatus.maxComboCount.ToString() + "HIT", new Vector2(500, 32), Color.Orange, 0, Vector2.Zero, new Vector2(.6f, .6f), SpriteEffects.None, 0f);
			}
		}
		/// <summary>
		/// Stage上の全てのObjectに関する描画処理を行う。
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch)
		{
			// BackGround
			scrollingBackGround.Draw(spriteBatch);
			frontalScrollingBackGround.Draw(spriteBatch);

			// Objects
			// 地中ユニット
			foreach (Character character in activeCharacters) {
				if ((character is FlyingOutEnemy) && !(character as FlyingOutEnemy).hasFlownOut)
					character.Draw(spriteBatch);
			}
			foreach (BackGround bg in backGrounds) bg.Draw(spriteBatch);
			foreach (Terrain terrain in activeDynamicTerrains) terrain.Draw(spriteBatch);
			foreach (Character character in activeCharacters) {
				if (!(character is FlyingOutEnemy) || ((character is FlyingOutEnemy) && (character as FlyingOutEnemy).hasFlownOut))
					character.Draw(spriteBatch);// 地中以外
			}
			foreach (Weapon weapon in weapons) weapon.Draw(spriteBatch);
			foreach (Bullet bullet in bullets) bullet.Draw(spriteBatch);

			// UI
			userInterface.Draw();
			DrawGameStatus(spriteBatch);

			// Effect
			effectControl.Update();

			// TASEffect
			if (reverse.isReversed)
				scrollingTASEffect.Draw(spriteBatch);
			if (slowmotion.isSlow)
				scrollingTASEffect.Draw(spriteBatch);
			if (isAccelerated)
				scrollingTASEffect.Draw(spriteBatch);

			// Scene Effects
			if (!hasEffectedBeginning && !game.hasReachedCheckPoint) {
				effectStage.DrawStageScreenEffect(spriteBatch, game.stageNum, 120);
			}
			if (!hasEffectedWarning && toBossScene) {
				//if (BGM != null && !BGM.IsPaused && !BGM.IsStopped) BGM.Pause();
				effectBoss.DrawBossScreenEffect(spriteBatch, 180);
				//inBossBattle = true;
			}
			if (!hasEffectedBossExplosion && game.stageNum != 0 && !boss.isAlive) {
				effectBoss.DrawBossDeathEffect(spriteBatch, boss);
			}
			if (!hasEffectedPlayerDeath && toGameOver) {
				//camera.position//2965
				effectPlayerDeath.DrawPlayerDeathEffect(spriteBatch, 4
					, new Vector2(player.position.X + player.width / 2, player.position.Y + player.height / 2), 360 / (float)4, 2, 1, 1);//Effect.deathEffectNum
			}//player.drawPos == (200, y) やっぱりずれてた
			
			// debug
			debug.Draw(spriteBatch);
			//foreach (Terrain t in activeStaticTerrains) if (t is Block && (t as Block).IsInterrupt(player)) spriteBatch.DrawString(game.pumpDemi, "this is it!!", t.drawPos, Color.White);
		}
	}
}