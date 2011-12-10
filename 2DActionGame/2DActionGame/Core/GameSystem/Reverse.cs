using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _2DActionGame
{
	[Author("faintmocha", Affiliation = "Kawaz")]// 勝手に付けてみた。（属性のテスト）
    public class Reverse
    {
        private class LogData : ICloneable
        {
            public Vector2 playerVector;
            public Vector2 cameraVector;
            public TerrainData[] FallingBlocks;
            public TerrainData nowTerrainData;
            public double playervx;
            public double playervy;
            public int playerHP;        
            public int fallingBlocksRange;
            
            public LogData()
            {
                nowTerrainData = new TerrainData();
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

        }

        //地形の情報のうちReverseに必要な分だけ。
        private struct TerrainData
        {
            public Vector2 vector;
            public Vector2 speed;
            public int counter;
            public bool isUnderSomeone;
        }
        
        #region num
        public int ReversePower { get { return reverseLog.Count; } }
        public bool isReversed { get; set; }
        public bool isFinishing { get; set; }
        public bool isAutoReversed { get; set; }

        private LogData nowlogdata;
        private List<Terrain> fallingBlocksList;
        private List<LogData> reverseLog;

        private KeyboardState keystate_now;
        private KeyboardState keystate_prev;

        public Game1 game;
        public Stage stage;

        private int frameNumber;
        private int updateCount = 1;
        private double score_prev;
        
        private readonly int frameRange = 120;
        private readonly int updateSpeed = 2;// "1 / updateSpeed"倍速で逆再生する
        #endregion

        public Reverse(Stage stage, Game1 game)
        {
            this.game = game;
            this.stage = stage;
            nowlogdata = new LogData();
            reverseLog = new List<LogData>();
            score_prev = 0;
        }

        private void RegenerateTAS()
        {
            if (game.score > score_prev) {
                //(game.score - score_prev) * (stage.player.MAXTAS / 50) が基本的なTASの回復量。

                if (stage.player.TASpower + (int)(game.score - score_prev) * (stage.player.MAXTAS / 50) < stage.player.MAXTAS) {
                    stage.player.TASpower = stage.player.TASpower + (int)(game.score - score_prev) * 12;
                } else {
                    stage.player.TASpower = stage.player.MAXTAS;
                }
            }
            score_prev = game.score;
        }
        public void UpdateLog()
        {
            RegenerateTAS();
            if (!isReversed) {
                //現在の座標を取得
                nowlogdata.playerVector = stage.player.position;
                nowlogdata.playervx = stage.player.speed.X;
                nowlogdata.playervy = stage.player.speed.Y;
                nowlogdata.cameraVector = stage.camera.position;

                //地形関係。長さも必要じゃね？
                nowlogdata.fallingBlocksRange = stage.dynamicTerrains.Count;
                //座標を書きこみます
                nowlogdata.FallingBlocks = new TerrainData[nowlogdata.fallingBlocksRange];
                for (int i = 0; i < nowlogdata.fallingBlocksRange - 1; ++i) {
                    nowlogdata.FallingBlocks[i].vector = stage.dynamicTerrains[i].position;
                    nowlogdata.FallingBlocks[i].counter = stage.dynamicTerrains[i].counter;
                    nowlogdata.FallingBlocks[i].isUnderSomeone = stage.dynamicTerrains[i].isUnderSomeone;
                    nowlogdata.FallingBlocks[i].speed = stage.dynamicTerrains[i].speed;
                }
                
                //HPも記録するッス
                nowlogdata.playerHP = stage.player.HP;

                //リストに書き込み
                reverseLog.Add(nowlogdata.Clone() as LogData);

                //使わなくなったデータから消してく
                if (reverseLog.Count > frameRange)
                    reverseLog.RemoveAt(0);
            }
        }

        //巻き戻し開始
        public void StartReverse()
        {
            if (reverseLog.Count >= frameRange) {
                isReversed = true;
                frameNumber = frameRange;
            }
        }

        //巻き戻し中のUpdate
        public void Update()
        {
            updateCount += 1;
            stage.player.TASpower += -(stage.player.MAXTAS / 300);
            keystate_now = Keyboard.GetState();
            UpdateKeyPress(keystate_now);

            if (stage.player.TASpower < 0) isReversed = false;
            
            if (isReversed) {
                if (frameNumber > 1) {
                    if (updateCount % updateSpeed == 0) {
                        stage.player.position = reverseLog[frameNumber - 1].playerVector;
                        stage.camera.position = reverseLog[frameNumber - 1].cameraVector;
                        for (int i = 0; i < reverseLog[frameNumber - 1].fallingBlocksRange - 1; ++i) {
                            if (stage.dynamicTerrains[i] is CollapsingBlock) {
                                stage.dynamicTerrains[i].speed.Y = reverseLog[frameNumber - 1].FallingBlocks[i].speed.Y;
                                stage.dynamicTerrains[i].position = reverseLog[frameNumber - 1].FallingBlocks[i].vector;
                                stage.dynamicTerrains[i].counter = reverseLog[frameNumber - 1].FallingBlocks[i].counter;
                                stage.dynamicTerrains[i].isUnderSomeone = reverseLog[frameNumber - 1].FallingBlocks[i].isUnderSomeone;
                            }
                        }
                    } else {
                        //フレーム間補完
                        stage.player.position = (stage.player.position + reverseLog[frameNumber - 1].playerVector) / 2;
                        stage.camera.position = (stage.camera.position + reverseLog[frameNumber - 1].cameraVector) / 2;
                        for (int i = 0; i < reverseLog[frameNumber - 1].fallingBlocksRange - 1; ++i) {
                            if (stage.dynamicTerrains[i] is CollapsingBlock) {
                                stage.dynamicTerrains[i].position = (stage.dynamicTerrains[i].position + reverseLog[frameNumber - 1].FallingBlocks[i].vector) / 2;
                            }
                        }
                    }
                    if (updateCount % updateSpeed == 0) {
                        reverseLog.RemoveAt(frameNumber - 1);
                        frameNumber = frameNumber - 1;
                    }
                } else {
                    isFinishing = true;
                    FinishReverse();
                }
            } else {
                isFinishing = true;
                FinishReverse();
            }
            stage.player.HP = reverseLog[frameNumber - 1].playerHP;
            keystate_prev = keystate_now;
        }

        private void FinishReverse()
        {
            isReversed = false;
            if (isAutoReversed)
                isAutoReversed = false;
            stage.player.speed.X = 0;//or reverseLog[frameNumber - 1].playervx;
            stage.player.speed.Y = 0;//reverseLog[frameNumber - 1].playervy;
            updateCount = 1;
        }

        private void UpdateKeyPress(KeyboardState keystate_now)
        {
            if (!isAutoReversed && (!JoyStick.KEY(5) || JoyStick.IsOnKeyUp(5))) //自動発動でなければ
                isReversed = false;
        }

        public void ScrollUpdate()
        {
            if (updateCount % updateCount == 0) {
                stage.ScrollUpdateReverse(reverseLog[frameNumber - 1].cameraVector.X);
            } else {
                stage.ScrollUpdateReverse((stage.camera.position.X + reverseLog[frameNumber - 1].cameraVector.X) / 2);
            }
        }
    }
}
