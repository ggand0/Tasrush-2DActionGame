using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    /// <summary>
    /// 当初Trapとして、障害物全般の基本クラスを予定していたが、変更しPlayer(Character)にDamageを与えるオブジェクトの基本クラスに
    /// することにした.(それでもいまだ必要かどうかは疑問である)DamageBlockを無視して新たに...?とりあえず"触れたらダメージ"にしてtypeで分け、BUlletのようにtextureも指定できるようにすれば
    /// 汎用性は増しそうである.
    /// </summary>
    public class DamageObject : Terrain
    {
        private int textureType;

        public DamageObject(Game1 game, Stage stage, float x, float y, int width, int height)
            : this(game, stage, x, y, width, height, 0)
        {
        }
        public DamageObject(Game1 game, Stage stage, float x, float y, int width, int height, int type)
            : this(game, stage, x, y, width, height, 0, 0)
        {
        }
        public DamageObject(Game1 game, Stage stage, float x, float y, int width, int height, int type, int textureType)
            : this(game, stage, x, y, width, height, 0, 0, null, new Vector2())
        {
        }
        public DamageObject(Game1 game, Stage stage, float x, float y, int width, int height, int type, int textureType, Object user, Vector2 localPosition)
            : base(game, stage, x, y, width, height, type)
        {
            this.user = user;
            this.localPosition = localPosition;
            this.textureType = textureType;
        }

        public override void Load(ContentManager content)
        {
            //base.Load(content, texture_name);
            // Bulletに関してはStageでtexture名を指定してLoadではなく、textureTypeで最初から決めておくのがいいだろう
            switch(textureType){
                case 0 :
                    texture = content.Load<Texture2D>("Turret&Bullet\\");// なんなんだ？
                    break;
                case 1:
                    texture = content.Load<Texture2D>("Turret&BUllet\\Bullet_type1");
                    break;
            }
        }

        /// <summary>
        /// とにかく触れたらDamage.
        /// </summary>
        /// <param name="target_object"></param>
        public override void IsHit(Object target_object)
        {
            base.IsHit(target_object);
            if (target_object.isHit)
                target_object.isDamaged = true;
        }
    }
}
