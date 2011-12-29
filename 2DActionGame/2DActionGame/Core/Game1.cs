// Welcome to TASRUSH source code!

// ���ӁF�Ӗ��s���ȃR�����g�⎎�s���낵�Ă������Ɏc�����ӏ��A�S���g���Ă��Ȃ��ӏ��A�Ȃǂ�����܂��B
// ����ɁA�ǂ݂Â炢������t�ق�������I�ȏ��������Ă��鏊������ł��傤�B���A�􋖂��͂��̃R�[�h��ǂސl�̎Q�l�ɂȂ�Ǝv���܂��B�������ǂ�ł����ĂˁI
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
		//public int rank;
		public double score;
		public string name;

		public RankingStatus(double score, string name)
		{
			//this.rank = rank;
			this.score = score;
			this.name = name;
		}
	}
	/// <summary>
    /// This is the main movementType for our game. Yukkuri mite ittene!
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
	{
		#region Field
		/// <summary>
        /// �E�B���h�D�T�C�Y
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
		public SpriteFont menuFont { get; private set; }
		public SpriteFont pumpDemi { get; private set; }
        
        // Game
		private const int rankingNodeNum = 5;
		//public RankingStatus[] scores = new RankingStatus[rankingNodeNum];
		public List<RankingStatus> scores = new List<RankingStatus>();
		public RankingStatus[] dummyScore = { new RankingStatus(100, "nanashi")
			, new RankingStatus(100, "nanashi"), new RankingStatus(100, "nanashi"), new RankingStatus(100, "nanashi"), new RankingStatus(100, "nanashi") };

		/// <summary>
		/// ���̃X�e�[�W�̃{�X��܂ł̃X�R�A�B
		/// </summary>
		public double tmpScore { get; set; }
		/// <summary>
		/// �Q�[���S�̂�ʂ����X�R�A
		/// </summary>
        public double score { get; set; }
		/// <summary>
		/// �X�e�[�W���̃X�R�A
		/// </summary>
		public double[] stageScores { get; set; }

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
		#endregion

		private void graphics_DeviceResetting(object sender, EventArgs e)
		{
			// �E�B���h�E�̃t�H�[�J�X��������o�O(?)�΍�
			System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
			form.BringToFront();
		}
		public void PushScene(Scene scene)
		{
			scenes.Push(scene);//this.Window.
		}
		public void ReloadStage(bool isHighLvl)
        {
            this.isHighLvl = isHighLvl;
			if (hasReachedCheckPoint) {
				stageScores[stageNum - 1] = tmpScore;
			} else {
				stageScores[stageNum - 1] = 0;// �X�R�A���Z�b�g
				//score = 0;
			}

			scenes.Pop();// �N���A����Stage�������͎��s����Stage��Pop�B
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
		public void LoadRanking(string fileName, bool isTest, string fileNameT)
		{
			StreamReader sr = new StreamReader(fileNameT);
			string original;
			string[] tmp;
			string[,] tmp2;
			scores.Clear();

			if (isTest) {
				original = sr.ReadToEnd();
				// �Í����̎���
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(original));
				Encryption.EncryptionData(ms, fileName);
			}// �P�s�ǉ����ăe�X�g���Ă݂��Ƃ���Ō�̍s�̓r�����猇����d�l�炵���H

			// ������
			MemoryStream msd = new MemoryStream();
			Encryption.DecryptionFile(fileName, msd);//("test.txt", msd);
			byte[] bs = msd.ToArray();
			original = Encoding.UTF8.GetString(bs);		// UTF8����Ȃ��ƕ�����������
			//original = sr.ReadToEnd();
			original = original.TrimEnd(new char[] { '\r', '\n' });
			if (original == "") return;

			tmp = original.Replace("\r\n", "\n").Split('\n');// 0�����������...
			tmp2 = new string[tmp.Length, 2];
			for (int i = 0; i < tmp.Length; i++) {// �Ō�̍s�͍���邱�Ƃ�z�肵��length-1
				//try {
				string[] t = tmp[i].Split('\t');
				tmp2[i, 0] = t[0];
				tmp2[i, 1] = t[1];
					//tmp2[i, 2] = t[2];
				/*} catch {
					throw new Exception("failed ranking decoding.");// 5�Ԗڂ���"5 na"�ŏI����Ă錏���� buffer[2048]�ɂ�����nanas�܂ł���...?������ϊ֌W�Ȃ�����
					// ��������bs.length�̎��_�Ŗ��炩�ɑ���ĂȂ�
				}*/
			}
			for (int i = 0; i < tmp.Length; i++) {
				/*scores[i].rank = Int32.Parse(tmp2[i, 0]);
				scores[i].score = Double.Parse(tmp2[i, 2]);
				scores[i].name = tmp2[i, 1];*/
				//scores[i] = new RankingStatus(Int32.Parse(tmp2[i, 0]), Double.Parse(tmp2[i, 2]), tmp2[i, 1]);
				scores.Add(new RankingStatus(/*Int32.Parse(tmp2[i, 0]), */Double.Parse(tmp2[i, 1]), tmp2[i, 0]));
			}
		}
		public void EvaluateScore(string fileName)
		{
			int ranking = 999;//rankingNodeNum;
			string output = "";
			//StreamWriter sw = new StreamWriter("Ranking_test.txt");
			// �e�X�e�[�W�̃X�R�A�𑍌v
			foreach (double d in stageScores) {
				score += d;
			}

			// �����L���O�X�V
			int index = scores.FindIndex((x) => x.score < score);
			if (index != -1)
				scores.Insert(index, new RankingStatus(/*index+1, */score, playerName != "" ? playerName : "nanashi"));
			else
				scores.Add(new RankingStatus(score, playerName != "" ? playerName : "nanashi"));

			/*for (int i = 0; i < scores.Count; i++) {
				if (score > scores[i].score) {
					ranking = i + 1;
					break;
				} else {
					continue;
				}
			}

			if (ranking < scores.Count) {
				//scores[ranking-1].score = score;
				//if (playerName != "") {scores[ranking-1].name = playerName;
				//else scores[ranking].name = "nanashi";
				scores[ranking - 1] = new RankingStatus(ranking, score, playerName != "" ?  playerName : "nanashi");
			}*/

			// �Í������t�@�C����������
			for (int i = 0; i < scores.Count; i++) {
				output += /*scores[i].rank.ToString() + " " + */scores[i].name.ToString() + "\t" + scores[i].score + "\r\n";//(i+1).ToString() 
			}
			//output = output.TrimEnd(new char[] { '\r', '\n' });
			output += "dummy 100\r\n";
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(output));
			Encryption.EncryptionData(ms, fileName);
			/*sw.Write(output);
			sw.Close();*/
		}
		

		// �R���X�g���N�^
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

			//isMuted = true;
			//noEnemy = true;
			stageScores = new double[maxStageNum];
			//scores = dummyScore.ToList<RankingStatus>();// ���������s�̂Ƃ��p�{�e�X�g�p�{�v�f���m��
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
			//EvaluateScore("Ranking.txt");
			LoadRanking("Ranking.txt", false, "Ranking.txt");//"Ranking_original.txt");

			// Fonts
			Arial = Content.Load<SpriteFont>("General\\Arial32");
			menuFont = Content.Load<SpriteFont>("General\\Arial14");
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
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.BlendState = BlendState.NonPremultiplied;//Opaque;

            // TODO: Add your drawing code here
			//spriteBatch.Begin();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//SpriteSortMode.BackToFront, BlendState.NonPremultiplied); //Additive
			currentScene.Draw(spriteBatch);
            //dm.Draw(gameTime); // FPS�\��
			spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
