using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using xnainput = Microsoft.Xna.Framework.Input;

namespace _2DActionGame
{
	public enum config
	// jss.buttonMap:{L2:4, L1:6, R2:5, R1:7, crs:2, rct:3, cir:1, tri:0, select:9, start:8, R3:11,L3:10}
	{// XBOX    //index |  PS2		|  PS2_jss	| ELECOM_Pad	|	Action
		A,		//  0   |  1:tri	|  0:tri	|   4:sqr		|	sword_lite
		B,		//  1   |  3:cir	|  1:cir	|   1:tri		|	sword_strong 3
		key3,	//  2   |  2:crs	|  2:crs	|   3:crs		|	jump
		X,		//  3   |  0:sqr	|  3:sqr	|   2:cir		|	(nothing0)
		Y,		//  4   |  7:L2		|  4:L2		|   7:L1		|	dash
		key6,	//  5   |  6:R2		|  5:R2		|   8:R1		|	TAS
		MENU,	//  6   |  4:L1		|  6:L1		|   5:L2		|	(nothing1)
		LOCK,	//  7   |  5:R1		|  7:R1		|   6:R2		|	(nothing2)
		START,	//  8   |  8:start	|  8:start	|				|	PAUSE
		NUM_KEY
	}
	public enum configPS2
	{
		X, A, key3, B, MENU, LOCK, Y, key6, START, NUM_KEY
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
	/// JoyStickクラスのラッパークラス。DirectInput的な実装をJoyStickを使って行う。
	/// </summary>
	public static class JoyStick
	{
		static JoyStick()
		{
			hasaJoyStick = true;
			JoystickState jss = JoystickInput.GetAsJoystickState(0);
			if (!jss.IsConnected) hasaJoyStick = false;
		}
		public static int[] keyMap = new int[(int)config.NUM_KEY];
		public static int[] PS2KeyMap = { 3, 0, 2, 1, 6, 7, 5, 4, 8 };
		/// <summary>
		/// JoyStickクラスのindexを変えないでそのまま使えるように。
		/// </summary>
		public static int[] AdjKeyMap = { 1, 3, 2, 0, 7, 6, 4, 5, 8 };
		/// <summary>
		/// デフォルトではPS2コンのマッピング
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static int KeyMap(int index)
		{
			return keyMap[index];// そのまま返すだけ
			//return AdjKeyMap[index];
		}

		public static void Update(double dt)
		{
			if (hasaJoyStick) {
				JoystickUpdate(0, dt);//TODO:複数のジョイパッドに対応するならここをなんとかしないと。
			} else {
				//KeyboardUpdate(dt);
			}
		}

		static readonly float PRESSED_THRESHOLD = 0.1f;//100;
		private static xnainput.KeyboardState oldKeyboardState;
		private static xnainput.Keys[] KeyboardConfig;

		/// <summary>
		/// 方向キーもしくは方向パッド
		/// </summary>
		private static Vector2 V;
		public static Vector2 Vector { get { return V; } }
		/// <summary>
		/// 上下左右ニュートラル、の順に0～4の数値が返される。
		/// </summary>
		public static Direction stickDirection { get; private set; }
		/// <summary>
		/// その方向を押し続けている時間
		/// </summary>
		public static double stickTime { get; private set; }
		/// <summary>
		/// 方向が変わった瞬間か
		/// </summary>
		public static bool onStickDirectionChanged { get; private set; }

		/// <summary>
		/// キーが押されているか
		/// </summary>
		static bool[] Key = new bool[(int)config.NUM_KEY];
		public static bool KEY(int index) { return Key[KeyMap(index)]; }

		/// <summary>
		/// キーが押された瞬間か
		/// </summary>
		static bool[] onKeyDown = new bool[(int)config.NUM_KEY];
		public static bool IsOnKeyDown(int index) { return onKeyDown[KeyMap(index)]; }

		/// <summary>
		/// キーが離された瞬間か
		/// </summary>
		static bool[] onKeyUp = new bool[(int)config.NUM_KEY];
		public static bool IsOnKeyUp(int index) { return onKeyUp[KeyMap(index)]; }

		/// <summary>
		/// キーを押している時間
		/// </summary>
		static double[] keyTime = new double[(int)config.NUM_KEY];
		public static double KeyTime(int index) { return keyTime[KeyMap(index)]; }

		public static List<int> curButtons = new List<int>();
		/// <summary>
		/// ジョイスティックコントローラがあるか
		/// </summary>
		public static bool hasaJoyStick { get; private set; }

		static void JoystickUpdate(int deviceID, double dt)
		{
			JoystickState jss = JoystickInput.GetAsJoystickState(deviceID);
			curButtons.Clear();

			for (int i = 0; i < (int)config.NUM_KEY; i++) {
				if (!Key[i] && jss.buttonMap[i]) {//今押された
					curButtons.Add((byte)i);
					onKeyDown[i] = true;
					onKeyUp[i] = false;
					continue;//今押されたって事は、今離されたかどうかなんて聞くまでも無いだろって事で。
				} else {
					onKeyDown[i] = false;
				}
				if (Key[i] && !jss.buttonMap[i]) {//今離された
					onKeyUp[i] = true;
					keyTime[i] = 0.0;
				} else {
					onKeyUp[i] = false;
				}
			}
			for (int i = 0; i < (int)config.NUM_KEY; i++) {
				if (jss.buttonMap[i]) {//古い状態を捨てて新しい状態に切り替える
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
			float axisx = jss.PositionX;//jsStates[deviceID].X;
			float axisy = jss.PositionY;//jsStates[deviceID].Y;
			if (jss.PositionX == 0 && jss.PositionY == 0) {
				if (jss.DPad.Right == xnainput.ButtonState.Pressed) axisx = 1.0f;
				else if (jss.DPad.Left == xnainput.ButtonState.Pressed) axisx = -1.0f;
				if (jss.DPad.Down == xnainput.ButtonState.Pressed) axisy = -1.0f;
				else if (jss.DPad.Up == xnainput.ButtonState.Pressed) axisy = 1.0f;
			}
			Vector2 oldV = V;
			V.X = ((Math.Abs(axisx) < PRESSED_THRESHOLD) ? 0.0f : axisx);
			V.Y = ((Math.Abs(axisy) < PRESSED_THRESHOLD) ? 0.0f : axisy);
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
