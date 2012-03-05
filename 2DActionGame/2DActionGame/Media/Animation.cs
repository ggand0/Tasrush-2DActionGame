using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// アニメーションを管理するクラス。使うクラスが手動で(タイプを選んで)扱う.今のところ少し複雑すぎるかもしれない
	/// サク・リバーさんのコードを元に作成
    /// </summary>
    public class Animation
    {
		public static Game1 game;
		
		/// <summary>
		/// テクスチャ（大きな1枚絵）
		/// </summary>
		public Texture2D texture { get; set; }
		/// <summary>
		/// アニメーション用のカウンタ
		/// </summary>
		public int count { get; set; }
		/// <summary>
		/// モーション番号（攻撃、移動などの区分け）
		/// </summary>
        public int motionNum;
		/// <summary>
		/// アニメーションの描画始点･終点
		/// </summary>
        public Rectangle rect = new Rectangle();
		/// <summary>
		/// アニメーションのポーズ設定　animationtypeごとに分けてみる
		/// </summary>
		public int poseCount { get; set; }
		public int jumpPoseCount;
		/// <summary>
		/// モーションのポーズ番号の最大値
		/// </summary>
        internal int poseNum;
		/// <summary>
		/// アニメーションの速度を設定する
		/// </summary>
        internal int animationSpeed = 6;
        private bool displayed;

		/// <summary>
		/// Thunder用:暫定
		/// </summary>
        public bool hasStarted { get; set; }
		/// <summary>
		/// Thunder用:暫定
		/// </summary>
        public Vector2 vector { get; set; }

        public Animation()
        {
            count = 0;
            motionNum = 0;
            poseCount = 0;
            jumpPoseCount=0;

            rect.Width = 32;
            rect.Height = 32;
        }
        public Animation(int rectWidth, int rectHeight)
        {   
            count = 0;
            motionNum = 0;
            poseCount = 0;
            jumpPoseCount = 0;
            
            rect.Width = rectWidth;
            rect.Height = rectHeight;
        }

        #region Updates
        /// <summary>
        /// 使う側が引数で指定する仕様
        /// motionNumやrectはそのような指定方法でいいと思うがアニメーションのさせ方を全キャラ分ここに書くのはどうかと思う
        /// もっと自由に扱えるクラスを作って、複雑なアニメーションをするクラスはそちらを使うという感じにするべきだろうか？
        /// </summary>
        /// <param name="poseNum">コマ数</param>
        /// <param name="motionNum">モーション数</param>
        /// <param name="rectWidth">1コマ当たりの幅</param>
		/// <param name="rectHeight">1コマ当たりの高さ</param>
        /// <param name="animationSpeed">アニメーション速度（数値が小さい程速い）</param>
        /// <param name="animationType">アニメーションのさせ方</param>
        public void Update(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed, int animationType)
        {
            Inicialize(poseNum, motionNum, rectWidth, rectHeight, animationSpeed);
            DrawRect(animationType);
            AnimationCount(animationType);
        }
        /// <summary>
		/// 静止画専用メソッド：領域直接指定
        /// </summary>
        public void Update(int poseCount, int motionNum, int rectWidth, int rectHeight)
        {
            this.poseCount = poseCount;
            this.motionNum = motionNum;
            DrawRect(0);
            //AnimationCount(0);
        }
        /// <summary>
		/// 部分ループ用に特化
        /// </summary>
        public void Update(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed, int animationType, int startPoseNum, int endPoseNum)
        {
            Inicialize(poseNum, motionNum, rectWidth, rectHeight, animationSpeed);
            DrawRect(animationType);
            AlternativeAnimation(startPoseNum, endPoseNum);
        }
        /// <summary>
		/// フラグで管理型
        /// </summary>
        public void Update(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed, int animationType, bool timing)
        {
            Inicialize(poseNum, motionNum, rectWidth, rectHeight, animationSpeed);
            DrawRect(animationType);
            AnimationCount(animationType,timing);
        }

        public void Inicialize(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed)
        {
            this.poseNum = poseNum;
            this.motionNum = motionNum;
            this.rect.Width = rectWidth;
            this.rect.Height = rectHeight;
            this.animationSpeed = animationSpeed;
        }
        /// <summary>
		/// 描画範囲を定義
        /// </summary>
        private void DrawRect(int animationType)
        {
            rect.X = poseCount * rect.Width;
            if (animationType == 2) rect.X = jumpPoseCount * rect.Width;// jumpのときは分けた

            rect.Y = motionNum * rect.Height;          
        }
        internal virtual void AnimationCount(int animationType)
        {
            //poseCount = 0;// 初期化

			switch (animationType) {
				case 0:
					StopAnimation();
					break;
				case 1:
					LoopAnimation();
					break;
				case 2:
					JumpAnimation();
					break;
				case 5:
					EffectAnimation();
					break;
			}
        }
        internal virtual void AnimationCount(int animationType, bool timing)
        {
            //poseCount = 0;// 初期化
			switch (animationType) {
				case 0:
					StopAnimation();
					break;
				case 1:
					LoopAnimation();
					break;
				case 2:
					JumpAnimation();
					break;
				case 3:
					ShootAnimation(timing);
					break;
				case 4:
					JumpAnimation2(timing);
					break;
				case 5:
					EffectAnimation();
					break;
			}
        }
        #endregion
        #region AnimationTypes
		/// <summary>
		/// // animationType0
		/// </summary>
        private void StopAnimation()
        {
            poseCount = poseNum;
        }
		/// <summary>
		/// animationType1
		/// </summary>
        private void LoopAnimation()
        {   // ただループさせるタイプ
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= poseNum) poseCount = 0;// ポーズの最後までアニメーションしたら初期化
            }
        }
		/// <summary>
		/// animationType1 : 指定区間をループさせるタイプ(4から1まで」などは想定してない)
		/// </summary>
		/// <param name="startPoseNum"></param>
		/// <param name="endPoseNum"></param>
        private void LoopAnimation(int startPoseNum, int endPoseNum)
        {
            /*if(counter ==0) poseCount = startPoseNum;// もう少しスマートにしたい
            counter++;*/
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= endPoseNum) 
                    poseCount = startPoseNum;
            }
        }
		/// <summary>
		/// animationType1 未完成
		/// </summary>
		/// <param name="startPoseNum"></param>
		/// <param name="endPoseNum"></param>
        private void AlternativeAnimation(int startPoseNum, int endPoseNum)
        {   // 交互に表示させるタイプ
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0 && !displayed) { poseCount = endPoseNum; displayed = true; }
                else if (count % animationSpeed == 0 && displayed) { poseCount = startPoseNum; displayed = false; }
            }
        }
		/// <summary>
		/// animationType2
		/// </summary>
        private void JumpAnimation()
        {
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) jumpPoseCount++;
                if (jumpPoseCount >= poseNum-1) jumpPoseCount = poseNum-1;// ポーズの最後までアニメーションしてもそのまま
            }
        }
		/// <summary>
		/// animationType5
		/// </summary>
        private void EffectAnimation()
        {   
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= poseNum) poseCount = poseNum;			// ポーズの最後までアニメーションしてもそのまま
            }
        }
		/// <summary>
		/// animationType4 : ループするか否かをbool値によって管理するタイプ
		/// </summary>
		/// <param name="timing"></param>
        private void JumpAnimation2(bool timing)
        {
            if (animationSpeed > 0) {									//poseCount?
                count++;
                if (count % animationSpeed == 0 && timing) poseCount++;
                if (poseCount >= poseNum && timing)
                    poseCount = poseNum;								// 最後のポーズを持続
                else if (poseCount >= poseNum && !timing)
                    poseCount = 0;										// 着地したら元に戻しておく
            }
        }
		/// <summary>
		/// animationType3 : ループするか否かをbool値によって管理するタイプ
		/// </summary>
		/// <param name="timing"></param>
        private void ShootAnimation(bool timing)
        {
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0 && timing) poseCount++;
                if (poseCount >= poseNum && timing)  
                    poseCount = poseNum-1;
                else if (poseCount >= poseNum-1 && !timing) poseCount = 0;// ポーズの最後までアニメーションしたら初期化
            }
        }
        #endregion
    }
}
