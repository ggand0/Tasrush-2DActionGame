#region 元コメント
//Geek: http://thorshammer.blog95.fc2.com/blog-entry-107.html　からひっぱってきた。↓のサイト見れない……

// DirectX Inputでコントローラを管理するクラス
// http://wonderrabbitproject.or.tp/diarypro/archives/123.html
// をほぼ参考
#endregion
using System;
using System.Collections.Generic;
using xnainput = Microsoft.Xna.Framework.Input;
using DInput = Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{   
    /// <summary>
	/// アクションボタンとか
    /// </summary>
	public enum config
	{// XBOX    //index |  PS2 | ELECOMのPad
		A ,     //  0   |  1△  |   4(□)
		B ,     //  1   |  3○  |   1(△)
		key3 ,  //  2   |  2×  |   3(×)
		X ,     //  3   |  0□  |   2(○)
		Y ,     //  4   |  7L2  |   7(L1)
		key6 ,  //  5   |  6R2  |   8(R1)
		MENU ,  //  6   |  4L1  |   5(L2)
		LOCK ,  //  7   |  5R1  |   6(R2)
        START , //  8   | start| 
		NUM_KEY 
       
	}
    public enum configPS2
    {
        X,A,key3,B,MENU,LOCK,Y,key6,START,NUM_KEY
    }
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NEWTRAL
    }
	/// <summary>
	/// DirectX Inputでコントローラを管理するクラス
	/// </summary>
	/// <see cref="http://wonderrabbitproject.or.tp/diarypro/archives/123.html"/>
	/// <seealso cref="http://thorshammer.blog95.fc2.com/blog-entry-107.html"/>
	[Author("geekdrums", Affiliation = "Kawaz")]
	public static class Controller
	{
        //private static bottunNUm;// KeyMap用
        static Controller()
		{
            hasaJoystick = true;
			joysticks = new List<DInput.Device>();
			jsStates = new List<DInput.JoystickState>();
			V = new Vector2();
			Initialize();
            #region ジョイスティックが無かった場合キーボードのコンフィグ設定
            if (joysticks.Count == 0)
            {
                hasaJoystick = false;
                oldKeyboardState = xnainput.Keyboard.GetState();         
                KeyboardConfig = new xnainput.Keys[(int)config.NUM_KEY]; 
                KeyboardConfig[(int)config.A] = xnainput.Keys.Z;         // □
                KeyboardConfig[(int)config.B] = xnainput.Keys.X;         // △
                KeyboardConfig[(int)config.key3] = xnainput.Keys.C;      // ○
                KeyboardConfig[(int)config.X] = xnainput.Keys.A;         // ×
                KeyboardConfig[(int)config.Y] = xnainput.Keys.F;   
                KeyboardConfig[(int)config.key6] = xnainput.Keys.V;
                KeyboardConfig[(int)config.MENU] = xnainput.Keys.LeftShift;
                KeyboardConfig[(int)config.LOCK] = xnainput.Keys.D;      //Space;//TODO:コンフィグが俺コントローラ仕様すぎるから変えないと。。
                KeyboardConfig[(int)config.START] = xnainput.Keys.Enter; // 追加
            }
            #endregion
        }

        // config(仮)
        private static int KeyMap(int index) {
            int number=0;
            switch(index) {// 以下はPS2コンの仕様
                case 0: number = 3; break;
                case 1: number = 0; break;
                case 2: number = 2; break;
                case 3: number = 1; break;
                case 4: number = 6; break;
                case 5: number = 7; break;
                case 6: number = 5; break;
                case 7: number = 4; break;
                case 8: number = 8; break;
            }
            return number;
        }

        public static void Update(double dt)
        {
            if (hasaJoystick)
                JoustickUpdate(0,dt);//TODO:複数のジョイパッドに対応するならここをなんとかしないと。
            else KeyboardUpdate(dt);
        }


        //プロパティ=========================================
        private static Vector2 V;//方向キーもしくは方向パッド
        public static Vector2 Vector { get { return V; } }
        public static Direction stickDirection { get; private set; }//上下左右ニュートラル、の順に0～4の数値が返される。
        public static double stickTime { get; private set; }//その方向を押し続けている時間
        public static bool onStickDirectionChanged { get; private set; }//方向が変わった瞬間か

        static bool[] Key = new bool[(int)config.NUM_KEY];//キーが押されているか
        public static bool KEY(int index) { return Key[KeyMap(index)]; }

        static bool[] onKeyDown = new bool[(int)config.NUM_KEY];//キーが押された瞬間か
        public static bool IsOnKeyDown(int index) { return onKeyDown[KeyMap(index)]; }

        static bool[] onKeyUp = new bool[(int)config.NUM_KEY];//キーが離された瞬間か
        public static bool IsOnKeyUp(int index) { return onKeyUp[KeyMap(index)]; }

        static double[] keyTime = new double[(int)config.NUM_KEY];//キーを押している時間
        public static double KeyTime(int index) { return keyTime[KeyMap(index)]; }

        public static bool hasaJoystick { get; private set; }//ジョイスティックコントローラがあるか



        //privateなメンバ======================
        static readonly int PRESSED_THRESHOLD = 100;
        static List<DInput.Device> joysticks;
        static List<DInput.JoystickState> jsStates;
        static xnainput.KeyboardState oldKeyboardState;
        static xnainput.Keys[] KeyboardConfig;

        //privateなメソッド=====================
		/// <summary>
		/// デバイスの初期化
		/// </summary>
        static void Initialize()
		{
			// DirectInputデバイスのリストを取得
			DInput.DeviceList controllers =
				DInput.Manager.GetDevices(
					DInput.DeviceClass.GameControl,
					DInput.EnumDevicesFlags.AllDevices);

			//Joystickのデバイスを作る器
			DInput.Device d;

			//取得したデバイスのリストをforeachして1つづつjoysticksに登録
			foreach (DInput.DeviceInstance i in controllers)
			{
				//デバイスの生成
				d = new DInput.Device(i.InstanceGuid);

				//各種フラグの設定。Backgroundだと第一引数のFormはnullでいい
				d.SetCooperativeLevel(null,
					DInput.CooperativeLevelFlags.NonExclusive
				  | DInput.CooperativeLevelFlags.NoWindowsKey
				  | DInput.CooperativeLevelFlags.Background
				);

				//Joystickタイプのデータフォーマットを設定
				d.SetDataFormat(DInput.DeviceDataFormat.Joystick);

				//アナログスティックなどのAxis要素を持つDeviceObjectInstanceの出力レンジを設定
				foreach (DInput.DeviceObjectInstance oi in d.Objects)
				{
					if ((oi.ObjectId & (int)DInput.DeviceObjectTypeFlags.Axis) != 0)
					{
						d.Properties.SetRange(
							DInput.ParameterHow.ById,
							oi.ObjectId,
							new DInput.InputRange(-1000, 1000));
					}
				}

				//Axisの絶対位置モードを設定
				d.Properties.AxisModeAbsolute = true;

				//とりあえずデバイスを動かす
				try { d.Acquire(); }
				catch (Microsoft.DirectX.DirectXException) { }

				//作ったJoystickのDeviceをJoystickリストに追加
				joysticks.Add(d);

				//Joystickの状態を保持させる為のjsstatesを用意、とりあえず現在と以前の状態で2つ用意してみる
				//Geek:一個でよくね……？
				jsStates.Add(new DInput.JoystickState());
			}
		}
        static void KeyboardUpdate(double dt)
        {
            xnainput.KeyboardState newstate = xnainput.Keyboard.GetState();
            for (int i = 0; i < (int)config.NUM_KEY; i++)
            {
                if (!(oldKeyboardState.IsKeyDown(KeyboardConfig[i]))
                    && newstate.IsKeyDown(KeyboardConfig[i])) {//今押された
                    onKeyDown[i] = true;
                    onKeyUp[i] = false;
                    continue;//今押されたって事は、今離されたかどうかなんて聞くまでも無いだろって事で。
                } else {
                    onKeyDown[i] = false;
                }
                if (oldKeyboardState.IsKeyDown(KeyboardConfig[i])
                    && !(newstate.IsKeyDown(KeyboardConfig[i]))) {//今離された
                    onKeyUp[i] = true;
                    keyTime[i] = 0.0;
                } else {
                    onKeyUp[i] = false;
                }
            }
            for (int i = 0; i < (int)config.NUM_KEY; i++) {
                if (newstate.IsKeyDown(KeyboardConfig[i])) {//古い状態を捨てて新しい状態に切り替え
                    Key[i] = true;
                    keyTime[i] += dt;
                } else {
                    Key[i] = false;
                }
            }
            int horizontal = 0;
            if (newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right)) horizontal++;
            if (newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left)) horizontal--;
            int vertical = 0;
            if (newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up)) vertical++;
            if (newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down)) vertical--;
            V.X = (float)horizontal;
            V.Y = (float)-vertical;//2DでY軸が下向きのためそれの補正
            if (V.LengthSquared() != 0)
                V = Vector2.Normalize(V);
            Direction oldDirection = stickDirection;
            if (V.Y <= -0.7071) {//0.7071==√2
                stickDirection = Direction.UP;//上
            } else if (V.Y >= 0.7071) {
                stickDirection = Direction.DOWN;//下
            } else if (V.X <= -0.7071) {
                stickDirection = Direction.LEFT;//左
            } else if (V.X >= 0.7071) {
                stickDirection = Direction.RIGHT;//右
            } else stickDirection = Direction.NEWTRAL;//ニュートラル
            if (oldDirection == stickDirection) {
                stickTime += dt;
                onStickDirectionChanged = false;
            } else {
                stickTime = 0.0;
                onStickDirectionChanged = true;
            }
            oldKeyboardState = newstate;
        }

        static void JoustickUpdate(int deviceID, double dt)
        {
            //最新情報を取得
            try
            {
                //デバイスから最新情報を掘り出す
                joysticks[deviceID].Poll();
            }
            #region 例外処理
            catch (DInput.NotAcquiredException e1)
            {
                //ここに来た時はAcquireできていないので試してみる
                System.Diagnostics.Trace.WriteLine(e1.ToString());
                //Poll
                try
                {
                    joysticks[deviceID].Acquire();
                    joysticks[deviceID].Poll();
                }
                catch (DInput.InputException e2)
                {
                    System.Diagnostics.Trace.WriteLine(e2.ToString());
                }
            }
            catch (DInput.InputException e1)
            {
                System.Diagnostics.Trace.WriteLine(e1.ToString());
            }
            #endregion

            jsStates[deviceID] = joysticks[deviceID].CurrentJoystickState;//inputLostException"アプリケーションでエラーが発生しました" 多分途中で抜いたせい

            for (int i = 0; i < (int)config.NUM_KEY; i++) {
                if (!Key[i] && jsStates[deviceID].GetButtons()[i] > 0) {//今押された
                    onKeyDown[i] = true;
                    onKeyUp[i] = false;
                    continue;//今押されたって事は、今離されたかどうかなんて聞くまでも無いだろって事で。
                } else {
                    onKeyDown[i] = false;
                }
                if (Key[i] && jsStates[deviceID].GetButtons()[i] == 0) {//今離された
                    onKeyUp[i] = true;
                    keyTime[i] = 0.0;
                } else {
                    onKeyUp[i] = false;
                }
            }
            for (int i = 0; i < (int)config.NUM_KEY; i++) {
                if (jsStates[deviceID].GetButtons()[i] > 0) {//古い状態を捨てて新しい状態に切り替える
                    Key[i] = true;
                    keyTime[i] += dt;
                } else {
                    Key[i] = false;
                }
            }
            // デバイスIDのアナログの十字の状態、未入力で-1、上が0で時計回りに増加)
            //int degree = jsStates[deviceID].GetPointOfView()[0];
            //if ( degree == -1 ) V.set( 0.0f , 0.0f );
            //else	V.set((float)Math.Cos((double)xmath.ToRadians(degree/100)),(float)Math.Sin((double)xmath.ToRadians(degree/100)));
            decimal axisx = jsStates[deviceID].X;
            decimal axisy = jsStates[deviceID].Y;
            Vector2 oldV = V;
            V.X = ((Math.Abs(axisx) < PRESSED_THRESHOLD) ? 0.0f : (float)axisx);
            V.Y =    ((Math.Abs(axisy) < PRESSED_THRESHOLD) ? 0.0f : (float)axisy);
            //普通にセットしちゃってから
            if (V.Length() != 0)
                V.Normalize();
            //加工。TODO:アナログにちゃんと対応するならここを変えないと。

            Direction oldDirection = stickDirection;
            if (V.Y <= -0.7071) {//0.7071==√2
                stickDirection = Direction.UP;//上
            } else if (V.Y >= 0.7071) {
                stickDirection = Direction.DOWN;//下
            } else if (V.X <= -0.7071) {
                stickDirection = Direction.LEFT;//左
            } else if (V.X >= 0.7071) {
                stickDirection = Direction.RIGHT;//右
            } else stickDirection = Direction.NEWTRAL;//ニュートラル
            if (oldDirection == stickDirection) {
                stickTime += dt;
                onStickDirectionChanged = false;
            } else {
                stickTime = 0.0;
                onStickDirectionChanged = true;
            }
        }
	}
}