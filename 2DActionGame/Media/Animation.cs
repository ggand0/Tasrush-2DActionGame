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
    /// アニメーションを管理するクラス.使うクラスが手動で(タイプを選んで)扱う.今のところ少し複雑すぎるかもしれない
    /// </summary>
    public class Animation
    {
		public static Game1 game;
		
		// リバーさんのコードを参考
		/// <summary>
		/// テクスチャー（大きな1枚絵）
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
        /// <param name="poseNum"></param>ポーズ番号(→方向)
        /// <param name="motionNum"></param>モーション番号(↓方向)
        /// <param name="rectWidth"></param>
        /// <param name="rectHeight"></param>
        /// <param name="animationSpeed"></param>
        /// <param name="animationType"></param>アニメーションのさせ方
        public void Update(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed, int animationType)
        {
            Inicialize(poseNum, motionNum, rectWidth, rectHeight, animationSpeed);
            DrawRect(animationType);
            AnimationCount(animationType);
        }
        // 静止画専用：領域直接指定 11/30 
        public void Update(int poseCount, int motionNum, int rectWidth, int rectHeight)
        {
            this.poseCount = poseCount;
            this.motionNum = motionNum;
            DrawRect(0);
            //AnimationCount(0);
        }
        // 部分ループ用に特化:でも多分失敗
        public void Update(int poseNum, int motionNum, int rectWidth, int rectHeight, int animationSpeed, int animationType, int startPoseNum, int endPoseNum)
        {
            Inicialize(poseNum, motionNum, rectWidth, rectHeight, animationSpeed);
            DrawRect(animationType);
            AlternativeAnimation(startPoseNum, endPoseNum);
        }
        // フラグで管理型
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
        //描画範囲を定義
        private void DrawRect(int animationType)
        {
            rect.X = poseCount * rect.Width; // ここか
            if (animationType == 2) rect.X = jumpPoseCount * rect.Width;// jumpのときは分けた

            rect.Y = motionNum * rect.Height;          
        }
        internal virtual void AnimationCount(int animationType)
        {
            //poseCount = 0;// 初期化

            if (animationType == 0) StopAnimation();
            else if (animationType == 1) LoopAnimation();
            else if (animationType == 2) JumpAnimation();
            else if (animationType == 5) EffectAnimation();
            //else if (animationType == 3) ShootingAnimation(timing);
        }
        internal virtual void AnimationCount(int animationType, bool timing)
        {
            //poseCount = 0;// 初期化

            if (animationType == 0) StopAnimation();
            else if (animationType == 1) LoopAnimation();
            else if (animationType == 2) JumpAnimation();
            else if (animationType == 3) ShootAnimation(timing);
            else if (animationType == 4) JumpAnimation2(timing);
            else if (animationType == 5) EffectAnimation();
        }
        #endregion
        #region AnimationTypes
        private void StopAnimation()// animationType0
        {
            poseCount = poseNum;
        }
        private void LoopAnimation()// animationType1
        {   // ただループさせるタイプ
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= poseNum) poseCount = 0;// ポーズの最後までアニメーションしたら初期化
            }
        }
        private void LoopAnimation(int startPoseNum, int endPoseNum)// animationType1
        {   // 指定区間をループさせるタイプ(4から1まで」などは想定してない)
            /*if(counter ==0) poseCount = startPoseNum;// もう少しスマートにしたい
            counter++;*/
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= endPoseNum) 
                    poseCount = startPoseNum;// こう書くと1フレーム中にposeCountが1に戻っちゃう
            }
        }
        private void AlternativeAnimation(int startPoseNum, int endPoseNum)// animationType1 多分未完成
        {   // 交互に表示させるタイプ
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0 && !displayed) { poseCount = endPoseNum; displayed = true; }
                else if (count % animationSpeed == 0 && displayed) { poseCount = startPoseNum; displayed = false; }
            }
        }

        private void JumpAnimation()// animationType2
        {   // ジャンプのように最後のポーズを保つタイプ：押し続けないと最後までアニメーションされない...
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) jumpPoseCount++;
                if (jumpPoseCount >= poseNum-1) jumpPoseCount = poseNum-1;// ポーズの最後までアニメーションしてもそのまま
            }
        }

        private void EffectAnimation()// animationType5
        {   
            if (animationSpeed > 0) {
                count++;
                if (count % animationSpeed == 0) poseCount++;
                if (poseCount >= poseNum) poseCount = poseNum;// ポーズの最後までアニメーションしてもそのまま
            }
        }
        private void JumpAnimation2(bool timing)// animationType4
        {   // ループするか否かをbool値によって管理するタイプ
            if (animationSpeed > 0) {//poseCount?
                count++;
                if (count % animationSpeed == 0 && timing) poseCount++;
                if (poseCount >= poseNum && timing)
                    poseCount = poseNum;// 最後のポーズを持続
                else if (poseCount >= poseNum && !timing)
                    poseCount = 0;// 着地したら元に戻しておく
            }
        }
        private void ShootAnimation(bool timing)// animationType3
        {   // ループするか否かをbool値によって管理するタイプ
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
