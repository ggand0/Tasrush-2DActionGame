using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    public class LoadMapData
    {   // いい加減Stageのメソッドだけでやるのは面倒
        Game game;
        Stage stage;
        public string input_original;
        public string[] input_second;
        public string[,] input_last;

        public LoadMapData(Stage stage)
        {
            
            this.stage = stage;
            input_second = new string[5000];
            input_last = new string[5000, 5];// List化(ry
        }

                // Load
        protected void  LoadMapData(int stageNumber, string FileName, float LocationX)
        {
            /* <実装済>
             * stageNumberに合わせてマップデータ引っ張ってきてロードだ！
             * コンテンツ何たらをつかってstageNumber通りのファイルひらいてー
             * あとはListのなかに順々にぶちこんでいくだけだよねー
             * あ、blockがどういうblockかはクラスによって決まってんのか。
             * データの要素としては①ブロックの種類、②x座標、③y座標、④幅、⑤高さでいいよね
             * ロードするときは①で場合分けする感じかな
             */

            /* <解決済>
             * 今は32×32の単位ブロックを積み上げてマップを作成している状態
             * 全てに対して当たり判定がなされているので、無駄な部分があるのと同時に引っかかりを生じさせている可能性大
             * 左右に入力している状態で32ﾋﾟｸｾﾙ単位でひっかかるので多分そう　改善したい
             * MapEditorで何とかするか、「自分の上下左右に他のブロックがあった場合その方向の当たり判定をしない」処理を入れるかで解決できる?
             * 最初の画面に映っているブロックのみで発生している気がしたのでファイルを調べてみたらブロックの情報がいくつか被ってた　Editorのほうに原因があるらしい
             * と思ったがそうでもなかった　値を手動で入力していた頃のコードで試しても同様の現象がおきるのでたぶん当たり判定の問題
             */

            // inicialize
            //mapDatas.Add(new MapData());
            //MapData mapData = new MapData();

            // テキストファイルの読み込み
            /*try{
                fs = new FileStream(@"sample4.txt", FileMode.Open);
            }
            catch{
                return;}*/
            StreamReader sr = new StreamReader(FileName);//@"Flat_Map.txt");//sample4
            input_original = sr.ReadToEnd(); //とりあえず最後まで文字列に放り込む
            input_second = input_original.Replace("\r\n", "\n").Split('\n'); // "\r\n"を'\n'に置き換えた後(Spritはchar型しか受け取らない)に、まず１行ずつに分割　
            //input2 = input.Split(new char[] { '\n' }); 

            for(int i=0;i<input_second.Length;i++) {
                for(int j=0;j<5;j++) {
                    string[] input_tmp = new string[5];
                    input_tmp =input_second[i].Split( ',' );
                    input_last[i,j] = input_tmp[j];// 次にカンマで分割：一時的な配列に入れた後に二次元配列の列へ代入
                }// テキストファイルの最後に改行コードがあるとエラー
            } 
                                                     
            // 地形や敵のリストへデータを追加
            AddMapData(input_last, LocationX);
            // 横にもう１つ繋げる
            //AddMapData(input_last, );//+32? CheckMapEnd(input_last)
            
            sr.Close();
            //fs.Close();
        }
        protected void  AddMapData(string[,] input, float additionalX)
        {
            for (int i = 0; i < input_second.Length-1;i++) {// 引数がnullだとエラーになるので注意(配列の長さ的な意味で)
                // Objectの種類ごとに指定 (できれば new "読み取った文字列" (constructor...)としたい)
                switch(input[i, 0]) {
                    case ("Block"): 
                        /*terrains.Add(new Block(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]), 
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]),-4));*/
                        stage.staticTerrains.Add(new Block(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]), 0)); // こうじゃなくてTerrain内のBlockを参照するように
                        break;
                    case ("CollapsingBlock"):
                        stage.dynamicTerrains.Add(new CollapsingBlock(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]), 0)); 
                        break;
                    case ("DamageBlock"):
                        stage.staticTerrains.Add(new DamageBlock(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]))); 
                        break;
                    case ("Foothold"):
                        stage.staticTerrains.Add(new Foothold(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]))); 
                        break;
                    case ("StationalEnemy"):
                        stage.characters.Add(new StationalEnemy(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]), 
                            48, 48, 3));
                        break;
                    case ("JumpingEnemy"):
                        stage.characters.Add(new JumpingEnemy(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]), 3));
                        break;
                    case ("FlyingEnemy"):
                        stage.characters.Add(new FlyingEnemy(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]), 3));
                        break;
                    case ("ShootingEnemy"):
                        stage.characters.Add(new ShootingEnemy(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            48, 48, 3));
                        break;
                    case ("FlyingOutEnemy"):
                        stage.characters.Add(new FlyingOutEnemy(stage, float.Parse(input[i, 1]) + additionalX, float.Parse(input[i, 2]),
                            int.Parse(input[i, 3]), int.Parse(input[i, 4]), 3));
                        break;
                    default: break;
                }
            }
            // Terrain内のBlockをblocksに追加
            /*foreach(Terrain terrain in terrains) {
                if(terrain is Block) blocks.Add(terrain as Block);
            }*/

            // objectsに全部放り込む
            stage.objects1.Clear();
            foreach (Terrain terrain in stage.dynamicTerrains) {
                //if(!(terrain is Block))// 壊れないblockは背景バッファに統一
                    stage.objects1.Add(terrain);
            }
            foreach (Terrain terrain in stage.staticTerrains) stage.objects1.Add(terrain);
            foreach (Character character in stage.characters) stage.objects1.Add(character);
            foreach (Bullet bullet in stage.bullets) stage.objects1.Add(bullet);
            stage.objects1.Add(stage.sword);

            // 背景バッファを必要分追加: このブロックは機能しているようだ addtionalXそのまま書いていいか...? ()忘れてた
            for(int i=0; i<((int)CheckMapEnd(input_last) /*+ additionalX*/) / 2048 + 1;i++) {// stage0は45までしかinputastに入ってない xna側でいくつか読んでつなげてるから
                stage.backGrounds.Add(new BackGround(stage,2048,480));
                if (additionalX == 0) stage.backGrounds[i].vector.X += i * 2048;
                else if (additionalX > 0) stage.backGrounds[i].vector.X += additionalX + i * 2048;
            }
            foreach (BackGround bg in stage.backGrounds) stage.objects1.Add(bg);
        }
        protected float CheckMapEnd(string[,] input)
        {   // マップの地形の最大X座標を返す
            float max; int i;
            max = 0;
            for (i = 0; i < input.GetLength(0); i++)// 列要素を比較
                if (input[i, 1] == null) break;
                else if (max < float.Parse(input[i, 1])) max = float.Parse(input[i, 1]);

            return max;
        }
    
    }
}
