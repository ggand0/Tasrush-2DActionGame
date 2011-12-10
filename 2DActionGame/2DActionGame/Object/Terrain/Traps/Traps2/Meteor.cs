using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class Meteor : DamageObject//Terrain
    {
        public Meteor(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height)//, user, localPosition)
        {
			this.user = user;
			this.localPosition = localPosition;

			Load();
        }
		/*public Meteor(Stage stage, float x, float y, int width, int height, int type, int textureType, Object user, Vector2 localPosition)// ,mapObjects
			: base(stage, x, y, width, height, type)
		{
		}*/

		protected override void Load()
		{
			base.Load();
			texture = game.Content.Load<Texture2D>("Object\\Terrain\\Meteor01");
			texture2 = game.Content.Load<Texture2D>("Object\\Terrain\\Meteor02");
		}
        public void Load(ContentManager content, string texture_name, ref Texture2D texture)
        {
            texture = content.Load<Texture2D>(texture_name);
        }

        public override void Update()
        {
            time++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (time % 5 == 0 && !stage.isPausing) d++;
            if (d >= 360) d = 0;
            float dColor = (float)Math.Sin(d * 8) / 2.0f + 0.5f;

			if (isActive && isAlive && IsBeingUsed()) {
				spriteBatch.Draw(texture, drawPos, Color.White);
				spriteBatch.Draw(texture2, drawPos + new Vector2(-5, -5), new Color(255, 255, 255, dColor));//Color.White);
			}  
        }
    }
}
