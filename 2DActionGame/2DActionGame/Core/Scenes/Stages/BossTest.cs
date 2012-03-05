using System;
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
    public class BossTest : Stage
    {
		public BossTest(Scene privousScene, bool isHighLvl)
			: base(privousScene, isHighLvl)
        {
            contentDirectory = "BossTest";
            scrollingBackGround = new ScrollingBackground(Vector2.Zero);
            frontalScrollingBackGround = new ScrollingBackground(new Vector2(0, 260));
            surfaceHeightAtBoss = 482;
            //characters.Add(new StationalEnemy(this, 150, 100, 48, 48, 5));
            //characters.Add(new StationalEnemy(this, 200, 100, 48, 48, 5));
            /*characters.Add(new StationalEnemy(this, 250, 100, 48, 48, 5));// 毎フレームダメージ判定入れる技の調整用
            characters.Add(new JumpingEnemy(this, 300, 100, 32, 32, 5));
            characters.Add(new FlyingEnemy(this, 350, 100, 32, 32, 5));
            characters.Add(new ShootingEnemy(this, 400, 100, 48, 48, 5));// サイズ変更
            characters.Add(new FlyingOutEnemy(this, 4500, 480 - 32, 32, 32, 5));*/

            
            //sword = new Sword(this,player, 200, 100, 64, 8, 0);
            //devided1 = new string[500];
            //devided2 = new string[500, 5];
            //LoadMapData(1, "Flat_Map.txt", 0, 0);//"Flat_Map3840.txt"
            //LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 2 + 32);
            /*LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 3 + 32);
            LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 4 + 32);
            LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 5 + 32);
            LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 6 + 32);
            LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2) * 7 + 32);// このくらいでちょっと重くなってくる(GetDirection切った上体で)
            //LoadMapData(1, "Flat_Map3840.txt", CheckMapEnd(devided2)*2 + 32);*/
            // CheckMapEndを使うとすごく重くなるようだ？　端が重なる値にしたからかもしれないが...
            // Mapを追加すると重いのかもCheckMapEndは全然関係なさそう　1つのファイルから読み込んだ場合は長くても重くならないような...

            //LoadMapData(1, "Flat_Map2.txt", 0);
            //LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(devided2));
            /*LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(devided2) * 2);
            LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(devided2) * 3);
            LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(devided2) * 4);
            LoadMapData(1, "Flat_Map2.txt", CheckMapEnd(devided2) * 5);*/
            
        }
        // LoadMapDataの位置を変えたせいでbossのweaponのbulletなどを手動でobjectsに追加しなければならなくなった。なんと面倒なのか
        public override void Load()
        {
            LoadMapData(1, "BossTest.txt", 0, 0);

            player = new Player(this, 100, 100, 48, 48, 0);
            characters.Add(player);
            this.sword = player.sword;//new Sword(this, player, 200, 100, 64, 8, 0);
            weapons.Add(sword);// 今度はここがライバルの...？？ raibal.csでは2つ入っててusersも正常、しかしここでは何故？　ここに示されるのは過去の情報？
            //boss = new Raijin(this, 500, 50, 210, 210, 50);//210, 210
            //boss = new Rival(this, 600, 300, 60, 60, 50, 0, 1, 1);    // 100だとBackWArd()のときに、defPosを通過しようとして死ぬ
            boss = new Fuujin(this, 500, 100, 210, 210, 50, 0, 5, 1); //380, 310
            characters.Add(boss);
            bossLocation = boss.position + new Vector2(-640, 0);
            //objects.Add(player);
            //objects.Add(sword);
            //objects.Add(boss);
            AddObjects();
            SetTerrainDirection();
            base.Load();

            foreach (Weapon weapon in weapons) {
                if(weapon is Turret)                        weapon.Load(game.Content, "Object\\Turret&Bullet" + "\\" + "windTurret2");
            }
        }

		public override void Update(double dt)
        {
            //isScrolled = true;
            //characters[1].scalarSpeed = camera.scalarSpeed;
            base.Update(dt);
            //characters[1].scalarSpeed = scrollSpeed;
            // speed.Xだとずれていくので画面にくっつけることに
            // しかしshootPositionの関係でやはりspeed.Xを持たせたい

            //characters[1].position.X = camera.position.X +200;
            /*if(camera.position.X > 1000) {
                camera.position.X = 0;
                for(int i=0;i<characters.Count;i++) {
                    characters[i].position.X =0;
                }
            }*/
        }
        /*protected override void ScrollUpdate()
        {
            for(int i=0;i<objects.Count;i++) {// Object全体に対する処理はobjectsでやると便利
                camera.ScrollUpdate(objects[i]);
            }

            //camera.ScrollUpdate(sword); なぜ剣だけ..?
            if (isScrolled) { }// ボス戦は背景だけループさせた方がずっと軽いし楽ではなかろうか
                //camera.position.X += scrollSpeed;//.60f
                //camera.scalarSpeed = scrollSpeed;
        }*/

    }
}
