// Welcome to TASLASH source code!

// 注意：意味不明なコメントや試行錯誤していた時に残した箇所、全く使っていない箇所、などがあります。
// さらに、読みづらい部分や稚拙かつ非効率的な処理をしている所もあるでしょう。が、幾許かはこのコードを読む人の参考になると思います。ゆっくり読んでいってね！
using System;
using System.Collections.Generic;
using System.Linq;
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

		//public WinControlManager winControlManager { get; private set; }
		public Random random { get; private set; }
		public double dt { get; private set; }
        public GraphicsDeviceManager graphics { get; set; }
		public SpriteBatch spriteBatch { get; private set; }

        // Scene
		public Stack<Scene> scenes = new Stack<Scene>();
		private Scene currentScene;

		// Font
		public SpriteFont Arial { get; private set; }
		public SpriteFont Arial2 { get; private set; }
		public SpriteFont pumpDemi { get; private set; }

        // Audio
        public AudioEngine audioEngine { get; private set; }
        public WaveBank    waveBank { get; private set; }
        public SoundBank   soundBank { get; set; }
        
        // Game
        public double score { get; set; }
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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            Content.RootDirectory = "Content";

			System.Windows.Forms.Form MyForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
			MyForm.MaximizeBox = false;
			MyForm.MinimizeBox = false; 

			random = new Random();
			//isMuted = true;
			//noEnemy = true;
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

			// Fonts
			Arial = Content.Load<SpriteFont>("General\\Arial32");
			Arial2 = Content.Load<SpriteFont>("General\\Arial10");
			pumpDemi = Content.Load<SpriteFont>("General\\Pump_Demi_Bold");

            // Audio
            /*audioEngine = new AudioEngine("Content\\Audio\\Audio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");*/
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
            Controller.Update(1);
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
			spriteBatch.Begin();
			currentScene.Draw(spriteBatch);
			spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
