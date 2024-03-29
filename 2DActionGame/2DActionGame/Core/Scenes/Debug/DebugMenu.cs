﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	public class DebugMenu : SelectScene
	{
		private Vector2 CONTEXT_POSITION;
		private Stage stage;
		protected string[] contextString;

		public DebugMenu(PauseMenu privousScene, Stage stage)
			: base(privousScene)
		{
			sceneTitle = "Debug Menu";
			this.stage = stage;

			
			menuString = new string[] { 
				"Back",
				//"change on/off",
				"inDebugMode",
				"isMuted",
				"visibleSword",
				//"score",
				"syoryuMode",
				"thrustChargeMode",
				"twoButtonMode",
				//"empty",
			};
			contextString = new string[] { 
				"戻る",
				"デバッグモードをオン／オフにします",
				"ミュートのオン／オフ",
				"ゲーム中、剣の判定矩形を表示します",
				//"ゲーム中、画面上にスコアを表示します",
				"斬り上げ時のモーションを変更します",
				"連続突きを発動するアサインを変更します",
				"攻撃を２ボタンで行うモードをオン／オフします",
				//"空の項目",
			};
            buttonNum = menuString.Length;
			button = new Button[buttonNum];

			for (int i = 0; i < buttonNum; i++) {
				button[i].color = Color.Blue;
				button[i].name = menuString[i];
			}
			Load();
		}

		public override void Load()
		{
			base.Load();
			TEXT_POSITION = new Vector2(Game1.Width / 2,
				Game1.Height / 2 - game.menuFont.MeasureString("A").Y * (buttonNum * 5 / 8));/*3 / 4*/
			CONTEXT_POSITION = new Vector2(0, Game1.Height - game.japaneseFont.MeasureString("あ").Y);
		}
		protected override void UpdateTexts()
		{
			base.UpdateTexts();

			button[1].name = "inDebugMode : " + game.inDebugMode.ToString();
			button[2].name = "isMuted : " + game.isMuted.ToString();
			button[3].name = "visibleSword : " + game.visibleSword.ToString();
			//button[4].name = "score : " + game.visibleScore.ToString();
			button[4].name = "syoryuMode : " + stage.player.syouryuuMode.ToString();
			button[5].name = "thrustChargeMode : " + stage.player.thrustChargeMode.ToString();
			button[6].name = "twoButtonMode : " + game.twoButtonMode.ToString();
		}
		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
			if (button[1].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else game.inDebugMode = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.isMuted) game.isMuted = true;
				else game.isMuted = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[3].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.visibleSword) game.visibleSword = true;
				else game.visibleSword = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			/*if (button[4].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.visibleScore) game.visibleScore = true;
				else game.visibleScore = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}*/

			if (button[4].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!stage.player.syouryuuMode) stage.player.syouryuuMode = true;
				else stage.player.syouryuuMode = false;


				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[5].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!stage.player.thrustChargeMode) stage.player.thrustChargeMode = true;
				else stage.player.thrustChargeMode = false;


				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[6].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.twoButtonMode) game.twoButtonMode = true;
				else game.twoButtonMode = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			foreach (String s in contextString) {
				spriteBatch.DrawString(game.japaneseFont, MessageTable.debugMenuMsgs[curButton], CONTEXT_POSITION, Color.Orange);
			}
		}
	}
}
