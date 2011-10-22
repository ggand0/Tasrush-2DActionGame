using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace _2DActionGame
{
    public class Stage0 : Stage
    {
        // デバッグ用の単色矩形のみ,みたいなステージ
		public Stage0(Scene privousScene, bool isHighLvl)
			: base(privousScene, isHighLvl)
        {
            contentDirectory = "Stage00";

            /*characters.Add(new StationalEnemy(this, 300, 100,48, 48, 5));
            characters.Add(new StationalEnemy(this, 450, 100,48, 48, 5));
            characters.Add(new StationalEnemy(this, 600, 100,48, 48, 5));
            characters.Add(new StationalEnemy(this, 670, 100,48, 48, 5));// 毎フレームダメージ判定入れる技の調整用
            characters.Add(new JumpingEnemy  (this, 800, 100, 32, 32,5));
            characters.Add(new FlyingEnemy   (this, 1000, 100, 32, 32, 5));
            characters.Add(new ShootingEnemy (this, 500, 100, 48, 48,5));// サイズ変更
            characters.Add(new FlyingOutEnemy(this, 500, 480-32, 32, 32, 5));*/

            //characters.Add(new RotatingEnemy (this, 200, 50 - 32, 32, 32, 5,new Vector2(16,16)));
            

            //characters.Add(new SkatingEnemy(this, 500, 100, 32, 32, -6, 5));
            //characters.Add(new SkatingEnemy(this, 500, 100, 32, 32, -4, 5));
            //characters.Add(new SkatingEnemy(this, 500, 100, 32, 32, -2, 5));
            
                                                                                       
            //dynamicTerrains.Add(new CollapsingBlock(this, 200, 300, 32, 32, 0));
            //dynamicTerrains.Add(new CollapsingBlock(this, 300, 300, 32, 32, 0));



            //dynamicTerrains.Add(new Thread(this, 200, 100, 8, 150, 0,45,150));

            //dynamicTerrains.Add(new CollapsingBlock(this, 100, 300, 32, 32, 0));
            //dynamicTerrains.Add(new CollapsingBlock(this, 000, 300, 32, 32, 0));
            

            /*staticTerrains.Add(new Slope128(this, 400, 320, 128, 128));
            staticTerrains.Add(new Slope(this, 200, 300, 32, 32));

            staticTerrains.Add(new Slope(this, 32, 250, 32, 32, true, 0));
            staticTerrains.Add(new Slope(this, 64, 282, 32, 32, true, 0));
            staticTerrains.Add(new Slope(this, 96, 250, 64, 32, true, 1));// width&heightの間違いが多くなりそうである
            staticTerrains.Add(new Block(this, 96, 282, 32, 32));

            staticTerrains.Add(new Slope(this, 96, 416, 128, 32, true, 2));
            staticTerrains.Add(new Slope(this, 224, 384, 128, 32, true, 2));
            //dynamicTerrains.Add(new TestBlock(this, 200, 3528, 32, 32));

            staticTerrains.Add(new Slope(this, 320, 288, 32, 32, false, 0));
            staticTerrains.Add(new Slope(this, 352, 320, 128, 32, false, 2));*/

            
            //objects.Add(player);                                                       
           

            //objects.Add(sword);

            //devided1 = new string[500];
            //devided2 = new string[500, 5];
            /*
            LoadMapData(1, "Flat_Map.txt", 0, 0);
            LoadMapData(1, "Flat_Map.txt", 0, -448);
            LoadMapData(1, "Flat_Map.txt", CheckMapEnd(mapDatas[0].devided2), 0);
            LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(mapDatas[0].devided2) * 2, 0);*///　個々の実装を直さなければ...

            //LoadMapData(0, "SlopeTest2.txt", 0, 0);
            //LoadMapData(0, "Stage2Testb.txt", 0, 0);
            //LoadMapData(0, "segmentTest.txt", 0, 0);
            //LoadMapData(0, "SlopeTest3.txt", 0, 0);
            //LoadMapData(0, "Stage2_1.txt", 0, 0);
            
            //SetTerrainDirection();// 実行位置を変更
        }

        public override void Load()
        {
            game.inDebugMode = true;

            LoadMapData(0, "t4.txt", 0, 0);
            SetTerrainDirection();

            player = new Player(this, 100, 100, 48, 48, 0);
            characters.Add(player);
            //objects.Add(player);
            sword = player.sword;
            weapons.Add(sword);
            //objects.Add(sword);
            staticTerrains.Add(new Block(this, 100, 200, 32, 32));
            //objects.Add(staticTerrains[staticTerrains.Count - 1]);


            /*characters.Add(new Boss(this, 3200, 100, 400, 300, 10));
            characters.Add(new FlyingEnemy2(this, 500, 300, 32, 32, 5));
            characters.Add(new SkatingEnemy(this, 500, 100, 32, 32, -8, 5));
            characters.Add(new FlyingOutEnemy2(this, 400, 0, 32, 32, 5));
            staticTerrains.Add(new Block(this, 100, 148, 32, 3));
            foreach(Terrain terrain in dynamicTerrains) 
                objects.Add(terrain);*/

            AddObjects();
            base.Load();
            
            //player.Load(game.content, contentDirectory + "\\" + "Player1");
        }
    }
}
