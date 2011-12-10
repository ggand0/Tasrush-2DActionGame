using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class UserInterface
    {
        /*
         * UIの要素を管理するクラス。 
         */
        public Stage stage;
        public Game1 game;
        private string uiDirectry = "UI";
        public Vector2 position { get; set; }
        private UI_TAS tasGauge;
        private UI_HP hpGauge;
        private UI_BossHP bosshpGauge;
        private UIObject uiFront;
        private UIObject uiBack;
        private UIObject uiCenter;
        public List<Enemy> enemyList;
        public List<UI_EnemyHP> enemyHPList;

        public UserInterface(Game1 game, Stage stage)
        {
            this.game = game;
            this.stage = stage;

            position = Vector2.Zero;
            uiFront = new UIObject(stage, position);
            uiCenter = new UIObject(stage, position);
            uiBack = new UIObject(stage, position);
            tasGauge = new UI_TAS(stage, position);
            hpGauge = new UI_HP(stage, position);
            bosshpGauge = new UI_BossHP(stage, position);

            enemyList = new List<Enemy>();
            enemyHPList = new List<UI_EnemyHP>();

            //reverseGauge = new UI_ReverseGauge(stage, new Vector2(500,10));

        }

        public void Initialize()
        {
            enemyList.Clear();
            enemyHPList.Clear();

            foreach (Character character in stage.characters)
                if (character is Enemy)
                    enemyList.Add(character as Enemy);

            foreach (Enemy enemy in enemyList)
                enemyHPList.Add(new UI_EnemyHP(stage, enemy.position, enemy.HP));
        }


        public void Load()
        {
            tasGauge.Load(game.Content, uiDirectry + "\\" + "UI_TAS");
            hpGauge.Load(game.Content, uiDirectry + "\\" + "UI_HP");
            bosshpGauge.Load(game.Content, uiDirectry + "\\Gauge");

            uiFront.Load(game.Content, uiDirectry + "\\" + "UI_Front");
            uiCenter.Load(game.Content, uiDirectry + "\\" + "UI_Center");
            uiBack.Load(game.Content, uiDirectry + "\\" + "UI_Back");
            foreach (UI_EnemyHP uienemyhp in enemyHPList) uienemyhp.Load(game.Content, uiDirectry + "\\" + "Gauge");


            //reverseGauge.Load(game.content, uiDirectry + "\\" + "Gauge");
        }

        public void Update()
        {
            tasGauge.Update();
            hpGauge.Update();
            bosshpGauge.Update();
            for (int i = 0; i < enemyHPList.Count - 1; ++i) {
                enemyHPList[i].Update(i + 1);
            }
            //reverseGauge.Update();
        }

        public void Draw()
        {

            uiBack.Draw(game.spriteBatch);
            tasGauge.Draw(game.spriteBatch);
            uiCenter.Draw(game.spriteBatch);
            hpGauge.Draw(game.spriteBatch);
            uiFront.Draw(game.spriteBatch);
            bosshpGauge.Draw(game.spriteBatch);
            foreach (UI_EnemyHP uiEnemyHP in enemyHPList)
                uiEnemyHP.Draw(game.spriteBatch);


            //reverseGauge.Draw(game.spriteBatch);
        }

    }
}
