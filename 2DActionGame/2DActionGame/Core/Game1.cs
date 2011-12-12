// Welcome to TASRUSH source code!

// 注意：意味不明なコメントや試行錯誤していた時に残した箇所、全く使っていない箇所、などがあります。
// さらに、読みづらい部分や稚拙かつ非効率的な処理をしている所もあるでしょう。が、幾許かはこのコードを読む人の参考になると思います。ゆっくり読んでいってね！
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace _2DActionGame
{
	public struct RankingStatus
	{
		public int rank;
		public double score;
		public string name;

		public RankingStatus(int rank, double score, string name)
		{
			this.rank = rank;
			this.score = score;
			this.name = name;
		}
	}
	/// <summary>
    /// This is the main movementType for our game. Yukkuri mite ittene!
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// ウィンドゥサイズ
        /// </summary>
		public static readonly int Width = 640, Height = 480;
		public static readonly int maxStageNum = 3;

		public WinControlManager winControlManager { get; set; }
		public Random random { get; private set; }
		public double dt { get; private set; }
        public GraphicsDeviceManager graphics { get; set; }
		public SpriteBatch spriteBatch { get; private set; }
        private DebugMessage dm;

        // Scene
		public Stack<Scene> scenes = new Stack<Scene>();
		private Scene currentScene;

		// Font
		public SpriteFont Arial { get; private set; }
		public SpriteFont Arial2 { get; private set; }
		public SpriteFont pumpDemi { get; private set; }
        
        // Game
		private const int rankingNodeNum = 5;
		public RankingStatus[] scores = new RankingStatus[rankingNodeNum];
		public RankingStatus[] dummyScore = { new RankingStatus(1, 100, "nanashi")
			, new RankingStatus(2, 100, "nanashi"), new RankingStatus(3, 100, "nanashi"), new RankingStatus(4, 100, "nanashi"), new RankingStatus(5, 100, "nanashi") };
        public double score { get; set; }
		public string playerName { get; set; }
		public float wholeVolume { get; set; }
        public int avilityNum { get; set; }
		public int stageNum { get; set; }

		public bool isHighLvl { get; set; }
		public bool isMuted { get; set; }
		public bool noEnemy { get; set; }
		public bool inDebugMode { get; set; }
		public bool visibleSword { get; set; }
		public bool visibleScore { get; set; }
		public bool hasReachedCheckPoint { get; set; }

		public void PushScene(Scene scene)
		{
			scenes.Push(scene);//this.Window.
		}
		private void graphics_DeviceResetting(object sender, EventArgs e)
		{
			// ウィンドウのフォーカスが失われるバグ(?)対策
			System.Windows.Forms.Form form = (System.Windows.Forms.Form) System.Windows.Forms.Form.FromHandle(Window.Handle);
			form.BringToFront();
		}
		public void LoadRanking(string fileName, bool isTest, string fileNameT)
		{
			StreamReader sr = new StreamReader(fileNameT);
			string original;
			string[] tmp;
			string[,] tmp2;

			if (isTest) {
				original = sr.ReadToEnd();
				// 暗号化の実験
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(original));
				Encryption.EncryptionData(ms, fileName);
			}// １行追加してテストしてみたところ最後の行の途中から欠ける仕様らしい？

			// 復号化
			MemoryStream msd = new MemoryStream();
			Encryption.DecryptionFile(fileName, msd);//("test.txt", msd);
			byte[] bs = msd.ToArray();
			original = Encoding.UTF8.GetString(bs);		// UTF8じゃないと文字化けする
			//original = sr.ReadToEnd();
			original = original.TrimEnd(new char[] { '\r', '\n' });

			tmp = original.Replace("\r\n", "\n").Split('\n');// 0が無視される...
			tmp2 = new string[tmp.Length, 3];
			for (int i = 0; i < tmp.Length-1; i++) {// 最後の行は削られることを想定してlength-1
				try {
					string[] t = tmp[i].Split(' ');
					tmp2[i, 0] = t[0];
					tmp2[i, 1] = t[1];
					tmp2[i, 2] = t[2];
				} catch {
					throw new Exception("failed ranking decoding.");// 5番目だけ"5 na"で終わってる件ｗｗ buffer[2048]にしたらnanasまでｋｔ...?←やっぱ関係なかった
					// そもそもbs.lengthの時点で明らかに足りてない
				}
			}
			for (int i = 0; i < tmp.Length-1; i++) {
				scores[i].rank = Int32.Parse(tmp2[i, 0]);
				scores[i].score = Double.Parse(tmp2[i, 2]);
				scores[i].name = tmp2[i, 1];
			}
		}
		public void EvaluateScore(string fileName)
		{
			int ranking = 999;//rankingNodeNum;
			string output = "";
			//StreamWriter sw = new StreamWriter("Ranking_test.txt");

			// ランキング更新
			for (int i = 0; i < scores.Length; i++) {
				if (score > scores[i].score) {
					ranking = i + 1;
					break;
				} else {
					continue;
				}
			}
			/*for (int i = scores.Length - 1; i >= 0; i--) {
				if (scores[i].score < score) {
					continue;
				} else {
					ranking = scores.Length - i;
					break;
				}
			}*/

			if (ranking < scores.Length/*>= rankingNodeNum*/) {
				scores[ranking-1].score = score;
				if (playerName != "") scores[ranking-1].name = playerName;
				else scores[ranking].name = "nanashi";
			}

			// 暗号化＆ファイル書き込み
			for (int i = 0; i < scores.Length; i++) {
				output += scores[i].rank.ToString() + " " + scores[i].name.ToString() + " " + scores[i].score + "\r\n";//(i+1).ToString() 
			}
			//output = output.TrimEnd(new char[] { '\r', '\n' });
			output += "6 dummy 100\r\n";
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(output));
			Encryption.EncryptionData(ms, fileName);
			/*sw.Write(output);
			sw.Close();*/
		}
		public void ReloadStage(bool isHighLvl)
        {
            this.isHighLvl = isHighLvl;
            score = 0;

			scenes.Pop();// クリアしたStageもしくは失敗したStageをPop。
			switch (stageNum) {
				case 1:
					Scene.PushScene(new Stage1(scenes.Peek(), isHighLvl));
					break;
				case 2:
					Scene.PushScene(new Stage2(scenes.Peek(), isHighLvl));
					break;
				case 3:
					Scene.PushScene(new Stage3(scenes.Peek(), isHighLvl));
					break;
			}
        }

		// コンストラクタ
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Width;
			graphics.PreferredBackBufferHeight = Height;
			Content.RootDirectory = "Content";

			//winControlManager = new WinControlManager(this, graphics);
			//winControlManager.ControlForm.Visible = false;
			System.Windows.Forms.Form MyForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
			MyForm.MaximizeBox = false;
			MyForm.MinimizeBox = false;

			random = new Random();

			isMuted = true;
			//noEnemy = true;
			scores = dummyScore;
		}
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            CollisionDetection.game = this;
			Animation.game = this;
			EffectControl.game = this;
			DamageControl.game = this;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
			Object.Inicialize(this, spriteBatch, Content);
			SoundControl.Initialize(this, Content);
			Scene.Initialize(this, spriteBatch, Content);
            // TODO: use this.content to load your game content here
			PushScene(new MainTitle(null));
			EvaluateScore("Ranking.txt");
			LoadRanking("Ranking.txt", false, "Ranking_original.txt");

			// Fonts
			Arial = Content.Load<SpriteFont>("General\\Arial32");
			Arial2 = Content.Load<SpriteFont>("General\\Arial10");
			pumpDemi = Content.Load<SpriteFont>("General\\Pump_Demi_Bold");
            dm = new DebugMessage(this, spriteBatch);
            dm.Initialize();

            // Audio
            /*audioEngine = new AudioEngine("Content\\Audio\\Audio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");*/
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            //JoyStick.Update(1);
            JoyStick.Update(1);
			KeyInput.Update();
			dt = gameTime.ElapsedGameTime.TotalSeconds;

			currentScene = scenes.Peek();
			while (currentScene.isEndScene) {
				scenes.Pop();
				if (scenes.Count > 0) {
					currentScene = scenes.Peek();
				}
				else {
					this.Exit();
					break;
				}
			}
			if (scenes.Count > 0) {
				currentScene.Update(gameTime.ElapsedGameTime.TotalSeconds);
			}
			else this.Exit();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
			//spriteBatch.Begin();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
			currentScene.Draw(spriteBatch);
            dm.Draw(gameTime);
			spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
